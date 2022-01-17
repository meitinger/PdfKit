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
using System.Globalization;

namespace Aufbauwerk.Tools.PdfKit
{
    public partial class JpegFormatDialog : ImageFormatDialog
    {
        public JpegFormatDialog()
        {
            InitializeComponent();
        }

        public override void FillArguments(IList<string> args)
        {
            base.FillArguments(args);

            // add the arguments
            if (radioButtonGrayscale.Checked)
            {
                args.Add("-sDEVICE=jpeggray");
            }
            if (radioButtonColor.Checked)
            {
                args.Add("-sDEVICE=jpeg");
            }
            if (radioButtonJpegQuality.Checked)
            {
                args.Add(string.Format(CultureInfo.InvariantCulture, "-dJPEGQ={0}", Math.Round(numericUpDownJpegQuality.Value)));
            }
            if (radioButtonQFactor.Checked)
            {
                args.Add(string.Format(CultureInfo.InvariantCulture, "-dQFactor={0}", numericUpDownQFactor.Value));
            }
        }

        protected override void UpdateControls(object sender, EventArgs e)
        {
            base.UpdateControls(sender, e);

            // set the enabled states
            numericUpDownJpegQuality.Enabled = radioButtonJpegQuality.Checked;
            numericUpDownQFactor.Enabled = radioButtonQFactor.Checked;
        }
    }
}
