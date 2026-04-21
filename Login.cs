using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Sector_File
{
    public partial class Login : Form
    {
        private const string AuthorizationEndpoint = "https://sso.ivao.aero/authorize";
        private const string TokenEndpoint         = "https://api.ivao.aero/v2/oauth/token";
        private static string ClientId             => ConfigManager.OAuthClientId;
        private static string ClientSecret         => ConfigManager.OAuthClientSecret;
        private const string RedirectUri           = "http://localhost:5000/callback";
        private const string Scopes                = "openid profile email";
        private const string State                 = "12345678";

        private HttpListener listener;
        private readonly int[] allowedAccess = { 493962 };

        public Login()
        {
            InitializeComponent();
            try { this.Icon = new System.Drawing.Icon("./tools.ico"); } catch { }
            this.FormClosed += (s, e) => Application.Exit();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Show AIRAC cycle fetched during splash
            if (SplashForm.AiracDaysLeft >= 0)
            {
                airacBadgeLabel.Text = $"AIRAC {SplashForm.AiracCycle}  ·  " +
                                       $"{SplashForm.AiracEffective} → {SplashForm.AiracExpiry}  " +
                                       $"({SplashForm.AiracDaysLeft} days left)";

                airacBadgeLabel.ForeColor =
                    SplashForm.AiracDaysLeft > 14 ? System.Drawing.Color.FromArgb(100, 200, 140) :
                    SplashForm.AiracDaysLeft > 6  ? System.Drawing.Color.Orange :
                                                    System.Drawing.Color.Tomato;
            }
            else
            {
                airacBadgeLabel.Text      = "AIRAC data unavailable - working offline";
                airacBadgeLabel.ForeColor = System.Drawing.Color.FromArgb(100, 110, 130);
            }

            // Try to load the IVAO logo into the header
            LoadIvaoLogo();
        }

        private async void LoadIvaoLogo()
        {
            try
            {
                // ivao_blue.png ships with the project - use it for the header
                if (System.IO.File.Exists("./ivao_blue.png"))
                {
                    var img = System.Drawing.Image.FromFile("./ivao_blue.png");
                    logoLabel.Image     = img;
                    logoLabel.Text      = "";
                    logoLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                }
            }
            catch { /* keep emoji fallback */ }
        }

        // ── OAuth login flow ─────────────────────────────────────────────────

        private void LoginButton_Click(object sender, EventArgs e)
        {
            LoginButton.Enabled = false;
            LoginButton.Text    = "Opening browser...";
            StartListener();
            string url = $"{AuthorizationEndpoint}?response_type=code&client_id={ClientId}" +
                         $"&redirect_uri={RedirectUri}&scope={Scopes}&state={State}";
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }

        private void StartListener()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/callback/");
            listener.Start();
            listener.BeginGetContext(OnRequestReceived, listener);
        }

        private async void OnRequestReceived(IAsyncResult result)
        {
            var context  = listener.EndGetContext(result);
            var request  = context.Request;
            string code  = request.QueryString["code"];
            var response = context.Response;

            if (!string.IsNullOrEmpty(code))
            {
                string token = await ExchangeCodeForTokenAsync(code);
                if (!string.IsNullOrEmpty(token))
                {
                    string userJson = await FetchUserDataAsync(token);
                    if (!string.IsNullOrEmpty(userJson))
                    {
                        JObject info      = JObject.Parse(userJson);
                        int     userId    = info.Value<int>("id");
                        string  firstName = info.Value<string>("firstName") ?? "";
                        string  lastName  = info.Value<string>("lastName")  ?? "";
                        bool    isStaff   = info.Value<bool>("isStaff");

                        // Close browser tab
                        byte[] buf = System.Text.Encoding.UTF8.GetBytes(
                            "<html><body><script>window.close();</script>" +
                            "<p>You may close this tab.</p></body></html>");
                        response.ContentType      = "text/html";
                        response.ContentLength64  = buf.Length;
                        response.OutputStream.Write(buf, 0, buf.Length);
                        response.OutputStream.Close();

                        if (isStaff || allowedAccess.Contains(userId))
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Hide();
                                var choose = new Choose(userId, firstName, lastName);
                                choose.Show();
                            });
                        }
                        else
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                LoginButton.Enabled = true;
                                LoginButton.Text    = "Login with IVAO SSO";
                                MessageBox.Show(
                                    "Access denied - staff accounts only.",
                                    "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            });
                        }
                    }
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        LoginButton.Enabled = true;
                        LoginButton.Text    = "Login with IVAO SSO";
                    });
                }
            }

            listener.Stop();
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
                var res   = await client.PostAsync(TokenEndpoint, content);
                var body  = await res.Content.ReadAsStringAsync();
                return JObject.Parse(body)["access_token"]?.ToString();
            }
            catch { return null; }
        }

        private async Task<string> FetchUserDataAsync(string token)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            try
            {
                var res = await client.GetStringAsync("https://api.ivao.aero/v2/users/me");
                return JObject.Parse(res).ToString();
            }
            catch { return null; }
        }
    }
}
