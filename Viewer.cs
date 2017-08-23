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

namespace Aufbauwerk.Tools.PdfKit
{
    public partial class Viewer : UserControl
    {
        private class GhostscriptImage : Control
        {
            private bool _doDrag;
            private Point _dragLocation;
            private Point _dragScroll;
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
                    if (_page != value)
                    {
                        _page = value;
                        SetImage(null);
                    }
                    UpdateImage();
                }
            }

            private void DoScroll(ScrollProperties scrollProperties, int value)
            {
                // perform the scroll operation if possible and within bounds
                if (scrollProperties.Enabled)
                {
                    scrollProperties.Value = Math.Min(scrollProperties.Maximum, Math.Max(scrollProperties.Minimum, value));
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
                _doDrag = true;
                _dragLocation = Cursor.Position;
                _dragScroll = new Point((Parent as ScrollableControl).HorizontalScroll.Value, (Parent as ScrollableControl).VerticalScroll.Value);
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                // perform a scroll operation if the mouse is pressed
                if (_doDrag)
                {
                    var location = Cursor.Position;
                    DoScroll((Parent as ScrollableControl).HorizontalScroll, _dragScroll.X + _dragLocation.X - location.X);
                    DoScroll((Parent as ScrollableControl).VerticalScroll, _dragScroll.Y + _dragLocation.Y - location.Y);
                }
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                // stop draging
                _doDrag = false;
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
                            _image.Tag = false;
                        }
                    }
                }
            }

            internal void UpdateImage()
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
                        // set the preview image if it's valid
                        lock (newImage)
                        {
                            if ((bool)newImage.Tag)
                            {
                                SetImage(newImage);
                            }
                        }
                    }
                }
                else
                {
                    // only invalidate the view
                    Invalidate();
                }
            }
        }

        private class Bookmark
        {
            public int PageNumber;
            public double Zoom;
            public int Rotate;
        }

        private class Page
        {
            public Document Document;
            public int PageNumber;
            public double Zoom;
            public int Rotate;
            public volatile Image Image;
        }

        private const double ZoomMinimum = 10;
        private const double ZoomMaximum = 1600;
        private const double ZoomDefault = 100;
        private const double ZoomStep = 25;

        private readonly Color _backgroundColor;
        private readonly Dictionary<string, Bookmark> _bookmarks = new Dictionary<string, Bookmark>();
        private Document _document;
        private string _error;
        private int _pageNumber;
        private int _rotate;
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

        [DefaultValue(null), Localizable(true), Bindable(true)]
        public string Path
        {
            get
            {
                return _document != null ? _document.FilePath : null;
            }
            set
            {
                // check if the path has changed
                var newFilePath = string.IsNullOrEmpty(value) ? null : value;
                if (Path == newFilePath)
                {
                    return;
                }

                // clear everything first and redraw the control
                if (_document != null)
                {
                    _document.Dispose();
                    _document = null;
                }
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

                    // try to open the document
                    var prevCursor = Cursor;
                    Cursor = Cursors.WaitCursor;
                    try
                    {
                        _document = Document.FromFile(newFilePath);
                    }
                    catch (Exception e)
                    {
                        _error = e.Message;
                    }
                    Cursor = prevCursor;

                    // set the bookmarked variables
                    Bookmark bookmark;
                    var hasBookmark = _bookmarks.TryGetValue(newFilePath, out bookmark);
                    _pageNumber = hasBookmark ? Math.Min(_document == null ? 1 : _document.PageCount, bookmark.PageNumber) : 1;
                    _zoom = hasBookmark ? bookmark.Zoom : ZoomDefault;
                    _rotate = hasBookmark ? bookmark.Rotate : 0;

                    // start rendering if possible
                    if (!backgroundWorker.IsBusy)
                    {
                        StartRenderIfNecessary();
                    }

                    // sync the controls
                    SyncStates();
                }
            }
        }

        #endregion

        #region Methods

        private void SetRenderAttribute<T>(ref T field, T value)
        {
            // only do something if the viewer is ready and a value changes
            if (_document == null || object.Equals(field, value))
            {
                return;
            }

            // set the new value and sync the states
            field = value;
            SyncStates();

            // store the current view
            Bookmark bookmark;
            if (!_bookmarks.TryGetValue(_document.FilePath, out bookmark))
            {
                _bookmarks.Add(_document.FilePath, bookmark = new Bookmark());
            }
            bookmark.PageNumber = _pageNumber;
            bookmark.Zoom = _zoom;
            bookmark.Rotate = _rotate;

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
            if (_document == null || _error == error)
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
                _document == null ||
                _view.Page != null && _view.Page.Document.FilePath == _document.FilePath && _view.Page.PageNumber == _pageNumber && _view.Page.Zoom == _zoom && _view.Page.Rotate == _rotate &&
                (_view.Page.Image != null && _view.Page.Image.Tag == null || _error != null)
            )
            {
                return;
            }

            // start rendering the new page
            var page = new Page()
            {
                Document = _document,
                PageNumber = _pageNumber,
                Zoom = _zoom,
                Rotate = _rotate,
            };
            _view.Page = page;
            SetRenderError(null);
            backgroundWorker.RunWorkerAsync(page);
        }

        private void SyncStates()
        {
            // set the panel
            label.Text = _error ?? string.Empty;
            label.Visible = _document != null && _error != null;
            panel.BackColor = label.Visible ? label.BackColor : _backgroundColor;
            _view.Visible = _document != null && _error == null;

            // update the tool controls
            toolStripButtonFirst.Enabled = _document != null && _pageNumber > 1;
            toolStripButtonPrevious.Enabled = _document != null && _pageNumber > 1;
            toolStripTextBoxPage.Enabled = _document != null;
            toolStripTextBoxPage.Text = _document != null ? _pageNumber.ToString() : string.Empty;
            toolStripLabelTotal.Text = _document != null ? string.Format(_totalPagesFormat, _document.PageCount) : string.Empty;
            toolStripButtonNext.Enabled = _document != null && _pageNumber < _document.PageCount;
            toolStripButtonLast.Enabled = _document != null && _pageNumber < _document.PageCount;
            toolStripButtonZoomOut.Enabled = _document != null && _zoom > ZoomMinimum;
            toolStripButtonZoomIn.Enabled = _document != null && _zoom < ZoomMaximum;
            toolStripTextBoxZoom.Enabled = _document != null;
            toolStripTextBoxZoom.Text = _document != null ? string.Format(_zoomFormat, _zoom) : string.Empty;
            toolStripButtonRotateLeft.Enabled = _document != null;
            toolStripButtonRotateRight.Enabled = _document != null;
        }

        #endregion

        #region Event Handlers

        private void backgroundWorkerRenderPage_DoWork(object sender, DoWorkEventArgs e)
        {
            // render the page
            var page = (Page)e.Argument;
            var worker = (sender as BackgroundWorker);
            page.Image = page.Document.RenderPage(page.PageNumber, CurrentAutoScaleDimensions.Width, CurrentAutoScaleDimensions.Height, page.Zoom / 100, page.Rotate, image =>
            {
                page.Image = image;
                worker.ReportProgress(-1);
            }, () =>
            {
                // check if a cancellation was requested
                if (worker.CancellationPending)
                {
                    // abort rendering
                    e.Cancel = true;
                    return true;
                }

                // keep going
                return false;
            });
        }

        private void backgroundWorkerRenderPage_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // update the image
            _view.UpdateImage();
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
                    _view.UpdateImage();
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
            SetRenderAttribute(ref _pageNumber, _document.PageCount);
        }

        private void toolStripButtonNext_Click(object sender, EventArgs e)
        {
            SetRenderAttribute(ref _pageNumber, Math.Min(_document.PageCount, _pageNumber + 1));
        }

        private void toolStripButtonPrevious_Click(object sender, EventArgs e)
        {
            SetRenderAttribute(ref _pageNumber, Math.Max(1, _pageNumber - 1));
        }

        private void toolStripButtonRotateLeft_Click(object sender, EventArgs e)
        {
            SetRenderAttribute(ref _rotate, _rotate + 90);
        }

        private void toolStripButtonRotateRight_Click(object sender, EventArgs e)
        {
            SetRenderAttribute(ref _rotate, _rotate - 90);
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
                SetRenderAttribute(ref _pageNumber, Math.Max(1, Math.Min(_document.PageCount, page)));
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
