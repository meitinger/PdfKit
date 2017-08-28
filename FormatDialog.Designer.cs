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
            System.Windows.Forms.TableLayoutPanel tableLayoutPanelPages;
            System.Windows.Forms.Button buttonOK;
            System.Windows.Forms.Button buttonCancel;
            this.groupBoxPages = new System.Windows.Forms.GroupBox();
            this.radioButtonSingleFile = new System.Windows.Forms.RadioButton();
            this.radioButtonMultipleFiles = new System.Windows.Forms.RadioButton();
            this.flowLayoutPanelButtons = new System.Windows.Forms.FlowLayoutPanel();
            tableLayoutPanelPages = new System.Windows.Forms.TableLayoutPanel();
            buttonOK = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxPages.SuspendLayout();
            tableLayoutPanelPages.SuspendLayout();
            this.flowLayoutPanelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxPages
            // 
            resources.ApplyResources(this.groupBoxPages, "groupBoxPages");
            this.groupBoxPages.Controls.Add(tableLayoutPanelPages);
            this.groupBoxPages.Name = "groupBoxPages";
            this.groupBoxPages.TabStop = false;
            // 
            // tableLayoutPanelPages
            // 
            resources.ApplyResources(tableLayoutPanelPages, "tableLayoutPanelPages");
            tableLayoutPanelPages.Controls.Add(this.radioButtonSingleFile, 0, 0);
            tableLayoutPanelPages.Controls.Add(this.radioButtonMultipleFiles, 0, 1);
            tableLayoutPanelPages.Name = "tableLayoutPanelPages";
            // 
            // radioButtonSingleFile
            // 
            resources.ApplyResources(this.radioButtonSingleFile, "radioButtonSingleFile");
            this.radioButtonSingleFile.Checked = true;
            this.radioButtonSingleFile.Name = "radioButtonSingleFile";
            this.radioButtonSingleFile.TabStop = true;
            this.radioButtonSingleFile.UseVisualStyleBackColor = true;
            // 
            // radioButtonMultipleFiles
            // 
            resources.ApplyResources(this.radioButtonMultipleFiles, "radioButtonMultipleFiles");
            this.radioButtonMultipleFiles.Name = "radioButtonMultipleFiles";
            this.radioButtonMultipleFiles.UseVisualStyleBackColor = true;
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
            this.Controls.Add(this.groupBoxPages);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormatDialog";
            this.Load += new System.EventHandler(this.FormatDialog_Load);
            this.groupBoxPages.ResumeLayout(false);
            this.groupBoxPages.PerformLayout();
            tableLayoutPanelPages.ResumeLayout(false);
            tableLayoutPanelPages.PerformLayout();
            this.flowLayoutPanelButtons.ResumeLayout(false);
            this.flowLayoutPanelButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonSingleFile;
        private System.Windows.Forms.RadioButton radioButtonMultipleFiles;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelButtons;
        private System.Windows.Forms.GroupBox groupBoxPages;
    }
}