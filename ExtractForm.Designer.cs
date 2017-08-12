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
            this.components = new System.ComponentModel.Container();
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
            this.toolStripDropDownButtonCancel = new System.Windows.Forms.ToolStripDropDownButton();
            this.backgroundWorkerExtract = new System.ComponentModel.BackgroundWorker();
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
            this.listViewPages.CacheVirtualItems += new System.Windows.Forms.CacheVirtualItemsEventHandler(this.listViewPages_CacheVirtualItems);
            this.listViewPages.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewPages_ItemDrag);
            this.listViewPages.MouseLeave += new System.EventHandler(this.listViewPages_MouseLeave);
            this.listViewPages.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listViewPages_MouseMove);
            this.listViewPages.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listViewPages_RetrieveVirtualItem);
            this.listViewPages.SelectedIndexChanged += new System.EventHandler(this.listViewPages_SelectedIndexChanged);
            this.listViewPages.VirtualItemsSelectionRangeChanged += new System.Windows.Forms.ListViewVirtualItemsSelectionRangeChangedEventHandler(this.listViewPages_VirtualItemsSelectionRangeChanged);
            // 
            // imageList
            // 
            resources.ApplyResources(this.imageList, "imageList");
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // folderBrowserDialog
            // 
            resources.ApplyResources(this.folderBrowserDialog, "folderBrowserDialog");
            // 
            // saveFileDialog
            // 
            resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
            this.saveFileDialog.DefaultExt = "pdf";
            // 
            // statusStrip
            // 
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButtonZoomIn,
            this.toolStripDropDownButtonZoomOut,
            this.toolStripDropDownButtonSave,
            this.toolStripStatusLabelSingleFiles,
            this.toolStripProgressBarExtract,
            this.toolStripStatusLabelExtract,
            this.toolStripDropDownButtonCancel});
            this.statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.ShowItemToolTips = true;
            // 
            // toolStripDropDownButtonZoomIn
            // 
            resources.ApplyResources(this.toolStripDropDownButtonZoomIn, "toolStripDropDownButtonZoomIn");
            this.toolStripDropDownButtonZoomIn.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripDropDownButtonZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButtonZoomIn.Name = "toolStripDropDownButtonZoomIn";
            this.toolStripDropDownButtonZoomIn.ShowDropDownArrow = false;
            this.toolStripDropDownButtonZoomIn.Click += new System.EventHandler(this.toolStripDropDownButtonZoomIn_Click);
            // 
            // toolStripDropDownButtonZoomOut
            // 
            resources.ApplyResources(this.toolStripDropDownButtonZoomOut, "toolStripDropDownButtonZoomOut");
            this.toolStripDropDownButtonZoomOut.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripDropDownButtonZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButtonZoomOut.Name = "toolStripDropDownButtonZoomOut";
            this.toolStripDropDownButtonZoomOut.ShowDropDownArrow = false;
            this.toolStripDropDownButtonZoomOut.Click += new System.EventHandler(this.toolStripDropDownButtonZoomOut_Click);
            // 
            // toolStripDropDownButtonSave
            // 
            resources.ApplyResources(this.toolStripDropDownButtonSave, "toolStripDropDownButtonSave");
            this.toolStripDropDownButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButtonSave.Name = "toolStripDropDownButtonSave";
            this.toolStripDropDownButtonSave.ShowDropDownArrow = false;
            this.toolStripDropDownButtonSave.Click += new System.EventHandler(this.toolStripDropDownButtonSave_Click);
            // 
            // toolStripStatusLabelSingleFiles
            // 
            resources.ApplyResources(this.toolStripStatusLabelSingleFiles, "toolStripStatusLabelSingleFiles");
            this.toolStripStatusLabelSingleFiles.Name = "toolStripStatusLabelSingleFiles";
            // 
            // toolStripProgressBarExtract
            // 
            resources.ApplyResources(this.toolStripProgressBarExtract, "toolStripProgressBarExtract");
            this.toolStripProgressBarExtract.Name = "toolStripProgressBarExtract";
            // 
            // toolStripStatusLabelExtract
            // 
            resources.ApplyResources(this.toolStripStatusLabelExtract, "toolStripStatusLabelExtract");
            this.toolStripStatusLabelExtract.Name = "toolStripStatusLabelExtract";
            // 
            // toolStripDropDownButtonCancel
            // 
            resources.ApplyResources(this.toolStripDropDownButtonCancel, "toolStripDropDownButtonCancel");
            this.toolStripDropDownButtonCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButtonCancel.Name = "toolStripDropDownButtonCancel";
            this.toolStripDropDownButtonCancel.ShowDropDownArrow = false;
            this.toolStripDropDownButtonCancel.Click += new System.EventHandler(this.toolStripDropDownButtonCancel_Click);
            // 
            // backgroundWorkerExtract
            // 
            this.backgroundWorkerExtract.WorkerReportsProgress = true;
            this.backgroundWorkerExtract.WorkerSupportsCancellation = true;
            this.backgroundWorkerExtract.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerExtract_DoWork);
            this.backgroundWorkerExtract.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerExtract_ProgressChanged);
            this.backgroundWorkerExtract.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerExtract_RunWorkerCompleted);
            // 
            // ExtractForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
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
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBarExtract;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelExtract;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonCancel;
        private System.ComponentModel.BackgroundWorker backgroundWorkerExtract;
    }
}