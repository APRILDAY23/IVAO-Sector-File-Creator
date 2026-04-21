using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Sector_File
{
    // ─────────────────────────────────────────────────────────────────────────
    //  MainForm  -  single-window shell for IVAO Sector File Creator
    //  Handles: login → hub → ATC grid / Flight grid → embedded tools
    // ─────────────────────────────────────────────────────────────────────────
    public partial class MainForm : Form
    {
        // ── Localisation shorthand ───────────────────────────────────────────
        private static string L(string key) => Localization.Get(key);

        // ── Auth / user state ────────────────────────────────────────────────
        private int    _userId;
        private string _firstName = "";
        private string _lastName  = "";

        // ── Navigation stack ─────────────────────────────────────────────────
        private readonly Stack<(Panel Page, string Name)> _navStack = new();
        private bool _sidebarCollapsed = false;

        // ── Sidebar width constants ───────────────────────────────────────────
        private const int SidebarNormal   = 300;
        private const int SidebarMaximized = 400;

        // ── OAuth constants ──────────────────────────────────────────────────
        private const string AuthorizationEndpoint = "https://sso.ivao.aero/authorize";
        private const string TokenEndpoint         = "https://api.ivao.aero/v2/oauth/token";
        private const string RedirectUri           = "http://localhost:5000/callback";
        private const string Scopes                = "openid profile email";
        private string _oauthState = "";
        // ClientId / ClientSecret are loaded from encrypted config (ConfigManager)
        private static string ClientId     => ConfigManager.OAuthClientId;
        private static string ClientSecret => ConfigManager.OAuthClientSecret;
        private HttpListener _listener;
        private readonly int[] _allowedAccess = { 493962 };

        // ── SID/STAR state ───────────────────────────────────────────────────
        private const string ApiBaseUrl = "https://airac.net/api/v1";
        private string _sidOutput  = "";
        private string _starOutput = "";

        // ────────────────────────────────────────────────────────────────────
        public MainForm()
        {
            InitializeComponent();
            try { this.Icon = new System.Drawing.Icon("./tools.ico"); } catch { }
            this.FormClosed += (s, e) => { Application.Exit(); Environment.Exit(0); };
            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Build settings page (after InitializeComponent has run)
            BuildSettingsPage();

            // Add settingsPage to content panel
            contentPanel.Controls.Add(settingsPage);

            // Wire settings nav button
            WireSidebarItem(navSettingsBtn, () =>
            {
                NavigateTo(settingsPage, "Settings");
                SetActiveSidebarItem(navSettingsBtn);
            });

            // Populate AIRAC badge (fetched during splash)
            RefreshAiracBadge();

            // Sidebar navigation is wired in BuildSidebar() via Designer

            // Navigate to login first
            ShowOnly(loginPage);
            _navStack.Clear();
            _navStack.Push((loginPage, "Login"));
            UpdateNav();

            // Expand/collapse sidebar on window-state change
            this.SizeChanged += (s, e) =>
            {
                if (_sidebarCollapsed) return;
                int targetW = (this.WindowState == FormWindowState.Maximized)
                    ? SidebarMaximized : SidebarNormal;
                if (sidebarPanel.Width != targetW)
                    sidebarPanel.Width = targetW;
            };
        }

        // ────────────────────────────────────────────────────────────────────
        //  Navigation helpers
        // ────────────────────────────────────────────────────────────────────

        private void NavigateTo(Panel page, string name)
        {
            ShowOnly(page);

            // Prevent stack from growing with duplicate entries:
            // if this page is already somewhere in the stack, pop back to it.
            if (_navStack.Any(x => x.Page == page))
            {
                while (_navStack.Count > 0 && _navStack.Peek().Page != page)
                    _navStack.Pop();
            }
            else
            {
                _navStack.Push((page, name));
            }

            UpdateNav();

            // Page-specific on-show hooks
            if (page == countryPage) OnCountryPageShown();
        }

        private void NavigateBack()
        {
            if (_navStack.Count <= 1) return;
            _navStack.Pop();
            ShowOnly(_navStack.Peek().Page);
            UpdateNav();
        }

        private void ShowOnly(Panel page)
        {
            // Hide country dropdown overlay when leaving that page
            if (page != countryPage) CntHideDropdown();

            // Hide all content panels
            foreach (Control c in contentPanel.Controls)
                if (c is Panel p) p.Visible = false;
            page.Visible = true;

            // Hide sidebar and header on the login page - let it fill the whole window
            bool isLogin = (page == loginPage);
            sidebarPanel.Visible = !isLogin;
            headerPanel.Visible  = !isLogin;

            // Lock window size on login (no maximize, fixed dimensions)
            if (isLogin)
            {
                this.MaximizeBox  = false;
                this.MinimumSize  = new System.Drawing.Size(0, 0);
                this.MaximumSize  = new System.Drawing.Size(0, 0);
                this.ClientSize   = new System.Drawing.Size(980, 620);
                this.MinimumSize  = new System.Drawing.Size(980, 620);
                this.MaximumSize  = new System.Drawing.Size(980, 620);
            }
            else
            {
                this.MaximizeBox   = true;
                this.MaximumSize   = new System.Drawing.Size(0, 0);
                this.MinimumSize   = new System.Drawing.Size(900, 620);
                this.WindowState   = System.Windows.Forms.FormWindowState.Maximized;
            }
        }

        private void UpdateNav()
        {
            var all  = _navStack.Reverse().ToArray();   // oldest → newest
            // Show at most 3 entries; prefix with "…" if there's more history
            int skip  = Math.Max(0, all.Length - 3);
            var show  = all.Skip(skip).ToArray();
            string prefix = skip > 0 ? "…  ›  " : "";
            breadcrumbLabel.Text = prefix + string.Join("  ›  ", show.Select(x => x.Name));
            backButton.Visible   = _navStack.Count > 1;
        }

        private static void WireCardClick(Panel card, EventHandler handler)
        {
            card.Click += handler;
            foreach (System.Windows.Forms.Control c in card.Controls)
                c.Click += handler;
        }

        // ────────────────────────────────────────────────────────────────────
        //  Launch external tool forms (airport, FIR, KML, flight)
        // ────────────────────────────────────────────────────────────────────

        private void LaunchTool(Func<Form> factory)
        {
            var form = factory();
            form.Owner = this;
            this.Hide();
            form.ShowDialog();
            this.Show();
        }

        private void LaunchKml()
        {
            var ofd = new OpenFileDialog { Filter = "KML Files (*.kml)|*.kml|All Files (*.*)|*.*" };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            string path = ofd.FileName;
            if (!path.EndsWith(".kml", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Please upload only KML files.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.Hide();
            var lf = new LoadingForm(path, _userId) { Owner = this };
            lf.ShowDialog();
            this.Show();
        }

        private void RefreshAiracBadge()
        {
            if (SplashForm.AiracDaysLeft >= 0)
            {
                var color =
                    SplashForm.AiracDaysLeft > 14 ? System.Drawing.Color.FromArgb(100, 220, 140) :
                    SplashForm.AiracDaysLeft > 6  ? System.Drawing.Color.Orange :
                                                    System.Drawing.Color.Tomato;
                headerAiracLabel.Text         = $"AIRAC {SplashForm.AiracCycle}";
                headerAiracLabel.ForeColor    = color;
                headerAiracDaysLabel.Text     = $"{SplashForm.AiracDaysLeft} days left";
                headerAiracDaysLabel.ForeColor = color;
            }
            else
            {
                headerAiracLabel.Text         = "AIRAC offline";
                headerAiracLabel.ForeColor    = System.Drawing.Color.FromArgb(160, 160, 170);
                headerAiracDaysLabel.Text     = "";
            }
        }

        // ────────────────────────────────────────────────────────────────────
        //  Login page - IVAO OAuth 2.0 SSO
        // ────────────────────────────────────────────────────────────────────

        private void LoginButton_Click(object sender, EventArgs e)
        {
            loginButton.Enabled = false;
            loginButton.Text    = "Opening browser…";
            _oauthState = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32))
                              .Replace("+", "-").Replace("/", "_").TrimEnd('=');
            StartListener();
            string url = $"{AuthorizationEndpoint}?response_type=code&client_id={ClientId}" +
                         $"&redirect_uri={RedirectUri}&scope={Scopes}&state={_oauthState}";
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }

        private void StartListener()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:5000/callback/");
            _listener.Start();
            _listener.BeginGetContext(OnRequestReceived, _listener);
        }

        private async void OnRequestReceived(IAsyncResult result)
        {
            var ctx           = _listener.EndGetContext(result);
            string code       = ctx.Request.QueryString["code"];
            string returnedState = ctx.Request.QueryString["state"];
            var resp          = ctx.Response;

            if (!string.IsNullOrEmpty(code) && returnedState == _oauthState)
            {
                string token = await ExchangeCodeForTokenAsync(code);
                if (!string.IsNullOrEmpty(token))
                {
                    string json = await FetchUserDataAsync(token);
                    if (!string.IsNullOrEmpty(json))
                    {
                        JObject info      = JObject.Parse(json);
                        int     userId    = info.Value<int>("id");
                        string  firstName = info.Value<string>("firstName") ?? "";
                        string  lastName  = info.Value<string>("lastName")  ?? "";
                        bool    isStaff   = info.Value<bool>("isStaff");

                        byte[] buf = System.Text.Encoding.UTF8.GetBytes(
                            "<html><body><script>window.close();</script><p>You may close this tab.</p></body></html>");
                        resp.ContentType     = "text/html";
                        resp.ContentLength64 = buf.Length;
                        resp.OutputStream.Write(buf, 0, buf.Length);
                        resp.OutputStream.Close();

                        if (isStaff || _allowedAccess.Contains(userId))
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                _userId    = userId;
                                _firstName = firstName;
                                _lastName  = lastName;
                                OnLoginSuccess();
                            });
                        }
                        else
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                loginButton.Enabled = true;
                                loginButton.Text    = "Continue with IVAO SSO";
                                MessageBox.Show("Access denied - staff accounts only.",
                                    "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            });
                        }
                    }
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        loginButton.Enabled = true;
                        loginButton.Text    = "Continue with IVAO SSO";
                    });
                }
            }

            _listener.Stop();
        }

        private void OnLoginSuccess()
        {
            // Update header user info
            string name = string.IsNullOrEmpty(_firstName)
                ? $"#{_userId}" : $"{_firstName} {_lastName}".Trim();
            string initial = string.IsNullOrEmpty(_firstName) ? "#" : _firstName[0].ToString().ToUpper();
            headerAppIconLabel.Text = initial;
            headerUserLabel.Text    = name;
            headerVidLabel.Text     = $"{_userId}";

            // Update welcome page
            string greeting = string.IsNullOrEmpty(_firstName) ? $"#{_userId}" : _firstName;
            hubWelcomeLabel.Text = $"Welcome back, {greeting}!";
            hubHintLabel.Text    = $"Third party sector file creation tool for IVAO  ·  {name}  (VID {_userId})";

            NavigateTo(welcomePage, "Home");
            SetActiveSidebarItem(navDashboardCard);
        }

        private async Task<string> ExchangeCodeForTokenAsync(string code)
        {
            using var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type",    "authorization_code"),
                new KeyValuePair<string, string>("code",          code),
                new KeyValuePair<string, string>("client_id",     ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("redirect_uri",  RedirectUri)
            });
            try
            {
                var res  = await client.PostAsync(TokenEndpoint, content);
                var body = await res.Content.ReadAsStringAsync();
                return JObject.Parse(body)["access_token"]?.ToString();
            }
            catch { return null; }
        }

        private async Task<string> FetchUserDataAsync(string token)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            try   { return await client.GetStringAsync("https://api.ivao.aero/v2/users/me"); }
            catch { return null; }
        }

        // Hub page navigation is now handled via the sidebar buttons

        // ────────────────────────────────────────────────────────────────────
        //  Back button
        // ────────────────────────────────────────────────────────────────────

        private void BackButton_Click(object sender, EventArgs e) =>
            NavigateBack();

        // ────────────────────────────────────────────────────────────────────
        //  Credits
        // ────────────────────────────────────────────────────────────────────

        private void CreditsButton_Click(object sender, EventArgs e) =>
            new CreditsForm().ShowDialog(this);
    }
}
