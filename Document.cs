/* Copyright (C) 2016-2017, Manuel Meitinger
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Aufbauwerk.Tools.PdfKit.Properties;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Aufbauwerk.Tools.PdfKit
{
    [Flags]
    public enum DocumentType
    {
        PortableDocumentFormat = 1,
        PostScript = 2,
        EncapsulatedPostScript = 4,
        Image = 8,
        Any = PortableDocumentFormat | PostScript | EncapsulatedPostScript | Image,
    }

    public abstract class Document : IDisposable
    {
        public static Document FromFile(string path, DocumentType supportedType = DocumentType.Any)
        {
            // check the argument and get the full path
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            path = Path.GetFullPath(path);

            // open the file
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, false))
            {
                // get the suitable document
                var header = streamReader.ReadLine();
                Document doc;
                if ((supportedType & DocumentType.PortableDocumentFormat) != 0 && PortableDocumentFormatDocument.Header.IsMatch(header))
                {
                    streamReader.DiscardBufferedData();
                    fileStream.Position = 0;
                    doc = new PortableDocumentFormatDocument(fileStream);
                }
                else if ((supportedType & DocumentType.PostScript) != 0 && PostScriptDocument.Header.IsMatch(header))
                {
                    doc = new PostScriptDocument(header, streamReader);
                }
                else if ((supportedType & DocumentType.EncapsulatedPostScript) != 0 && EncapsulatedPostScriptDocument.Header.IsMatch(header))
                {
                    doc = new EncapsulatedPostScriptDocument(header, streamReader);
                }
                else if ((supportedType & DocumentType.Image) != 0)
                {
                    doc = new ImageDocument(path);
                }
                else
                {
                    throw new InvalidDataException(string.Format(Resources.Document_InvalidFileFormat, supportedType));
                }

                // set the path and return the document
                doc.FilePath = path;
                return doc;
            }
        }

        public static Document FromPdf(PdfDocument pdf)
        {
            // check the argument
            if (pdf == null)
            {
                throw new ArgumentNullException("pdf");
            }

            // quickly create a pdf document
            var doc = new PortableDocumentFormatDocument(pdf.PageCount);
            doc.FilePath = pdf.FullPath;
            return doc;
        }

        private abstract class GhostscriptDocument : Document
        {
            private GhostscriptRenderer _renderer = null;

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                EnsureNoOpenRenderer();
            }

            protected override void DoConvert(ConvertFormat format, Action pageCompletedCallback, Func<bool> cancellationCallback)
            {
                // convert the file using Ghostscript
                using (var ghostscript = new Ghostscript(format.GetArguments(FilePath)))
                {
                    if (cancellationCallback != null)
                    {
                        ghostscript.Poll += (s, e) => e.Cancel = cancellationCallback();
                    }
                    if (format == ConvertFormat.Pdf)
                    {
                        ghostscript.Run(".setpdfwrite");
                    }
                    DoRunInitialize(ghostscript);
                    for (var i = 0; i < PageCount; i++)
                    {
                        DoRunPage(ghostscript, i + 1);
                        if (pageCompletedCallback != null)
                        {
                            pageCompletedCallback();
                        }
                    }
                }
            }

            protected override Image DoRenderPage(int pageNumber, float dpiX, float dpiY, double scaleFactor, int rotate, Action<Image> progressiveUpdate, Func<bool> cancellationCallback)
            {
                // try to render the page
                var image = (Bitmap)null;
                var newRenderer = false;
            Retry:
                try
                {
                    // ensure there is a renderer
                    if (_renderer == null)
                    {
                        newRenderer = true;
                        _renderer = new GhostscriptRenderer();
                        DoRunInitialize(_renderer);
                    }

                    // clear the output buffers
                    _renderer.StdOut.Clear();
                    _renderer.StdErr.Clear();

                    // hook up the poll event
                    var poll = new EventHandler<CancelEventArgs>((o, e) => e.Cancel = cancellationCallback());
                    if (cancellationCallback != null)
                    {
                        _renderer.Poll += poll;
                    }
                    try
                    {
                        // hook up the progressive update event
                        var update = new EventHandler<GhostscriptRendererEventArgs>((o, e) => { if (image == null) { progressiveUpdate(e.Image); } });
                        if (progressiveUpdate != null)
                        {
                            _renderer.Update += update;
                        }
                        try
                        {
                            // hook up the page event
                            var page = new EventHandler<GhostscriptRendererEventArgs>((o, e) => image = e.Image);
                            _renderer.Page += page;
                            try
                            {
                                // set the page settings and run the page
                                _renderer.Run(string.Format(CultureInfo.InvariantCulture, "<<\n/HWResolution [{0} {1}]\n/Orientation {2}\n>> setpagedevice\n", dpiX * scaleFactor, dpiY * scaleFactor, (rotate / 90) % 4));
                                DoRunPage(_renderer, pageNumber);

                                // eps files might need a showpage
                                if (image == null && Type == DocumentType.EncapsulatedPostScript)
                                {
                                    _renderer.Run("showpage");
                                }
                            }
                            finally
                            {
                                // remove the page event
                                _renderer.Page -= page;
                            }
                        }
                        finally
                        {
                            // remove the progressive update event
                            if (progressiveUpdate != null)
                            {
                                _renderer.Update -= update;
                            }
                        }
                    }
                    finally
                    {
                        // remove the poll event
                        if (cancellationCallback != null)
                        {
                            _renderer.Poll -= poll;
                        }
                    }
                }
                catch (Exception)
                {
                    // dispose the renderer and image
                    EnsureNoOpenRenderer();
                    if (image != null)
                    {
                        image.Dispose();
                        image = null;
                    }

                    // try again if we used an old renderer (sometimes it fails)
                    if (!newRenderer)
                    {
                        goto Retry;
                    }

                    // rethrow the error
                    throw;
                }

                // ensure there is an image
                if (image == null)
                {
                    throw new InvalidOperationException(string.Format(Resources.Document_PageNotRendered, pageNumber));
                }

                // set the resolution and return the image
                image.SetResolution(dpiX, dpiY);
                return image;
            }

            public override void EnsureNoOpenRenderer()
            {
                // dispose any open renderer
                if (_renderer != null)
                {
                    _renderer.Dispose();
                    _renderer = null;
                }
            }
        }

        private class PortableDocumentFormatDocument : GhostscriptDocument
        {
            public static readonly Regex Header = new Regex(@"^%PDF-\d\.\d(\s|$)", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

            private readonly int _pageCount;

            internal PortableDocumentFormatDocument(Stream fileStream)
            {
                using (var document = PdfReader.Open(fileStream, PdfDocumentOpenMode.InformationOnly))
                {
                    _pageCount = document.PageCount;
                }
            }

            internal PortableDocumentFormatDocument(int pageCount)
            {
                _pageCount = pageCount;
            }

            public override int PageCount
            {
                get { return _pageCount; }
            }

            public override DocumentType Type
            {
                get { return DocumentType.PortableDocumentFormat; }
            }

            protected override void DoRunInitialize(Ghostscript ghostscript)
            {
                // escape the path
                var escapedFileName = new StringBuilder(FilePath);
                for (var i = escapedFileName.Length - 1; i >= 0; i--)
                {
                    switch (escapedFileName[i])
                    {
                        case '(':
                        case ')':
                        case '\\':
                        {
                            escapedFileName.Insert(i, '\\');
                            break;
                        }
                    }
                }

                // load the pdf
                ghostscript.Run(string.Format(CultureInfo.InvariantCulture, "({0}) (r) file runpdfbegin", escapedFileName));
            }

            protected override void DoRunPage(Ghostscript ghostscript, int pageNumber)
            {
                // run the page
                ghostscript.Run(string.Format(CultureInfo.InvariantCulture, "{0} pdfgetpage pdfshowpage", pageNumber));
            }
        }

        private class PostScriptDocument : GhostscriptDocument
        {
            public static readonly Regex Header = new Regex(@"^%!PS-Adobe-\d\.\d(\s+(?!EPSF-)|$)", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);
            private static readonly char[] Separator = new char[] { ' ', '\t' };

            private readonly SortedList<int, byte[]> _pages = new SortedList<int, byte[]>();
            private readonly byte[] _prologAndSetup;

            internal PostScriptDocument(string header, StreamReader reader)
            {
                // create a memory writer
                using (var memory = new MemoryStream())
                using (var writer = new StreamWriter(memory, new UTF8Encoding(false)))
                {
                    writer.NewLine = "\n";
                    writer.WriteLine(header);
                    var page = 0;

                    // read all lines
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // check for comments
                        if (line.StartsWith("%%", StringComparison.Ordinal))
                        {
                            var comment = line.Substring(2).TrimEnd();
                            if (comment == "Trailer")
                            {
                                // if there are no pages, read the rest as well
                                if (page == 0)
                                {
                                    do
                                    {
                                        writer.WriteLine(line);
                                    }
                                    while ((line = reader.ReadLine()) != null);
                                }

                                // quit reading
                                break;
                            }
                            else if (comment.StartsWith("Page:", StringComparison.Ordinal))
                            {
                                // store the previous page or prolog
                                writer.Flush();
                                if (page > 0)
                                {
                                    _pages[page] = memory.ToArray();
                                }
                                else
                                {
                                    _prologAndSetup = memory.ToArray();
                                }
                                memory.SetLength(0);

                                // get the next page number
                                var sepIndex = comment.LastIndexOfAny(Separator);
                                var pageNumber = comment.Substring(Math.Max(sepIndex + 1, 5));
                                if (!int.TryParse(pageNumber, NumberStyles.Integer, CultureInfo.InvariantCulture, out page) || page < 1)
                                {
                                    throw new InvalidDataException(string.Format(Resources.Document_InvalidPageNumber, pageNumber));
                                }
                            }
                        }

                        // write the line
                        writer.WriteLine(line);
                    }

                    // store the last page or the entire document
                    writer.Flush();
                    _pages[Math.Max(1, page)] = memory.ToArray();
                    memory.SetLength(0);
                }
            }

            public override int PageCount
            {
                get { return _pages.Count; }
            }

            public override DocumentType Type
            {
                get { return DocumentType.PostScript; }
            }

            protected override void DoRunInitialize(Ghostscript ghostscript)
            {
                // run the prolog if there is any
                if (_prologAndSetup != null)
                {
                    ghostscript.Run(_prologAndSetup);
                }
            }

            protected override void DoRunPage(Ghostscript ghostscript, int pageNumber)
            {
                // run the given page
                ghostscript.Run(_pages.Values[pageNumber - 1]);
            }
        }

        private class EncapsulatedPostScriptDocument : PostScriptDocument
        {
            public static readonly new Regex Header = new Regex(@"^%!PS-Adobe-\d\.\d\s+EPSF-\d\.\d(\s|$)", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

            internal EncapsulatedPostScriptDocument(string header, StreamReader reader) : base(header, reader) { }

            public override DocumentType Type
            {
                get { return DocumentType.EncapsulatedPostScript; }
            }
        }

        private class ImageDocument : Document
        {
            private class XImageWrapper : XImage
            {
                private static readonly MethodInfo _initialize;
                private static readonly FieldInfo _gdiImage;

                static XImageWrapper()
                {
                    // get the necessary members
                    var type = typeof(XImage);
                    const string initializeName = "Initialize";
                    _initialize = type.GetMethod(initializeName, BindingFlags.Instance | BindingFlags.NonPublic, null, System.Type.EmptyTypes, null);
                    if (_initialize == null)
                    {
                        throw new MissingMemberException(type.FullName, initializeName);
                    }
                    const string gdiImageName = "_gdiImage";
                    _gdiImage = type.GetField(gdiImageName, BindingFlags.Instance | BindingFlags.NonPublic);
                    if (_gdiImage == null)
                    {
                        throw new MissingMemberException(type.FullName, gdiImageName);
                    }
                }

                public XImageWrapper(Image image)
                {
                    // set the image and initialize the object
                    _gdiImage.SetValue(this, image);
                    try
                    {
                        _initialize.Invoke(this, null);
                    }
                    catch (TargetInvocationException e)
                    {
                        throw e;
                    }
                }

                protected override void Dispose(bool disposing)
                {
                    // clear the image before disposing
                    _gdiImage.SetValue(this, null);
                    base.Dispose(disposing);
                }
            }

            private readonly Image _image;
            private readonly int _pageCount;

            public ImageDocument(string path)
            {
                // load the image
                _image = Image.FromFile(path);

                // set the page count
                try
                {
                    _pageCount = _image.GetFrameCount(FrameDimension.Page);
                }
                catch
                {
                    _pageCount = 1;
                }
            }

            public override int PageCount
            {
                get { return _pageCount; }
            }

            public override DocumentType Type
            {
                get { return DocumentType.Image; }
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (_image != null)
                {
                    lock (_image)
                    {
                        _image.Dispose();
                    }
                }
            }

            protected override void DoConvert(ConvertFormat format, Action pageCompletedCallback, Func<bool> cancellationCallback)
            {
                // only support pdf conversion
                if (format != ConvertFormat.Pdf || !format.UseSingleFile)
                {
                    throw new NotSupportedException();
                }

                // convert all pages into a pdf
                lock (_image)
                {
                    using (var pdfDocument = new PdfDocument())
                    {
                        for (var i = 0; i < PageCount; i++)
                        {
                            // select the page from the image
                            if (PageCount > 1)
                            {
                                _image.SelectActiveFrame(FrameDimension.Page, i);
                                if (cancellationCallback != null && cancellationCallback())
                                {
                                    throw new OperationCanceledException();
                                }
                            }

                            // draw the page into the document
                            var page = pdfDocument.AddPage();
                            using (var imageWrapper = new XImageWrapper(_image))
                            {
                                page.Width = imageWrapper.PointWidth;
                                page.Height = imageWrapper.PointHeight;
                                using (var gc = XGraphics.FromPdfPage(page))
                                {
                                    gc.DrawImage(imageWrapper, 0, 0, imageWrapper.PointWidth, imageWrapper.PointHeight);
                                }
                            }

                            // check for cancellation and notify the caller
                            if (cancellationCallback != null && cancellationCallback())
                            {
                                throw new OperationCanceledException();
                            }
                            if (pageCompletedCallback != null)
                            {
                                pageCompletedCallback();
                            }
                        }

                        // save the document
                        pdfDocument.Save(Path.ChangeExtension(FilePath, format.FileExtension));
                    }
                }
            }

            protected override void DoRunInitialize(Ghostscript ghostscript)
            {
                // not a Ghostscript file
                throw new NotSupportedException();
            }

            protected override void DoRunPage(Ghostscript ghostscript, int pageNumber)
            {
                // not a Ghostscript file
                throw new NotSupportedException();
            }

            protected override Image DoRenderPage(int pageNumber, float dpiX, float dpiY, double scaleFactor, int rotate, Action<Image> progressiveUpdate, Func<bool> cancellationCallback)
            {
                lock (_image)
                {
                    // select the page
                    if (PageCount > 1)
                    {
                        _image.SelectActiveFrame(FrameDimension.Page, pageNumber - 1);
                        if (cancellationCallback != null && cancellationCallback())
                        {
                            throw new OperationCanceledException();
                        }
                    }

                    // calculate the new size and create the image
                    var size = _image.Size;
                    var newSize = new Size((int)Math.Round(scaleFactor * (dpiX / _image.HorizontalResolution) * size.Width), (int)Math.Round(scaleFactor * (dpiY / _image.VerticalResolution) * size.Height));
                    var result = new Bitmap(newSize.Width, newSize.Height);
                    try
                    {
                        // set the resolution
                        result.SetResolution(dpiX, dpiX);
                        if (cancellationCallback != null && cancellationCallback())
                        {
                            throw new OperationCanceledException();
                        }

                        // draw the original
                        using (var gc = Graphics.FromImage(result))
                        {
                            gc.Clear(Color.Transparent);
                            gc.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            if (cancellationCallback == null)
                            {
                                gc.DrawImage(_image, new Rectangle(Point.Empty, newSize), 0, 0, size.Width, size.Height, GraphicsUnit.Pixel);
                            }
                            else
                            {
                                var cancelled = false;
                                gc.DrawImage(_image, new Rectangle(Point.Empty, newSize), 0, 0, size.Width, size.Height, GraphicsUnit.Pixel, null, _ => cancelled || (cancelled = cancellationCallback()));
                                if (cancelled)
                                {
                                    throw new OperationCanceledException();
                                }
                            }
                        }

                        // rotate the image
                        result.RotateFlip((RotateFlipType)((4 - (rotate / 90)) % 4));
                        if (cancellationCallback != null && cancellationCallback())
                        {
                            throw new OperationCanceledException();
                        }

                        // return the image
                        return result;
                    }
                    catch
                    {
                        // dispose the image and rethrow
                        result.Dispose();
                        throw;
                    }
                }
            }

            public override void EnsureNoOpenRenderer()
            {
                // not applicable
            }
        }

        private bool _disposed = false;

        ~Document()
        {
            Dispose(false);
        }

        public string FilePath { get; private set; }

        public abstract int PageCount { get; }

        public abstract DocumentType Type { get; }

        private void CheckDisposed()
        {
            // throw an error if the object is disposed
            if (_disposed)
            {
                throw new ObjectDisposedException(string.Format(CultureInfo.InvariantCulture, "{0}: {1}", GetType(), FilePath));
            }
        }

        public void Convert(ConvertFormat format, Action pageCompletedCallback = null, Func<bool> cancellationCallback = null)
        {
            // check the arguments and state
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            CheckDisposed();

            // convert the document
            DoConvert(format, pageCompletedCallback, cancellationCallback);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }

        protected abstract void DoConvert(ConvertFormat format, Action pageCompletedCallback, Func<bool> cancellationCallback);

        protected abstract void DoRunInitialize(Ghostscript ghostscript);

        protected abstract void DoRunPage(Ghostscript ghostscript, int pageNumber);

        protected abstract Image DoRenderPage(int pageNumber, float dpiX, float dpiY, double scaleFactor, int rotate, Action<Image> progressiveUpdate, Func<bool> cancellationCallback);

        public abstract void EnsureNoOpenRenderer();

        public void RunInitialize(Ghostscript ghostscript)
        {
            // check the arguments and state
            if (ghostscript == null)
            {
                throw new ArgumentNullException("ghostscript");
            }
            CheckDisposed();

            // perform the action
            DoRunInitialize(ghostscript);
        }

        public void RunPage(Ghostscript ghostscript, int pageNumber, string setPageDevice = null)
        {
            // check the arguments and state
            if (ghostscript == null)
            {
                throw new ArgumentNullException("ghostscript");
            }
            if (pageNumber < 1 || pageNumber > PageCount)
            {
                throw new ArgumentOutOfRangeException("pageNumber");
            }
            CheckDisposed();

            // perform the action
            DoRunPage(ghostscript, pageNumber);
        }

        public Image RenderPage(int pageNumber, float dpiX = 96, float dpiY = 96, double scaleFactor = 1, int rotate = 0, Action<Image> progressiveUpdate = null, Func<bool> cancellationCallback = null)
        {
            // check the arguments and state
            if (pageNumber < 1 || pageNumber > PageCount)
            {
                throw new ArgumentOutOfRangeException("pageNumber");
            }
            if (dpiX <= 0)
            {
                throw new ArgumentOutOfRangeException("dpiX");
            }
            if (dpiY <= 0)
            {
                throw new ArgumentOutOfRangeException("dpiY");
            }
            if (scaleFactor <= 0)
            {
                throw new ArgumentOutOfRangeException("scaleFactor");
            }
            if (rotate % 90 != 0)
            {
                throw new ArgumentOutOfRangeException("rotate");
            }
            rotate %= 360;
            if (rotate < 0)
            {
                rotate += 360;
            }
            CheckDisposed();

            // perform the action
            return DoRenderPage(pageNumber, dpiX, dpiY, scaleFactor, rotate, progressiveUpdate, cancellationCallback);
        }
    }
}
