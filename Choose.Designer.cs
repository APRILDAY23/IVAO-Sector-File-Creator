// NOTE: Choose.cs is kept for backwards compatibility but is no longer the main entry point.
// MainForm now handles the hub navigation. Choose.cs is unused but retained.
namespace Sector_File
{
    partial class Choose
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel  headerPanel;
        private System.Windows.Forms.Label  appLabel;
        private System.Windows.Forms.Label  welcomeLabel;
        private System.Windows.Forms.Label  userIdLabel;
        private System.Windows.Forms.Panel  cardArea;
        private System.Windows.Forms.Panel  flightCard;
        private System.Windows.Forms.Label  flightIcon;
        private System.Windows.Forms.Label  flightTitle;
        private System.Windows.Forms.Label  flightDesc;
        private System.Windows.Forms.Panel  atcCard;
        private System.Windows.Forms.Label  atcIcon;
        private System.Windows.Forms.Label  atcTitle;
        private System.Windows.Forms.Label  atcDesc;
        private System.Windows.Forms.Panel  footerPanel;
        private System.Windows.Forms.Label  airacFooterLabel;
        private System.Windows.Forms.Button creditsButton;
        private System.Windows.Forms.Button flightOperationsButton;
        private System.Windows.Forms.Button atcOperationsButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.headerPanel     = new System.Windows.Forms.Panel();
            this.appLabel        = new System.Windows.Forms.Label();
            this.welcomeLabel    = new System.Windows.Forms.Label();
            this.userIdLabel     = new System.Windows.Forms.Label();
            this.cardArea        = new System.Windows.Forms.Panel();
            this.flightCard      = new System.Windows.Forms.Panel();
            this.flightIcon      = new System.Windows.Forms.Label();
            this.flightTitle     = new System.Windows.Forms.Label();
            this.flightDesc      = new System.Windows.Forms.Label();
            this.atcCard         = new System.Windows.Forms.Panel();
            this.atcIcon         = new System.Windows.Forms.Label();
            this.atcTitle        = new System.Windows.Forms.Label();
            this.atcDesc         = new System.Windows.Forms.Label();
            this.footerPanel     = new System.Windows.Forms.Panel();
            this.airacFooterLabel = new System.Windows.Forms.Label();
            this.creditsButton   = new System.Windows.Forms.Button();
            this.flightOperationsButton = new System.Windows.Forms.Button();
            this.atcOperationsButton    = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── Header ───────────────────────────────────────────────────────
            this.headerPanel.Dock      = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Height    = 90;
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(13, 71, 161);
            this.headerPanel.Padding   = new System.Windows.Forms.Padding(24, 0, 24, 0);

            this.appLabel.Text      = AppInfo.Name;
            this.appLabel.ForeColor = System.Drawing.Color.White;
            this.appLabel.Font      = new System.Drawing.Font("Segoe UI", 15f, System.Drawing.FontStyle.Bold);
            this.appLabel.Dock      = System.Windows.Forms.DockStyle.Top;
            this.appLabel.Height    = 46;
            this.appLabel.Padding   = new System.Windows.Forms.Padding(0, 14, 0, 0);
            this.appLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;

            this.welcomeLabel.Text      = "Welcome back";
            this.welcomeLabel.ForeColor = System.Drawing.Color.FromArgb(180, 210, 255);
            this.welcomeLabel.Font      = new System.Drawing.Font("Segoe UI", 9f);
            this.welcomeLabel.Dock      = System.Windows.Forms.DockStyle.Top;
            this.welcomeLabel.Height    = 22;
            this.welcomeLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;

            this.userIdLabel.Text      = "";
            this.userIdLabel.ForeColor = System.Drawing.Color.FromArgb(150, 180, 220);
            this.userIdLabel.Font      = new System.Drawing.Font("Segoe UI", 7.5f);
            this.userIdLabel.Dock      = System.Windows.Forms.DockStyle.Top;
            this.userIdLabel.Height    = 18;
            this.userIdLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;

            this.headerPanel.Controls.Add(this.userIdLabel);
            this.headerPanel.Controls.Add(this.welcomeLabel);
            this.headerPanel.Controls.Add(this.appLabel);

            // ── Card area ────────────────────────────────────────────────────
            this.cardArea.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.cardArea.BackColor = System.Drawing.Color.FromArgb(245, 248, 255);
            this.cardArea.Padding   = new System.Windows.Forms.Padding(30, 28, 30, 10);
            this.cardArea.Resize   += (s, e) => LayoutCards();

