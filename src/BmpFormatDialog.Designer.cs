namespace Aufbauwerk.Tools.PdfKit
{
    partial class BmpFormatDialog
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
            System.Windows.Forms.GroupBox groupBoxBmp;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BmpFormatDialog));
            System.Windows.Forms.TableLayoutPanel tableLayoutPanelBmp;
            this.radioButtonMonochrome = new System.Windows.Forms.RadioButton();
            this.radioButtonGrayscale = new System.Windows.Forms.RadioButton();
            this.radioButtonColor16 = new System.Windows.Forms.RadioButton();
            this.radioButtonColor256 = new System.Windows.Forms.RadioButton();
            this.radioButtonColor16m = new System.Windows.Forms.RadioButton();
            groupBoxBmp = new System.Windows.Forms.GroupBox();
            tableLayoutPanelBmp = new System.Windows.Forms.TableLayoutPanel();
            groupBoxBmp.SuspendLayout();
            tableLayoutPanelBmp.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxBmp
            // 
            resources.ApplyResources(groupBoxBmp, "groupBoxBmp");
            groupBoxBmp.Controls.Add(tableLayoutPanelBmp);
            groupBoxBmp.Name = "groupBoxBmp";
            groupBoxBmp.TabStop = false;
            // 
            // tableLayoutPanelBmp
            // 
            resources.ApplyResources(tableLayoutPanelBmp, "tableLayoutPanelBmp");
            tableLayoutPanelBmp.Controls.Add(this.radioButtonMonochrome, 0, 0);
            tableLayoutPanelBmp.Controls.Add(this.radioButtonGrayscale, 0, 1);
            tableLayoutPanelBmp.Controls.Add(this.radioButtonColor16, 0, 2);
            tableLayoutPanelBmp.Controls.Add(this.radioButtonColor256, 0, 3);
            tableLayoutPanelBmp.Controls.Add(this.radioButtonColor16m, 0, 4);
            tableLayoutPanelBmp.Name = "tableLayoutPanelBmp";
            // 
            // radioButtonMonochrome
            // 
            resources.ApplyResources(this.radioButtonMonochrome, "radioButtonMonochrome");
            this.radioButtonMonochrome.Name = "radioButtonMonochrome";
            this.radioButtonMonochrome.UseVisualStyleBackColor = true;
            // 
            // radioButtonGrayscale
            // 
            resources.ApplyResources(this.radioButtonGrayscale, "radioButtonGrayscale");
            this.radioButtonGrayscale.Name = "radioButtonGrayscale";
            this.radioButtonGrayscale.UseVisualStyleBackColor = true;
            // 
            // radioButtonColor16
            // 
            resources.ApplyResources(this.radioButtonColor16, "radioButtonColor16");
            this.radioButtonColor16.Name = "radioButtonColor16";
            this.radioButtonColor16.UseVisualStyleBackColor = true;
            // 
            // radioButtonColor256
            // 
            resources.ApplyResources(this.radioButtonColor256, "radioButtonColor256");
            this.radioButtonColor256.Name = "radioButtonColor256";
            this.radioButtonColor256.UseVisualStyleBackColor = true;
            // 
            // radioButtonColor16m
            // 
            resources.ApplyResources(this.radioButtonColor16m, "radioButtonColor16m");
            this.radioButtonColor16m.Checked = true;
            this.radioButtonColor16m.Name = "radioButtonColor16m";
            this.radioButtonColor16m.TabStop = true;
            this.radioButtonColor16m.UseVisualStyleBackColor = true;
            // 
            // BmpFormatDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(groupBoxBmp);
            this.Name = "BmpFormatDialog";
            groupBoxBmp.ResumeLayout(false);
            groupBoxBmp.PerformLayout();
            tableLayoutPanelBmp.ResumeLayout(false);
            tableLayoutPanelBmp.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonMonochrome;
        private System.Windows.Forms.RadioButton radioButtonGrayscale;
        private System.Windows.Forms.RadioButton radioButtonColor16;
        private System.Windows.Forms.RadioButton radioButtonColor256;
        private System.Windows.Forms.RadioButton radioButtonColor16m;

    }
}
