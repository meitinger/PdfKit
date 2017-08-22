using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aufbauwerk.Tools.PdfKit
{
    public partial class FormatDialog : Form
    {
        public FormatDialog()
        {
            InitializeComponent();
        }

        protected virtual void FillArguments(IList<string> args)
        {
        }

        protected virtual void UpdateControls(object sender, EventArgs e)
        {
            var args = new List<string>();
            FillArguments(args);
            System.Diagnostics.Debug.WriteLine(string.Join(" ", args));
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

            // get the maximum table size, add the padding and set the minimum client size
            var groups = Controls.OfType<GroupBox>();
            var maxGroupWidth = groups.Select(g => g.Controls.OfType<TableLayoutPanel>().Single()).Select(t => t.Left * 2 + t.Width).Max();
            this.MinimumSize = SizeFromClientSize(new Size(maxGroupWidth + Padding.Horizontal, 0));

            // order and size the groups
            var offset = 0;
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
        }

        public string[] Arguments { get; private set; }

        public bool EvenPages { get; set; }

        public bool OddPages { get; set; }

        public string FormatName { get; set; }
    }
}
