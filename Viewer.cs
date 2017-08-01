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
using System.Text;
using System.Windows.Forms;
using Ghostscript.NET.Viewer;

namespace Aufbauwerk.Tools.PdfKit
{
    public partial class Viewer : UserControl
    {
        private static string GetShortPathName(string path)
        {
            // return the 8.3 version of the path
            var buffer = new StringBuilder(300);
            int len;
            while ((len = Native.GetShortPathNameW(path, buffer, buffer.Capacity)) > buffer.Capacity)
            {
                buffer.EnsureCapacity(len);
            }
            if (len == 0)
            {
                throw new Win32Exception();
            }
            return buffer.ToString();
        }

        private GhostscriptViewer _currentViewer = null;
        private Point? _previewDragLocation = null;
        private readonly string _totalPagesFormat;
        private readonly Dictionary<string, GhostscriptViewerState> _viewerStates = new Dictionary<string, GhostscriptViewerState>();

        public Viewer()
        {
            InitializeComponent();
            _totalPagesFormat = toolStripLabelTotal.Text;
            pictureBoxPreview.MouseWheel += new MouseEventHandler(pictureBoxPreview_MouseWheel);
            SyncStates();
        }

        [Bindable(true), Localizable(true)]
        public string Path
        {
            get
            {
                return _currentViewer == null ? string.Empty : _currentViewer.FilePath;
            }
            set
            {
                SetViewer(value);
            }
        }

        #region Methods

        private void DoScroll(ScrollProperties scrollProperties, int delta)
        {
            // perform the scroll operation if possible and within bounds
            if (scrollProperties.Enabled)
            {
                scrollProperties.Value = Math.Min(scrollProperties.Maximum, Math.Max(scrollProperties.Minimum, scrollProperties.Value - delta));
            }
        }

        private void SetViewer(string path, bool saveState = true)
        {
            // get the 8.3 path to circumvent unicode issues
            if (!string.IsNullOrEmpty(path))
            {
                path = GetShortPathName(path);
            }

            // remove the old viewer
            if (_currentViewer != null)
            {
                // do nothing if it's the same path
                if (path == _currentViewer.FilePath)
                {
                    return;
                }

                // unhook the viewer
                _currentViewer.DisplayPage -= currentViewer_DisplayPage;
                _currentViewer.DisplaySize -= currentViewer_DisplaySize;
                _currentViewer.DisplayUpdate -= currentViewer_DisplayUpdate;

                // clear the image
                pictureBoxPreview.Image = null;
                panelPreview.Update();

                // save the state and dispose of the viewer
                try
                {
                    if (saveState)
                    {
                        _viewerStates[_currentViewer.FilePath] = _currentViewer.SaveState();
                    }
                    _currentViewer.Dispose();
                }
                catch
                {
                    // don't care on the way out
                }

                // clear the viewer and update the controls
                _currentViewer = null;
                SyncStates();
            }

            // set the new viewer if a path is given
            if (!string.IsNullOrEmpty(path))
            {
                // create, hook and open the viewer and restore the state if possible
                _currentViewer = new GhostscriptViewer();
                _currentViewer.ShowPageAfterOpen = false;
                _currentViewer.DisplayUpdate += currentViewer_DisplayUpdate;
                _currentViewer.DisplaySize += currentViewer_DisplaySize;
                _currentViewer.DisplayPage += currentViewer_DisplayPage;
                try
                {
                    _currentViewer.Open(path, Program.GhostscriptVersion, false);
                    GhostscriptViewerState state;
                    if (_viewerStates.TryGetValue(_currentViewer.FilePath, out state))
                    {
                        _currentViewer.RestoreState(state);
                        _currentViewer.RefreshPage();
                    }
                    else
                    {
                        _currentViewer.ShowPage(_currentViewer.FirstPageNumber, true);
                    }
                }
                catch
                {
                    // clear the viewer and rethrow the error
                    SetViewer(null, false);
                    throw;
                }

                // update the controls
                SyncStates();
            }
        }

