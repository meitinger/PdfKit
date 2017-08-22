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
using System.IO;
using System.Windows.Forms;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Aufbauwerk.Tools.PdfKit
{
    public partial class CombineForm : Form
    {
        private readonly IEnumerable<string> _initialFiles;
        private int _selectionChangeSuspensions = 0;

        public CombineForm()
        {
            // initialize the components, hook the wheel event and set the controls
            InitializeComponent();
            SetEnabledState(true);
        }

        public CombineForm(IEnumerable<string> files)
            : this()
        {
            // store the initial files
            if (files == null)
            {
                throw new ArgumentNullException("files");
            }
            _initialFiles = files;
        }

        #region Methods

        private void InsertFiles(IEnumerable<string> filePaths, int startIndex)
        {
            // ensure the insertion of files is allowed at the time
            if (!listViewFiles.Enabled)
            {
                throw new InvalidOperationException();
            }

            // insert the files
            DoWithSuspendedSelectionChange(() =>
            {
                // clear the selection
                listViewFiles.SelectedItems.Clear();
                HandleSelectionChange(true);
                Update();

                // insert all files
                foreach (var filePath in filePaths)
                {
                    // get the number of pages
                    int pageCount;
                Retry:
                    try
                    {
                        using (var document = PdfReader.Open(filePath, PdfDocumentOpenMode.InformationOnly))
                        {
                            pageCount = document.PageCount;
                        }
                    }
                    catch (Exception e)
                    {
                        // show a dialog and follow the user's choice
                        switch (MessageBox.Show(filePath + ":\n" + e.Message, Text, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning))
                        {
                            case DialogResult.Ignore:
                            {
                                continue;
                            }
                            case DialogResult.Retry:
                            {
                                goto Retry;
                            }
                            case DialogResult.Abort:
                            {
                                return;
                            }
                        }
                        throw;
                    }

                    // create the item
                    var item = new ListViewItem(Path.GetFileName(filePath));
                    item.Tag = new Tuple<string, int>(filePath, pageCount);
                    item.SubItems.Add(pageCount.ToString());
                    item.SubItems.Add(Path.GetDirectoryName(filePath));

                    // insert, select and focus the item
                    listViewFiles.Items.Insert(startIndex++, item);
                    item.EnsureVisible();
                    item.Selected = true;
                    item.Focused = true;
                    listViewFiles.Update();
                }
            });
        }

        private void RemoveFiles(ListViewItem[] items)
        {
            // remove the item, redraw the list and update the controls
            DoWithSuspendedSelectionChange(() =>
            {
                for (var i = 0; i < items.Length; i++)
                {
                    items[i].Remove();
                }
            });
        }

        private void ChangeSelectedIndices(int delta)
        {
            DoWithSuspendedSelectionChange(() =>
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
                {
                    listViewFiles.Items.RemoveAt(indices[i]);
                }

                // insert them again at the proper index
                for (var i = 0; i < selected.Length; i++)
                {
                    listViewFiles.Items.Insert(indices[i] + delta, selected[i]);
                }

                // restore the focues item
                listViewFiles.FocusedItem = focused;
            });
        }

        private void DoWithSuspendedSelectionChange(Action action)
        {
            // disable updates
            _selectionChangeSuspensions++;
            try
            {
                action();
            }
            finally
            {
                // enable updates and perform change
                _selectionChangeSuspensions--;
                HandleSelectionChange();
            }
        }

        private void HandleSelectionChange(bool force = false)
        {
            // only do something if no suspension is in place
            if (_selectionChangeSuspensions == 0 || force)
            {
                // update the controls
                SyncToolStrip();

                // set the viewer
                viewer.Path = listViewFiles.SelectedItems.Count == 1 ? ((Tuple<string, int>)listViewFiles.SelectedItems[0].Tag).Item1 : null;
            }
        }

        private void SetEnabledState(bool enabled)
        {
            // set the enabled state of all controls
            splitContainer.Enabled = enabled;
            toolStripDropDownButtonSave.Enabled = enabled;
            toolStripProgressBar.Visible = !enabled;
            toolStripProgressBar.Value = 0;
            toolStripDropDownButtonCancel.Visible = !enabled;
            listViewFiles.Enabled = enabled;
            SyncToolStrip();
        }

        private void SyncToolStrip()
        {
            // update the file controls
            toolStripButtonInsert.Enabled = listViewFiles.Enabled;
            toolStripButtonRemove.Enabled = listViewFiles.Enabled && listViewFiles.SelectedItems.Count > 0;
            toolStripButtonUp.Enabled = listViewFiles.Enabled && listViewFiles.SelectedItems.Count > 0 && !listViewFiles.SelectedItems.Contains(listViewFiles.Items[0]);
            toolStripButtonDown.Enabled = listViewFiles.Enabled && listViewFiles.SelectedItems.Count > 0 && !listViewFiles.SelectedItems.Contains(listViewFiles.Items[listViewFiles.Items.Count - 1]);
        }

        #endregion

        #region Event Handlers

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // get the arguments and create the combined document
            var worker = sender as BackgroundWorker;
            var allFiles = (Tuple<string, int>[])e.Argument;
            worker.ReportProgress(0);
            var prevProgress = 0;
            using (var combinedDocument = new PdfDocument())
            {
                // go over all input documents and keep track of the page count
                var currentPages = 0;
                var totalPages = allFiles[0].Item2;
                for (var docIndex = 1; docIndex < allFiles.Length; docIndex++)
                {
                    // quit if cancelled
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    // merge the next document
                    using (var document = PdfReader.Open(allFiles[docIndex].Item1, PdfDocumentOpenMode.Import))
                    {
                        // update the total pages (document might have been changed)
                        totalPages += document.PageCount - allFiles[docIndex].Item2;
                        for (var pageIndex = 0; pageIndex < document.PageCount; pageIndex++)
                        {
                            // quit if cancelled
                            if (worker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }

                            // import the page
                            combinedDocument.AddPage(document.Pages[pageIndex]);
                            currentPages++;

                            // report any further progress
                            var progress = (int)Math.Round((99.0 * currentPages) / totalPages);
                            if (progress != prevProgress)
                            {
                                worker.ReportProgress(progress);
                                prevProgress = progress;
                            }
                        }
                    }
                }

                // save the document
                combinedDocument.Save(allFiles[0].Item1);
            }
            worker.ReportProgress(100);

            // show the combined PDF
            Process.Start(allFiles[0].Item1);
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // set the progress bar
            toolStripProgressBar.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // restore the state
            SetEnabledState(true);

            // handle any potential error
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

        private void CombineForm_Shown(object sender, EventArgs e)
        {
            // insert all initial files
            if (_initialFiles != null)
            {
                Update();
                InsertFiles(_initialFiles, 0);
            }
        }

        private void listViewFiles_DragDrop(object sender, DragEventArgs e)
        {
            // get the insertion mark
            var insertIndex = listViewFiles.InsertionMark.Index;
            if (listViewFiles.InsertionMark.AppearsAfterItem)
            {
                insertIndex++;
            }
            listViewFiles.InsertionMark.Index = -1;

            // quit the operation if we don't have a valid index
            if (insertIndex < 0)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            // make sure the effect is not none
            if (e.Effect == DragDropEffects.None)
            {
                return;
            }

            // get the file names
            var files = e.Data.GetData(DataFormats.FileDrop, false) as string[];
            if (files == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            // insert the files
            InsertFiles(files, insertIndex);
        }

        private void listViewFiles_DragLeave(object sender, EventArgs e)
        {
            // hide the insertion mask
            listViewFiles.InsertionMark.Index = -1;
            listViewFiles.InsertionMark.AppearsAfterItem = false;
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
                    {
                        if ((e.AllowedEffect & DragDropEffects.Move) != 0)
                        {
                            effect = DragDropEffects.Move;
                        }
                        else if ((e.AllowedEffect & DragDropEffects.Copy) != 0)
                        {
                            effect = DragDropEffects.Copy;
                        }
                        else
                        {
                            effect = DragDropEffects.None;
                        }
                        break;
                    }
                    case 4: //SHIFT
                    {
                        effect = (e.AllowedEffect & DragDropEffects.Move) != 0 ? DragDropEffects.Move : DragDropEffects.None;
                        break;
                    }
                    case 8: //CTRL
                    {
                        effect = (e.AllowedEffect & DragDropEffects.Copy) != 0 ? DragDropEffects.Copy : DragDropEffects.None;
                        break;
                    }
                    default:
                    {
                        effect = DragDropEffects.None;
                        break;
                    }
                }
            }
            else
            {
                effect = DragDropEffects.None;
            }

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
                {
                    // hide the mark if the cursor is above the moved item
                    showAfterItem = listViewFiles.Items.Count == 0;
                }
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

        private void listViewFiles_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // get all selected items
            var selected = new ListViewItem[listViewFiles.SelectedItems.Count];
            listViewFiles.SelectedItems.CopyTo(selected, 0);
            var files = Array.ConvertAll(selected, i => ((Tuple<string, int>)i.Tag).Item1);

            // create the file drop object and start the drag & drop operation
            var data = new DataObject(DataFormats.FileDrop, files);
            if (listViewFiles.DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Move) == DragDropEffects.Move)
            {
                RemoveFiles(selected);
            }
        }

        private void listViewFiles_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            HandleSelectionChange();
        }

        private void toolStripButtonDown_Click(object sender, EventArgs e)
        {
            ChangeSelectedIndices(+1);
        }

        private void toolStripButtonInsert_Click(object sender, EventArgs e)
        {
            // show the dialog
            if (openFilesDialog.ShowDialog(this) == DialogResult.OK)
            {
                // insert the chosen files at the end
                InsertFiles(openFilesDialog.FileNames, listViewFiles.Items.Count);
            }
        }

        private void toolStripButtonRemove_Click(object sender, EventArgs e)
        {
            // remove all selected items
            var selected = new ListViewItem[listViewFiles.SelectedItems.Count];
            listViewFiles.SelectedItems.CopyTo(selected, 0);
            RemoveFiles(selected);
        }

        private void toolStripButtonUp_Click(object sender, EventArgs e)
        {
            ChangeSelectedIndices(-1);
        }

        private void toolStripDropDownButtonCancel_Click(object sender, EventArgs e)
        {
            // cancel the task
            backgroundWorker.CancelAsync();
        }

        private void toolStripDropDownButtonSave_Click(object sender, EventArgs e)
        {
            // show the save file dialog
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                // get all file paths and the total page count
                var allFiles = new Tuple<string, int>[1 + listViewFiles.Items.Count];
                var totalPages = 0;
                for (var i = 0; i < listViewFiles.Items.Count; i++)
                {
                    var file = (Tuple<string, int>)listViewFiles.Items[i].Tag;
                    allFiles[i + 1] = file;
                    totalPages += file.Item2;
                }
                allFiles[0] = new Tuple<string, int>(saveFileDialog.FileName, totalPages);

                // clear the selection and disable all elements
                SetEnabledState(false);

                // combine the documents
                backgroundWorker.RunWorkerAsync(allFiles);
            }
        }

        #endregion
    }
}
