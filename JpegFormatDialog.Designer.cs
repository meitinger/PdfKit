namespace Aufbauwerk.Tools.PdfKit
{
    partial class JpegFormatDialog
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
            System.Windows.Forms.GroupBox groupBoxJpeg;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JpegFormatDialog));
            System.Windows.Forms.TableLayoutPanel tableLayoutPanelJpeg;
            System.Windows.Forms.Label labelQuality;
            System.Windows.Forms.Label labelColorFormat;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanelColorFormat;
            this.numericUpDownJpegQuality = new System.Windows.Forms.NumericUpDown();
            this.radioButtonJpegQuality = new System.Windows.Forms.RadioButton();
            this.radioButtonQFactor = new System.Windows.Forms.RadioButton();
            this.numericUpDownQFactor = new System.Windows.Forms.NumericUpDown();
            this.radioButtonGrayscale = new System.Windows.Forms.RadioButton();
            this.radioButtonColor = new System.Windows.Forms.RadioButton();
            groupBoxJpeg = new System.Windows.Forms.GroupBox();
            tableLayoutPanelJpeg = new System.Windows.Forms.TableLayoutPanel();
            labelQuality = new System.Windows.Forms.Label();
            labelColorFormat = new System.Windows.Forms.Label();
            tableLayoutPanelColorFormat = new System.Windows.Forms.TableLayoutPanel();
            groupBoxJpeg.SuspendLayout();
            tableLayoutPanelJpeg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownJpegQuality)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownQFactor)).BeginInit();
            tableLayoutPanelColorFormat.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxJpeg
            // 
            resources.ApplyResources(groupBoxJpeg, "groupBoxJpeg");
            groupBoxJpeg.Controls.Add(tableLayoutPanelJpeg);
            groupBoxJpeg.Name = "groupBoxJpeg";
            groupBoxJpeg.TabStop = false;
            // 
            // tableLayoutPanelJpeg
            // 
            resources.ApplyResources(tableLayoutPanelJpeg, "tableLayoutPanelJpeg");
            tableLayoutPanelJpeg.Controls.Add(labelQuality, 0, 0);
            tableLayoutPanelJpeg.Controls.Add(this.numericUpDownJpegQuality, 2, 0);
            tableLayoutPanelJpeg.Controls.Add(this.radioButtonJpegQuality, 1, 0);
            tableLayoutPanelJpeg.Controls.Add(this.radioButtonQFactor, 1, 1);
            tableLayoutPanelJpeg.Controls.Add(this.numericUpDownQFactor, 2, 1);
            tableLayoutPanelJpeg.Controls.Add(labelColorFormat, 0, 2);
            tableLayoutPanelJpeg.Controls.Add(tableLayoutPanelColorFormat, 1, 2);
            tableLayoutPanelJpeg.Name = "tableLayoutPanelJpeg";
            // 
            // labelQuality
            // 
            resources.ApplyResources(labelQuality, "labelQuality");
            labelQuality.Name = "labelQuality";
            tableLayoutPanelJpeg.SetRowSpan(labelQuality, 2);
            // 
            // numericUpDownJpegQuality
            // 
            resources.ApplyResources(this.numericUpDownJpegQuality, "numericUpDownJpegQuality");
            this.numericUpDownJpegQuality.Name = "numericUpDownJpegQuality";
            this.numericUpDownJpegQuality.Value = new decimal(new int[] {
            75,
            0,
            0,
            0});
            // 
            // radioButtonJpegQuality
            // 
            resources.ApplyResources(this.radioButtonJpegQuality, "radioButtonJpegQuality");
            this.radioButtonJpegQuality.Checked = true;
            this.radioButtonJpegQuality.Name = "radioButtonJpegQuality";
            this.radioButtonJpegQuality.TabStop = true;
            this.radioButtonJpegQuality.UseVisualStyleBackColor = true;
            // 
            // radioButtonQFactor
            // 
            resources.ApplyResources(this.radioButtonQFactor, "radioButtonQFactor");
            this.radioButtonQFactor.Name = "radioButtonQFactor";
            this.radioButtonQFactor.UseVisualStyleBackColor = true;
            // 
            // numericUpDownQFactor
            // 
            resources.ApplyResources(this.numericUpDownQFactor, "numericUpDownQFactor");
            this.numericUpDownQFactor.DecimalPlaces = 1;
            this.numericUpDownQFactor.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownQFactor.Name = "numericUpDownQFactor";
            this.numericUpDownQFactor.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            // 
            // labelColorFormat
            // 
            resources.ApplyResources(labelColorFormat, "labelColorFormat");
            labelColorFormat.Name = "labelColorFormat";
            // 
            // tableLayoutPanelColorFormat
            // 
            resources.ApplyResources(tableLayoutPanelColorFormat, "tableLayoutPanelColorFormat");
            tableLayoutPanelJpeg.SetColumnSpan(tableLayoutPanelColorFormat, 2);
            tableLayoutPanelColorFormat.Controls.Add(this.radioButtonGrayscale, 0, 0);
            tableLayoutPanelColorFormat.Controls.Add(this.radioButtonColor, 1, 0);
            tableLayoutPanelColorFormat.Name = "tableLayoutPanelColorFormat";
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
            this.radioButtonColor.Checked = true;
            this.radioButtonColor.Name = "radioButtonColor";
            this.radioButtonColor.TabStop = true;
            this.radioButtonColor.UseVisualStyleBackColor = true;
            // 
            // JpegFormatDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(groupBoxJpeg);
            this.Name = "JpegFormatDialog";
            groupBoxJpeg.ResumeLayout(false);
            groupBoxJpeg.PerformLayout();
            tableLayoutPanelJpeg.ResumeLayout(false);
            tableLayoutPanelJpeg.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownJpegQuality)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownQFactor)).EndInit();
            tableLayoutPanelColorFormat.ResumeLayout(false);
            tableLayoutPanelColorFormat.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDownJpegQuality;
        private System.Windows.Forms.RadioButton radioButtonJpegQuality;
        private System.Windows.Forms.RadioButton radioButtonQFactor;
        private System.Windows.Forms.NumericUpDown numericUpDownQFactor;
        private System.Windows.Forms.RadioButton radioButtonGrayscale;
        private System.Windows.Forms.RadioButton radioButtonColor;
    }
}
