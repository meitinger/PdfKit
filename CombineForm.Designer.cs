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
            this.panelPreview = new System.Windows.Forms.Panel();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.toolStripPreview = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonFirst = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPrevious = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripTextBoxPage = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabelTotal = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonNext = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLast = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonZoomIn = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonZoomOut = new System.Windows.Forms.ToolStripButton();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.openFilesDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.toolStripFiles.SuspendLayout();
            this.panelPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.toolStripPreview.SuspendLayout();
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
            this.splitContainer.Panel2.Controls.Add(this.panelPreview);
            this.splitContainer.Panel2.Controls.Add(this.toolStripPreview);
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
            // panelPreview
            // 
            resources.ApplyResources(this.panelPreview, "panelPreview");
            this.panelPreview.Controls.Add(this.pictureBoxPreview);
            this.panelPreview.Name = "panelPreview";
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.Cursor = System.Windows.Forms.Cursors.NoMove2D;
            resources.ApplyResources(this.pictureBoxPreview, "pictureBoxPreview");
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.TabStop = false;
            this.pictureBoxPreview.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxPreview_MouseDown);
            this.pictureBoxPreview.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxPreview_MouseMove);
            this.pictureBoxPreview.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxPreview_MouseUp);
            // 
            // toolStripPreview
            // 
            resources.ApplyResources(this.toolStripPreview, "toolStripPreview");
            this.toolStripPreview.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripPreview.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonFirst,
            this.toolStripButtonPrevious,
            this.toolStripSeparator1,
            this.toolStripTextBoxPage,
            this.toolStripLabelTotal,
            this.toolStripSeparator2,
            this.toolStripButtonNext,
            this.toolStripButtonLast,
            this.toolStripSeparator3,
            this.toolStripButtonZoomIn,
            this.toolStripButtonZoomOut});
            this.toolStripPreview.Name = "toolStripPreview";
            // 
            // toolStripButtonFirst
            // 
            this.toolStripButtonFirst.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonFirst, "toolStripButtonFirst");
            this.toolStripButtonFirst.Name = "toolStripButtonFirst";
            this.toolStripButtonFirst.Click += new System.EventHandler(this.toolStripButtonFirst_Click);
            // 
            // toolStripButtonPrevious
            // 
            this.toolStripButtonPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonPrevious, "toolStripButtonPrevious");
            this.toolStripButtonPrevious.Name = "toolStripButtonPrevious";
            this.toolStripButtonPrevious.Click += new System.EventHandler(this.toolStripButtonPrevious_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripTextBoxPage
            // 
            this.toolStripTextBoxPage.Name = "toolStripTextBoxPage";
            resources.ApplyResources(this.toolStripTextBoxPage, "toolStripTextBoxPage");
            this.toolStripTextBoxPage.Validated += new System.EventHandler(this.toolStripTextBoxPage_Validated);
            // 
            // toolStripLabelTotal
            // 
            this.toolStripLabelTotal.Name = "toolStripLabelTotal";
            resources.ApplyResources(this.toolStripLabelTotal, "toolStripLabelTotal");
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // toolStripButtonNext
            // 
            this.toolStripButtonNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonNext, "toolStripButtonNext");
            this.toolStripButtonNext.Name = "toolStripButtonNext";
            this.toolStripButtonNext.Click += new System.EventHandler(this.toolStripButtonNext_Click);
            // 
            // toolStripButtonLast
            // 
            this.toolStripButtonLast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonLast, "toolStripButtonLast");
            this.toolStripButtonLast.Name = "toolStripButtonLast";
            this.toolStripButtonLast.Click += new System.EventHandler(this.toolStripButtonLast_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // toolStripButtonZoomIn
            // 
            this.toolStripButtonZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonZoomIn, "toolStripButtonZoomIn");
            this.toolStripButtonZoomIn.Name = "toolStripButtonZoomIn";
            this.toolStripButtonZoomIn.Click += new System.EventHandler(this.toolStripButtonZoomIn_Click);
            // 
            // toolStripButtonZoomOut
            // 
            this.toolStripButtonZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonZoomOut, "toolStripButtonZoomOut");
            this.toolStripButtonZoomOut.Name = "toolStripButtonZoomOut";
            this.toolStripButtonZoomOut.Click += new System.EventHandler(this.toolStripButtonZoomOut_Click);
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
            resources.ApplyResources(this.openFilesDialog, "openFilesDialog");
            this.openFilesDialog.DefaultExt = "pdf";
            this.openFilesDialog.Multiselect = true;
            // 
            // saveFileDialog
            // 
            resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
            this.saveFileDialog.DefaultExt = "pdf";
            // 
            // CombineForm
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.splitContainer);
            this.Name = "CombineForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Shown += new System.EventHandler(this.CombineForm_Shown);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.toolStripFiles.ResumeLayout(false);
            this.toolStripFiles.PerformLayout();
            this.panelPreview.ResumeLayout(false);
            this.panelPreview.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.toolStripPreview.ResumeLayout(false);
            this.toolStripPreview.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ToolStrip toolStripPreview;
        private System.Windows.Forms.ToolStripButton toolStripButtonFirst;
        private System.Windows.Forms.ToolStripButton toolStripButtonPrevious;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxPage;
        private System.Windows.Forms.ToolStripLabel toolStripLabelTotal;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonNext;
        private System.Windows.Forms.ToolStripButton toolStripButtonLast;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonZoomIn;
        private System.Windows.Forms.ToolStripButton toolStripButtonZoomOut;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.ListView listViewFiles;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderPages;
        private System.Windows.Forms.ColumnHeader columnHeaderPath;
        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.ToolStrip toolStripFiles;
        private System.Windows.Forms.ToolStripButton toolStripButtonInsert;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemove;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripButtonUp;
        private System.Windows.Forms.ToolStripButton toolStripButtonDown;
        private System.Windows.Forms.OpenFileDialog openFilesDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}