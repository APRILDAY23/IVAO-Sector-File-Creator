using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sector_File
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        // ── Header ───────────────────────────────────────────────────────────
        private Panel  headerPanel;
        private Label  headerAppIconLabel;    // avatar initial letter
        private Label  headerAppNameLabel;    // "IVAO" text
        private Label  headerAiracLabel;      // "AIRAC 2604"
        private Label  headerAiracDaysLabel;  // "23 days left"
        private Label  headerUserLabel;       // full name
        private Label  headerVidLabel;        // VID number
        private Panel  headerPipeSeparator;

        // ── Main body (sidebar + content) ────────────────────────────────────
        private Panel  mainBodyPanel;

        // ── Sidebar ──────────────────────────────────────────────────────────
        private Panel   sidebarPanel;
        private Panel   sidebarNavPanel;   // scrollable nav area inside sidebar
        private Panel   sidebarMiniPanel;  // kept as stub
        private TextBox sidebarIcaoBox;    // kept as stub (quick jump removed)
        // Nav items
        private Panel   navDashboardCard;
        private Panel   navSidStarCard;
        private Panel   navAirportCard;
        private Panel   navFirCard;
        private Panel   navCountryCard;
        private Panel   navKmlCard;
        private Panel   navFlightCard;
        // Footer nav items
        private Panel   navCreditsBtn;
        private Panel   navSettingsBtn;
        private Panel   navLogoutBtn;
        // Legacy button aliases (kept so click-handler code still compiles)
        private Panel   sidebarDivider1;
        private Panel   sidebarDivider2;
        private Panel   sidebarDivider3;
        private Label   sidebarSectionAtc;
        private Label   sidebarSectionFlight;
        private Button  navSidStarBtn;
        private Button  navAirportBtn;
        private Button  navFirBtn;
        private Button  navCountryBtn;
        private Button  navKmlBtn;
        private Button  navFlightBtn;

        // ── Content panel ─────────────────────────────────────────────────────
        private Panel  contentPanel;

        // ── Page: Login ───────────────────────────────────────────────────────
        private Panel  loginPage;
        private Panel  loginCard;       // white right panel (for ApplyTheme)
        private Panel  loginLeftPanel;  // dark gradient left panel
        private System.Windows.Forms.Timer _loginRadarTimer;
        private float  _loginRadarAngle = 0f;
        private PictureBox loginIconLabel;
        private Label  loginTitleLabel;
        private Label  loginDescLabel;
        private Label  loginAiracLabel;
        private Button loginButton;
        private Label  loginNoteLabel;

        // (atcGridPage and flightGridPage replaced by sidebar navigation)

        // ── Page: Welcome (home after login) ─────────────────────────────────
        private Panel  welcomePage;
        private Label  hubWelcomeLabel;
        private Label  hubHintLabel;
        private Panel  hubAiracCard;
        private Label  hubAiracCycleLabel;
        private Label  hubAiracDatesLabel;

        // ── Page: SID & STAR ─────────────────────────────────────────────────
        private Panel     sidStarPage;
        private Panel     ssHeaderPanel;
        private Label     ssTitleLabel;
        private Label     ssAiracCycleLabel;
        private Label     ssAiracDaysLabel;
        private Panel     ssSearchPanel;
        private Panel     ssIcaoWrapper;
        private TextBox   ssIcaoBox;
        private Button    ssSearchButton;
        private Panel     ssOptionsPanel;
        private Panel     ssOptionsLeft;
        private Label     ssOptionsLeftLabel;
        private CheckBox  chkSidAlt;
        private CheckBox  chkSidSpd;
        private CheckBox  chkSidCoord;
        private Panel     ssOptionsRight;
        private Label     ssOptionsRightLabel;
        private CheckBox  chkStarAlt;
        private CheckBox  chkStarSpd;
        private CheckBox  chkStarCoord;
        private Panel     ssProgressPanel;
        private ProgressBar ssProgressBar;
        private Label     ssStatusLabel;
        private RichTextBox ssLogBox;
        private Panel     ssDownloadPanel;
        private Button    downloadSidButton;
        private Button    downloadStarButton;

        // ── Page: Airport Data ───────────────────────────────────────────────
        private Panel       airportPage;
        private Panel       apHeaderPanel;
        private Label       apTitleLabel;
        private Label       apAiracLabel;
        private Panel       apSearchPanel;
        private Panel       apRegionWrapper;
        private TextBox     apRegionBox;
        private Button      apSearchButton;
        private Panel       apProgressPanel;
        private ProgressBar apProgressBar;
        private Label       apStatusLabel;
        private RichTextBox apLogBox;
        private Panel       apDownloadPanel;
        private Button      apDownloadAirportBtn;
        private Button      apDownloadRunwayBtn;
        private Button      apDownloadFreqBtn;
        private Button      apDownloadVorBtn;
        private Button      apDownloadNdbBtn;

        // ── Page: Country Data ───────────────────────────────────────────────
        private Panel       countryPage;
        private Panel       cntHeaderPanel;
        private Label       cntTitleLabel;
        private Panel       cntSearchPanel;
        private Panel       cntDropdownWrapper;
        private TextBox     cntDropdown;
        private Panel       cntDropdownOverlay;    // floating list
        private ListBox     cntDropdownListBox;
        private Button      cntSearchButton;
        private Panel       cntProgressPanel;
        private ProgressBar cntProgressBar;
        private Label       cntStatusLabel;
        private Panel       cntBodyPanel;
        private RichTextBox cntLogBox;
        private Panel       cntDownloadPanel;
        private Button      cntDownloadButton;

        // ── Page: FIR Data (embedded) ────────────────────────────────────────
        private Panel       firPage;
        private Panel       firHeaderPanel;
        private Label       firTitleLabel;
        private Label       firAiracCycleLabel;
        private Label       firAiracDaysLabel;
        private Panel       firSearchPanel;
        private Panel       firSearchWrapper;
        private TextBox     firSearchBox;
        private Button      firSearchButton;
        private Panel       firOptionsPanel;
        private CheckBox    chkFirBoundary;
        private CheckBox    chkFirUir;
        private CheckBox    chkFirTma;
        private CheckBox    chkFirCtr;
        private CheckBox    chkFirCta;
        private CheckBox    chkFirRestricted;
        private CheckBox    chkFirDanger;
        private CheckBox    chkFirProhibited;
        private Panel       firProgressPanel;
        private ProgressBar firProgressBar;
        private Label       firStatusLabel;
        private RichTextBox firLogBox;
        private Panel       firDownloadPanel;
        private RadioButton firRadioSeparate;
        private RadioButton firRadioCombined;
        private Panel       firSeparatePanel;
        private Panel       firCombinedPanel;
        private Button      firDlFirBtn;
        private Button      firDlUirBtn;
        private Button      firDlTmaBtn;
        private Button      firDlCtrBtn;
        private Button      firDlCtaBtn;
        private Button      firDlRestrictedBtn;
        private Button      firDlDangerBtn;
        private Button      firDlProhibitedBtn;
        private Button      firDlCombinedBtn;

        // ── Page: Ground Layout (embedded KML) ──────────────────────────────
        private Panel       groundPage;
        private Panel       grHeaderPanel;
        private Label       grTitleLabel;
        private Panel       grImportPanel;
        private Button      grImportButton;
        private Label       grFileLabel;
        private Panel       grProgressPanel;
        private ProgressBar grProgressBar;
        private Label       grStatusLabel;
        private RichTextBox grLogBox;
        private Panel       grDownloadPanel;
        private Button      grDownloadTflBtn;
        private Button      grDownloadGeoBtn;
        private Button      grDownloadTxiBtn;
        private Button      grDownloadRwBtn;

        // ── Sidebar: OSM nav item ────────────────────────────────────────────
        private Panel navOsmCard;

        // ── Page: OSM Data (Coming Soon) ─────────────────────────────────────
        private Panel osmPage;

        // ── Page: Credits ────────────────────────────────────────────────────
        private Panel       creditsPage;
        private Panel       creditsHeaderPanel;
        private Panel       creditsBodyPanel;

        // ── Page: Flight Schedules ────────────────────────────────────────────
        private Panel          flightPage;
        private Panel          flHeaderPanel;
        private Label          flTitleLabel;
        private Label          flSubLabel;
        private Panel          flSearchPanel;
        private Panel          flDepWrapper;
        private TextBox        flDepBox;
        private Panel          flArrWrapper;
        private TextBox        flArrBox;
        private Panel          flAirlineWrapper;
        private TextBox        flAirlineBox;
        private DateTimePicker flDatePicker;
        private DateTimePicker flFromTimePicker;
        private DateTimePicker flToTimePicker;
        private ComboBox       flStatusCombo;
        private Button         flSearchButton;
        private Panel          flProgressPanel;
        private ProgressBar    flProgressBar;
        private Label          flStatusLabel;
        private RichTextBox    flLogBox;
        private Panel          flDownloadPanel;
        private Button         flDownloadExcelBtn;

        // ── ATC/Flight card buttons (for popup tools) ─────────────────────────
        private Panel  airportCardPanel;
        private Button airportCardBtn;
        private Panel  firCardPanel;
        private Button firCardBtn;
        private Panel  sidStarCard;
        private Panel  sidStarCardPanel;
        private Panel  countryCardPanel;
        private Panel  kmlCardPanel;
        private Button kmlCardBtn;
        private Panel  flightSchedPanel;
        private Button flightSchedBtn;

        // ── Back button in header ─────────────────────────────────────────────
        private Button backButton;
        private Label  breadcrumbLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            _loginRadarTimer?.Stop();
            _loginRadarTimer?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            BuildHeader();
            BuildSidebar();
            BuildContentPages();
            BuildMainLayout();

            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize    = new Size(1100, 720);
            this.MinimumSize   = new Size(960, 640);
            this.MaximizeBox   = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text          = AppInfo.Name;
            this.BackColor     = Color.FromArgb(248, 249, 252);

            this.ResumeLayout(false);
        }

        // ═══════════════════════════════════════════════════════════════════
        //  HEADER  - 72 px, split brand-zone / content-zone layout
        //  Brand zone (0 → sidebarW) matches sidebar background exactly so
        //  they form one seamless dark chrome column visually.
        // ═══════════════════════════════════════════════════════════════════
        private const int SidebarW = 300;
        private const int HeaderH  = 100;

        private void BuildHeader()
        {
            var hdrBg = Color.FromArgb(33, 50, 147);

            this.headerPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = HeaderH,
                BackColor = hdrBg,
            };

            // ── IVAO white logo - full height, proportional width ─────────
            const int LogoH   = HeaderH;      // stretch to full header height
            const int LogoW   = 160;          // wide enough to show full logo
            const int LogoX   = 16;
            var logoCircle = new PictureBox
            {
                BackColor = hdrBg,
                Location  = new Point(LogoX, 0),
                Size      = new Size(LogoW, LogoH),
                SizeMode  = PictureBoxSizeMode.Zoom,
            };
            try { logoCircle.Image = System.Drawing.Image.FromFile("./ivao_white_logo.png"); } catch { }

            // "IVAO" text label kept as hidden stub (field must remain instantiated)
            this.headerAppNameLabel = new Label { Visible = false };

            // Thin pipe after logo
            const int LogoSepX = LogoX + LogoW + 10;
            var logoSep = new Panel
            {
                BackColor = Color.FromArgb(100, 120, 200),
                Location  = new Point(LogoSepX, HeaderH / 4),
                Size      = new Size(1, HeaderH / 2),
            };

            // "Sector File Creator" subtitle
            var headerSubLabel = new Label
            {
                Text        = "Sector File Creator",
                ForeColor   = Color.FromArgb(200, 215, 255),
                Font        = new Font("Segoe UI", 11f),
                TextAlign   = ContentAlignment.MiddleLeft,
                Location    = new Point(LogoSepX + 12, 0),
                Size        = new Size(220, HeaderH),
                AutoSize    = false,
                UseMnemonic = false,
            };

            // Pipe at sidebar boundary (kept as field for sidebar resize tracking)
            this.headerPipeSeparator = new Panel
            {
                BackColor = Color.FromArgb(50, 70, 140),
                Location  = new Point(SidebarW - 1, HeaderH / 4),
                Size      = new Size(1, HeaderH / 2),
            };

            // Breadcrumb kept as orphan label (UpdateNav still references it)
            this.breadcrumbLabel = new Label { Visible = false };

            // Back button (no breadcrumb visible, but back still works)
            this.backButton = new Button
            {
                Text      = "← Back",
                Font      = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(180, 210, 255),
                BackColor = hdrBg,
                FlatStyle = FlatStyle.Flat,
                Size      = new Size(86, 32),
                Visible   = false,
                Cursor    = Cursors.Hand,
            };
            this.backButton.FlatAppearance.BorderColor = Color.FromArgb(50, 80, 150);
            this.backButton.FlatAppearance.BorderSize  = 1;
            this.backButton.Click += BackButton_Click;

            // ── Right section: AIRAC ──────────────────────────────────────
            // Two stacked labels, right-aligned
            // Line 1: "AIRAC 2604"  Line 2: "23 days left"
            const int LineH1 = 24;
            const int LineH2 = 20;
            const int BlockH = LineH1 + 2 + LineH2;   // 46
            const int BlockY = (HeaderH - BlockH) / 2; // 27

            this.headerAiracLabel = new Label
            {
                Text      = "",
                ForeColor = Color.FromArgb(74, 222, 128),
                Font      = new Font("Segoe UI", 11f, FontStyle.Bold),
                TextAlign = ContentAlignment.BottomRight,
                Location  = new Point(800, BlockY),
                Size      = new Size(175, LineH1),
                AutoSize  = false,
            };

            this.headerAiracDaysLabel = new Label
            {
                Text      = "",
                ForeColor = Color.FromArgb(74, 222, 128),
                Font      = new Font("Segoe UI", 9f),
                TextAlign = ContentAlignment.TopRight,
                Location  = new Point(800, BlockY + LineH1 + 2),
                Size      = new Size(175, LineH2),
                AutoSize  = false,
            };

            // Separator between AIRAC and user section
            var rightSep = new Panel
            {
                BackColor = Color.FromArgb(50, 70, 140),
                Location  = new Point(940, HeaderH / 4),
                Size      = new Size(1, HeaderH / 2),
            };

            // ── Right section: User avatar ────────────────────────────────
            const int AvSize = 42;
            var avatarPanel = new Panel
            {
                BackColor = Color.FromArgb(37, 99, 235),
                Location  = new Point(960, (HeaderH - AvSize) / 2),
                Size      = new Size(AvSize, AvSize),
            };
            avatarPanel.Region = new Region(RoundedRect(
                new Rectangle(0, 0, AvSize, AvSize), AvSize / 2));
            this.headerAppIconLabel = new Label
            {
                Text        = "",
                Font        = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor   = Color.White,
                TextAlign   = ContentAlignment.MiddleCenter,
                Dock        = DockStyle.Fill,
                UseMnemonic = false,
            };
            avatarPanel.Controls.Add(this.headerAppIconLabel);

            // Name (line 1) + VID (line 2) stacked
            this.headerUserLabel = new Label
            {
                Text      = "",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                TextAlign = ContentAlignment.BottomLeft,
                Location  = new Point(1012, BlockY),
                Size      = new Size(170, LineH1),
                AutoSize  = false,
            };

            this.headerVidLabel = new Label
            {
                Text      = "",
                ForeColor = Color.FromArgb(147, 180, 245),
                Font      = new Font("Segoe UI", 9f),
                TextAlign = ContentAlignment.TopLeft,
                Location  = new Point(1012, BlockY + LineH1 + 2),
                Size      = new Size(170, LineH2),
                AutoSize  = false,
            };

            // Bottom border
            var headerBorder = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 1,
                BackColor = Color.FromArgb(15, 30, 75),
            };

            this.headerPanel.Controls.Add(logoCircle);
            this.headerPanel.Controls.Add(logoSep);
            this.headerPanel.Controls.Add(headerSubLabel);
            this.headerPanel.Controls.Add(this.headerPipeSeparator);
            this.headerPanel.Controls.Add(this.headerAiracLabel);
            this.headerPanel.Controls.Add(this.headerAiracDaysLabel);
            this.headerPanel.Controls.Add(rightSep);
            this.headerPanel.Controls.Add(avatarPanel);
            this.headerPanel.Controls.Add(this.headerUserLabel);
            this.headerPanel.Controls.Add(this.headerVidLabel);
            this.headerPanel.Controls.Add(headerBorder);

            // ── Responsive reposition ─────────────────────────────────────
            this.headerPanel.Resize += (s, e) =>
            {
                int w     = headerPanel.ClientSize.Width;
                int right = w - 20;

                // User labels (name + VID), fixed width from right edge
                const int UserW = 170;
                int nameX = right - UserW;
                headerUserLabel.Location = new Point(nameX, BlockY);
                headerUserLabel.Width    = UserW;
                headerVidLabel.Location  = new Point(nameX, BlockY + LineH1 + 2);
                headerVidLabel.Width     = UserW;

                // Avatar: left of name labels
                int avLeft = nameX - AvSize - 10;
                avatarPanel.Location = new Point(avLeft, (HeaderH - AvSize) / 2);

                // Separator: left of avatar
                int rSepX = avLeft - 16;
                rightSep.Location = new Point(rSepX, HeaderH / 4);

                // AIRAC labels: left of separator
                const int AiracW = 175;
                int airacX = rSepX - AiracW - 12;
                headerAiracLabel.Location     = new Point(airacX, BlockY);
                headerAiracLabel.Width        = AiracW;
                headerAiracDaysLabel.Location = new Point(airacX, BlockY + LineH1 + 2);
                headerAiracDaysLabel.Width    = AiracW;

                // Pipe separator tracks sidebar right edge
                int sbRight = sidebarPanel.Visible ? sidebarPanel.Width : 0;
                headerPipeSeparator.Left = sbRight - 1;
            };
        }

        // ================================================================
        //  SIDEBAR  - collapsible nav panel
        // ================================================================
        private void BuildSidebar()
        {
            this.sidebarIcaoBox  = new TextBox();  // stub (quick jump removed)
            this.sidebarMiniPanel = new Panel();   // stub (no longer used)

            this.sidebarPanel = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = SidebarW,
                BackColor = Color.White,
                Visible   = false,
            };

            // Right border
            this.sidebarPanel.Paint += (s, e) =>
            {
                var bc = _darkMode ? Color.FromArgb(30, 42, 70) : Color.FromArgb(226, 232, 240);
                using var pen = new Pen(bc, 1);
                e.Graphics.DrawLine(pen, sidebarPanel.Width - 1, 0,
                                         sidebarPanel.Width - 1, sidebarPanel.Height);
            };

            // Track pipe separator when sidebar resizes
            sidebarPanel.Resize += (s, e) =>
            {
                if (headerPipeSeparator != null)
                    headerPipeSeparator.Left = sidebarPanel.Width - 1;
            };

            // ── Nav items ─────────────────────────────────────────────────
            this.navDashboardCard = MakeSidebarNavItem("🏠", "Dashboard",        "Home");
            this.navSidStarCard   = MakeSidebarNavItem("✈",  "SID & STAR",       "Generate procedures");
            this.navAirportCard   = MakeSidebarNavItem("🏗", "Airport Data",     "Runways, freq & navaids");
            this.navFirCard       = MakeSidebarNavItem("📡", "FIR Data",         "FIR boundaries & ATC");
            this.navCountryCard   = MakeSidebarNavItem("🌍", "Country Data",     "National boundary data");
            this.navKmlCard       = MakeSidebarNavItem("🗺", "Ground Layout",    "Import KML ground files");
            this.navOsmCard       = MakeSidebarNavItem("🗂", "OSM Data",         "Coming soon");
            this.navFlightCard    = MakeSidebarNavItem("📋", "Flight Schedules", "Search ADS-B flight data");

            var atcSectionLabel    = MakeSidebarSectionLabel("ATC");
            var flightSectionLabel = MakeSidebarSectionLabel("Flight");
            var miscSectionLabel   = MakeSidebarSectionLabel("MISC");
            var sectionDivider     = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Color.FromArgb(226, 232, 240) };
            var flightMiscDivider  = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Color.FromArgb(226, 232, 240) };

            WireSidebarItem(this.navDashboardCard, () => { NavigateTo(welcomePage, "Home");            SetActiveSidebarItem(navDashboardCard); });
            WireSidebarItem(this.navSidStarCard,   () => { NavigateTo(sidStarPage, "SID & STAR");      SetActiveSidebarItem(navSidStarCard); });
            WireSidebarItem(this.navAirportCard,   () => { NavigateTo(airportPage, "Airport Data");    SetActiveSidebarItem(navAirportCard); });
            WireSidebarItem(this.navFirCard,       () => { NavigateTo(firPage,      "FIR Data");        SetActiveSidebarItem(navFirCard); });
            WireSidebarItem(this.navCountryCard,   () => { NavigateTo(countryPage, "Country Data");    SetActiveSidebarItem(navCountryCard); });
            WireSidebarItem(this.navKmlCard,       () => { NavigateTo(groundPage,  "Ground Layout");   SetActiveSidebarItem(navKmlCard); });
            WireSidebarItem(this.navOsmCard,       () => { NavigateTo(osmPage,     "OSM Data");        SetActiveSidebarItem(navOsmCard); });
            WireSidebarItem(this.navFlightCard,    () => { NavigateTo(flightPage,  "Flight Schedules"); SetActiveSidebarItem(navFlightCard); });

            // ── MISC nav items (inside scrollable nav) ────────────────────
            this.navSettingsBtn = MakeSidebarNavItem("⚙",  "Settings", "App preferences");
            this.navCreditsBtn  = MakeSidebarNavItem("ℹ",  "Credits",  "About this tool");
            this.navLogoutBtn   = MakeSidebarNavItem("⏻",  "Sign Out", "");
            // Tint the logout icon red
            var logoutInner = this.navLogoutBtn.Controls.OfType<Panel>().FirstOrDefault(p => p.Tag?.ToString() == "inner");
            if (logoutInner != null)
            {
                var logoutIconBox = logoutInner.Controls.OfType<Panel>().FirstOrDefault(p => p.Tag?.ToString() == "iconbox");
                if (logoutIconBox != null)
                {
                    logoutIconBox.BackColor = Color.FromArgb(254, 226, 226); // red-100
                    var logoutIconLbl = logoutIconBox.Controls.OfType<Label>().FirstOrDefault();
                    if (logoutIconLbl != null) logoutIconLbl.ForeColor = Color.FromArgb(220, 38, 38);
                }
                var logoutTitle = logoutInner.Controls.OfType<Label>()
                                              .FirstOrDefault(l => l.Tag?.ToString() == "title");
                if (logoutTitle != null) logoutTitle.ForeColor = Color.FromArgb(220, 38, 38);
            }

            WireSidebarItem(this.navCreditsBtn, () => { NavigateTo(creditsPage, "Credits"); SetActiveSidebarItem(navCreditsBtn); });
            // navSettingsBtn wired in MainForm_Load after BuildSettingsPage()
            WireSidebarItem(this.navLogoutBtn, () =>
            {
                // Reset collapse to expanded
                _sidebarCollapsed  = false;
                sidebarPanel.Width = SidebarW;
                sidebarPanel.Visible = false;
                headerUserLabel.Text = "";
                ShowOnly(loginPage);
                _navStack.Clear();
                _navStack.Push((loginPage, "Login"));
                UpdateNav();
                loginButton.Enabled = true;
                loginButton.Text    = "Continue with IVAO SSO";
            });

            // ── Collapse / expand button ───────────────────────────────────
            var collapseDivider = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 1,
                BackColor = Color.FromArgb(226, 232, 240),
            };
            var collapseBtn = new Button
            {
                Text      = "‹   Close sidebar",
                Font      = new Font("Segoe UI", 13f),
                ForeColor = Color.FromArgb(100, 116, 139),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock      = DockStyle.Bottom,
                Height    = 88,
                Cursor    = Cursors.Hand,
            };
            collapseBtn.FlatAppearance.BorderSize         = 0;
            collapseBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 245, 249);
            collapseBtn.Click += (s, e) =>
            {
                if (!_sidebarCollapsed)
                {
                    // Collapse to icon-only (82px ≈ 20% wider than before)
                    _sidebarCollapsed  = true;
                    sidebarPanel.Width = 82;
                    collapseBtn.Text   = "›";
                }
                else
                {
                    // Expand back
                    _sidebarCollapsed  = false;
                    sidebarPanel.Width = (this.WindowState == FormWindowState.Maximized)
                        ? SidebarMaximized : SidebarNormal;
                    collapseBtn.Text = "‹   Close sidebar";
                }
            };

            // ── Legacy stubs ───────────────────────────────────────────────
            this.sidebarDivider1      = new Panel();
            this.sidebarDivider2      = new Panel();
            this.sidebarDivider3      = new Panel();
            this.sidebarSectionAtc    = new Label();
            this.sidebarSectionFlight = new Label();
            this.navSidStarBtn        = new Button();
            this.navAirportBtn        = new Button();
            this.navFirBtn            = new Button();
            this.navCountryBtn        = new Button();
            this.navKmlBtn            = new Button();
            this.navFlightBtn         = new Button();

            // ── Scrollable nav area ────────────────────────────────────────
            this.sidebarNavPanel = new Panel
            {
                Dock       = DockStyle.Fill,
                AutoScroll = true,
                BackColor  = Color.Transparent,
            };

            // Items placed directly in sidebarNavPanel with manual Y positions
            // (no DockStyle so AutoScroll can measure them) and AutoScrollMinSize
            // is set explicitly - the only 100 % reliable way to fix the scroll range.
            var navOrder = new Control[]
            {
                navDashboardCard,
                atcSectionLabel,
                navSidStarCard, navAirportCard, navFirCard, navCountryCard, navKmlCard,
                sectionDivider,
                navOsmCard,
                flightSectionLabel,
                navFlightCard,
                flightMiscDivider,
                miscSectionLabel,
                navSettingsBtn, navCreditsBtn, navLogoutBtn,
            };

            int yOff = 0;
            foreach (var ctrl in navOrder)
            {
                ctrl.Dock     = DockStyle.None;
                ctrl.Location = new Point(0, yOff);
                ctrl.Width    = SidebarW;
                this.sidebarNavPanel.Controls.Add(ctrl);
                yOff += ctrl.Height;
            }
            // Force WinForms to allow scrolling the full virtual height
            this.sidebarNavPanel.AutoScrollMinSize = new Size(0, yOff);

            // Keep every item's width in sync when the sidebar resizes
            this.sidebarNavPanel.Resize += (s2, e2) =>
            {
                int w = this.sidebarNavPanel.ClientSize.Width;
                foreach (Control c in this.sidebarNavPanel.Controls)
                    c.Width = w;
            };

            // ── Assemble sidebarPanel ──────────────────────────────────────
            // DockStyle.Bottom: first added = TOPMOST in footer, last added = VERY BOTTOM
            this.sidebarPanel.Controls.Add(collapseDivider);
            this.sidebarPanel.Controls.Add(collapseBtn);
            // Fill panel goes last - takes all remaining space between top and footer
            this.sidebarPanel.Controls.Add(this.sidebarNavPanel);
        }

        // ── Nav item  (rounded icon box + title + optional subtitle) ─────────
        private Panel MakeSidebarNavItem(string icon, string title, string subtitle = "")
        {
            bool hasSub  = !string.IsNullOrEmpty(subtitle);
            const int BoxSize = 44;
            int  itemH   = hasSub ? 86 : 64;

            var item = new Panel
            {
                BackColor = Color.Transparent,
                Dock      = DockStyle.Top,
                Height    = itemH,
                Cursor    = Cursors.Hand,
            };

            var inner = new Panel
            {
                Tag       = "inner",
                BackColor = Color.Transparent,
                Location  = new Point(12, 0),
                Size      = new Size(SidebarW - 24, itemH),
                Cursor    = Cursors.Hand,
                Anchor    = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
            };

            var iconBox = new Panel
            {
                Tag       = "iconbox",
                BackColor = Color.FromArgb(241, 245, 249),
                Size      = new Size(BoxSize, BoxSize),
                Location  = new Point(0, (itemH - BoxSize) / 2),
                Cursor    = Cursors.Hand,
            };
            iconBox.Region = new System.Drawing.Region(RoundedRect(new Rectangle(0, 0, BoxSize, BoxSize), 9));

            var iconLbl = new Label
            {
                Tag         = "icon",
                Text        = icon,
                Font        = new Font("Segoe UI Emoji", 11f),
                ForeColor   = Color.FromArgb(100, 116, 139),
                BackColor   = Color.Transparent,
                TextAlign   = ContentAlignment.MiddleCenter,
                AutoSize    = false,
                Size        = new Size(BoxSize, BoxSize),
                Location    = new Point(0, 0),
                Cursor      = Cursors.Hand,
                UseMnemonic = false,
            };
            iconBox.Controls.Add(iconLbl);

            int textX  = BoxSize + 14;   // 58 px
            // Title 40px + 4 gap + sub 30px = 74px block, centred in item
            int titleH = 40;
            int subH   = 30;
            int blockH = hasSub ? (titleH + 4 + subH) : titleH;
            int blockY = (itemH - blockH) / 2;

            var titleLbl = new Label
            {
                Tag         = "title",
                Text        = title,
                Font        = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor   = Color.FromArgb(30, 41, 59),
                BackColor   = Color.Transparent,
                TextAlign   = ContentAlignment.MiddleLeft,
                AutoSize    = false,
                Size        = new Size(SidebarW - 24 - textX, titleH),
                Location    = new Point(textX, blockY),
                Cursor      = Cursors.Hand,
                UseMnemonic = false,
            };

            inner.Controls.Add(iconBox);
            inner.Controls.Add(titleLbl);

            if (hasSub)
            {
                var subLbl = new Label
                {
                    Tag         = "sub",
                    Text        = subtitle,
                    Font        = new Font("Segoe UI", 9.5f),
                    ForeColor   = Color.FromArgb(148, 163, 184),
                    BackColor   = Color.Transparent,
                    TextAlign   = ContentAlignment.TopLeft,
                    AutoSize    = false,
                    Size        = new Size(SidebarW - 24 - textX, subH),
                    Location    = new Point(textX, blockY + titleH + 4),
                    Cursor      = Cursors.Hand,
                    UseMnemonic = false,
                };
                inner.Controls.Add(subLbl);
            }

            item.Controls.Add(inner);

            item.Resize += (s, e) =>
            {
                inner.Width = item.ClientSize.Width - 24;
                int lw = inner.Width - textX;
                // In collapsed mode (no room for labels) centre the icon box
                iconBox.Left = (lw <= 0)
                    ? Math.Max(0, (inner.Width - BoxSize) / 2)
                    : 0;
                foreach (Control c in inner.Controls)
                {
                    string t = c.Tag?.ToString() ?? "";
                    if (t == "title" || t == "sub") c.Width = Math.Max(0, lw);
                }
            };

            // Hover highlight - light gray background
            void SetHover(bool on)
            {
                if (_activeSidebarItem == item) return;   // don't override active state
                item.BackColor = on
                    ? (_darkMode ? Color.FromArgb(25, 35, 60) : Color.FromArgb(241, 245, 249))
                    : Color.Transparent;
            }
            void WireHover(Control c)
            {
                c.MouseEnter += (s, e) => SetHover(true);
                c.MouseLeave += (s, e) => SetHover(false);
                foreach (Control child in c.Controls) WireHover(child);
            }
            WireHover(item);

            return item;
        }

        private static void WireSidebarItem(Panel item, Action onClick)
        {
            void WireAll(Control c)
            {
                c.Click += (s, e) => onClick();
                foreach (Control child in c.Controls)
                    WireAll(child);
            }
            WireAll(item);
        }

        private Control _activeSidebarItem;

        private void SetActiveSidebarItem(Control item)
        {
            if (_activeSidebarItem is Panel prev)
                NavItemSetActive(prev, false);
            if (item is Panel cur)
                NavItemSetActive(cur, true);
            _activeSidebarItem = item;
        }

        private void NavItemSetActive(Panel item, bool active)
        {
            var inner   = item.Controls.OfType<Panel>().FirstOrDefault(p => p.Tag?.ToString() == "inner");
            if (inner == null) return;
            var iconBox = inner.Controls.OfType<Panel>().FirstOrDefault(p => p.Tag?.ToString() == "iconbox");
            if (iconBox == null) return;
            var iconLbl = iconBox.Controls.OfType<Label>().FirstOrDefault();

            if (active)
            {
                item.BackColor    = _darkMode ? Color.FromArgb(25, 35, 60) : Color.FromArgb(241, 245, 249);
                iconBox.BackColor = Color.FromArgb(27, 43, 90);  // IVAO navy fill
                if (iconLbl != null) iconLbl.ForeColor = Color.White;
            }
            else
            {
                item.BackColor    = Color.Transparent;
                iconBox.BackColor = _darkMode
                    ? Color.FromArgb(30, 42, 70)
                    : Color.FromArgb(241, 245, 249);  // slate-100
                if (iconLbl != null)
                    iconLbl.ForeColor = _darkMode
                        ? Color.FromArgb(148, 163, 184)
                        : Color.FromArgb(100, 116, 139);
            }
            iconBox.Invalidate();

            foreach (Label lbl in inner.Controls.OfType<Label>())
            {
                string tag = lbl.Tag?.ToString() ?? "";
                if (tag == "title")
                    lbl.ForeColor = active
                        ? Color.FromArgb(37, 99, 235)
                        : (_darkMode ? Color.FromArgb(203, 213, 225) : Color.FromArgb(30, 41, 59));
                else if (tag == "sub")
                    lbl.ForeColor = Color.FromArgb(148, 163, 184);
            }
        }

        private static System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle r, int rad)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(r.X, r.Y, rad * 2, rad * 2, 180, 90);
            path.AddArc(r.Right - rad * 2, r.Y, rad * 2, rad * 2, 270, 90);
            path.AddArc(r.Right - rad * 2, r.Bottom - rad * 2, rad * 2, rad * 2, 0, 90);
            path.AddArc(r.X, r.Bottom - rad * 2, rad * 2, rad * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        private Label MakeSidebarSectionLabel(string text) => new Label
        {
            Text      = text,
            Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
            ForeColor = Color.FromArgb(100, 116, 139),   // slate-500
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.BottomLeft,
            Dock      = DockStyle.Top,
            Height    = 40,
            Padding   = new Padding(16, 0, 0, 6),
        };

        private Panel MakeSidebarDivider() => new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 1,
            BackColor = Color.FromArgb(229, 231, 235),
        };

        // ═══════════════════════════════════════════════════════════════════
        //  CONTENT PAGES
        // ═══════════════════════════════════════════════════════════════════
        private void BuildContentPages()
        {
            BuildLoginPage();
            BuildWelcomePage();
            BuildSidStarPage();
            BuildAirportPage();
            BuildCountryPage();
            BuildFirPage();
            BuildGroundLayoutPage();
            BuildOsmPage();
            BuildCreditsPage();
            BuildFlightPage();
        }

        // ─────────────────────────────────────────────────────────────────
        //  LOGIN PAGE  - two-panel: dark gradient left + white right
        // ─────────────────────────────────────────────────────────────────
        private void BuildLoginPage()
        {
            this.loginPage = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.White,
                Visible   = true,
            };

            // ── Left dark panel (gradient + radar animation) ──────────────
            this.loginLeftPanel = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 440,
                BackColor = Color.FromArgb(12, 36, 144),
            };
            this.loginLeftPanel.Paint += LoginLeftPanel_Paint;

            // IVAO logo top-left of dark panel
            var leftLogo = new PictureBox
            {
                Size      = new Size(110, 38),
                SizeMode  = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Location  = new Point(36, 36),
            };
            try { leftLogo.Image = Image.FromFile("./ivao_blue.png"); } catch { }

            // Hero text anchored to bottom of dark panel
            var heroArea = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 316,
                BackColor = Color.Transparent,
                Padding   = new Padding(36, 0, 36, 20),
            };

            var tagsRow = new Panel { Dock = DockStyle.Bottom, Height = 62, BackColor = Color.Transparent };
            var chipFont = new Font("Segoe UI", 7f, FontStyle.Bold);
            var tagList = new[] { "OSM", "Airport Data", "VOR/NDB Data", "Airspace Data" };
            int tagX = 0, tagY = 6;
            for (int i = 0; i < tagList.Length; i++)
            {
                if (i == 3) { tagX = 0; tagY = 34; }   // wrap 4th chip to second row
                int chipW = TextRenderer.MeasureText(tagList[i], chipFont).Width + 28;
                tagsRow.Controls.Add(new Label
                {
                    Text        = tagList[i],
                    Font        = chipFont,
                    ForeColor   = Color.FromArgb(147, 197, 253),
                    BackColor   = Color.FromArgb(30, 58, 138),
                    AutoSize    = false,
                    UseMnemonic = false,
                    Size        = new Size(chipW, 22),
                    Location    = new Point(tagX, tagY),
                    TextAlign   = ContentAlignment.MiddleCenter,
                });
                tagX += chipW + 6;
            }

            var heroSub = new Label
            {
                Text      = "Generate sector files for Aurora\nand other ATC client software.",
                ForeColor = Color.FromArgb(147, 197, 253),
                Font      = new Font("Segoe UI", 9f),
                BackColor = Color.Transparent,
                AutoSize  = false,
                Dock      = DockStyle.Top,
                Height    = 54,
                TextAlign = ContentAlignment.TopLeft,
            };

            var heroMain = new Label
            {
                Text         = "Build & Create\nSector Files",
                ForeColor    = Color.White,
                Font         = new Font("Segoe UI", 18f, FontStyle.Bold),
                BackColor    = Color.Transparent,
                AutoSize     = false,
                UseMnemonic  = false,
                Dock         = DockStyle.Top,
                Height       = 100,
                TextAlign    = ContentAlignment.TopLeft,
            };

            heroArea.Controls.Add(tagsRow);
            heroArea.Controls.Add(heroSub);
            heroArea.Controls.Add(heroMain);

            this.loginLeftPanel.Controls.Add(heroArea);
            this.loginLeftPanel.Controls.Add(leftLogo);

            // ── Right white panel (loginCard - referenced in ApplyTheme) ──
            this.loginCard = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.White,
            };

            // Inner card (centred inside loginCard)
            var innerCard = new Panel { BackColor = Color.White, Size = new Size(340, 390) };
            this.loginCard.Controls.Add(innerCard);
            this.loginCard.Resize += (s, e) =>
            {
                int cx = (loginCard.ClientSize.Width  - innerCard.Width)  / 2;
                int cy = (loginCard.ClientSize.Height - innerCard.Height) / 2;
                innerCard.Location = new Point(Math.Max(0, cx), Math.Max(12, cy));
            };

            // IVAO logo - centred above title, zoom-fitted so full image is always visible
            this.loginIconLabel = new PictureBox
            {
                Size      = new Size(280, 72),
                Location  = new Point(30, 0),   // centred in 340px card
                BackColor = Color.Transparent,
                SizeMode  = PictureBoxSizeMode.Zoom,
            };
            try   { this.loginIconLabel.Image = Image.FromFile("./ivao_blue.png"); }
            catch { /* no image - space stays blank */ }

            this.loginTitleLabel = new Label
            {
                Text      = AppInfo.Name,
                Font      = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(10, 20, 60),
                BackColor = Color.Transparent,
                AutoSize  = false,
                Size      = new Size(340, 50),
                Location  = new Point(0, 78),
                TextAlign = ContentAlignment.MiddleCenter,
            };

            this.loginDescLabel = new Label
            {
                Text      = "Sign in with your IVAO account to continue.",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(100, 115, 145),
                BackColor = Color.Transparent,
                AutoSize  = false,
                Size      = new Size(340, 40),
                Location  = new Point(0, 128),
                TextAlign = ContentAlignment.MiddleCenter,
            };

            var dividerLine = new Panel
            {
                Size      = new Size(340, 1),
                Location  = new Point(0, 176),
                BackColor = Color.FromArgb(226, 232, 245),
            };

            this.loginAiracLabel = new Label
            {
                Text      = SplashForm.AiracDaysLeft >= 0
                    ? $"AIRAC {SplashForm.AiracCycle}  ·  {SplashForm.AiracEffective} → {SplashForm.AiracExpiry}  ({SplashForm.AiracDaysLeft} days left)"
                    : "AIRAC data unavailable",
                Font      = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = SplashForm.AiracDaysLeft >= 0 ? Color.FromArgb(46, 125, 50) : Color.FromArgb(130, 140, 160),
                BackColor = Color.Transparent,
                Size      = new Size(340, 22),
                Location  = new Point(0, 186),
                TextAlign = ContentAlignment.MiddleCenter,
            };

            this.loginButton = new Button
            {
                Text      = "Continue with IVAO SSO",
                Font      = new Font("Segoe UI", 11f, FontStyle.Bold),
                BackColor = Color.FromArgb(12, 50, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size      = new Size(340, 50),
                Location  = new Point(0, 218),
                Cursor    = Cursors.Hand,
            };
            this.loginButton.FlatAppearance.BorderSize = 0;
            this.loginButton.UseVisualStyleBackColor   = false;
            this.loginButton.Click += LoginButton_Click;
            this.loginButton.Paint += (s, e) =>
            {
                var btn = (Button)s;
                using var br = new LinearGradientBrush(
                    btn.ClientRectangle,
                    btn.Enabled ? Color.FromArgb(25, 80, 210) : Color.FromArgb(120, 140, 170),
                    btn.Enabled ? Color.FromArgb(7,  40, 120) : Color.FromArgb(90, 110, 140),
                    LinearGradientMode.Horizontal);
                e.Graphics.FillRectangle(br, btn.ClientRectangle);
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString(btn.Text, btn.Font, Brushes.White, btn.ClientRectangle, sf);
            };

            // Amber staff notice
            var noticePanel = new Panel
            {
                BackColor = Color.FromArgb(255, 251, 235),
                Size      = new Size(340, 56),
                Location  = new Point(0, 280),
            };
            noticePanel.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(252, 211, 77), 1.5f);
                e.Graphics.DrawRectangle(pen, 0, 0, noticePanel.Width - 1, noticePanel.Height - 1);
            };
            noticePanel.Controls.Add(new Label
            {
                Text      = "Staff access only\nRestricted to authorised IVAO staff members.",
                Font      = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(146, 64, 14),
                BackColor = Color.Transparent,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(12, 0, 12, 0),
            });

            this.loginNoteLabel = new Label
            {
                Text      = "⚠  NOT FOR REAL WORLD USE  ·  v1.0",
                Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(185, 28, 28),
                BackColor = Color.Transparent,
                Size      = new Size(340, 22),
                Location  = new Point(0, 348),
                TextAlign = ContentAlignment.MiddleCenter,
            };

            innerCard.Controls.Add(this.loginIconLabel);
            innerCard.Controls.Add(this.loginTitleLabel);
            innerCard.Controls.Add(this.loginDescLabel);
            innerCard.Controls.Add(dividerLine);
            innerCard.Controls.Add(this.loginAiracLabel);
            innerCard.Controls.Add(this.loginButton);
            innerCard.Controls.Add(noticePanel);
            innerCard.Controls.Add(this.loginNoteLabel);

            // Radar sweep timer
            this._loginRadarTimer = new System.Windows.Forms.Timer { Interval = 50 };
            this._loginRadarTimer.Tick += (s, e) =>
            {
                _loginRadarAngle = (_loginRadarAngle + 3f) % 360f;
                loginLeftPanel.Invalidate();
            };
            this._loginRadarTimer.Start();

            // Assemble: left panel docks Left, right panel fills remaining
            this.loginPage.Controls.Add(this.loginCard);       // Fill first
            this.loginPage.Controls.Add(this.loginLeftPanel);  // then Left
        }

        private void LoginLeftPanel_Paint(object sender, PaintEventArgs e)
        {
            var g    = e.Graphics;
            var rect = loginLeftPanel.ClientRectangle;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Dark gradient background
            using (var bg = new LinearGradientBrush(rect,
                Color.FromArgb(12, 36, 144), Color.FromArgb(4, 13, 56),
                LinearGradientMode.ForwardDiagonal))
            {
                g.FillRectangle(bg, rect);
            }

            int cx = rect.Width  / 2 + 24;
            int cy = rect.Height / 2 - 10;

            // Concentric dashed rings
            using (var ringPen = new Pen(Color.FromArgb(40, 100, 180, 255), 1f))
            {
                ringPen.DashStyle   = DashStyle.Custom;
                ringPen.DashPattern = new float[] { 4f, 4f };
                foreach (int r in new[] { 55, 105, 155, 205 })
                    g.DrawEllipse(ringPen, cx - r, cy - r, r * 2, r * 2);
            }

            // Cross-hairs
            using (var crossPen = new Pen(Color.FromArgb(28, 100, 180, 255), 1f))
            {
                g.DrawLine(crossPen, cx - 215, cy, cx + 215, cy);
                g.DrawLine(crossPen, cx, cy - 215, cx, cy + 215);
            }

            // Sweep (filled pie slices)
            var pie = new RectangleF(cx - 205, cy - 205, 410, 410);
            using (var s1 = new SolidBrush(Color.FromArgb(22, 0, 230, 100)))
                g.FillPie(s1, pie.X, pie.Y, pie.Width, pie.Height, _loginRadarAngle - 30f, 30f);
            using (var s2 = new SolidBrush(Color.FromArgb(12, 0, 230, 100)))
                g.FillPie(s2, pie.X, pie.Y, pie.Width, pie.Height, _loginRadarAngle - 50f, 20f);

            // Sweep line
            double ang = _loginRadarAngle * Math.PI / 180.0;
            using (var lp = new Pen(Color.FromArgb(110, 0, 255, 130), 1.5f))
                g.DrawLine(lp, cx, cy, cx + 205f * (float)Math.Cos(ang), cy + 205f * (float)Math.Sin(ang));

            // Blip dots
            using (var blip = new SolidBrush(Color.FromArgb(180, 0, 255, 140)))
            {
                foreach (var (frac, offDeg) in new (float, float)[] { (0.55f, 8f), (0.72f, -6f), (0.88f, 12f) })
                {
                    double ba = ang + offDeg * Math.PI / 180.0;
                    float  bx = cx + frac * 205f * (float)Math.Cos(ba);
                    float  by = cy + frac * 205f * (float)Math.Sin(ba);
                    g.FillEllipse(blip, bx - 3, by - 3, 6, 6);
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────
        //  WELCOME / HOME PAGE  - clean document style matching IVAO Data Website
        // ─────────────────────────────────────────────────────────────────
        private void BuildWelcomePage()
        {
            // AutoScroll=true + no Padding: a fixed-height contentWrapper (no DockStyle)
            // lets the scroll panel measure the virtual area correctly.
            this.welcomePage = new Panel
            {
                Dock       = DockStyle.Fill,
                BackColor  = Color.White,
                Visible    = false,
                AutoScroll = true,
            };

            // Field stubs - kept so MainForm.cs compiles
            this.hubAiracCard       = new Panel { Visible = false };
            this.hubAiracCycleLabel = new Label { Visible = false };
            this.hubAiracDatesLabel = new Label { Visible = false };

            // contentWrapper: no DockStyle → AutoScroll sees its Height as virtual size
            var contentWrapper = new Panel
            {
                Location  = new Point(0, 36),
                BackColor = Color.White,
                Padding   = new Padding(52, 36, 52, 36),
            };

            // ── Title ─────────────────────────────────────────────────────
            this.hubWelcomeLabel = new Label
            {
                Text        = "Welcome to IVAO Sector File Creator",
                Font        = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor   = Color.FromArgb(17, 24, 39),
                Dock        = DockStyle.Top,
                Height      = 50,
                AutoSize    = false,
                TextAlign   = ContentAlignment.BottomLeft,
                UseMnemonic = false,
            };

            var gapTitle = new Panel { Dock = DockStyle.Top, Height = 10, BackColor = Color.Transparent };

            // ── Subtitle / user hint ──────────────────────────────────────
            this.hubHintLabel = new Label
            {
                Text        = "Third party sector file creation tool for IVAO.",
                Font        = new Font("Segoe UI", 10.5f),
                ForeColor   = Color.FromArgb(107, 114, 128),
                Dock        = DockStyle.Top,
                Height      = 28,
                AutoSize    = false,
                TextAlign   = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };

            var gap1 = new Panel { Dock = DockStyle.Top, Height = 32, BackColor = Color.Transparent };

            // ── Description paragraph ─────────────────────────────────────
            var descLbl = new Label
            {
                Text      = "Using the sidebar on the left, you can create, edit and export sector files " +
                             "for use on the IVAO network. Generate SID/STAR procedures, airport data, " +
                             "FIR boundaries, country boundaries, ground layouts and more. All output is " +
                             "directly applicable to Aurora sector files. Please verify all generated data " +
                             "before use on the IVAO network.",
                Font      = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(55, 65, 81),
                Dock      = DockStyle.Top,
                Height    = 80,
                AutoSize  = false,
                TextAlign = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };

            var gap2 = new Panel { Dock = DockStyle.Top, Height = 20, BackColor = Color.Transparent };

            // ── AIRAC status chip ─────────────────────────────────────────
            bool airacOk  = SplashForm.AiracDaysLeft >= 0;
            var airacChip = new Label
            {
                Text      = airacOk
                    ? $"  AIRAC {SplashForm.AiracCycle}  ·  {SplashForm.AiracEffective} → {SplashForm.AiracExpiry}  ·  {SplashForm.AiracDaysLeft} days remaining"
                    : "  AIRAC data unavailable - check your internet connection",
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = airacOk ? Color.FromArgb(22, 101, 52)   : Color.FromArgb(107, 114, 128),
                BackColor = airacOk ? Color.FromArgb(220, 252, 231) : Color.FromArgb(243, 244, 246),
                Dock      = DockStyle.Top,
                Height    = 34,
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleLeft,
            };

            var gap3 = new Panel { Dock = DockStyle.Top, Height = 28, BackColor = Color.Transparent };

            // ── Info card ─────────────────────────────────────────────────
            var availableTools = new (string label, bool done)[]
            {
                ("Dashboard",                    true),
                ("SID & STAR Procedures",        true),
                ("Airport Data",                 true),
                ("FIR Boundaries",               true),
                ("Country Boundaries",           true),
                ("Ground Layout (KML import)",   true),
                ("Flight Schedules (ADS-B)",     true),
                ("OSM Data import",              false),
            };

            int cardPadH = 20, cardPadV = 18;
            int itemH    = 22;
            int cardContentH = 26 + 32 + 14 + 22 + (availableTools.Length * itemH) + 14 + 22;
            var infoCard = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = cardContentH + cardPadV * 2,
                BackColor = Color.FromArgb(239, 246, 255),
                Padding   = new Padding(cardPadH, cardPadV, cardPadH, cardPadV),
            };
            infoCard.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(186, 211, 253), 1);
                e.Graphics.DrawRectangle(pen, 0, 0, infoCard.Width - 1, infoCard.Height - 1);
            };

            var cardTitle = new Label
            {
                Text        = "IVAO Sector File Creator  (Beta Version)",
                Font        = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor   = Color.FromArgb(17, 24, 39),
                Dock        = DockStyle.Top,
                Height      = 26,
                AutoSize    = false,
                TextAlign   = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };

            var cardDesc = new Label
            {
                Text      = "This tool is currently under active development. Several modules are available " +
                             "while others are still being built or planned for upcoming releases.",
                Font      = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(75, 85, 99),
                Dock      = DockStyle.Top,
                Height    = 32,
                AutoSize  = false,
                TextAlign = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };

            var cardGap1 = new Panel { Dock = DockStyle.Top, Height = 14, BackColor = Color.Transparent };

            var modulesHeader = new Label
            {
                Text      = "Modules:",
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Dock      = DockStyle.Top,
                Height    = 22,
                AutoSize  = false,
                TextAlign = ContentAlignment.TopLeft,
            };

            var toolItemLabels = availableTools.Select(t =>
            {
                string prefix = t.done ? "  ✅" : "  🔲";
                Color  col    = t.done ? Color.FromArgb(55, 65, 81) : Color.FromArgb(150, 160, 175);
                return new Label
                {
                    Text        = $"{prefix}  {t.label}",
                    Font        = new Font("Segoe UI", 9.5f),
                    ForeColor   = col,
                    BackColor   = Color.Transparent,
                    Dock        = DockStyle.Top,
                    Height      = itemH,
                    AutoSize    = false,
                    TextAlign   = ContentAlignment.MiddleLeft,
                    UseMnemonic = false,
                };
            }).ToArray();

            var cardGap2 = new Panel { Dock = DockStyle.Top, Height = 14, BackColor = Color.Transparent };

            var footNote = new Label
            {
                Text        = "Please note that features and functionality are subject to change as development continues.",
                Font        = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor   = Color.FromArgb(150, 160, 175),
                Dock        = DockStyle.Top,
                Height      = 22,
                AutoSize    = false,
                TextAlign   = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };

            // Assemble card (last added = topmost)
            infoCard.Controls.Add(footNote);
            infoCard.Controls.Add(cardGap2);
            for (int i = toolItemLabels.Length - 1; i >= 0; i--)
                infoCard.Controls.Add(toolItemLabels[i]);
            infoCard.Controls.Add(modulesHeader);
            infoCard.Controls.Add(cardGap1);
            infoCard.Controls.Add(cardDesc);
            infoCard.Controls.Add(cardTitle);

            // ── Quick-access tool cards ───────────────────────────────────
            var toolsHeader = new Label
            {
                Text      = "QUICK ACCESS",
                Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(156, 163, 175),
                Dock      = DockStyle.Top,
                Height    = 40,
                AutoSize  = false,
                TextAlign = ContentAlignment.BottomLeft,
            };

            // cardsArea: DockStyle.Top with computed height (not Fill)
            var cardsArea = new Panel { Dock = DockStyle.Top, Height = 400, BackColor = Color.Transparent };

            var cardDefs = new (string title, string sub, string desc, string icon, Color accent)[]
            {
                ("SID & STAR",       "Procedures",           "Departure & arrival procedures\nfrom AIRAC navigation data.",   "✈",  Color.FromArgb( 37,  99, 235)),
                ("Airport Data",     "Runways & facilities", "Runways, VOR/DME, NDB,\ngates and frequencies.",                "🏗", Color.FromArgb(  5, 150, 105)),
                ("FIR Data",         "Airspace boundaries",  "Flight Information Region\nboundaries and data.",                "📡", Color.FromArgb(217, 119,   6)),
                ("Country Data",     "GeoJSON boundaries",   "Country boundary data\nfrom GeoJSON sources.",                   "🌍", Color.FromArgb(124,  58, 237)),
                ("Ground Layout",    "KML import",           "Import KML file for airport\nground layout data.",               "🗺", Color.FromArgb(225,  29,  72)),
                ("OSM Data",         "Coming soon",          "OpenStreetMap ground layout\ndata import (coming soon).",        "🗂", Color.FromArgb(100, 120, 150)),
                ("Flight Schedules", "ADS-B data",           "Search and view real-world\nflight schedule data.",             "📋", Color.FromArgb(  2, 132, 199)),
            };

            var toolCards = new Panel[cardDefs.Length];
            for (int i = 0; i < cardDefs.Length; i++)
            {
                var (t, s, d, ico, ac) = cardDefs[i];
                int idx = i;
                toolCards[i] = BuildToolCard(t, s, d, ico, ac, () =>
                {
                    switch (idx)
                    {
                        case 0: NavigateTo(sidStarPage, "SID & STAR");      SetActiveSidebarItem(navSidStarCard);  break;
                        case 1: NavigateTo(airportPage, "Airport Data");     SetActiveSidebarItem(navAirportCard);  break;
                        case 2: NavigateTo(firPage,     "FIR Data");         SetActiveSidebarItem(navFirCard);      break;
                        case 3: NavigateTo(countryPage, "Country Data");     SetActiveSidebarItem(navCountryCard);  break;
                        case 4: NavigateTo(groundPage,  "Ground Layout");    SetActiveSidebarItem(navKmlCard);      break;
                        case 5: NavigateTo(osmPage,     "OSM Data");         SetActiveSidebarItem(navOsmCard);      break;
                        case 6: NavigateTo(flightPage,  "Flight Schedules"); SetActiveSidebarItem(navFlightCard);   break;
                    }
                });
                cardsArea.Controls.Add(toolCards[i]);
            }

            void LayoutCards()
            {
                int w     = cardsArea.ClientSize.Width;
                int cols  = w < 420 ? 1 : w < 780 ? 2 : w < 1100 ? 3 : 4;
                int gap   = 14;
                int cardH = 190;
                int cardW = Math.Max(160, (w - gap * (cols - 1)) / cols);
                int rows  = (toolCards.Length + cols - 1) / cols;
                cardsArea.Height = rows * cardH + Math.Max(0, rows - 1) * gap + 20; // +20 bottom breathing room
                for (int i = 0; i < toolCards.Length; i++)
                {
                    int col = i % cols, row = i / cols;
                    toolCards[i].SetBounds(col * (cardW + gap), row * (cardH + gap), cardW, cardH);
                }
                // Recompute contentWrapper height after cards height changed
                int totalH = contentWrapper.Padding.Top + contentWrapper.Padding.Bottom;
                foreach (Control c in contentWrapper.Controls) totalH += c.Height;
                contentWrapper.Height = totalH;
            }
            cardsArea.Resize += (s, e) => LayoutCards();

            // ── Assemble contentWrapper (last Controls.Add = topmost) ─────
            var gap4 = new Panel { Dock = DockStyle.Top, Height = 32, BackColor = Color.Transparent };
            contentWrapper.Controls.Add(cardsArea);
            contentWrapper.Controls.Add(toolsHeader);
            contentWrapper.Controls.Add(gap4);
            contentWrapper.Controls.Add(infoCard);
            contentWrapper.Controls.Add(gap3);
            contentWrapper.Controls.Add(airacChip);
            contentWrapper.Controls.Add(gap2);
            contentWrapper.Controls.Add(descLbl);
            contentWrapper.Controls.Add(gap1);
            contentWrapper.Controls.Add(this.hubHintLabel);
            contentWrapper.Controls.Add(gapTitle);
            contentWrapper.Controls.Add(this.hubWelcomeLabel);  // topmost

            // Keep contentWrapper width == welcomePage client width (no horiz scroll needed)
            this.welcomePage.Resize += (s, e) =>
            {
                contentWrapper.Width = welcomePage.ClientSize.Width;
                LayoutCards();
            };

            // ── Disclaimer banner ─────────────────────────────────────────
            var disclaimerBanner = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 36,
                BackColor = Color.FromArgb(220, 38, 38),
            };
            var disclaimerLabel = new Label
            {
                Text      = "⚠  This is a third-party app and is NOT an official IVAO application.",
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                Dock      = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
            };
            disclaimerBanner.Controls.Add(disclaimerLabel);
            this.welcomePage.Controls.Add(disclaimerBanner);

            this.welcomePage.Controls.Add(contentWrapper);
        }

        private Panel BuildToolCard(string title, string sub, string desc, string icon,
                                    Color accent, Action onClick)
        {
            var iconBg = Color.FromArgb(
                Math.Min(255, 220 + accent.R / 8),
                Math.Min(255, 220 + accent.G / 8),
                Math.Min(255, 220 + accent.B / 8));

            var card = new Panel { BackColor = Color.White, Cursor = Cursors.Hand };
            card.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(229, 231, 235), 1);
                e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };
            card.Click += (s, e) => onClick();

            // Rounded icon box
            var iconBox = new Panel { Size = new Size(44, 44), Location = new Point(20, 20), BackColor = iconBg, Cursor = Cursors.Hand };
            iconBox.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var br   = new SolidBrush(iconBg);
                using var path = RoundedRect(new Rectangle(0, 0, iconBox.Width - 1, iconBox.Height - 1), 10);
                e.Graphics.FillPath(br, path);
            };
            iconBox.Click += (s, e) => onClick();
            var iconLbl = new Label
            {
                Text = icon, Font = new Font("Segoe UI Emoji", 16f),
                ForeColor = accent, BackColor = Color.Transparent,
                Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Cursor = Cursors.Hand,
            };
            iconLbl.Click += (s, e) => onClick();
            iconBox.Controls.Add(iconLbl);

            var titleLbl = new Label
            {
                Text = title, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39), BackColor = Color.Transparent,
                AutoSize = false, Location = new Point(74, 18), Size = new Size(160, 22),
                TextAlign = ContentAlignment.MiddleLeft, Cursor = Cursors.Hand,
            };
            titleLbl.Click += (s, e) => onClick();

            var subLbl = new Label
            {
                Text = sub, Font = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(156, 163, 175), BackColor = Color.Transparent,
                AutoSize = false, Location = new Point(74, 40), Size = new Size(160, 18),
                TextAlign = ContentAlignment.MiddleLeft, Cursor = Cursors.Hand,
            };
            subLbl.Click += (s, e) => onClick();

            var divLine = new Panel
            {
                BackColor = Color.FromArgb(243, 244, 246),
                Size = new Size(200, 1), Location = new Point(20, 76),
            };

            var descLbl = new Label
            {
                Text = desc, Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(107, 114, 128), BackColor = Color.Transparent,
                AutoSize = false, Location = new Point(20, 86), Size = new Size(220, 40),
                TextAlign = ContentAlignment.TopLeft, Cursor = Cursors.Hand,
            };
            descLbl.Click += (s, e) => onClick();

            var openBtn = new Button
            {
                Text = "Open →", Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = accent, BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Size = new Size(76, 26),
            };
            openBtn.FlatAppearance.BorderSize         = 0;
            openBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(18, accent.R, accent.G, accent.B);
            openBtn.UseVisualStyleBackColor           = false;
            openBtn.Click += (s, e) => onClick();

            card.Controls.Add(iconBox);
            card.Controls.Add(titleLbl);
            card.Controls.Add(subLbl);
            card.Controls.Add(divLine);
            card.Controls.Add(descLbl);
            card.Controls.Add(openBtn);

            card.Resize += (s, e) =>
            {
                int w = card.Width;
                titleLbl.Width   = Math.Max(60, w - 94);
                subLbl.Width     = Math.Max(60, w - 94);
                divLine.Width    = Math.Max(40, w - 40);
                descLbl.Width    = Math.Max(40, w - 40);
                openBtn.Location = new Point(Math.Max(20, w - 96), card.Height - 36);
            };

            return card;
        }

        // ─────────────────────────────────────────────────────────────────
        //  SID & STAR PAGE
        // ─────────────────────────────────────────────────────────────────
        private void BuildSidStarPage()
        {
            this.sidStarPage = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.White,
                Visible   = false,
            };

            // Hidden CheckBox fields - read by SidStar.cs logic; not rendered directly
            this.chkSidAlt    = new CheckBox { Visible = false, Checked = true };
            this.chkSidSpd    = new CheckBox { Visible = false, Checked = true };
            this.chkSidCoord  = new CheckBox { Visible = false, Checked = true };
            this.chkStarAlt   = new CheckBox { Visible = false, Checked = true };
            this.chkStarSpd   = new CheckBox { Visible = false, Checked = true };
            this.chkStarCoord = new CheckBox { Visible = false, Checked = true };

            // Stub fields kept so Settings.cs / legacy refs compile
            this.ssOptionsLeft       = new Panel { Visible = false };
            this.ssOptionsRight      = new Panel { Visible = false };
            this.ssOptionsLeftLabel  = new Label { Visible = false };
            this.ssOptionsRightLabel = new Label { Visible = false };

            const int PadH = 52;

            // ── Header ─────────────────────────────────────────────────────
            this.ssHeaderPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 236,
                BackColor = Color.White,
                Padding   = new Padding(PadH, 20, PadH, 20),
            };

            this.ssTitleLabel = new Label
            {
                Text        = "SID & STAR Generator",
                Font        = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor   = Color.FromArgb(17, 24, 39),
                Dock        = DockStyle.Top,
                Height      = 52,
                AutoSize    = false,
                TextAlign   = ContentAlignment.MiddleLeft,
                UseMnemonic = false,
            };

            var ssGapTitle = new Panel { Dock = DockStyle.Top, Height = 10, BackColor = Color.Transparent };

            var ssSubLabel = new Label
            {
                Text        = "Generate departure and arrival procedures from AIRAC navigation data.",
                Font        = new Font("Segoe UI", 10.5f),
                ForeColor   = Color.FromArgb(107, 114, 128),
                Dock        = DockStyle.Top,
                Height      = 26,
                AutoSize    = false,
                TextAlign   = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };

            var ssGap1 = new Panel { Dock = DockStyle.Top, Height = 24, BackColor = Color.Transparent };

            var ssDescLabel = new Label
            {
                Text        = "Using the form below, generate SID (Standard Instrument Departure) and STAR " +
                              "(Standard Terminal Arrival Route) procedures for any airport using real AIRAC " +
                              "navigation data. Toggle which data fields to include - altitude limits, speed " +
                              "restrictions, and waypoint coordinates - then click Generate and download the " +
                              "output files directly into your Aurora sector files.",
                Font        = new Font("Segoe UI", 10f),
                ForeColor   = Color.FromArgb(55, 65, 81),
                Dock        = DockStyle.Top,
                Height      = 84,
                AutoSize    = false,
                TextAlign   = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };

            this.ssAiracCycleLabel = new Label
            {
                Text      = SplashForm.AiracDaysLeft >= 0 ? $"AIRAC  {SplashForm.AiracCycle}" : "",
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(13, 71, 161),
                TextAlign = ContentAlignment.BottomRight,
                Size      = new Size(180, 22),
                AutoSize  = false,
            };
            this.ssAiracDaysLabel = new Label
            {
                Text      = SplashForm.AiracDaysLeft >= 0 ? $"{SplashForm.AiracDaysLeft} days left" : "",
                Font      = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(100, 120, 150),
                TextAlign = ContentAlignment.TopRight,
                Size      = new Size(180, 18),
                AutoSize  = false,
            };

            ssHeaderPanel.Controls.Add(ssDescLabel);
            ssHeaderPanel.Controls.Add(ssGap1);
            ssHeaderPanel.Controls.Add(ssSubLabel);
            ssHeaderPanel.Controls.Add(ssGapTitle);
            ssHeaderPanel.Controls.Add(ssTitleLabel);
            ssHeaderPanel.Controls.Add(ssAiracCycleLabel);
            ssHeaderPanel.Controls.Add(ssAiracDaysLabel);
            ssHeaderPanel.Resize += (s, e) =>
            {
                int r = ssHeaderPanel.ClientSize.Width - PadH;
                ssAiracCycleLabel.Location = new Point(r - 180, 24);
                ssAiracDaysLabel.Location  = new Point(r - 180, 48);
            };

            // ── Search bar ─────────────────────────────────────────────────
            this.ssSearchPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 58,
                BackColor = Color.White,
            };

            // Styled ICAO input wrapper (custom-painted border)
            this.ssIcaoWrapper = new Panel
            {
                BackColor = Color.White,
                Height    = 42,
                Cursor    = Cursors.IBeam,
            };

            this.ssIcaoBox = new TextBox
            {
                Font            = new Font("Segoe UI", 11f),
                PlaceholderText = "Enter ICAO airport code  (e.g. VTBD, WMKK, VABB)",
                BorderStyle     = BorderStyle.None,
                BackColor       = Color.White,
                Dock            = DockStyle.None,
            };

            // Place textbox vertically centered inside wrapper with padding
            ssIcaoWrapper.Controls.Add(ssIcaoBox);

            // Repaint wrapper on focus change to switch border color
            ssIcaoBox.Enter += (s, e) => ssIcaoWrapper.Invalidate();
            ssIcaoBox.Leave += (s, e) => ssIcaoWrapper.Invalidate();

            // Click wrapper → focus textbox
            ssIcaoWrapper.Click += (s, e) => ssIcaoBox.Focus();

            // Paint: rounded-rect border, blue when focused, gray otherwise
            ssIcaoWrapper.Paint += (s, e) =>
            {
                bool focused = ssIcaoBox.Focused;
                var  g       = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var borderColor = focused
                    ? Color.FromArgb(37, 99, 235)
                    : Color.FromArgb(203, 213, 225);
                using var path = RoundedRect(
                    new Rectangle(0, 0, ssIcaoWrapper.Width - 1, ssIcaoWrapper.Height - 1), 6);
                using var pen = new Pen(borderColor, focused ? 2f : 1f);
                g.DrawPath(pen, path);
            };

            // Keep textbox properly sized inside wrapper
            ssIcaoWrapper.Resize += (s, e) =>
            {
                int pad = 12;
                ssIcaoBox.SetBounds(pad, (ssIcaoWrapper.Height - ssIcaoBox.Height) / 2,
                    ssIcaoWrapper.Width - pad * 2, ssIcaoBox.Height);
            };

            this.ssSearchButton = new Button
            {
                Text      = "Generate",
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height    = 42,
                Width     = 110,
                Cursor    = Cursors.Hand,
            };
            ssSearchButton.FlatAppearance.BorderSize = 0;
            ssSearchButton.FlatAppearance.MouseOverBackColor  = Color.FromArgb(29, 78, 216);
            ssSearchButton.FlatAppearance.MouseDownBackColor  = Color.FromArgb(30, 64, 175);

            ssSearchPanel.Controls.Add(ssIcaoWrapper);
            ssSearchPanel.Controls.Add(ssSearchButton);
            ssSearchPanel.Resize += (s, e) =>
            {
                int avail = ssSearchPanel.ClientSize.Width - PadH * 2;
                ssIcaoWrapper.Location      = new Point(PadH, 8);
                ssIcaoWrapper.Width         = Math.Max(80, avail - 118);
                ssSearchButton.Location     = new Point(PadH + avail - 110, 8);
            };

            // ── Options: toggle cards ──────────────────────────────────────
            this.ssOptionsPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 280,
                BackColor = Color.White,
            };
            ssOptionsPanel.Paint += PaintBottomBorder;

            Panel MakeSsToggleCard(CheckBox chk, string icon, string title, string desc, Color accent)
            {
                var activeBg = Color.FromArgb(
                    Math.Min(255, accent.R / 4 + 200),
                    Math.Min(255, accent.G / 4 + 200),
                    Math.Min(255, accent.B / 4 + 200));

                var card = new Panel { BackColor = Color.White, Cursor = Cursors.Hand };

                var iconBox = new Panel { Size = new Size(36, 36), Location = new Point(12, 14), Cursor = Cursors.Hand };
                iconBox.Region = new Region(RoundedRect(new Rectangle(0, 0, 36, 36), 8));

                var iconLbl = new Label
                {
                    Text        = icon,
                    Font        = new Font("Segoe UI Emoji", 12f),
                    BackColor   = Color.Transparent,
                    TextAlign   = ContentAlignment.MiddleCenter,
                    Dock        = DockStyle.Fill,
                    Cursor      = Cursors.Hand,
                    UseMnemonic = false,
                };
                iconBox.Controls.Add(iconLbl);

                var titleLbl = new Label
                {
                    Text        = title,
                    Font        = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                    AutoSize    = false,
                    Location    = new Point(56, 11),
                    Size        = new Size(90, 20),
                    TextAlign   = ContentAlignment.TopLeft,
                    Cursor      = Cursors.Hand,
                    UseMnemonic = false,
                };
                var descLbl = new Label
                {
                    Text        = desc,
                    Font        = new Font("Segoe UI", 8f),
                    ForeColor   = Color.FromArgb(148, 163, 184),
                    AutoSize    = false,
                    Location    = new Point(56, 33),
                    Size        = new Size(90, 18),
                    TextAlign   = ContentAlignment.TopLeft,
                    Cursor      = Cursors.Hand,
                    UseMnemonic = false,
                };

                card.Controls.Add(iconBox);
                card.Controls.Add(titleLbl);
                card.Controls.Add(descLbl);

                void Repaint()
                {
                    bool on = chk.Checked;
                    card.BackColor     = on ? Color.White : Color.FromArgb(248, 250, 252);
                    iconBox.BackColor  = on ? activeBg : Color.FromArgb(241, 245, 249);
                    iconLbl.ForeColor  = on ? accent : Color.FromArgb(148, 163, 184);
                    titleLbl.ForeColor = on ? Color.FromArgb(17, 24, 39) : Color.FromArgb(148, 163, 184);
                    card.Invalidate();
                }

                card.Paint += (s, e) =>
                {
                    bool on = chk.Checked;
                    using var borderPen = new Pen(on ? accent : Color.FromArgb(226, 232, 240), 1);
                    e.Graphics.DrawRectangle(borderPen, 0, 0, card.Width - 1, card.Height - 1);
                    using var barBrush = new SolidBrush(on ? accent : Color.FromArgb(203, 213, 225));
                    e.Graphics.FillRectangle(barBrush, 0, 0, 3, card.Height);
                };

                card.Resize += (s, e) =>
                {
                    int tw = Math.Max(10, card.ClientSize.Width - 56 - 8);
                    titleLbl.Width = tw;
                    descLbl.Width  = tw;
                };

                void Toggle(object s2, EventArgs e2) { chk.Checked = !chk.Checked; Repaint(); }
                card.Click     += Toggle;
                iconBox.Click  += Toggle;
                iconLbl.Click  += Toggle;
                titleLbl.Click += Toggle;
                descLbl.Click  += Toggle;

                Repaint();
                return card;
            }

            var sidBlue   = Color.FromArgb(37, 99, 235);
            var starGreen = Color.FromArgb(5, 150, 105);

            var cSidAlt   = MakeSsToggleCard(chkSidAlt,   "⬆",  "Altitude",    "Altitude limits",    sidBlue);
            var cSidSpd   = MakeSsToggleCard(chkSidSpd,   "⚡", "Speed",       "Speed restrictions", sidBlue);
            var cSidCoord = MakeSsToggleCard(chkSidCoord, "📍", "Coordinates", "Waypoint coords",    sidBlue);

            var cStarAlt   = MakeSsToggleCard(chkStarAlt,   "⬆",  "Altitude",    "Altitude limits",    starGreen);
            var cStarSpd   = MakeSsToggleCard(chkStarSpd,   "⚡", "Speed",       "Speed restrictions", starGreen);
            var cStarCoord = MakeSsToggleCard(chkStarCoord, "📍", "Coordinates", "Waypoint coords",    starGreen);

            var sidSectionLbl = new Label
            {
                Text      = "SID OPTIONS",
                Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = sidBlue,
                AutoSize  = false,
                TextAlign = ContentAlignment.BottomLeft,
            };
            var starSectionLbl = new Label
            {
                Text      = "STAR OPTIONS",
                Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = starGreen,
                AutoSize  = false,
                TextAlign = ContentAlignment.BottomLeft,
            };

            var sidCards  = new[] { cSidAlt, cSidSpd, cSidCoord };
            var starCards = new[] { cStarAlt, cStarSpd, cStarCoord };

            foreach (var c in sidCards)  ssOptionsPanel.Controls.Add(c);
            foreach (var c in starCards) ssOptionsPanel.Controls.Add(c);
            ssOptionsPanel.Controls.Add(sidSectionLbl);
            ssOptionsPanel.Controls.Add(starSectionLbl);

            const int CardH   = 64;
            const int CardGap = 10;
            const int SecLblH = 24;

            void LayoutSsOptions()
            {
                int avail = ssOptionsPanel.ClientSize.Width - PadH * 2;
                int cardW = Math.Max(100, (avail - CardGap * 2) / 3);

                int y0 = 16;
                sidSectionLbl.SetBounds(PadH, y0, avail, SecLblH);
                int y1 = y0 + SecLblH + 6;
                for (int i = 0; i < sidCards.Length; i++)
                    sidCards[i].SetBounds(PadH + i * (cardW + CardGap), y1, cardW, CardH);

                int y2 = y1 + CardH + 16;
                starSectionLbl.SetBounds(PadH, y2, avail, SecLblH);
                int y3 = y2 + SecLblH + 6;
                for (int i = 0; i < starCards.Length; i++)
                    starCards[i].SetBounds(PadH + i * (cardW + CardGap), y3, cardW, CardH);

                ssOptionsPanel.Height = y3 + CardH + 16;
            }

            ssOptionsPanel.Resize += (s, e) => LayoutSsOptions();

            // ── Progress strip ─────────────────────────────────────────────
            this.ssProgressPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 28,
                BackColor = Color.White,
            };

            this.ssProgressBar = new ProgressBar
            {
                Style    = ProgressBarStyle.Continuous,
                Location = new Point(0, 0),
                Size     = new Size(800, 5),
                Minimum  = 0,
                Maximum  = 100,
                Value    = 0,
            };

            this.ssStatusLabel = new Label
            {
                Text      = "Enter an ICAO code and click Generate",
                Font      = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(120, 140, 165),
                Location  = new Point(PadH, 10),
                Size      = new Size(700, 16),
            };

            ssProgressPanel.Controls.Add(ssProgressBar);
            ssProgressPanel.Controls.Add(ssStatusLabel);
            ssProgressPanel.Resize += (s, e) =>
            {
                ssProgressBar.Width = ssProgressPanel.ClientSize.Width;
                ssStatusLabel.Width = ssProgressPanel.ClientSize.Width - PadH - 10;
            };

            // ── Log output ─────────────────────────────────────────────────
            this.ssLogBox = new RichTextBox
            {
                Dock        = DockStyle.Fill,
                ReadOnly    = true,
                ScrollBars  = RichTextBoxScrollBars.Vertical,
                BackColor   = Color.FromArgb(250, 251, 253),
                ForeColor   = Color.FromArgb(30, 40, 65),
                Font        = new Font("Consolas", 9f),
                BorderStyle = BorderStyle.None,
            };
            ssLogBox.LinkClicked += (s, e) => System.Diagnostics.Process.Start(e.LinkText);

            var ssLogWrapper = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(250, 251, 253),
                Padding   = new Padding(PadH, 12, PadH, 12),
            };
            ssLogWrapper.Controls.Add(ssLogBox);

            // ── Download bar ───────────────────────────────────────────────
            this.ssDownloadPanel = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 72,
                BackColor = Color.White,
            };
            ssDownloadPanel.Paint += PaintTopBorder;

            this.downloadSidButton = new Button
            {
                Text      = "⬇  Download SID File (.SID)",
                Font      = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
            };
            downloadSidButton.FlatAppearance.BorderSize = 0;
            downloadSidButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(27, 94, 32);
            downloadSidButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(18, 62, 22);

            this.downloadStarButton = new Button
            {
                Text      = "⬇  Download STAR File (.STR)",
                Font      = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(21, 101, 192),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
            };
            downloadStarButton.FlatAppearance.BorderSize = 0;
            downloadStarButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(13, 71, 161);
            downloadStarButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(10, 55, 130);

            // Disabled until a successful fetch; wire visual dimming
            foreach (var btn in new[] { downloadSidButton, downloadStarButton })
            {
                var origBg = btn.BackColor;
                btn.Enabled = false;
                btn.EnabledChanged += (s, e) =>
                {
                    btn.BackColor = btn.Enabled ? origBg : Color.FromArgb(200, 200, 210);
                    btn.ForeColor = btn.Enabled ? Color.White : Color.FromArgb(150, 150, 160);
                    btn.Cursor    = btn.Enabled ? Cursors.Hand : Cursors.Default;
                };
            }

            ssDownloadPanel.Controls.Add(downloadSidButton);
            ssDownloadPanel.Controls.Add(downloadStarButton);
            ssDownloadPanel.Resize += (s, e) =>
            {
                int avail = ssDownloadPanel.ClientSize.Width - PadH * 2;
                int half  = (avail - 10) / 2;
                downloadSidButton.SetBounds(PadH, 14, half, 44);
                downloadStarButton.SetBounds(PadH + half + 10, 14, half, 44);
            };

            // Wire logic
            ssSearchButton.Click    += SsSearch_Click;
            ssIcaoBox.KeyDown       += (s, e) => { if (((KeyEventArgs)e).KeyCode == Keys.Enter) SsSearch_Click(s, e); };
            downloadSidButton.Click  += (s, e) => SsSaveFile(_sidOutput,  "SID Files (*.SID)|*.SID",  "SID");
            downloadStarButton.Click += (s, e) => SsSaveFile(_starOutput, "STAR Files (*.STR)|*.STR", "STR");
            ssLogBox.MouseDown      += SsLogBox_MouseDown;
            ssLogBox.MouseMove      += (s, e) =>
            {
                int idx = ssLogBox.GetCharIndexFromPosition(e.Location);
                bool onLink = false;
                foreach (var (start, length, _) in _ssLogLinks)
                    if (idx >= start && idx < start + length) { onLink = true; break; }
                ssLogBox.Cursor = onLink ? Cursors.Hand : Cursors.IBeam;
            };

            // Assemble (Bottom first, Fill, then Top outermost->innermost)
            sidStarPage.Controls.Add(ssLogWrapper);
            sidStarPage.Controls.Add(ssDownloadPanel);
            sidStarPage.Controls.Add(ssProgressPanel);
            sidStarPage.Controls.Add(ssOptionsPanel);
            sidStarPage.Controls.Add(ssSearchPanel);
            sidStarPage.Controls.Add(ssHeaderPanel);
        }

        // ─────────────────────────────────────────────────────────────────
        //  COUNTRY DATA PAGE
        // ─────────────────────────────────────────────────────────────────
        private void BuildCountryPage()
        {
            const int CntPad = 36;

            this.countryPage = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 248, 255),
                Visible   = false,
            };

            // ── Header (matches Airport/FIR style: 236px, white) ──────────
            this.cntHeaderPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 236,
                BackColor = Color.White,
                Padding   = new Padding(CntPad, 20, CntPad, 20),
            };

            this.cntTitleLabel = new Label
            {
                Text        = "Country Boundary Data",
                Font        = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor   = Color.FromArgb(17, 24, 39),
                Dock        = DockStyle.Top,
                Height      = 52,
                AutoSize    = false,
                TextAlign   = ContentAlignment.MiddleLeft,
                UseMnemonic = false,
            };

            var cntGapTitle = new Panel { Dock = DockStyle.Top, Height = 10, BackColor = Color.Transparent };

            var cntSubLbl = new Label
            {
                Text        = "Country Boundaries · GeoJSON → Aurora Sector File",
                Font        = new Font("Segoe UI", 10.5f),
                ForeColor   = Color.FromArgb(107, 114, 128),
                Dock        = DockStyle.Top,
                Height      = 26,
                AutoSize    = false,
                TextAlign   = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };

            var cntGap1 = new Panel { Dock = DockStyle.Top, Height = 24, BackColor = Color.Transparent };

            var cntDescLabel = new Label
            {
                Text        = "Select a country to fetch its geographic boundary and export it as an Aurora .artcc " +
                              "sector file. Type to search, then select from the dropdown list. Boundary data " +
                              "is converted from a global GeoJSON dataset into Aurora's T;IDENTIFIER;LAT;LON; " +
                              "point format, ready for direct use in sector files.",
                Font        = new Font("Segoe UI", 10f),
                ForeColor   = Color.FromArgb(55, 65, 81),
                Dock        = DockStyle.Top,
                Height      = 84,
                AutoSize    = false,
                TextAlign   = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };

            var cntAiracLabel = new Label
            {
                Text      = SplashForm.AiracDaysLeft >= 0 ? $"AIRAC  {SplashForm.AiracCycle}" : "",
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(13, 71, 161),
                TextAlign = ContentAlignment.BottomRight,
                Size      = new Size(180, 22),
                AutoSize  = false,
            };

            // Last added = topmost with DockStyle.Top (same pattern as Airport/FIR)
            cntHeaderPanel.Controls.Add(cntDescLabel);
            cntHeaderPanel.Controls.Add(cntGap1);
            cntHeaderPanel.Controls.Add(cntSubLbl);
            cntHeaderPanel.Controls.Add(cntGapTitle);
            cntHeaderPanel.Controls.Add(this.cntTitleLabel);
            cntHeaderPanel.Controls.Add(cntAiracLabel);
            cntHeaderPanel.Resize += (s, e) =>
            {
                int r = cntHeaderPanel.ClientSize.Width - CntPad;
                cntAiracLabel.Location = new Point(r - 180, 24);
            };

            // ── Search bar ─────────────────────────────────────────────────
            this.cntSearchPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 66,
                BackColor = Color.White,
            };

            this.cntDropdownWrapper = new Panel
            {
                BackColor = Color.White,
                Height    = 42,
                Cursor    = Cursors.IBeam,
            };

            this.cntDropdown = new TextBox
            {
                Font            = new Font("Segoe UI", 11f),
                PlaceholderText = "Type a country name or ISO code…",
                BorderStyle     = BorderStyle.None,
                BackColor       = Color.White,
            };

            cntDropdownWrapper.Controls.Add(this.cntDropdown);
            cntDropdownWrapper.Click += (s, e) => cntDropdown.Focus();
            cntDropdown.Enter += (s, e) => cntDropdownWrapper.Invalidate();
            cntDropdown.Leave += (s, e) => cntDropdownWrapper.Invalidate();
            cntDropdownWrapper.Paint += (s, e) =>
            {
                bool focused    = cntDropdown.Focused;
                var  g          = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var borderColor = focused
                    ? Color.FromArgb(37, 99, 235)
                    : Color.FromArgb(203, 213, 225);
                using var path = RoundedRect(
                    new Rectangle(0, 0, cntDropdownWrapper.Width - 1, cntDropdownWrapper.Height - 1), 6);
                using var pen  = new Pen(borderColor, focused ? 2f : 1f);
                g.DrawPath(pen, path);
            };
            cntDropdownWrapper.Resize += (s, e) =>
            {
                int pad = 12;
                cntDropdown.SetBounds(pad, (cntDropdownWrapper.Height - cntDropdown.Height) / 2,
                    cntDropdownWrapper.Width - pad * 2, cntDropdown.Height);
            };

            this.cntSearchButton = new Button
            {
                Text      = "Fetch",
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height    = 42,
                Width     = 100,
                Cursor    = Cursors.Hand,
            };
            this.cntSearchButton.FlatAppearance.BorderSize = 0;
            this.cntSearchButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 78, 216);
            this.cntSearchButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 64, 175);

            cntSearchPanel.Controls.Add(this.cntDropdownWrapper);
            cntSearchPanel.Controls.Add(this.cntSearchButton);
            cntSearchPanel.Resize += (s, e) =>
            {
                int avail = cntSearchPanel.ClientSize.Width - CntPad * 2;
                cntDropdownWrapper.Location = new Point(CntPad, 12);
                cntDropdownWrapper.Width    = Math.Max(80, avail - 108);
                cntSearchButton.Location    = new Point(CntPad + avail - 100, 12);
            };

            // ── Progress strip (always visible, matches apProgressPanel) ───
            this.cntProgressPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 28,
                BackColor = Color.White,
            };

            this.cntProgressBar = new ProgressBar
            {
                Style    = ProgressBarStyle.Continuous,
                Location = new Point(0, 0),
                Size     = new Size(800, 5),
                Minimum  = 0,
                Maximum  = 100,
                Value    = 0,
            };

            this.cntStatusLabel = new Label
            {
                Text      = "Type a country name above to search",
                Font      = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(120, 140, 165),
                Location  = new Point(10, 10),
                Size      = new Size(700, 16),
            };

            cntProgressPanel.Controls.Add(this.cntProgressBar);
            cntProgressPanel.Controls.Add(this.cntStatusLabel);
            cntProgressPanel.Resize += (s, e) =>
            {
                cntProgressBar.Width  = cntProgressPanel.ClientSize.Width;
                cntStatusLabel.Width  = cntProgressPanel.ClientSize.Width - 20;
            };

            // ── Log area (fill, matches apLogWrapper) ──────────────────────
            this.cntBodyPanel = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(250, 251, 253),
                Padding   = new Padding(CntPad, 12, CntPad, 12),
            };

            this.cntLogBox = new RichTextBox
            {
                Dock        = DockStyle.Fill,
                ReadOnly    = true,
                ScrollBars  = RichTextBoxScrollBars.Vertical,
                BackColor   = Color.FromArgb(250, 251, 253),
                ForeColor   = Color.FromArgb(30, 40, 65),
                Font        = new Font("Consolas", 9f),
                BorderStyle = BorderStyle.None,
            };

            cntBodyPanel.Controls.Add(this.cntLogBox);

            // ── Dropdown overlay (built here; parented to contentPanel in BuildMainLayout) ──
            this.cntDropdownListBox = new ListBox
            {
                Dock        = DockStyle.Fill,
                Font        = new Font("Segoe UI", 10f),
                ForeColor   = Color.FromArgb(17, 24, 39),
                BackColor   = Color.White,
                BorderStyle = BorderStyle.None,
                DrawMode    = DrawMode.OwnerDrawFixed,
                ItemHeight  = 26,
            };
            this.cntDropdownListBox.DrawItem += (s, e) =>
            {
                if (e.Index < 0) return;
                bool sel = (e.State & DrawItemState.Selected) != 0;
                using var bg = new SolidBrush(sel ? Color.FromArgb(239, 246, 255) : Color.White);
                using var fg = new SolidBrush(sel ? Color.FromArgb(37, 99, 235) : Color.FromArgb(17, 24, 39));
                e.Graphics.FillRectangle(bg, e.Bounds);
                e.Graphics.DrawString(
                    cntDropdownListBox.Items[e.Index].ToString(),
                    e.Font, fg,
                    new Rectangle(e.Bounds.X + 10, e.Bounds.Y + 3, e.Bounds.Width - 20, e.Bounds.Height));
            };

            this.cntDropdownOverlay = new Panel
            {
                BackColor   = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible     = false,
            };
            cntDropdownOverlay.Controls.Add(this.cntDropdownListBox);

            // ── Download bar ───────────────────────────────────────────────
            this.cntDownloadPanel = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 70,
                BackColor = Color.White,
            };
            this.cntDownloadPanel.Paint += PaintTopBorder;

            this.cntDownloadButton = MakeDownloadButton(
                "⬇  Download Country Boundary (.artcc)", Color.FromArgb(37, 99, 235));
            this.cntDownloadButton.Enabled = false;

            cntDownloadPanel.Controls.Add(this.cntDownloadButton);
            cntDownloadPanel.Resize += (s, e) =>
            {
                int w = cntDownloadPanel.ClientSize.Width - CntPad * 2;
                cntDownloadButton.SetBounds(CntPad, 14, w, 44);
            };

            // ── Wire events ────────────────────────────────────────────────
            cntSearchButton.Click         += CntSearch_Click;
            cntDropdown.TextChanged       += CntDropdown_TextChanged;
            cntDropdownListBox.MouseClick += CntDropdownList_MouseClick;
            cntDropdown.KeyDown           += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)  { CntSearch_Click(s, e); e.SuppressKeyPress = true; }
                if (e.KeyCode == Keys.Escape) { CntHideDropdown(); e.SuppressKeyPress = true; }
                if (e.KeyCode == Keys.Down && cntDropdownOverlay.Visible)
                {
                    cntDropdownListBox.Focus();
                    if (cntDropdownListBox.Items.Count > 0)
                        cntDropdownListBox.SelectedIndex = 0;
                    e.SuppressKeyPress = true;
                }
            };
            cntDropdownListBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && cntDropdownListBox.SelectedItem != null)
                { CntDropdownList_MouseClick(s, null); e.SuppressKeyPress = true; }
                if (e.KeyCode == Keys.Escape)
                { CntHideDropdown(); cntDropdown.Focus(); e.SuppressKeyPress = true; }
            };
            cntDownloadButton.Click += CntDownload_Click;

            // ── Assemble (Fill first, Bottom, then Top outermost last) ─────
            this.countryPage.Controls.Add(this.cntBodyPanel);       // Fill
            this.countryPage.Controls.Add(this.cntDownloadPanel);   // Bottom
            this.countryPage.Controls.Add(this.cntProgressPanel);   // Top (innermost)
            this.countryPage.Controls.Add(this.cntSearchPanel);     // Top
            this.countryPage.Controls.Add(this.cntHeaderPanel);     // Top (outermost)
            // Note: cntDropdownOverlay is added to contentPanel in BuildMainLayout()
        }

        // ═══════════════════════════════════════════════════════════════════
        //  ASSEMBLE MAIN LAYOUT
        // ═══════════════════════════════════════════════════════════════════
        private void BuildMainLayout()
        {
            this.contentPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

            // All pages added to contentPanel (Fill - all layered, only one visible)
            this.contentPanel.Controls.Add(this.loginPage);
            this.contentPanel.Controls.Add(this.welcomePage);
            this.contentPanel.Controls.Add(this.sidStarPage);
            this.contentPanel.Controls.Add(this.airportPage);
            this.contentPanel.Controls.Add(this.countryPage);
            this.contentPanel.Controls.Add(this.firPage);
            this.contentPanel.Controls.Add(this.groundPage);
            this.contentPanel.Controls.Add(this.osmPage);
            this.contentPanel.Controls.Add(this.creditsPage);
            this.contentPanel.Controls.Add(this.flightPage);

            // Floating country dropdown - parented to contentPanel so it overlays all pages
            this.contentPanel.Controls.Add(this.cntDropdownOverlay);
            this.cntDropdownOverlay.BringToFront();

            this.mainBodyPanel = new Panel { Dock = DockStyle.Fill };
            this.mainBodyPanel.Controls.Add(this.contentPanel);  // added first → fills right
            this.mainBodyPanel.Controls.Add(this.sidebarPanel);  // DockStyle.Left, collapsible

            // Stubs kept for field declaration satisfaction only (unused at runtime)
            this.airportCardPanel  = new Panel();
            this.airportCardBtn    = new Button();
            this.firCardPanel      = new Panel();
            this.firCardBtn        = new Button();
            this.sidStarCard       = new Panel();
            this.sidStarCardPanel  = new Panel();
            this.countryCardPanel  = new Panel();
            this.kmlCardPanel      = new Panel();
            this.kmlCardBtn        = new Button();
            this.flightSchedPanel  = new Panel();
            this.flightSchedBtn    = new Button();

            this.Controls.Add(this.mainBodyPanel);
            this.Controls.Add(this.headerPanel);
        }

        // ═══════════════════════════════════════════════════════════════════
        //  SHARED HELPERS
        // ═══════════════════════════════════════════════════════════════════
        private static Label MakeOptionLabel(string text) => new Label
        {
            Text      = text,
            Font      = new Font("Segoe UI", 8f, FontStyle.Bold),
            ForeColor = Color.FromArgb(80, 100, 135),
            AutoSize  = true,
        };

        private static CheckBox MakeCheckBox(string text) => new CheckBox
        {
            Text      = text,
            Font      = new Font("Segoe UI", 8.5f),
            ForeColor = Color.FromArgb(40, 60, 100),
            AutoSize  = true,
            Cursor    = Cursors.Hand,
        };

        // ─────────────────────────────────────────────────────────────────
        //  AIRPORT DATA PAGE
        // ─────────────────────────────────────────────────────────────────
        private void BuildAirportPage()
        {
            this.airportPage = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 248, 255),
                Visible   = false,
            };

            // ── Header ────────────────────────────────────────────────────
            const int ApPadH = 36;

            this.apHeaderPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 236,
                BackColor = Color.White,
                Padding   = new Padding(ApPadH, 20, ApPadH, 20),
            };

            this.apTitleLabel = new Label
            {
                Text        = "Airport Data",
                Font        = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor   = Color.FromArgb(17, 24, 39),
                Dock        = DockStyle.Top,
                Height      = 52,
                AutoSize    = false,
                TextAlign   = ContentAlignment.MiddleLeft,
                UseMnemonic = false,
            };

            var apGapTitle = new Panel { Dock = DockStyle.Top, Height = 10, BackColor = Color.Transparent };

            var apSubLabel = new Label
            {
                Text        = "Airports · Runways · Frequencies · Navaids",
                Font        = new Font("Segoe UI", 10.5f),
                ForeColor   = Color.FromArgb(107, 114, 128),
                Dock        = DockStyle.Top,
                Height      = 26,
                AutoSize    = false,
                TextAlign   = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };

            var apGap1 = new Panel { Dock = DockStyle.Top, Height = 24, BackColor = Color.Transparent };

            var apDescLabel = new Label
            {
                Text        = "Fetch full airport data for any ICAO code, or all airports within a FIR using " +
                              "a 2-letter prefix. Output includes runway geometry, ATC frequencies, VOR, " +
                              "VOR/DME, DME, and NDB navaids formatted for direct use in Aurora sector files. " +
                              "Each data type downloads as a separate file.",
                Font        = new Font("Segoe UI", 10f),
                ForeColor   = Color.FromArgb(55, 65, 81),
                Dock        = DockStyle.Top,
                Height      = 84,
                AutoSize    = false,
                TextAlign   = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };

            this.apAiracLabel = new Label
            {
                Text      = SplashForm.AiracDaysLeft >= 0 ? $"AIRAC  {SplashForm.AiracCycle}" : "",
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(13, 71, 161),
                TextAlign = ContentAlignment.BottomRight,
                Size      = new Size(180, 22),
                AutoSize  = false,
            };

            // Assembly (last added = topmost with DockStyle.Top)
            apHeaderPanel.Controls.Add(apDescLabel);
            apHeaderPanel.Controls.Add(apGap1);
            apHeaderPanel.Controls.Add(apSubLabel);
            apHeaderPanel.Controls.Add(apGapTitle);
            apHeaderPanel.Controls.Add(this.apTitleLabel);
            apHeaderPanel.Controls.Add(this.apAiracLabel);
            apHeaderPanel.Resize += (s, e) =>
            {
                int r = apHeaderPanel.ClientSize.Width - ApPadH;
                apAiracLabel.Location = new Point(r - 180, 24);
            };

            // ── Search bar ────────────────────────────────────────────────
            this.apSearchPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 66,
                BackColor = Color.White,
            };

            this.apRegionWrapper = new Panel
            {
                BackColor = Color.White,
                Height    = 42,
                Cursor    = Cursors.IBeam,
            };

            this.apRegionBox = new TextBox
            {
                Font            = new Font("Segoe UI", 11f),
                PlaceholderText = "Airport ICAO or FIR prefix  (e.g. VABB, VTBS  or  VA, VT)",
                BorderStyle     = BorderStyle.None,
                BackColor       = Color.White,
                CharacterCasing = CharacterCasing.Upper,
            };

            apRegionWrapper.Controls.Add(this.apRegionBox);
            apRegionWrapper.Click += (s, e) => apRegionBox.Focus();
            apRegionBox.Enter += (s, e) => apRegionWrapper.Invalidate();
            apRegionBox.Leave += (s, e) => apRegionWrapper.Invalidate();
            apRegionWrapper.Paint += (s, e) =>
            {
                bool focused = apRegionBox.Focused;
                var  g       = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var borderColor = focused
                    ? Color.FromArgb(5, 150, 105)
                    : Color.FromArgb(203, 213, 225);
                using var path = RoundedRect(
                    new Rectangle(0, 0, apRegionWrapper.Width - 1, apRegionWrapper.Height - 1), 6);
                using var pen = new Pen(borderColor, focused ? 2f : 1f);
                g.DrawPath(pen, path);
            };
            apRegionWrapper.Resize += (s, e) =>
            {
                int pad = 12;
                apRegionBox.SetBounds(pad, (apRegionWrapper.Height - apRegionBox.Height) / 2,
                    apRegionWrapper.Width - pad * 2, apRegionBox.Height);
            };

            this.apSearchButton = new Button
            {
                Text      = "Fetch Data",
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                BackColor = Color.FromArgb(5, 150, 105),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height    = 42,
                Width     = 120,
                Cursor    = Cursors.Hand,
            };
            this.apSearchButton.FlatAppearance.BorderSize = 0;
            this.apSearchButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(4, 120, 87);
            this.apSearchButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(6, 95, 70);

            this.apSearchPanel.Controls.Add(this.apRegionWrapper);
            this.apSearchPanel.Controls.Add(this.apSearchButton);
            this.apSearchPanel.Resize += (s, e) =>
            {
                int avail = apSearchPanel.ClientSize.Width - ApPadH * 2;
                apRegionWrapper.Location    = new Point(ApPadH, 12);
                apRegionWrapper.Width       = Math.Max(80, avail - 128);
                apSearchButton.Location     = new Point(ApPadH + avail - 120, 12);
            };

            // ── Progress strip ────────────────────────────────────────────
            this.apProgressPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 28,
                BackColor = Color.White,
            };

            this.apProgressBar = new ProgressBar
            {
                Style   = ProgressBarStyle.Continuous,
                Location = new Point(0, 0),
                Size    = new Size(800, 5),
                Minimum = 0,
                Maximum = 100,
                Value   = 0,
            };

            this.apStatusLabel = new Label
            {
                Text      = "Enter a 4-letter airport ICAO (full data) or 2-letter FIR prefix (navaids only)",
                Font      = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(120, 140, 165),
                Location  = new Point(10, 10),
                Size      = new Size(700, 16),
            };

            this.apProgressPanel.Controls.Add(this.apProgressBar);
            this.apProgressPanel.Controls.Add(this.apStatusLabel);
            this.apProgressPanel.Resize += (s, e) =>
            {
                apProgressBar.Width = apProgressPanel.ClientSize.Width;
                apStatusLabel.Width = apProgressPanel.ClientSize.Width - 20;
            };

            // ── Log output ────────────────────────────────────────────────
            this.apLogBox = new RichTextBox
            {
                Dock        = DockStyle.Fill,
                ReadOnly    = true,
                ScrollBars  = RichTextBoxScrollBars.Vertical,
                BackColor   = Color.FromArgb(250, 251, 253),
                ForeColor   = Color.FromArgb(30, 40, 65),
                Font        = new Font("Consolas", 9f),
                BorderStyle = BorderStyle.None,
            };
            this.apLogBox.LinkClicked += (s, e) =>
                System.Diagnostics.Process.Start(e.LinkText);

            var apLogWrapper = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(250, 251, 253),
                Padding   = new Padding(ApPadH, 12, ApPadH, 12),
            };
            apLogWrapper.Controls.Add(this.apLogBox);

            // ── Download bar (2 rows of buttons) ─────────────────────────
            this.apDownloadPanel = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 130,
                BackColor = Color.White,
            };
            this.apDownloadPanel.Paint += PaintTopBorder;

            this.apDownloadAirportBtn = MakeDownloadButton("⬇  Airport Data (.ap)",  Color.FromArgb(5,   150, 105));
            this.apDownloadRunwayBtn  = MakeDownloadButton("⬇  Runway Data (.rw)",   Color.FromArgb(37,   99, 235));
            this.apDownloadFreqBtn    = MakeDownloadButton("⬇  Frequencies (.atc)",  Color.FromArgb(147,  51, 234));
            this.apDownloadVorBtn     = MakeDownloadButton("⬇  VOR/DME Data (.vor)", Color.FromArgb(217, 119,   6));
            this.apDownloadNdbBtn     = MakeDownloadButton("⬇  NDB Data (.ndb)",     Color.FromArgb(225,  29,  72));

            // Disabled until a successful fetch (MakeDownloadButton's EnabledChanged handles dimming)
            apDownloadAirportBtn.Enabled = apDownloadRunwayBtn.Enabled =
            apDownloadFreqBtn.Enabled   = apDownloadVorBtn.Enabled = apDownloadNdbBtn.Enabled = false;

            this.apDownloadPanel.Controls.Add(this.apDownloadAirportBtn);
            this.apDownloadPanel.Controls.Add(this.apDownloadRunwayBtn);
            this.apDownloadPanel.Controls.Add(this.apDownloadFreqBtn);
            this.apDownloadPanel.Controls.Add(this.apDownloadVorBtn);
            this.apDownloadPanel.Controls.Add(this.apDownloadNdbBtn);

            this.apDownloadPanel.Resize += (s, e) =>
            {
                int total = apDownloadPanel.ClientSize.Width - 48;
                int gap   = 8;
                // Row 1: Airport · Runway · Frequency  (3 equal)
                int w3 = (total - gap * 2) / 3;
                apDownloadAirportBtn.SetBounds(24,                    14, w3, 44);
                apDownloadRunwayBtn .SetBounds(24 + (w3 + gap),      14, w3, 44);
                apDownloadFreqBtn   .SetBounds(24 + (w3 + gap) * 2,  14, w3, 44);
                // Row 2: VOR/DME · NDB  (2 equal)
                int w2 = (total - gap) / 2;
                apDownloadVorBtn.SetBounds(24,            66, w2, 44);
                apDownloadNdbBtn.SetBounds(24 + w2 + gap, 66, w2, 44);
            };

            // Wire handlers (logic lives in MainForm.Airport.cs)
            this.apSearchButton.Click       += ApSearch_Click;
            this.apDownloadAirportBtn.Click += (s, e) => ApSave(_apAirportOutput, "Airport Data Files (*.ap)|*.ap",  "ap");
            this.apDownloadRunwayBtn.Click  += (s, e) => ApSave(_apRunwayOutput,  "Runway Data Files (*.rw)|*.rw",   "rw");
            this.apDownloadFreqBtn.Click    += (s, e) => ApSave(_apFreqOutput,    "Frequency Files (*.atc)|*.atc",   "atc");
            this.apDownloadVorBtn.Click     += (s, e) => ApSave(_apVorOutput,     "VOR/DME Files (*.vor)|*.vor",     "vor");
            this.apDownloadNdbBtn.Click     += (s, e) => ApSave(_apNdbOutput,     "NDB Files (*.ndb)|*.ndb",         "ndb");

            // Assemble (bottom first, then Fill, then Top controls)
            this.airportPage.Controls.Add(apLogWrapper);           // Fill
            this.airportPage.Controls.Add(this.apDownloadPanel);   // Bottom
            this.airportPage.Controls.Add(this.apProgressPanel);   // Top (inner)
            this.airportPage.Controls.Add(this.apSearchPanel);     // Top
            this.airportPage.Controls.Add(this.apHeaderPanel);     // Top (outermost)
        }

        // ─────────────────────────────────────────────────────────────────
        //  FIR DATA PAGE
        // ─────────────────────────────────────────────────────────────────
        private void BuildFirPage()
        {
            const int FirPadH   = 36;
            const int FirCardH  = 84;
            const int FirCardGap = 10;
            const int FirSecLblH = 22;

            this.firPage = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 248, 255),
                Visible   = false,
            };

            // ── Header ─────────────────────────────────────────────────────
            this.firHeaderPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 236,
                BackColor = Color.White,
                Padding   = new Padding(FirPadH, 20, FirPadH, 20),
            };

            this.firTitleLabel = new Label
            {
                Text        = "FIR & Airspace Data",
                Font        = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor   = Color.FromArgb(17, 24, 39),
                Dock        = DockStyle.Top,
                Height      = 52,
                AutoSize    = false,
                TextAlign   = ContentAlignment.MiddleLeft,
                UseMnemonic = false,
            };

            var firGapTitle = new Panel { Dock = DockStyle.Top, Height = 10, BackColor = Color.Transparent };

            var firSubLabel = new Label
            {
                Text        = "Flight Information Regions · Controlled & Restricted Airspace",
                Font        = new Font("Segoe UI", 10.5f),
                ForeColor   = Color.FromArgb(107, 114, 128),
                Dock        = DockStyle.Top,
                Height      = 26,
                AutoSize    = false,
                TextAlign   = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };

            var firGap1 = new Panel { Dock = DockStyle.Top, Height = 24, BackColor = Color.Transparent };

            var firDescLabel = new Label
            {
                Text        = "Enter a FIR identifier to fetch its boundary and all contained airspace shapes. " +
                              "Toggle which airspace types to include - FIR/UIR boundaries, Terminal Maneuvering " +
                              "Areas, Control Zones, Control Areas, and Restricted/Danger/Prohibited areas. " +
                              "Each type downloads as a separate .artcc file for use in Aurora sector files.",
                Font        = new Font("Segoe UI", 10f),
                ForeColor   = Color.FromArgb(55, 65, 81),
                Dock        = DockStyle.Top,
                Height      = 84,
                AutoSize    = false,
                TextAlign   = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };

            this.firAiracCycleLabel = new Label
            {
                Text      = SplashForm.AiracDaysLeft >= 0 ? $"AIRAC  {SplashForm.AiracCycle}" : "",
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(13, 71, 161),
                TextAlign = ContentAlignment.BottomRight,
                Size      = new Size(180, 22),
                AutoSize  = false,
            };
            this.firAiracDaysLabel = new Label
            {
                Text      = SplashForm.AiracDaysLeft >= 0 ? $"{SplashForm.AiracDaysLeft} days left" : "",
                Font      = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(100, 120, 150),
                TextAlign = ContentAlignment.TopRight,
                Size      = new Size(180, 18),
                AutoSize  = false,
            };

            firHeaderPanel.Controls.Add(firDescLabel);
            firHeaderPanel.Controls.Add(firGap1);
            firHeaderPanel.Controls.Add(firSubLabel);
            firHeaderPanel.Controls.Add(firGapTitle);
            firHeaderPanel.Controls.Add(this.firTitleLabel);
            firHeaderPanel.Controls.Add(this.firAiracCycleLabel);
            firHeaderPanel.Controls.Add(this.firAiracDaysLabel);
            firHeaderPanel.Resize += (s, e) =>
            {
                int r = firHeaderPanel.ClientSize.Width - FirPadH;
                firAiracCycleLabel.Location = new Point(r - 180, 24);
                firAiracDaysLabel.Location  = new Point(r - 180, 48);
            };

            // ── Search bar ─────────────────────────────────────────────────
            this.firSearchPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 66,
                BackColor = Color.White,
            };

            this.firSearchWrapper = new Panel
            {
                BackColor = Color.White,
                Height    = 42,
                Cursor    = Cursors.IBeam,
            };

            this.firSearchBox = new TextBox
            {
                Font            = new Font("Segoe UI", 11f),
                PlaceholderText = "Enter FIR identifier  (e.g. VOCF, VABF, VTBB, WSSS)",
                BorderStyle     = BorderStyle.None,
                BackColor       = Color.White,
                CharacterCasing = CharacterCasing.Upper,
            };

            firSearchWrapper.Controls.Add(this.firSearchBox);
            firSearchWrapper.Click += (s, e) => firSearchBox.Focus();
            firSearchBox.Enter += (s, e) => firSearchWrapper.Invalidate();
            firSearchBox.Leave += (s, e) => firSearchWrapper.Invalidate();
            firSearchWrapper.Paint += (s, e) =>
            {
                bool focused = firSearchBox.Focused;
                var  g       = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var borderColor = focused
                    ? Color.FromArgb(217, 119, 6)
                    : Color.FromArgb(203, 213, 225);
                using var path = RoundedRect(
                    new Rectangle(0, 0, firSearchWrapper.Width - 1, firSearchWrapper.Height - 1), 6);
                using var pen = new Pen(borderColor, focused ? 2f : 1f);
                g.DrawPath(pen, path);
            };
            firSearchWrapper.Resize += (s, e) =>
            {
                int pad = 12;
                firSearchBox.SetBounds(pad, (firSearchWrapper.Height - firSearchBox.Height) / 2,
                    firSearchWrapper.Width - pad * 2, firSearchBox.Height);
            };

            this.firSearchButton = new Button
            {
                Text      = "Fetch",
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                BackColor = Color.FromArgb(217, 119, 6),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height    = 42,
                Width     = 100,
                Cursor    = Cursors.Hand,
            };
            this.firSearchButton.FlatAppearance.BorderSize = 0;
            this.firSearchButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(180, 95, 0);
            this.firSearchButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(146, 76, 0);

            firSearchPanel.Controls.Add(this.firSearchWrapper);
            firSearchPanel.Controls.Add(this.firSearchButton);
            firSearchPanel.Resize += (s, e) =>
            {
                int avail = firSearchPanel.ClientSize.Width - FirPadH * 2;
                firSearchWrapper.Location = new Point(FirPadH, 12);
                firSearchWrapper.Width    = Math.Max(80, avail - 108);
                firSearchButton.Location  = new Point(FirPadH + avail - 100, 12);
            };

            // ── Toggle cards ───────────────────────────────────────────────
            // Hidden checkboxes - state holders read by logic
            this.chkFirBoundary  = new CheckBox { Checked = true,  Visible = false };
            this.chkFirUir       = new CheckBox { Checked = true,  Visible = false };
            this.chkFirTma       = new CheckBox { Checked = true,  Visible = false };
            this.chkFirCtr       = new CheckBox { Checked = true,  Visible = false };
            this.chkFirCta       = new CheckBox { Checked = true,  Visible = false };
            this.chkFirRestricted = new CheckBox { Checked = true, Visible = false };
            this.chkFirDanger    = new CheckBox { Checked = true,  Visible = false };
            this.chkFirProhibited = new CheckBox { Checked = true, Visible = false };

            this.firOptionsPanel = new Panel
            {
                Dock      = DockStyle.Top,
                BackColor = Color.FromArgb(248, 250, 252),
            };

            // Section labels
            var firSec1Lbl = new Label
            {
                Text      = "FIR / UIR & Controlled Airspace",
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize  = false,
                Height    = FirSecLblH,
            };
            var firSec2Lbl = new Label
            {
                Text      = "Restricted Airspace",
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize  = false,
                Height    = FirSecLblH,
            };

            // Toggle card factory (local function)
            Panel MakeFirCard(CheckBox chk, string icon, string title, string desc, Color accent)
            {
                var activeBg = Color.FromArgb(
                    Math.Min(255, accent.R / 4 + 200),
                    Math.Min(255, accent.G / 4 + 200),
                    Math.Min(255, accent.B / 4 + 200));

                var card = new Panel { BackColor = Color.White, Cursor = Cursors.Hand };

                var iconBox = new Panel { Size = new Size(36, 36), Location = new Point(12, (FirCardH - 36) / 2), Cursor = Cursors.Hand };
                iconBox.Region = new Region(RoundedRect(new Rectangle(0, 0, 36, 36), 8));

                var iconLbl = new Label
                {
                    Text      = icon,
                    Font      = new Font("Segoe UI", 14f),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock      = DockStyle.Fill,
                    Cursor    = Cursors.Hand,
                };
                iconBox.Controls.Add(iconLbl);

                var titleLbl = new Label
                {
                    Text      = title,
                    Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                    Location  = new Point(57, 18),
                    AutoSize  = false,
                    Height    = 20,
                    Cursor    = Cursors.Hand,
                };
                var descLbl = new Label
                {
                    Text      = desc,
                    Font      = new Font("Segoe UI", 8f),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    Location  = new Point(57, 40),
                    AutoSize  = false,
                    Height    = 30,
                    Cursor    = Cursors.Hand,
                };

                card.Controls.Add(iconBox);
                card.Controls.Add(titleLbl);
                card.Controls.Add(descLbl);

                void Repaint()
                {
                    bool on = chk.Checked;
                    card.BackColor     = on ? Color.White : Color.FromArgb(248, 250, 252);
                    iconBox.BackColor  = on ? activeBg : Color.FromArgb(241, 245, 249);
                    iconLbl.ForeColor  = on ? accent : Color.FromArgb(148, 163, 184);
                    titleLbl.ForeColor = on ? Color.FromArgb(17, 24, 39) : Color.FromArgb(148, 163, 184);
                    card.Invalidate();
                }

                card.Paint += (s, e) =>
                {
                    bool on = chk.Checked;
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    using var borderPen = new Pen(on ? accent : Color.FromArgb(226, 232, 240), 1);
                    e.Graphics.DrawRectangle(borderPen, 0, 0, card.Width - 1, card.Height - 1);
                    using var barBrush = new SolidBrush(on ? accent : Color.FromArgb(203, 213, 225));
                    e.Graphics.FillRectangle(barBrush, 0, 0, 3, card.Height);
                };

                card.Resize += (s, e) =>
                {
                    titleLbl.Width = card.Width - 60;
                    descLbl.Width  = card.Width - 60;
                };

                void Toggle(object s2, EventArgs e2) { chk.Checked = !chk.Checked; Repaint(); }
                card.Click    += Toggle;
                iconBox.Click += Toggle;
                iconLbl.Click += Toggle;
                titleLbl.Click += Toggle;
                descLbl.Click  += Toggle;

                Repaint();
                return card;
            }

            // Row 1 cards
            var cFirBoundary  = MakeFirCard(chkFirBoundary,  "🗺", "FIR Boundary",   "FIR outer boundary",       Color.FromArgb(37,  99, 235));
            var cFirUir       = MakeFirCard(chkFirUir,       "✈", "UIR Boundary",   "Upper info region",        Color.FromArgb(99, 102, 241));
            var cFirTma       = MakeFirCard(chkFirTma,       "📡", "TMA",            "Terminal Maneuvering Area", Color.FromArgb(5,  150, 105));
            var cFirCtr       = MakeFirCard(chkFirCtr,       "🔵", "CTR / CTZ",      "Control Zone",             Color.FromArgb(217, 119,  6));

            // Row 2 cards
            var cFirCta       = MakeFirCard(chkFirCta,       "🔷", "CTA",            "Control Area",             Color.FromArgb(6,  182, 212));
            var cFirRestricted = MakeFirCard(chkFirRestricted,"🚫", "Restricted",     "Restricted areas",         Color.FromArgb(220, 38,  38));
            var cFirDanger    = MakeFirCard(chkFirDanger,    "⚠", "Danger",         "Danger areas",             Color.FromArgb(245, 158, 11));
            var cFirProhibited = MakeFirCard(chkFirProhibited,"⛔", "Prohibited",     "Prohibited areas",         Color.FromArgb(225,  29, 72));

            var firRow1Cards = new[] { cFirBoundary, cFirUir, cFirTma, cFirCtr };
            var firRow2Cards = new[] { cFirCta, cFirRestricted, cFirDanger, cFirProhibited };

            foreach (var c in firRow1Cards) firOptionsPanel.Controls.Add(c);
            foreach (var c in firRow2Cards) firOptionsPanel.Controls.Add(c);
            firOptionsPanel.Controls.Add(firSec1Lbl);
            firOptionsPanel.Controls.Add(firSec2Lbl);

            void LayoutFirOptions()
            {
                int avail = firOptionsPanel.ClientSize.Width - FirPadH * 2;
                int cardW = Math.Max(100, (avail - FirCardGap * 3) / 4);
                int y0 = 14;
                firSec1Lbl.SetBounds(FirPadH, y0, avail, FirSecLblH);
                int y1 = y0 + FirSecLblH + 6;
                for (int i = 0; i < firRow1Cards.Length; i++)
                    firRow1Cards[i].SetBounds(FirPadH + i * (cardW + FirCardGap), y1, cardW, FirCardH);
                int y2 = y1 + FirCardH + 14;
                firSec2Lbl.SetBounds(FirPadH, y2, avail, FirSecLblH);
                int y3 = y2 + FirSecLblH + 6;
                for (int i = 0; i < firRow2Cards.Length; i++)
                    firRow2Cards[i].SetBounds(FirPadH + i * (cardW + FirCardGap), y3, cardW, FirCardH);
                firOptionsPanel.Height = y3 + FirCardH + 14;
            }
            firOptionsPanel.Resize += (s, e) => LayoutFirOptions();

            // ── Progress strip ─────────────────────────────────────────────
            this.firProgressPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 36,
                BackColor = Color.White,
            };

            this.firProgressBar = new ProgressBar
            {
                Height   = 5,
                Minimum  = 0,
                Maximum  = 100,
                Value    = 0,
                Style    = ProgressBarStyle.Continuous,
                Location = new Point(0, 0),
                Size     = new Size(800, 5),
            };
            this.firStatusLabel = new Label
            {
                Text      = "Ready",
                Font      = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(120, 135, 160),
                Location  = new Point(FirPadH, 10),
                AutoSize  = true,
            };
            firProgressPanel.Controls.Add(this.firProgressBar);
            firProgressPanel.Controls.Add(this.firStatusLabel);
            firProgressPanel.Resize += (s, e) =>
            {
                firProgressBar.Width = firProgressPanel.ClientSize.Width;
                firStatusLabel.Width = firProgressPanel.ClientSize.Width - FirPadH * 2;
            };

            // ── Log area ───────────────────────────────────────────────────
            this.firLogBox = new RichTextBox
            {
                Dock        = DockStyle.Fill,
                Font        = new Font("Consolas", 9f),
                BackColor   = Color.FromArgb(250, 251, 253),
                ForeColor   = Color.FromArgb(30, 40, 65),
                BorderStyle = BorderStyle.None,
                ReadOnly    = true,
                ScrollBars  = RichTextBoxScrollBars.Vertical,
            };

            var firLogWrapper = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(250, 251, 253),
                Padding   = new Padding(FirPadH, 12, FirPadH, 12),
            };
            firLogWrapper.Controls.Add(this.firLogBox);

            // ── Download bar ───────────────────────────────────────────────
            this.firDownloadPanel = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 170,
                BackColor = Color.White,
            };
            this.firDownloadPanel.Paint += PaintTopBorder;

            // ── Mode toggle: Separate Files | Combined File ────────────────
            this.firRadioSeparate = new RadioButton
            {
                Text      = "Separate Files",
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Checked   = true,
                AutoSize  = false,
                Size      = new Size(160, 26),
                Location  = new Point(FirPadH, 16),
                Cursor    = Cursors.Hand,
                ForeColor = Color.FromArgb(30, 64, 175),
            };
            this.firRadioCombined = new RadioButton
            {
                Text      = "Combined File",
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                AutoSize  = false,
                Size      = new Size(160, 26),
                Location  = new Point(FirPadH + 170, 16),
                Cursor    = Cursors.Hand,
                ForeColor = Color.FromArgb(30, 64, 175),
            };

            // ── Separate panel (8 buttons, 2 rows) ────────────────────────
            this.firSeparatePanel = new Panel
            {
                BackColor = Color.Transparent,
                Dock      = DockStyle.None,
            };

            this.firDlFirBtn        = MakeDownloadButton("⬇  FIR Boundary",  Color.FromArgb(37,  99, 235));
            this.firDlUirBtn        = MakeDownloadButton("⬇  UIR Boundary",  Color.FromArgb(99, 102, 241));
            this.firDlTmaBtn        = MakeDownloadButton("⬇  TMA",           Color.FromArgb(5,  150, 105));
            this.firDlCtrBtn        = MakeDownloadButton("⬇  CTR / CTZ",     Color.FromArgb(217, 119,  6));
            this.firDlCtaBtn        = MakeDownloadButton("⬇  CTA",           Color.FromArgb(6,  182, 212));
            this.firDlRestrictedBtn = MakeDownloadButton("⬇  Restricted",    Color.FromArgb(220, 38,  38));
            this.firDlDangerBtn     = MakeDownloadButton("⬇  Danger",        Color.FromArgb(245, 158, 11));
            this.firDlProhibitedBtn = MakeDownloadButton("⬇  Prohibited",    Color.FromArgb(225,  29, 72));

            firDlFirBtn.Enabled = firDlUirBtn.Enabled = firDlTmaBtn.Enabled = firDlCtrBtn.Enabled =
            firDlCtaBtn.Enabled = firDlRestrictedBtn.Enabled = firDlDangerBtn.Enabled = firDlProhibitedBtn.Enabled = false;

            firSeparatePanel.Controls.AddRange(new Control[] {
                firDlFirBtn, firDlUirBtn, firDlTmaBtn, firDlCtrBtn,
                firDlCtaBtn, firDlRestrictedBtn, firDlDangerBtn, firDlProhibitedBtn,
            });

            // ── Combined panel (1 button) ──────────────────────────────────
            this.firCombinedPanel = new Panel
            {
                BackColor = Color.Transparent,
                Dock      = DockStyle.None,
                Visible   = false,
            };

            this.firDlCombinedBtn = MakeDownloadButton("⬇  Download Combined Airspace File (.artcc)",
                Color.FromArgb(37, 99, 235));
            this.firDlCombinedBtn.Enabled = false;
            firCombinedPanel.Controls.Add(firDlCombinedBtn);

            firDownloadPanel.Controls.AddRange(new Control[] {
                firRadioSeparate, firRadioCombined,
                firSeparatePanel, firCombinedPanel,
            });

            // Layout helper
            void LayoutFirDownload()
            {
                int total  = firDownloadPanel.ClientSize.Width - FirPadH * 2;
                int gap    = 8;
                int panelY = 54;   // radio row height (16+26) + 12px breathing room

                // Separate panel: 2 rows of 4
                firSeparatePanel.SetBounds(0, panelY, firDownloadPanel.Width, 100);
                int w4 = Math.Max(60, (total - gap * 3) / 4);
                firDlFirBtn  .SetBounds(FirPadH,                      0, w4, 44);
                firDlUirBtn  .SetBounds(FirPadH + (w4 + gap),         0, w4, 44);
                firDlTmaBtn  .SetBounds(FirPadH + (w4 + gap) * 2,     0, w4, 44);
                firDlCtrBtn  .SetBounds(FirPadH + (w4 + gap) * 3,     0, w4, 44);
                firDlCtaBtn       .SetBounds(FirPadH,                 52, w4, 44);
                firDlRestrictedBtn.SetBounds(FirPadH + (w4 + gap),    52, w4, 44);
                firDlDangerBtn    .SetBounds(FirPadH + (w4 + gap)*2,  52, w4, 44);
                firDlProhibitedBtn.SetBounds(FirPadH + (w4 + gap)*3,  52, w4, 44);

                // Combined panel: 1 full-width button
                firCombinedPanel.SetBounds(0, panelY, firDownloadPanel.Width, 50);
                firDlCombinedBtn.SetBounds(FirPadH, 0, total, 44);
            }

            firDownloadPanel.Resize += (s, e) => LayoutFirDownload();

            // Toggle between panels on radio change
            firRadioSeparate.CheckedChanged += (s, e) =>
            {
                firSeparatePanel.Visible = firRadioSeparate.Checked;
                firCombinedPanel.Visible = !firRadioSeparate.Checked;
                firDownloadPanel.Height  = firRadioSeparate.Checked ? 170 : 110;
            };

            // ── Wire events ────────────────────────────────────────────────
            firSearchButton.Click += FirSearch_Click;
            firSearchBox.KeyDown  += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { FirSearch_Click(s, e); e.SuppressKeyPress = true; }
            };
            firDlFirBtn.Click        += (s, e) => FirSave(_firOutput,          "FIR_Boundary");
            firDlUirBtn.Click        += (s, e) => FirSave(_firUirOutput,        "UIR_Boundary");
            firDlTmaBtn.Click        += (s, e) => FirSave(_firTmaOutput,        "TMA");
            firDlCtrBtn.Click        += (s, e) => FirSave(_firCtrOutput,        "CTR");
            firDlCtaBtn.Click        += (s, e) => FirSave(_firCtaOutput,        "CTA");
            firDlRestrictedBtn.Click += (s, e) => FirSave(_firRestrictedOutput, "Restricted");
            firDlDangerBtn.Click     += (s, e) => FirSave(_firDangerOutput,     "Danger");
            firDlProhibitedBtn.Click += (s, e) => FirSave(_firProhibitedOutput, "Prohibited");
            firDlCombinedBtn.Click   += (s, e) => FirSaveCombined();

            // Assemble (bottom first, Fill, then Top outermost→innermost)
            firPage.Controls.Add(firLogWrapper);
            firPage.Controls.Add(this.firDownloadPanel);
            firPage.Controls.Add(this.firProgressPanel);
            firPage.Controls.Add(this.firOptionsPanel);
            firPage.Controls.Add(this.firSearchPanel);
            firPage.Controls.Add(this.firHeaderPanel);
        }

        // ─────────────────────────────────────────────────────────────────
        //  GROUND LAYOUT PAGE  - embedded KML import
        // ─────────────────────────────────────────────────────────────────
        private void BuildGroundLayoutPage()
        {
            const int GrPadH = 36;

            this.groundPage = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 248, 255),
                Visible   = false,
            };

            // ── Header ─────────────────────────────────────────────────────
            this.grHeaderPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 412,
                BackColor = Color.White,
                Padding   = new Padding(GrPadH, 20, GrPadH, 20),
            };

            this.grTitleLabel = new Label
            {
                Text        = "Ground Layout",
                Font        = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor   = Color.FromArgb(17, 24, 39),
                Dock        = DockStyle.Top,
                Height      = 52,
                AutoSize    = false,
                TextAlign   = ContentAlignment.MiddleLeft,
                UseMnemonic = false,
            };

            var grGapTitle = new Panel { Dock = DockStyle.Top, Height = 10, BackColor = Color.Transparent };

            var grSubLabel = new Label
            {
                Text        = "KML / KMZ Import  ·  Aurora .tfl / .geo / .txi / .rw",
                Font        = new Font("Segoe UI", 10.5f),
                ForeColor   = Color.FromArgb(107, 114, 128),
                Dock        = DockStyle.Top,
                Height      = 26,
                AutoSize    = false,
                TextAlign   = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };

            var grGap1 = new Panel { Dock = DockStyle.Top, Height = 24, BackColor = Color.Transparent };

            var grDescLabel = new Label
            {
                Text        = "Import a KML or KMZ file exported from Google Earth or QGIS to generate " +
                              "Aurora ground layout files. Geometry type is detected automatically - " +
                              "polygons → .tfl (filled areas), line strings → .geo (centre-lines), " +
                              "points → .txi or .rw depending on the placemark name.\n\n" +
                              "Point naming rules:  name a point  \"Runway 09\",  \"Runway 27L\",  or  \"R09L\" " +
                              "(starts with \"Runway\" or \"R\" followed by a digit) to mark a runway endpoint. " +
                              "Opposite pairs are linked automatically - Runway 09 pairs with Runway 27. " +
                              "Any other point name (e.g. A, B, Gate 12, Stand 4) becomes a taxi label in the .txi file.\n\n" +
                              "Polygon and line names are written as comments only. " +
                              "The colours you set in Google Earth or QGIS are carried directly into Aurora - " +
                              "fill colour, line colour, and opacity all reflect exactly as styled, so what you " +
                              "see in Google Earth is what you get in Aurora.",
                Font        = new Font("Segoe UI", 10f),
                ForeColor   = Color.FromArgb(55, 65, 81),
                Dock        = DockStyle.Top,
                Height      = 260,
                AutoSize    = false,
                TextAlign   = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };

            grHeaderPanel.Controls.Add(grDescLabel);
            grHeaderPanel.Controls.Add(grGap1);
            grHeaderPanel.Controls.Add(grSubLabel);
            grHeaderPanel.Controls.Add(grGapTitle);
            grHeaderPanel.Controls.Add(this.grTitleLabel);

            // ── Import bar ─────────────────────────────────────────────────
            this.grImportPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 66,
                BackColor = Color.White,
            };

            var grFileWrapper = new Panel
            {
                BackColor = Color.White,
                Height    = 42,
                Cursor    = Cursors.Default,
            };

            this.grFileLabel = new Label
            {
                Text      = "No file selected - click Import to browse for a KML or KMZ file",
                Font      = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(148, 163, 184),
                Dock      = DockStyle.None,
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleLeft,
            };
            grFileWrapper.Controls.Add(this.grFileLabel);

            grFileWrapper.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var path = RoundedRect(
                    new Rectangle(0, 0, grFileWrapper.Width - 1, grFileWrapper.Height - 1), 6);
                using var pen = new Pen(Color.FromArgb(203, 213, 225), 1f);
                g.DrawPath(pen, path);
            };
            grFileWrapper.Resize += (s, e) =>
            {
                int pad = 12;
                this.grFileLabel.SetBounds(pad, 0, grFileWrapper.Width - pad * 2, grFileWrapper.Height);
            };

            this.grImportButton = new Button
            {
                Text      = "📂  Import KML / KMZ…",
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                BackColor = Color.FromArgb(225, 29, 72),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height    = 42,
                Width     = 190,
                Cursor    = Cursors.Hand,
            };
            this.grImportButton.FlatAppearance.BorderSize            = 0;
            this.grImportButton.FlatAppearance.MouseOverBackColor    = Color.FromArgb(190, 18, 60);
            this.grImportButton.FlatAppearance.MouseDownBackColor    = Color.FromArgb(159, 18, 57);
            this.grImportButton.UseVisualStyleBackColor              = false;

            grImportPanel.Controls.Add(grFileWrapper);
            grImportPanel.Controls.Add(this.grImportButton);
            grImportPanel.Resize += (s, e) =>
            {
                int avail = grImportPanel.ClientSize.Width - GrPadH * 2;
                grFileWrapper.Location   = new Point(GrPadH, 12);
                grFileWrapper.Width      = Math.Max(80, avail - 198);
                grImportButton.Location  = new Point(GrPadH + avail - 190, 12);
            };

            // ── Progress strip ─────────────────────────────────────────────
            this.grProgressPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 28,
                BackColor = Color.White,
            };

            this.grProgressBar = new ProgressBar
            {
                Style    = ProgressBarStyle.Continuous,
                Location = new Point(0, 0),
                Size     = new Size(800, 5),
                Minimum  = 0,
                Maximum  = 100,
                Value    = 0,
            };

            this.grStatusLabel = new Label
            {
                Text      = "Ready - select a KML or KMZ file to begin",
                Font      = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(120, 140, 165),
                Location  = new Point(10, 10),
                Size      = new Size(700, 16),
            };

            grProgressPanel.Controls.Add(this.grProgressBar);
            grProgressPanel.Controls.Add(this.grStatusLabel);
            grProgressPanel.Resize += (s, e) =>
            {
                grProgressBar.Width  = grProgressPanel.ClientSize.Width;
                grStatusLabel.Width  = grProgressPanel.ClientSize.Width - 20;
            };

            // ── Log area ───────────────────────────────────────────────────
            this.grLogBox = new RichTextBox
            {
                Dock        = DockStyle.Fill,
                Font        = new Font("Consolas", 9f),
                BackColor   = Color.FromArgb(250, 251, 253),
                ForeColor   = Color.FromArgb(30, 40, 65),
                BorderStyle = BorderStyle.None,
                ReadOnly    = true,
                ScrollBars  = RichTextBoxScrollBars.Vertical,
            };

            var grLogWrapper = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(250, 251, 253),
                Padding   = new Padding(GrPadH, 12, GrPadH, 12),
            };
            grLogWrapper.Controls.Add(this.grLogBox);

            // ── Download bar - 4 buttons: TFL, GEO, TXI, RW ───────────────
            this.grDownloadPanel = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 72,
                BackColor = Color.White,
            };
            this.grDownloadPanel.Paint += PaintTopBorder;

            this.grDownloadTflBtn = MakeDownloadButton("⬇  Polygons (.tfl)",    Color.FromArgb(225,  29,  72));
            this.grDownloadGeoBtn = MakeDownloadButton("⬇  Lines (.geo)",        Color.FromArgb( 37,  99, 235));
            this.grDownloadTxiBtn = MakeDownloadButton("⬇  Taxi Labels (.txi)", Color.FromArgb(  5, 150, 105));
            this.grDownloadRwBtn  = MakeDownloadButton("⬇  Runways (.rw)",       Color.FromArgb(217, 119,   6));

            grDownloadTflBtn.Enabled = grDownloadGeoBtn.Enabled =
            grDownloadTxiBtn.Enabled = grDownloadRwBtn.Enabled  = false;

            this.grDownloadPanel.Controls.Add(this.grDownloadTflBtn);
            this.grDownloadPanel.Controls.Add(this.grDownloadGeoBtn);
            this.grDownloadPanel.Controls.Add(this.grDownloadTxiBtn);
            this.grDownloadPanel.Controls.Add(this.grDownloadRwBtn);
            this.grDownloadPanel.Resize += (s, e) =>
            {
                int total = grDownloadPanel.ClientSize.Width - 48;
                int gap   = 8;
                int w4    = (total - gap * 3) / 4;
                grDownloadTflBtn.SetBounds(24,                   14, w4, 44);
                grDownloadGeoBtn.SetBounds(24 + (w4 + gap),     14, w4, 44);
                grDownloadTxiBtn.SetBounds(24 + (w4 + gap) * 2, 14, w4, 44);
                grDownloadRwBtn .SetBounds(24 + (w4 + gap) * 3, 14, w4, 44);
            };

            // Wire events
            this.grImportButton.Click  += GrImport_Click;
            this.grDownloadTflBtn.Click += GrDownloadTfl_Click;
            this.grDownloadGeoBtn.Click += GrDownloadGeo_Click;
            this.grDownloadTxiBtn.Click += GrDownloadTxi_Click;
            this.grDownloadRwBtn.Click  += GrDownloadRw_Click;

            // Assemble (bottom first, then Fill, then Top)
            this.groundPage.Controls.Add(grLogWrapper);
            this.groundPage.Controls.Add(this.grDownloadPanel);
            this.groundPage.Controls.Add(this.grProgressPanel);
            this.groundPage.Controls.Add(this.grImportPanel);
            this.groundPage.Controls.Add(this.grHeaderPanel);
        }

        // ─────────────────────────────────────────────────────────────────
        //  OSM DATA PAGE  - Coming Soon placeholder
        // ─────────────────────────────────────────────────────────────────
        private void BuildOsmPage()
        {
            const int OsmPadH = 36;

            this.osmPage = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 248, 255),
                Visible   = false,
            };

            // ── Header ─────────────────────────────────────────────────────
            var osmHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 236,
                BackColor = Color.White,
                Padding   = new Padding(OsmPadH, 20, OsmPadH, 20),
            };

            var osmTitleLabel = new Label
            {
                Text        = "Auto Ground Layout",
                Font        = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor   = Color.FromArgb(17, 24, 39),
                Dock        = DockStyle.Top,
                Height      = 52,
                AutoSize    = false,
                TextAlign   = ContentAlignment.MiddleLeft,
                UseMnemonic = false,
            };

            var osmGapTitle = new Panel { Dock = DockStyle.Top, Height = 10, BackColor = Color.Transparent };

            var osmSubLabel = new Label
            {
                Text        = "One click  ·  Any airport  ·  Instant Aurora output",
                Font        = new Font("Segoe UI", 10.5f),
                ForeColor   = Color.FromArgb(107, 114, 128),
                Dock        = DockStyle.Top,
                Height      = 26,
                AutoSize    = false,
                TextAlign   = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };

            var osmGap1 = new Panel { Dock = DockStyle.Top, Height = 24, BackColor = Color.Transparent };

            var osmDescLabel = new Label
            {
                Text        = "Generate complete airport ground layouts with a single click - no manual KML " +
                              "drawing required. Simply enter an airport ICAO code and the tool will automatically " +
                              "build taxiways, aprons, stands, and runways, outputting ready-to-use Aurora sector " +
                              "files in the same .tfl / .geo / .txi / .rw format as the KML importer.",
                Font        = new Font("Segoe UI", 10f),
                ForeColor   = Color.FromArgb(55, 65, 81),
                Dock        = DockStyle.Top,
                Height      = 84,
                AutoSize    = false,
                TextAlign   = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };

            osmHeader.Controls.Add(osmDescLabel);
            osmHeader.Controls.Add(osmGap1);
            osmHeader.Controls.Add(osmSubLabel);
            osmHeader.Controls.Add(osmGapTitle);
            osmHeader.Controls.Add(osmTitleLabel);

            // ── Centred Coming Soon card ────────────────────────────────────
            var centerPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            var osmCard     = new Panel { BackColor = Color.White, Size = new Size(640, 290) };

            osmCard.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var borderPen = new Pen(Color.FromArgb(228, 235, 245), 1);
                g.DrawRectangle(borderPen, 0, 0, osmCard.Width - 1, osmCard.Height - 1);
                using var accent = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Point(0, 0), new Point(osmCard.Width, 0),
                    Color.FromArgb(225, 29, 72), Color.FromArgb(37, 99, 235));
                g.FillRectangle(accent, 0, 0, osmCard.Width, 4);
            };

            var osmCardEmoji = new Label
            {
                Text      = "✨",
                Font      = new Font("Segoe UI Emoji", 36f),
                BackColor = Color.Transparent,
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleCenter,
            };

            var osmCardTitle = new Label
            {
                Text      = "Coming Soon",
                Font      = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                BackColor = Color.Transparent,
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleCenter,
            };

            var osmCardDesc = new Label
            {
                Text      = "With a single click, generate stunning ground layouts for any airport in the world - instantly.",
                Font      = new Font("Segoe UI", 10.5f),
                ForeColor = Color.FromArgb(107, 114, 128),
                BackColor = Color.Transparent,
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleCenter,
            };

            var osmCardDivider = new Panel { BackColor = Color.FromArgb(228, 235, 245), Height = 1 };

            var osmCardBadge = new Label
            {
                Text      = "Planned Feature",
                Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 120, 150),
                BackColor = Color.Transparent,
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleCenter,
            };

            osmCard.Controls.Add(osmCardEmoji);
            osmCard.Controls.Add(osmCardTitle);
            osmCard.Controls.Add(osmCardDesc);
            osmCard.Controls.Add(osmCardDivider);
            osmCard.Controls.Add(osmCardBadge);

            // Layout card children on resize so they stretch with the card width
            osmCard.Resize += (s, e) =>
            {
                int w = osmCard.ClientSize.Width;
                osmCardEmoji  .SetBounds(0,   20, w,      68);
                osmCardTitle  .SetBounds(0,   96, w,      48);
                osmCardDesc   .SetBounds(40, 152, w - 80, 52);
                osmCardDivider.SetBounds(40, 216, w - 80,  1);
                osmCardBadge  .SetBounds(0,  228, w,      24);
            };

            centerPanel.Controls.Add(osmCard);
            centerPanel.Resize += (s, e) =>
            {
                // Card fills 75% of the available width, centred vertically
                int cardW = Math.Max(400, (int)(centerPanel.ClientSize.Width * 0.75));
                osmCard.Size     = new Size(cardW, 320);
                int cx = (centerPanel.ClientSize.Width  - cardW) / 2;
                int cy = (centerPanel.ClientSize.Height - osmCard.Height) / 2;
                osmCard.Location = new Point(Math.Max(0, cx), Math.Max(0, cy));
            };

            this.osmPage.Controls.Add(centerPanel);
            this.osmPage.Controls.Add(osmHeader);
        }

        // ─────────────────────────────────────────────────────────────────
        //  CREDITS PAGE  - embedded, same style as other pages
        // ─────────────────────────────────────────────────────────────────
        private void BuildCreditsPage()
        {
            const int CrPadH = 36;

            this.creditsPage = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 248, 255),
                Visible   = false,
            };

            // ── Header - dark navy with IVAO logo ──────────────────────────
            this.creditsHeaderPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 200,
                BackColor = Color.FromArgb(13, 27, 75),
            };

            var creditsLogo = new PictureBox
            {
                Size     = new Size(180, 80),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
            };

            var creditsLogoFallback = new Label
            {
                Text      = "IVAO",
                Font      = new Font("Segoe UI", 28f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Size      = new Size(180, 80),
            };

            var creditsTitleLbl = new Label
            {
                Text      = "IVAO ATC Utilities",
                Font      = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize  = false,
                Size      = new Size(400, 30),
            };

            var creditsSubLbl = new Label
            {
                Text      = "Sector File Creator",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(148, 163, 184),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize  = false,
                Size      = new Size(400, 22),
            };

            creditsHeaderPanel.Controls.Add(creditsLogo);
            creditsHeaderPanel.Controls.Add(creditsLogoFallback);
            creditsHeaderPanel.Controls.Add(creditsTitleLbl);
            creditsHeaderPanel.Controls.Add(creditsSubLbl);

            creditsHeaderPanel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                // Gradient accent strip at the bottom
                using var grad = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Point(0, creditsHeaderPanel.Height - 4),
                    new Point(creditsHeaderPanel.Width, creditsHeaderPanel.Height - 4),
                    Color.FromArgb(225, 29, 72), Color.FromArgb(37, 99, 235));
                g.FillRectangle(grad, 0, creditsHeaderPanel.Height - 4, creditsHeaderPanel.Width, 4);
            };

            creditsHeaderPanel.Resize += (s, e) =>
            {
                int cx = creditsHeaderPanel.ClientSize.Width / 2;
                int logoW = 180;
                creditsLogo.Location         = new Point(cx - logoW / 2, 18);
                creditsLogoFallback.Location  = new Point(cx - logoW / 2, 18);
                creditsTitleLbl.Location     = new Point(cx - 200, 108);
                creditsSubLbl.Location       = new Point(cx - 200, 140);
            };

            // Try to load IVAO white logo
            creditsHeaderPanel.HandleCreated += async (s, e) =>
            {
                try
                {
                    if (System.IO.File.Exists("./ivao_white.png"))
                    {
                        creditsLogo.Image    = System.Drawing.Image.FromFile("./ivao_white.png");
                        creditsLogoFallback.Visible = false;
                        return;
                    }
                    using var client = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(5) };
                    byte[] data = await client.GetByteArrayAsync("https://static.ivao.aero/img/logos/logo_white.png");
                    using var ms = new System.IO.MemoryStream(data);
                    creditsLogo.Image = System.Drawing.Image.FromStream(ms);
                    creditsLogoFallback.Visible = false;
                }
                catch { /* keep fallback text */ }
            };

            // ── Body ──────────────────────────────────────────────────────
            this.creditsBodyPanel = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.White,
                Padding   = new Padding(CrPadH, 24, CrPadH, 24),
            };

            // Helpers
            Panel MakeGapCr(int h) => new Panel { Dock = DockStyle.Top, Height = h, BackColor = Color.Transparent };
            Panel MakeDivCr()     => new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Color.FromArgb(226, 232, 240) };
            Label MakeSecLbl(string t) => new Label
            {
                Text      = t,
                Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                Dock      = DockStyle.Top,
                Height    = 28,
                TextAlign = ContentAlignment.BottomLeft,
            };

            Panel MakePersonCard(string initials, Color avatarBg, string name, string id, string url)
            {
                var card = new Panel
                {
                    Dock      = DockStyle.Top,
                    Height    = 68,
                    BackColor = Color.FromArgb(248, 250, 252),
                };
                card.Paint += (s, e) =>
                {
                    using var pen = new Pen(Color.FromArgb(226, 232, 240), 1);
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                };

                // Avatar drawn via Paint
                var avatar = new Panel { Size = new Size(42, 42), Location = new Point(14, 13), BackColor = Color.Transparent };
                var capBg  = avatarBg;
                var capTxt = initials;
                avatar.Paint += (s, e) =>
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    using var b = new SolidBrush(capBg);
                    e.Graphics.FillEllipse(b, 0, 0, 41, 41);
                    using var sf = new System.Drawing.StringFormat
                    {
                        Alignment     = System.Drawing.StringAlignment.Center,
                        LineAlignment = System.Drawing.StringAlignment.Center,
                        FormatFlags   = System.Drawing.StringFormatFlags.NoWrap,
                    };
                    using var f = new Font("Segoe UI", 10f, FontStyle.Bold);
                    e.Graphics.DrawString(capTxt, f, Brushes.White, new RectangleF(0, 0, 41, 41), sf);
                };

                var nameL = new Label
                {
                    Text      = name,
                    Font      = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(17, 24, 39),
                    Location  = new Point(68, 12),
                    AutoSize  = true,
                    BackColor = Color.Transparent,
                };
                var idL = new Label
                {
                    Text      = id,
                    Font      = new Font("Segoe UI", 8.5f),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    Location  = new Point(68, 36),
                    AutoSize  = true,
                    BackColor = Color.Transparent,
                };
                var linkL = new LinkLabel
                {
                    Text      = "View IVAO profile →",
                    Font      = new Font("Segoe UI", 8.5f),
                    LinkColor = Color.FromArgb(37, 99, 235),
                    BackColor = Color.Transparent,
                    AutoSize  = true,
                };
                var capUrl = url;
                linkL.LinkClicked += (s, e) =>
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = capUrl, UseShellExecute = true });

                card.Resize += (s, e) => linkL.Location = new Point(card.Width - linkL.Width - 16, 26);
                card.Controls.Add(avatar);
                card.Controls.Add(nameL);
                card.Controls.Add(idL);
                card.Controls.Add(linkL);
                return card;
            }

            var warningBanner = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 32,
                BackColor = Color.FromArgb(255, 241, 242),
            };
            warningBanner.Controls.Add(new Label
            {
                Text      = "⚠   NOT FOR REAL WORLD USE   ⚠",
                Font      = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = Color.FromArgb(190, 18, 57),
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
            });

            var apiNoticeCr = new Label
            {
                Text      = "Aviation and navigation data is sourced from free,\n" +
                            "publicly available APIs. All usage is legal and safe,\n" +
                            "for simulation purposes only.",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(71, 85, 105),
                Dock      = DockStyle.Top,
                Height    = 58,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(241, 245, 249),
                Padding   = new Padding(8, 0, 8, 0),
            };

            creditsBodyPanel.Controls.Add(apiNoticeCr);
            creditsBodyPanel.Controls.Add(MakeSecLbl("DATA SOURCES"));
            creditsBodyPanel.Controls.Add(MakeDivCr());
            creditsBodyPanel.Controls.Add(MakeGapCr(10));
            creditsBodyPanel.Controls.Add(MakePersonCard("NP", Color.FromArgb(5, 150, 105),  "Nilay Parsodkar", "VID 709833", "https://ivao.aero/Member.aspx?Id=709833"));
            creditsBodyPanel.Controls.Add(MakeGapCr(8));
            creditsBodyPanel.Controls.Add(MakePersonCard("VM", Color.FromArgb(37, 99, 235),  "Veda Moola",      "VID 656077", "https://ivao.aero/Member.aspx?Id=656077"));
            creditsBodyPanel.Controls.Add(MakeSecLbl("MADE BY  &  TESTED BY"));
            creditsBodyPanel.Controls.Add(MakeDivCr());
            creditsBodyPanel.Controls.Add(MakeGapCr(10));
            creditsBodyPanel.Controls.Add(warningBanner);
            creditsBodyPanel.Controls.Add(MakeGapCr(10));

            this.creditsPage.Controls.Add(creditsBodyPanel);
            this.creditsPage.Controls.Add(creditsHeaderPanel);
        }

        // ─────────────────────────────────────────────────────────────────
        //  FLIGHT SCHEDULES PAGE  - AeroDataBox (RapidAPI)
        // ─────────────────────────────────────────────────────────────────
        private void BuildFlightPage()
        {
            const int FlPadH = 36;

            this.flightPage = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = LightBg,
                Visible   = false,
            };

            // ── Header ─────────────────────────────────────────────────────
            this.flHeaderPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 236,
                BackColor = LightCard,
                Padding   = new Padding(FlPadH, 20, FlPadH, 20),
            };
            this.flTitleLabel = new Label
            {
                Text        = "Flight Schedules",
                Font        = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor   = Color.FromArgb(17, 24, 39),
                Dock        = DockStyle.Top,
                Height      = 52,
                AutoSize    = false,
                TextAlign   = ContentAlignment.MiddleLeft,
                UseMnemonic = false,
            };
            var flGapTitle = new Panel { Dock = DockStyle.Top, Height = 10, BackColor = Color.Transparent };
            this.flSubLabel = new Label
            {
                Text        = "Past · Present · Future",
                Font        = new Font("Segoe UI", 10.5f),
                ForeColor   = Color.FromArgb(107, 114, 128),
                Dock        = DockStyle.Top,
                Height      = 26,
                AutoSize    = false,
                TextAlign   = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };
            var flGap1 = new Panel { Dock = DockStyle.Top, Height = 24, BackColor = Color.Transparent };
            var flDescLabel = new Label
            {
                Text        = "Search real-world flight schedules by departure/arrival airport " +
                              "(ICAO 4-letter) or airline (ICAO 3-letter), date and time window. " +
                              "Returns past (historical ADS-B), current (live) and future (scheduled) " +
                              "flights. Data is sourced from a free, publicly available API. " +
                              "All usage is legal and for simulation purposes only.",
                Font        = new Font("Segoe UI", 10f),
                ForeColor   = Color.FromArgb(55, 65, 81),
                Dock        = DockStyle.Top,
                Height      = 84,
                AutoSize    = false,
                TextAlign   = ContentAlignment.TopLeft,
                UseMnemonic = false,
            };
            this.flHeaderPanel.Controls.Add(flDescLabel);
            this.flHeaderPanel.Controls.Add(flGap1);
            this.flHeaderPanel.Controls.Add(this.flSubLabel);
            this.flHeaderPanel.Controls.Add(flGapTitle);
            this.flHeaderPanel.Controls.Add(this.flTitleLabel);

            // ── Search panel ───────────────────────────────────────────────
            // Layout: 14px top pad | 14px labels | 6px gap | 54px inputs | 14px bottom
            const int SpH   = 102;  // panel height
            const int LblY  = 12;   // small section labels row
            const int WrapH = 54;   // input / control height
            const int WrapY = 34;   // inputs start below labels

            var inputBg = Color.FromArgb(247, 249, 252);  // subtle fill for wrappers

            this.flSearchPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = SpH,
                BackColor = LightCard,
            };
            this.flSearchPanel.Paint += PaintBottomBorder;

            // Small label helper used for column headers
            Label MakeFlLabel(string text) => new Label
            {
                Text      = text,
                Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize  = true,
                BackColor = Color.Transparent,
            };

            // Input wrapper factory - filled + bordered rounded box with centred TextBox
            Panel MakeFlWrapper(int w, ref Panel wrapperField, ref TextBox boxField,
                                string placeholder, int maxLen)
            {
                var wrapper = new Panel
                {
                    Size      = new Size(w, WrapH),
                    BackColor = inputBg,
                    Cursor    = Cursors.IBeam,
                };
                var box = new TextBox
                {
                    Font            = new Font("Segoe UI", 11.5f),
                    PlaceholderText = placeholder,
                    BorderStyle     = BorderStyle.None,
                    CharacterCasing = CharacterCasing.Upper,
                    MaxLength       = maxLen,
                    BackColor       = inputBg,
                };
                wrapper.Paint += (s, e) =>
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    bool focused = box.Focused;
                    var  rect   = new Rectangle(1, 1, wrapper.Width - 2, wrapper.Height - 2);
                    var  path   = RoundedRect(rect, 8);
                    using var fill = new System.Drawing.SolidBrush(inputBg);
                    e.Graphics.FillPath(fill, path);
                    using var pen = new Pen(
                        focused ? Color.FromArgb(37, 99, 235) : Color.FromArgb(190, 205, 225),
                        focused ? 2f : 1.5f);
                    e.Graphics.DrawPath(pen, path);
                };
                wrapper.Click  += (s, e) => box.Focus();
                box.Enter      += (s, e) => wrapper.Invalidate();
                box.Leave      += (s, e) => wrapper.Invalidate();
                wrapper.Resize += (s, e) => box.SetBounds(12,
                    (wrapper.Height - box.PreferredHeight) / 2,
                    wrapper.Width - 24, box.PreferredHeight);
                wrapper.Controls.Add(box);
                wrapperField = wrapper;
                boxField     = box;
                return wrapper;
            }

            Panel depW = null!, arrW = null!, airW = null!;
            TextBox depB = null!, arrB = null!, airB = null!;
            MakeFlWrapper(165, ref depW, ref depB, "Dep ICAO", 4);
            MakeFlWrapper(165, ref arrW, ref arrB, "Arr ICAO", 4);
            MakeFlWrapper(155, ref airW, ref airB, "Airline",  3);
            this.flDepWrapper     = depW; this.flDepBox     = depB;
            this.flArrWrapper     = arrW; this.flArrBox     = arrB;
            this.flAirlineWrapper = airW; this.flAirlineBox = airB;

            // Arrow - fixed size, never wraps
            var flArrowLbl = new Label
            {
                Text      = "→",
                Font      = new Font("Segoe UI", 12f),
                ForeColor = Color.FromArgb(160, 178, 210),
                Size      = new Size(28, 28),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
            };

            // Left-side column labels (fixed positions)
            var flDepLbl = MakeFlLabel("DEPARTURE");
            var flArrLbl = MakeFlLabel("ARRIVAL");
            var flAirLbl = MakeFlLabel("AIRLINE");

            // Fixed left positions:  [DEP 165] [→ 28+gap] [ARR 165] [gap 14] [Airline 155]
            const int ArrX     = FlPadH + 165 + 32;   // 32 = 8 + 28arrow - 8
            const int AirlineX = ArrX + 165 + 14;

            this.flDepWrapper.Location = new Point(FlPadH, WrapY);
            flArrowLbl.Location        = new Point(FlPadH + 165 + 4, WrapY + (WrapH - 28) / 2);
            this.flArrWrapper.Location = new Point(ArrX, WrapY);
            this.flAirlineWrapper.Location = new Point(AirlineX, WrapY);

            flDepLbl.Location = new Point(FlPadH,    LblY);
            flArrLbl.Location = new Point(ArrX,      LblY);
            flAirLbl.Location = new Point(AirlineX,  LblY);

            // Date picker
            this.flDatePicker = new DateTimePicker
            {
                Font   = new Font("Segoe UI", 10f),
                Format = DateTimePickerFormat.Short,
                Size   = new Size(148, WrapH),
                Value  = DateTime.Today,
            };

            // Time pickers - labels embedded in CustomFormat
            this.flFromTimePicker = new DateTimePicker
            {
                Font         = new Font("Segoe UI", 10f),
                Format       = DateTimePickerFormat.Custom,
                CustomFormat = "HH:mm",
                ShowUpDown   = true,
                Size         = new Size(72, WrapH),
                Value        = DateTime.Today,
            };
            this.flToTimePicker = new DateTimePicker
            {
                Font         = new Font("Segoe UI", 10f),
                Format       = DateTimePickerFormat.Custom,
                CustomFormat = "HH:mm",
                ShowUpDown   = true,
                Size         = new Size(72, WrapH),
                Value        = DateTime.Today.AddHours(23).AddMinutes(59),
            };

            // Filter combo
            this.flStatusCombo = new ComboBox
            {
                Font          = new Font("Segoe UI", 10f),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Size          = new Size(188, WrapH),
            };
            this.flStatusCombo.Items.AddRange(new object[]
                { "Departures", "Arrivals" });
            this.flStatusCombo.SelectedIndex = 0;

            // Search button
            this.flSearchButton = new Button
            {
                Text      = "Search",
                Font      = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height    = WrapH,
                Width     = 108,
                Cursor    = Cursors.Hand,
            };
            this.flSearchButton.FlatAppearance.BorderSize = 0;
            this.flSearchButton.UseVisualStyleBackColor   = false;

            // Right-side column labels (repositioned in Resize)
            var flDateLbl   = MakeFlLabel("DATE");
            var flTimeLbl   = MakeFlLabel("TIME WINDOW");
            var flFilterLbl = MakeFlLabel("FILTER");

            // Small "→" separator between time pickers
            var flTimeSepLbl = new Label
            {
                Text      = "–",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(160, 178, 210),
                AutoSize  = true,
                BackColor = Color.Transparent,
            };

            // Right-anchored - resize keeps everything flush to right edge
            this.flSearchPanel.Resize += (s, e) =>
            {
                int right  = flSearchPanel.ClientSize.Width - FlPadH;
                int comboY = WrapY + (WrapH - flStatusCombo.Height)   / 2;
                int dtY    = WrapY + (WrapH - flDatePicker.Height)     / 2;
                int timeY  = WrapY + (WrapH - flFromTimePicker.Height) / 2;
                int sepY   = WrapY + (WrapH - flTimeSepLbl.Height)     / 2;

                flSearchButton.Location   = new Point(right - flSearchButton.Width, WrapY);
                flStatusCombo.Location    = new Point(flSearchButton.Left - flStatusCombo.Width - 10, comboY);
                flToTimePicker.Location   = new Point(flStatusCombo.Left  - flToTimePicker.Width  - 8, timeY);
                flTimeSepLbl.Location     = new Point(flToTimePicker.Left - flTimeSepLbl.Width    - 4, sepY);
                flFromTimePicker.Location = new Point(flTimeSepLbl.Left   - flFromTimePicker.Width - 4, timeY);
                flDatePicker.Location     = new Point(flFromTimePicker.Left - flDatePicker.Width  - 14, dtY);

                // Reposition right-side labels
                flFilterLbl.Location = new Point(flStatusCombo.Left,    LblY);
                flTimeLbl.Location   = new Point(flFromTimePicker.Left,  LblY);
                flDateLbl.Location   = new Point(flDatePicker.Left,      LblY);
            };

            this.flSearchPanel.Controls.Add(this.flDepWrapper);
            this.flSearchPanel.Controls.Add(flArrowLbl);
            this.flSearchPanel.Controls.Add(this.flArrWrapper);
            this.flSearchPanel.Controls.Add(this.flAirlineWrapper);
            this.flSearchPanel.Controls.Add(flDepLbl);
            this.flSearchPanel.Controls.Add(flArrLbl);
            this.flSearchPanel.Controls.Add(flAirLbl);
            this.flSearchPanel.Controls.Add(this.flDatePicker);
            this.flSearchPanel.Controls.Add(flDateLbl);
            this.flSearchPanel.Controls.Add(this.flFromTimePicker);
            this.flSearchPanel.Controls.Add(flTimeSepLbl);
            this.flSearchPanel.Controls.Add(this.flToTimePicker);
            this.flSearchPanel.Controls.Add(flTimeLbl);
            this.flSearchPanel.Controls.Add(this.flStatusCombo);
            this.flSearchPanel.Controls.Add(flFilterLbl);
            this.flSearchPanel.Controls.Add(this.flSearchButton);

            // ── Progress strip ─────────────────────────────────────────────
            this.flProgressPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 36,
                BackColor = LightCard,
            };
            this.flProgressPanel.Paint += PaintBottomBorder;

            this.flProgressBar = new ProgressBar
            {
                Height   = 5,
                Minimum  = 0,
                Maximum  = 100,
                Value    = 0,
                Style    = ProgressBarStyle.Continuous,
                Location = new Point(0, 0),
            };
            this.flStatusLabel = new Label
            {
                Text      = "Enter search criteria above and click Search",
                Font      = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(120, 135, 160),
                AutoSize  = true,
                Location  = new Point(FlPadH, 10),
            };
            // Request counter label (right side of progress panel)
            var flReqCountLabel = new Label
            {
                Name      = "flReqCountLabel",
                Text      = $"{ConfigManager.GetTodayFlightRequests()} / 10 requests today",
                Font      = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(120, 135, 160),
                AutoSize  = true,
                BackColor = Color.Transparent,
            };
            this.flProgressPanel.Controls.Add(this.flProgressBar);
            this.flProgressPanel.Controls.Add(this.flStatusLabel);
            this.flProgressPanel.Controls.Add(flReqCountLabel);
            this.flProgressPanel.Resize += (s, e) =>
            {
                flProgressBar.SetBounds(0, 0, flProgressPanel.ClientSize.Width, 5);
                flReqCountLabel.Location = new Point(
                    flProgressPanel.ClientSize.Width - flReqCountLabel.Width - FlPadH, 10);
            };

            // ── Results log ────────────────────────────────────────────────
            // RichTextBox ignores Padding - wrap in a Panel so the text is indented
            this.flLogBox = new RichTextBox
            {
                Dock        = DockStyle.Fill,
                Font        = new Font("Consolas", 8.5f),
                BackColor   = LightLogBg,
                ForeColor   = LightLogFg,
                BorderStyle = BorderStyle.None,
                ReadOnly    = true,
                ScrollBars  = RichTextBoxScrollBars.Vertical,
            };
            var flLogWrapper = new Panel
            {
                Dock      = DockStyle.Fill,
                Padding   = new Padding(FlPadH, 12, FlPadH, 8),
                BackColor = LightLogBg,
            };
            flLogWrapper.Controls.Add(this.flLogBox);

            // ── Download panel ─────────────────────────────────────────────
            this.flDownloadPanel = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 72,
                BackColor = LightCard,
            };
            this.flDownloadPanel.Paint += PaintTopBorder;

            this.flDownloadExcelBtn = MakeDownloadButton(
                "⬇  Download Flight Schedules (.xlsx)",
                Color.FromArgb(21, 128, 61));
            this.flDownloadExcelBtn.Enabled = false;

            this.flDownloadPanel.Controls.Add(this.flDownloadExcelBtn);
            this.flDownloadPanel.Resize += (s, e) =>
            {
                int padH = FlPadH;
                int pw   = flDownloadPanel.ClientSize.Width - padH * 2;
                flDownloadExcelBtn.SetBounds(padH, 14, pw, 44);
            };

            // Wire events
            this.flSearchButton.Click      += FlSearch_Click;
            this.flDownloadExcelBtn.Click  += FlDownloadExcel_Click;
            foreach (var box in new[] { flDepBox, flArrBox, flAirlineBox })
                box.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter) { FlSearch_Click(s, e); e.SuppressKeyPress = true; }
                };

            // Assemble (top → bottom via DockStyle)
            this.flightPage.Controls.Add(flLogWrapper);            // Fill
            this.flightPage.Controls.Add(this.flDownloadPanel);    // Bottom
            this.flightPage.Controls.Add(this.flProgressPanel);    // Top
            this.flightPage.Controls.Add(this.flSearchPanel);      // Top
            this.flightPage.Controls.Add(this.flHeaderPanel);      // Top
        }

        private static Button MakeDownloadButton(string text, Color bg)
        {
            var btn = new Button
            {
                Text      = text,
                Font      = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.UseVisualStyleBackColor   = false;

            // Dim visually when disabled
            btn.EnabledChanged += (s, e) =>
            {
                btn.BackColor = btn.Enabled ? bg : Color.FromArgb(200, 200, 210);
                btn.ForeColor = btn.Enabled ? Color.White : Color.FromArgb(150, 150, 160);
                btn.Cursor    = btn.Enabled ? Cursors.Hand : Cursors.Default;
            };

            return btn;
        }

        // Draws a 1px border on the bottom edge of a panel
        private static void PaintBottomBorder(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            var p = (Panel)sender;
            using var pen = new Pen(Color.FromArgb(228, 235, 245), 1);
            e.Graphics.DrawLine(pen, 0, p.Height - 1, p.Width, p.Height - 1);
        }

        // Draws a 1px border on the top edge of a panel
        private static void PaintTopBorder(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            var p = (Panel)sender;
            using var pen = new Pen(Color.FromArgb(228, 235, 245), 1);
            e.Graphics.DrawLine(pen, 0, 0, p.Width, 0);
        }
    }
}
