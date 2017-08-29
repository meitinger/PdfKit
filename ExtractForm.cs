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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Aufbauwerk.Tools.PdfKit
{
    public partial class ExtractForm : Form
    {
        private const string ErrorImageName = "error";
        private const string ImageNameFormat = "image{0}";
        private const string LoadingImageName = "loading";

        private class FilesDataObject : IDataObject, IDisposable
        {
            private bool _disposed = false;
            private readonly Func<Work, string[]> _extractDelegate;
            private string[] _filePaths = null;
            private readonly object _filePathsLock = new object();
            private DragDropEffects _result = DragDropEffects.None;
            private readonly Work _work;

            public FilesDataObject(Func<Work, string[]> extractDelegate, Work work)
            {
                // store the parameters
                _extractDelegate = extractDelegate;
                _work = work;
            }

            ~FilesDataObject()
            {
                Dispose(false);
            }

            public DragDropEffects Result
            {
                get { return _result; }
                set
                {
                    // ensure the object is not disposed and set the result
                    CheckDisposed();
                    _result = value;
                }
            }

            private void CheckDisposed()
            {
                // throw an error if the object is disposed
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().ToString());
                }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                // dispose the drop object if not done so already
                if (!_disposed)
                {
                    _disposed = true;

                    // cleanup extracted documents if they weren't moved
                    if (_filePaths != null && _result != DragDropEffects.Move)
                    {
                        for (var i = 0; i < _filePaths.Length; i++)
                        {
                            try { File.Delete(_filePaths[i]); }
                            catch { }
                        }
                    }
                }
            }

            private string[] Extract()
            {
                // extract the documents if not done so already
                CheckDisposed();
                lock (_filePathsLock)
                {
                    if (_filePaths == null)
                    {
                        _filePaths = _extractDelegate(_work);
                    }
                    return _filePaths;
                }
            }

            public object GetData(Type format)
            {
                return format == typeof(string[]) ? Extract() : null;
            }

            public object GetData(string format)
            {
                return GetData(format, true);
            }

            public object GetData(string format, bool autoConvert)
            {
                return format == DataFormats.FileDrop ? Extract() : null;
            }

            public bool GetDataPresent(Type format)
            {
                return format == typeof(string[]);
            }

            public bool GetDataPresent(string format)
            {
                return GetDataPresent(format, true);
            }

            public bool GetDataPresent(string format, bool autoConvert)
            {
                return format == DataFormats.FileDrop;
            }

            public string[] GetFormats()
            {
                return GetFormats(true);
            }

            public string[] GetFormats(bool autoConvert)
            {
                return new string[] { DataFormats.FileDrop };
            }

            public void SetData(object data)
            {
                throw new NotSupportedException();
            }

            public void SetData(Type format, object data)
            {
                throw new NotSupportedException();
            }

            public void SetData(string format, object data)
            {
                throw new NotSupportedException();
            }

            public void SetData(string format, bool autoConvert, object data)
            {
                throw new NotSupportedException();
            }
        }

        private class Preview : IDisposable
        {
            private readonly Size _borderSize = SystemInformation.BorderSize;
            private Size _calculatedSize;
            private readonly Func<Point, int> _currentIndex;
            private bool _disposed = false;
            private readonly Form _form;
            private Image _image;
            private Cursor _prevCursor;
            private readonly ToolTip _toolTip;

            public Preview(Form form, int itemIndex, Func<Point, int> currentIndex)
            {
                // set the variables and create the tooltip
                _form = form;
                _currentIndex = currentIndex;
                ItemIndex = itemIndex;
                _toolTip = new ToolTip()
                {
                    IsBalloon = false,
                    OwnerDraw = true,
                    UseAnimation = false,
                    UseFading = false,
                };
                _toolTip.Draw += Draw;
                _toolTip.Popup += Popup;
                _prevCursor = _form.Cursor;
                _form.Cursor = Cursors.WaitCursor;
            }

            ~Preview()
            {
                Dispose(false);
            }

            public bool IsPrestine
            {
                get { return !_disposed && _image == null; }
            }

            public int ItemIndex { get; private set; }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                // only dispose once
                if (!_disposed)
                {
                    _disposed = true;

                    // restore the cursor if necessary 
                    if (_image == null)
                    {
                        _form.Cursor = Cursors.Default;
                    }

                    // forward the dispose call
                    if (disposing)
                    {
                        _toolTip.Dispose();
                    }
                }
            }

            private void Draw(object sender, DrawToolTipEventArgs e)
            {
                // draw the background and border
                e.DrawBackground();
                e.DrawBorder();

                // draw the image within the border
                var destRect = e.Bounds;
                destRect.Inflate(-_borderSize.Width, -_borderSize.Height);
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.DrawImage(_image, destRect, 0, 0, _image.Width, _image.Height, GraphicsUnit.Pixel, null, _ =>
                {
                    // abort if the index has changed
                    return ItemIndex != _currentIndex(Cursor.Position);
                });
            }

            public void Initialize(Image image)
            {
                // check the arguments and state
                if (image == null)
                {
                    throw new ArgumentNullException("image");
                }
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().ToString());
                }
                if (_image != null)
                {
                    throw new InvalidOperationException();
                }

                // make a copy of the image (might be used in the abort callback) and restore the cursor
                _image = (Image)image.Clone();
                _form.Cursor = _prevCursor;

                // get the best fit next to the window
                var imageWidthF = (float)_image.Width;
                var imageHeightF = (float)_image.Height;
                var imageRatio = imageWidthF / imageHeightF;
                var workingArea = Screen.GetWorkingArea(_form);
                workingArea.Inflate(-_borderSize.Width, -_borderSize.Height);
                var windowRect = _form.Bounds;
                windowRect.Inflate(_borderSize.Width, _borderSize.Height);

                var leftRightRect = windowRect.Left - workingArea.Left > workingArea.Right - windowRect.Right ?
                    Rectangle.FromLTRB(workingArea.Left, workingArea.Top, windowRect.Left, workingArea.Bottom) :
                    Rectangle.FromLTRB(windowRect.Right, workingArea.Top, workingArea.Right, workingArea.Bottom);
                var hasLeftRight = leftRightRect.Width > 0 && leftRightRect.Height > 0;
                var topBottomRect = windowRect.Top - workingArea.Top > workingArea.Bottom - windowRect.Bottom ?
                    Rectangle.FromLTRB(workingArea.Left, workingArea.Top, workingArea.Right, windowRect.Top) :
                    Rectangle.FromLTRB(workingArea.Left, windowRect.Bottom, workingArea.Right, workingArea.Bottom);
                var hasTopBottom = topBottomRect.Width > 0 && topBottomRect.Height > 0;
                var toolTipRect =
                    !hasLeftRight && !hasTopBottom ? workingArea :
                    !hasLeftRight ? topBottomRect :
                    !hasTopBottom ? leftRightRect :
                    Math.Min(1, (float)topBottomRect.Width / (float)topBottomRect.Height > imageRatio ? topBottomRect.Height / imageHeightF : topBottomRect.Width / imageWidthF) >
                    Math.Min(1, (float)leftRightRect.Width / (float)leftRightRect.Height > imageRatio ? leftRightRect.Height / imageHeightF : leftRightRect.Width / imageWidthF) ?
                    topBottomRect :
                    leftRightRect;

                // shrink too large images
                _calculatedSize = toolTipRect.Size;
                if ((float)toolTipRect.Width / (float)toolTipRect.Height > imageRatio)
                {
                    if (imageHeightF > toolTipRect.Height)
                    {
                        _calculatedSize = new Size((int)Math.Round(imageRatio * toolTipRect.Height), toolTipRect.Height);
                    }
                }
                else
                {
                    if (imageWidthF > toolTipRect.Width)
                    {
                        _calculatedSize = new Size(toolTipRect.Width, (int)Math.Round(toolTipRect.Width / imageRatio));
                    }
                }

                // get the location
                var point =
                    toolTipRect.Left == windowRect.Right ? new Point(toolTipRect.Left, toolTipRect.Top + (toolTipRect.Height - _calculatedSize.Height) / 2) :
                    toolTipRect.Right == windowRect.Left ? new Point(toolTipRect.Right - _calculatedSize.Width, toolTipRect.Top + (toolTipRect.Height - _calculatedSize.Height) / 2) :
                    toolTipRect.Top == windowRect.Bottom ? new Point(toolTipRect.Left + (toolTipRect.Width - _calculatedSize.Width) / 2, toolTipRect.Top) :
                    toolTipRect.Bottom == windowRect.Top ? new Point(toolTipRect.Left + (toolTipRect.Width - _calculatedSize.Width) / 2, toolTipRect.Bottom - _calculatedSize.Height) :
                    new Point(Cursor.Position.X * 2 <= toolTipRect.Right - toolTipRect.Left ? toolTipRect.Right - _calculatedSize.Width : toolTipRect.Left, Cursor.Position.Y * 2 <= toolTipRect.Bottom - toolTipRect.Top ? toolTipRect.Bottom - _calculatedSize.Height : toolTipRect.Top);

                // add the border
                point.X -= _borderSize.Width;
                point.Y -= _borderSize.Height;
                _calculatedSize.Width += 2 * _borderSize.Width;
                _calculatedSize.Height += 2 * _borderSize.Height;

                // show the preview
                _toolTip.Show(">", _form, point.X - _form.Location.X, point.Y - _form.Location.Y);
            }

            private void Popup(object sender, PopupEventArgs e)
            {
                // set the tooltip size
                e.ToolTipSize = _calculatedSize;
            }
        }

        private class Work
        {
            private string _path = null;

            public Work(ExtractForm form)
            {
                // create a new work object
                MultipleFiles = form.checkBoxMultipleFiles.Checked;
                Format = (ConvertFormat)form.toolStripDropDownButtonFormat.Tag;
                var indices = new int[form.listViewPages.SelectedIndices.Count];
                form.listViewPages.SelectedIndices.CopyTo(indices, 0);
                Array.Sort(indices);
                Indices = indices;
            }

            public ConvertFormat Format { get; private set; }

            public int[] Indices { get; private set; }

            public bool MultipleFiles { get; private set; }

            public string Path
            {
                get
                {
                    // query the path if it is set
                    if (_path == null)
                    {
                        throw new InvalidOperationException();
                    }
                    return _path;
                }
            }

            public void SetPath(string path)
            {
                // ensure the argument and state are valid before setting the path
                if (path == null)
                {
                    throw new ArgumentNullException("path");
                }
                if (_path != null)
                {
                    throw new InvalidOperationException();
                }
                _path = path;
            }
        }

        private PdfDocument _document;
        private readonly string _filePath;
        private readonly string _filterFormatString;
        private readonly string _formatTextFormatString;
        private readonly Dictionary<int, Image> _imageCache = new Dictionary<int, Image>();
        private volatile int _imageStartRange;
        private bool? _lastMultipleFilesUserChoice;
        private int _pageCount;
        private volatile Preview _preview;
        private Document _renderDocument;

        public ExtractForm(string path)
        {
            // intialize the components
            InitializeComponent();
            InitializeAdditionalStatusStripComponents();

            // store the path and filter text and set the default locations
            _filePath = Path.GetFullPath(path);
            _filterFormatString = saveFileDialog.Filter;
            Text = string.Format(Text, Application.ProductName, Path.GetFileName(_filePath));
            var directory = Path.GetDirectoryName(path);
            saveFileDialog.InitialDirectory = directory;
            folderBrowserDialog.SelectedPath = directory;

            // initialize the various formats
            _formatTextFormatString = toolStripDropDownButtonFormat.Text;
            foreach (var format in Enumerable.Repeat(ConvertFormat.Pdf, 1).Concat(ConvertFormat.GetApplicable(DocumentType.PortableDocumentFormat)))
            {
                toolStripDropDownButtonFormat.DropDownItems.Add(format.Name).Tag = format;
            }
            SetFormat(ConvertFormat.Pdf);

            // simulate a selection and trackbar change
            listViewPages_SelectedIndexChanged(this, EventArgs.Empty);
            trackBarZoom_ValueChanged(this, EventArgs.Empty);

            // hide the status elements
            ShowStatus(false, false);
        }

        #region Additional Designer Code

        private void InitializeAdditionalStatusStripComponents()
        {
            // suspend the layout
            statusStrip.SuspendLayout();

            // create the single file checkbox
            checkBoxMultipleFiles = new CheckBox();
            var checkboxHost = new ToolStripControlHost(checkBoxMultipleFiles);
            checkboxHost.ToolTipText = toolStripStatusLabelMultipleFiles.ToolTipText;
            toolStripStatusLabelMultipleFiles.ToolTipText = null;
            checkboxHost.Margin = checkboxHost.Margin + new Padding(8, 4, 0, 0);
            statusStrip.Items.Insert(statusStrip.Items.IndexOf(toolStripStatusLabelMultipleFiles), checkboxHost);
            toolStripStatusLabelMultipleFiles.Click += (s, e) => { checkBoxMultipleFiles.Checked = !checkBoxMultipleFiles.Checked; };

            // create the zoom trackbar
            trackBarZoom = new TrackBar();
            trackBarZoom.TickStyle = TickStyle.None;
            trackBarZoom.MaximumSize = new Size(trackBarZoom.Width, toolStripDropDownButtonSave.Height);
            trackBarZoom.Minimum = 1;
            trackBarZoom.Maximum = 8;
            trackBarZoom.Value = 4;
            trackBarZoom.ValueChanged += trackBarZoom_ValueChanged;
            var trackbarHost = new ToolStripControlHost(trackBarZoom);
            trackbarHost.ToolTipText = new StringBuilder().Append(toolStripDropDownButtonZoomOut.ToolTipText).Append("/").Append(toolStripDropDownButtonZoomIn.ToolTipText).ToString();
            trackbarHost.Alignment = ToolStripItemAlignment.Right;
            statusStrip.Items.Insert(statusStrip.Items.IndexOf(toolStripDropDownButtonZoomOut), trackbarHost);

            // resume and perform layout
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
        }

        private System.Windows.Forms.CheckBox checkBoxMultipleFiles;
        private System.Windows.Forms.TrackBar trackBarZoom;

        #endregion

        #region Methods

        private string[] ExtractForDragAndDrop(Work work)
        {
            // determine how to extract the pages
            if (work.MultipleFiles)
            {
                // extract each page to a file and store all paths
                work.SetPath(Path.GetTempPath());
                var fileNames = PerformSync(ExtractToMultipleFiles, work);
                if (fileNames != null)
                {
                    return Array.ConvertAll(fileNames, f => Path.Combine(work.Path, f));
                }
            }
            else
            {
                // extract the pages into one document and store its path
                work.SetPath(Path.Combine(Path.GetTempPath(), GetDocumentFileName(work.Indices, work.Format.FileExtension)));
                if (PerformSync(ExtractToSingleFile, work))
                {
                    return new string[] { work.Path };
                }
            }

            // an error occured
            return null;
        }

        private string[] ExtractToMultipleFiles(Work work, Func<bool> isCanelled, Action<int, object> reportStatus)
        {
            // extract all selected pages
            var fileNames = new string[work.Indices.Length];
            var progress = 0;
            for (var i = 0; i < work.Indices.Length; i++)
            {
                // return if cancelled
                if (isCanelled())
                {
                    if (i > 0)
                    {
                        Array.Resize(ref fileNames, i);
                        return fileNames;
                    }
                    else
                    {
                        return null;
                    }
                }

                // get the current index and file name
                var index = work.Indices[i];
                var fileName = GetDocumentFileName(index, work.Format.FileExtension);
                var filePath = Path.Combine(work.Path, fileName);
                reportStatus(progress, fileName);

                // extract and save the page
                if (work.Format == ConvertFormat.Pdf)
                {
                    using (var onePageDocument = new PdfDocument())
                    {
                        onePageDocument.AddPage(_document.Pages[index]);
                        onePageDocument.Save(filePath);
                    }
                }
                else
                {
                    lock (_renderDocument)
                    {
                        _renderDocument.EnsureNoOpenRenderer();
                        using (var ghostscript = new Ghostscript(work.Format.GetArguments(filePath)))
                        {
                            _renderDocument.RunInitialize(ghostscript);
                            _renderDocument.RunPage(ghostscript, index + 1);
                        }
                    }
                }

                // store the file name and continue
                fileNames[i] = fileName;
                progress = (int)Math.Round((100.0 * (i + 1)) / work.Indices.Length);
                reportStatus(progress, string.Empty);
            }
            return fileNames;
        }

        private bool ExtractToSingleFile(Work work, Func<bool> isCanelled, Action<int, object> reportStatus)
        {
            // extract the pages to a single file
            var fileName = Path.GetFileName(work.Path);
            reportStatus(0, fileName);
            if (work.Format == ConvertFormat.Pdf)
            {
                // create the resulting pdf
                using (var combinedDocument = new PdfDocument())
                {
                    var prevProgress = 0;
                    for (var i = 0; i < work.Indices.Length; i++)
                    {
                        // return if cancelled
                        if (isCanelled())
                        {
                            return false;
                        }

                        // add the page and increment the progress
                        combinedDocument.AddPage(_document.Pages[work.Indices[i]]);
                        var progress = (int)Math.Round((90.0 * (i + 1)) / work.Indices.Length);
                        if (progress != prevProgress)
                        {
                            reportStatus(progress, fileName);
                            prevProgress = progress;
                        }
                    }
                    combinedDocument.Save(work.Path);
                }
            }
            else
            {
                // ensure there is no open renderer and create Ghostscript
                lock (_renderDocument)
                {
                    _renderDocument.EnsureNoOpenRenderer();
                    try
                    {
                        using (var ghostscript = new Ghostscript(work.Format.GetArguments(work.Path)))
                        {
                            // hook up the listener and initialize the document
                            ghostscript.Poll += (s, e) => e.Cancel = isCanelled();
                            _renderDocument.RunInitialize(ghostscript);
                            var prevProgress = 5;
                            reportStatus(prevProgress, fileName);

                            // extract each page
                            for (var i = 0; i < work.Indices.Length; i++)
                            {
                                _renderDocument.RunPage(ghostscript, work.Indices[i] + 1);
                                var progress = 5 + (int)Math.Round((90.0 * (i + 1)) / work.Indices.Length);
                                if (progress != prevProgress)
                                {
                                    reportStatus(progress, fileName);
                                    prevProgress = progress;
                                }
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // delete the file and exit
                        try
                        {
                            File.Delete(work.Path);
                        }
                        catch { }
                        return false;
                    }
                }
            }

            // finish the operation
            reportStatus(100, string.Empty);
            return true;
        }

        private string GetDocumentFileName(int index, string fileExtension)
        {
            // return the file name plus extension for a single page
            return new StringBuilder(Path.GetFileNameWithoutExtension(_filePath)).Append("_").Append(index + 1).Append(".").Append(fileExtension).ToString();
        }

        private string GetDocumentFileName(int[] indices, string fileExtension)
        {
            // return the combined file name plus extension for multiple pages
            var buffer = new StringBuilder(Path.GetFileNameWithoutExtension(_filePath));
            var isFirstSuffix = true;
            var firstIndex = 0;
            while (firstIndex < indices.Length)
            {
                // find the end of the current page range
                var lastIndex = firstIndex;
                var value = indices[firstIndex];
                while (lastIndex < indices.Length - 1 && indices[lastIndex + 1] == value + 1)
                {
                    lastIndex++;
                    value++;
                }

                // append the separator
                if (isFirstSuffix)
                {
                    buffer.Append("_");
                    isFirstSuffix = false;
                }
                else
                {
                    buffer.Append(",");
                }

                // append the single index or range
                buffer.Append(indices[firstIndex] + 1);
                if (lastIndex > firstIndex)
                {
                    buffer.Append("-").Append(indices[lastIndex] + 1);
                }

                // search for the next index or range
                firstIndex = lastIndex + 1;
            }
            return buffer.Append(".").Append(fileExtension).ToString();
        }

        private int GetItemIndexWithImageAt(Point position)
        {
            // get the item at the position
            position = listViewPages.PointToClient(position);
            var item = listViewPages.GetItemAt(position.X, position.Y);
            if (item == null)
            {
                return -1;
            }

            // ensure the position is within the image
            var imageBounds = listViewPages.GetItemRect(item.Index, ItemBoundsPortion.Icon);
            imageBounds.Inflate(-(imageBounds.Width - imageList.ImageSize.Width) / 2, -(imageBounds.Height - imageList.ImageSize.Height) / 2);
            return imageBounds.Contains(position) ? item.Index : -1;
        }

        private Image GetThumbFromImage(Image image)
        {
            // create the thumbnail
            var thumb = new Bitmap(imageList.ImageSize.Width, imageList.ImageSize.Height, PixelFormat.Format32bppArgb);
            using (var gc = Graphics.FromImage(thumb))
            {
                // check if we need to scale the image
                gc.Clear(Color.Transparent);
                gc.InterpolationMode = InterpolationMode.High;
                var thumbSize = (SizeF)imageList.ImageSize;
                var imageSize = (SizeF)image.Size;
                if (imageSize.Width <= thumbSize.Width && imageSize.Height <= thumbSize.Height)
                {
                    thumbSize = imageSize;
                }
                else
                {
                    // fit the image into the thumb size
                    var imageRatio = imageSize.Width / imageSize.Height;
                    if (thumbSize.Width / thumbSize.Height > imageRatio)
                    {
                        thumbSize.Width = imageRatio * thumbSize.Height;
                    }
                    else
                    {
                        thumbSize.Height = thumbSize.Width / imageRatio;
                    }
                }

                // draw the image centered
                gc.DrawImage(image, new RectangleF(new PointF((imageList.ImageSize.Width - thumbSize.Width) / 2, (imageList.ImageSize.Height - thumbSize.Height) / 2), thumbSize), new RectangleF(PointF.Empty, imageSize), GraphicsUnit.Pixel);
            }
            return thumb;
        }

        private void ImageLoader()
        {
            // initialize and repeat until every image is loaded or the form is disposed
            var lastStartRange = _imageStartRange;
            var lastIndex = lastStartRange;
            while (!IsDisposed)
            {
                // get the next index to load
                int index;
                lock (_imageCache)
                {
                    // check if we should load the preview
                    var preview = _preview;
                    if (preview != null && !_imageCache.ContainsKey(preview.ItemIndex))
                    {
                        // set the preview index
                        index = preview.ItemIndex;
                    }
                    else
                    {
                        // update the start index if it has changed
                        var newStartRange = _imageStartRange;
                        if (lastStartRange != newStartRange)
                        {
                            lastStartRange = newStartRange;
                            lastIndex = lastStartRange;
                        }

                        // skip all loaded indices until the end
                        while (lastIndex < _pageCount && _imageCache.ContainsKey(lastIndex))
                        {
                            lastIndex++;
                        }

                        // check if we reached the end
                        if (lastIndex >= _pageCount)
                        {
                            // skip all loaded indices until the start
                            lastIndex = lastStartRange - 1;
                            while (lastIndex >= 0 && _imageCache.ContainsKey(lastIndex))
                            {
                                lastIndex--;
                            }

                            // check if we reached the start
                            if (lastIndex < 0)
                            {
                                // all done, exit thread
                                break;
                            }
                        }

                        // set the index
                        index = lastIndex;
                    }
                }

                // try to load the image
                Image image;
                try
                {
                    lock (_renderDocument)
                    {
                        image = _renderDocument.RenderPage(index + 1, CurrentAutoScaleDimensions.Width, CurrentAutoScaleDimensions.Height, 1, 0, null, () => IsDisposed);
                    }
                }
                catch (OperationCanceledException)
                {
                    continue;
                }
                catch
                {
                    image = null;
                }

                // report the result
                lock (_imageCache)
                {
                    _imageCache.Add(index, image);
                }
                BeginInvoke(new Action<Image, int>(SetImage), image, index);
            }
        }

        private void OpenFolderAndSelectItems(string folder, string[] files)
        {
            // get the folder pidl
            var desktop = Native.SHGetDesktopFolder();
            IntPtr folderPidl;
            desktop.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, folder, IntPtr.Zero, out folderPidl, IntPtr.Zero);
            try
            {
                // check if there are any files to select
                if (files != null && files.Length > 0)
                {
                    // get the folder object
                    var folderObject = (Native.IShellFolder)desktop.BindToObject(folderPidl, IntPtr.Zero, typeof(Native.IShellFolder).GUID);
                    var filesPidl = new List<IntPtr>();
                    try
                    {
                        // get all child pidls
                        foreach (var file in files)
                        {
                            IntPtr filePidl;
                            folderObject.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, file, IntPtr.Zero, out filePidl, IntPtr.Zero);
                            filesPidl.Add(filePidl);
                        }

                        // show the folder and select the items
                        Native.SHOpenFolderAndSelectItems(folderPidl, filesPidl.Count, filesPidl.ToArray(), 0);
                    }
                    finally
                    {
                        // free the child pidls
                        foreach (var filePidl in filesPidl)
                        {
                            Marshal.FreeCoTaskMem(filePidl);
                        }
                    }
                }
                else
                {
                    // simply show the folder
                    Native.SHOpenFolderAndSelectItems(folderPidl, 0, null, 0);
                }
            }
            finally
            {
                // free the folder pidl
                Marshal.FreeCoTaskMem(folderPidl);
            }
        }

        private T PerformSync<T>(Func<Work, Func<bool>, Action<int, object>, T> action, Work work)
        {
            try
            {
                // perform the action
                return action(work, () => false, (progress, status) =>
                {
                    // update the status
                    try
                    {
                        toolStripProgressBarExtract.Value = progress;
                        toolStripStatusLabelExtract.Text = (string)status;
                    }
                    catch (InvalidOperationException)
                    {
                        // ignore thread call exceptions
                    }
                });
            }
            catch (Exception e)
            {
                // display PdfSharp an I/O errors and rethrow others
                if (e is PdfSharpException || e is IOException)
                {
                    MessageBox.Show(e.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return default(T);
                }
                throw;
            }
        }

        private void SetFormat(ConvertFormat format)
        {
            // set the new format
            toolStripDropDownButtonFormat.Tag = format;
            toolStripDropDownButtonFormat.Text = string.Format(_formatTextFormatString, format.Name);
            checkBoxMultipleFiles.Enabled = format.SupportsSingleFile;
            toolStripStatusLabelMultipleFiles.Enabled = format.SupportsSingleFile;

            // check or restore the multiple files box
            if (!format.SupportsSingleFile)
            {
                if (_lastMultipleFilesUserChoice == null)
                {
                    _lastMultipleFilesUserChoice = checkBoxMultipleFiles.Checked;
                }
                checkBoxMultipleFiles.Checked = true;
            }
            else if (_lastMultipleFilesUserChoice != null)
            {
                checkBoxMultipleFiles.Checked = _lastMultipleFilesUserChoice.Value;
                _lastMultipleFilesUserChoice = null;
            }
        }

        private void SetImage(Image image, int itemIndex)
        {
            // check if there is a preview pending for the image
            if (_preview != null && _preview.ItemIndex == itemIndex && _preview.IsPrestine)
            {
                // either initialize it or dispose it if there was an error
                if (image != null)
                {
                    _preview.Initialize(image);
                }
                else
                {
                    _preview.Dispose();
                }
            }

            // invalidate the given item
            listViewPages.RedrawItems(itemIndex, itemIndex, true);
        }

        private void SetPreview(int itemIndex)
        {
            // do nothing if it is already displayed or clear the old preview
            if (_preview != null)
            {
                if (itemIndex > -1 && _preview.ItemIndex == itemIndex)
                {
                    return;
                }
                _preview.Dispose();
                _preview = null;
            }

            // check if a tooltip should be shown
            if (itemIndex > -1)
            {
                // create and show the new preview if possible
                _preview = new Preview(this, itemIndex, GetItemIndexWithImageAt);
                Image image;
                if (TryGetImage(itemIndex, out image))
                {
                    if (image != null)
                    {
                        _preview.Initialize(image);
                    }
                    else
                    {
                        _preview.Dispose();
                    }
                }
            }
        }

        private void SetSelection(Converter<int, bool> selectIndex)
        {
            // go over all pages and select the requested ones
            for (var i = 0; i < _pageCount; i++)
            {
                if (selectIndex(i))
                {
                    listViewPages.SelectedIndices.Add(i);
                }
                else
                {
                    listViewPages.SelectedIndices.Remove(i);
                }
            }
        }

        private void ShowStatus(bool visible, bool cancelable)
        {
            // set the visibilty and enabled states
            toolStripProgressBarExtract.Visible = visible;
            toolStripStatusLabelExtract.Visible = visible;
            toolStripDropDownButtonCancel.Visible = visible;
            toolStripDropDownButtonCancel.Enabled = cancelable;
            toolStripDropDownButtonSave.Visible = !visible;
            toolStripDropDownButtonFormat.Visible = !visible;
            checkBoxMultipleFiles.Visible = !visible;
            toolStripStatusLabelMultipleFiles.Visible = !visible;
            toolStripDropDownButtonZoomOut.Visible = !visible;
            trackBarZoom.Visible = !visible;
            toolStripDropDownButtonZoomIn.Visible = !visible;
            listViewPages.Enabled = !visible;

            // reset the progress bar and status label
            toolStripProgressBarExtract.Value = 0;
            toolStripStatusLabelExtract.Text = string.Empty;
        }

        private bool TryGetImage(int itemIndex, out Image image)
        {
            // query the item cache within a lock
            lock (_imageCache)
            {
                return _imageCache.TryGetValue(itemIndex, out image);
            }
        }

        #endregion

        #region Event Handlers

        private void backgroundWorkerExtract_DoWork(object sender, DoWorkEventArgs e)
        {
            var work = (Work)e.Argument;
            if (work.MultipleFiles)
            {
                // extract each page as one file and show them in the explorer
                var files = ExtractToMultipleFiles(work, () => (sender as BackgroundWorker).CancellationPending, (sender as BackgroundWorker).ReportProgress);
                if (files == null)
                {
                    e.Cancel = true;
                }
                else
                {
                    e.Result = files;
                    OpenFolderAndSelectItems(folderBrowserDialog.SelectedPath, files);
                }
            }
            else
            {
                // extract all pages into one file and show it
                if (ExtractToSingleFile(work, () => (sender as BackgroundWorker).CancellationPending, (sender as BackgroundWorker).ReportProgress))
                {
                    e.Result = true;
                    Process.Start(work.Path);
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void backgroundWorkerExtract_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // update the status bar
            toolStripProgressBarExtract.Value = e.ProgressPercentage;
            toolStripStatusLabelExtract.Text = (string)e.UserState;
        }

        private void backgroundWorkerExtract_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // hide the status elements and handle any potential error
            ShowStatus(false, true);
            if (!e.Cancelled && e.Error != null)
            {
                // display PdfSharp an I/O errors and rethrow others
                if (e.Error is PdfSharpException || e.Error is IOException)
                {
                    MessageBox.Show(e.Error.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    throw e.Error;
                }
            }
        }

        private void ExtractForm_Shown(object sender, EventArgs e)
        {
            // draw the form
            Update();

            // open the document and set the page count
            var prevCursor = Cursor;
            Cursor = Cursors.WaitCursor;
            try
            {
                _document = PdfReader.Open(_filePath, PdfDocumentOpenMode.Import);
                _renderDocument = Document.FromPdf(_document);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
            finally
            {
                Cursor = prevCursor;
            }
            _pageCount = _document.PageCount;
            listViewPages.VirtualMode = true;
            listViewPages.VirtualListSize = _pageCount;

            // start the image loader
            var loader = new Thread(ImageLoader);
            loader.IsBackground = true;
            loader.Start();
        }

        private void listViewPages_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            // set the range
            _imageStartRange = e.StartIndex;
        }

        private void listViewPages_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // hide the tooltip
            SetPreview(-1);

            // perform the operation
            using (var data = new FilesDataObject(ExtractForDragAndDrop, new Work(this)))
            {
                ShowStatus(true, false);
                Update();
                data.Result = listViewPages.DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Move);
                ShowStatus(false, false);
            }
        }

        private void listViewPages_MouseLeave(object sender, EventArgs e)
        {
            // hide the preview
            SetPreview(-1);
        }

        private void listViewPages_MouseMove(object sender, MouseEventArgs e)
        {
            // show the preview
            SetPreview(GetItemIndexWithImageAt((sender as Control).PointToScreen(e.Location)));
        }

        private void listViewPages_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            // check if the thumb needs to be added
            var imageName = string.Format(CultureInfo.InvariantCulture, ImageNameFormat, e.ItemIndex);
            if (!imageList.Images.ContainsKey(imageName))
            {
                // try to get a loaded image
                Image image;
                if (TryGetImage(e.ItemIndex, out image))
                {
                    // store the thumb if it's valid or use the error image
                    if (image != null)
                    {
                        imageList.Images.Add(imageName, GetThumbFromImage(image));
                    }
                    else
                    {
                        imageName = ErrorImageName;
                    }
                }
                else
                {
                    imageName = LoadingImageName;
                }
            }

            // get the image index and create the item
            e.Item = new ListViewItem((e.ItemIndex + 1).ToString(), imageList.Images.IndexOfKey(imageName));
        }

        private void listViewPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            // update the save button state
            toolStripDropDownButtonSave.Enabled = listViewPages.SelectedIndices.Count > 0;
        }

        private void listViewPages_VirtualItemsSelectionRangeChanged(object sender, ListViewVirtualItemsSelectionRangeChangedEventArgs e)
        {
            // do the same as a single selection change
            listViewPages_SelectedIndexChanged(sender, e);
        }

        private void toolStripDropDownButtonCancel_Click(object sender, EventArgs e)
        {
            // cancel the extraction
            backgroundWorkerExtract.CancelAsync();
        }

        private void toolStripDropDownButtonFormat_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // change the format
            var format = (ConvertFormat)e.ClickedItem.Tag;
            if (format.ShowDialog(this))
            {
                SetFormat(format);
            }
        }

        private void toolStripDropDownButtonSave_Click(object sender, EventArgs e)
        {
            // get the selection and check what to do
            var work = new Work(this);
            if (work.MultipleFiles)
            {
                // make sure the user picks a directory
                if (folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
                work.SetPath(folderBrowserDialog.SelectedPath);
            }
            else
            {
                // get a proper default name and make sure the user finishes the save dialog
                saveFileDialog.DefaultExt = work.Format.FileExtension;
                saveFileDialog.Filter = string.Format(_filterFormatString, work.Format.Name, work.Format.FileExtension);
                saveFileDialog.FileName = GetDocumentFileName(work.Indices, work.Format.FileExtension);
                if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
                work.SetPath(saveFileDialog.FileName);
            }

            // show the status elements and start the work
            ShowStatus(true, true);
            backgroundWorkerExtract.RunWorkerAsync(work);
        }

        private void toolStripDropDownButtonZoomIn_Click(object sender, EventArgs e)
        {
            trackBarZoom.Value++;
        }

        private void toolStripDropDownButtonZoomOut_Click(object sender, EventArgs e)
        {
            trackBarZoom.Value--;
        }

        private void toolStripMenuItemSelectAll_Click(object sender, EventArgs e)
        {
            SetSelection(i => true);
        }

        private void toolStripMenuItemSelectEven_Click(object sender, EventArgs e)
        {
            SetSelection(i => i % 2 == 0);
        }

        private void toolStripMenuItemSelectInvert_Click(object sender, EventArgs e)
        {
            SetSelection(i => !listViewPages.SelectedIndices.Contains(i));
        }

        private void toolStripMenuItemSelectOdd_Click(object sender, EventArgs e)
        {
            SetSelection(i => i % 2 != 0);
        }

        private void trackBarZoom_ValueChanged(object sender, EventArgs e)
        {
            // set the enabled state
            toolStripDropDownButtonZoomOut.Enabled = trackBarZoom.Value > trackBarZoom.Minimum;
            toolStripDropDownButtonZoomIn.Enabled = trackBarZoom.Value < trackBarZoom.Maximum;

            // reset the image list
            imageList.Images.Clear();
            var maxSize = (double)(trackBarZoom.Value * 32);
            imageList.ImageSize = new Size((int)Math.Round(maxSize * ((double)CurrentAutoScaleDimensions.Width / 96.0)), (int)Math.Round(maxSize * ((double)CurrentAutoScaleDimensions.Height / 96.0)));

            // add the loading and error image
            using (var pictureBox = new PictureBox())
            {
                imageList.Images.Add(LoadingImageName, GetThumbFromImage(pictureBox.InitialImage));
                imageList.Images.Add(ErrorImageName, GetThumbFromImage(pictureBox.ErrorImage));
            }

            // invalidate all items
            listViewPages.Invalidate();
        }

        #endregion
    }
}
