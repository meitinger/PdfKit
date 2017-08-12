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
using System.Windows.Forms;
using PdfSharp.Pdf.IO;

namespace Aufbauwerk.Tools.PdfKit
{
    public partial class Viewer : UserControl
    {
        private class GhostscriptImage : Control
        {
            private Point? _dragLocation;
            private Image _image;
            private Size _imageSize;
            private Page _page;

            protected override Cursor DefaultCursor
            {
                get { return Cursors.NoMove2D; }
            }

            public Page Page
            {
                get { return _page; }
                set
                {
                    _page = value;
                    UpdateImage(null);
                }
            }

            private void DoScroll(ScrollProperties scrollProperties, int delta)
            {
                // perform the scroll operation if possible and within bounds
                if (scrollProperties.Enabled)
                {
                    scrollProperties.Value = Math.Min(scrollProperties.Maximum, Math.Max(scrollProperties.Minimum, scrollProperties.Value - delta));
                }
            }

            public override Size GetPreferredSize(Size proposedSize)
            {
                // use the image size
                return _imageSize;
            }

            private void SetImage(Image image)
            {
                // set a new image and issue a repaint
                _image = image;
                Size = _imageSize = image == null ? Size.Empty : image.Size;
                Invalidate();
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                // focus the picture and start draging
                Focus();
                _dragLocation = e.Location;
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                // perform a scroll operation if the mouse is pressed
                if (_dragLocation.HasValue)
                {
                    // get and clear the last location
                    var lastLocation = _dragLocation.Value;
                    _dragLocation = null;

                    // convert the current location to screen coords and scroll
                    var currentScreen = PointToScreen(e.Location);
                    DoScroll((Parent as ScrollableControl).HorizontalScroll, e.Location.X - lastLocation.X);
                    DoScroll((Parent as ScrollableControl).VerticalScroll, e.Location.Y - lastLocation.Y);

                    // get the changed current location and store it
                    _dragLocation = PointToClient(currentScreen);
                }
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                // stop draging
                _dragLocation = null;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (_image == null)
                {
                    e.Graphics.Clear(BackColor);
                }
                else if (_image.Tag == null)
                {
                    e.Graphics.DrawImage(_image, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
                }
                else
                {
                    lock (_image)
                    {
                        if ((bool)_image.Tag)
                        {
                            e.Graphics.DrawImage(_image, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
                        }
                    }
                }
            }

            internal void UpdateImage(Rectangle? area)
            {
                // check if the image has changed
                var newImage = _page != null ? _page.Image : null;
                if (newImage != _image)
                {
                    // simply set empty and complete images
                    if (newImage == null || newImage.Tag == null)
                    {
                        SetImage(newImage);
                    }
                    else
                    {
                        // set the preview image if it's valid or an empty image otherwise (should not happen)
                        lock (newImage)
                        {
                            SetImage((bool)newImage.Tag ? newImage : null);
                        }
                    }
                }
                else if (area.HasValue)
                {
                    // only invalidate the given area
                    Invalidate(area.Value);
                }
            }
        }

        private class Bookmark
        {
            public int PageNumber;
            public double Zoom;
        }

        private class Page
        {
            public string FilePath;
            public int PageNumber;
            public double Zoom;
            public volatile Image Image;
        }

        private const double ZoomMinimum = 10;
        private const double ZoomMaximum = 1600;
        private const double ZoomDefault = 100;
        private const double ZoomStep = 25;

        private readonly Color _backgroundColor;
        private readonly Dictionary<string, Bookmark> _bookmarks = new Dictionary<string, Bookmark>();
        private string _error;
        private string _filePath;
        private int _pageNumber;
        private int _totalPages;
        private readonly string _totalPagesFormat;
        private GhostscriptImage _view;
        private double _zoom;
        private readonly string _zoomFormat;

        public Viewer()
        {
            InitializeComponent();
            _view = new GhostscriptImage();
            panel.Controls.Add(_view);
            toolStripTextBoxPage.Size = new Size(toolStripTextBoxPage.Size.Width / 2, toolStripTextBoxPage.Size.Height);
            toolStripTextBoxZoom.Size = new Size(toolStripTextBoxZoom.Size.Width / 2, toolStripTextBoxZoom.Size.Height);
            _backgroundColor = panel.BackColor;
            _totalPagesFormat = toolStripLabelTotal.Text;
            _zoomFormat = toolStripTextBoxZoom.Text;
            SyncStates();
        }

        #region Properties

        [Browsable(false)]
        public bool IsReady
        {
            get
            {
                return _filePath != null && _totalPages > 0;
            }
        }

        [DefaultValue(null), Localizable(true), Bindable(true)]
        public string Path
        {
            get
            {
                return _filePath;
            }
            set
            {
                // check if the path has changed
                var newFilePath = string.IsNullOrEmpty(value) ? null : value;
                if (_filePath == newFilePath)
                {
                    return;
                }

                // clear everything first and redraw the control
                _filePath = null;
                _totalPages = 0;
                _error = null;
                if (backgroundWorker.IsBusy)
                {
                    backgroundWorker.CancelAsync();
                }
                SyncStates();
                Update();

                // set the new file
                if (newFilePath != null)
                {
                    // set the bookmarked variables
                    Bookmark bookmark;
                    var hasBookmark = _bookmarks.TryGetValue(newFilePath, out bookmark);
                    _pageNumber = hasBookmark ? bookmark.PageNumber : 1;
                    _zoom = hasBookmark ? bookmark.Zoom : ZoomDefault;

                    // set the path and already start rendering if possible
                    _filePath = newFilePath;
                    if (!backgroundWorker.IsBusy)
                    {
                        StartRenderIfNecessary();
                    }

                    // try to get the page count
                    var prevCursor = Cursor;
                    Cursor = Cursors.WaitCursor;
                    try
                    {
                        _totalPages = GetPageCount(newFilePath);
                    }
                    catch (Exception e)
                    {
                        _error = e.Message;
                    }
                    Cursor = prevCursor;

                    // sync the controls
                    SyncStates();
                }
            }
        }

        #endregion

        #region Methods

        private int GetPageCount(string fileName)
        {
            using (var doc = PdfReader.Open(_filePath, PdfDocumentOpenMode.InformationOnly))
            {
                return doc.PageCount;
            }
        }

        private void SetRenderAttribute<T>(ref T field, T value)
        {
            // only do something if the viewer is ready and a value changes
            if (!IsReady || object.Equals(field, value))
            {
                return;
            }

            // set the new value and sync the states
            field = value;
            SyncStates();

            // store the current view
            Bookmark bookmark;
            if (!_bookmarks.TryGetValue(_filePath, out bookmark))
            {
                _bookmarks.Add(_filePath, bookmark = new Bookmark());
            }
            bookmark.PageNumber = _pageNumber;
            bookmark.Zoom = _zoom;

            // render the page if not busy or cancel the current render
            if (!backgroundWorker.IsBusy)
            {
                StartRenderIfNecessary();
            }
            else
            {
                backgroundWorker.CancelAsync();
            }
        }

        private void SetRenderError(string error)
        {
            // only do something if the viewer is ready and the error changes
            if (!IsReady || _error == error)
            {
                return;
            }

            // set the new error and sync the states
            _error = error;
            SyncStates();
        }

        private void StartRenderIfNecessary()
        {
            // do nothing if the complete same page is rendered or has an error, or there is no file path
            if
            (
                _filePath == null ||
                _view.Page != null && _view.Page.FilePath == _filePath && _view.Page.PageNumber == _pageNumber && _view.Page.Zoom == _zoom &&
                (_view.Page.Image != null && _view.Page.Image.Tag == null || _error != null)
            )
            {
                return;
            }

            // start rendering the new page
            var page = new Page()
            {
                FilePath = _filePath,
                PageNumber = _pageNumber,
                Zoom = _zoom,
            };
            _view.Page = page;
            SetRenderError(null);
            backgroundWorker.RunWorkerAsync(page);
        }

        private void SyncStates()
        {
            // set the panel
            label.Text = _error ?? string.Empty;
            label.Visible = _filePath != null && _error != null;
            panel.BackColor = label.Visible ? label.BackColor : _backgroundColor;
            _view.Visible = _filePath != null && _error == null;

            // update the tool controls
            toolStripButtonFirst.Enabled = IsReady && _pageNumber > 1;
            toolStripButtonPrevious.Enabled = IsReady && _pageNumber > 1;
            toolStripTextBoxPage.Enabled = IsReady;
            toolStripTextBoxPage.Text = IsReady ? _pageNumber.ToString() : string.Empty;
            toolStripLabelTotal.Text = IsReady ? string.Format(_totalPagesFormat, _totalPages) : string.Empty;
            toolStripButtonNext.Enabled = IsReady && _pageNumber < _totalPages;
            toolStripButtonLast.Enabled = IsReady && _pageNumber < _totalPages;
            toolStripButtonZoomOut.Enabled = IsReady && _zoom > ZoomMinimum;
            toolStripButtonZoomIn.Enabled = IsReady && _zoom < ZoomMaximum;
            toolStripTextBoxZoom.Enabled = IsReady;
            toolStripTextBoxZoom.Text = IsReady ? string.Format(_zoomFormat, _zoom) : string.Empty;
        }

        #endregion

        #region Event Handlers

        private void backgroundWorkerRenderPage_DoWork(object sender, DoWorkEventArgs e)
        {
            // render the page
            var page = (Page)e.Argument;
            var worker = (sender as BackgroundWorker);
            page.Image = Ghostscript.RenderPage(page.FilePath, page.PageNumber, CurrentAutoScaleDimensions.Width, CurrentAutoScaleDimensions.Height, page.Zoom / 100, (image, area) =>
            {
                page.Image = image;
                worker.ReportProgress(-1, area);
            }, () =>
            {
                // check if a cancellation was requested
                if (worker.CancellationPending)
                {
                    // abort rendering
                    e.Cancel = true;
                    return true;
                }
                else
                {
                    // keep going
                    return false;
                }
            });
        }

        private void backgroundWorkerRenderPage_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // update the image
            _view.UpdateImage((Rectangle)e.UserState);
        }

