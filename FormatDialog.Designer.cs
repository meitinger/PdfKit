namespace Aufbauwerk.Tools.PdfKit
{
    partial class FormatDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.GroupBox groupBoxPages;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormatDialog));
            this.tableLayoutPanelGeneral = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonAllPages = new System.Windows.Forms.RadioButton();
            this.radioButtonSelectedPages = new System.Windows.Forms.RadioButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            groupBoxPages = new System.Windows.Forms.GroupBox();
            groupBoxPages.SuspendLayout();
            this.tableLayoutPanelGeneral.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxPages
            // 
            resources.ApplyResources(groupBoxPages, "groupBoxPages");
            groupBoxPages.Controls.Add(this.tableLayoutPanelGeneral);
            groupBoxPages.Name = "groupBoxPages";
            groupBoxPages.TabStop = false;
            // 
            // tableLayoutPanelGeneral
            // 
            resources.ApplyResources(this.tableLayoutPanelGeneral, "tableLayoutPanelGeneral");
            this.tableLayoutPanelGeneral.Controls.Add(this.radioButtonAllPages, 0, 0);
            this.tableLayoutPanelGeneral.Controls.Add(this.radioButtonSelectedPages, 0, 1);
            this.tableLayoutPanelGeneral.Controls.Add(this.textBox1, 1, 1);
            this.tableLayoutPanelGeneral.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanelGeneral.Name = "tableLayoutPanelGeneral";
            // 
            // radioButtonAllPages
            // 
            resources.ApplyResources(this.radioButtonAllPages, "radioButtonAllPages");
            this.radioButtonAllPages.Checked = true;
            this.tableLayoutPanelGeneral.SetColumnSpan(this.radioButtonAllPages, 2);
            this.radioButtonAllPages.Name = "radioButtonAllPages";
            this.radioButtonAllPages.TabStop = true;
            this.radioButtonAllPages.UseVisualStyleBackColor = true;
            // 
            // radioButtonSelectedPages
            // 
            resources.ApplyResources(this.radioButtonSelectedPages, "radioButtonSelectedPages");
            this.radioButtonSelectedPages.Name = "radioButtonSelectedPages";
            this.radioButtonSelectedPages.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // FormatDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(groupBoxPages);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormatDialog";
            this.Load += new System.EventHandler(this.FormatDialog_Load);
            groupBoxPages.ResumeLayout(false);
            groupBoxPages.PerformLayout();
            this.tableLayoutPanelGeneral.ResumeLayout(false);
            this.tableLayoutPanelGeneral.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelGeneral;
        private System.Windows.Forms.RadioButton radioButtonAllPages;
        private System.Windows.Forms.RadioButton radioButtonSelectedPages;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
    }
}