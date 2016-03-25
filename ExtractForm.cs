/* Copyright (C) 2016, Manuel Meitinger
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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Ghostscript.NET;
using Ghostscript.NET.Processor;
using Ghostscript.NET.Rasterizer;

namespace Aufbauwerk.Tools.PdfKit
{
    public partial class ExtractForm : Form
    {
        private readonly string shortPath;
        private readonly string fileName;
        private GhostscriptRasterizer rasterizer;
        private Image[] cache;
        private CheckBox checkBoxSingleFiles;
        private TrackBar trackBarZoom;
        private Image imagePreview;
        private Size sizePreview;

        public ExtractForm(string path)
        {
            // intialize the components
            InitializeComponent();
            InitializeAdditionalStatusStripComponents();

            // store the path and set the default locations
            fileName = Path.GetFileNameWithoutExtension(path);
            shortPath = Program.GetShortPathName(path);
            Text = string.Format(Text, fileName);
            var directory = Path.GetDirectoryName(path);
            saveFileDialog.InitialDirectory = directory;
            folderBrowserDialog.SelectedPath = directory;

            // simulate a trackbar change
            trackBarZoom_ValueChanged(this, EventArgs.Empty);
        }

        private void InitializeAdditionalStatusStripComponents()
        {
            // suspend layout
            statusStrip.SuspendLayout();

            // create the single file checkbox
            checkBoxSingleFiles = new CheckBox();
            var checkboxHost = new ToolStripControlHost(checkBoxSingleFiles);
            checkboxHost.ToolTipText = toolStripStatusLabelSingleFiles.ToolTipText;
            toolStripStatusLabelSingleFiles.ToolTipText = null;
            checkboxHost.Margin = checkboxHost.Margin + new Padding(8, 4, 0, 0);
            statusStrip.Items.Insert(statusStrip.Items.IndexOf(toolStripStatusLabelSingleFiles), checkboxHost);
            toolStripStatusLabelSingleFiles.Click += (s, e) => { checkBoxSingleFiles.Checked = !checkBoxSingleFiles.Checked; };

            // create the zoom trackbar
            trackBarZoom = new TrackBar();
            trackBarZoom.TickStyle = TickStyle.None;
            trackBarZoom.MaximumSize = new Size(100, 20);
            trackBarZoom.Minimum = 1;
            trackBarZoom.Maximum = 8;
            trackBarZoom.Value = 4;
            trackBarZoom.ValueChanged += trackBarZoom_ValueChanged;
            var trackbarHost = new ToolStripControlHost(trackBarZoom);
            trackbarHost.ToolTipText = string.Format("{0}/{1}", toolStripDropDownButtonZoomOut.ToolTipText, toolStripDropDownButtonZoomIn.ToolTipText);
            trackbarHost.Alignment = ToolStripItemAlignment.Right;
            statusStrip.Items.Insert(statusStrip.Items.IndexOf(toolStripDropDownButtonZoomOut), trackbarHost);

            // resume and perform layout
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
        }

        private void SetTooltip(ListViewItem item)
        {
            // check if a tooltip should be shown
            if (item != null)
            {
                // do nothing if it is already shown
                if (imagePreview == cache[item.Index])
                    return;

                // store the image and calculate the best size
                imagePreview = cache[item.Index];
                var screenRect = Screen.FromControl(listViewPages).Bounds;
                var itemRect = listViewPages.RectangleToScreen(item.Bounds);
                var factor = Math.Min(1, Math.Min((float)((screenRect.Width - 2 - itemRect.Width) / 2) / (float)imagePreview.Size.Width, (float)(screenRect.Height - 2) / (float)imagePreview.Size.Height));
                sizePreview = new Size((int)(imagePreview.Size.Width * factor), (int)(imagePreview.Size.Height * factor));

                // calculate the location (prefer the right top corner)
                var pointPreview = new Point()
                {
                    X = itemRect.Right + sizePreview.Width > screenRect.Right ? itemRect.Left - sizePreview.Width : itemRect.Right,
                    Y = Math.Min(itemRect.Top, screenRect.Bottom - sizePreview.Height)
                };

                // show the tooltip
                toolTipPreview.Show(">", listViewPages, listViewPages.PointToClient(pointPreview));
            }
            else
            {
                // clear the preview variables and hide the tooltip
                if (imagePreview == null)
                    return;
                imagePreview = null;
                sizePreview = Size.Empty;
                toolTipPreview.Hide(listViewPages);
            }
        }

        private void ShowStatus(bool visible, int maximum)
        {
            // set the visibilty and update
            toolStripProgressBarExtract.Visible = visible;
            toolStripStatusLabelExtract.Visible = visible;
            toolStripDropDownButtonSave.Visible = !visible;
            checkBoxSingleFiles.Visible = !visible;
            toolStripStatusLabelSingleFiles.Visible = !visible;
            toolStripDropDownButtonZoomOut.Visible = !visible;
            trackBarZoom.Visible = !visible;
            toolStripDropDownButtonZoomIn.Visible = !visible;
            listViewPages.Enabled = !visible;
            listViewPages.Update();
            toolStripProgressBarExtract.Value = 0;
            toolStripProgressBarExtract.Maximum = maximum;
            toolStripStatusLabelExtract.Text = string.Empty;
            statusStrip.Update();
        }

        private int[] GetSelectedIndices()
        {
            // get the selected indices   
            var selected = new int[listViewPages.SelectedIndices.Count];
            listViewPages.SelectedIndices.CopyTo(selected, 0);
            Array.Sort(selected);
            return selected;
        }

        private Tuple<int, int>[] GetRangesFromIndices(int[] indices)
        {
            // find continuous indices
            var ranges = new List<Tuple<int, int>>();
            if (indices.Length > 0)
            {
                var start = 0;
                for (var i = 1; i < indices.Length; i++)
                {
                    // add the previous range if there is a gap
                    if (indices[start] + (i - start) < indices[i])
                    {
                        ranges.Add(new Tuple<int, int>(indices[start], indices[i - 1]));
                        start = i;
                    }
                }

                // add the remaining range
                ranges.Add(new Tuple<int, int>(indices[start], indices[indices.Length - 1]));
            }

            // return the array
            return ranges.ToArray();
        }

        private T PerformGhostscriptOperation<T>(int steps, Func<GhostscriptProcessor, Action, Action<string>, T> action)
        {
            // make sure the rasterizer is closed during the action and show the status elements
            rasterizer.Close();
            ShowStatus(true, steps);
            try
            {
                // perform the action
                using (var processor = new GhostscriptProcessor(Program.GhostscriptVersion, false))
                {
                    return action
                    (
                        processor,
                        () => toolStripProgressBarExtract.Value++,
                        text =>
                        {
                            toolStripStatusLabelExtract.Text = text;
                            statusStrip.Update();
                        }
                    );
                }
            }
            catch (GhostscriptException e)
            {
                // show the error
                MessageBox.Show(e.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return default(T);
            }
            finally
            {
                // hide the status and reopen the rasterizer
                ShowStatus(false, 0);
                rasterizer.Open(shortPath, Program.GhostscriptVersion, false);
            }
        }

        private string GetFileStatus(string path, string text)
        {
            // gets the string to be displayed in the status area
            return string.Format("{0} => {1}", text, Path.GetFileNameWithoutExtension(path));
        }

        private void ExtractRange(GhostscriptProcessor processor, Action stepId, Action<string> status, string path, int start, int end)
        {
            // make the range 1-based
            start++;
            end++;

            // extract the given range of pages
            status(start == end ? start.ToString() : string.Format("{0}-{1}", start, end));
            processor.Process(new string[]
            {
                "-q",
                "-dSAFER",
                "-dBATCH",
                "-dNOPAUSE",
                "-dNOPROMPT",
                "-sDEVICE=pdfwrite",
                "-dFirstPage=" + start.ToString(CultureInfo.InvariantCulture),
                "-dLastPage=" + end.ToString(CultureInfo.InvariantCulture),
                "-dAutoRotatePages=/None",
                "-sOutputFile=" + path,
                shortPath
            });
            stepId();
        }

        private void ExtractToSingleDocument(string path, Tuple<int, int>[] ranges)
        {
            PerformGhostscriptOperation(ranges.Length == 1 ? 1 : ranges.Length + 1, (processor, stepIt, status) =>
            {
                if (ranges.Length != 1)
                {
                    // extract all ranges and combine them into one document
                    var files = new List<string>();
                    try
                    {
                        // extract each range
                        var pages = new StringBuilder();
                        for (var i = 0; i < ranges.Length; i++)
                        {
                            var file = Path.GetTempFileName();
                            files.Add(file);
                            ExtractRange
                            (
                                processor,
                                stepIt,
                                t =>
                                {
                                    // append the page string
                                    if (pages.Length > 0)
                                        pages.Append(',');
                                    pages.Append(t);
                                    status(pages.ToString());
                                },
                                file,
                                ranges[i].Item1,
                                ranges[i].Item2
                            );
                        }

                        // combine all pages
                        status(GetFileStatus(path, pages.ToString()));
                        var args = new string[]
                        {
                            "-q",
                            "-dSAFER",
                            "-dBATCH",
                            "-dNOPAUSE",
                            "-dNOPROMPT",
                            "-sDEVICE=pdfwrite",
                            "-dAutoRotatePages=/None",
                            "-sOutputFile=" + path
                        };
                        var argc = args.Length;
                        Array.Resize(ref args, argc + files.Count);
                        files.CopyTo(args, argc);
                        processor.Process(args);
                        stepIt();
                    }
                    finally
                    {
                        // cleanup temporary files
                        foreach (var file in files)
                        {
                            try { File.Delete(file); }
                            catch { }
                        }
                    }
                }
                else
                    // simply extract the single range
                    ExtractRange(processor, stepIt, t => status(GetFileStatus(path, t)), path, ranges[0].Item1, ranges[0].Item2);
                return true;
            });
        }

        private string GetDocumentFileName(int index)
        {
            // return the file name plus extension for a single page
            return string.Format("{0}_{1}.pdf", fileName, index + 1);
        }

        private string GetDocumentFileName(Tuple<int, int>[] ranges)
        {
            // return the file name plus extension for a range of pages
            var pages = Array.ConvertAll(ranges, r => r.Item1 == r.Item2 ? (r.Item1 + 1).ToString() : string.Format("{0}-{1}", r.Item1 + 1, r.Item2 + 1));
            return string.Format("{0}_{1}.pdf", fileName, string.Join(",", pages));
        }

        private string[] ExtractToMultipleDocuments(string path, int[] indices)
        {
            return PerformGhostscriptOperation(indices.Length, (processor, stepIt, status) =>
            {
                // extract all selected pages
                var fileNames = Array.ConvertAll(indices, i => GetDocumentFileName(i));
                for (var i = 0; i < indices.Length; i++)
                {
                    var index = indices[i];
                    var fileName = Path.Combine(path, fileNames[i]);
                    ExtractRange(processor, stepIt, t => status(GetFileStatus(fileName, t)), fileName, index, index);
                }
                return fileNames;
            });
        }

        private void ExtractForm_Load(object sender, EventArgs e)
        {
            // create the rasterizer, initialize the cache and virtual mode
            rasterizer = new GhostscriptRasterizer();
            rasterizer.Open(shortPath, Program.GhostscriptVersion, false);
            cache = new Image[rasterizer.PageCount];
            listViewPages.VirtualListSize = rasterizer.PageCount;
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

        private void toolStripDropDownButtonZoomOut_Click(object sender, EventArgs e)
        {
            trackBarZoom.Value--;
        }

        private void toolStripDropDownButtonZoomIn_Click(object sender, EventArgs e)
        {
            trackBarZoom.Value++;
        }

        private void listViewPages_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            // check if the icon already exists
            var key = e.ItemIndex.ToString(CultureInfo.InvariantCulture);
            if (!imageList.Images.ContainsKey(key))
            {
                // get the cached image or rasterize the page
                var image = cache[e.ItemIndex];
                if (image == null)
                    cache[e.ItemIndex] = image = rasterizer.GetPage(96, 96, e.ItemIndex + 1);

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

        private void listViewPages_ItemDrag(object sender, ItemDragEventArgs e)
        {
            string[] files;
            if (checkBoxSingleFiles.Checked)
            {
            }
            else
            {

            }
            //var data = new DataObject(DataFormats.FileDrop, 
        }

        private void listViewPages_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {

        }

        private void toolStripDropDownButtonSave_Click(object sender, EventArgs e)
        {
            // get the selection and check what to do
            var indices = GetSelectedIndices();
            if (checkBoxSingleFiles.Checked)
            {
                // pick a directory and extract the pages
                if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                {
                    var files = ExtractToMultipleDocuments(folderBrowserDialog.SelectedPath, indices);
                    Program.OpenFolderAndSelectItems(folderBrowserDialog.SelectedPath, files);
                }
            }
            else
            {
                // show the save dialog and extract the pages to a single document
                var ranges = GetRangesFromIndices(indices);
                saveFileDialog.FileName = GetDocumentFileName(ranges);
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    ExtractToSingleDocument(saveFileDialog.FileName, ranges);
                    Process.Start(saveFileDialog.FileName);
                }
            }
        }

        private void toolTipPreview_Draw(object sender, DrawToolTipEventArgs e)
        {
            // draw the background, image and border
            e.DrawBackground();
            var bounds = e.Bounds;
            bounds.Inflate(-1, 1);
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.DrawImage(imagePreview, bounds);
            e.DrawBorder();
        }

        private void toolTipPreview_Popup(object sender, PopupEventArgs e)
        {
            // cancel the tooltip or set the tooltip size
            if (imagePreview == null)
                e.Cancel = true;
            else
                e.ToolTipSize = sizePreview + new Size(2, 2);
        }

        private void listViewPages_MouseLeave(object sender, EventArgs e)
        {
            // hide the preview image
            SetTooltip(null);
        }

        private void listViewPages_MouseMove(object sender, MouseEventArgs e)
        {
            // ensure the cursor is above the image and show the preview image
            var item = listViewPages.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                var imageBounds = listViewPages.GetItemRect(item.Index, ItemBoundsPortion.Icon);
                imageBounds.Inflate(-(imageBounds.Width - imageList.ImageSize.Width) / 2, -(imageBounds.Height - imageList.ImageSize.Height) / 2);
                if (!imageBounds.Contains(e.Location))
                    item = null;
            }
            SetTooltip(item);
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
    }
}
