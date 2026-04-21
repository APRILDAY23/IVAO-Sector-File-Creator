using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Sector_File
{
    partial class Login
    {
        private System.ComponentModel.IContainer components = null;

        // ── Left decorative panel ─────────────────────────────────────────────
        private System.Windows.Forms.Panel  leftPanel;
        private System.Windows.Forms.Label  heroLabel;
        private System.Windows.Forms.Label  heroSubLabel;
        private System.Windows.Forms.Timer  radarTimer;
        private float _radarAngle = 0f;

        // ── Right login panel ─────────────────────────────────────────────────
        private System.Windows.Forms.Panel  rightPanel;
        private System.Windows.Forms.Label  logoLabel;         // IVAO logo image
        private System.Windows.Forms.Label  titleLabel;
        private System.Windows.Forms.Label  subtitleLabel;     // sign-in subtitle
        private System.Windows.Forms.Label  airacBadgeLabel;   // AIRAC cycle info
        private System.Windows.Forms.Button LoginButton;       // SSO button
        private System.Windows.Forms.Panel  notePanel;         // amber staff notice
        private System.Windows.Forms.Label  noteLabel;         // notice text
        private System.Windows.Forms.Label  versionLabel;      // version / warning

        // Legacy fields (kept so Login.cs compiles without changes)
        private System.Windows.Forms.Panel  headerPanel;
        private System.Windows.Forms.Panel  bodyPanel;
        private System.Windows.Forms.Label  welcomeLabel;
        private System.Windows.Forms.Label  descLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            radarTimer?.Stop();
            radarTimer?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.leftPanel       = new System.Windows.Forms.Panel();
            this.heroLabel       = new System.Windows.Forms.Label();
            this.heroSubLabel    = new System.Windows.Forms.Label();
            this.radarTimer      = new System.Windows.Forms.Timer();
            this.rightPanel      = new System.Windows.Forms.Panel();
            this.logoLabel       = new System.Windows.Forms.Label();
            this.titleLabel      = new System.Windows.Forms.Label();
            this.subtitleLabel   = new System.Windows.Forms.Label();
            this.airacBadgeLabel = new System.Windows.Forms.Label();
            this.LoginButton     = new System.Windows.Forms.Button();
            this.notePanel       = new System.Windows.Forms.Panel();
            this.noteLabel       = new System.Windows.Forms.Label();
            this.versionLabel    = new System.Windows.Forms.Label();
            // legacy
            this.headerPanel  = new System.Windows.Forms.Panel();
            this.bodyPanel    = new System.Windows.Forms.Panel();
            this.welcomeLabel = new System.Windows.Forms.Label();
            this.descLabel    = new System.Windows.Forms.Label();

            this.SuspendLayout();

            // ═══════════════════════════════════════════════════════════════════
            //  LEFT PANEL  (dark gradient + radar animation)
            // ═══════════════════════════════════════════════════════════════════
            this.leftPanel.Dock      = System.Windows.Forms.DockStyle.Left;
            this.leftPanel.Width     = 380;
            this.leftPanel.BackColor = Color.FromArgb(12, 36, 144);
            this.leftPanel.Paint    += LeftPanel_Paint;

            // IVAO logo in top-left of left panel
            var leftLogoBox = new System.Windows.Forms.PictureBox
            {
                Size      = new Size(110, 38),
                SizeMode  = System.Windows.Forms.PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Location  = new Point(36, 36),
            };
            try   { leftLogoBox.Image = Image.FromFile("./ivao_blue.png"); }
            catch { /* no logo fallback needed on dark panel */ }

            // Hero text area anchored to the bottom of the left panel
            var heroArea = new System.Windows.Forms.Panel
            {
                Dock      = System.Windows.Forms.DockStyle.Bottom,
                Height    = 200,
                BackColor = Color.Transparent,
                Padding   = new System.Windows.Forms.Padding(36, 0, 36, 44),
            };

            // Tag chips row
            var tagsRow = new System.Windows.Forms.Panel
            {
                Dock      = System.Windows.Forms.DockStyle.Bottom,
                Height    = 34,
                BackColor = Color.Transparent,
            };
            var tagNames = new[] { "AIRAC Data", "SID/STAR", "Sector Files" };
            int tagX = 0;
            foreach (var tagText in tagNames)
            {
                tagsRow.Controls.Add(new System.Windows.Forms.Label
                {
                    Text      = tagText,
                    Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(147, 197, 253),
                    BackColor = Color.FromArgb(30, 58, 138),
                    AutoSize  = false,
                    Size      = new Size(84, 22),
                    Location  = new Point(tagX, 6),
                    TextAlign = ContentAlignment.MiddleCenter,
                });
                tagX += 90;
            }

            this.heroSubLabel.Text      = "Generate sector files for EuroScope\nand other ATC client software.";
            this.heroSubLabel.ForeColor = Color.FromArgb(147, 197, 253);
            this.heroSubLabel.Font      = new Font("Segoe UI", 9.5f);
            this.heroSubLabel.BackColor = Color.Transparent;
            this.heroSubLabel.Dock      = System.Windows.Forms.DockStyle.Top;
            this.heroSubLabel.Height    = 50;
            this.heroSubLabel.TextAlign = ContentAlignment.TopLeft;

            this.heroLabel.Text      = "Build & Create\nSector Files";
            this.heroLabel.ForeColor = Color.White;
            this.heroLabel.Font      = new Font("Segoe UI", 22f, FontStyle.Bold);
            this.heroLabel.BackColor = Color.Transparent;
            this.heroLabel.Dock      = System.Windows.Forms.DockStyle.Top;
            this.heroLabel.Height    = 82;
            this.heroLabel.TextAlign = ContentAlignment.BottomLeft;

            heroArea.Controls.Add(tagsRow);
            heroArea.Controls.Add(this.heroSubLabel);
            heroArea.Controls.Add(this.heroLabel);

            this.leftPanel.Controls.Add(heroArea);
            this.leftPanel.Controls.Add(leftLogoBox);

            // ═══════════════════════════════════════════════════════════════════
            //  RIGHT PANEL  (white, centered login card)
            // ═══════════════════════════════════════════════════════════════════
            this.rightPanel.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.BackColor = Color.White;

            // Center card inside right panel
            var card = new System.Windows.Forms.Panel
            {
                BackColor = Color.White,
                Size      = new Size(340, 390),
            };
            this.rightPanel.Controls.Add(card);
            this.rightPanel.Resize += (s, e) =>
            {
                int cx = (rightPanel.ClientSize.Width  - card.Width)  / 2;
                int cy = (rightPanel.ClientSize.Height - card.Height) / 2;
                card.Location = new Point(Math.Max(0, cx), Math.Max(12, cy));
            };

            // ── Logo (IVAO image, top of card) ────────────────────────────────
            this.logoLabel.Size      = new Size(120, 42);
            this.logoLabel.Location  = new Point(0, 0);
            this.logoLabel.BackColor = Color.Transparent;
            this.logoLabel.TextAlign = ContentAlignment.MiddleLeft;
            try
            {
                this.logoLabel.Image     = Image.FromFile("./ivao_blue.png");
                this.logoLabel.ImageAlign = ContentAlignment.MiddleLeft;
                this.logoLabel.Text      = "";
            }
            catch
            {
                this.logoLabel.Text      = "IVAO";
                this.logoLabel.Font      = new Font("Segoe UI", 13f, FontStyle.Bold);
                this.logoLabel.ForeColor = Color.FromArgb(12, 36, 144);
            }

            // ── App name heading ──────────────────────────────────────────────
            this.titleLabel.Text      = AppInfo.Name;
            this.titleLabel.Font      = new Font("Segoe UI", 19f, FontStyle.Bold);
            this.titleLabel.ForeColor = Color.FromArgb(10, 20, 60);
            this.titleLabel.BackColor = Color.Transparent;
            this.titleLabel.Size      = new Size(340, 50);
            this.titleLabel.Location  = new Point(0, 56);
            this.titleLabel.TextAlign = ContentAlignment.MiddleLeft;

            // ── Subtitle ──────────────────────────────────────────────────────
            this.subtitleLabel.Text      = "Sign in with your IVAO account to continue.";
            this.subtitleLabel.Font      = new Font("Segoe UI", 9f);
            this.subtitleLabel.ForeColor = Color.FromArgb(100, 115, 145);
            this.subtitleLabel.BackColor = Color.Transparent;
            this.subtitleLabel.Size      = new Size(340, 24);
            this.subtitleLabel.Location  = new Point(0, 108);
            this.subtitleLabel.TextAlign = ContentAlignment.MiddleLeft;

            // ── Thin divider ──────────────────────────────────────────────────
            var divider = new System.Windows.Forms.Panel
            {
                Size      = new Size(340, 1),
                Location  = new Point(0, 142),
                BackColor = Color.FromArgb(226, 232, 245),
            };

            // ── AIRAC badge (populated in OnLoad) ─────────────────────────────
            this.airacBadgeLabel.Text      = "";
            this.airacBadgeLabel.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            this.airacBadgeLabel.ForeColor = Color.FromArgb(46, 125, 50);
            this.airacBadgeLabel.BackColor = Color.Transparent;
            this.airacBadgeLabel.Size      = new Size(340, 24);
            this.airacBadgeLabel.Location  = new Point(0, 152);
            this.airacBadgeLabel.TextAlign = ContentAlignment.MiddleLeft;

            // ── SSO button ────────────────────────────────────────────────────
            this.LoginButton.Text      = "Continue with IVAO SSO";
            this.LoginButton.Font      = new Font("Segoe UI", 11f, FontStyle.Bold);
            this.LoginButton.BackColor = Color.FromArgb(12, 50, 160);
            this.LoginButton.ForeColor = Color.White;
            this.LoginButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LoginButton.FlatAppearance.BorderSize = 0;
            this.LoginButton.UseVisualStyleBackColor   = false;
            this.LoginButton.Size      = new Size(340, 50);
            this.LoginButton.Location  = new Point(0, 186);
            this.LoginButton.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.LoginButton.Click    += new System.EventHandler(this.LoginButton_Click);
            this.LoginButton.Paint    += (s, e) =>
            {
                var btn = (System.Windows.Forms.Button)s;
                using var br = new LinearGradientBrush(
                    btn.ClientRectangle,
                    btn.Enabled ? Color.FromArgb(25, 80, 210) : Color.FromArgb(120, 140, 170),
                    btn.Enabled ? Color.FromArgb(7,  40, 120) : Color.FromArgb(90,  110, 140),
                    LinearGradientMode.Horizontal);
                e.Graphics.FillRectangle(br, btn.ClientRectangle);
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString(btn.Text, btn.Font, Brushes.White, btn.ClientRectangle, sf);
            };

            // ── Staff notice (amber) ──────────────────────────────────────────
            this.notePanel.BackColor = Color.FromArgb(255, 251, 235);
            this.notePanel.Size      = new Size(340, 58);
            this.notePanel.Location  = new Point(0, 250);
            this.notePanel.Paint    += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(252, 211, 77), 1.5f);
                e.Graphics.DrawRectangle(pen, 0, 0, notePanel.Width - 1, notePanel.Height - 1);
            };

            this.noteLabel.Text      = "Staff access only\nRestricted to authorised IVAO staff members.";
            this.noteLabel.Font      = new Font("Segoe UI", 8.5f);
            this.noteLabel.ForeColor = Color.FromArgb(146, 64, 14);
            this.noteLabel.BackColor = Color.Transparent;
            this.noteLabel.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.noteLabel.TextAlign = ContentAlignment.MiddleLeft;
            this.noteLabel.Padding   = new System.Windows.Forms.Padding(12, 0, 12, 0);
            this.notePanel.Controls.Add(this.noteLabel);

            // ── NOT FOR REAL WORLD USE ────────────────────────────────────────
            this.versionLabel.Text      = "⚠  NOT FOR REAL WORLD USE  ·  v1.0";
            this.versionLabel.Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            this.versionLabel.ForeColor = Color.FromArgb(185, 28, 28);
            this.versionLabel.BackColor = Color.Transparent;
            this.versionLabel.Size      = new Size(340, 22);
            this.versionLabel.Location  = new Point(0, 322);
            this.versionLabel.TextAlign = ContentAlignment.MiddleLeft;

            card.Controls.Add(this.logoLabel);
            card.Controls.Add(this.titleLabel);
            card.Controls.Add(this.subtitleLabel);
            card.Controls.Add(divider);
            card.Controls.Add(this.airacBadgeLabel);
            card.Controls.Add(this.LoginButton);
            card.Controls.Add(this.notePanel);
            card.Controls.Add(this.versionLabel);

            // ── Radar animation timer ─────────────────────────────────────────
            this.radarTimer.Interval = 50;
            this.radarTimer.Tick    += (s, e) =>
            {
                _radarAngle = (_radarAngle + 3f) % 360f;
                leftPanel.Invalidate();
            };
            this.radarTimer.Start();

            // ── Form ──────────────────────────────────────────────────────────
            this.Controls.Add(this.rightPanel);
            this.Controls.Add(this.leftPanel);

            this.AutoScaleMode   = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize      = new System.Drawing.Size(880, 560);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.ControlBox      = true;
            this.MaximizeBox     = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text            = $"Login  |  {AppInfo.Name}";
            this.BackColor       = Color.White;

            this.ResumeLayout(false);
        }

        // ── Paint handler for left dark panel ────────────────────────────────
        private void LeftPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            var g    = e.Graphics;
            var rect = leftPanel.ClientRectangle;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Background gradient: #0C2490 → #040D38
            using (var bg = new LinearGradientBrush(
                rect,
                Color.FromArgb(12,  36, 144),
                Color.FromArgb(4,   13,  56),
                LinearGradientMode.ForwardDiagonal))
            {
                g.FillRectangle(bg, rect);
            }

            // Radar origin - centre of panel, shifted slightly up
            int cx = rect.Width  / 2 + 24;
            int cy = rect.Height / 2 - 10;

            // Concentric dashed rings
            int[] radii = { 55, 105, 155, 205 };
            using var ringPen = new Pen(Color.FromArgb(40, 100, 180, 255), 1f);
            ringPen.DashStyle = DashStyle.Custom;
            ringPen.DashPattern = new float[] { 4f, 4f };
            foreach (int r in radii)
                g.DrawEllipse(ringPen, cx - r, cy - r, r * 2, r * 2);

            // Cross-hair lines
            using var crossPen = new Pen(Color.FromArgb(28, 100, 180, 255), 1f);
            g.DrawLine(crossPen, cx - 215, cy, cx + 215, cy);
            g.DrawLine(crossPen, cx, cy - 215, cx, cy + 215);

            // Sweep: filled pie + trailing fade
            double ang = _radarAngle * Math.PI / 180.0;
            var pieBounds = new RectangleF(cx - 205, cy - 205, 410, 410);

            using (var sweep1 = new SolidBrush(Color.FromArgb(22, 0, 230, 100)))
                g.FillPie(sweep1, pieBounds.X, pieBounds.Y, pieBounds.Width, pieBounds.Height, _radarAngle - 30f, 30f);
            using (var sweep2 = new SolidBrush(Color.FromArgb(12, 0, 230, 100)))
                g.FillPie(sweep2, pieBounds.X, pieBounds.Y, pieBounds.Width, pieBounds.Height, _radarAngle - 50f, 20f);

            // Sweep leading edge line
            float ex = cx + 205f * (float)Math.Cos(ang);
            float ey = cy + 205f * (float)Math.Sin(ang);
            using var linePen = new Pen(Color.FromArgb(110, 0, 255, 130), 1.5f);
            g.DrawLine(linePen, cx, cy, ex, ey);

            // Blip dots near the leading edge
            var blipPositions = new (float frac, float off)[] { (0.55f, 8f), (0.72f, -6f), (0.88f, 12f) };
            using var blipBrush = new SolidBrush(Color.FromArgb(180, 0, 255, 140));
            foreach (var (frac, off) in blipPositions)
            {
                float bx = cx + frac * 205f * (float)Math.Cos(ang + off * Math.PI / 180.0);
                float by = cy + frac * 205f * (float)Math.Sin(ang + off * Math.PI / 180.0);
                g.FillEllipse(blipBrush, bx - 3, by - 3, 6, 6);
            }
        }
    }
}
