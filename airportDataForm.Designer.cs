namespace Sector_File
{
    partial class airportDataForm
    {
        private System.ComponentModel.IContainer components = null;
        private ProgressBar loadingProgressBar;  // Progress bar for loading status
        private RichTextBox logRichTextBox;       // Debug output box
        private TextBox searchBox;                // Search input box
        private Button searchButton;              // Search button
        private Button downloadAirportData;       // Download Airport Data button
        private Button downloadRunwayData;        // Download Runway Data button
        private Button airportFrequencyData;      // Download Airport Frequency Data button
        private Button downloadVORDMEData;        // New button for VOR/DME Data
        private Button downloadNDBData;        // New button for VOR/DME Data
        private Button downloadGateData;        // New button for VOR/DME Data
        private Button backButton;                // Go back button

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            loadingProgressBar = new ProgressBar();
            logRichTextBox = new RichTextBox();
            searchBox = new TextBox();
            searchButton = new Button();
            downloadAirportData = new Button();
            downloadRunwayData = new Button();
            airportFrequencyData = new Button();  // New button for Airport Frequency Data
            downloadVORDMEData = new Button();    // New button for VOR/DME Data
            downloadNDBData = new Button();    // New button for VOR/DME Data
            downloadGateData = new Button();    // New button for VOR/DME Data
            backButton = new Button();
            SuspendLayout();

            // 
            // loadingProgressBar
            // 
            loadingProgressBar.Dock = DockStyle.Top;
            loadingProgressBar.Location = new Point(0, 0);
            loadingProgressBar.Name = "loadingProgressBar";
            loadingProgressBar.Size = new Size(550, 30);
            loadingProgressBar.TabIndex = 6;

            // 
            // logRichTextBox
            // 
            logRichTextBox.Dock = DockStyle.Fill;
            logRichTextBox.Location = new Point(0, 97);
            logRichTextBox.Name = "logRichTextBox";
            logRichTextBox.ReadOnly = true;
            logRichTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            logRichTextBox.Size = new Size(550, 183);
            logRichTextBox.TabIndex = 0;
            logRichTextBox.Text = "";

            // 
            // searchBox
            // 
            searchBox.Dock = DockStyle.Top;
            searchBox.Location = new Point(0, 30);
            searchBox.Name = "searchBox";
            searchBox.Size = new Size(550, 27);
            searchBox.TabIndex = 5;

            // 
            // searchButton
            // 
            searchButton.Dock = DockStyle.Top;
            searchButton.Location = new Point(0, 57);
            searchButton.Name = "searchButton";
            searchButton.Size = new Size(550, 40);
            searchButton.TabIndex = 4;
            searchButton.Text = "Search";
            searchButton.UseVisualStyleBackColor = true;

            // 
            // downloadAirportData
            // 
            downloadAirportData.Dock = DockStyle.Bottom;
            downloadAirportData.Location = new Point(0, 360);
            downloadAirportData.Name = "downloadAirportData";
            downloadAirportData.Size = new Size(550, 40);
            downloadAirportData.TabIndex = 2;
            downloadAirportData.Text = "Download Airport Data File";
            downloadAirportData.UseVisualStyleBackColor = true;

            // 
            // downloadRunwayData
            // 
            downloadRunwayData.Dock = DockStyle.Bottom;
            downloadRunwayData.Location = new Point(0, 320);
            downloadRunwayData.Name = "downloadRunwayData";
            downloadRunwayData.Size = new Size(550, 40);
            downloadRunwayData.TabIndex = 1;
            downloadRunwayData.Text = "Download Runway Data File";
            downloadRunwayData.UseVisualStyleBackColor = true;

            // 
            // airportFrequencyData
            // 
            airportFrequencyData.Dock = DockStyle.Bottom;
            airportFrequencyData.Location = new Point(0, 280);
            airportFrequencyData.Name = "airportFrequencyData";
            airportFrequencyData.Size = new Size(550, 40);
            airportFrequencyData.TabIndex = 7;
            airportFrequencyData.Text = "Download Airport Frequency Data";
            airportFrequencyData.UseVisualStyleBackColor = true;

            // 
            // downloadVORDMEData
            // 
            downloadVORDMEData.Dock = DockStyle.Bottom;
            downloadVORDMEData.Location = new Point(0, 240); // Positioned above the back button
            downloadVORDMEData.Name = "downloadVORDMEData";
            downloadVORDMEData.Size = new Size(550, 40);
            downloadVORDMEData.TabIndex = 8;
            downloadVORDMEData.Text = "Download VOR/DME Data File";
            downloadVORDMEData.UseVisualStyleBackColor = true;

            // 
            // downloadVORDMEData
            // 
            downloadNDBData.Dock = DockStyle.Bottom;
            downloadNDBData.Location = new Point(0, 240); // Positioned above the back button
            downloadNDBData.Name = "downloadNDBData";
            downloadNDBData.Size = new Size(550, 40);
            downloadNDBData.TabIndex = 8;
            downloadNDBData.Text = "Download NDB Data File";
            downloadNDBData.UseVisualStyleBackColor = true;

            // 
            // downloadVORDMEData
            // 
            downloadGateData.Dock = DockStyle.Bottom;
            downloadGateData.Location = new Point(0, 240); // Positioned above the back button
            downloadGateData.Name = "downloadGateData";
            downloadGateData.Size = new Size(550, 40);
            downloadGateData.TabIndex = 8;
            downloadGateData.Text = "Download Gate Data File";
            downloadGateData.UseVisualStyleBackColor = true;

            // 
            // backButton
            // 
            backButton.Dock = DockStyle.Bottom;
            backButton.Location = new Point(0, 400);
            backButton.Name = "backButton";
            backButton.Size = new Size(550, 40);
            backButton.TabIndex = 3;
            backButton.Text = "Back to Main Menu";
            backButton.UseVisualStyleBackColor = true;

            // 
            // airportDataForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(550, 600);  // Increased height
            Controls.Add(logRichTextBox);
            Controls.Add(downloadAirportData);
            Controls.Add(downloadRunwayData);
            Controls.Add(airportFrequencyData);
            Controls.Add(downloadVORDMEData); // Add the VORDME Data download button
            Controls.Add(downloadNDBData); // Add the VORDME Data download button
            Controls.Add(downloadGateData); // Add the VORDME Data download button
            Controls.Add(backButton);
            Controls.Add(searchButton);
            Controls.Add(searchBox);
            Controls.Add(loadingProgressBar);
            MaximizeBox = false;
            Name = "airportDataForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Airport Data | IVAO ATC Utilities";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
