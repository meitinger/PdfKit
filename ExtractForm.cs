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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Ghostscript.NET;

namespace Aufbauwerk.Tools.PdfKit
{
    public partial class ExtractForm : Form
    {
        private readonly GhostscriptVersionInfo version = new GhostscriptVersionInfo("gsdll32.dll");
        private readonly string path;
        private CheckBox checkBoxSingleFiles;
        private TrackBar trackBarZoom;

        public ExtractForm(string path)
        {
            // store the path
            this.path = path;
            InitializeComponent();
            InitializeAdditionalComponents();
            Text = string.Format(Text, Path.GetFileNameWithoutExtension(path));
        }

        private void InitializeAdditionalComponents()
        {
            // create the single file checkbox
            checkBoxSingleFiles = new CheckBox();
            var checkboxHost = new ToolStripControlHost(checkBoxSingleFiles);
            checkboxHost.Margin = checkboxHost.Margin + new Padding(8, 4, 0, 0);
            statusStrip.Items.Insert(statusStrip.Items.IndexOf(toolStripStatusLabelInfo), checkboxHost);
            toolStripStatusLabelInfo.Click += (s, e) => { checkBoxSingleFiles.Checked = !checkBoxSingleFiles.Checked; };

            // create the zoom trackbar
            trackBarZoom = new TrackBar();
            trackBarZoom.TickStyle = TickStyle.None;
            trackBarZoom.MaximumSize = new Size(100, 20);
            trackBarZoom.ValueChanged += trackBarZoom_ValueChanged;
            var trackbarHost = new ToolStripControlHost(trackBarZoom);
            trackbarHost.Alignment = ToolStripItemAlignment.Right;
            statusStrip.Items.Insert(statusStrip.Items.IndexOf(toolStripDropDownButtonZoomOut), trackbarHost);
        }

        private void trackBarZoom_ValueChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void listViewPages_VirtualItemsSelectionRangeChanged(object sender, ListViewVirtualItemsSelectionRangeChangedEventArgs e)
        {

        }

        private void listViewPages_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {

        }

        private void listViewPages_ItemDrag(object sender, ItemDragEventArgs e)
        {

        }
    }
}
