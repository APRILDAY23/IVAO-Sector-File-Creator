namespace Sector_File
{
    partial class flightOperationOptions
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel  headerPanel;
        private System.Windows.Forms.Label  headerTitle;
        private System.Windows.Forms.Label  headerSub;
        private System.Windows.Forms.Panel  gridPanel;
        private System.Windows.Forms.Panel  schedCard;
        private System.Windows.Forms.Button getSchedulesData;
        private System.Windows.Forms.Button backButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.headerPanel   = new System.Windows.Forms.Panel();
            this.headerTitle   = new System.Windows.Forms.Label();
            this.headerSub     = new System.Windows.Forms.Label();
            this.gridPanel     = new System.Windows.Forms.Panel();
            this.schedCard     = new System.Windows.Forms.Panel();
            this.getSchedulesData = new System.Windows.Forms.Button();
            this.backButton    = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── Header ───────────────────────────────────────────────────────
            this.headerPanel.Dock      = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Height    = 70;
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(13, 71, 161);
            this.headerPanel.Padding   = new System.Windows.Forms.Padding(24, 0, 0, 0);

            this.headerTitle.Text      = "Flight Operations";
            this.headerTitle.ForeColor = System.Drawing.Color.White;
            this.headerTitle.Font      = new System.Drawing.Font("Segoe UI", 15f, System.Drawing.FontStyle.Bold);
            this.headerTitle.Dock      = System.Windows.Forms.DockStyle.Top;
            this.headerTitle.Height    = 46;
            this.headerTitle.Padding   = new System.Windows.Forms.Padding(0, 12, 0, 0);

            this.headerSub.Text      = "Select a tool to begin";
            this.headerSub.ForeColor = System.Drawing.Color.FromArgb(180, 210, 255);
            this.headerSub.Font      = new System.Drawing.Font("Segoe UI", 8.5f);
            this.headerSub.Dock      = System.Windows.Forms.DockStyle.Top;
            this.headerSub.Height    = 22;

            this.headerPanel.Controls.Add(this.headerSub);
            this.headerPanel.Controls.Add(this.headerTitle);

            // ── Grid ─────────────────────────────────────────────────────────
            this.gridPanel.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.gridPanel.BackColor = System.Drawing.Color.FromArgb(245, 248, 255);

            // Card (centred)
            this.schedCard.BackColor = System.Drawing.Color.White;
            this.schedCard.Location  = new System.Drawing.Point(140, 40);
            this.schedCard.Size      = new System.Drawing.Size(210, 170);
            this.schedCard.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.schedCard.Click    += new System.EventHandler(this.getSchedulesButton_Click);
            this.schedCard.Paint    += (s, e) =>
            {
                using var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(220, 228, 245), 1);
                e.Graphics.DrawRectangle(pen, 0, 0, schedCard.Width - 1, schedCard.Height - 1);
            };

            var icon = new System.Windows.Forms.Label
            {
                Text      = "📋",
                ForeColor = System.Drawing.Color.FromArgb(255, 180, 0),
                Font      = new System.Drawing.Font("Segoe UI", 26f),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock      = System.Windows.Forms.DockStyle.Top,
                Height    = 62,
                Cursor    = System.Windows.Forms.Cursors.Hand,
            };
            icon.Click += new System.EventHandler(this.getSchedulesButton_Click);

            var title = new System.Windows.Forms.Label
            {
                Text      = "Flight Schedules",
                ForeColor = System.Drawing.Color.FromArgb(20, 30, 60),
                Font      = new System.Drawing.Font("Segoe UI", 10f, System.Drawing.FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock      = System.Windows.Forms.DockStyle.Top,
                Height    = 28,
                Cursor    = System.Windows.Forms.Cursors.Hand,
            };
            title.Click += new System.EventHandler(this.getSchedulesButton_Click);

            var desc = new System.Windows.Forms.Label
            {
                Text      = "Download & export\nflight schedule data",
                ForeColor = System.Drawing.Color.FromArgb(100, 120, 150),
                Font      = new System.Drawing.Font("Segoe UI", 7.5f),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock      = System.Windows.Forms.DockStyle.Top,
                Height    = 36,
                Cursor    = System.Windows.Forms.Cursors.Hand,
            };
            desc.Click += new System.EventHandler(this.getSchedulesButton_Click);

            this.getSchedulesData.Text               = "Open";
            this.getSchedulesData.Font               = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.getSchedulesData.BackColor          = System.Drawing.Color.FromArgb(25, 118, 210);
            this.getSchedulesData.ForeColor          = System.Drawing.Color.White;
            this.getSchedulesData.FlatStyle          = System.Windows.Forms.FlatStyle.Flat;
            this.getSchedulesData.FlatAppearance.BorderSize = 0;
            this.getSchedulesData.UseVisualStyleBackColor = false;
            this.getSchedulesData.Dock               = System.Windows.Forms.DockStyle.Bottom;
            this.getSchedulesData.Height             = 36;
            this.getSchedulesData.Cursor             = System.Windows.Forms.Cursors.Hand;
            this.getSchedulesData.Click             += new System.EventHandler(this.getSchedulesButton_Click);

            this.schedCard.Controls.Add(desc);
            this.schedCard.Controls.Add(title);
            this.schedCard.Controls.Add(icon);
            this.schedCard.Controls.Add(this.getSchedulesData);
            this.gridPanel.Controls.Add(this.schedCard);

            // Centre card on resize
            this.gridPanel.Resize += (s, e) =>
            {
                int cx = (gridPanel.ClientSize.Width  - schedCard.Width)  / 2;
                int cy = (gridPanel.ClientSize.Height - schedCard.Height) / 2;
                schedCard.Location = new System.Drawing.Point(Math.Max(0, cx), Math.Max(0, cy));
            };

            // ── Back button ──────────────────────────────────────────────────
            this.backButton.Dock               = System.Windows.Forms.DockStyle.Bottom;
            this.backButton.Height             = 36;
            this.backButton.Text               = "← Back";
            this.backButton.Font               = new System.Drawing.Font("Segoe UI", 9f);
            this.backButton.BackColor          = System.Drawing.Color.FromArgb(245, 248, 255);
            this.backButton.ForeColor          = System.Drawing.Color.FromArgb(100, 130, 170);
            this.backButton.FlatStyle          = System.Windows.Forms.FlatStyle.Flat;
            this.backButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(220, 228, 245);
            this.backButton.FlatAppearance.BorderSize  = 1;
            this.backButton.UseVisualStyleBackColor = false;
            this.backButton.Click += (s, e) => this.Close();

            // ── Form ─────────────────────────────────────────────────────────
            this.Controls.Add(this.gridPanel);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.headerPanel);

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize    = new System.Drawing.Size(520, 340);
            this.MaximizeBox   = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text          = $"Flight Operations  |  {AppInfo.Name}";
            this.BackColor     = System.Drawing.Color.FromArgb(245, 248, 255);
            this.ResumeLayout(false);
        }
    }
}
