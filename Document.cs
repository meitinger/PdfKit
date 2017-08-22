﻿/* Copyright (C) 2016-2017, Manuel Meitinger
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
                    doc = new PostScriptDocument(streamReader);
                }
                else if ((supportedType & DocumentType.EncapsulatedPostScript) != 0 && PostScriptDocument.Header.IsMatch(header))
                {
                    doc = new EncapsulatedPostScriptDocument(streamReader);
                }
                else if ((supportedType & DocumentType.Image) != 0)
                {
                    doc = new ImageDocument(path);
                }
                else
                {
                    throw new InvalidDataException(string.Format(Resources.Document_InvalidFileFormat, supportedType));
                }

                // check the page count before returning the document
                if (doc.PageCount < 1)
                {
                    throw new InvalidDataException(string.Format(Resources.Document_PageCountOutOfRange, doc.PageCount));
                }
                doc.FilePath = path;
                return doc;
            }
        }

        private abstract class GhostscriptDocument : Document
        {
            private GhostscriptRenderer _renderer = null;

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                EnsureNoOpenRenderer();
            }

            protected override Image DoRenderPage(int pageNumber, float dpiX, float dpiY, double scaleFactor, int rotate, Action<Image, Rectangle> progressiveUpdate, Func<bool> cancellationCallback)
            {
                // try to render the page
                var image = (Bitmap)null;
                var cancelled = false;
                try
                {
                    // ensure there is a renderer
                    if (_renderer == null)
                    {
                        _renderer = new GhostscriptRenderer();
                        DoRunInitialize(_renderer);
                    }

                    // clear the output buffers
                    _renderer.StdOut.Clear();
                    _renderer.StdErr.Clear();

                    // hook up the poll event
                    var poll = new EventHandler<CancelEventArgs>((o, e) =>
                    {
                        if (cancellationCallback())
                        {
                            e.Cancel = cancelled = true;
                        }
                    });
                    if (cancellationCallback != null)
                    {
                        _renderer.Poll += poll;
                    }
                    try
                    {
                        // hook up the progressive update event
                        var update = new EventHandler<GhostscriptDisplayEventArgs>((o, e) =>
                        {
                            if (!cancelled && image == null)
                            {
                                progressiveUpdate(e.Image, e.UpdateArea);
                            }
                        });
                        if (progressiveUpdate != null)
                        {
                            _renderer.Update += update;
                        }
                        try
                        {
                            // hook up the page event
                            var page = new EventHandler<GhostscriptDisplayEventArgs>((o, e) => image = e.Image);
                            _renderer.Page += page;
                            try
                            {
                                // set the page settings and run the page
                                _renderer.Run(string.Format(CultureInfo.InvariantCulture, "<<\n/HWResolution [{0} {1}]\n/Orientation {2}\n>> setpagedevice\n", dpiX * scaleFactor, dpiY * scaleFactor, (rotate / 90) % 4));
                                DoRunPage(_renderer, pageNumber);
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
                catch (OperationCanceledException)
                {
                    // dispose any image and return null
                    if (image != null)
                    {
                        image.Dispose();
                    }
                    return null;
                }
                catch (GhostscriptException)
                {
                    // dispose the renderer and rethrow
                    EnsureNoOpenRenderer();
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
            public static readonly Regex Header = new Regex(@"^%PDF-\d\.\d$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            private readonly int _pageCount;

            public PortableDocumentFormatDocument(Stream fileStream)
            {
                using (var document = PdfReader.Open(fileStream, PdfDocumentOpenMode.InformationOnly))
                {
                    _pageCount = document.PageCount;
                }
            }

            public override int PageCount
            {
                get { return _pageCount; }
            }

            public override DocumentType Type
            {
                get { return DocumentType.PortableDocumentFormat; }
            }

            protected override void DoConvertToPdf(string path, Action<int> pageCompletedCallback, Func<bool> cancellationCallback)
            {
                // already a pdf
                throw new NotSupportedException();
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
            public static readonly Regex Header = new Regex(@"^%!PS-Adobe-\d\.\d$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            public PostScriptDocument(StreamReader reader)
            {
            }

            public override int PageCount
            {
                get { throw new NotImplementedException(); }
            }

            public override DocumentType Type
            {
                get { return DocumentType.PostScript; }
            }

            protected override void DoConvertToPdf(string path, Action<int> pageCompletedCallback, Func<bool> cancellationCallback)
            {
                throw new NotImplementedException();
            }

            protected override void DoRunInitialize(Ghostscript ghostscript)
            {
                throw new NotImplementedException();
            }

            protected override void DoRunPage(Ghostscript ghostscript, int pageNumber)
            {
                throw new NotImplementedException();
            }
        }

        private class EncapsulatedPostScriptDocument : PostScriptDocument
        {
            public static readonly new Regex Header = new Regex(@"^%!PS-Adobe-\d\.\d\s+EPSF-\d\.\d$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            public EncapsulatedPostScriptDocument(StreamReader reader) : base(reader) { }

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
            }

            private readonly Image _image;
            private readonly int _pageCount;
            private readonly XImageWrapper _wrapper;

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

                // create the wrapper
                _wrapper = new XImageWrapper(_image);
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
                if (_wrapper != null)
                {
                    _wrapper.Dispose();
                }
                if (_image != null)
                {
                    _image.Dispose();
                }
            }

            protected override void DoConvertToPdf(string path, Action<int> pageCompletedCallback, Func<bool> cancellationCallback)
            {
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
                                    return;
                                }
                            }

                            // draw the page in the document
                            var page = pdfDocument.AddPage();
                            page.Width = _wrapper.PointWidth;
                            page.Height = _wrapper.PointHeight;
                            using (var gc = XGraphics.FromPdfPage(page))
                            {
                                gc.DrawImage(_wrapper, 0, 0, _wrapper.PointWidth, _wrapper.PointHeight);
                            }
                            if (cancellationCallback != null && cancellationCallback())
                            {
                                return;
                            }
                            if (pageCompletedCallback != null)
                            {
                                pageCompletedCallback(i + 1);
                            }
                        }

                        // save the document
                        pdfDocument.Save(path);
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

            protected override Image DoRenderPage(int pageNumber, float dpiX, float dpiY, double scaleFactor, int rotate, Action<Image, Rectangle> progressiveUpdate, Func<bool> cancellationCallback)
            {
                lock (_image)
                {
                    // select the page
                    if (PageCount > 1)
                    {
                        _image.SelectActiveFrame(FrameDimension.Page, pageNumber - 1);
                        if (cancellationCallback != null && cancellationCallback())
                        {
                            return null;
                        }
                    }

                    // calculate the new size
                    var size = _image.Size;
                    var doRotate = rotate % 180 != 0;
                    var newSize = new Size
                    (
                        (int)Math.Round(scaleFactor * ((doRotate ? _image.VerticalResolution : _image.HorizontalResolution) / dpiX) * (doRotate ? size.Height : size.Width)),
                        (int)Math.Round(scaleFactor * ((doRotate ? _image.HorizontalResolution : _image.VerticalResolution) / dpiY) * (doRotate ? size.Width : size.Height))
                    );
                    if (cancellationCallback != null && cancellationCallback())
                    {
                        return null;
                    }


                    // create the new image
                    var result = new Bitmap(newSize.Width, newSize.Height);
                    result.SetResolution(dpiX, dpiX);
                    if (cancellationCallback != null && cancellationCallback())
                    {
                        result.Dispose();
                        return null;
                    }

                    // draw the original
                    var cancelled = false;
                    using (var gc = Graphics.FromImage(result))
                    {
                        gc.Clear(Color.Transparent);
                        gc.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gc.RotateTransform(rotate);
                        if (cancellationCallback == null)
                        {
                            gc.DrawImage(_image, new Rectangle(Point.Empty, newSize), 0, 0, size.Width, size.Height, GraphicsUnit.Pixel);
                        }
                        else
                        {
                            gc.DrawImage(_image, new Rectangle(Point.Empty, newSize), 0, 0, size.Width, size.Height, GraphicsUnit.Pixel, null, _ => cancelled = cancellationCallback());
                        }
                    }

                    // handle cancellation
                    if (cancelled)
                    {
                        result.Dispose();
                        return null;
                    }

                    // return the result
                    return result;
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

        public void ConvertToPdf(string path, Action<int> pageCompletedCallback = null, Func<bool> cancellationCallback = null)
        {
            // check the arguments and state
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            CheckDisposed();

            // ensure the directory exists and convert the document
            path = Path.GetFullPath(path);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            DoConvertToPdf(path, pageCompletedCallback, cancellationCallback);
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

        protected abstract void DoConvertToPdf(string path, Action<int> pageCompletedCallback, Func<bool> cancellationCallback);

        protected abstract void DoRunInitialize(Ghostscript ghostscript);

        protected abstract void DoRunPage(Ghostscript ghostscript, int pageNumber);

        protected abstract Image DoRenderPage(int pageNumber, float dpiX, float dpiY, double scaleFactor, int rotate, Action<Image, Rectangle> progressiveUpdate, Func<bool> cancellationCallback);

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

        public Image RenderPage(int pageNumber, float dpiX = 96, float dpiY = 96, double scaleFactor = 1, int rotate = 0, Action<Image, Rectangle> progressiveUpdate = null, Func<bool> cancellationCallback = null)
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
            CheckDisposed();

            // perform the action
            return DoRenderPage(pageNumber, dpiX, dpiY, scaleFactor, rotate, progressiveUpdate, cancellationCallback);
        }
    }
}