namespace Sector_File
{
    partial class SidStarDataForm
    {
        private System.ComponentModel.IContainer components = null;

        // ── Header ──────────────────────────────────────────────────────────
        private Panel      headerPanel;
        private Label      titleLabel;
        private Label      subtitleLabel;
        private Label      airacCycleLabel;   // "AIRAC 2604"
        private Label      airacDaysLabel;    // "23 days remaining"

        // ── Search row ──────────────────────────────────────────────────────
        private Panel      searchPanel;
        private TextBox    searchBox;
        private Button     searchButton;

        // ── Progress / status strip ─────────────────────────────────────────
        private Panel      progressPanel;
        private ProgressBar loadingProgressBar;
        private Label      statusLabel;

        // ── Log (output) ────────────────────────────────────────────────────
        private RichTextBox logRichTextBox;

        // ── Download row ────────────────────────────────────────────────────
        private Panel      downloadPanel;
        private Button     downloadSidButton;
        private Button     downloadStarButton;

        // ── Back ─────────────────────────────────────────────────────────────
        private Button     backButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.headerPanel       = new System.Windows.Forms.Panel();
            this.titleLabel        = new System.Windows.Forms.Label();
            this.subtitleLabel     = new System.Windows.Forms.Label();
            this.airacCycleLabel   = new System.Windows.Forms.Label();
            this.airacDaysLabel    = new System.Windows.Forms.Label();

            this.searchPanel       = new System.Windows.Forms.Panel();
            this.searchBox         = new System.Windows.Forms.TextBox();
            this.searchButton      = new System.Windows.Forms.Button();

            this.progressPanel     = new System.Windows.Forms.Panel();
            this.loadingProgressBar = new System.Windows.Forms.ProgressBar();
            this.statusLabel       = new System.Windows.Forms.Label();

            this.logRichTextBox    = new System.Windows.Forms.RichTextBox();

            this.downloadPanel     = new System.Windows.Forms.Panel();
            this.downloadSidButton = new System.Windows.Forms.Button();
            this.downloadStarButton = new System.Windows.Forms.Button();

            this.backButton        = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ────────────────────────────────────────────────────────────────
            // HEADER PANEL  (dark IVAO blue, 80 px tall)
            // ────────────────────────────────────────────────────────────────
            this.headerPanel.Dock      = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Height    = 80;
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(13, 71, 161);

            this.titleLabel.Text      = "SID & STAR Generator";
            this.titleLabel.ForeColor = System.Drawing.Color.White;
            this.titleLabel.Font      = new System.Drawing.Font("Segoe UI", 16f, System.Drawing.FontStyle.Bold);
            this.titleLabel.Location  = new System.Drawing.Point(16, 12);
            this.titleLabel.AutoSize  = true;

            this.subtitleLabel.Text      = "IVAO ATC Utilities  ·  Data from free API";
            this.subtitleLabel.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.subtitleLabel.Font      = new System.Drawing.Font("Segoe UI", 8.5f);
            this.subtitleLabel.Location  = new System.Drawing.Point(18, 47);
            this.subtitleLabel.AutoSize  = true;

            // AIRAC badge (right side of header)
            this.airacCycleLabel.Text      = "AIRAC  ─ ─ ─ ─";
            this.airacCycleLabel.ForeColor = System.Drawing.Color.FromArgb(255, 214, 0);
            this.airacCycleLabel.Font      = new System.Drawing.Font("Segoe UI", 13f, System.Drawing.FontStyle.Bold);
            this.airacCycleLabel.Location  = new System.Drawing.Point(478, 12);
            this.airacCycleLabel.Size      = new System.Drawing.Size(206, 28);
            this.airacCycleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.airacDaysLabel.Text      = "Fetching cycle info...";
            this.airacDaysLabel.ForeColor = System.Drawing.Color.LightBlue;
            this.airacDaysLabel.Font      = new System.Drawing.Font("Segoe UI", 8.5f);
            this.airacDaysLabel.Location  = new System.Drawing.Point(478, 46);
            this.airacDaysLabel.Size      = new System.Drawing.Size(206, 18);
            this.airacDaysLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.headerPanel.Controls.Add(this.titleLabel);
            this.headerPanel.Controls.Add(this.subtitleLabel);
            this.headerPanel.Controls.Add(this.airacCycleLabel);
            this.headerPanel.Controls.Add(this.airacDaysLabel);

