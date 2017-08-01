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
using System.Windows.Forms;
using Ghostscript.NET.Rasterizer;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Aufbauwerk.Tools.PdfKit
{
    public partial class ExtractForm : Form
    {
        private class FilesDataObject : IDataObject, IDisposable
        {
            private readonly object filePathsLock = new object();
            private readonly int[] indices;
            private readonly bool singleFiles;
            private readonly Func<int[], bool, string[]> extractDelegate;
            private bool disposed = false;
            private DragDropEffects result = DragDropEffects.None;
            private string[] filePaths = null;

            public FilesDataObject(Func<int[], bool, string[]> extractDelegate, int[] indices, bool singleFiles)
            {
                // store the parameters
                this.indices = indices;
                this.singleFiles = singleFiles;
                this.extractDelegate = extractDelegate;
            }

            ~FilesDataObject()
            {
                Dispose(false);
            }

            private string[] Extract()
            {
                // extract the documents if not done so already
                CheckDisposed();
                lock (filePathsLock)
                {
                    if (filePaths == null)
                    {
                        filePaths = extractDelegate(indices, singleFiles);
                    }
                    return filePaths;
                }
            }

            private void CheckDisposed()
            {
                // throw an error if the object is disposed
                if (disposed)
                {
                    throw new ObjectDisposedException(GetType().ToString());
                }
            }

            protected virtual void Dispose(bool disposing)
            {
                // dispose the drop object if not done so already
                if (!disposed)
                {
                    disposed = true;

                    // cleanup extracted documents if they weren't moved
                    if (filePaths != null && result != DragDropEffects.Move)
                    {
                        for (var i = 0; i < filePaths.Length; i++)
                        {
                            try { File.Delete(filePaths[i]); }
                            catch { }
                        }
                    }
                }
            }

            public DragDropEffects Result
            {
                get { return result; }
                set
                {
                    // ensure the object is not disposed and set the result
                    CheckDisposed();
                    result = value;
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

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
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
        private Image[] _imageCache;
        private Image _previewImage;
        private Size _previewSize;
        private GhostscriptRasterizer _rasterizer;

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
            trackBarZoom.MaximumSize = new Size(100, 20);
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
                    Array.Resize(ref fileNames, i);
                    return fileNames;
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

        private int[] GetSelectedIndices()
        {
            // get the selected indices   
            var selected = new int[listViewPages.SelectedIndices.Count];
            listViewPages.SelectedIndices.CopyTo(selected, 0);
            Array.Sort(selected);
            return selected;
        }

        private void OpenFolderAndSelectItems(string folder, string[] files)
        {
            // get the folder pidl
            var desktop = Native.SHGetDesktopFolder();
            IntPtr folderPidl;
            desktop.ParseDisplayName(Handle, IntPtr.Zero, folder, IntPtr.Zero, out folderPidl, IntPtr.Zero);
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
                            folderObject.ParseDisplayName(Handle, IntPtr.Zero, file, IntPtr.Zero, out filePidl, IntPtr.Zero);
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
            // show the status elements and redraw the form
            ShowStatus(true, false);
            Update();
            try
            {
                // perform the action
                return action(path, indizes, () => false, (progress, status) =>
                {
                    // update the status
                    toolStripProgressBarExtract.Value = progress;
                    toolStripStatusLabelExtract.Text = (string)status;
                    statusStrip.Update();
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
            finally
            {
                // hide the status elements and redraw the form
                ShowStatus(false, false);
                Update();
            }
        }

        private void SetTooltip(ListViewItem item)
        {
            // check if a tooltip should be shown
            if (item != null)
            {
                // do nothing if it is already shown
                if (_previewImage == _imageCache[item.Index])
                {
                    return;
                }

                // store the image and calculate the best size
                _previewImage = _imageCache[item.Index];
                var screenRect = Screen.FromControl(listViewPages).Bounds;
                var itemRect = listViewPages.RectangleToScreen(item.Bounds);
                var factor = Math.Min(1, Math.Min((float)((screenRect.Width - 2 - itemRect.Width) / 2) / (float)_previewImage.Size.Width, (float)(screenRect.Height - 2) / (float)_previewImage.Size.Height));
                _previewSize = new Size((int)(_previewImage.Size.Width * factor), (int)(_previewImage.Size.Height * factor));

                // calculate the location (prefer the right top corner)
                var pointPreview = new Point()
                {
                    X = itemRect.Right + _previewSize.Width > screenRect.Right ? itemRect.Left - _previewSize.Width : itemRect.Right,
                    Y = Math.Min(itemRect.Top, screenRect.Bottom - _previewSize.Height),
                };

                // show the tooltip
                toolTipPreview.Show(">", listViewPages, listViewPages.PointToClient(pointPreview));
            }
            else
            {
                // clear the preview variables and hide the tooltip if there is one
                if (_previewImage == null)
                {
                    return;
                }
                _previewImage = null;
                _previewSize = Size.Empty;
                toolTipPreview.Hide(listViewPages);
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

        #endregion

        #region Event Handlers

        private void backgroundWorkerExtract_DoWork(object sender, DoWorkEventArgs e)
        {
            var work = (Work)e.Argument;
            if (work.MultipleFiles)
            {
                // extract each page as one document and show them in the explorer
                var files = ExtractToMultipleDocuments(work.Path, work.Indices, () => (sender as BackgroundWorker).CancellationPending, (sender as BackgroundWorker).ReportProgress);
                e.Result = files;
                if (files != null)
                {
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
                    e.Result = false;
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
            // enable the virtual mode and paint the form
            listViewPages.VirtualMode = true;
            Update();

            // open the document and create the rasterizer
            try
            {
                _document = PdfReader.Open(_filePath, PdfDocumentOpenMode.Import);
                _rasterizer = new GhostscriptRasterizer();
                _rasterizer.Open(_filePath, Program.GhostscriptVersion, false);
            }
            catch
            {
                // turn off the virtual mode and rethrow the error
                listViewPages.VirtualMode = false;
                throw;
            }

            // initialize the cache and set the page count
            _imageCache = new Image[_document.PageCount];
            listViewPages.VirtualListSize = _document.PageCount;
        }

        private void listViewPages_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // hide the tooltip and make sure the progress bar handle is created
            SetTooltip(null);
            toolStripProgressBarExtract.Control.Handle.GetHashCode();

            // perform the operation
            using (var data = new FilesDataObject(ExtractForDragAndDrop, GetSelectedIndices(), checkBoxMultipleFiles.Checked))
            {
                data.Result = listViewPages.DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        private void listViewPages_MouseLeave(object sender, EventArgs e)
        {
            // hide the preview image
            SetTooltip(null);
        }

        private void listViewPages_MouseMove(object sender, MouseEventArgs e)
        {
            // ensure the cursor is above the image and show the preview
            var item = listViewPages.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                var imageBounds = listViewPages.GetItemRect(item.Index, ItemBoundsPortion.Icon);
                imageBounds.Inflate(-(imageBounds.Width - imageList.ImageSize.Width) / 2, -(imageBounds.Height - imageList.ImageSize.Height) / 2);
                if (!imageBounds.Contains(e.Location))
                {
                    item = null;
                }
            }
            SetTooltip(item);
        }

        private void listViewPages_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            // check if the icon already exists
            var key = e.ItemIndex.ToString(CultureInfo.InvariantCulture);
            if (!imageList.Images.ContainsKey(key))
            {
                // get the cached image or rasterize the page
                var image = _imageCache[e.ItemIndex];
                if (image == null)
                {
                    _imageCache[e.ItemIndex] = image = _rasterizer.GetPage(96, 96, e.ItemIndex + 1);
                }

                // draw the icon
                var icon = new Bitmap(imageList.ImageSize.Width, imageList.ImageSize.Height, PixelFormat.Format32bppArgb);
                var factor = Math.Min(1, Math.Min((float)icon.Size.Width / (float)image.Size.Width, (float)icon.Size.Height / (float)image.Size.Height));
                var sizef = new SizeF(image.Size.Width * factor, image.Size.Height * factor);
                var rectf = new RectangleF((icon.Width - sizef.Width) / 2, (icon.Height - sizef.Height) / 2, sizef.Width, sizef.Height);
                using (var graphics = Graphics.FromImage(icon))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(image, rectf);
                }
                imageList.Images.Add(key, icon);
            }

            // create the item
            e.Item = new ListViewItem((e.ItemIndex + 1).ToString(), imageList.Images.IndexOfKey(key));
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

        private void toolTipPreview_Draw(object sender, DrawToolTipEventArgs e)
        {
            // draw the background, image and border
            e.DrawBackground();
            var bounds = e.Bounds;
            bounds.Inflate(-1, 1);
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.DrawImage(_previewImage, bounds);
            e.DrawBorder();
        }

        private void toolTipPreview_Popup(object sender, PopupEventArgs e)
        {
            // cancel the tooltip or set the tooltip size
            if (_previewImage == null)
            {
                e.Cancel = true;
            }
            else
            {
                e.ToolTipSize = _previewSize + new Size(2, 2);
            }
        }

        private void trackBarZoom_ValueChanged(object sender, EventArgs e)
        {
            // set the enabled state and reset the image list
            toolStripDropDownButtonZoomOut.Enabled = trackBarZoom.Value > trackBarZoom.Minimum;
            toolStripDropDownButtonZoomIn.Enabled = trackBarZoom.Value < trackBarZoom.Maximum;
            imageList.Images.Clear();
            imageList.ImageSize = new Size(trackBarZoom.Value * 32, trackBarZoom.Value * 32);

            // refrash the list
            listViewPages.Invalidate();
        }

        #endregion
    }
}
