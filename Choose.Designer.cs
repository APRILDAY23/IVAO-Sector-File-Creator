namespace Sector_File
{
    partial class Choose
    {
        private System.ComponentModel.IContainer components = null;
        private Button flightOperationsButton;
        private Button atcOperationsButton;

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
            this.flightOperationsButton = new Button();
            this.atcOperationsButton = new Button();

            this.SuspendLayout();

            // 
            // flightOperationsButton
            // 
            this.flightOperationsButton.Size = new System.Drawing.Size(200, 50);
            this.flightOperationsButton.Text = "Flight Operations";
            this.flightOperationsButton.UseVisualStyleBackColor = true;
            this.flightOperationsButton.Click += new EventHandler(this.flightOperationsButton_Click);

            // 
            // atcOperationsButton
            // 
            this.atcOperationsButton.Size = new System.Drawing.Size(200, 50);
            this.atcOperationsButton.Text = "ATC Operations";
            this.atcOperationsButton.UseVisualStyleBackColor = true;
            this.atcOperationsButton.Click += new EventHandler(this.atcOperationsButton_Click);

            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 200); // Form size to accommodate centered buttons
            this.Controls.Add(this.flightOperationsButton);
            this.Controls.Add(this.atcOperationsButton);
            this.Text = "Choose Operations | IVAO Utilities";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.ResumeLayout(false);

            // Positioning the buttons
            int buttonSpacing = 10;
            int totalButtonWidth = (this.flightOperationsButton.Width + this.atcOperationsButton.Width + buttonSpacing);


            // Center buttons horizontally
            this.flightOperationsButton.Location = new System.Drawing.Point(
                (this.ClientSize.Width - totalButtonWidth) / 2,
                (this.ClientSize.Height - this.flightOperationsButton.Height) / 2
            );

            this.atcOperationsButton.Location = new System.Drawing.Point(
                this.flightOperationsButton.Location.X + this.flightOperationsButton.Width + buttonSpacing,
                this.flightOperationsButton.Location.Y
            );
        }

        #endregion
    }
}
