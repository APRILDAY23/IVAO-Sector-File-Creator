namespace Sector_File
{
    partial class LoadingForm
    {
        private System.ComponentModel.IContainer components = null;
        private ProgressBar progressBar;
        private Label outputLabel;
        private RichTextBox debugTextBox;
        private Button downloadButton;
        private Button downloadRawXmlButton;
        private Button downloadGeoButton; // New button for downloading GEO file
        private Button downloadtxiButton; // New button for downloading Taxiway Labels file
        private Button goBackButton; // New button for going back to the main menu
        private Button downloadRunwayButton; // New button for downloading runway configurations
        private Button previewButton; // New button for previewing output
        private FlowLayoutPanel buttonPanel; // FlowLayoutPanel for buttons

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
            this.progressBar = new ProgressBar();
            this.outputLabel = new Label();
            this.debugTextBox = new RichTextBox();
            this.downloadButton = new Button();
            this.downloadRawXmlButton = new Button();
            this.downloadGeoButton = new Button(); // Initialize new GEO button
            this.downloadtxiButton = new Button(); // Initialize new Taxiway Labels button
            this.goBackButton = new Button(); // Initialize new Go Back button
            this.downloadRunwayButton = new Button(); // New button for downloading runway configurations
            this.previewButton = new Button(); // New button for previewing output
            this.buttonPanel = new FlowLayoutPanel(); // Initialize FlowLayoutPanel

            this.SuspendLayout();

            // 
            // progressBar
            // 
            this.progressBar.Dock = DockStyle.Top; // Dock to the top
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(550, 30); // Set width to match form width
            this.progressBar.Style = ProgressBarStyle.Blocks; // Change to Blocks style
            this.progressBar.Minimum = 0; // Set minimum value
            this.progressBar.Maximum = 100; // Set maximum value

            // 
            // outputLabel
            // 
            this.outputLabel.Dock = DockStyle.Top; // Dock below the progress bar
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(550, 50);
            this.outputLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // debugTextBox
            // 
            this.debugTextBox.Dock = DockStyle.Fill; // Fill the remaining space
            this.debugTextBox.Name = "debugTextBox";
            this.debugTextBox.Multiline = true;
            this.debugTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            this.debugTextBox.ReadOnly = true;
            this.debugTextBox.LinkClicked += new LinkClickedEventHandler(debugTextBox_LinkClicked);
            this.debugTextBox.DetectUrls = true; // Enable link detection

            // 
            // buttonPanel
            // 
            this.buttonPanel.Dock = DockStyle.Bottom; // Dock at the bottom
            this.buttonPanel.AutoSize = true; // Allow automatic size adjustment
            this.buttonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink; // Control grows and shrinks with content

            // 
            // downloadButton
            // 
            this.downloadButton.Name = "downloadButton";
            this.downloadButton.Size = new System.Drawing.Size(250, 40); // Set size for both buttons
            this.downloadButton.Text = "Download TFL File";
            this.downloadButton.UseVisualStyleBackColor = true;
            this.downloadButton.Enabled = false; // Disabled until the file is loaded
            this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click);

            // 
            // downloadRawXmlButton
            // 
            this.downloadRawXmlButton.Name = "downloadRawXmlButton";
            this.downloadRawXmlButton.Size = new System.Drawing.Size(250, 40); // Same size as the other button
            this.downloadRawXmlButton.Text = "Download Raw Debug File";
            this.downloadRawXmlButton.UseVisualStyleBackColor = true;
            this.downloadRawXmlButton.Click += new System.EventHandler(this.downloadRawXmlButton_Click);

            // 
            // downloadGeoButton
            // 
            this.downloadGeoButton.Name = "downloadGeoButton";
            this.downloadGeoButton.Size = new System.Drawing.Size(250, 40); // Same size as other buttons
            this.downloadGeoButton.Text = "Download GEO File";
            this.downloadGeoButton.UseVisualStyleBackColor = true;
            this.downloadGeoButton.Click += new System.EventHandler(this.downloadGeoButton_Click);

            // 
            // downloadRunwayButton
            // 
            this.downloadRunwayButton.Name = "downloadRunwayButton";
            this.downloadRunwayButton.Size = new System.Drawing.Size(250, 40); // Set size similar to other buttons
            this.downloadRunwayButton.Text = "Download Runway Configurations"; // Button label
            this.downloadRunwayButton.UseVisualStyleBackColor = true;
            this.downloadRunwayButton.Click += new System.EventHandler(this.downloadRunwayButton_Click); // Click event for download

            // 
            // downloadtxiButton
            // 
            this.downloadtxiButton.Name = "downloadtxiButton"; // Ensure correct name
            this.downloadtxiButton.Size = new System.Drawing.Size(250, 40); // Same size as other buttons
            this.downloadtxiButton.Text = "Download Taxiway Labels File"; // Corrected label
            this.downloadtxiButton.UseVisualStyleBackColor = true;
            this.downloadtxiButton.Click += new System.EventHandler(this.downloadTXIButton_Click); // Click event to handle downloading taxiway labels

            // 
            // goBackButton
            // 
            this.goBackButton.Dock = DockStyle.Bottom; // Dock the button to the bottom of the panel
            this.goBackButton.Name = "goBackButton";
            this.goBackButton.Size = new System.Drawing.Size(780, 40); // Full width, same height as other buttons
            this.goBackButton.Text = "Go Back to Main Menu";
            this.goBackButton.UseVisualStyleBackColor = true;
            this.goBackButton.Click += new System.EventHandler(this.goBackButton_Click); // Click event to handle going back

            // 
            // previewButton
            // 
            this.previewButton.Name = "previewButton";
            this.previewButton.Size = new System.Drawing.Size(250, 40); // Same size as other buttons
            this.previewButton.Text = "Preview Output (WIP)";
            this.previewButton.UseVisualStyleBackColor = true;
            this.previewButton.Click += new System.EventHandler(this.previewButton_Click);

            // Add buttons to the FlowLayoutPanel
            this.buttonPanel.Controls.Add(this.downloadButton);
            this.buttonPanel.Controls.Add(this.downloadRawXmlButton);
            this.buttonPanel.Controls.Add(this.downloadGeoButton); // Add new GEO button to panel
            this.buttonPanel.Controls.Add(this.downloadtxiButton); // Add new Taxiway Labels button to panel
            this.buttonPanel.Controls.Add(this.downloadRunwayButton); // Add new Runway Config button to the panel
            this.buttonPanel.Controls.Add(this.previewButton); // Add Preview button to panel
            this.buttonPanel.Controls.Add(this.goBackButton); // Add Go Back button to panel

            // 
            // LoadingForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 500); // Set initial size of the form
            this.Controls.Add(this.debugTextBox);
            this.Controls.Add(this.outputLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonPanel); // Add the button panel to the form
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Loading KML File | IVAO ATC Utilities";
            this.ResumeLayout(false);
            this.MaximizeBox = false; // Disable maximize button
            this.PerformLayout();
        }
        #endregion
    }
}
