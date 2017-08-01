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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.openFilesDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.progressBarStatus = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.toolStripFiles.SuspendLayout();
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
            this.listViewFiles.AllowDrop = true;
            this.listViewFiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderPages,
            this.columnHeaderPath});
            resources.ApplyResources(this.listViewFiles, "listViewFiles");
            this.listViewFiles.FullRowSelect = true;
            this.listViewFiles.HideSelection = false;
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.View = System.Windows.Forms.View.Details;
            this.listViewFiles.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewFiles_ItemDrag);
            this.listViewFiles.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewFiles_ItemSelectionChanged);
            this.listViewFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewFiles_DragDrop);
            this.listViewFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewFiles_DragOver);
            this.listViewFiles.DragOver += new System.Windows.Forms.DragEventHandler(this.listViewFiles_DragOver);
            this.listViewFiles.DragLeave += new System.EventHandler(this.listViewFiles_DragLeave);
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
            this.toolStripButtonInsert.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonInsert, "toolStripButtonInsert");
            this.toolStripButtonInsert.Name = "toolStripButtonInsert";
            this.toolStripButtonInsert.Click += new System.EventHandler(this.toolStripButtonInsert_Click);
            // 
            // toolStripButtonRemove
            // 
            this.toolStripButtonRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonRemove, "toolStripButtonRemove");
            this.toolStripButtonRemove.Name = "toolStripButtonRemove";
            this.toolStripButtonRemove.Click += new System.EventHandler(this.toolStripButtonRemove_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // toolStripButtonUp
            // 
            this.toolStripButtonUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonUp, "toolStripButtonUp");
            this.toolStripButtonUp.Name = "toolStripButtonUp";
            this.toolStripButtonUp.Click += new System.EventHandler(this.toolStripButtonUp_Click);
            // 
            // toolStripButtonDown
            // 
            this.toolStripButtonDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonDown, "toolStripButtonDown");
            this.toolStripButtonDown.Name = "toolStripButtonDown";
            this.toolStripButtonDown.Click += new System.EventHandler(this.toolStripButtonDown_Click);
            // 
            // viewer
            // 
            resources.ApplyResources(this.viewer, "viewer");
            this.viewer.Name = "viewer";
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // openFilesDialog
            // 
            this.openFilesDialog.DefaultExt = "pdf";
            resources.ApplyResources(this.openFilesDialog, "openFilesDialog");
            this.openFilesDialog.Multiselect = true;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "pdf";
            resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
            // 
            // progressBarStatus
            // 
            resources.ApplyResources(this.progressBarStatus, "progressBarStatus");
            this.progressBarStatus.Name = "progressBarStatus";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // CombineForm
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.progressBarStatus);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.splitContainer);
            this.Name = "CombineForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Shown += new System.EventHandler(this.CombineForm_Shown);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.toolStripFiles.ResumeLayout(false);
            this.toolStripFiles.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
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
        private System.Windows.Forms.ProgressBar progressBarStatus;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private Viewer viewer;
    }
}