namespace Sector_File
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private Button chooseKmlButton;
        private Button getCountryDataButton;
        private Button getSidAndStarDataButton;
        private Button getFirDataButton; // New button for FIR Data
        private Button getAirportDataButton; // New button for Airport Data

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
            this.chooseKmlButton = new Button();
            this.getCountryDataButton = new Button();
            this.getSidAndStarDataButton = new Button();
            this.getFirDataButton = new Button(); // Initialize FIR Data button
            this.getAirportDataButton = new Button(); // Initialize Airport Data button

            this.SuspendLayout();

            // 
            // chooseKmlButton
            // 
            this.chooseKmlButton.Size = new System.Drawing.Size(200, 50);
            this.chooseKmlButton.Text = "Choose KML file";
            this.chooseKmlButton.UseVisualStyleBackColor = true;
            this.chooseKmlButton.Click += new EventHandler(this.chooseKmlButton_Click);

            // 
            // getCountryDataButton
            // 
            this.getCountryDataButton.Size = new System.Drawing.Size(200, 50);
            this.getCountryDataButton.Text = "Get Country Data";
            this.getCountryDataButton.UseVisualStyleBackColor = true;
            this.getCountryDataButton.Click += new EventHandler(this.getCountryDataButton_Click);

            // 
            // getSidAndStarDataButton
            // 
            this.getSidAndStarDataButton.Size = new System.Drawing.Size(200, 50);
            this.getSidAndStarDataButton.Text = "Get SID and STAR Data";
            this.getSidAndStarDataButton.UseVisualStyleBackColor = true;
            this.getSidAndStarDataButton.Click += new EventHandler(this.getSidAndStarDataButton_Click);

            // 
            // getFirDataButton
            // 
            this.getFirDataButton.Size = new System.Drawing.Size(200, 50);
            this.getFirDataButton.Text = "Get FIR Data";
            this.getFirDataButton.UseVisualStyleBackColor = true;
            this.getFirDataButton.Click += new EventHandler(this.getFirDataButton_Click);

            // 
            // getAirportDataButton
            // 
            this.getAirportDataButton.Size = new System.Drawing.Size(200, 50);
            this.getAirportDataButton.Text = "Get Airport Data";
            this.getAirportDataButton.UseVisualStyleBackColor = true;
            this.getAirportDataButton.Click += new EventHandler(this.getAirportDataButton_Click);

            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 250); // Adjust form size to accommodate buttons
            this.Controls.Add(this.chooseKmlButton);
            this.Controls.Add(this.getCountryDataButton);
            this.Controls.Add(this.getSidAndStarDataButton);
            this.Controls.Add(this.getFirDataButton);
            this.Controls.Add(this.getAirportDataButton);
            this.Text = "ATC Operations Menu | IVAO ATC Utilities";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.ResumeLayout(false);

            // Positioning the buttons
            int buttonSpacing = 10;

            // First row buttons
            this.chooseKmlButton.Location = new System.Drawing.Point(
                (this.ClientSize.Width - (this.chooseKmlButton.Width * 3 + buttonSpacing * 2)) / 2,
                50
            );

            this.getCountryDataButton.Location = new System.Drawing.Point(
                this.chooseKmlButton.Location.X + this.chooseKmlButton.Width + buttonSpacing,
                this.chooseKmlButton.Location.Y
            );

            this.getSidAndStarDataButton.Location = new System.Drawing.Point(
                this.getCountryDataButton.Location.X + this.getCountryDataButton.Width + buttonSpacing,
                this.chooseKmlButton.Location.Y
            );

            // Second row buttons
            this.getFirDataButton.Location = new System.Drawing.Point(
                this.chooseKmlButton.Location.X,
                this.chooseKmlButton.Location.Y + this.chooseKmlButton.Height + buttonSpacing
            );

            this.getAirportDataButton.Location = new System.Drawing.Point(
                this.getFirDataButton.Location.X + this.getFirDataButton.Width + buttonSpacing,
                this.getFirDataButton.Location.Y
            );
        }

        #endregion
    }
}
