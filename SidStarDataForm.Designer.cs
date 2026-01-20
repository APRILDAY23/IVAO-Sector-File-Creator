namespace Sector_File
{
    partial class SidStarDataForm
    {
        private System.ComponentModel.IContainer components = null;
        private ProgressBar loadingProgressBar;  // Progress bar for loading status
        private RichTextBox logRichTextBox;       // Debug output box
        private TextBox searchBox;                // Search input box
        private Button searchButton;              // Search button
        private Button downloadSidButton;         // Download SID button
        private Button downloadStarButton;        // Download STAR button
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
            this.loadingProgressBar = new ProgressBar();
            this.logRichTextBox = new RichTextBox();
            this.searchBox = new TextBox();           // Initialize search box
            this.searchButton = new Button();         // Initialize search button
            this.downloadSidButton = new Button();    // Initialize Download SID button
            this.downloadStarButton = new Button();   // Initialize Download STAR button
            this.backButton = new Button();           // Initialize back button

            this.SuspendLayout();

            // 
            // loadingProgressBar
            // 
            this.loadingProgressBar.Dock = DockStyle.Top;
            this.loadingProgressBar.Name = "loadingProgressBar";
            this.loadingProgressBar.Size = new System.Drawing.Size(550, 30); // Full width, height 30
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
            this.searchButton.Size = new System.Drawing.Size(550, 40); // Full width, height 40
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Dock = DockStyle.Top;

            // 
            // logRichTextBox
            // 
            this.logRichTextBox.Dock = DockStyle.Fill;
            this.logRichTextBox.Name = "logRichTextBox";
            this.logRichTextBox.ReadOnly = true;
            this.logRichTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;

            // 
            // downloadSidButton
            // 
            this.downloadSidButton.Name = "downloadSidButton";
            this.downloadSidButton.Size = new System.Drawing.Size(550, 40); // Full width, height 40
            this.downloadSidButton.Text = "Download SID File";
            this.downloadSidButton.UseVisualStyleBackColor = true;
            this.downloadSidButton.Dock = DockStyle.Bottom;

            // 
            // downloadStarButton
            // 
            this.downloadStarButton.Name = "downloadStarButton";
            this.downloadStarButton.Size = new System.Drawing.Size(550, 40); // Full width, height 40
            this.downloadStarButton.Text = "Download STAR File";
            this.downloadStarButton.UseVisualStyleBackColor = true;
            this.downloadStarButton.Dock = DockStyle.Bottom;

            // 
            // backButton
            // 
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(550, 40); // Full width, height 40
            this.backButton.Text = "Back to Main Menu";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Dock = DockStyle.Bottom; // Positioned at the bottom
            this.backButton.Click += new System.EventHandler(this.backButton_Click); // Event handler for going back

            // 
            // SidStarDataForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 400);


            this.Controls.Add(this.logRichTextBox);   // Log box above the buttons
            this.Controls.Add(this.downloadStarButton);
            this.Controls.Add(this.downloadSidButton);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.loadingProgressBar);

            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sid & Star Data | IVAO ATC Utilities";
            this.ResumeLayout(false);
            this.MaximizeBox = false; // Disable maximize button
            this.PerformLayout();
        }

        #endregion
    }
}
