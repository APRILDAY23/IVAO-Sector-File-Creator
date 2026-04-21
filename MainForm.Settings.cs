using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Sector_File
{
    partial class MainForm
    {
        // ── Theme state ───────────────────────────────────────────────────────
        private static bool _darkMode = false;

        // ── API key storage ───────────────────────────────────────────────────
        internal static string _openAipApiKey = "";

        // ── Update section controls ───────────────────────────────────────────
        private Label  _updateStatusLabel;
        private Button _updateActionBtn;
        private UpdateInfo? _pendingUpdate;

        // Light palette
        private static readonly Color LightBg          = Color.FromArgb(245, 248, 255);
        private static readonly Color LightCard         = Color.White;
        private static readonly Color LightSidebar      = Color.White;
        private static readonly Color LightHeader       = Color.FromArgb(27, 43, 90);
        private static readonly Color LightHeaderFg     = Color.White;
        private static readonly Color LightText         = Color.FromArgb(20, 30, 60);
        private static readonly Color LightMuted        = Color.FromArgb(100, 120, 150);
        private static readonly Color LightLogBg        = Color.FromArgb(250, 251, 253);
        private static readonly Color LightLogFg        = Color.FromArgb(30, 40, 65);
        private static readonly Color LightPanelBorder  = Color.FromArgb(228, 235, 245);

        // Dark palette
        private static readonly Color DarkBg           = Color.FromArgb(12, 15, 26);
        private static readonly Color DarkCard          = Color.FromArgb(20, 26, 44);
        private static readonly Color DarkSidebar       = Color.FromArgb(15, 23, 42);
        private static readonly Color DarkHeader        = Color.FromArgb(10, 18, 50);
        private static readonly Color DarkText          = Color.FromArgb(220, 228, 248);
        private static readonly Color DarkMuted         = Color.FromArgb(120, 140, 170);
        private static readonly Color DarkLogBg         = Color.FromArgb(10, 13, 22);
        private static readonly Color DarkLogFg         = Color.FromArgb(200, 215, 240);
        private static readonly Color DarkPanelBorder   = Color.FromArgb(30, 42, 70);

        // ── Settings page controls ────────────────────────────────────────────
        private Panel    settingsPage;
        private Panel    settingsCard;          // repurposed as content panel
        private Panel    settingsHeaderPanel;
        private Panel    settingsDarkModeRow;
        private Panel    settingsToggleSwitch;
        private Label    settingsTitle;
        private Label    settingsSubLabel;
        private Label    settingsDmTitle;
        private Label    settingsDescLabel;
        private Label    themeDivider_unused;   // kept for compat; not shown
        private Panel    themeDivider;
        private Label    themeLabel;
        private CheckBox darkModeToggle;        // hidden state holder
        private Label    themeHintLabel;
        private TextBox  openAipKeyBox;
        private TextBox  ivaoApiKeyBox;
        private TextBox  mapboxTokenBox;
        private TextBox  aviationStackKeyBox;
        private Panel    aviationKeyWrapper;

        // ── Build settings page ───────────────────────────────────────────────
        internal void BuildSettingsPage()
        {
            const int StPadH = 36;

            this.settingsPage = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = LightBg,
                Visible   = false,
            };

            // ── Header ─────────────────────────────────────────────────────
            this.settingsHeaderPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 236,
                BackColor = LightCard,
                Padding   = new Padding(StPadH, 20, StPadH, 20),
            };

            this.settingsTitle = new Label
            {
                Text        = "Settings",
                Font        = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor   = Color.FromArgb(17, 24, 39),
                Dock        = DockStyle.Top,
                Height      = 52,
                AutoSize    = false,
                TextAlign   = ContentAlignment.MiddleLeft,
                UseMnemonic = false,
            };

            var stGapTitle = new Panel { Dock = DockStyle.Top, Height = 10, BackColor = Color.Transparent };

            this.settingsSubLabel = new Label
            {
                Text        = "Appearance & Preferences",
                Font        = new Font("Segoe UI", 10.5f),
                ForeColor   = Color.FromArgb(107, 114, 128),
                Dock        = DockStyle.Top,
                Height      = 26,
                AutoSize    = false,
                TextAlign   = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };

            var stGap1 = new Panel { Dock = DockStyle.Top, Height = 24, BackColor = Color.Transparent };

            this.settingsDescLabel = new Label
            {
                Text        = "Customise the look and feel of IVAO ATC Utilities. " +
                              "Changes apply instantly across the entire application " +
                              "without needing to restart.",
                Font        = new Font("Segoe UI", 10f),
                ForeColor   = Color.FromArgb(55, 65, 81),
                Dock        = DockStyle.Top,
                Height      = 84,
                AutoSize    = false,
                TextAlign   = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };

            settingsHeaderPanel.Controls.Add(this.settingsDescLabel);
            settingsHeaderPanel.Controls.Add(stGap1);
            settingsHeaderPanel.Controls.Add(this.settingsSubLabel);
            settingsHeaderPanel.Controls.Add(stGapTitle);
            settingsHeaderPanel.Controls.Add(this.settingsTitle);

            // ── Content area ───────────────────────────────────────────────
            var settingsContent = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = LightBg,
                Padding   = new Padding(StPadH, 24, StPadH, 24),
            };
            this.settingsCard = settingsContent; // alias for ApplyTheme

            // Section label
            this.themeLabel = new Label
            {
                Text      = "APPEARANCE",
                Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                Dock      = DockStyle.Top,
                Height    = 28,
                TextAlign = ContentAlignment.BottomLeft,
            };

            this.themeDivider = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 1,
                BackColor = LightPanelBorder,
            };

            var stGap2 = new Panel { Dock = DockStyle.Top, Height = 10, BackColor = Color.Transparent };

            // ── Dark mode row ──────────────────────────────────────────────
            this.settingsDarkModeRow = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 72,
                BackColor = LightCard,
            };
            this.settingsDarkModeRow.Paint += (s, e) =>
            {
                Color border = _darkMode ? DarkPanelBorder : LightPanelBorder;
                using var pen = new Pen(border, 1);
                e.Graphics.DrawRectangle(pen, 0, 0,
                    settingsDarkModeRow.Width - 1, settingsDarkModeRow.Height - 1);
            };

            var moonIcon = new Label
            {
                Text      = "🌙",
                Font      = new Font("Segoe UI Emoji", 20f),
                AutoSize  = false,
                Size      = new Size(44, 44),
                Location  = new Point(20, 14),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
            };

            this.settingsDmTitle = new Label
            {
                Text      = "Dark Mode",
                Font      = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = LightText,
                Location  = new Point(74, 14),
                AutoSize  = true,
                BackColor = Color.Transparent,
            };

            this.themeHintLabel = new Label
            {
                Text      = "Dark mode is coming soon in a future update.",
                Font      = new Font("Segoe UI", 8.5f),
                ForeColor = LightMuted,
                Location  = new Point(74, 38),
                AutoSize  = true,
                BackColor = Color.Transparent,
            };

            var comingSoonBadge = new Label
            {
                Text      = "Coming Soon",
                Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 120, 0),
                BackColor = Color.FromArgb(255, 243, 205),
                AutoSize  = true,
                Padding   = new Padding(6, 2, 6, 2),
                Cursor    = Cursors.Default,
            };
            this.settingsDarkModeRow.Controls.Add(comingSoonBadge);
            this.settingsDarkModeRow.Resize += (s, e) =>
            {
                comingSoonBadge.Location = new Point(settingsDarkModeRow.ClientSize.Width - comingSoonBadge.Width - 20, 26);
            };

            // ── Custom toggle switch (painted pill) ────────────────────────
            this.settingsToggleSwitch = new Panel
            {
                Size      = new Size(52, 28),
                Cursor    = Cursors.Hand,
                BackColor = Color.Transparent,
            };
            this.settingsToggleSwitch.Paint += (s, e) =>
            {
                var  g  = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                bool on = _darkMode;

                Color trackColor = on ? Color.FromArgb(37, 99, 235)
                                      : Color.FromArgb(203, 213, 225);
                using var trackBrush = new SolidBrush(trackColor);

                // Draw pill track
                g.FillEllipse(trackBrush, 0, 0, 28, 28);
                g.FillEllipse(trackBrush, 24, 0, 28, 28);
                g.FillRectangle(trackBrush, 14, 0, 24, 28);

                // Draw white thumb
                int tx = on ? 26 : 2;
                using var thumbBrush = new SolidBrush(Color.White);
                g.FillEllipse(thumbBrush, tx, 2, 24, 24);
            };
            // Toggle hidden - dark mode not yet implemented (badge shown instead)
            this.settingsToggleSwitch.Visible = false;

            this.settingsDarkModeRow.Controls.Add(moonIcon);
            this.settingsDarkModeRow.Controls.Add(this.settingsDmTitle);
            this.settingsDarkModeRow.Controls.Add(this.themeHintLabel);
            this.settingsDarkModeRow.Controls.Add(this.settingsToggleSwitch);

            // ── Helper: section label ─────────────────────────────────────────
            Label MakeStSecLbl(string text) => new Label
            {
                Text      = text,
                Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                Dock      = DockStyle.Top,
                Height    = 28,
                TextAlign = ContentAlignment.BottomLeft,
            };
            Panel MakeStDivider() => new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 1,
                BackColor = LightPanelBorder,
            };
            Panel MakeStGap(int h) => new Panel
            {
                Dock      = DockStyle.Top,
                Height    = h,
                BackColor = Color.Transparent,
            };

            // ── UPDATES section ───────────────────────────────────────────────
            var updateRow = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 90,
                BackColor = LightCard,
            };
            updateRow.Paint += (s, e) =>
            {
                using var pen = new Pen(LightPanelBorder, 1);
                e.Graphics.DrawRectangle(pen, 0, 0, updateRow.Width - 1, updateRow.Height - 1);
            };

            var verLabel = new Label
            {
                Text      = $"Current version:  {AppInfo.Version}",
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location  = new Point(20, 12),
                AutoSize  = true,
                BackColor = Color.Transparent,
            };

            var channelLabel = new Label
            {
                Text      = "Channel:",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(55, 65, 81),
                Location  = new Point(20, 48),
                AutoSize  = true,
                BackColor = Color.Transparent,
            };

            var channelCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 9f),
                Location      = new Point(90, 44),
                Width         = 160,
            };
            channelCombo.Items.AddRange(new object[] { " Stable", " Beta" });
            channelCombo.SelectedIndex = UpdateManager.Channel == "beta" ? 1 : 0;
            channelCombo.SelectedIndexChanged += async (s, e) =>
            {
                UpdateManager.Channel = channelCombo.SelectedIndex == 1 ? "beta" : "stable";
                _updateActionBtn.Enabled = false;
                _updateStatusLabel.Text  = "Checking…";
                _pendingUpdate = await UpdateManager.CheckAsync();
                RefreshUpdateUI();
                _updateActionBtn.Enabled = true;
            };

            _updateStatusLabel = new Label
            {
                Text      = "Checking for updates…",
                Font      = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 120, 150),
                AutoSize  = true,
                BackColor = Color.Transparent,
            };
            updateRow.Resize += (s, e) =>
                _updateStatusLabel.Location = new Point(
                    updateRow.ClientSize.Width - _updateStatusLabel.Width - 16, 14);

            _updateActionBtn = new Button
            {
                Text      = "Check Now",
                Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(13, 27, 75),
                FlatStyle = FlatStyle.Flat,
                Size      = new Size(100, 28),
                Cursor    = Cursors.Hand,
                Visible   = true,
            };
            _updateActionBtn.FlatAppearance.BorderSize = 0;
            updateRow.Resize += (s, e) =>
                _updateActionBtn.Location = new Point(
                    updateRow.ClientSize.Width - _updateActionBtn.Width - 16, 50);

            _updateActionBtn.Click += async (s, e) =>
            {
                _updateActionBtn.Enabled = false;
                _updateStatusLabel.Text  = "Checking…";
                _pendingUpdate = await UpdateManager.CheckAsync();
                RefreshUpdateUI();
                _updateActionBtn.Enabled = true;
            };

            updateRow.Controls.Add(verLabel);
            updateRow.Controls.Add(channelLabel);
            updateRow.Controls.Add(channelCombo);
            updateRow.Controls.Add(_updateStatusLabel);
            updateRow.Controls.Add(_updateActionBtn);

            // ── LANGUAGE section ──────────────────────────────────────────────
            var langRow = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 100,
                BackColor = LightCard,
            };
            langRow.Paint += (s, e) =>
            {
                using var pen = new Pen(LightPanelBorder, 1);
                e.Graphics.DrawRectangle(pen, 0, 0, langRow.Width - 1, langRow.Height - 1);
            };
            var langLabel = new Label
            {
                Text      = "Language:",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(55, 65, 81),
                Location  = new Point(20, 24),
                Width     = 90,
                AutoSize  = false,
                BackColor = Color.Transparent,
            };
            var langCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 9f),
                Location      = new Point(120, 20),
                Width         = 200,
            };
            var langs = Localization.Available();
            foreach (var (code, name) in langs)
                langCombo.Items.Add(new LangItem(code, name));
            langCombo.SelectedIndex = Math.Max(0, langs.FindIndex(l => l.Code == ConfigManager.Language));
            var langRestartBtn = new Button
            {
                Text      = "Restart Now",
                Font      = new Font("Segoe UI", 9f),
                Location  = new Point(120, 54),
                Width     = 120,
                Height    = 30,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
                Visible   = false,
            };
            langRestartBtn.FlatAppearance.BorderSize = 0;
            langRestartBtn.Click += (s, e) => Application.Restart();
            langCombo.SelectedIndexChanged += (s, e) =>
            {
                if (langCombo.SelectedItem is LangItem item)
                {
                    ConfigManager.Language  = item.Code;
                    langRestartBtn.Visible  = item.Code != Localization.CurrentCode;
                }
            };
            langRow.Controls.Add(langLabel);
            langRow.Controls.Add(langCombo);
            langRow.Controls.Add(langRestartBtn);

            // ── Stack sections (last added = topmost) ────────────────────────
            // Visual order top→bottom: APPEARANCE → LANGUAGE → UPDATES
            settingsContent.Controls.Add(updateRow);
            settingsContent.Controls.Add(MakeStGap(8));
            settingsContent.Controls.Add(MakeStDivider());
            settingsContent.Controls.Add(MakeStSecLbl("UPDATES"));
            settingsContent.Controls.Add(MakeStGap(8));
            settingsContent.Controls.Add(langRow);
            settingsContent.Controls.Add(MakeStGap(8));
            settingsContent.Controls.Add(MakeStDivider());
            settingsContent.Controls.Add(MakeStSecLbl("LANGUAGE"));
            settingsContent.Controls.Add(MakeStGap(16));

            settingsContent.Controls.Add(this.settingsDarkModeRow);
            settingsContent.Controls.Add(stGap2);
            settingsContent.Controls.Add(this.themeDivider);
            settingsContent.Controls.Add(this.themeLabel);

            // Hidden compat fields
            this.darkModeToggle       = new CheckBox { Visible = false };
            this.openAipKeyBox        = new TextBox  { Visible = false };
            this.ivaoApiKeyBox        = new TextBox  { Visible = false };
            this.mapboxTokenBox       = new TextBox  { Visible = false };
            this.aviationStackKeyBox  = new TextBox  { Visible = false };
            this.aviationKeyWrapper   = new Panel    { Visible = false };
            _openAipApiKey = ConfigManager.OpenAipApiKey;

            this.settingsPage.Controls.Add(settingsContent);
            this.settingsPage.Controls.Add(this.settingsHeaderPanel);

            // Background update check on startup
            _ = Task.Run(async () =>
            {
                _pendingUpdate = await UpdateManager.CheckAsync();
                this.BeginInvoke(RefreshUpdateUI);
            });
        }

        // ── Update UI refresh ─────────────────────────────────────────────────
        private void RefreshUpdateUI()
        {
            if (_updateStatusLabel == null || _updateActionBtn == null) return;
            if (_pendingUpdate == null)
            {
                _updateStatusLabel.Text      = $"✔  Up to date  ({AppInfo.Version})";
                _updateStatusLabel.ForeColor = Color.FromArgb(22, 163, 74);
                _updateActionBtn.Text        = "Check Now";
                _updateActionBtn.BackColor   = Color.FromArgb(13, 27, 75);
                _updateActionBtn.Click      -= InstallUpdate_Click;
            }
            else
            {
                string badge = _pendingUpdate.IsPreRelease ? "  [Beta]" : "";
                _updateStatusLabel.Text      = $"⬆  {_pendingUpdate.Version}{badge} available";
                _updateStatusLabel.ForeColor = Color.FromArgb(234, 88, 12);
                _updateActionBtn.Text        = "Install Update";
                _updateActionBtn.BackColor   = Color.FromArgb(234, 88, 12);
                _updateActionBtn.Click      -= InstallUpdate_Click;
                _updateActionBtn.Click      += InstallUpdate_Click;
            }
            _updateStatusLabel.Parent?.Invalidate();
        }

        private async void InstallUpdate_Click(object? sender, EventArgs e)
        {
            if (_pendingUpdate == null) return;
            _updateActionBtn.Enabled = false;
            _updateActionBtn.Text    = "Downloading…";

            var progress = new Progress<int>(p =>
                _updateActionBtn.Text = $"Downloading {p}%…");

            await UpdateManager.DownloadAndInstallAsync(_pendingUpdate, progress);
        }

        // ── Toggle handler (kept for compat) ──────────────────────────────────
        private void DarkModeToggle_Changed(object sender, EventArgs e)
        {
            _darkMode = darkModeToggle.Checked;
            settingsToggleSwitch?.Invalidate();
            ApplyTheme();
        }

        // ── Sidebar theme ─────────────────────────────────────────────────────
        private void ApplySidebarTheme(Color sidebarBg)
        {
            sidebarPanel.BackColor = sidebarBg;
            sidebarPanel.Invalidate();

            Color idleIcon, idleText, subText, sectionFg, dividerC, logoutFg, footerText, inputBg, inputFg;
            if (_darkMode)
            {
                idleIcon   = Color.FromArgb(148, 163, 184);
                idleText   = Color.FromArgb(203, 213, 225);
                subText    = Color.FromArgb(148, 163, 184);
                sectionFg  = Color.FromArgb(80,  100, 140);
                dividerC   = Color.FromArgb(30,   42,  70);
                logoutFg   = Color.FromArgb(248, 113, 113);
                footerText = Color.FromArgb(148, 163, 184);
                inputBg    = Color.FromArgb(14,   24,  56);
                inputFg    = Color.FromArgb(203, 213, 225);
            }
            else
            {
                idleIcon   = Color.FromArgb(100, 116, 139);
                idleText   = Color.FromArgb(30,   41,  59);
                subText    = Color.FromArgb(148, 163, 184);
                sectionFg  = Color.FromArgb(148, 163, 184);
                dividerC   = Color.FromArgb(226, 232, 240);
                logoutFg   = Color.FromArgb(220,  38,  38);
                footerText = Color.FromArgb(71,   85, 105);
                inputBg    = Color.FromArgb(248, 250, 252);
                inputFg    = Color.FromArgb(30,   41,  59);
            }

            Color idleBoxBg   = _darkMode ? Color.FromArgb(30, 42, 70)  : Color.FromArgb(241, 245, 249);
            Color logoutBoxBg = _darkMode ? Color.FromArgb(60, 20, 20)  : Color.FromArgb(254, 226, 226);
            Color logoutIcon  = _darkMode ? Color.FromArgb(248, 113, 113) : Color.FromArgb(220, 38, 38);

            foreach (Control child in sidebarPanel.Controls)
            {
                if (child is Panel navItem && child.Height >= 30)
                {
                    navItem.BackColor = Color.Transparent;
                    if (navItem == _activeSidebarItem) continue;

                    bool isLogout = (navItem == navLogoutBtn);
                    var inner = navItem.Controls.OfType<Panel>()
                                       .FirstOrDefault(p => p.Tag?.ToString() == "inner");
                    if (inner != null)
                    {
                        inner.BackColor = Color.Transparent;
                        var iconBox = inner.Controls.OfType<Panel>()
                                           .FirstOrDefault(p => p.Tag?.ToString() == "iconbox");
                        if (iconBox != null)
                        {
                            iconBox.BackColor = isLogout ? logoutBoxBg : idleBoxBg;
                            iconBox.Invalidate();
                            var iconLbl = iconBox.Controls.OfType<Label>().FirstOrDefault();
                            if (iconLbl != null)
                                iconLbl.ForeColor = isLogout ? logoutIcon : idleIcon;
                        }
                        foreach (Label lbl in inner.Controls.OfType<Label>())
                        {
                            string tag = lbl.Tag?.ToString() ?? "";
                            lbl.ForeColor = isLogout ? logoutFg
                                          : tag == "sub" ? subText
                                          : idleText;
                        }
                        inner.Invalidate();
                    }
                }
                else if (child is Panel divider && divider.Height == 1)
                    divider.BackColor = dividerC;
                else if (child is Label sectionLbl)
                {
                    sectionLbl.BackColor = Color.Transparent;
                    sectionLbl.ForeColor = sectionFg;
                }
                else if (child is Button btn)
                {
                    btn.BackColor = Color.Transparent;
                    btn.ForeColor = footerText;
                    btn.FlatAppearance.MouseOverBackColor = _darkMode
                        ? Color.FromArgb(30, 48, 90)
                        : Color.FromArgb(241, 245, 249);
                }
            }

            sidebarIcaoBox.BackColor = inputBg;
            sidebarIcaoBox.ForeColor = inputFg;
        }

        // ── Apply theme to all pages ──────────────────────────────────────────
        internal void ApplyTheme()
        {
            Color bg    = _darkMode ? DarkBg      : LightBg;
            Color card  = _darkMode ? DarkCard     : LightCard;
            Color sidebar = _darkMode ? DarkSidebar : LightSidebar;
            Color text  = _darkMode ? DarkText     : LightText;
            Color muted = _darkMode ? DarkMuted    : LightMuted;
            Color logBg = _darkMode ? DarkLogBg    : LightLogBg;
            Color logFg = _darkMode ? DarkLogFg    : LightLogFg;
            Color border = _darkMode ? DarkPanelBorder : LightPanelBorder;

            this.BackColor = bg;

            // Header
            Color hdrBg = _darkMode ? DarkHeader : LightHeader;
            headerPanel.BackColor                 = hdrBg;
            backButton.BackColor                  = hdrBg;
            backButton.FlatAppearance.BorderColor = Color.FromArgb(50, 80, 150);

            // Sidebar
            ApplySidebarTheme(sidebar);
            sidebarPanel.Invalidate();

            // Pages backgrounds
            loginPage.BackColor    = bg;
            loginCard.BackColor    = card;
            welcomePage.BackColor  = bg;
            sidStarPage.BackColor  = bg;
            countryPage.BackColor  = bg;

            // SID/STAR
            ssHeaderPanel.BackColor    = card;
            ssSearchPanel.BackColor    = card;
            ssOptionsPanel.BackColor   = _darkMode ? Color.FromArgb(20, 26, 44) : Color.FromArgb(248, 250, 255);
            ssOptionsLeft.BackColor    = Color.Transparent;
            ssOptionsRight.BackColor   = Color.Transparent;
            ssProgressPanel.BackColor  = card;
            ssDownloadPanel.BackColor  = card;
            ssTitleLabel.ForeColor     = text;
            ssAiracCycleLabel.ForeColor = _darkMode ? Color.FromArgb(100, 160, 255) : Color.FromArgb(13, 71, 161);
            ssAiracDaysLabel.ForeColor  = muted;
            ssStatusLabel.ForeColor     = muted;
            ssOptionsLeftLabel.ForeColor  = muted;
            ssOptionsRightLabel.ForeColor = muted;
            foreach (var chk in new[] { chkSidAlt, chkSidSpd, chkSidCoord, chkStarAlt, chkStarSpd, chkStarCoord })
            {
                chk.ForeColor = text;
                chk.BackColor = Color.Transparent;
            }
            ssLogBox.BackColor = logBg;
            ssLogBox.ForeColor = logFg;
            ssIcaoBox.BackColor   = _darkMode ? Color.FromArgb(22, 28, 48) : Color.White;
            ssIcaoBox.ForeColor   = text;
            ssIcaoWrapper.BackColor = _darkMode ? Color.FromArgb(22, 28, 48) : Color.White;
            ssIcaoWrapper.Invalidate();

            // Country
            cntHeaderPanel.BackColor    = card;
            cntSearchPanel.BackColor    = card;
            cntDropdownWrapper.BackColor  = _darkMode ? Color.FromArgb(22, 28, 48) : Color.White;
            cntDropdown.BackColor       = _darkMode ? Color.FromArgb(22, 28, 48) : Color.White;
            cntDropdown.ForeColor       = text;
            cntDropdownOverlay.BackColor  = _darkMode ? Color.FromArgb(22, 28, 48) : Color.White;
            cntDropdownListBox.BackColor  = _darkMode ? Color.FromArgb(22, 28, 48) : Color.White;
            cntDropdownListBox.ForeColor  = text;
            cntDownloadPanel.BackColor  = card;
            cntTitleLabel.ForeColor     = text;
            cntLogBox.BackColor         = logBg;
            cntLogBox.ForeColor         = logFg;
            cntProgressPanel.BackColor  = _darkMode ? Color.FromArgb(18, 24, 40) : Color.FromArgb(248, 250, 252);
            cntStatusLabel.ForeColor    = muted;

            // Welcome
            hubWelcomeLabel.ForeColor = text;
            hubHintLabel.ForeColor    = muted;
            hubAiracCard.BackColor    = card;
            hubAiracCycleLabel.ForeColor = _darkMode ? Color.FromArgb(100, 160, 255) : Color.FromArgb(13, 71, 161);
            hubAiracDatesLabel.ForeColor = muted;

            // Ground Layout
            groundPage.BackColor       = bg;
            grHeaderPanel.BackColor    = card;
            grImportPanel.BackColor    = card;
            grProgressPanel.BackColor  = card;
            grDownloadPanel.BackColor  = card;
            grLogBox.BackColor         = logBg;
            grLogBox.ForeColor         = logFg;
            grTitleLabel.ForeColor     = text;
            grStatusLabel.ForeColor    = muted;

            // OSM page
            osmPage.BackColor = bg;

            // Credits page
            creditsPage.BackColor         = bg;
            creditsHeaderPanel.BackColor  = Color.FromArgb(13, 27, 75); // always dark navy
            creditsBodyPanel.BackColor    = card;

            // Flight Schedules
            Color flInputBg = _darkMode ? Color.FromArgb(22, 28, 48) : Color.White;
            flightPage.BackColor      = bg;
            flHeaderPanel.BackColor   = card;
            flSearchPanel.BackColor   = card;
            flProgressPanel.BackColor = card;
            flLogBox.BackColor        = logBg;
            flLogBox.ForeColor        = logFg;
            if (flLogBox.Parent != null) flLogBox.Parent.BackColor = logBg;
            flTitleLabel.ForeColor    = text;
            flSubLabel.ForeColor      = muted;
            flStatusLabel.ForeColor   = muted;
            flDepWrapper.BackColor    = flInputBg;
            flArrWrapper.BackColor    = flInputBg;
            flAirlineWrapper.BackColor = flInputBg;
            flDepBox.BackColor        = flInputBg;
            flDepBox.ForeColor        = text;
            flArrBox.BackColor        = flInputBg;
            flArrBox.ForeColor        = text;
            flAirlineBox.BackColor    = flInputBg;
            flAirlineBox.ForeColor    = text;
            flStatusCombo.BackColor   = flInputBg;
            flStatusCombo.ForeColor   = text;
            flDepWrapper.Invalidate();
            flArrWrapper.Invalidate();
            flAirlineWrapper.Invalidate();

            // FIR Data
            firPage.BackColor          = bg;
            firHeaderPanel.BackColor   = card;
            firSearchPanel.BackColor   = card;
            firProgressPanel.BackColor = card;
            firDownloadPanel.BackColor = card;
            firLogBox.BackColor        = logBg;
            firLogBox.ForeColor        = logFg;
            firTitleLabel.ForeColor    = text;
            firStatusLabel.ForeColor   = muted;
            firSearchBox.BackColor     = _darkMode ? Color.FromArgb(22, 28, 48) : Color.White;
            firSearchBox.ForeColor     = text;

            // Airport
            airportPage.BackColor       = bg;
            apHeaderPanel.BackColor     = card;
            apSearchPanel.BackColor     = card;
            apProgressPanel.BackColor   = card;
            apDownloadPanel.BackColor   = card;
            apLogBox.BackColor          = logBg;
            apLogBox.ForeColor          = logFg;
            apTitleLabel.ForeColor      = text;
            apStatusLabel.ForeColor     = muted;
            apRegionBox.BackColor       = _darkMode ? Color.FromArgb(22, 28, 48) : Color.White;
            apRegionBox.ForeColor       = text;
            apRegionWrapper.BackColor   = _darkMode ? Color.FromArgb(22, 28, 48) : Color.White;
            apRegionWrapper.Invalidate();

            // Settings page
            settingsPage.BackColor        = bg;
            settingsHeaderPanel.BackColor = card;
            settingsTitle.ForeColor       = text;
            settingsSubLabel.ForeColor    = muted;
            settingsDescLabel.ForeColor   = _darkMode ? Color.FromArgb(160, 170, 190) : Color.FromArgb(55, 65, 81);
            settingsCard.BackColor        = bg;
            themeLabel.ForeColor          = muted;
            themeHintLabel.ForeColor      = muted;
            settingsDmTitle.ForeColor     = text;
            themeDivider.BackColor        = border;
            settingsDarkModeRow.BackColor = card;
            settingsDarkModeRow.Invalidate();
            settingsToggleSwitch?.Invalidate();

            this.Refresh();
        }
    }

    // Helper for language dropdown items
    internal sealed class LangItem
    {
        public string Code { get; }
        public string Name { get; }
        public LangItem(string code, string name) { Code = code; Name = name; }
        public override string ToString() => Name;
    }
}
