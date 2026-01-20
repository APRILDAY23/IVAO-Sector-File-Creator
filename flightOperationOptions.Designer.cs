namespace Sector_File
{
    partial class flightOperationOptions
    {
        private System.ComponentModel.IContainer components = null;
        private Button getSchedulesData;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.getSchedulesData = new Button();

            this.SuspendLayout();

            // 
            // getSchedulesData
            // 
            this.getSchedulesData.Size = new System.Drawing.Size(200, 50);
            this.getSchedulesData.Text = "Get Flight Schedules Data";
            this.getSchedulesData.UseVisualStyleBackColor = true;
            this.getSchedulesData.Click += new EventHandler(this.getSchedulesButton_Click);

            // 
            // flightOperationOptions Form
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 250); // Form size to accommodate centered button
            this.Controls.Add(this.getSchedulesData);
            this.Text = "Flight Operations Menu | IVAO Flight Utilities";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.ResumeLayout(false);

            // Center the button
            this.getSchedulesData.Location = new System.Drawing.Point(
                (this.ClientSize.Width - this.getSchedulesData.Width) / 2,
                (this.ClientSize.Height - this.getSchedulesData.Height) / 2
            );
        }

        #endregion
    }
}
