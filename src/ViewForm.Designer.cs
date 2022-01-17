namespace Aufbauwerk.Tools.PdfKit
{
    partial class ViewForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewForm));
            this.viewer = new Aufbauwerk.Tools.PdfKit.Viewer();
            this.SuspendLayout();
            // 
            // viewer
            // 
            resources.ApplyResources(this.viewer, "viewer");
            this.viewer.Name = "viewer";
            // 
            // ViewForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.viewer);
            this.Name = "ViewForm";
            this.ResumeLayout(false);

        }

        #endregion

        private Viewer viewer;
    }
}