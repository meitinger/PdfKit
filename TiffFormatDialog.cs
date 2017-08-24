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
using System.IO;
using System.Windows.Forms;

namespace Aufbauwerk.Tools.PdfKit
{
    public partial class TiffFormatDialog : Aufbauwerk.Tools.PdfKit.ImageFormatDialog
    {
        private bool _adjustWidthSet = false;

        public TiffFormatDialog()
        {
            InitializeComponent();
        }

        public override string FileExtension
        {
            get { return "tif"; }
        }

        public override bool SupportsSingleFile
        {
            get { return true; }
        }

        private void FillScaleArguments(IList<string> args)
        {
            // add the scale arguments
            args.Add(string.Format(CultureInfo.InvariantCulture, "-dDownScaleFactor={0}", numericUpDownDownscaleFactor.Value));
            if (radioButtonCompressionNone.Checked)
            {
                args.Add("-sCompression=none");
            }
            if (radioButtonCompressionLzw.Checked)
            {
                args.Add("-sCompression=lzw");
            }
            if (radioButtonCompressionPack.Checked)
            {
                args.Add("-sCompression=pack");
            }
            if (buttonPostRenderProfile.Tag != null)
            {
                args.Add(string.Format(CultureInfo.InvariantCulture, "-sPostRenderProfile=\"{0}\"", buttonPostRenderProfile.Tag));
            }
        }

        protected override void FillArguments(IList<string> args)
        {
            // add the arguments
            if (radioButtonMonochrome.Checked)
            {
                if (radioButtonMonochromeUnscaled.Checked)
                {
                    if (radioButtonMonochromeLzw.Checked)
                    {
                        args.Add("-sDEVICE=tifflzw");
                    }
                    if (radioButtonMonochromePack.Checked)
                    {
                        args.Add("-sDEVICE=tiffpack");
                    }
                }
                if (radioButtonMonochromeScaled.Checked)
                {
                    args.Add("-sDEVICE=tiffscaled");
                    FillScaleArguments(args);
                }
                if (radioButtonMonochromeG3.Checked)
                {
                    if (radioButtonG3.Checked)
                    {
                        args.Add("-sDEVICE=tiffg3");
                    }
                    if (radioButtonG32D.Checked)
                    {
                        args.Add("-sDEVICE=tiffg32d");
                    }
                    if (radioButtonG3Crle.Checked)
                    {
                        args.Add("-sDEVICE=tiffcrle");
                    }
                }
                if (radioButtonMonochromeG4.Checked)
                {
                    args.Add("-sDEVICE=tiffg4");
                }
                args.Add(string.Format(CultureInfo.InvariantCulture, "-dMinFeatureSize={0}", numericUpDownMinFeatureSize.Value));
                if (radioButtonAdjustWidthOff.Checked)
                {
                    args.Add("-dAdjustWidth=0");
                }
                if (radioButtonAdjustWidthCommon.Checked)
                {
                    args.Add("-dAdjustWidth=1");
                }
                if (radioButtonAdjustWidthValue.Checked)
                {
                    args.Add(string.Format(CultureInfo.InvariantCulture, "-dAdjustWidth={0}", numericUpDownAdjustWidthValue.Value));
                }
            }
            if (radioButtonGrayscale.Checked)
            {
                if (checkBoxScaled.Checked)
                {
                    args.Add("-sDEVICE=tiffscaled8");
                    FillScaleArguments(args);
                }
                else
                {
                    args.Add("-sDEVICE=tiffgray");
                }
            }
            if (radioButtonRgb.Checked)
            {
                if (radioButtonRgb12.Checked)
                {
                    args.Add("-sDEVICE=tiff12nc");
                }
                if (radioButtonRgb24.Checked)
                {
                    if (checkBoxScaled.Checked)
                    {
                        args.Add("-sDEVICE=tiffscaled24");
                        FillScaleArguments(args);
                    }
                    else
                    {
                        args.Add("-sDEVICE=tiff24nc");
                    }
                }
                if (radioButtonRgb48.Checked)
                {
                    args.Add("-sDEVICE=tiff48nc");
                }
            }
            if (radioButtonCmyk.Checked)
            {
                if (radioButtonCmyk4.Checked)
                {
                    args.Add("-sDEVICE=tiffscaled4");
                    FillScaleArguments(args);
                }
                if (radioButtonCmyk32.Checked)
                {
                    if (checkBoxScaled.Checked)
                    {
                        args.Add("-sDEVICE=tiffscaled32");
                        FillScaleArguments(args);
                    }
                    else
                    {
                        args.Add("-sDEVICE=tiff32nc");
                    }
                }
                if (radioButtonCmyk64.Checked)
                {
                    args.Add("-sDEVICE=tiff64nc");
                }
            }
            args.Add(string.Format(CultureInfo.InvariantCulture, "-dMaxStripSize={0}", numericUpDownMaxStripSize.Value));
            args.Add(checkBoxFillOrder.Checked ? "-dFillOrder=1" : "-dFillOrder=2");
            args.Add(checkBoxUseBigTiff.Checked ? "-dUseBigTIFF=true" : "-dUseBigTIFF=false");
            args.Add(checkBoxTiffDateTime.Checked ? "-dTIFFDateTime=true" : "-dTIFFDateTime=false");
            base.FillArguments(args);
        }

