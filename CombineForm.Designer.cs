namespace Aufbauwerk.Tools.PdfKit
{
    partial class CombineForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CombineForm));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.listViewFiles = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderPages = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStripFiles = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonInsert = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRemove = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonUp = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDown = new System.Windows.Forms.ToolStripButton();
            this.viewer = new Aufbauwerk.Tools.PdfKit.Viewer();
            this.openFilesDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripDropDownButtonCancel = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownButtonSave = new System.Windows.Forms.ToolStripDropDownButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.toolStripFiles.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            resources.ApplyResources(this.splitContainer, "splitContainer");
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.listViewFiles);
            this.splitContainer.Panel1.Controls.Add(this.toolStripFiles);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.viewer);
            // 
            // listViewFiles
            // 
            resources.ApplyResources(this.listViewFiles, "listViewFiles");
            this.listViewFiles.AllowDrop = true;
            this.listViewFiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderPages,
            this.columnHeaderPath});
            this.listViewFiles.FullRowSelect = true;
            this.listViewFiles.HideSelection = false;
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.View = System.Windows.Forms.View.Details;
            this.listViewFiles.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.ListViewFiles_ItemDrag);
            this.listViewFiles.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.ListViewFiles_ItemSelectionChanged);
            this.listViewFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListViewFiles_DragDrop);
            this.listViewFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.ListViewFiles_DragOver);
            this.listViewFiles.DragOver += new System.Windows.Forms.DragEventHandler(this.ListViewFiles_DragOver);
            this.listViewFiles.DragLeave += new System.EventHandler(this.ListViewFiles_DragLeave);
            // 
            // columnHeaderName
            // 
            resources.ApplyResources(this.columnHeaderName, "columnHeaderName");
            // 
            // columnHeaderPages
            // 
            resources.ApplyResources(this.columnHeaderPages, "columnHeaderPages");
            // 
            // columnHeaderPath
            // 
            resources.ApplyResources(this.columnHeaderPath, "columnHeaderPath");
            // 
            // toolStripFiles
            // 
            resources.ApplyResources(this.toolStripFiles, "toolStripFiles");
            this.toolStripFiles.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripFiles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonInsert,
            this.toolStripButtonRemove,
            this.toolStripSeparator4,
            this.toolStripButtonUp,
            this.toolStripButtonDown});
            this.toolStripFiles.Name = "toolStripFiles";
            // 
            // toolStripButtonInsert
            // 
            resources.ApplyResources(this.toolStripButtonInsert, "toolStripButtonInsert");
            this.toolStripButtonInsert.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonInsert.Name = "toolStripButtonInsert";
            this.toolStripButtonInsert.Click += new System.EventHandler(this.ToolStripButtonInsert_Click);
            // 
            // toolStripButtonRemove
            // 
            resources.ApplyResources(this.toolStripButtonRemove, "toolStripButtonRemove");
            this.toolStripButtonRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRemove.Name = "toolStripButtonRemove";
            this.toolStripButtonRemove.Click += new System.EventHandler(this.ToolStripButtonRemove_Click);
            // 
            // toolStripSeparator4
            // 
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            // 
            // toolStripButtonUp
            // 
            resources.ApplyResources(this.toolStripButtonUp, "toolStripButtonUp");
            this.toolStripButtonUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUp.Name = "toolStripButtonUp";
            this.toolStripButtonUp.Click += new System.EventHandler(this.ToolStripButtonUp_Click);
            // 
            // toolStripButtonDown
            // 
            resources.ApplyResources(this.toolStripButtonDown, "toolStripButtonDown");
            this.toolStripButtonDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDown.Name = "toolStripButtonDown";
            this.toolStripButtonDown.Click += new System.EventHandler(this.ToolStripButtonDown_Click);
            // 
            // viewer
            // 
            resources.ApplyResources(this.viewer, "viewer");
            this.viewer.Name = "viewer";
            // 
            // openFilesDialog
            // 
            resources.ApplyResources(this.openFilesDialog, "openFilesDialog");
            this.openFilesDialog.DefaultExt = "pdf";
            this.openFilesDialog.Multiselect = true;
            // 
            // saveFileDialog
            // 
            resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
            this.saveFileDialog.DefaultExt = "pdf";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker_RunWorkerCompleted);
            // 
            // statusStrip
            // 
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButtonSave,
            this.toolStripProgressBar,
            this.toolStripDropDownButtonCancel});
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.ShowItemToolTips = true;
            // 
            // toolStripProgressBar
            // 
            resources.ApplyResources(this.toolStripProgressBar, "toolStripProgressBar");
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            // 
            // toolStripDropDownButtonCancel
            // 
            resources.ApplyResources(this.toolStripDropDownButtonCancel, "toolStripDropDownButtonCancel");
            this.toolStripDropDownButtonCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButtonCancel.Name = "toolStripDropDownButtonCancel";
            this.toolStripDropDownButtonCancel.ShowDropDownArrow = false;
            this.toolStripDropDownButtonCancel.Click += new System.EventHandler(this.ToolStripDropDownButtonCancel_Click);
            // 
            // toolStripDropDownButtonSave
            // 
            resources.ApplyResources(this.toolStripDropDownButtonSave, "toolStripDropDownButtonSave");
            this.toolStripDropDownButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButtonSave.Name = "toolStripDropDownButtonSave";
            this.toolStripDropDownButtonSave.ShowDropDownArrow = false;
            this.toolStripDropDownButtonSave.Click += new System.EventHandler(this.ToolStripDropDownButtonSave_Click);
            // 
            // CombineForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.statusStrip);
            this.Name = "CombineForm";
            this.Shown += new System.EventHandler(this.CombineForm_Shown);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.toolStripFiles.ResumeLayout(false);
            this.toolStripFiles.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ListView listViewFiles;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderPages;
        private System.Windows.Forms.ColumnHeader columnHeaderPath;
        private System.Windows.Forms.ToolStrip toolStripFiles;
        private System.Windows.Forms.ToolStripButton toolStripButtonInsert;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemove;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripButtonUp;
        private System.Windows.Forms.ToolStripButton toolStripButtonDown;
        private System.Windows.Forms.OpenFileDialog openFilesDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private Viewer viewer;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonCancel;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonSave;
    }
}