            // Flight card
            this.flightCard.BackColor = System.Drawing.Color.White;
            this.flightCard.Location  = new System.Drawing.Point(30, 28);
            this.flightCard.Size      = new System.Drawing.Size(270, 200);
            this.flightCard.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.flightCard.Click    += new System.EventHandler(this.flightOperationsButton_Click);
            this.flightCard.Paint    += (s, e) =>
            {
                using var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(220, 228, 245), 1);
                e.Graphics.DrawRectangle(pen, 0, 0, flightCard.Width - 1, flightCard.Height - 1);
            };

            this.flightIcon.Text      = "✈";
            this.flightIcon.ForeColor = System.Drawing.Color.FromArgb(255, 180, 0);
            this.flightIcon.Font      = new System.Drawing.Font("Segoe UI", 30f);
            this.flightIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.flightIcon.Dock      = System.Windows.Forms.DockStyle.Top;
            this.flightIcon.Height    = 72;
            this.flightIcon.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.flightIcon.Click    += new System.EventHandler(this.flightOperationsButton_Click);

            this.flightTitle.Text      = "Flight Operations";
            this.flightTitle.ForeColor = System.Drawing.Color.FromArgb(20, 30, 60);
            this.flightTitle.Font      = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold);
            this.flightTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.flightTitle.Dock      = System.Windows.Forms.DockStyle.Top;
            this.flightTitle.Height    = 32;
            this.flightTitle.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.flightTitle.Click    += new System.EventHandler(this.flightOperationsButton_Click);

            this.flightDesc.Text      = "Flight schedules & data";
            this.flightDesc.ForeColor = System.Drawing.Color.FromArgb(100, 120, 150);
            this.flightDesc.Font      = new System.Drawing.Font("Segoe UI", 8.5f);
            this.flightDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.flightDesc.Dock      = System.Windows.Forms.DockStyle.Top;
            this.flightDesc.Height    = 24;
            this.flightDesc.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.flightDesc.Click    += new System.EventHandler(this.flightOperationsButton_Click);

            this.flightOperationsButton.Text               = "Open";
            this.flightOperationsButton.Font               = new System.Drawing.Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Bold);
            this.flightOperationsButton.BackColor          = System.Drawing.Color.FromArgb(25, 118, 210);
            this.flightOperationsButton.ForeColor          = System.Drawing.Color.White;
            this.flightOperationsButton.FlatStyle          = System.Windows.Forms.FlatStyle.Flat;
            this.flightOperationsButton.FlatAppearance.BorderSize = 0;
            this.flightOperationsButton.UseVisualStyleBackColor = false;
            this.flightOperationsButton.Dock               = System.Windows.Forms.DockStyle.Bottom;
            this.flightOperationsButton.Height             = 38;
            this.flightOperationsButton.Cursor             = System.Windows.Forms.Cursors.Hand;
            this.flightOperationsButton.Click             += new System.EventHandler(this.flightOperationsButton_Click);

            this.flightCard.Controls.Add(this.flightDesc);
            this.flightCard.Controls.Add(this.flightTitle);
            this.flightCard.Controls.Add(this.flightIcon);
            this.flightCard.Controls.Add(this.flightOperationsButton);

            // ATC card
            this.atcCard.BackColor = System.Drawing.Color.White;
            this.atcCard.Location  = new System.Drawing.Point(330, 28);
            this.atcCard.Size      = new System.Drawing.Size(270, 200);
            this.atcCard.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.atcCard.Click    += new System.EventHandler(this.atcOperationsButton_Click);
            this.atcCard.Paint    += (s, e) =>
            {
                using var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(220, 228, 245), 1);
                e.Graphics.DrawRectangle(pen, 0, 0, atcCard.Width - 1, atcCard.Height - 1);
            };

            this.atcIcon.Text      = "📡";
            this.atcIcon.ForeColor = System.Drawing.Color.FromArgb(13, 71, 161);
            this.atcIcon.Font      = new System.Drawing.Font("Segoe UI", 30f);
            this.atcIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.atcIcon.Dock      = System.Windows.Forms.DockStyle.Top;
            this.atcIcon.Height    = 72;
            this.atcIcon.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.atcIcon.Click    += new System.EventHandler(this.atcOperationsButton_Click);

            this.atcTitle.Text      = "ATC Operations";
            this.atcTitle.ForeColor = System.Drawing.Color.FromArgb(20, 30, 60);
            this.atcTitle.Font      = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold);
            this.atcTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.atcTitle.Dock      = System.Windows.Forms.DockStyle.Top;
            this.atcTitle.Height    = 32;
            this.atcTitle.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.atcTitle.Click    += new System.EventHandler(this.atcOperationsButton_Click);

            this.atcDesc.Text      = "SID/STAR · Airport · FIR · Country";
            this.atcDesc.ForeColor = System.Drawing.Color.FromArgb(100, 120, 150);
            this.atcDesc.Font      = new System.Drawing.Font("Segoe UI", 8.5f);
            this.atcDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.atcDesc.Dock      = System.Windows.Forms.DockStyle.Top;
            this.atcDesc.Height    = 24;
            this.atcDesc.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.atcDesc.Click    += new System.EventHandler(this.atcOperationsButton_Click);

            this.atcOperationsButton.Text               = "Open";
            this.atcOperationsButton.Font               = new System.Drawing.Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Bold);
            this.atcOperationsButton.BackColor          = System.Drawing.Color.FromArgb(13, 71, 161);
            this.atcOperationsButton.ForeColor          = System.Drawing.Color.White;
            this.atcOperationsButton.FlatStyle          = System.Windows.Forms.FlatStyle.Flat;
            this.atcOperationsButton.FlatAppearance.BorderSize = 0;
            this.atcOperationsButton.UseVisualStyleBackColor = false;
            this.atcOperationsButton.Dock               = System.Windows.Forms.DockStyle.Bottom;
            this.atcOperationsButton.Height             = 38;
            this.atcOperationsButton.Cursor             = System.Windows.Forms.Cursors.Hand;
            this.atcOperationsButton.Click             += new System.EventHandler(this.atcOperationsButton_Click);

            this.atcCard.Controls.Add(this.atcDesc);
            this.atcCard.Controls.Add(this.atcTitle);
            this.atcCard.Controls.Add(this.atcIcon);
            this.atcCard.Controls.Add(this.atcOperationsButton);

            this.cardArea.Controls.Add(this.flightCard);
            this.cardArea.Controls.Add(this.atcCard);

            // ── Footer ───────────────────────────────────────────────────────
            this.footerPanel.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.footerPanel.Height    = 38;
            this.footerPanel.BackColor = System.Drawing.Color.FromArgb(245, 248, 255);
            this.footerPanel.Paint    += (s, e) =>
            {
                using var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(220, 228, 245), 1);
                e.Graphics.DrawLine(pen, 0, 0, footerPanel.Width, 0);
            };

            this.airacFooterLabel.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.airacFooterLabel.ForeColor = System.Drawing.Color.FromArgb(130, 150, 180);
            this.airacFooterLabel.Font      = new System.Drawing.Font("Segoe UI", 7.5f);
            this.airacFooterLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.airacFooterLabel.Padding   = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.airacFooterLabel.Text      = $"AIRAC {SplashForm.AiracCycle}  ·  NOT FOR REAL WORLD USE";

            this.creditsButton.Dock               = System.Windows.Forms.DockStyle.Right;
            this.creditsButton.Width              = 80;
            this.creditsButton.Text               = "Credits";
            this.creditsButton.Font               = new System.Drawing.Font("Segoe UI", 7.5f);
            this.creditsButton.ForeColor          = System.Drawing.Color.FromArgb(100, 130, 180);
            this.creditsButton.BackColor          = System.Drawing.Color.FromArgb(245, 248, 255);
            this.creditsButton.FlatStyle          = System.Windows.Forms.FlatStyle.Flat;
            this.creditsButton.FlatAppearance.BorderSize = 0;
            this.creditsButton.UseVisualStyleBackColor = false;
            this.creditsButton.Click             += new System.EventHandler(this.creditsButton_Click);

            this.footerPanel.Controls.Add(this.airacFooterLabel);
            this.footerPanel.Controls.Add(this.creditsButton);

            // ── Form ─────────────────────────────────────────────────────────
            this.Controls.Add(this.cardArea);
            this.Controls.Add(this.footerPanel);
            this.Controls.Add(this.headerPanel);

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize    = new System.Drawing.Size(660, 380);
            this.MaximizeBox   = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text          = AppInfo.Name;
            this.BackColor     = System.Drawing.Color.FromArgb(245, 248, 255);
            this.ResumeLayout(false);
        }
    }
}
