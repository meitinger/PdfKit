/* Copyright (C) 2016-2022, Manuel Meitinger
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
using System.Globalization;
using System.Windows.Forms;

namespace Aufbauwerk.Tools.PdfKit
{
    public partial class PngFormatDialog : ImageFormatDialog
    {
        public PngFormatDialog()
        {
            InitializeComponent();
        }

        public override void FillArguments(IList<string> args)
        {
            base.FillArguments(args);

            // add the arguments
            args.Add(string.Format(CultureInfo.InvariantCulture, "-dDownScaleFactor={0}", Math.Round(numericUpDownDownScaleFactor.Value)));
            if (radioButtonMonochrome.Checked)
            {
                args.Add("-sDEVICE=pngmonod");
                args.Add(string.Format(CultureInfo.InvariantCulture, "-dMinFeatureSize={0}", Math.Round(numericUpDownMinFeatureSize.Value)));
            }
            if (radioButtonGrayscale.Checked)
            {
                args.Add("-sDEVICE=pnggray");
            }
            if (radioButtonColor.Checked)
            {
                args.Add("-sDEVICE=png16m");
            }
            if (radioButtonAlpha.Checked)
            {
                args.Add("-sDEVICE=pngalpha");
                args.Add(string.Format(CultureInfo.InvariantCulture, "-dBackgroundColor=16#{0:X6}", buttonBackgroundColor.BackColor.ToArgb() & 0x00FFFFFF));
            }
        }

        protected override void RestoreState()
        {
            base.RestoreState();
            buttonBackgroundColor.BackColor = (Color)States[buttonBackgroundColor];
        }

        protected override void SaveState()
        {
            base.SaveState();
            States[buttonBackgroundColor] = buttonBackgroundColor.BackColor;
        }

        protected override void UpdateControls(object sender, EventArgs e)
        {
            base.UpdateControls(sender, e);

            // set the enabled states
            labelMinFeatureSize.Enabled = radioButtonMonochrome.Checked;
            numericUpDownMinFeatureSize.Enabled = radioButtonMonochrome.Checked;
            labelBackgroundColor.Enabled = radioButtonAlpha.Checked;
            buttonBackgroundColor.Enabled = radioButtonAlpha.Checked;

            // update the color button
            var colorValue = buttonBackgroundColor.BackColor.ToArgb() & 0x00FFFFFF;
            buttonBackgroundColor.ForeColor = Color.FromArgb(~colorValue);
            buttonBackgroundColor.Text = colorValue.ToString("X6");
        }

        private void ButtonBackgroundColor_Click(object sender, EventArgs e)
        {
            // show the color picker and update the control upon success
            var button = (sender as Button);
            colorDialog.Color = button.BackColor;
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
            {
                button.BackColor = colorDialog.Color;
                UpdateControls(sender, e);
            }
        }
    }
}
