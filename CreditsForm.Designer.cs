using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sector_File
{
    partial class CreditsForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel      headerPanel;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Label      logoFallback;
        private System.Windows.Forms.Panel      bodyPanel;
        private System.Windows.Forms.Label      madeByLabel;
        private System.Windows.Forms.LinkLabel  vedaLink;
        private System.Windows.Forms.Label      testedByLabel;
        private System.Windows.Forms.LinkLabel  nilayLink;
        private System.Windows.Forms.Label      divider1;
        private System.Windows.Forms.Label      dataByLabel;
        private System.Windows.Forms.LinkLabel  airacLink;
        private System.Windows.Forms.Label      divider2;
        private System.Windows.Forms.Label      warningLabel;
        private System.Windows.Forms.Button     closeButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.headerPanel    = new System.Windows.Forms.Panel();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.logoFallback   = new System.Windows.Forms.Label();
            this.bodyPanel      = new System.Windows.Forms.Panel();
            this.madeByLabel    = new System.Windows.Forms.Label();
            this.vedaLink       = new System.Windows.Forms.LinkLabel();
            this.testedByLabel  = new System.Windows.Forms.Label();
            this.nilayLink      = new System.Windows.Forms.LinkLabel();
            this.divider1       = new System.Windows.Forms.Label();
            this.dataByLabel    = new System.Windows.Forms.Label();
            this.airacLink      = new System.Windows.Forms.LinkLabel();
            this.divider2       = new System.Windows.Forms.Label();
            this.warningLabel   = new System.Windows.Forms.Label();
            this.closeButton    = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── Header ───────────────────────────────────────────────────────
            this.headerPanel.Dock      = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Height    = 130;
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(13, 27, 75);

            this.headerPanel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                // Gradient accent strip at the bottom of the header
                using var grad = new LinearGradientBrush(
                    new System.Drawing.Point(0, headerPanel.Height - 4),
                    new System.Drawing.Point(headerPanel.Width, headerPanel.Height - 4),
                    System.Drawing.Color.FromArgb(225, 29, 72),
                    System.Drawing.Color.FromArgb(37, 99, 235));
                g.FillRectangle(grad, 0, headerPanel.Height - 4, headerPanel.Width, 4);
            };

            this.logoPictureBox.Location  = new System.Drawing.Point(160, 15);
            this.logoPictureBox.Size      = new System.Drawing.Size(200, 96);
            this.logoPictureBox.SizeMode  = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoPictureBox.BackColor = System.Drawing.Color.Transparent;

            this.logoFallback.Text      = "IVAO";
            this.logoFallback.ForeColor = System.Drawing.Color.White;
            this.logoFallback.Font      = new System.Drawing.Font("Segoe UI", 30f, System.Drawing.FontStyle.Bold);
            this.logoFallback.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.logoFallback.Location  = new System.Drawing.Point(160, 15);
            this.logoFallback.Size      = new System.Drawing.Size(200, 96);

            this.headerPanel.Controls.Add(this.logoPictureBox);
            this.headerPanel.Controls.Add(this.logoFallback);
            this.headerPanel.Resize += (s, e) =>
            {
                int cx = (headerPanel.ClientSize.Width - 200) / 2;
                logoPictureBox.Location = new System.Drawing.Point(cx, 15);
                logoFallback.Location   = new System.Drawing.Point(cx, 15);
            };

            // ── Body ─────────────────────────────────────────────────────────
            this.bodyPanel.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.bodyPanel.BackColor = System.Drawing.Color.White;
            this.bodyPanel.Padding   = new System.Windows.Forms.Padding(32, 20, 32, 8);

            // ── Person card helper ────────────────────────────────────────────
            System.Windows.Forms.Panel MakePersonCard(
                string initials, System.Drawing.Color avatarColor,
                string name, string id, System.Windows.Forms.LinkLabel link)
            {
                var card = new System.Windows.Forms.Panel
                {
                    Dock      = System.Windows.Forms.DockStyle.Top,
                    Height    = 60,
                    BackColor = System.Drawing.Color.FromArgb(248, 250, 252),
                };
                card.Paint += (s, e) =>
                {
                    using var pen = new System.Drawing.Pen(
                        System.Drawing.Color.FromArgb(226, 232, 240), 1);
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                };

                // Avatar circle (painted, no Region clipping needed)
                var avatar = new System.Windows.Forms.Panel
                {
                    Size      = new System.Drawing.Size(40, 40),
                    Location  = new System.Drawing.Point(12, 10),
                    BackColor = System.Drawing.Color.Transparent,
                };
                var capturedColor = avatarColor;
                var capturedText  = initials;
                avatar.Paint += (ps, pe) =>
                {
                    pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using var b = new System.Drawing.SolidBrush(capturedColor);
                    pe.Graphics.FillEllipse(b, 0, 0, 39, 39);
                    using var sf = new System.Drawing.StringFormat
                    {
                        Alignment     = System.Drawing.StringAlignment.Center,
                        LineAlignment = System.Drawing.StringAlignment.Center,
                    };
                    using var f = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold);
                    pe.Graphics.DrawString(capturedText, f, System.Drawing.Brushes.White,
                        new System.Drawing.RectangleF(0, 0, 39, 39), sf);
                };

                // Name label
                var nameLabel = new System.Windows.Forms.Label
                {
                    Text      = name,
                    Font      = new System.Drawing.Font("Segoe UI", 10.5f, System.Drawing.FontStyle.Bold),
                    ForeColor = System.Drawing.Color.FromArgb(17, 24, 39),
                    Location  = new System.Drawing.Point(64, 10),
                    AutoSize  = true,
                    BackColor = System.Drawing.Color.Transparent,
                };

                // ID badge
                var idLabel = new System.Windows.Forms.Label
                {
                    Text      = id,
                    Font      = new System.Drawing.Font("Segoe UI", 8f),
                    ForeColor = System.Drawing.Color.FromArgb(100, 116, 139),
                    Location  = new System.Drawing.Point(64, 34),
                    AutoSize  = true,
                    BackColor = System.Drawing.Color.Transparent,
                };

                link.Font      = new System.Drawing.Font("Segoe UI", 8f);
                link.LinkColor = System.Drawing.Color.FromArgb(37, 99, 235);
                link.ActiveLinkColor = System.Drawing.Color.FromArgb(29, 78, 216);
                link.AutoSize  = true;
                link.BackColor = System.Drawing.Color.Transparent;

                card.Resize += (s, e) =>
                {
                    link.Location = new System.Drawing.Point(card.Width - link.Width - 16, 22);
                };

                card.Controls.Add(avatar);
                card.Controls.Add(nameLabel);
                card.Controls.Add(idLabel);
                card.Controls.Add(link);

                return card;
            }

            // ── Section label helper ──────────────────────────────────────────
            System.Windows.Forms.Label MakeSectionLabel(string text)
                => new System.Windows.Forms.Label
                {
                    Text      = text,
                    Font      = new System.Drawing.Font("Segoe UI", 7.5f, System.Drawing.FontStyle.Bold),
                    ForeColor = System.Drawing.Color.FromArgb(148, 163, 184),
                    Dock      = System.Windows.Forms.DockStyle.Top,
                    Height    = 28,
                    TextAlign = System.Drawing.ContentAlignment.BottomLeft,
                };

            System.Windows.Forms.Panel MakeDivider()
                => new System.Windows.Forms.Panel
                {
                    Dock      = System.Windows.Forms.DockStyle.Top,
                    Height    = 1,
                    BackColor = System.Drawing.Color.FromArgb(226, 232, 240),
                };

            System.Windows.Forms.Panel MakeGap(int h)
                => new System.Windows.Forms.Panel
                {
                    Dock      = System.Windows.Forms.DockStyle.Top,
                    Height    = h,
                    BackColor = System.Drawing.Color.Transparent,
                };

            // ── Link targets (wired in CreditsForm.cs) ───────────────────────
            this.vedaLink.Text  = "View IVAO profile →";
            this.nilayLink.Text = "View IVAO profile →";
            this.airacLink.Text    = "";
            this.airacLink.Visible = false;

            this.vedaLink.LinkClicked  += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.vedaLink_LinkClicked);
            this.nilayLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.nilayLink_LinkClicked);

            var vedaCard  = MakePersonCard("VM", System.Drawing.Color.FromArgb(37,  99, 235),
                                           "Veda Moola",      "656077", this.vedaLink);
            var nilayCard = MakePersonCard("NP", System.Drawing.Color.FromArgb(5,  150, 105),
                                           "Nilay Parsodkar", "709833", this.nilayLink);

            // Warning label
            this.warningLabel.Text      = "⚠   NOT FOR REAL WORLD USE   ⚠";
            this.warningLabel.ForeColor = System.Drawing.Color.FromArgb(200, 60, 60);
            this.warningLabel.Font      = new System.Drawing.Font("Segoe UI", 8f, System.Drawing.FontStyle.Bold);
            this.warningLabel.Dock      = System.Windows.Forms.DockStyle.Top;
            this.warningLabel.Height    = 28;
            this.warningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.warningLabel.BackColor = System.Drawing.Color.FromArgb(255, 241, 242);

            // Close button
            this.closeButton.Text               = "Close";
            this.closeButton.Font               = new System.Drawing.Font("Segoe UI", 10f, System.Drawing.FontStyle.Bold);
            this.closeButton.BackColor          = System.Drawing.Color.FromArgb(13, 27, 75);
            this.closeButton.ForeColor          = System.Drawing.Color.White;
            this.closeButton.FlatStyle          = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.FlatAppearance.BorderSize = 0;
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Dock               = System.Windows.Forms.DockStyle.Bottom;
            this.closeButton.Height             = 44;
            this.closeButton.Cursor             = System.Windows.Forms.Cursors.Hand;
            this.closeButton.Click             += new System.EventHandler(this.closeButton_Click);

            // Unused fields wired to satisfy designer
            this.madeByLabel   = MakeSectionLabel("");  // replaced by inline labels
            this.testedByLabel = MakeSectionLabel("");
            this.dataByLabel   = MakeSectionLabel("");
            this.divider1 = new System.Windows.Forms.Label { Visible = false };
            this.divider2 = new System.Windows.Forms.Label { Visible = false };

            // Free-API notice label
            var apiNoticeLabel = new System.Windows.Forms.Label
            {
                Text      = "This application sources aviation and navigation data from\n" +
                            "free, publicly available APIs across multiple providers.\n" +
                            "All usage is legal, safe, and for simulation purposes only.",
                Font      = new System.Drawing.Font("Segoe UI", 9f),
                ForeColor = System.Drawing.Color.FromArgb(71, 85, 105),
                Dock      = System.Windows.Forms.DockStyle.Top,
                Height    = 58,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                BackColor = System.Drawing.Color.FromArgb(241, 245, 249),
                Padding   = new System.Windows.Forms.Padding(8, 0, 8, 0),
            };

            // Stack body controls - last added = topmost (DockStyle.Top)
            this.bodyPanel.Controls.Add(apiNoticeLabel);
            this.bodyPanel.Controls.Add(MakeSectionLabel("DATA SOURCES"));
            this.bodyPanel.Controls.Add(MakeDivider());
            this.bodyPanel.Controls.Add(MakeGap(8));
            this.bodyPanel.Controls.Add(nilayCard);
            this.bodyPanel.Controls.Add(MakeGap(6));
            this.bodyPanel.Controls.Add(vedaCard);
            this.bodyPanel.Controls.Add(MakeSectionLabel("MADE BY  &  TESTED BY"));
            this.bodyPanel.Controls.Add(MakeDivider());
            this.bodyPanel.Controls.Add(MakeGap(8));
            this.bodyPanel.Controls.Add(this.warningLabel);
            this.bodyPanel.Controls.Add(MakeGap(8));

            // ── Form ─────────────────────────────────────────────────────────
            this.Controls.Add(this.bodyPanel);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.closeButton);

            this.AutoScaleMode   = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize      = new System.Drawing.Size(520, 480);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text            = $"Credits  |  {AppInfo.Name}";
            this.BackColor       = System.Drawing.Color.White;
            this.ResumeLayout(false);
        }
    }
}
