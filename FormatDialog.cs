/* Copyright (C) 2016-2021, Manuel Meitinger
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Aufbauwerk.Tools.PdfKit
{
    public partial class FormatDialog : Form
    {
        public FormatDialog()
        {
            InitializeComponent();
        }

        protected IDictionary<Control, object> States;

        public virtual void FillArguments(IList<string> args)
        {
        }

        protected virtual void RestoreState()
        {
            // restore the control values
            foreach (var entry in States)
            {
                if (entry.Key is RadioButton radioButton)
                {
                    radioButton.Checked = (bool)entry.Value;
                }
                else if (entry.Key is CheckBox checkBox)
                {
                    checkBox.Checked = (bool)entry.Value;
                }
                else if (entry.Key is NumericUpDown numericUpDown)
                {
                    numericUpDown.Value = (decimal)entry.Value;
                }
            }
        }

        protected virtual void SaveState()
        {
            // save the control values
            var controls = new Control[States.Count];
            States.Keys.CopyTo(controls, 0);
            foreach (var control in controls)
            {
                if (control is RadioButton radioButton)
                {
                    States[control] = radioButton.Checked;
                }
                else if (control is CheckBox checkBox)
                {
                    States[control] = checkBox.Checked;
                }
                else if (control is NumericUpDown numericUpDown)
                {
                    States[control] = numericUpDown.Value;
                }
            }
        }

        protected virtual void UpdateControls(object sender, EventArgs e)
        {
        }

        private void FormatDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            // save or restore the state
            if (DialogResult == DialogResult.OK)
            {
                SaveState();
            }
            else
            {
                RestoreState();
            }
        }

        private void FormatDialog_Load(object sender, EventArgs e)
        {
            // only initialize once
            if (States == null)
            {
                States = new Dictionary<Control, object>();

                // update all controls
                UpdateControls(sender, e);

                // enumerate over all controls
                var controls = new Queue<Control>();
                controls.Enqueue(this);
                while (controls.Count > 0)
                {
                    var controlCollection = controls.Dequeue().Controls;
                    for (var i = 0; i < controlCollection.Count; i++)
                    {
                        // enqueue the child control if it has children of its own
                        var control = controlCollection[i];
                        if (control.HasChildren)
                        {
                            controls.Enqueue(control);
                        }

                        // hookup check and value changes
                        if (control is RadioButton radioButton)
                        {
                            radioButton.CheckedChanged += UpdateControls;
                        }
                        else if (control is CheckBox checkBox)
                        {
                            checkBox.CheckedChanged += UpdateControls;
                        }
                        else if (control is NumericUpDown numericUpDown)
                        {
                            numericUpDown.ValueChanged += UpdateControls;
                        }
                        else
                        {
                            continue;
                        }

                        // add the control to the states
                        States.Add(control, null);
                    }
                }

                // save the initial state
                SaveState();

                // get the maximum table size and add the padding
                var groups = Controls.OfType<GroupBox>();
                var maxGroupWidth = groups.Select(g => g.Controls.OfType<TableLayoutPanel>().Single()).Select(t => t.Left * 2 + t.Width).Max();

                // order and size the groups
                var offset = Padding.Top;
                foreach (var group in groups)
                {
                    group.Dock = DockStyle.None;
                    group.Left = Padding.Left;
                    offset += group.Margin.Top;
                    group.Top = offset;
                    var table = group.Controls.OfType<TableLayoutPanel>().Single();
                    group.AutoSize = false;
                    group.Width = maxGroupWidth;
                    group.Height = table.Top + table.Height + table.Margin.Bottom + group.Padding.Top;
                    offset += group.Height + group.Margin.Bottom;
                }

                // include the buttons and set the client size
                offset += flowLayoutPanelButtons.Height + flowLayoutPanelButtons.Margin.Horizontal;
                offset += Padding.Bottom;
                ClientSize = new Size(maxGroupWidth + Padding.Horizontal, offset);
            }

            // center the dialog
            CenterToParent();
        }
    }
}
