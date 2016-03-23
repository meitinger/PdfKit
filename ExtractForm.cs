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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;
using System.Diagnostics;

namespace Aufbauwerk.Tools.PdfKit
{
    public partial class ExtractForm : Form
    {
        private readonly GhostscriptVersionInfo version = new GhostscriptVersionInfo("gsdll32.dll");
        private readonly Image[] cache;
        private readonly string fileName;
        private GhostscriptRasterizer rasterizer;
        private CheckBox checkBoxSingleFiles;
        private TrackBar trackBarZoom;
        private Image imagePreview;
        private Size sizePreview;

        public ExtractForm(string path)
        {
            // store the path and create the rasterizer
            fileName = Path.GetFileNameWithoutExtension(path);
            rasterizer = new GhostscriptRasterizer();
            rasterizer.Open(Program.GetShortPathName(path), version, false);
            cache = new Image[rasterizer.PageCount];
           
            // intialize the components
            InitializeComponent();
            InitializeAdditionalComponents();

            // set the text and default browse path
            Text = string.Format(Text, fileName);
            var directory = Path.GetDirectoryName(path);
            saveFileDialog.InitialDirectory = directory;
            folderBrowserDialog.SelectedPath = directory;

            // simulate a trackbar change and set the virtual list size
            trackBarZoom_ValueChanged(this, EventArgs.Empty);
            listViewPages.VirtualListSize = rasterizer.PageCount;
        }

        private void InitializeAdditionalComponents()
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

        private bool IsBetter(Size maxSize)
        {
            // remove the border from the max size and ensure its positive
            maxSize.Width -= 2;
            maxSize.Height -= 2;
            if (maxSize.Width <= 0 || maxSize.Height <= 0)
                return false;

            // calculate the factor and size
            var factor = Math.Min((float)maxSize.Width / (float)imagePreview.Size.Width, (float)maxSize.Height / (float)imagePreview.Size.Height);
            var size = new Size((int)(imagePreview.Size.Width * factor), (int)(imagePreview.Size.Height * factor));

            // test if the size is larger
            if (size.Width * size.Height > sizePreview.Width * sizePreview.Height)
            {
                sizePreview = size;
                return true;
            }
            else
                return false;
        }

        private void SetTooltip(ListViewItem item)
        {
            // check if a tooltip should be shown
            if (item != null)
            {
                // do nothing if already shown
                if (imagePreview == cache[item.Index])
                    return;

                // store the image, reset the size and get the bounds
                imagePreview = cache[item.Index];
                sizePreview = Size.Empty;
                var screenRect = Screen.FromControl(listViewPages).Bounds;
                var itemRect = listViewPages.RectangleToScreen(item.Bounds);

                // find the best sector
                var pointPreview = Point.Empty;
                if (IsBetter(new Size(itemRect.Left - screenRect.Left, screenRect.Height)))
                    pointPreview = new Point(itemRect.Left - sizePreview.Width, screenRect.Top + (screenRect.Height - sizePreview.Height) / 2);
                if (IsBetter(new Size(screenRect.Width, itemRect.Top - screenRect.Top)))
                    pointPreview = new Point(screenRect.Left + (screenRect.Width - sizePreview.Width) / 2, itemRect.Top - sizePreview.Height);
                if (IsBetter(new Size(screenRect.Right - itemRect.Right, screenRect.Height)))
                    pointPreview = new Point(itemRect.Right, screenRect.Top + (screenRect.Height - sizePreview.Height) / 2);
                if (IsBetter(new Size(screenRect.Width, screenRect.Bottom - itemRect.Bottom)))
                    pointPreview = new Point(screenRect.Left + (screenRect.Width - sizePreview.Width) / 2, itemRect.Bottom);

                // show the tooltip
                toolTipPreview.Show("-", listViewPages, listViewPages.PointToClient(pointPreview));
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
                var factor = Math.Min((float)icon.Size.Width / (float)image.Size.Width, (float)icon.Size.Height / (float)image.Size.Height);
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

        private void toolStripDropDownButtonSave_Click(object sender, EventArgs e)
        {
        }

        private void toolTipPreview_Draw(object sender, DrawToolTipEventArgs e)
        {
            // draw the background, image and border
            e.DrawBackground();
            var bounds = e.Bounds;
            bounds.Inflate(-1, 1);
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
    }
}