        private void backgroundWorkerRenderPage_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // handle the different results
            if (!e.Cancelled)
            {
                // either set the error or update the image
                if (e.Error != null)
                {
                    SetRenderError(e.Error.Message);
                }
                else
                {
                    _view.UpdateImage(null);
                }
            }

            // rerender if necessary
            StartRenderIfNecessary();
        }

        private void panel_MouseWheel(object sender, MouseEventArgs e)
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
            SetRenderAttribute(ref _pageNumber, 1);
        }

        private void toolStripButtonLast_Click(object sender, EventArgs e)
        {
            SetRenderAttribute(ref _pageNumber, _totalPages);
        }

        private void toolStripButtonNext_Click(object sender, EventArgs e)
        {
            SetRenderAttribute(ref _pageNumber, Math.Min(_totalPages, _pageNumber + 1));
        }

        private void toolStripButtonPrevious_Click(object sender, EventArgs e)
        {
            SetRenderAttribute(ref _pageNumber, Math.Max(1, _pageNumber - 1));
        }

        private void toolStripButtonZoomIn_Click(object sender, EventArgs e)
        {
            SetRenderAttribute(ref _zoom, Math.Min(ZoomMaximum, _zoom + ZoomStep));
        }

        private void toolStripButtonZoomOut_Click(object sender, EventArgs e)
        {
            SetRenderAttribute(ref _zoom, Math.Max(ZoomMinimum, _zoom - ZoomStep));
        }

        private void toolStripTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // focus the panel if enter is pressed
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                panel.Focus();
            }
        }

        private void toolStripTextBoxPage_Validated(object sender, EventArgs e)
        {
            // parse the text and constrain the page number within its bounds
            int page;
            if (int.TryParse(toolStripTextBoxPage.Text, out page))
            {
                SetRenderAttribute(ref _pageNumber, Math.Max(1, Math.Min(_totalPages, page)));
            }
            SyncStates();
        }

        private void toolStripTextBoxZoom_Validated(object sender, EventArgs e)
        {
            // parse the text and constrain the zoom factor within its bounds
            double zoom;
            if (double.TryParse((sender as ToolStripTextBox).Text.Replace('%', ' '), out zoom))
            {
                SetRenderAttribute(ref _zoom, Math.Max(ZoomMinimum, Math.Min(ZoomMaximum, zoom)));
            }
            SyncStates();
        }

        #endregion
    }
}