        private void SyncStates()
        {
            // update the controls
            toolStripButtonFirst.Enabled = _currentViewer != null && _currentViewer.CanShowFirstPage;
            toolStripButtonPrevious.Enabled = _currentViewer != null && _currentViewer.CanShowPreviousPage;
            toolStripButtonNext.Enabled = _currentViewer != null && _currentViewer.CanShowNextPage;
            toolStripButtonLast.Enabled = _currentViewer != null && _currentViewer.CanShowLastPage;
            toolStripButtonZoomIn.Enabled = _currentViewer != null && _currentViewer.CanZoomIn;
            toolStripButtonZoomOut.Enabled = _currentViewer != null && _currentViewer.CanZoomOut;
            toolStripTextBoxPage.Enabled = _currentViewer != null;
            toolStripTextBoxPage.Text = _currentViewer != null ? (_currentViewer.CurrentPageNumber - _currentViewer.FirstPageNumber + 1).ToString() : string.Empty;
            toolStripLabelTotal.Text = _currentViewer != null ? string.Format(_totalPagesFormat, _currentViewer.LastPageNumber - _currentViewer.FirstPageNumber + 1) : string.Empty;
        }

        #endregion

        #region Event Handlers

        private void currentViewer_DisplayPage(object sender, GhostscriptViewerViewEventArgs e)
        {
            // redraw the image and update the controls
            pictureBoxPreview.Invalidate();
            pictureBoxPreview.Update();
            SyncStates();
        }

        private void currentViewer_DisplaySize(object sender, GhostscriptViewerViewEventArgs e)
        {
            // set the image
            pictureBoxPreview.Image = e.Image;
            panelPreview.Update();
        }

        private void currentViewer_DisplayUpdate(object sender, GhostscriptViewerViewEventArgs e)
        {
            // redraw the image
            pictureBoxPreview.Invalidate();
            pictureBoxPreview.Update();
        }

        private void pictureBoxPreview_MouseDown(object sender, MouseEventArgs e)
        {
            // focus the picture and start draging
            pictureBoxPreview.Focus();
            _previewDragLocation = e.Location;
        }

        private void pictureBoxPreview_MouseMove(object sender, MouseEventArgs e)
        {
            // perform a scroll operation if the mouse is pressed
            if (_previewDragLocation.HasValue)
            {
                // get and clear the last location
                var lastLocation = _previewDragLocation.Value;
                _previewDragLocation = null;

                // convert the current location to screen coords and scroll
                var currentScreen = pictureBoxPreview.PointToScreen(e.Location);
                DoScroll(panelPreview.HorizontalScroll, e.Location.X - lastLocation.X);
                DoScroll(panelPreview.VerticalScroll, e.Location.Y - lastLocation.Y);

                // get the changed current location and store it
                _previewDragLocation = pictureBoxPreview.PointToClient(currentScreen);
            }
        }

        private void pictureBoxPreview_MouseUp(object sender, MouseEventArgs e)
        {
            // stop draging
            _previewDragLocation = null;
        }

        private void pictureBoxPreview_MouseWheel(object sender, MouseEventArgs e)
        {
            // zoom instead of scroll if CTRL is pressed
            if ((ModifierKeys & Keys.Control) != 0)
            {
                ((HandledMouseEventArgs)e).Handled = true;
                if (e.Delta > 0)
                {
                    toolStripButtonZoomIn.PerformClick();
                }
                if (e.Delta < 0)
                {
                    toolStripButtonZoomOut.PerformClick();
                }
            }
        }

        private void toolStripButtonFirst_Click(object sender, EventArgs e)
        {
            _currentViewer.ShowFirstPage();
        }

        private void toolStripButtonLast_Click(object sender, EventArgs e)
        {
            _currentViewer.ShowLastPage();
        }

        private void toolStripButtonNext_Click(object sender, EventArgs e)
        {
            _currentViewer.ShowNextPage();
        }

        private void toolStripButtonPrevious_Click(object sender, EventArgs e)
        {
            _currentViewer.ShowPreviousPage();
        }

        private void toolStripButtonZoomIn_Click(object sender, EventArgs e)
        {
            _currentViewer.ZoomIn();
        }

        private void toolStripButtonZoomOut_Click(object sender, EventArgs e)
        {
            _currentViewer.ZoomOut();
        }

        private void toolStripTextBoxPage_Validated(object sender, EventArgs e)
        {
            // parse the text and constrain the page number within the its bounds
            int page;
            if (int.TryParse(toolStripTextBoxPage.Text, out page))
            {
                page += _currentViewer.FirstPageNumber - 1;
                _currentViewer.ShowPage(Math.Min(_currentViewer.LastPageNumber, Math.Max(_currentViewer.FirstPageNumber, page)), true);
            }

            // always update the label
            SyncStates();
        }

        #endregion
    }
}
