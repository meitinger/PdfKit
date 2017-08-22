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
using System.Globalization;

namespace Aufbauwerk.Tools.PdfKit
{
    public partial class ImageFormatDialog : Aufbauwerk.Tools.PdfKit.FormatDialog
    {
        public ImageFormatDialog()
        {
            InitializeComponent();
        }

        protected override void FillArguments(IList<string> args)
        {
            // add the arguments
            if (checkBoxLinkedResolution.Checked)
            {
                args.Add(string.Format(CultureInfo.InvariantCulture, "-r{0}", numericUpDownResolutionX.Value));
            }
            else
            {
                args.Add(string.Format(CultureInfo.InvariantCulture, "-r{0}x{1}", numericUpDownResolutionX.Value, numericUpDownResolutionY.Value));
            }
            if (radioButtonTextAlphaBits1.Checked)
            {
                args.Add("-dTextAlphaBits=1");
            }
            if (radioButtonTextAlphaBits2.Checked)
            {
                args.Add("-dTextAlphaBits=2");
            }
            if (radioButtonTextAlphaBits4.Checked)
            {
                args.Add("-dTextAlphaBits=4");
            }
            if (radioButtonGraphicsAlphaBits1.Checked)
            {
                args.Add("-dGraphicsAlphaBits=1");
            }
            if (radioButtonGraphicsAlphaBits2.Checked)
            {
                args.Add("-dGraphicsAlphaBits=2");
            }
            if (radioButtonGraphicsAlphaBits4.Checked)
            {
                args.Add("-dGraphicsAlphaBits=4");
            }
            base.FillArguments(args);
        }

        protected override void UpdateControls(object sender, EventArgs e)
        {
            base.UpdateControls(sender, e);

            // link the resolutions
            if (checkBoxLinkedResolution.Checked)
            {
                if (sender == numericUpDownResolutionY)
                {
                    numericUpDownResolutionX.Value = numericUpDownResolutionY.Value;
                }
                else
                {
                    numericUpDownResolutionY.Value = numericUpDownResolutionX.Value;
                }
            }
        }
    }
}
