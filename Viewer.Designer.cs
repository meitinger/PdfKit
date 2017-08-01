namespace Aufbauwerk.Tools.PdfKit
{
    partial class Viewer
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Viewer));
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
            this.panelPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.toolStripPreview.SuspendLayout();
            this.SuspendLayout();
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
            // Viewer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelPreview);
            this.Controls.Add(this.toolStripPreview);
            this.Name = "Viewer";
            this.panelPreview.ResumeLayout(false);
            this.panelPreview.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.toolStripPreview.ResumeLayout(false);
            this.toolStripPreview.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
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
    }
}
