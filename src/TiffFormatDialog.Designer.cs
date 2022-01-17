namespace Aufbauwerk.Tools.PdfKit
{
    partial class TiffFormatDialog
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.GroupBox groupBoxTiff;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TiffFormatDialog));
            System.Windows.Forms.TableLayoutPanel tableLayoutPanelRgb;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanelCmyk;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanelCompression;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanelMonochrome;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanelMonochromeUnscaled;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanelG3;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanelAdjustWidth;
            System.Windows.Forms.Label labelAdditional;
            System.Windows.Forms.Label labelMaxStripSize;
            this.tableLayoutPanelTiff = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonMonochrome = new System.Windows.Forms.RadioButton();
            this.radioButtonGrayscale = new System.Windows.Forms.RadioButton();
            this.radioButtonRgb = new System.Windows.Forms.RadioButton();
            this.radioButtonCmyk = new System.Windows.Forms.RadioButton();
            this.radioButtonRgb12 = new System.Windows.Forms.RadioButton();
            this.radioButtonRgb24 = new System.Windows.Forms.RadioButton();
            this.radioButtonRgb48 = new System.Windows.Forms.RadioButton();
            this.radioButtonCmyk4 = new System.Windows.Forms.RadioButton();
            this.radioButtonCmyk32 = new System.Windows.Forms.RadioButton();
            this.radioButtonCmyk64 = new System.Windows.Forms.RadioButton();
            this.checkBoxScaled = new System.Windows.Forms.CheckBox();
            this.labelDownscaleFactor = new System.Windows.Forms.Label();
            this.numericUpDownDownscaleFactor = new System.Windows.Forms.NumericUpDown();
            this.labelCompression = new System.Windows.Forms.Label();
            this.radioButtonCompressionNone = new System.Windows.Forms.RadioButton();
            this.radioButtonCompressionLzw = new System.Windows.Forms.RadioButton();
            this.radioButtonCompressionPack = new System.Windows.Forms.RadioButton();
            this.labelPostRenderProfile = new System.Windows.Forms.Label();
            this.buttonPostRenderProfile = new System.Windows.Forms.Button();
            this.radioButtonMonochromeUnscaled = new System.Windows.Forms.RadioButton();
            this.radioButtonMonochromeScaled = new System.Windows.Forms.RadioButton();
            this.radioButtonMonochromeG3 = new System.Windows.Forms.RadioButton();
            this.radioButtonMonochromeG4 = new System.Windows.Forms.RadioButton();
            this.radioButtonMonochromeLzw = new System.Windows.Forms.RadioButton();
            this.radioButtonMonochromePack = new System.Windows.Forms.RadioButton();
            this.radioButtonG3 = new System.Windows.Forms.RadioButton();
            this.radioButtonG32D = new System.Windows.Forms.RadioButton();
            this.radioButtonG3Crle = new System.Windows.Forms.RadioButton();
            this.labelMinFeatureSize = new System.Windows.Forms.Label();
            this.numericUpDownMinFeatureSize = new System.Windows.Forms.NumericUpDown();
            this.labelAdjustWidth = new System.Windows.Forms.Label();
            this.radioButtonAdjustWidthOff = new System.Windows.Forms.RadioButton();
            this.radioButtonAdjustWidthValue = new System.Windows.Forms.RadioButton();
            this.radioButtonAdjustWidthCommon = new System.Windows.Forms.RadioButton();
            this.numericUpDownAdjustWidthValue = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownMaxStripSize = new System.Windows.Forms.NumericUpDown();
            this.checkBoxFillOrder = new System.Windows.Forms.CheckBox();
            this.checkBoxUseBigTiff = new System.Windows.Forms.CheckBox();
            this.checkBoxTiffDateTime = new System.Windows.Forms.CheckBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            groupBoxTiff = new System.Windows.Forms.GroupBox();
            tableLayoutPanelRgb = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanelCmyk = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanelCompression = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanelMonochrome = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanelMonochromeUnscaled = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanelG3 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanelAdjustWidth = new System.Windows.Forms.TableLayoutPanel();
            labelAdditional = new System.Windows.Forms.Label();
            labelMaxStripSize = new System.Windows.Forms.Label();
            groupBoxTiff.SuspendLayout();
            this.tableLayoutPanelTiff.SuspendLayout();
            tableLayoutPanelRgb.SuspendLayout();
            tableLayoutPanelCmyk.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDownscaleFactor)).BeginInit();
            tableLayoutPanelCompression.SuspendLayout();
            tableLayoutPanelMonochrome.SuspendLayout();
            tableLayoutPanelMonochromeUnscaled.SuspendLayout();
            tableLayoutPanelG3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinFeatureSize)).BeginInit();
            tableLayoutPanelAdjustWidth.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAdjustWidthValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxStripSize)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxTiff
            // 
            resources.ApplyResources(groupBoxTiff, "groupBoxTiff");
            groupBoxTiff.Controls.Add(this.tableLayoutPanelTiff);
            groupBoxTiff.Name = "groupBoxTiff";
            groupBoxTiff.TabStop = false;
            // 
            // tableLayoutPanelTiff
            // 
            resources.ApplyResources(this.tableLayoutPanelTiff, "tableLayoutPanelTiff");
            this.tableLayoutPanelTiff.Controls.Add(this.radioButtonMonochrome, 0, 0);
            this.tableLayoutPanelTiff.Controls.Add(this.radioButtonGrayscale, 0, 3);
            this.tableLayoutPanelTiff.Controls.Add(this.radioButtonRgb, 0, 4);
            this.tableLayoutPanelTiff.Controls.Add(this.radioButtonCmyk, 0, 5);
            this.tableLayoutPanelTiff.Controls.Add(tableLayoutPanelRgb, 1, 4);
            this.tableLayoutPanelTiff.Controls.Add(tableLayoutPanelCmyk, 1, 5);
            this.tableLayoutPanelTiff.Controls.Add(this.checkBoxScaled, 0, 6);
            this.tableLayoutPanelTiff.Controls.Add(this.labelDownscaleFactor, 1, 6);
            this.tableLayoutPanelTiff.Controls.Add(this.numericUpDownDownscaleFactor, 2, 6);
            this.tableLayoutPanelTiff.Controls.Add(this.labelCompression, 1, 7);
            this.tableLayoutPanelTiff.Controls.Add(tableLayoutPanelCompression, 2, 7);
            this.tableLayoutPanelTiff.Controls.Add(this.labelPostRenderProfile, 1, 8);
            this.tableLayoutPanelTiff.Controls.Add(this.buttonPostRenderProfile, 2, 8);
            this.tableLayoutPanelTiff.Controls.Add(tableLayoutPanelMonochrome, 1, 0);
            this.tableLayoutPanelTiff.Controls.Add(this.labelMinFeatureSize, 1, 1);
            this.tableLayoutPanelTiff.Controls.Add(this.numericUpDownMinFeatureSize, 2, 1);
            this.tableLayoutPanelTiff.Controls.Add(this.labelAdjustWidth, 1, 2);
            this.tableLayoutPanelTiff.Controls.Add(tableLayoutPanelAdjustWidth, 2, 2);
            this.tableLayoutPanelTiff.Controls.Add(labelAdditional, 0, 9);
            this.tableLayoutPanelTiff.Controls.Add(labelMaxStripSize, 1, 9);
            this.tableLayoutPanelTiff.Controls.Add(this.numericUpDownMaxStripSize, 2, 9);
            this.tableLayoutPanelTiff.Controls.Add(this.checkBoxFillOrder, 1, 10);
            this.tableLayoutPanelTiff.Controls.Add(this.checkBoxUseBigTiff, 1, 11);
            this.tableLayoutPanelTiff.Controls.Add(this.checkBoxTiffDateTime, 1, 12);
            this.tableLayoutPanelTiff.Name = "tableLayoutPanelTiff";
            // 
            // radioButtonMonochrome
            // 
            resources.ApplyResources(this.radioButtonMonochrome, "radioButtonMonochrome");
            this.radioButtonMonochrome.Name = "radioButtonMonochrome";
            this.tableLayoutPanelTiff.SetRowSpan(this.radioButtonMonochrome, 3);
            this.radioButtonMonochrome.UseVisualStyleBackColor = true;
            // 
            // radioButtonGrayscale
            // 
            resources.ApplyResources(this.radioButtonGrayscale, "radioButtonGrayscale");
            this.radioButtonGrayscale.Name = "radioButtonGrayscale";
            this.radioButtonGrayscale.UseVisualStyleBackColor = true;
            // 
            // radioButtonRgb
            // 
            resources.ApplyResources(this.radioButtonRgb, "radioButtonRgb");
            this.radioButtonRgb.Checked = true;
            this.radioButtonRgb.Name = "radioButtonRgb";
            this.radioButtonRgb.TabStop = true;
            this.radioButtonRgb.UseVisualStyleBackColor = true;
            // 
            // radioButtonCmyk
            // 
            resources.ApplyResources(this.radioButtonCmyk, "radioButtonCmyk");
            this.radioButtonCmyk.Name = "radioButtonCmyk";
            this.radioButtonCmyk.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelRgb
            // 
            resources.ApplyResources(tableLayoutPanelRgb, "tableLayoutPanelRgb");
            this.tableLayoutPanelTiff.SetColumnSpan(tableLayoutPanelRgb, 2);
            tableLayoutPanelRgb.Controls.Add(this.radioButtonRgb12, 0, 0);
            tableLayoutPanelRgb.Controls.Add(this.radioButtonRgb24, 1, 0);
            tableLayoutPanelRgb.Controls.Add(this.radioButtonRgb48, 2, 0);
            tableLayoutPanelRgb.Name = "tableLayoutPanelRgb";
            // 
            // radioButtonRgb12
            // 
            resources.ApplyResources(this.radioButtonRgb12, "radioButtonRgb12");
            this.radioButtonRgb12.Name = "radioButtonRgb12";
            this.radioButtonRgb12.UseVisualStyleBackColor = true;
            // 
            // radioButtonRgb24
            // 
            resources.ApplyResources(this.radioButtonRgb24, "radioButtonRgb24");
            this.radioButtonRgb24.Checked = true;
            this.radioButtonRgb24.Name = "radioButtonRgb24";
            this.radioButtonRgb24.TabStop = true;
            this.radioButtonRgb24.UseVisualStyleBackColor = true;
            // 
            // radioButtonRgb48
            // 
            resources.ApplyResources(this.radioButtonRgb48, "radioButtonRgb48");
            this.radioButtonRgb48.Name = "radioButtonRgb48";
            this.radioButtonRgb48.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelCmyk
            // 
            resources.ApplyResources(tableLayoutPanelCmyk, "tableLayoutPanelCmyk");
            this.tableLayoutPanelTiff.SetColumnSpan(tableLayoutPanelCmyk, 2);
            tableLayoutPanelCmyk.Controls.Add(this.radioButtonCmyk4, 0, 0);
            tableLayoutPanelCmyk.Controls.Add(this.radioButtonCmyk32, 1, 0);
            tableLayoutPanelCmyk.Controls.Add(this.radioButtonCmyk64, 2, 0);
            tableLayoutPanelCmyk.Name = "tableLayoutPanelCmyk";
            // 
            // radioButtonCmyk4
            // 
            resources.ApplyResources(this.radioButtonCmyk4, "radioButtonCmyk4");
            this.radioButtonCmyk4.Name = "radioButtonCmyk4";
            this.radioButtonCmyk4.UseVisualStyleBackColor = true;
            // 
            // radioButtonCmyk32
            // 
            resources.ApplyResources(this.radioButtonCmyk32, "radioButtonCmyk32");
            this.radioButtonCmyk32.Checked = true;
            this.radioButtonCmyk32.Name = "radioButtonCmyk32";
            this.radioButtonCmyk32.TabStop = true;
            this.radioButtonCmyk32.UseVisualStyleBackColor = true;
            // 
            // radioButtonCmyk64
            // 
            resources.ApplyResources(this.radioButtonCmyk64, "radioButtonCmyk64");
            this.radioButtonCmyk64.Name = "radioButtonCmyk64";
            this.radioButtonCmyk64.UseVisualStyleBackColor = true;
            // 
            // checkBoxScaled
            // 
            resources.ApplyResources(this.checkBoxScaled, "checkBoxScaled");
            this.checkBoxScaled.Checked = true;
            this.checkBoxScaled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxScaled.Name = "checkBoxScaled";
            this.tableLayoutPanelTiff.SetRowSpan(this.checkBoxScaled, 3);
            this.checkBoxScaled.UseVisualStyleBackColor = true;
            // 
            // labelDownscaleFactor
            // 
            resources.ApplyResources(this.labelDownscaleFactor, "labelDownscaleFactor");
            this.labelDownscaleFactor.Name = "labelDownscaleFactor";
            // 
            // numericUpDownDownscaleFactor
            // 
            resources.ApplyResources(this.numericUpDownDownscaleFactor, "numericUpDownDownscaleFactor");
            this.numericUpDownDownscaleFactor.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericUpDownDownscaleFactor.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownDownscaleFactor.Name = "numericUpDownDownscaleFactor";
            this.numericUpDownDownscaleFactor.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // labelCompression
            // 
            resources.ApplyResources(this.labelCompression, "labelCompression");
            this.labelCompression.Name = "labelCompression";
            // 
            // tableLayoutPanelCompression
            // 
            resources.ApplyResources(tableLayoutPanelCompression, "tableLayoutPanelCompression");
            tableLayoutPanelCompression.Controls.Add(this.radioButtonCompressionNone, 0, 0);
            tableLayoutPanelCompression.Controls.Add(this.radioButtonCompressionLzw, 1, 0);
            tableLayoutPanelCompression.Controls.Add(this.radioButtonCompressionPack, 2, 0);
            tableLayoutPanelCompression.Name = "tableLayoutPanelCompression";
            // 
            // radioButtonCompressionNone
            // 
            resources.ApplyResources(this.radioButtonCompressionNone, "radioButtonCompressionNone");
            this.radioButtonCompressionNone.Name = "radioButtonCompressionNone";
            this.radioButtonCompressionNone.UseVisualStyleBackColor = true;
            // 
            // radioButtonCompressionLzw
            // 
            resources.ApplyResources(this.radioButtonCompressionLzw, "radioButtonCompressionLzw");
            this.radioButtonCompressionLzw.Checked = true;
            this.radioButtonCompressionLzw.Name = "radioButtonCompressionLzw";
            this.radioButtonCompressionLzw.TabStop = true;
            this.radioButtonCompressionLzw.UseVisualStyleBackColor = true;
            // 
            // radioButtonCompressionPack
            // 
            resources.ApplyResources(this.radioButtonCompressionPack, "radioButtonCompressionPack");
            this.radioButtonCompressionPack.Name = "radioButtonCompressionPack";
            this.radioButtonCompressionPack.UseVisualStyleBackColor = true;
            // 
            // labelPostRenderProfile
            // 
            resources.ApplyResources(this.labelPostRenderProfile, "labelPostRenderProfile");
            this.labelPostRenderProfile.Name = "labelPostRenderProfile";
            // 
            // buttonPostRenderProfile
            // 
            resources.ApplyResources(this.buttonPostRenderProfile, "buttonPostRenderProfile");
            this.buttonPostRenderProfile.Name = "buttonPostRenderProfile";
            this.buttonPostRenderProfile.UseVisualStyleBackColor = true;
            this.buttonPostRenderProfile.Click += new System.EventHandler(this.ButtonPostRenderProfile_Click);
            // 
            // tableLayoutPanelMonochrome
            // 
            resources.ApplyResources(tableLayoutPanelMonochrome, "tableLayoutPanelMonochrome");
            this.tableLayoutPanelTiff.SetColumnSpan(tableLayoutPanelMonochrome, 2);
            tableLayoutPanelMonochrome.Controls.Add(this.radioButtonMonochromeUnscaled, 0, 0);
            tableLayoutPanelMonochrome.Controls.Add(this.radioButtonMonochromeScaled, 0, 1);
            tableLayoutPanelMonochrome.Controls.Add(this.radioButtonMonochromeG3, 0, 2);
            tableLayoutPanelMonochrome.Controls.Add(this.radioButtonMonochromeG4, 0, 3);
            tableLayoutPanelMonochrome.Controls.Add(tableLayoutPanelMonochromeUnscaled, 1, 0);
            tableLayoutPanelMonochrome.Controls.Add(tableLayoutPanelG3, 1, 2);
            tableLayoutPanelMonochrome.Name = "tableLayoutPanelMonochrome";
            // 
            // radioButtonMonochromeUnscaled
            // 
            resources.ApplyResources(this.radioButtonMonochromeUnscaled, "radioButtonMonochromeUnscaled");
            this.radioButtonMonochromeUnscaled.Name = "radioButtonMonochromeUnscaled";
            this.radioButtonMonochromeUnscaled.UseVisualStyleBackColor = true;
            // 
            // radioButtonMonochromeScaled
            // 
            resources.ApplyResources(this.radioButtonMonochromeScaled, "radioButtonMonochromeScaled");
            this.radioButtonMonochromeScaled.Checked = true;
            this.radioButtonMonochromeScaled.Name = "radioButtonMonochromeScaled";
            this.radioButtonMonochromeScaled.TabStop = true;
            this.radioButtonMonochromeScaled.UseVisualStyleBackColor = true;
            // 
            // radioButtonMonochromeG3
            // 
            resources.ApplyResources(this.radioButtonMonochromeG3, "radioButtonMonochromeG3");
            this.radioButtonMonochromeG3.Name = "radioButtonMonochromeG3";
            this.radioButtonMonochromeG3.UseVisualStyleBackColor = true;
            // 
            // radioButtonMonochromeG4
            // 
            resources.ApplyResources(this.radioButtonMonochromeG4, "radioButtonMonochromeG4");
            this.radioButtonMonochromeG4.Name = "radioButtonMonochromeG4";
            this.radioButtonMonochromeG4.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelMonochromeUnscaled
            // 
            resources.ApplyResources(tableLayoutPanelMonochromeUnscaled, "tableLayoutPanelMonochromeUnscaled");
            tableLayoutPanelMonochromeUnscaled.Controls.Add(this.radioButtonMonochromeLzw, 0, 0);
            tableLayoutPanelMonochromeUnscaled.Controls.Add(this.radioButtonMonochromePack, 1, 0);
            tableLayoutPanelMonochromeUnscaled.Name = "tableLayoutPanelMonochromeUnscaled";
            // 
            // radioButtonMonochromeLzw
            // 
            resources.ApplyResources(this.radioButtonMonochromeLzw, "radioButtonMonochromeLzw");
            this.radioButtonMonochromeLzw.Checked = true;
            this.radioButtonMonochromeLzw.Name = "radioButtonMonochromeLzw";
            this.radioButtonMonochromeLzw.TabStop = true;
            this.radioButtonMonochromeLzw.UseVisualStyleBackColor = true;
            // 
            // radioButtonMonochromePack
            // 
            resources.ApplyResources(this.radioButtonMonochromePack, "radioButtonMonochromePack");
            this.radioButtonMonochromePack.Name = "radioButtonMonochromePack";
            this.radioButtonMonochromePack.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelG3
            // 
            resources.ApplyResources(tableLayoutPanelG3, "tableLayoutPanelG3");
            tableLayoutPanelG3.Controls.Add(this.radioButtonG3, 0, 0);
            tableLayoutPanelG3.Controls.Add(this.radioButtonG32D, 1, 0);
            tableLayoutPanelG3.Controls.Add(this.radioButtonG3Crle, 2, 0);
            tableLayoutPanelG3.Name = "tableLayoutPanelG3";
            // 
            // radioButtonG3
            // 
            resources.ApplyResources(this.radioButtonG3, "radioButtonG3");
            this.radioButtonG3.Checked = true;
            this.radioButtonG3.Name = "radioButtonG3";
            this.radioButtonG3.TabStop = true;
            this.radioButtonG3.UseVisualStyleBackColor = true;
            // 
            // radioButtonG32D
            // 
            resources.ApplyResources(this.radioButtonG32D, "radioButtonG32D");
            this.radioButtonG32D.Name = "radioButtonG32D";
            this.radioButtonG32D.UseVisualStyleBackColor = true;
            // 
            // radioButtonG3Crle
            // 
            resources.ApplyResources(this.radioButtonG3Crle, "radioButtonG3Crle");
            this.radioButtonG3Crle.Name = "radioButtonG3Crle";
            this.radioButtonG3Crle.UseVisualStyleBackColor = true;
            // 
            // labelMinFeatureSize
            // 
            resources.ApplyResources(this.labelMinFeatureSize, "labelMinFeatureSize");
            this.labelMinFeatureSize.Name = "labelMinFeatureSize";
            // 
            // numericUpDownMinFeatureSize
            // 
            resources.ApplyResources(this.numericUpDownMinFeatureSize, "numericUpDownMinFeatureSize");
            this.numericUpDownMinFeatureSize.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericUpDownMinFeatureSize.Name = "numericUpDownMinFeatureSize";
            // 
            // labelAdjustWidth
            // 
            resources.ApplyResources(this.labelAdjustWidth, "labelAdjustWidth");
            this.labelAdjustWidth.Name = "labelAdjustWidth";
            // 
            // tableLayoutPanelAdjustWidth
            // 
            resources.ApplyResources(tableLayoutPanelAdjustWidth, "tableLayoutPanelAdjustWidth");
            tableLayoutPanelAdjustWidth.Controls.Add(this.radioButtonAdjustWidthOff, 0, 0);
            tableLayoutPanelAdjustWidth.Controls.Add(this.radioButtonAdjustWidthValue, 0, 1);
            tableLayoutPanelAdjustWidth.Controls.Add(this.radioButtonAdjustWidthCommon, 1, 0);
            tableLayoutPanelAdjustWidth.Controls.Add(this.numericUpDownAdjustWidthValue, 1, 1);
            tableLayoutPanelAdjustWidth.Name = "tableLayoutPanelAdjustWidth";
            // 
            // radioButtonAdjustWidthOff
            // 
            resources.ApplyResources(this.radioButtonAdjustWidthOff, "radioButtonAdjustWidthOff");
            this.radioButtonAdjustWidthOff.Checked = true;
            this.radioButtonAdjustWidthOff.Name = "radioButtonAdjustWidthOff";
            this.radioButtonAdjustWidthOff.TabStop = true;
            this.radioButtonAdjustWidthOff.UseVisualStyleBackColor = true;
            // 
            // radioButtonAdjustWidthValue
            // 
            resources.ApplyResources(this.radioButtonAdjustWidthValue, "radioButtonAdjustWidthValue");
            this.radioButtonAdjustWidthValue.Name = "radioButtonAdjustWidthValue";
            this.radioButtonAdjustWidthValue.UseVisualStyleBackColor = true;
            // 
            // radioButtonAdjustWidthCommon
            // 
            resources.ApplyResources(this.radioButtonAdjustWidthCommon, "radioButtonAdjustWidthCommon");
            this.radioButtonAdjustWidthCommon.Name = "radioButtonAdjustWidthCommon";
            this.radioButtonAdjustWidthCommon.UseVisualStyleBackColor = true;
            // 
            // numericUpDownAdjustWidthValue
            // 
            resources.ApplyResources(this.numericUpDownAdjustWidthValue, "numericUpDownAdjustWidthValue");
            this.numericUpDownAdjustWidthValue.Maximum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            0});
            this.numericUpDownAdjustWidthValue.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownAdjustWidthValue.Name = "numericUpDownAdjustWidthValue";
            this.numericUpDownAdjustWidthValue.Value = new decimal(new int[] {
            1728,
            0,
            0,
            0});
            // 
            // labelAdditional
            // 
            resources.ApplyResources(labelAdditional, "labelAdditional");
            labelAdditional.Name = "labelAdditional";
            this.tableLayoutPanelTiff.SetRowSpan(labelAdditional, 4);
            // 
            // labelMaxStripSize
            // 
            resources.ApplyResources(labelMaxStripSize, "labelMaxStripSize");
            labelMaxStripSize.Name = "labelMaxStripSize";
            // 
            // numericUpDownMaxStripSize
            // 
            resources.ApplyResources(this.numericUpDownMaxStripSize, "numericUpDownMaxStripSize");
            this.numericUpDownMaxStripSize.Maximum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            0});
            this.numericUpDownMaxStripSize.Name = "numericUpDownMaxStripSize";
            this.numericUpDownMaxStripSize.Value = new decimal(new int[] {
            8192,
            0,
            0,
            0});
            // 
            // checkBoxFillOrder
            // 
            resources.ApplyResources(this.checkBoxFillOrder, "checkBoxFillOrder");
            this.checkBoxFillOrder.Checked = true;
            this.checkBoxFillOrder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanelTiff.SetColumnSpan(this.checkBoxFillOrder, 2);
            this.checkBoxFillOrder.Name = "checkBoxFillOrder";
            this.checkBoxFillOrder.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseBigTiff
            // 
            resources.ApplyResources(this.checkBoxUseBigTiff, "checkBoxUseBigTiff");
            this.tableLayoutPanelTiff.SetColumnSpan(this.checkBoxUseBigTiff, 2);
            this.checkBoxUseBigTiff.Name = "checkBoxUseBigTiff";
            this.checkBoxUseBigTiff.UseVisualStyleBackColor = true;
            // 
            // checkBoxTiffDateTime
            // 
            resources.ApplyResources(this.checkBoxTiffDateTime, "checkBoxTiffDateTime");
            this.checkBoxTiffDateTime.Checked = true;
            this.checkBoxTiffDateTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanelTiff.SetColumnSpan(this.checkBoxTiffDateTime, 2);
            this.checkBoxTiffDateTime.Name = "checkBoxTiffDateTime";
            this.checkBoxTiffDateTime.UseVisualStyleBackColor = true;
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "icc";
            resources.ApplyResources(this.openFileDialog, "openFileDialog");
            // 
            // TiffFormatDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(groupBoxTiff);
            this.Name = "TiffFormatDialog";
            groupBoxTiff.ResumeLayout(false);
            groupBoxTiff.PerformLayout();
            this.tableLayoutPanelTiff.ResumeLayout(false);
            this.tableLayoutPanelTiff.PerformLayout();
            tableLayoutPanelRgb.ResumeLayout(false);
            tableLayoutPanelRgb.PerformLayout();
            tableLayoutPanelCmyk.ResumeLayout(false);
            tableLayoutPanelCmyk.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDownscaleFactor)).EndInit();
            tableLayoutPanelCompression.ResumeLayout(false);
            tableLayoutPanelCompression.PerformLayout();
            tableLayoutPanelMonochrome.ResumeLayout(false);
            tableLayoutPanelMonochrome.PerformLayout();
            tableLayoutPanelMonochromeUnscaled.ResumeLayout(false);
            tableLayoutPanelMonochromeUnscaled.PerformLayout();
            tableLayoutPanelG3.ResumeLayout(false);
            tableLayoutPanelG3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinFeatureSize)).EndInit();
            tableLayoutPanelAdjustWidth.ResumeLayout(false);
            tableLayoutPanelAdjustWidth.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAdjustWidthValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxStripSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelTiff;
        private System.Windows.Forms.RadioButton radioButtonMonochrome;
        private System.Windows.Forms.RadioButton radioButtonGrayscale;
        private System.Windows.Forms.RadioButton radioButtonRgb;
        private System.Windows.Forms.RadioButton radioButtonCmyk;
        private System.Windows.Forms.RadioButton radioButtonRgb12;
        private System.Windows.Forms.RadioButton radioButtonRgb24;
        private System.Windows.Forms.RadioButton radioButtonRgb48;
        private System.Windows.Forms.RadioButton radioButtonCmyk4;
        private System.Windows.Forms.RadioButton radioButtonCmyk32;
        private System.Windows.Forms.RadioButton radioButtonCmyk64;
        private System.Windows.Forms.CheckBox checkBoxScaled;
        private System.Windows.Forms.Label labelDownscaleFactor;
        private System.Windows.Forms.NumericUpDown numericUpDownDownscaleFactor;
        private System.Windows.Forms.Label labelCompression;
        private System.Windows.Forms.RadioButton radioButtonCompressionNone;
        private System.Windows.Forms.RadioButton radioButtonCompressionLzw;
        private System.Windows.Forms.RadioButton radioButtonCompressionPack;
        private System.Windows.Forms.Label labelPostRenderProfile;
        private System.Windows.Forms.Button buttonPostRenderProfile;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.RadioButton radioButtonMonochromeUnscaled;
        private System.Windows.Forms.RadioButton radioButtonMonochromeScaled;
        private System.Windows.Forms.RadioButton radioButtonMonochromeG3;
        private System.Windows.Forms.RadioButton radioButtonMonochromeG4;
        private System.Windows.Forms.RadioButton radioButtonMonochromeLzw;
        private System.Windows.Forms.RadioButton radioButtonMonochromePack;
        private System.Windows.Forms.RadioButton radioButtonG3;
        private System.Windows.Forms.RadioButton radioButtonG32D;
        private System.Windows.Forms.RadioButton radioButtonG3Crle;
        private System.Windows.Forms.Label labelMinFeatureSize;
        private System.Windows.Forms.NumericUpDown numericUpDownMinFeatureSize;
        private System.Windows.Forms.Label labelAdjustWidth;
        private System.Windows.Forms.RadioButton radioButtonAdjustWidthOff;
        private System.Windows.Forms.RadioButton radioButtonAdjustWidthValue;
        private System.Windows.Forms.RadioButton radioButtonAdjustWidthCommon;
        private System.Windows.Forms.NumericUpDown numericUpDownAdjustWidthValue;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxStripSize;
        private System.Windows.Forms.CheckBox checkBoxFillOrder;
        private System.Windows.Forms.CheckBox checkBoxUseBigTiff;
        private System.Windows.Forms.CheckBox checkBoxTiffDateTime;

    }
}
