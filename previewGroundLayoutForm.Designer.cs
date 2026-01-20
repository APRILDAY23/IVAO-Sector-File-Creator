namespace Sector_File
{
    partial class previewGroundLayoutForm
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
        /// 
        private System.Windows.Forms.Panel plotCanvas;
        private void InitializeComponent()
        {
            this.plotCanvas = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // plotCanvas
            // 
            this.plotCanvas.Location = new System.Drawing.Point(12, 12); // Position the panel on the form
            this.plotCanvas.Name = "plotCanvas";
            this.plotCanvas.Size = new System.Drawing.Size(700, 400); // Set the canvas size
            this.plotCanvas.TabIndex = 0;
            this.plotCanvas.Dock = DockStyle.Fill;
            this.plotCanvas.BackColor = System.Drawing.Color.FromArgb(0, 24, 36); // Set background color to match your XAML example

            // 
            // previewGroundLayoutForm
            // 
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.plotCanvas);
            this.Text = "Preview the ground layout | IVAO ATC Utilities";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}