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
        private const string LoadingImageName = "loading";
        private const string ErrorImageName = "error";

        private class FilesDataObject : IDataObject, IDisposable
        {
            private bool _disposed = false;
            private readonly Func<int[], bool, string[]> _extractDelegate;
            private string[] _filePaths = null;
            private readonly object _filePathsLock = new object();
            private readonly int[] _indices;
            private readonly bool _multipleFiles;
            private DragDropEffects _result = DragDropEffects.None;

            public FilesDataObject(Func<int[], bool, string[]> extractDelegate, int[] indices, bool multipleFiles)
            {
                // store the parameters
                _indices = indices;
                _multipleFiles = multipleFiles;
                _extractDelegate = extractDelegate;
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
                        _filePaths = _extractDelegate(_indices, _multipleFiles);
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

            public bool IsPrestine
            {
                get { return !_disposed && _image == null; }
            }

            public int ItemIndex { get; private set; }

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

        private struct Work
        {
            public bool MultipleFiles;
            public string Path;
            public int[] Indices;
        }

        private PdfDocument _document;
        private readonly string _filePath;
        private readonly Dictionary<int, Image> _imageCache = new Dictionary<int, Image>();
        private volatile int _imageStartRange;
        private int _pageCount;
        private Preview _preview;

        public ExtractForm(string path)
        {
            // intialize the components
            InitializeComponent();
            InitializeAdditionalStatusStripComponents();

            // store the path and set the default locations
            _filePath = Path.GetFullPath(path);
            Text = string.Format(Text, Path.GetFileName(_filePath));
            var directory = Path.GetDirectoryName(path);
            saveFileDialog.InitialDirectory = directory;
            folderBrowserDialog.SelectedPath = directory;

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
            checkboxHost.ToolTipText = toolStripStatusLabelSingleFiles.ToolTipText;
            toolStripStatusLabelSingleFiles.ToolTipText = null;
            checkboxHost.Margin = checkboxHost.Margin + new Padding(8, 4, 0, 0);
            statusStrip.Items.Insert(statusStrip.Items.IndexOf(toolStripStatusLabelSingleFiles), checkboxHost);
            toolStripStatusLabelSingleFiles.Click += (s, e) => { checkBoxMultipleFiles.Checked = !checkBoxMultipleFiles.Checked; };

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

        #region Properties

        private float DpiX
        {
            get
            {
                if (AutoScaleMode != AutoScaleMode.Dpi)
                {
                    throw new InvalidOperationException();
                }
                return CurrentAutoScaleDimensions.Width;
            }
        }

        private float DpiY
        {
            get
            {
                if (AutoScaleMode != AutoScaleMode.Dpi)
                {
                    throw new InvalidOperationException();
                }
                return CurrentAutoScaleDimensions.Height;
            }
        }

        #endregion

        #region Methods

        private string[] ExtractForDragAndDrop(int[] indices, bool multipleFiles)
        {
            // determine how to extract the pages
            if (!multipleFiles)
            {
                // extract the pages into one document and store its path
                var path = Path.Combine(Path.GetTempPath(), GetDocumentFileName(indices));
                if (PerformSync(ExtractToSingleDocument, path, indices))
                {
                    return new string[] { path };
                }
            }
            else
            {
                // extract each page to a document and store all paths
                var root = Path.GetTempPath();
                var fileNames = PerformSync(ExtractToMultipleDocuments, root, indices);
                if (fileNames != null)
                {
                    return Array.ConvertAll(fileNames, f => Path.Combine(root, f));
                }
            }

            // an error occured
            return null;
        }

        private string[] ExtractToMultipleDocuments(string path, int[] indices, Func<bool> isCanelled, Action<int, object> reportStatus)
        {
            // extract all selected pages
            var fileNames = new string[indices.Length];
            var progress = 0;
            for (var i = 0; i < indices.Length; i++)
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
                var index = indices[i];
                var fileName = GetDocumentFileName(index);
                reportStatus(progress, fileName);

                // extract and save the page
                using (var onePageDocument = new PdfDocument())
                {
                    onePageDocument.AddPage(_document.Pages[index]);
                    onePageDocument.Save(Path.Combine(path, fileName));
                }

                // store the file name and continue
                fileNames[i] = fileName;
                progress = (int)Math.Round((100.0 * (i + 1)) / indices.Length);
                reportStatus(progress, string.Empty);
            }
            return fileNames;
        }

        private bool ExtractToSingleDocument(string path, int[] indices, Func<bool> isCanelled, Action<int, object> reportStatus)
        {
            // create the resulting pdf
            var fileName = Path.GetFileName(path);
            reportStatus(0, fileName);
            using (var combinedDocument = new PdfDocument())
            {
                var prevProgress = 0;
                for (var i = 0; i < indices.Length; i++)
                {
                    // return if cancelled
                    if (isCanelled())
                    {
                        return false;
                    }

                    // add the page and increment the progress
                    combinedDocument.AddPage(_document.Pages[indices[i]]);
                    var progress = (int)Math.Round((99.0 * (i + 1)) / indices.Length);
                    if (progress != prevProgress)
                    {
                        reportStatus(progress, fileName);
                        prevProgress = progress;
                    }
                }
                combinedDocument.Save(path);
            }
            reportStatus(100, string.Empty);
            return true;
        }

        private string GetDocumentFileName(int index)
        {
            // return the file name plus extension for a single page
            return new StringBuilder(Path.GetFileNameWithoutExtension(_filePath)).Append("_").Append(index + 1).Append(".pdf").ToString();
        }

        private string GetDocumentFileName(int[] indices)
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
            return buffer.Append(".pdf").ToString();
        }

        private string GetImageName(int itemIndex)
        {
            return "image" + itemIndex.ToString(CultureInfo.InvariantCulture);
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

        private int[] GetSelectedIndices()
        {
            // get the selected indices   
            var selected = new int[listViewPages.SelectedIndices.Count];
            listViewPages.SelectedIndices.CopyTo(selected, 0);
            Array.Sort(selected);
            return selected;
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
                if (imageSize.Width > thumbSize.Width || imageSize.Height > thumbSize.Height)
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
            // repeat infinitely
            while (true)
            {
                int firstIndex;
                int lastIndex;
                lock (_imageCache)
                {
                    // get the first index to load
                    var startIndex = _imageStartRange;
                    firstIndex = startIndex;
                    while (firstIndex < _pageCount && _imageCache.ContainsKey(firstIndex))
                    {
                        firstIndex++;
                    }
                    if (firstIndex >= _pageCount)
                    {
                        // everything loaded from start, load before
                        firstIndex = startIndex - 1;
                        while (firstIndex >= 0 && _imageCache.ContainsKey(firstIndex))
                        {
                            firstIndex--;
                        }
                        if (firstIndex < 0)
                        {
                            // all done, exit thread
                            break;
                        }
                    }

                    // get the last not loaded index
                    lastIndex = firstIndex;
                    while (lastIndex + 1 < _pageCount && !_imageCache.ContainsKey(lastIndex + 1))
                    {
                        lastIndex++;
                    }
                }

                // start loading the range of images
                var currentIndex = firstIndex;
                try
                {
                    Ghostscript.RenderPages(_filePath, firstIndex + 1, lastIndex + 1, DpiX, DpiY, 1, null, () =>
                    {
                        // cancel the operation if the user scrolled before the first index or after the loaded area
                        var startIndex = _imageStartRange;
                        return startIndex < firstIndex || currentIndex < startIndex;
                    }, (pageNumber, image) =>
                    {
                        // add and report the image
                        currentIndex = pageNumber;
                        lock (_imageCache)
                        {
                            _imageCache.Add(pageNumber - 1, image);
                        }
                        BeginInvoke(new Action<Image, int>(SetImage), image, pageNumber - 1);
                    });
                }
                catch
                {
                    // report the current page as error if it's within range
                    if (firstIndex <= currentIndex && currentIndex <= lastIndex)
                    {
                        lock (_imageCache)
                        {
                            _imageCache.Add(currentIndex, null);
                        }
                        BeginInvoke(new Action<Image, int>(SetImage), null, currentIndex);
                    }
                }
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

        private T PerformSync<T>(Func<string, int[], Func<bool>, Action<int, object>, T> action, string path, int[] indizes)
        {
            try
            {
                // perform the action
                return action(path, indizes, () => false, (progress, status) =>
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
            catch (PdfSharpException e)
            {
                // show the error
                MessageBox.Show(e.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return default(T);
            }
            catch (IOException e)
            {
                // show the error
                MessageBox.Show(e.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return default(T);
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

        private void ShowStatus(bool visible, bool cancelable)
        {
            // set the visibilty and enabled states
            toolStripProgressBarExtract.Visible = visible;
            toolStripStatusLabelExtract.Visible = visible;
            toolStripDropDownButtonCancel.Visible = visible;
            toolStripDropDownButtonCancel.Enabled = cancelable;
            toolStripDropDownButtonSave.Visible = !visible;
            checkBoxMultipleFiles.Visible = !visible;
            toolStripStatusLabelSingleFiles.Visible = !visible;
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
                // extract each page as one document and show them in the explorer
                var files = ExtractToMultipleDocuments(work.Path, work.Indices, () => (sender as BackgroundWorker).CancellationPending, (sender as BackgroundWorker).ReportProgress);
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
                // extract all pages into one document and show it
                if (ExtractToSingleDocument(work.Path, work.Indices, () => (sender as BackgroundWorker).CancellationPending, (sender as BackgroundWorker).ReportProgress))
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
            _document = PdfReader.Open(_filePath, PdfDocumentOpenMode.Import);
            _pageCount = _document.PageCount;
            listViewPages.VirtualMode = true;
            listViewPages.VirtualListSize = _pageCount;

            // start the image loader
            var loader = new Thread(ImageLoader);
            loader.IsBackground = true;
            Disposed += (_, __) => loader.Abort();
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
            using (var data = new FilesDataObject(ExtractForDragAndDrop, GetSelectedIndices(), checkBoxMultipleFiles.Checked))
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
            var imageName = GetImageName(e.ItemIndex);
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

        private void toolStripDropDownButtonSave_Click(object sender, EventArgs e)
        {
            // get the selection and check what to do
            var work = new Work()
            {
                Indices = GetSelectedIndices(),
                MultipleFiles = checkBoxMultipleFiles.Checked,
            };
            if (work.MultipleFiles)
            {
                // make sure the user picks a directory
                if (folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
                work.Path = folderBrowserDialog.SelectedPath;
            }
            else
            {
                // get a proper default name and make sure the user finishes the save dialog
                saveFileDialog.FileName = GetDocumentFileName(work.Indices);
                if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
                work.Path = saveFileDialog.FileName;
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

        private void trackBarZoom_ValueChanged(object sender, EventArgs e)
        {
            // set the enabled state
            toolStripDropDownButtonZoomOut.Enabled = trackBarZoom.Value > trackBarZoom.Minimum;
            toolStripDropDownButtonZoomIn.Enabled = trackBarZoom.Value < trackBarZoom.Maximum;

            // reset the image list
            imageList.Images.Clear();
            var maxSize = trackBarZoom.Value * 32;
            imageList.ImageSize =
                DpiX > DpiY ? new Size(maxSize, (int)Math.Round(maxSize * (DpiY / DpiX))) :
                DpiX < DpiY ? new Size((int)Math.Round(maxSize * (DpiX / DpiY)), maxSize) :
                new Size(maxSize, maxSize);

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
