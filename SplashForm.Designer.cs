using System.Drawing;
using System.Drawing.Drawing2D;

namespace Sector_File
{
    partial class SplashForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel       rootPanel;
        private System.Windows.Forms.Panel       headerStripe;
        private System.Windows.Forms.Label       appTitleLabel;
        private System.Windows.Forms.Label       appSubtitleLabel;
        private System.Windows.Forms.PictureBox  logoPicture;
        private System.Windows.Forms.Panel       bodyPanel;
        private System.Windows.Forms.Label       divisionLabel;
        private System.Windows.Forms.Label       step1Label;
        private System.Windows.Forms.Label       step2Label;
        private System.Windows.Forms.Label       step3Label;
        private System.Windows.Forms.Label       step4Label;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label       versionLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.rootPanel        = new System.Windows.Forms.Panel();
            this.headerStripe     = new System.Windows.Forms.Panel();
            this.appTitleLabel    = new System.Windows.Forms.Label();
            this.appSubtitleLabel = new System.Windows.Forms.Label();
            this.logoPicture      = new System.Windows.Forms.PictureBox();
            this.bodyPanel        = new System.Windows.Forms.Panel();
            this.divisionLabel    = new System.Windows.Forms.Label();
            this.step1Label       = new System.Windows.Forms.Label();
            this.step2Label       = new System.Windows.Forms.Label();
            this.step3Label       = new System.Windows.Forms.Label();
            this.step4Label       = new System.Windows.Forms.Label();
            this.progressBar      = new System.Windows.Forms.ProgressBar();
            this.versionLabel     = new System.Windows.Forms.Label();

