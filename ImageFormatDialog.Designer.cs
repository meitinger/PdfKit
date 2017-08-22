namespace Aufbauwerk.Tools.PdfKit
{
    partial class ImageFormatDialog
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
            System.Windows.Forms.GroupBox groupBoxImage;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageFormatDialog));
            System.Windows.Forms.TableLayoutPanel tableLayoutPanelImage;
            System.Windows.Forms.Label labelResolution;
            System.Windows.Forms.Label labelResolutionX;
            System.Windows.Forms.Label labelResolutionY;
            System.Windows.Forms.Label labelAlphaBits;
            System.Windows.Forms.Label labelTextAlphaBits;
            System.Windows.Forms.Label labelGraphicsAlphaBits;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanelGraphicsAlphaBits;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanelTextAlphaBits;
            this.numericUpDownResolutionX = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownResolutionY = new System.Windows.Forms.NumericUpDown();
            this.radioButtonGraphicsAlphaBits1 = new System.Windows.Forms.RadioButton();
            this.radioButtonGraphicsAlphaBits4 = new System.Windows.Forms.RadioButton();
            this.radioButtonGraphicsAlphaBits2 = new System.Windows.Forms.RadioButton();
            this.radioButtonTextAlphaBits1 = new System.Windows.Forms.RadioButton();
            this.radioButtonTextAlphaBits2 = new System.Windows.Forms.RadioButton();
            this.radioButtonTextAlphaBits4 = new System.Windows.Forms.RadioButton();
            this.checkBoxLinkedResolution = new System.Windows.Forms.CheckBox();
            groupBoxImage = new System.Windows.Forms.GroupBox();
            tableLayoutPanelImage = new System.Windows.Forms.TableLayoutPanel();
            labelResolution = new System.Windows.Forms.Label();
            labelResolutionX = new System.Windows.Forms.Label();
            labelResolutionY = new System.Windows.Forms.Label();
            labelAlphaBits = new System.Windows.Forms.Label();
            labelTextAlphaBits = new System.Windows.Forms.Label();
            labelGraphicsAlphaBits = new System.Windows.Forms.Label();
            tableLayoutPanelGraphicsAlphaBits = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanelTextAlphaBits = new System.Windows.Forms.TableLayoutPanel();
            groupBoxImage.SuspendLayout();
            tableLayoutPanelImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownResolutionX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownResolutionY)).BeginInit();
            tableLayoutPanelGraphicsAlphaBits.SuspendLayout();
            tableLayoutPanelTextAlphaBits.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxImage
            // 
            resources.ApplyResources(groupBoxImage, "groupBoxImage");
            groupBoxImage.Controls.Add(tableLayoutPanelImage);
            groupBoxImage.Name = "groupBoxImage";
            groupBoxImage.TabStop = false;
            // 
            // tableLayoutPanelImage
            // 
            resources.ApplyResources(tableLayoutPanelImage, "tableLayoutPanelImage");
            tableLayoutPanelImage.Controls.Add(labelResolution, 0, 0);
            tableLayoutPanelImage.Controls.Add(this.numericUpDownResolutionX, 2, 0);
            tableLayoutPanelImage.Controls.Add(labelResolutionX, 1, 0);
            tableLayoutPanelImage.Controls.Add(labelResolutionY, 1, 1);
            tableLayoutPanelImage.Controls.Add(this.numericUpDownResolutionY, 2, 1);
            tableLayoutPanelImage.Controls.Add(labelAlphaBits, 0, 2);
            tableLayoutPanelImage.Controls.Add(labelTextAlphaBits, 1, 2);
            tableLayoutPanelImage.Controls.Add(labelGraphicsAlphaBits, 1, 3);
            tableLayoutPanelImage.Controls.Add(tableLayoutPanelGraphicsAlphaBits, 2, 3);
            tableLayoutPanelImage.Controls.Add(tableLayoutPanelTextAlphaBits, 2, 2);
            tableLayoutPanelImage.Controls.Add(this.checkBoxLinkedResolution, 3, 0);
            tableLayoutPanelImage.Name = "tableLayoutPanelImage";
            // 
            // labelResolution
            // 
            resources.ApplyResources(labelResolution, "labelResolution");
            labelResolution.Name = "labelResolution";
            tableLayoutPanelImage.SetRowSpan(labelResolution, 2);
            // 
            // numericUpDownResolutionX
            // 
            resources.ApplyResources(this.numericUpDownResolutionX, "numericUpDownResolutionX");
            this.numericUpDownResolutionX.Maximum = new decimal(new int[] {
            1600,
            0,
            0,
            0});
            this.numericUpDownResolutionX.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownResolutionX.Name = "numericUpDownResolutionX";
            this.numericUpDownResolutionX.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            // 
            // labelResolutionX
            // 
            resources.ApplyResources(labelResolutionX, "labelResolutionX");
            labelResolutionX.Name = "labelResolutionX";
            // 
            // labelResolutionY
            // 
            resources.ApplyResources(labelResolutionY, "labelResolutionY");
            labelResolutionY.Name = "labelResolutionY";
            // 
            // numericUpDownResolutionY
            // 
            resources.ApplyResources(this.numericUpDownResolutionY, "numericUpDownResolutionY");
            this.numericUpDownResolutionY.Maximum = new decimal(new int[] {
            1600,
            0,
            0,
            0});
            this.numericUpDownResolutionY.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownResolutionY.Name = "numericUpDownResolutionY";
            this.numericUpDownResolutionY.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            // 
            // labelAlphaBits
            // 
            resources.ApplyResources(labelAlphaBits, "labelAlphaBits");
            labelAlphaBits.Name = "labelAlphaBits";
            tableLayoutPanelImage.SetRowSpan(labelAlphaBits, 2);
            // 
            // labelTextAlphaBits
            // 
            resources.ApplyResources(labelTextAlphaBits, "labelTextAlphaBits");
            labelTextAlphaBits.Name = "labelTextAlphaBits";
            // 
            // labelGraphicsAlphaBits
            // 
            resources.ApplyResources(labelGraphicsAlphaBits, "labelGraphicsAlphaBits");
            labelGraphicsAlphaBits.Name = "labelGraphicsAlphaBits";
            // 
            // tableLayoutPanelGraphicsAlphaBits
            // 
            resources.ApplyResources(tableLayoutPanelGraphicsAlphaBits, "tableLayoutPanelGraphicsAlphaBits");
            tableLayoutPanelImage.SetColumnSpan(tableLayoutPanelGraphicsAlphaBits, 2);
            tableLayoutPanelGraphicsAlphaBits.Controls.Add(this.radioButtonGraphicsAlphaBits1, 0, 0);
            tableLayoutPanelGraphicsAlphaBits.Controls.Add(this.radioButtonGraphicsAlphaBits4, 2, 0);
            tableLayoutPanelGraphicsAlphaBits.Controls.Add(this.radioButtonGraphicsAlphaBits2, 1, 0);
            tableLayoutPanelGraphicsAlphaBits.Name = "tableLayoutPanelGraphicsAlphaBits";
            // 
            // radioButtonGraphicsAlphaBits1
            // 
            resources.ApplyResources(this.radioButtonGraphicsAlphaBits1, "radioButtonGraphicsAlphaBits1");
            this.radioButtonGraphicsAlphaBits1.Name = "radioButtonGraphicsAlphaBits1";
            this.radioButtonGraphicsAlphaBits1.UseVisualStyleBackColor = true;
            // 
            // radioButtonGraphicsAlphaBits4
            // 
            resources.ApplyResources(this.radioButtonGraphicsAlphaBits4, "radioButtonGraphicsAlphaBits4");
            this.radioButtonGraphicsAlphaBits4.Checked = true;
            this.radioButtonGraphicsAlphaBits4.Name = "radioButtonGraphicsAlphaBits4";
            this.radioButtonGraphicsAlphaBits4.TabStop = true;
            this.radioButtonGraphicsAlphaBits4.UseVisualStyleBackColor = true;
            // 
            // radioButtonGraphicsAlphaBits2
            // 
            resources.ApplyResources(this.radioButtonGraphicsAlphaBits2, "radioButtonGraphicsAlphaBits2");
            this.radioButtonGraphicsAlphaBits2.Name = "radioButtonGraphicsAlphaBits2";
            this.radioButtonGraphicsAlphaBits2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelTextAlphaBits
            // 
            resources.ApplyResources(tableLayoutPanelTextAlphaBits, "tableLayoutPanelTextAlphaBits");
            tableLayoutPanelImage.SetColumnSpan(tableLayoutPanelTextAlphaBits, 2);
            tableLayoutPanelTextAlphaBits.Controls.Add(this.radioButtonTextAlphaBits1, 0, 0);
            tableLayoutPanelTextAlphaBits.Controls.Add(this.radioButtonTextAlphaBits2, 1, 0);
            tableLayoutPanelTextAlphaBits.Controls.Add(this.radioButtonTextAlphaBits4, 2, 0);
            tableLayoutPanelTextAlphaBits.Name = "tableLayoutPanelTextAlphaBits";
            // 
            // radioButtonTextAlphaBits1
            // 
            resources.ApplyResources(this.radioButtonTextAlphaBits1, "radioButtonTextAlphaBits1");
            this.radioButtonTextAlphaBits1.Name = "radioButtonTextAlphaBits1";
            this.radioButtonTextAlphaBits1.UseVisualStyleBackColor = true;
            // 
            // radioButtonTextAlphaBits2
            // 
            resources.ApplyResources(this.radioButtonTextAlphaBits2, "radioButtonTextAlphaBits2");
            this.radioButtonTextAlphaBits2.Name = "radioButtonTextAlphaBits2";
            this.radioButtonTextAlphaBits2.UseVisualStyleBackColor = true;
            // 
            // radioButtonTextAlphaBits4
            // 
            resources.ApplyResources(this.radioButtonTextAlphaBits4, "radioButtonTextAlphaBits4");
            this.radioButtonTextAlphaBits4.Checked = true;
            this.radioButtonTextAlphaBits4.Name = "radioButtonTextAlphaBits4";
            this.radioButtonTextAlphaBits4.TabStop = true;
            this.radioButtonTextAlphaBits4.UseVisualStyleBackColor = true;
            // 
            // checkBoxLinkedResolution
            // 
            resources.ApplyResources(this.checkBoxLinkedResolution, "checkBoxLinkedResolution");
            this.checkBoxLinkedResolution.Checked = true;
            this.checkBoxLinkedResolution.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLinkedResolution.Name = "checkBoxLinkedResolution";
            tableLayoutPanelImage.SetRowSpan(this.checkBoxLinkedResolution, 2);
            this.checkBoxLinkedResolution.UseVisualStyleBackColor = true;
            // 
            // ImageFormatDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(groupBoxImage);
            this.Name = "ImageFormatDialog";
            groupBoxImage.ResumeLayout(false);
            groupBoxImage.PerformLayout();
            tableLayoutPanelImage.ResumeLayout(false);
            tableLayoutPanelImage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownResolutionX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownResolutionY)).EndInit();
            tableLayoutPanelGraphicsAlphaBits.ResumeLayout(false);
            tableLayoutPanelGraphicsAlphaBits.PerformLayout();
            tableLayoutPanelTextAlphaBits.ResumeLayout(false);
            tableLayoutPanelTextAlphaBits.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDownResolutionX;
        private System.Windows.Forms.NumericUpDown numericUpDownResolutionY;
        private System.Windows.Forms.RadioButton radioButtonTextAlphaBits1;
        private System.Windows.Forms.RadioButton radioButtonTextAlphaBits4;
        private System.Windows.Forms.RadioButton radioButtonTextAlphaBits2;
        private System.Windows.Forms.RadioButton radioButtonGraphicsAlphaBits4;
        private System.Windows.Forms.RadioButton radioButtonGraphicsAlphaBits2;
        private System.Windows.Forms.RadioButton radioButtonGraphicsAlphaBits1;
        private System.Windows.Forms.CheckBox checkBoxLinkedResolution;
    }
}
