namespace Aufbauwerk.Tools.PdfKit
{
    partial class ExtractForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtractForm));
            this.listViewPages = new System.Windows.Forms.ListView();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripDropDownButtonZoomIn = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownButtonZoomOut = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownButtonSave = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripStatusLabelInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewPages
            // 
            this.listViewPages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewPages.Location = new System.Drawing.Point(0, 0);
            this.listViewPages.Name = "listViewPages";
            this.listViewPages.Size = new System.Drawing.Size(632, 431);
            this.listViewPages.TabIndex = 0;
            this.listViewPages.UseCompatibleStateImageBehavior = false;
            this.listViewPages.VirtualMode = true;
            this.listViewPages.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewPages_ItemDrag);
            this.listViewPages.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listViewPages_RetrieveVirtualItem);
            this.listViewPages.VirtualItemsSelectionRangeChanged += new System.Windows.Forms.ListViewVirtualItemsSelectionRangeChangedEventHandler(this.listViewPages_VirtualItemsSelectionRangeChanged);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "pdf";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButtonZoomIn,
            this.toolStripDropDownButtonZoomOut,
            this.toolStripDropDownButtonSave,
            this.toolStripStatusLabelInfo});
            this.statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip.Location = new System.Drawing.Point(0, 431);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.ShowItemToolTips = true;
            this.statusStrip.Size = new System.Drawing.Size(632, 22);
            this.statusStrip.TabIndex = 1;
            // 
            // toolStripDropDownButtonZoomIn
            // 
            this.toolStripDropDownButtonZoomIn.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripDropDownButtonZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButtonZoomIn.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonZoomIn.Image")));
            this.toolStripDropDownButtonZoomIn.Name = "toolStripDropDownButtonZoomIn";
            this.toolStripDropDownButtonZoomIn.ShowDropDownArrow = false;
            this.toolStripDropDownButtonZoomIn.Size = new System.Drawing.Size(20, 20);
            // 
            // toolStripDropDownButtonZoomOut
            // 
            this.toolStripDropDownButtonZoomOut.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripDropDownButtonZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButtonZoomOut.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonZoomOut.Image")));
            this.toolStripDropDownButtonZoomOut.Name = "toolStripDropDownButtonZoomOut";
            this.toolStripDropDownButtonZoomOut.ShowDropDownArrow = false;
            this.toolStripDropDownButtonZoomOut.Size = new System.Drawing.Size(20, 20);
            // 
            // toolStripDropDownButtonSave
            // 
            this.toolStripDropDownButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButtonSave.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonSave.Image")));
            this.toolStripDropDownButtonSave.Name = "toolStripDropDownButtonSave";
            this.toolStripDropDownButtonSave.ShowDropDownArrow = false;
            this.toolStripDropDownButtonSave.Size = new System.Drawing.Size(20, 20);
            // 
            // toolStripStatusLabelInfo
            // 
            this.toolStripStatusLabelInfo.Name = "toolStripStatusLabelInfo";
            this.toolStripStatusLabelInfo.Size = new System.Drawing.Size(109, 17);
            this.toolStripStatusLabelInfo.Text = "toolStripStatusLabel1";
            // 
            // ExtractForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 453);
            this.Controls.Add(this.listViewPages);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ExtractForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewPages;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonZoomIn;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonZoomOut;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonSave;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelInfo;
    }
}