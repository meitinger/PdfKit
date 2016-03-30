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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Ghostscript.NET;
using Ghostscript.NET.Processor;
using Ghostscript.NET.Viewer;

namespace Aufbauwerk.Tools.PdfKit
{
    public partial class CombineForm : Form
    {
        private readonly Dictionary<string, GhostscriptViewerState> viewerStates = new Dictionary<string, GhostscriptViewerState>();
        private readonly IEnumerable<string> initialFiles;
        private readonly string totalFormat;
        private GhostscriptViewer currentViewer = null;
        private Point? previewDrag = null;

        public CombineForm()
        {
            // initialize the components, hook the wheel event and set the controls
            InitializeComponent();
            totalFormat = toolStripLabelTotal.Text;
            pictureBoxPreview.MouseWheel += new MouseEventHandler(pictureBoxPreview_MouseWheel);
            UpdateFiles();
            UpdatePreview();
        }

        public CombineForm(IEnumerable<string> files)
            : this()
        {
            // store the initial files
            if (files == null)
                throw new ArgumentNullException("files");
            initialFiles = files;
        }

        public void AddFile(string path)
        {
            // add the given file if the list is enabled
            if (listViewFiles.Enabled)
                InsertPdfFile(path, listViewFiles.Items.Count);
        }

        private void SetViewer(string path)
        {
            // get the 8.3 path to circumvent non-ascii characters
            if (path != null)
                path = Program.GetShortPathName(path);

            // remove the old viewer
            if (currentViewer != null)
            {
                // do nothing if it's the same path
                if (currentViewer.FilePath == path)
                    return;

                // clear the image, save the state and dispose of the viewer
                pictureBoxPreview.Image = null;
                panelPreview.Update();
                try
                {
                    viewerStates[currentViewer.FilePath] = currentViewer.SaveState();
                    currentViewer.DisplayPage -= currentViewer_DisplayPage;
                    currentViewer.DisplaySize -= currentViewer_DisplaySize;
                    currentViewer.DisplayUpdate -= currentViewer_DisplayUpdate;
                    currentViewer.Dispose();
                }
                catch (GhostscriptException)
                {
                    // don't care on the way out
                }
                finally
                {
                    currentViewer = null;
                    UpdatePreview();
                }
            }

            // set the new viewer if a path is given
            if (path != null)
            {
                // create, hook and open the viewer and restore the state if possible
                currentViewer = new GhostscriptViewer();
                try
                {
                    currentViewer.GetType().GetProperty("ShowPageAfterOpen", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(currentViewer, false, null);
                    currentViewer.DisplayUpdate += currentViewer_DisplayUpdate;
                    currentViewer.DisplaySize += currentViewer_DisplaySize;
                    currentViewer.DisplayPage += currentViewer_DisplayPage;
                    currentViewer.Open(path, Program.GhostscriptVersion, false);
                    GhostscriptViewerState state;
                    if (viewerStates.TryGetValue(currentViewer.FilePath, out state))
                    {
                        currentViewer.RestoreState(state);
                        currentViewer.RefreshPage();
                    }
                    else
                        currentViewer.ShowPage(currentViewer.FirstPageNumber, true);
                }
                catch (GhostscriptException e)
                {
                    // show an error message
                    MessageBox.Show(e.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    UpdatePreview();
                }
            }
        }

        private ListViewItem InsertPdfFile(string path, int index)
        {
            // create the item
            SetViewer(path);
            var item = new ListViewItem(Path.GetFileName(path));
            item.SubItems.Add((currentViewer.LastPageNumber - currentViewer.FirstPageNumber + 1).ToString());
            item.SubItems.Add(Path.GetDirectoryName(path));
            item.Tag = path;

            // insert and select the item
            listViewFiles.Items.Insert(index, item);
            item.Focused = true;
            item.Selected = true;

            // redraw the list and update the controls
            UpdateFiles();
            return item;
        }

        private void RemovePdfFile(ListViewItem item)
        {
            // remove the item, redraw the list and update the controls
            item.Remove();
            UpdateFiles();
        }

        private void ClearSelectionAndViewer()
        {
            // clear the viewer
            SetViewer(null);

            // disable updates and deselect all items
            listViewFiles.ItemSelectionChanged -= listViewFiles_ItemSelectionChanged;
            try { listViewFiles.SelectedItems.Clear(); }
            finally { listViewFiles.ItemSelectionChanged += listViewFiles_ItemSelectionChanged; }

            // redraw the list and the controls
            UpdateFiles();
        }

        private void DoScroll(ScrollProperties scrollProperties, int delta)
        {
            // perform the scroll operation if possible and within bounds
            if (scrollProperties.Enabled)
                scrollProperties.Value = Math.Min(scrollProperties.Maximum, Math.Max(scrollProperties.Minimum, scrollProperties.Value - delta));
        }

        private void UpdateFiles()
        {
            // redraw the list
            listViewFiles.Update();

            // update the file controls
            toolStripButtonRemove.Enabled = listViewFiles.SelectedItems.Count > 0;
            toolStripButtonUp.Enabled = listViewFiles.SelectedItems.Count > 0 && !listViewFiles.SelectedItems.Contains(listViewFiles.Items[0]);
            toolStripButtonDown.Enabled = listViewFiles.SelectedItems.Count > 0 && !listViewFiles.SelectedItems.Contains(listViewFiles.Items[listViewFiles.Items.Count - 1]);
            toolStripFiles.Update();
        }

        private void UpdatePreview()
        {
            // update the preview controls
            toolStripButtonFirst.Enabled = currentViewer != null && currentViewer.CanShowFirstPage;
            toolStripButtonPrevious.Enabled = currentViewer != null && currentViewer.CanShowPreviousPage;
            toolStripButtonNext.Enabled = currentViewer != null && currentViewer.CanShowNextPage;
            toolStripButtonLast.Enabled = currentViewer != null && currentViewer.CanShowLastPage;
            toolStripButtonZoomIn.Enabled = currentViewer != null && currentViewer.CanZoomIn;
            toolStripButtonZoomOut.Enabled = currentViewer != null && currentViewer.CanZoomOut;
            toolStripTextBoxPage.Enabled = currentViewer != null;
            toolStripTextBoxPage.Text = currentViewer != null ? (currentViewer.CurrentPageNumber - currentViewer.FirstPageNumber + 1).ToString() : string.Empty;
            toolStripLabelTotal.Text = currentViewer != null ? string.Format(totalFormat, currentViewer.LastPageNumber - currentViewer.FirstPageNumber + 1) : string.Empty;
            toolStripPreview.Update();
        }

        private void UpdateOther(bool enabled)
        {
            // set the enabled state of all other controls
            toolStripButtonInsert.Enabled = enabled;
            toolStripFiles.Update();
            listViewFiles.Enabled = enabled;
            listViewFiles.Update();
            buttonOK.Enabled = enabled;
            buttonOK.Update();
            buttonCancel.Enabled = enabled;
            buttonCancel.Update();
            splitContainer.Enabled = enabled;
            splitContainer.Update();
            progressBarStatus.Visible = !enabled;
            progressBarStatus.Update();
        }

        private void ChangeSelectedIndices(int delta)
        {
            // disable updates
            listViewFiles.ItemSelectionChanged -= listViewFiles_ItemSelectionChanged;
            try
            {
                // get all selected items
                var selected = new ListViewItem[listViewFiles.SelectedItems.Count];
                listViewFiles.SelectedItems.CopyTo(selected, 0);

                // sort them by their position and store the indices
                Array.Sort(selected, (i1, i2) => i1.Index - i2.Index);
                var indices = Array.ConvertAll(selected, i => i.Index);

                // get the focused item
                var focused = listViewFiles.FocusedItem;

                // remove all selected items
                for (var i = indices.Length - 1; i >= 0; i--)
                    listViewFiles.Items.RemoveAt(indices[i]);

                // insert them again at the proper index
                for (var i = 0; i < selected.Length; i++)
                    listViewFiles.Items.Insert(indices[i] + delta, selected[i]);

                // restore the focues item
                listViewFiles.FocusedItem = focused;

                // redraw the list and the controls
                UpdateFiles();
            }
            finally
            {
                listViewFiles.ItemSelectionChanged += listViewFiles_ItemSelectionChanged;
            }
        }

        private void CombineForm_Shown(object sender, EventArgs e)
        {
            // insert all initial files
            if (initialFiles != null)
            {
                Update();
                var index = 0;
                foreach (var initialFile in initialFiles)
                    InsertPdfFile(initialFile, index++);
            }
        }

        private void currentViewer_DisplayUpdate(object sender, GhostscriptViewerViewEventArgs e)
        {
            // redraw the image
            pictureBoxPreview.Invalidate();
            pictureBoxPreview.Update();
        }

        private void currentViewer_DisplaySize(object sender, GhostscriptViewerViewEventArgs e)
        {
            // set the image
            pictureBoxPreview.Image = e.Image;
            panelPreview.Update();
        }

        private void currentViewer_DisplayPage(object sender, GhostscriptViewerViewEventArgs e)
        {
            // redraw the image and update the controls
            pictureBoxPreview.Invalidate();
            pictureBoxPreview.Update();
            UpdatePreview();
        }

        private void toolStripButtonFirst_Click(object sender, EventArgs e)
        {
            currentViewer.ShowFirstPage();
        }

        private void toolStripButtonPrevious_Click(object sender, EventArgs e)
        {
            currentViewer.ShowPreviousPage();
        }

        private void toolStripButtonNext_Click(object sender, EventArgs e)
        {
            currentViewer.ShowNextPage();
        }

        private void toolStripButtonLast_Click(object sender, EventArgs e)
        {
            currentViewer.ShowLastPage();
        }

        private void toolStripButtonZoomIn_Click(object sender, EventArgs e)
        {
            currentViewer.ZoomIn();
        }

        private void toolStripButtonZoomOut_Click(object sender, EventArgs e)
        {
            currentViewer.ZoomOut();
        }

        private void toolStripTextBoxPage_Validated(object sender, EventArgs e)
        {
            // parse the text and constrain the page number within the its bounds
            int page;
            if (int.TryParse(toolStripTextBoxPage.Text, out page))
            {
                page += currentViewer.FirstPageNumber - 1;
                currentViewer.ShowPage(Math.Min(currentViewer.LastPageNumber, Math.Max(currentViewer.FirstPageNumber, page)), true);
            }

            // always update the label
            UpdatePreview();
        }

        private void pictureBoxPreview_MouseWheel(object sender, MouseEventArgs e)
        {
            // zoom instead of scroll if CTRL is pressed
            if ((ModifierKeys & Keys.Control) != 0)
            {
                ((HandledMouseEventArgs)e).Handled = true;
                if (e.Delta > 0)
                    toolStripButtonZoomIn.PerformClick();
                if (e.Delta < 0)
                    toolStripButtonZoomOut.PerformClick();
            }
        }

        private void pictureBoxPreview_MouseDown(object sender, MouseEventArgs e)
        {
            // focus the picture and start draging
            pictureBoxPreview.Focus();
            previewDrag = e.Location;
        }

        private void pictureBoxPreview_MouseMove(object sender, MouseEventArgs e)
        {
            // perform a scroll operation if the mouse is pressed
            if (previewDrag.HasValue)
            {
                // get and clear the last location
                var lastLocation = previewDrag.Value;
                previewDrag = null;

                // convert the current location to screen coords and scroll
                var currentScreen = pictureBoxPreview.PointToScreen(e.Location);
                DoScroll(panelPreview.HorizontalScroll, e.Location.X - lastLocation.X);
                DoScroll(panelPreview.VerticalScroll, e.Location.Y - lastLocation.Y);

                // get the changed current location and store it
                previewDrag = pictureBoxPreview.PointToClient(currentScreen);
            }
        }

        private void pictureBoxPreview_MouseUp(object sender, MouseEventArgs e)
        {
            // stop draging
            previewDrag = null;
        }

        private void listViewFiles_DragDrop(object sender, DragEventArgs e)
        {
            // get the insertion mark
            var insertIndex = listViewFiles.InsertionMark.Index;
            if (listViewFiles.InsertionMark.AppearsAfterItem)
                insertIndex++;
            listViewFiles.InsertionMark.Index = -1;

            // quit the operation if we don't have a valid index
            if (insertIndex < 0)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            // make sure the effect is not none
            if (e.Effect == DragDropEffects.None)
                return;

            // get the file names
            var files = e.Data.GetData(DataFormats.FileDrop, false) as string[];
            if (files == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            // clear the selection and add all files
            ClearSelectionAndViewer();
            for (var i = 0; i < files.Length; i++)
                InsertPdfFile(files[i], insertIndex + i);
        }

        private void listViewFiles_DragOver(object sender, DragEventArgs e)
        {
            // determine what to do with the data
            DragDropEffects effect;
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                switch (e.KeyState & (4 + 8 + 32))
                {
                    case 0:
                        if ((e.AllowedEffect & DragDropEffects.Move) != 0)
                            effect = DragDropEffects.Move;
                        else if ((e.AllowedEffect & DragDropEffects.Copy) != 0)
                            effect = DragDropEffects.Copy;
                        else
                            effect = DragDropEffects.None;
                        break;
                    case 4: //SHIFT
                        effect = (e.AllowedEffect & DragDropEffects.Move) != 0 ? DragDropEffects.Move : DragDropEffects.None;
                        break;
                    case 8: //CTRL
                        effect = (e.AllowedEffect & DragDropEffects.Copy) != 0 ? DragDropEffects.Copy : DragDropEffects.None;
                        break;
                    default:
                        effect = DragDropEffects.None;
                        break;
                }
            }
            else
                effect = DragDropEffects.None;

            // get the insertion mask
            int index;
            bool showAfterItem;
            if (effect != DragDropEffects.None)
            {
                // find the nearest item
                var location = listViewFiles.PointToClient(new Point(e.X, e.Y));
                index = listViewFiles.InsertionMark.NearestIndex(location);
                if (index > -1)
                {
                    // determine whether to insert the data above or below
                    var rect = listViewFiles.GetItemRect(index);
                    showAfterItem = location.Y > rect.Top + rect.Height / 2;
                }
                else
                    // hide the mark if the cursor is above the moved item
                    showAfterItem = listViewFiles.Items.Count == 0;
            }
            else
            {
                // move the insertion index to an invalid region
                index = -1;
                showAfterItem = false;
            }

            // apply the values
            e.Effect = effect;
            listViewFiles.InsertionMark.Index = index;
            listViewFiles.InsertionMark.AppearsAfterItem = showAfterItem;
        }

        private void listViewFiles_DragLeave(object sender, EventArgs e)
        {
            // hide the insertion mask
            listViewFiles.InsertionMark.Index = -1;
            listViewFiles.InsertionMark.AppearsAfterItem = false;
        }

        private void listViewFiles_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // get all selected items
            var selected = new ListViewItem[listViewFiles.SelectedItems.Count];
            listViewFiles.SelectedItems.CopyTo(selected, 0);
            var files = Array.ConvertAll(selected, i => (string)i.Tag);

            // create the file drop object and start the drag & drop operation
            var data = new DataObject(DataFormats.FileDrop, files);
            if (listViewFiles.DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Move) == DragDropEffects.Move)
                for (var i = 0; i < selected.Length; i++)
                    RemovePdfFile(selected[i]);
        }