        protected override void UpdateControls(object sender, EventArgs e)
        {
            base.UpdateControls(sender, e);

            // set the enabled states
            checkBoxScaled.Enabled =
                radioButtonMonochrome.Checked && radioButtonMonochromeScaled.Checked ||
                radioButtonGrayscale.Checked ||
                radioButtonRgb.Checked && radioButtonRgb24.Checked ||
                radioButtonCmyk.Checked && (radioButtonCmyk4.Checked || radioButtonCmyk32.Checked);
            if (radioButtonMonochrome.Checked && radioButtonMonochromeScaled.Checked || radioButtonCmyk.Checked && radioButtonCmyk4.Checked)
            {
                checkBoxScaled.Checked = true;
            }
            radioButtonMonochromeUnscaled.Enabled = radioButtonMonochrome.Checked;
            radioButtonMonochromeScaled.Enabled = radioButtonMonochrome.Checked;
            radioButtonMonochromeG3.Enabled = radioButtonMonochrome.Checked;
            radioButtonMonochromeG4.Enabled = radioButtonMonochrome.Checked;
            radioButtonMonochromeLzw.Enabled = radioButtonMonochromeUnscaled.Enabled && radioButtonMonochromeUnscaled.Checked;
            radioButtonMonochromePack.Enabled = radioButtonMonochromeUnscaled.Enabled && radioButtonMonochromeUnscaled.Checked;
            radioButtonG3.Enabled = radioButtonMonochromeG3.Enabled && radioButtonMonochromeG3.Checked;
            radioButtonG32D.Enabled = radioButtonMonochromeG3.Enabled && radioButtonMonochromeG3.Checked;
            radioButtonG3Crle.Enabled = radioButtonMonochromeG3.Enabled && radioButtonMonochromeG3.Checked;
            labelMinFeatureSize.Enabled = radioButtonMonochrome.Checked;
            numericUpDownMinFeatureSize.Enabled = radioButtonMonochrome.Checked;
            labelAdjustWidth.Enabled = radioButtonMonochrome.Checked;
            radioButtonAdjustWidthOff.Enabled = radioButtonMonochrome.Checked;
            radioButtonAdjustWidthCommon.Enabled = radioButtonMonochrome.Checked;
            radioButtonAdjustWidthValue.Enabled = radioButtonMonochrome.Checked;
            numericUpDownAdjustWidthValue.Enabled = radioButtonAdjustWidthValue.Enabled && radioButtonAdjustWidthValue.Checked;
            radioButtonRgb12.Enabled = radioButtonRgb.Checked;
            radioButtonRgb24.Enabled = radioButtonRgb.Checked;
            radioButtonRgb48.Enabled = radioButtonRgb.Checked;
            radioButtonCmyk4.Enabled = radioButtonCmyk.Checked;
            radioButtonCmyk32.Enabled = radioButtonCmyk.Checked;
            radioButtonCmyk64.Enabled = radioButtonCmyk.Checked;
            labelDownscaleFactor.Enabled = checkBoxScaled.Enabled && checkBoxScaled.Checked;
            numericUpDownDownscaleFactor.Enabled = checkBoxScaled.Enabled && checkBoxScaled.Checked;
            labelCompression.Enabled = checkBoxScaled.Enabled && checkBoxScaled.Checked;
            radioButtonCompressionNone.Enabled = checkBoxScaled.Enabled && checkBoxScaled.Checked;
            radioButtonCompressionLzw.Enabled = checkBoxScaled.Enabled && checkBoxScaled.Checked;
            radioButtonCompressionPack.Enabled = checkBoxScaled.Enabled && checkBoxScaled.Checked;
            labelPostRenderProfile.Enabled = checkBoxScaled.Enabled && checkBoxScaled.Checked;
            buttonPostRenderProfile.Enabled = checkBoxScaled.Enabled && checkBoxScaled.Checked;
            buttonPostRenderProfile.Text = buttonPostRenderProfile.Tag == null ? " " : Path.GetFileName((string)buttonPostRenderProfile.Tag);

            // set the width default
            if (!_adjustWidthSet)
            {
                if (sender == radioButtonAdjustWidthOff || sender == radioButtonAdjustWidthCommon || sender == radioButtonAdjustWidthValue)
                {
                    _adjustWidthSet = true;
                }
                else
                {
                    if (sender == radioButtonMonochromeG3 || sender == radioButtonMonochromeG4)
                    {
                        radioButtonAdjustWidthCommon.Checked = true;
                    }
                    if (sender == radioButtonMonochromeUnscaled || sender == radioButtonMonochromeScaled)
                    {
                        radioButtonAdjustWidthOff.Checked = true;
                    }
                    _adjustWidthSet = false;
                }
            }
        }

        private void buttonPostRenderProfile_Click(object sender, EventArgs e)
        {
            // query the file name
            var button = (sender as Button);
            openFileDialog.FileName = (string)button.Tag;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                button.Tag = openFileDialog.FileName;
                UpdateControls(sender, e);
            }
        }
    }
}
