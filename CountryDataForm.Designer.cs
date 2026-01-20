namespace Sector_File
{
    partial class CountryDataForm
    {
        private System.ComponentModel.IContainer components = null;
        private ProgressBar loadingProgressBar;  // Progress bar for loading status
        private RichTextBox logRichTextBox;       // Debug output box
        private TextBox searchBox;                 // Search input box
        private Button searchButton;                // Search button
        private Button downloadButton;              // Download button
        private Button backButton;                  // Go back button

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
            this.loadingProgressBar = new ProgressBar();
            this.logRichTextBox = new RichTextBox();
            this.searchBox = new TextBox();           // Initialize search box
            this.searchButton = new Button();          // Initialize search button
            this.downloadButton = new Button();        // Initialize download button
            this.backButton = new Button();            // Initialize back button

            this.SuspendLayout();

            // 
            // loadingProgressBar
            // 
            this.loadingProgressBar.Dock = DockStyle.Top;
            this.loadingProgressBar.Name = "loadingProgressBar";
            this.loadingProgressBar.Size = new System.Drawing.Size(550, 30);
            this.loadingProgressBar.Style = ProgressBarStyle.Blocks;

            // 
            // searchBox
            // 
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(440, 40); // 80% width
            this.searchBox.Dock = DockStyle.Top;

            // 
            // searchButton
            // 
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(110, 40); // 20% width
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click); // Event handler for search
            this.searchButton.Dock = DockStyle.Top;

            // 
            // logRichTextBox
            // 
            this.logRichTextBox.Dock = DockStyle.Fill;       // Fill the remaining space
            this.logRichTextBox.Name = "logRichTextBox";
            this.logRichTextBox.ReadOnly = true;
            this.logRichTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;

            // 
            // downloadButton
            // 
            this.downloadButton.Name = "downloadButton";
            this.downloadButton.Size = new System.Drawing.Size(550, 40); // Full width
            this.downloadButton.Text = "Download";
            this.downloadButton.UseVisualStyleBackColor = true;
            this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click); // Event handler for download
            this.downloadButton.Dock = DockStyle.Bottom; // Positioned at the bottom

            // 
            // backButton
            // 
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(550, 40); // Full width
            this.backButton.Text = "Back to Main Menu";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.backButton_Click); // Event handler for going back
            this.backButton.Dock = DockStyle.Bottom; // Positioned at the bottom

            // 
            // CountryDataForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 400);

            // Add controls in the desired order
            this.Controls.Add(this.logRichTextBox);   // Log box above the buttons
            this.Controls.Add(this.downloadButton);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.loadingProgressBar);

            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Country Data | IVAO ATC Utilities";
            this.ResumeLayout(false);
            this.MaximizeBox = false; // Disable maximize button
            this.PerformLayout();
        }

        #endregion
    }
}