            // ────────────────────────────────────────────────────────────────
            // SEARCH PANEL  (light gray, 54 px tall)
            // ────────────────────────────────────────────────────────────────
            this.searchPanel.Dock      = System.Windows.Forms.DockStyle.Top;
            this.searchPanel.Height    = 54;
            this.searchPanel.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);

            this.searchBox.Name            = "searchBox";
            this.searchBox.Location        = new System.Drawing.Point(12, 12);
            this.searchBox.Size            = new System.Drawing.Size(494, 30);
            this.searchBox.Font            = new System.Drawing.Font("Segoe UI", 10f);
            this.searchBox.PlaceholderText = "Enter ICAO airport code  (e.g. VTBD)";
            this.searchBox.BorderStyle     = System.Windows.Forms.BorderStyle.FixedSingle;

            this.searchButton.Name               = "searchButton";
            this.searchButton.Location           = new System.Drawing.Point(518, 12);
            this.searchButton.Size               = new System.Drawing.Size(166, 30);
            this.searchButton.Text               = "Search";
            this.searchButton.Font               = new System.Drawing.Font("Segoe UI", 10f, System.Drawing.FontStyle.Bold);
            this.searchButton.BackColor          = System.Drawing.Color.FromArgb(25, 118, 210);
            this.searchButton.ForeColor          = System.Drawing.Color.White;
            this.searchButton.FlatStyle          = System.Windows.Forms.FlatStyle.Flat;
            this.searchButton.FlatAppearance.BorderSize = 0;
            this.searchButton.UseVisualStyleBackColor = false;

            this.searchPanel.Controls.Add(this.searchBox);
            this.searchPanel.Controls.Add(this.searchButton);

            // ────────────────────────────────────────────────────────────────
            // PROGRESS PANEL  (white, 26 px tall)
            // ────────────────────────────────────────────────────────────────
            this.progressPanel.Dock      = System.Windows.Forms.DockStyle.Top;
            this.progressPanel.Height    = 26;
            this.progressPanel.BackColor = System.Drawing.Color.White;

            this.loadingProgressBar.Name     = "loadingProgressBar";
            this.loadingProgressBar.Location = new System.Drawing.Point(0, 0);
            this.loadingProgressBar.Size     = new System.Drawing.Size(700, 8);
            this.loadingProgressBar.Style    = System.Windows.Forms.ProgressBarStyle.Continuous;

            this.statusLabel.Name      = "statusLabel";
            this.statusLabel.Location  = new System.Drawing.Point(8, 10);
            this.statusLabel.Size      = new System.Drawing.Size(684, 14);
            this.statusLabel.Font      = new System.Drawing.Font("Segoe UI", 7.5f);
            this.statusLabel.ForeColor = System.Drawing.Color.DimGray;
            this.statusLabel.Text      = "Ready";

            this.progressPanel.Controls.Add(this.loadingProgressBar);
            this.progressPanel.Controls.Add(this.statusLabel);

