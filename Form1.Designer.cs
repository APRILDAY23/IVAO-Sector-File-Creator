namespace Sector_File
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        // Header
        private System.Windows.Forms.Panel  headerPanel;
        private System.Windows.Forms.Label  headerTitle;
        private System.Windows.Forms.Label  headerSub;

        // Tool cards grid
        private System.Windows.Forms.Panel  gridPanel;
        private System.Windows.Forms.Panel  sidStarCard;
        private System.Windows.Forms.Panel  airportCard;
        private System.Windows.Forms.Panel  firCard;
        private System.Windows.Forms.Panel  countryCard;
        private System.Windows.Forms.Panel  kmlCard;
        private System.Windows.Forms.Button backButton;

        private System.Windows.Forms.Button getSidAndStarDataButton;
        private System.Windows.Forms.Button getAirportDataButton;
        private System.Windows.Forms.Button getFirDataButton;
        private System.Windows.Forms.Button getCountryDataButton;
        private System.Windows.Forms.Button chooseKmlButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.headerPanel  = new System.Windows.Forms.Panel();
            this.headerTitle  = new System.Windows.Forms.Label();
            this.headerSub    = new System.Windows.Forms.Label();
            this.gridPanel    = new System.Windows.Forms.Panel();
            this.sidStarCard  = new System.Windows.Forms.Panel();
            this.airportCard  = new System.Windows.Forms.Panel();
            this.firCard      = new System.Windows.Forms.Panel();
            this.countryCard  = new System.Windows.Forms.Panel();
            this.kmlCard      = new System.Windows.Forms.Panel();
            this.backButton   = new System.Windows.Forms.Button();
            this.getSidAndStarDataButton = new System.Windows.Forms.Button();
            this.getAirportDataButton    = new System.Windows.Forms.Button();
            this.getFirDataButton        = new System.Windows.Forms.Button();
            this.getCountryDataButton    = new System.Windows.Forms.Button();
            this.chooseKmlButton         = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── Header ───────────────────────────────────────────────────────
            this.headerPanel.Dock      = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Height    = 70;
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(13, 71, 161);
            this.headerPanel.Padding   = new System.Windows.Forms.Padding(24, 0, 0, 0);

            this.headerTitle.Text      = "ATC Operations";
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

            // ── Tool grid ────────────────────────────────────────────────────
            this.gridPanel.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.gridPanel.BackColor = System.Drawing.Color.FromArgb(245, 248, 255);
            this.gridPanel.Resize   += (s, e) => LayoutToolCards();

            BuildToolCard(this.sidStarCard,
                icon: "✈", iconColor: System.Drawing.Color.FromArgb(25, 118, 210),
                title: "SID & STAR",
                desc: "Departure & arrival\nprocedures from free API",
                x: 0, y: 0, btn: this.getSidAndStarDataButton,
                handler: this.getSidAndStarDataButton_Click);

            BuildToolCard(this.airportCard,
                icon: "🏔", iconColor: System.Drawing.Color.FromArgb(46, 125, 50),
                title: "Airport Data",
                desc: "Runways · VOR/DME\nNDB · Gates · Freqs",
                x: 0, y: 0, btn: this.getAirportDataButton,
                handler: this.getAirportDataButton_Click);

            BuildToolCard(this.firCard,
                icon: "🗺", iconColor: System.Drawing.Color.FromArgb(200, 120, 0),
                title: "FIR Data",
                desc: "Flight Information\nRegion boundaries",
                x: 0, y: 0, btn: this.getFirDataButton,
                handler: this.getFirDataButton_Click);

            BuildToolCard(this.countryCard,
                icon: "🌍", iconColor: System.Drawing.Color.FromArgb(100, 60, 180),
                title: "Country Data",
                desc: "Country GeoJSON\nboundaries",
                x: 0, y: 0, btn: this.getCountryDataButton,
                handler: this.getCountryDataButton_Click);

            BuildToolCard(this.kmlCard,
                icon: "📐", iconColor: System.Drawing.Color.FromArgb(200, 50, 50),
                title: "Ground Layout",
                desc: "Import KML file\nfor ground layout",
                x: 0, y: 0, btn: this.chooseKmlButton,
                handler: this.chooseKmlButton_Click);

            this.gridPanel.Controls.Add(this.sidStarCard);
            this.gridPanel.Controls.Add(this.airportCard);
            this.gridPanel.Controls.Add(this.firCard);
            this.gridPanel.Controls.Add(this.countryCard);
            this.gridPanel.Controls.Add(this.kmlCard);

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
            this.backButton.Click             += (s, e) => this.Close();

            // ── Form ─────────────────────────────────────────────────────────
            this.Controls.Add(this.gridPanel);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.headerPanel);

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize    = new System.Drawing.Size(720, 460);
            this.MaximizeBox   = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text          = $"ATC Operations  |  {AppInfo.Name}";
            this.BackColor     = System.Drawing.Color.FromArgb(245, 248, 255);
            this.ResumeLayout(false);
        }

        private void BuildToolCard(
            System.Windows.Forms.Panel card,
            string icon, System.Drawing.Color iconColor,
            string title, string desc,
            int x, int y,
            System.Windows.Forms.Button btn,
            System.EventHandler handler)
        {
            card.BackColor = System.Drawing.Color.White;
            card.Location  = new System.Drawing.Point(x, y);
            card.Size      = new System.Drawing.Size(200, 160);
            card.Cursor    = System.Windows.Forms.Cursors.Hand;
            card.Click    += handler;
            card.Paint    += (s, e) =>
            {
                using var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(220, 228, 245), 1);
                e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };

            var iconLbl = new System.Windows.Forms.Label
            {
                Text      = icon,
                ForeColor = iconColor,
                Font      = new System.Drawing.Font("Segoe UI", 24f),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock      = System.Windows.Forms.DockStyle.Top,
                Height    = 60,
                Cursor    = System.Windows.Forms.Cursors.Hand,
            };
            iconLbl.Click += handler;

            var titleLbl = new System.Windows.Forms.Label
            {
                Text      = title,
                ForeColor = System.Drawing.Color.FromArgb(20, 30, 60),
                Font      = new System.Drawing.Font("Segoe UI", 10f, System.Drawing.FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock      = System.Windows.Forms.DockStyle.Top,
                Height    = 28,
                Cursor    = System.Windows.Forms.Cursors.Hand,
            };
            titleLbl.Click += handler;

            var descLbl = new System.Windows.Forms.Label
            {
                Text      = desc,
                ForeColor = System.Drawing.Color.FromArgb(100, 120, 150),
                Font      = new System.Drawing.Font("Segoe UI", 7.5f),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock      = System.Windows.Forms.DockStyle.Top,
                Height    = 36,
                Cursor    = System.Windows.Forms.Cursors.Hand,
            };
            descLbl.Click += handler;

            btn.Text               = "Open";
            btn.Font               = new System.Drawing.Font("Segoe UI", 8.5f, System.Drawing.FontStyle.Bold);
            btn.BackColor          = System.Drawing.Color.FromArgb(25, 118, 210);
            btn.ForeColor          = System.Drawing.Color.White;
            btn.FlatStyle          = System.Windows.Forms.FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.UseVisualStyleBackColor = false;
            btn.Dock               = System.Windows.Forms.DockStyle.Bottom;
            btn.Height             = 34;
            btn.Cursor             = System.Windows.Forms.Cursors.Hand;
            btn.Click             += handler;

            card.Controls.Add(descLbl);
            card.Controls.Add(titleLbl);
            card.Controls.Add(iconLbl);
            card.Controls.Add(btn);
        }
    }
}
