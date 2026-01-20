namespace Sector_File
{
    partial class flightSechedulesData
    {
        private System.ComponentModel.IContainer components = null;
        private ProgressBar loadingProgressBar;  // Progress bar for loading status
        private RichTextBox logRichTextBox;       // Debug output box
        private TextBox searchBox;                // Search input box
        private Button searchButton;              // Search button
        private Button downloadFlightScheduleButton;        // Download STAR button
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
            downloadFlightScheduleButton = new Button();
            backButton = new Button();

            // Add alert panel and labels for title and description
            Panel alertPanel = new Panel();
            Label alertTitleLabel = new Label();
            Label alertDescriptionLabel = new Label();
            SuspendLayout();

            // 
            // alertPanel
            // 
            alertPanel.Dock = DockStyle.Top;
            alertPanel.BackColor = ColorTranslator.FromHtml("#fff3cd"); // Light yellow background
            alertPanel.Padding = new Padding(10);
            alertPanel.Controls.Add(alertTitleLabel);
            alertPanel.Controls.Add(alertDescriptionLabel);
            alertPanel.Height = 80;

            // 
            // alertTitleLabel
            // 
            alertTitleLabel.Dock = DockStyle.Top;
            alertTitleLabel.Text = "Note:"; // Title text (in uppercase)
            alertTitleLabel.Font = new Font("Arial", 10F, FontStyle.Bold); // Bold Arial font
            alertTitleLabel.TextAlign = ContentAlignment.MiddleLeft; // Left-align text
            alertTitleLabel.ForeColor = Color.Black;

            // 
            // alertDescriptionLabel
            // 
            alertDescriptionLabel.Dock = DockStyle.Fill;
            alertDescriptionLabel.Text = "Please use this feature when necessary, as the API has a limited number of requests per month, and we want to ensure it remains available for all divisions.";
            alertDescriptionLabel.Font = new Font("Arial", 8F); // Simple Arial font
            alertDescriptionLabel.TextAlign = ContentAlignment.MiddleLeft; // Left-align text
            alertDescriptionLabel.ForeColor = Color.Black;
            alertDescriptionLabel.Padding = new Padding(0, 20, 0, 0); // Add padding to the top of the description

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
            // downloadFlightScheduleButton
            // 
            downloadFlightScheduleButton.Dock = DockStyle.Bottom;
            downloadFlightScheduleButton.Location = new Point(0, 280);
            downloadFlightScheduleButton.Name = "downloadFlightScheduleButton";
            downloadFlightScheduleButton.Size = new Size(550, 40);
            downloadFlightScheduleButton.TabIndex = 1;
            downloadFlightScheduleButton.Text = "Download Flight Schedules Data File";
            downloadFlightScheduleButton.UseVisualStyleBackColor = true;
            // 
            // backButton
            // 
            backButton.Dock = DockStyle.Bottom;
            backButton.Location = new Point(0, 360);
            backButton.Name = "backButton";
            backButton.Size = new Size(550, 40);
            backButton.TabIndex = 3;
            backButton.Text = "Back to Main Menu";
            backButton.UseVisualStyleBackColor = true;
            backButton.Click += backButton_Click;
            // 
            // firDataForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(550, 500);
            Controls.Add(logRichTextBox);
            Controls.Add(downloadFlightScheduleButton);
            Controls.Add(backButton);
            Controls.Add(searchButton);
            Controls.Add(searchBox);
            Controls.Add(loadingProgressBar);
            Controls.Add(alertPanel);
            MaximizeBox = false;
            Name = "firDataForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Flight Schedules Data | IVAO Flight Utilities";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
