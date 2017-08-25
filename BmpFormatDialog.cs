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

using System.Collections.Generic;

namespace Aufbauwerk.Tools.PdfKit
{
    public partial class BmpFormatDialog : Aufbauwerk.Tools.PdfKit.ImageFormatDialog
    {
        public BmpFormatDialog()
        {
            InitializeComponent();
        }

        public override void FillArguments(IList<string> args)
        {
            if (radioButtonMonochrome.Checked)
            {
                args.Add("-sDEVICE=bmpmono");
            }
            if (radioButtonGrayscale.Checked)
            {
                args.Add("-sDEVICE=bmpgray");
            }
            if (radioButtonColor16.Checked)
            {
                args.Add("-sDEVICE=bmp16");
            }
            if (radioButtonColor256.Checked)
            {
                args.Add("-sDEVICE=bmp256");
            }
            if (radioButtonColor16m.Checked)
            {
                args.Add("-sDEVICE=bmp16m");
            }
            base.FillArguments(args);
        }
    }
}