            // ────────────────────────────────────────────────────────────────
            // LOG / OUTPUT  (dark terminal, fills remaining space)
            // ────────────────────────────────────────────────────────────────
            this.logRichTextBox.Name        = "logRichTextBox";
            this.logRichTextBox.Dock        = System.Windows.Forms.DockStyle.Fill;
            this.logRichTextBox.ReadOnly    = true;
            this.logRichTextBox.ScrollBars  = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.logRichTextBox.BackColor   = System.Drawing.Color.FromArgb(18, 18, 18);
            this.logRichTextBox.ForeColor   = System.Drawing.Color.FromArgb(220, 220, 220);
            this.logRichTextBox.Font        = new System.Drawing.Font("Consolas", 9f);
            this.logRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.logRichTextBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.debugTextBox_LinkClicked);

            // ────────────────────────────────────────────────────────────────
            // DOWNLOAD PANEL  (two side-by-side buttons, 52 px)
            // ────────────────────────────────────────────────────────────────
            this.downloadPanel.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.downloadPanel.Height    = 52;
            this.downloadPanel.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);

            this.downloadSidButton.Name               = "downloadSidButton";
            this.downloadSidButton.Location           = new System.Drawing.Point(10, 10);
            this.downloadSidButton.Size               = new System.Drawing.Size(335, 32);
            this.downloadSidButton.Text               = "Download SID File  (.SID)";
            this.downloadSidButton.Font               = new System.Drawing.Font("Segoe UI", 9.5f);
            this.downloadSidButton.BackColor          = System.Drawing.Color.FromArgb(46, 125, 50);
            this.downloadSidButton.ForeColor          = System.Drawing.Color.White;
            this.downloadSidButton.FlatStyle          = System.Windows.Forms.FlatStyle.Flat;
            this.downloadSidButton.FlatAppearance.BorderSize = 0;
            this.downloadSidButton.UseVisualStyleBackColor = false;

            this.downloadStarButton.Name               = "downloadStarButton";
            this.downloadStarButton.Location           = new System.Drawing.Point(355, 10);
            this.downloadStarButton.Size               = new System.Drawing.Size(335, 32);
            this.downloadStarButton.Text               = "Download STAR File  (.STR)";
            this.downloadStarButton.Font               = new System.Drawing.Font("Segoe UI", 9.5f);
            this.downloadStarButton.BackColor          = System.Drawing.Color.FromArgb(21, 101, 192);
            this.downloadStarButton.ForeColor          = System.Drawing.Color.White;
            this.downloadStarButton.FlatStyle          = System.Windows.Forms.FlatStyle.Flat;
            this.downloadStarButton.FlatAppearance.BorderSize = 0;
            this.downloadStarButton.UseVisualStyleBackColor = false;

            this.downloadPanel.Controls.Add(this.downloadSidButton);
            this.downloadPanel.Controls.Add(this.downloadStarButton);

            // ────────────────────────────────────────────────────────────────
            // BACK BUTTON  (dark strip at very bottom, 34 px)
            // ────────────────────────────────────────────────────────────────
            this.backButton.Name               = "backButton";
            this.backButton.Dock               = System.Windows.Forms.DockStyle.Bottom;
            this.backButton.Height             = 34;
            this.backButton.Text               = "← Back to Main Menu";
            this.backButton.Font               = new System.Drawing.Font("Segoe UI", 9f);
            this.backButton.BackColor          = System.Drawing.Color.FromArgb(50, 50, 50);
            this.backButton.ForeColor          = System.Drawing.Color.FromArgb(200, 200, 200);
            this.backButton.FlatStyle          = System.Windows.Forms.FlatStyle.Flat;
            this.backButton.FlatAppearance.BorderSize = 0;
            this.backButton.UseVisualStyleBackColor = false;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);

            // ────────────────────────────────────────────────────────────────
            // FORM  - controls added in dock-stacking order:
            //   Top controls: last added = topmost
            //   Bottom controls: last added = bottommost
            // ────────────────────────────────────────────────────────────────
            this.Controls.Add(this.logRichTextBox);     // Fill - always first
            this.Controls.Add(this.downloadPanel);      // Bottom - inner (just above fill)
            this.Controls.Add(this.backButton);         // Bottom - outer (very bottom edge)
            this.Controls.Add(this.progressPanel);      // Top - inner (just below search)
            this.Controls.Add(this.searchPanel);        // Top - middle
            this.Controls.Add(this.headerPanel);        // Top - outer (very top edge)

            this.AutoScaleMode  = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize     = new System.Drawing.Size(700, 580);
            this.StartPosition  = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text           = "SID & STAR Generator  |  IVAO ATC Utilities";
            this.MaximizeBox    = false;
            this.Load          += new System.EventHandler(this.SidStarDataForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