            this.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPicture)).BeginInit();

            // ─────────────────────────────────────────────────────────────────────
            //  Layout (top → bottom):
            //    headerStripe  90px   blue gradient
            //    logoPicture   100px  white, IVAO logo
            //    bodyPanel     Fill   white, steps + credits
            //    warningBand   38px   red tint strip
            //    progressBar   14px   full width
            // ─────────────────────────────────────────────────────────────────────

            // ── Root ─────────────────────────────────────────────────────────────
            this.rootPanel.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.rootPanel.BackColor = Color.White;

            // ── Blue gradient header ──────────────────────────────────────────────
            this.headerStripe.Dock      = System.Windows.Forms.DockStyle.Top;
            this.headerStripe.Height    = 90;
            this.headerStripe.BackColor = Color.FromArgb(12, 36, 144);
            this.headerStripe.Paint    += (s, e) =>
            {
                using var br = new LinearGradientBrush(
                    this.headerStripe.ClientRectangle,
                    Color.FromArgb(12, 36, 144),
                    Color.FromArgb(7, 20, 89),
                    LinearGradientMode.Horizontal);
                e.Graphics.FillRectangle(br, this.headerStripe.ClientRectangle);
            };

            this.appTitleLabel.Text      = AppInfo.Name;
            this.appTitleLabel.Font      = new Font("Segoe UI", 18f, FontStyle.Bold);
            this.appTitleLabel.ForeColor = Color.White;
            this.appTitleLabel.BackColor = Color.Transparent;
            this.appTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.appTitleLabel.Dock      = System.Windows.Forms.DockStyle.Top;
            this.appTitleLabel.Height    = 60;
            this.appTitleLabel.Padding   = new System.Windows.Forms.Padding(0, 12, 0, 0);

            this.appSubtitleLabel.Text      = "ATC && Flight Operations";
            this.appSubtitleLabel.ForeColor = Color.FromArgb(170, 200, 255);
            this.appSubtitleLabel.Font      = new Font("Segoe UI", 9f);
            this.appSubtitleLabel.BackColor = Color.Transparent;
            this.appSubtitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.appSubtitleLabel.Dock      = System.Windows.Forms.DockStyle.Top;
            this.appSubtitleLabel.Height    = 36;
            this.appSubtitleLabel.Padding   = new System.Windows.Forms.Padding(0, 0, 0, 10);

            this.headerStripe.Controls.Add(this.appSubtitleLabel);
            this.headerStripe.Controls.Add(this.appTitleLabel);

            // ── IVAO logo (white panel) ───────────────────────────────────────────
            this.logoPicture.Dock      = System.Windows.Forms.DockStyle.Top;
            this.logoPicture.Height    = 120;
            this.logoPicture.BackColor = Color.White;
            this.logoPicture.SizeMode  = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoPicture.Padding   = new System.Windows.Forms.Padding(80, 12, 80, 12);
            try { this.logoPicture.Image = Image.FromFile("./ivao_blue.png"); } catch { }

            // ── Warning band (red tint, full width) ───────────────────────────────
            var warningBand = new System.Windows.Forms.Panel
            {
                Dock      = System.Windows.Forms.DockStyle.Bottom,
                Height    = 38,
                BackColor = Color.FromArgb(255, 238, 238),
            };
            this.versionLabel.Text      = "⚠   NOT FOR REAL WORLD USE";
            this.versionLabel.ForeColor = Color.FromArgb(185, 28, 28);
            this.versionLabel.Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            this.versionLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.versionLabel.BackColor = Color.Transparent;
            this.versionLabel.Dock      = System.Windows.Forms.DockStyle.Fill;
            warningBand.Controls.Add(this.versionLabel);

            // ── Progress bar (full width, below warning) ──────────────────────────
            this.progressBar.Dock    = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar.Height  = 14;
            this.progressBar.Style   = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.Minimum = 0;
            this.progressBar.Maximum = 100;
            this.progressBar.Value   = 0;

            // ── Body (white, fills between logo and warning) ──────────────────────
            this.bodyPanel.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.bodyPanel.BackColor = Color.White;
            this.bodyPanel.Padding   = new System.Windows.Forms.Padding(52, 0, 52, 0);

            // Thin separator line below logo
            var topLine = new System.Windows.Forms.Panel
            {
                Dock      = System.Windows.Forms.DockStyle.Top,
                Height    = 1,
                BackColor = Color.FromArgb(228, 235, 248),
            };

            this.divisionLabel.Text      = "Made by Veda Moola  (656077)";
            this.divisionLabel.Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            this.divisionLabel.ForeColor = Color.FromArgb(55, 75, 110);
            this.divisionLabel.BackColor = Color.White;
            this.divisionLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.divisionLabel.Dock      = System.Windows.Forms.DockStyle.Top;
            this.divisionLabel.Height    = 32;
            this.divisionLabel.Padding   = new System.Windows.Forms.Padding(0, 6, 0, 0);

            var stepDivider = new System.Windows.Forms.Panel
            {
                Dock      = System.Windows.Forms.DockStyle.Top,
                Height    = 1,
                BackColor = Color.FromArgb(228, 235, 248),
            };

            var stepFont  = new Font("Segoe UI", 10.5f);
            var stepColor = Color.FromArgb(45, 65, 105);

            this.step1Label.Text      = "○   Initializing application";
            this.step1Label.ForeColor = stepColor;
            this.step1Label.Font      = stepFont;
            this.step1Label.BackColor = Color.White;
            this.step1Label.Dock      = System.Windows.Forms.DockStyle.Top;
            this.step1Label.Height    = 44;
            this.step1Label.Padding   = new System.Windows.Forms.Padding(4, 10, 0, 0);
            this.step1Label.TextAlign = ContentAlignment.TopLeft;

            this.step2Label.Text      = "○   Checking network connection";
            this.step2Label.ForeColor = stepColor;
            this.step2Label.Font      = stepFont;
            this.step2Label.BackColor = Color.White;
            this.step2Label.Dock      = System.Windows.Forms.DockStyle.Top;
            this.step2Label.Height    = 40;
            this.step2Label.Padding   = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.step2Label.TextAlign = ContentAlignment.MiddleLeft;

            this.step3Label.Text      = "○   Fetching AIRAC cycle data";
            this.step3Label.ForeColor = stepColor;
            this.step3Label.Font      = stepFont;
            this.step3Label.BackColor = Color.White;
            this.step3Label.Dock      = System.Windows.Forms.DockStyle.Top;
            this.step3Label.Height    = 40;
            this.step3Label.Padding   = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.step3Label.TextAlign = ContentAlignment.MiddleLeft;

            this.step4Label.Text      = "○   Preparing login";
            this.step4Label.ForeColor = stepColor;
            this.step4Label.Font      = stepFont;
            this.step4Label.BackColor = Color.White;
            this.step4Label.Dock      = System.Windows.Forms.DockStyle.Top;
            this.step4Label.Height    = 40;
            this.step4Label.Padding   = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.step4Label.TextAlign = ContentAlignment.MiddleLeft;

            // Version label at bottom of body
            var buildLabel = new System.Windows.Forms.Label
            {
                Text      = "v1.0",
                ForeColor = Color.FromArgb(180, 190, 210),
                Font      = new Font("Segoe UI", 7.5f),
                BackColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock      = System.Windows.Forms.DockStyle.Bottom,
                Height    = 26,
            };

            // Body: top items (last added = topmost), bottom items (first added = bottommost)
            this.bodyPanel.Controls.Add(this.step4Label);
            this.bodyPanel.Controls.Add(this.step3Label);
            this.bodyPanel.Controls.Add(this.step2Label);
            this.bodyPanel.Controls.Add(this.step1Label);
            this.bodyPanel.Controls.Add(stepDivider);
            this.bodyPanel.Controls.Add(this.divisionLabel);
            this.bodyPanel.Controls.Add(topLine);
            this.bodyPanel.Controls.Add(buildLabel);   // bottom

            // White gap between header and logo
            var headerGap = new System.Windows.Forms.Panel
            {
                Dock      = System.Windows.Forms.DockStyle.Top,
                Height    = 24,
                BackColor = Color.White,
            };

            // Root: Top items (last added = topmost); Bottom items (first added = bottommost)
            this.rootPanel.Controls.Add(this.progressBar);  // bottom-most
            this.rootPanel.Controls.Add(warningBand);       // above progress
            this.rootPanel.Controls.Add(this.bodyPanel);    // fill
            this.rootPanel.Controls.Add(this.logoPicture);  // top (below gap)
            this.rootPanel.Controls.Add(headerGap);         // top (below header)
            this.rootPanel.Controls.Add(this.headerStripe); // topmost

            this.Controls.Add(this.rootPanel);

            // ── Form ─────────────────────────────────────────────────────────────
            this.AutoScaleMode   = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize      = new System.Drawing.Size(680, 500);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ControlBox      = false;
            this.MinimizeBox     = false;
            this.MaximizeBox     = false;
            this.ShowInTaskbar   = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text            = AppInfo.Name;
            this.BackColor       = Color.White;
            this.Load           += new System.EventHandler(this.SplashForm_Load);

            ((System.ComponentModel.ISupportInitialize)(this.logoPicture)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
