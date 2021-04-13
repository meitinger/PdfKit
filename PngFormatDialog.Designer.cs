namespace Aufbauwerk.Tools.PdfKit
{
    partial class PngFormatDialog
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
            System.Windows.Forms.GroupBox groupBoxPng;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PngFormatDialog));
            System.Windows.Forms.TableLayoutPanel tableLayoutPanelPng;
            System.Windows.Forms.Label labelDownscaleFactor;
            this.radioButtonMonochrome = new System.Windows.Forms.RadioButton();
            this.numericUpDownDownScaleFactor = new System.Windows.Forms.NumericUpDown();
            this.labelMinFeatureSize = new System.Windows.Forms.Label();
            this.numericUpDownMinFeatureSize = new System.Windows.Forms.NumericUpDown();
            this.radioButtonGrayscale = new System.Windows.Forms.RadioButton();
            this.radioButtonColor = new System.Windows.Forms.RadioButton();
            this.radioButtonAlpha = new System.Windows.Forms.RadioButton();
            this.labelBackgroundColor = new System.Windows.Forms.Label();
            this.buttonBackgroundColor = new System.Windows.Forms.Button();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            groupBoxPng = new System.Windows.Forms.GroupBox();
            tableLayoutPanelPng = new System.Windows.Forms.TableLayoutPanel();
            labelDownscaleFactor = new System.Windows.Forms.Label();
            groupBoxPng.SuspendLayout();
            tableLayoutPanelPng.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDownScaleFactor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinFeatureSize)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxPng
            // 
            resources.ApplyResources(groupBoxPng, "groupBoxPng");
            groupBoxPng.Controls.Add(tableLayoutPanelPng);
            groupBoxPng.Name = "groupBoxPng";
            groupBoxPng.TabStop = false;
            // 
            // tableLayoutPanelPng
            // 
            resources.ApplyResources(tableLayoutPanelPng, "tableLayoutPanelPng");
            tableLayoutPanelPng.Controls.Add(this.radioButtonMonochrome, 0, 1);
            tableLayoutPanelPng.Controls.Add(this.numericUpDownDownScaleFactor, 1, 0);
            tableLayoutPanelPng.Controls.Add(this.labelMinFeatureSize, 1, 1);
            tableLayoutPanelPng.Controls.Add(this.numericUpDownMinFeatureSize, 2, 1);
            tableLayoutPanelPng.Controls.Add(this.radioButtonGrayscale, 0, 2);
            tableLayoutPanelPng.Controls.Add(this.radioButtonColor, 0, 3);
            tableLayoutPanelPng.Controls.Add(this.radioButtonAlpha, 0, 4);
            tableLayoutPanelPng.Controls.Add(this.labelBackgroundColor, 1, 4);
            tableLayoutPanelPng.Controls.Add(this.buttonBackgroundColor, 2, 4);
            tableLayoutPanelPng.Controls.Add(labelDownscaleFactor, 0, 0);
            tableLayoutPanelPng.Name = "tableLayoutPanelPng";
            // 
            // radioButtonMonochrome
            // 
            resources.ApplyResources(this.radioButtonMonochrome, "radioButtonMonochrome");
            this.radioButtonMonochrome.Name = "radioButtonMonochrome";
            this.radioButtonMonochrome.UseVisualStyleBackColor = true;
            // 
            // numericUpDownDownScaleFactor
            // 
            resources.ApplyResources(this.numericUpDownDownScaleFactor, "numericUpDownDownScaleFactor");
            tableLayoutPanelPng.SetColumnSpan(this.numericUpDownDownScaleFactor, 2);
            this.numericUpDownDownScaleFactor.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericUpDownDownScaleFactor.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownDownScaleFactor.Name = "numericUpDownDownScaleFactor";
            this.numericUpDownDownScaleFactor.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
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
            // radioButtonGrayscale
            // 
            resources.ApplyResources(this.radioButtonGrayscale, "radioButtonGrayscale");
            this.radioButtonGrayscale.Name = "radioButtonGrayscale";
            this.radioButtonGrayscale.UseVisualStyleBackColor = true;
            // 
            // radioButtonColor
            // 
            resources.ApplyResources(this.radioButtonColor, "radioButtonColor");
            this.radioButtonColor.Name = "radioButtonColor";
            this.radioButtonColor.UseVisualStyleBackColor = true;
            // 
            // radioButtonAlpha
            // 
            resources.ApplyResources(this.radioButtonAlpha, "radioButtonAlpha");
            this.radioButtonAlpha.Checked = true;
            this.radioButtonAlpha.Name = "radioButtonAlpha";
            this.radioButtonAlpha.TabStop = true;
            this.radioButtonAlpha.UseVisualStyleBackColor = true;
            // 
            // labelBackgroundColor
            // 
            resources.ApplyResources(this.labelBackgroundColor, "labelBackgroundColor");
            this.labelBackgroundColor.Name = "labelBackgroundColor";
            // 
            // buttonBackgroundColor
            // 
            resources.ApplyResources(this.buttonBackgroundColor, "buttonBackgroundColor");
            this.buttonBackgroundColor.BackColor = System.Drawing.Color.White;
            this.buttonBackgroundColor.Name = "buttonBackgroundColor";
            this.buttonBackgroundColor.UseVisualStyleBackColor = false;
            this.buttonBackgroundColor.Click += new System.EventHandler(this.ButtonBackgroundColor_Click);
            // 
            // labelDownscaleFactor
            // 
            resources.ApplyResources(labelDownscaleFactor, "labelDownscaleFactor");
            labelDownscaleFactor.Name = "labelDownscaleFactor";
            // 
            // colorDialog
            // 
            this.colorDialog.AnyColor = true;
            // 
            // PngFormatDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(groupBoxPng);
            this.Name = "PngFormatDialog";
            groupBoxPng.ResumeLayout(false);
            groupBoxPng.PerformLayout();
            tableLayoutPanelPng.ResumeLayout(false);
            tableLayoutPanelPng.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDownScaleFactor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinFeatureSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonMonochrome;
        private System.Windows.Forms.NumericUpDown numericUpDownDownScaleFactor;
        private System.Windows.Forms.NumericUpDown numericUpDownMinFeatureSize;
        private System.Windows.Forms.RadioButton radioButtonGrayscale;
        private System.Windows.Forms.RadioButton radioButtonColor;
        private System.Windows.Forms.RadioButton radioButtonAlpha;
        private System.Windows.Forms.Button buttonBackgroundColor;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Label labelMinFeatureSize;
        private System.Windows.Forms.Label labelBackgroundColor;
    }
}
