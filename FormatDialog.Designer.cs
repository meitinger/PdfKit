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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormatDialog));
            System.Windows.Forms.Button buttonOK;
            System.Windows.Forms.Button buttonCancel;
            this.flowLayoutPanelButtons = new System.Windows.Forms.FlowLayoutPanel();
            buttonOK = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            this.flowLayoutPanelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            resources.ApplyResources(buttonOK, "buttonOK");
            buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            buttonOK.Name = "buttonOK";
            buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            resources.ApplyResources(buttonCancel, "buttonCancel");
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.Name = "buttonCancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanelButtons
            // 
            resources.ApplyResources(this.flowLayoutPanelButtons, "flowLayoutPanelButtons");
            this.flowLayoutPanelButtons.Controls.Add(buttonCancel);
            this.flowLayoutPanelButtons.Controls.Add(buttonOK);
            this.flowLayoutPanelButtons.Name = "flowLayoutPanelButtons";
            // 
            // FormatDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AcceptButton = buttonOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = buttonCancel;
            this.Controls.Add(this.flowLayoutPanelButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormatDialog";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormatDialog_FormClosed);
            this.Load += new System.EventHandler(this.FormatDialog_Load);
            this.flowLayoutPanelButtons.ResumeLayout(false);
            this.flowLayoutPanelButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelButtons;
    }
}