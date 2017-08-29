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
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemSelectOdd = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSelectEven = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemSelectInvert = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripDropDownButtonZoomIn = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownButtonZoomOut = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownButtonSave = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownButtonFormat = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripStatusLabelMultipleFiles = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBarExtract = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabelExtract = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripDropDownButtonCancel = new System.Windows.Forms.ToolStripDropDownButton();
            this.backgroundWorkerExtract = new System.ComponentModel.BackgroundWorker();
            this.contextMenuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewPages
            // 
            resources.ApplyResources(this.listViewPages, "listViewPages");
            this.listViewPages.ContextMenuStrip = this.contextMenuStrip;
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
            // contextMenuStrip
            // 
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemSelectOdd,
            this.toolStripMenuItemSelectEven,
            this.toolStripSeparator1,
            this.toolStripMenuItemSelectAll,
            this.toolStripSeparator2,
            this.toolStripMenuItemSelectInvert});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.ShowImageMargin = false;
            // 
            // toolStripMenuItemSelectOdd
            // 
            resources.ApplyResources(this.toolStripMenuItemSelectOdd, "toolStripMenuItemSelectOdd");
            this.toolStripMenuItemSelectOdd.Name = "toolStripMenuItemSelectOdd";
            this.toolStripMenuItemSelectOdd.Click += new System.EventHandler(this.toolStripMenuItemSelectOdd_Click);
            // 
            // toolStripMenuItemSelectEven
            // 
            resources.ApplyResources(this.toolStripMenuItemSelectEven, "toolStripMenuItemSelectEven");
            this.toolStripMenuItemSelectEven.Name = "toolStripMenuItemSelectEven";
            this.toolStripMenuItemSelectEven.Click += new System.EventHandler(this.toolStripMenuItemSelectEven_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // toolStripMenuItemSelectAll
            // 
            resources.ApplyResources(this.toolStripMenuItemSelectAll, "toolStripMenuItemSelectAll");
            this.toolStripMenuItemSelectAll.Name = "toolStripMenuItemSelectAll";
            this.toolStripMenuItemSelectAll.Click += new System.EventHandler(this.toolStripMenuItemSelectAll_Click);
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // toolStripMenuItemSelectInvert
            // 
            resources.ApplyResources(this.toolStripMenuItemSelectInvert, "toolStripMenuItemSelectInvert");
            this.toolStripMenuItemSelectInvert.Name = "toolStripMenuItemSelectInvert";
            this.toolStripMenuItemSelectInvert.Click += new System.EventHandler(this.toolStripMenuItemSelectInvert_Click);
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
            // 
            // statusStrip
            // 
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButtonZoomIn,
            this.toolStripDropDownButtonZoomOut,
            this.toolStripDropDownButtonSave,
            this.toolStripDropDownButtonFormat,
            this.toolStripStatusLabelMultipleFiles,
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
            // toolStripDropDownButtonFormat
            // 
            resources.ApplyResources(this.toolStripDropDownButtonFormat, "toolStripDropDownButtonFormat");
            this.toolStripDropDownButtonFormat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButtonFormat.Name = "toolStripDropDownButtonFormat";
            this.toolStripDropDownButtonFormat.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStripDropDownButtonFormat_DropDownItemClicked);
            // 
            // toolStripStatusLabelMultipleFiles
            // 
            resources.ApplyResources(this.toolStripStatusLabelMultipleFiles, "toolStripStatusLabelMultipleFiles");
            this.toolStripStatusLabelMultipleFiles.Name = "toolStripStatusLabelMultipleFiles";
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
            this.contextMenuStrip.ResumeLayout(false);
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
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelMultipleFiles;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBarExtract;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelExtract;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonCancel;
        private System.ComponentModel.BackgroundWorker backgroundWorkerExtract;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonFormat;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonSave;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSelectOdd;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSelectEven;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSelectAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSelectInvert;
    }
}