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

        private IEnumerable<Control> AllChildren
        {
            get
            {
                // enumerate over all controls
                var controls = new Queue<Control>();
                controls.Enqueue(this);
                while (controls.Count > 0)
                {
                    var controlCollection = controls.Dequeue().Controls;
                    for (var i = 0; i < controlCollection.Count; i++)
                    {
                        var control = controlCollection[i];
                        yield return control;
                        if (control.HasChildren)
                        {
                            controls.Enqueue(control);
                        }
                    }
                }
            }
        }

        public bool SupportsSingleFile
        {
            set
            {
                // set the support
                groupBoxPages.Visible = value;
                radioButtonSingleFile.Checked = value;
                radioButtonMultipleFiles.Checked = !value;
            }
        }

        public bool UseSingleFile
        {
            get { return radioButtonSingleFile.Checked; }
        }

        public virtual void FillArguments(IList<string> args)
        {
        }

        protected virtual void UpdateControls(object sender, EventArgs e)
        {
        }

        private void FormatDialog_Load(object sender, EventArgs e)
        {
            // update all controls
            UpdateControls(sender, e);

            // hookup check and value changes
            foreach (var child in AllChildren)
            {
                if (child is RadioButton)
                {
                    ((RadioButton)child).CheckedChanged += UpdateControls;
                }
                else if (child is CheckBox)
                {
                    ((CheckBox)child).CheckedChanged += UpdateControls;
                }
                else if (child is NumericUpDown)
                {
                    ((NumericUpDown)child).ValueChanged += UpdateControls;
                }
            }

            // get the maximum table size and add the padding
            var groups = Controls.OfType<GroupBox>().Where(g => g.Visible);
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
    }
}