        private void listViewFiles_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // redraw the list and update the controls
            UpdateFiles();

            // set the viewer
            SetViewer(listViewFiles.SelectedItems.Count == 1 ? (string)listViewFiles.SelectedItems[0].Tag : null);
        }

        private void toolStripButtonInsert_Click(object sender, EventArgs e)
        {
            // show the dialog
            if (openFilesDialog.ShowDialog(this) == DialogResult.OK)
            {
                // get start index and chosen files
                var index = listViewFiles.Items.Count;
                var files = openFilesDialog.FileNames;

                // clear the selection and add the files
                ClearSelectionAndViewer();
                for (var i = 0; i < files.Length; i++)
                    InsertPdfFile(files[i], index + i);
            }
        }

        private void toolStripButtonRemove_Click(object sender, EventArgs e)
        {
            // remove all selected items
            var selected = new ListViewItem[listViewFiles.SelectedItems.Count];
            listViewFiles.SelectedItems.CopyTo(selected, 0);
            for (var i = 0; i < selected.Length; i++)
                RemovePdfFile(selected[i]);
        }

        private void toolStripButtonUp_Click(object sender, EventArgs e)
        {
            ChangeSelectedIndices(-1);
        }

        private void toolStripButtonDown_Click(object sender, EventArgs e)
        {
            ChangeSelectedIndices(+1);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // combine the pdfs
            using (var processor = new GhostscriptProcessor(Program.GhostscriptVersion, false))
                processor.Process((string[])e.Argument);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // restore the state and reselect the OK button
            UpdateOther(true);
            buttonOK.Focus();

            // handle the result
            if (e.Error != null)
            {
                // rethrow all non-Ghostscript errors
                if (!(e.Error is GhostscriptException))
                    throw e.Error;

                // show an error message
                MessageBox.Show(e.Error.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                // show the combined PDF
                Process.Start(saveFileDialog.FileName);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // show the save file dialog
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                // build the argument array
                var args = new string[]
                    {
                        "-q",
                        "-dSAFER",
                        "-dBATCH",
                        "-dNOPAUSE",
                        "-dNOPROMPT",
                        "-sDEVICE=pdfwrite",
                        "-dAutoRotatePages=/None",
                        "-sOutputFile=" + saveFileDialog.FileName
                    };
                var offset = args.Length;
                Array.Resize(ref args, offset + listViewFiles.Items.Count);
                for (var i = 0; i < listViewFiles.Items.Count; i++)
                    args[offset + i] = (string)listViewFiles.Items[i].Tag;

                // clear the selection and disable all elements
                ClearSelectionAndViewer();
                UpdateOther(false);

                // combine the documents
                backgroundWorker.RunWorkerAsync(args);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
