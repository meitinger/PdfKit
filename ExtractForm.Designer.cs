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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtractForm));
            this.listViewPages = new System.Windows.Forms.ListView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripDropDownButtonZoomIn = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownButtonZoomOut = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownButtonSave = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripStatusLabelSingleFiles = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBarExtract = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabelExtract = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTipPreview = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewPages
            // 
            resources.ApplyResources(this.listViewPages, "listViewPages");
            this.listViewPages.HideSelection = false;
            this.listViewPages.LargeImageList = this.imageList;
            this.listViewPages.Name = "listViewPages";
            this.listViewPages.UseCompatibleStateImageBehavior = false;
            this.listViewPages.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewPages_ItemDrag);
            this.listViewPages.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listViewPages_RetrieveVirtualItem);
            this.listViewPages.SelectedIndexChanged += new System.EventHandler(this.listViewPages_SelectedIndexChanged);
            this.listViewPages.VirtualItemsSelectionRangeChanged += new System.Windows.Forms.ListViewVirtualItemsSelectionRangeChangedEventHandler(this.listViewPages_VirtualItemsSelectionRangeChanged);
            this.listViewPages.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(this.listViewPages_QueryContinueDrag);
            this.listViewPages.MouseLeave += new System.EventHandler(this.listViewPages_MouseLeave);
            this.listViewPages.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listViewPages_MouseMove);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            resources.ApplyResources(this.imageList, "imageList");
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // saveFileDialog
            // 
            resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
            this.saveFileDialog.DefaultExt = "pdf";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButtonZoomIn,
            this.toolStripDropDownButtonZoomOut,
            this.toolStripDropDownButtonSave,
            this.toolStripStatusLabelSingleFiles,
            this.toolStripProgressBarExtract,
            this.toolStripStatusLabelExtract});
            this.statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.ShowItemToolTips = true;
            // 
            // toolStripDropDownButtonZoomIn
            // 
            this.toolStripDropDownButtonZoomIn.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripDropDownButtonZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripDropDownButtonZoomIn, "toolStripDropDownButtonZoomIn");
            this.toolStripDropDownButtonZoomIn.Name = "toolStripDropDownButtonZoomIn";
            this.toolStripDropDownButtonZoomIn.ShowDropDownArrow = false;
            this.toolStripDropDownButtonZoomIn.Click += new System.EventHandler(this.toolStripDropDownButtonZoomIn_Click);
            // 
            // toolStripDropDownButtonZoomOut
            // 
            this.toolStripDropDownButtonZoomOut.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripDropDownButtonZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripDropDownButtonZoomOut, "toolStripDropDownButtonZoomOut");
            this.toolStripDropDownButtonZoomOut.Name = "toolStripDropDownButtonZoomOut";
            this.toolStripDropDownButtonZoomOut.ShowDropDownArrow = false;
            this.toolStripDropDownButtonZoomOut.Click += new System.EventHandler(this.toolStripDropDownButtonZoomOut_Click);
            // 
            // toolStripDropDownButtonSave
            // 
            this.toolStripDropDownButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripDropDownButtonSave, "toolStripDropDownButtonSave");
            this.toolStripDropDownButtonSave.Name = "toolStripDropDownButtonSave";
            this.toolStripDropDownButtonSave.ShowDropDownArrow = false;
            this.toolStripDropDownButtonSave.Click += new System.EventHandler(this.toolStripDropDownButtonSave_Click);
            // 
            // toolStripStatusLabelSingleFiles
            // 
            this.toolStripStatusLabelSingleFiles.Name = "toolStripStatusLabelSingleFiles";
            resources.ApplyResources(this.toolStripStatusLabelSingleFiles, "toolStripStatusLabelSingleFiles");
            // 
            // toolStripProgressBarExtract
            // 
            this.toolStripProgressBarExtract.Name = "toolStripProgressBarExtract";
            resources.ApplyResources(this.toolStripProgressBarExtract, "toolStripProgressBarExtract");
            // 
            // toolStripStatusLabelExtract
            // 
            this.toolStripStatusLabelExtract.Name = "toolStripStatusLabelExtract";
            resources.ApplyResources(this.toolStripStatusLabelExtract, "toolStripStatusLabelExtract");
            // 
            // toolTipPreview
            // 
            this.toolTipPreview.OwnerDraw = true;
            this.toolTipPreview.Draw += new System.Windows.Forms.DrawToolTipEventHandler(this.toolTipPreview_Draw);
            this.toolTipPreview.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTipPreview_Popup);
            // 
            // ExtractForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listViewPages);
            this.Controls.Add(this.statusStrip);
            this.Name = "ExtractForm";
            this.Shown += new System.EventHandler(this.ExtractForm_Shown);
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
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelSingleFiles;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolTip toolTipPreview;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBarExtract;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelExtract;
    }
}