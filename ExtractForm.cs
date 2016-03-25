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
        private readonly GhostscriptVersionInfo version = new GhostscriptVersionInfo("gsdll32.dll");
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

        private void ShowStatus(bool visible)
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
            toolStripStatusLabelExtract.Text = string.Empty;
            statusStrip.Update();
        }

        private T PerformGhostscriptOperation<T>(Func<int[], GhostscriptProcessor, T> action)
        {
            // get the selected indices   
            var selected = new int[listViewPages.SelectedIndices.Count];
            listViewPages.SelectedIndices.CopyTo(selected, 0);
            Array.Sort(selected);

            // make sure the rasterizer is closed during the action and show the status elements
            rasterizer.Close();
            ShowStatus(true);
            try
            {
                // perform the action
                using (var processor = new GhostscriptProcessor(version, false))
                    return action(selected, processor);
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
                ShowStatus(false);
                rasterizer.Open(shortPath, version, false);
            }
        }

        private void ExtractRange(GhostscriptProcessor processor, bool appendFile, string path, int start, int end)
        {
            // make the range 1-based
            start++;
            end++;

            // set the progress and status label
            toolStripProgressBarExtract.Value = start;
            var pages = start == end ? start.ToString() : string.Format("{0}-{1}", start, end);
            if (appendFile)
            {
                if (toolStripStatusLabelExtract.Text.Length > 0)
                    toolStripStatusLabelExtract.Text += ",";
                toolStripStatusLabelExtract.Text += pages;
            }
            else
                toolStripStatusLabelExtract.Text = string.Format("{0} => {1}", pages, Path.GetFileNameWithoutExtension(path));
            statusStrip.Update();

            // extract the given range of pages
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

            // increment the progress and reset the label if necessary
            toolStripProgressBarExtract.Value = end;
            if (!appendFile)
            {
                toolStripStatusLabelExtract.Text = string.Empty;
                statusStrip.Update();
            }
        }

        private void ExtractToSingleDocument(string path)
        {
            PerformGhostscriptOperation((selected, processor) =>
            {
                // extract all continuous selections and combine them into one document
                var statusInfo = new StringBuilder();
                var files = new List<string>();
                try
                {
                    // find continuous selections
                    var start = 0;
                    for (var i = 1; i < selected.Length; i++)
                    {
                        // extract the previous range if there is a gap
                        if (selected[start] + (i - start) < selected[i])
                        {
                            var fileNameInner = Path.GetTempFileName();
                            files.Add(fileNameInner);
                            ExtractRange(processor, true, fileNameInner, selected[start], selected[i - 1]);
                            start = i;
                        }
                    }

                    // check whether previos ranges have been extracted
                    if (start > 0)
                    {
                        // extract the remaining range
                        var fileNameOuter = Path.GetTempFileName();
                        files.Add(fileNameOuter);
                        ExtractRange(processor, true, fileNameOuter, selected[start], selected[selected.Length - 1]);

                        // combine all pages
                        toolStripProgressBarExtract.Value = cache.Length;
                        toolStripStatusLabelExtract.Text += " => " + Path.GetFileNameWithoutExtension(path);
                        statusStrip.Update();
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
                    }
                    else
                        // simply extract the single range
                        ExtractRange(processor, false, path, selected[start], selected[selected.Length - 1]);

                    // done
                    toolStripProgressBarExtract.Value = toolStripProgressBarExtract.Maximum;
                    return true;
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
            });
        }

        private string GetPageFileName(int index)
        {
            // return the file name plus extension for a page
            return string.Format("{0}_{1}.pdf", fileName, index + 1);
        }

        private string[] ExtractToMultipleDocuments(string path)
        {
            return PerformGhostscriptOperation((selected, processor) =>
            {
                // extract all selected pages
                var fileNames = Array.ConvertAll(selected, i => GetPageFileName(i));
                for (var i = 0; i < selected.Length; i++)
                {
                    var index = selected[i];
                    ExtractRange(processor, false, Path.Combine(path, fileNames[i]), index, index);
                }
                return fileNames;
            });
        }

        private void ExtractForm_Load(object sender, EventArgs e)
        {
            // create the rasterizer, initialize the cache and virtual mode
            rasterizer = new GhostscriptRasterizer();
            rasterizer.Open(shortPath, version, false);
            cache = new Image[rasterizer.PageCount];
            listViewPages.VirtualListSize = rasterizer.PageCount;
            toolStripProgressBarExtract.Maximum = (int)(rasterizer.PageCount * 1.1f);
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

        }

        private void listViewPages_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {

        }

        private void toolStripDropDownButtonSave_Click(object sender, EventArgs e)
        {
            if (checkBoxSingleFiles.Checked)
            {
                // pick a directory and extract the pages
                if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                {
                    var files = ExtractToMultipleDocuments(folderBrowserDialog.SelectedPath);
                    Program.OpenFolderAndSelectItems(folderBrowserDialog.SelectedPath, files);
                }
            }
            else
            {
                // show the save dialog and extract the pages to a single document
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    ExtractToSingleDocument(saveFileDialog.FileName);
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
