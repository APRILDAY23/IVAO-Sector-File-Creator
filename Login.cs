using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace Sector_File
{
    public partial class Login : Form
    {
        private string authorizationEndpoint = "https://sso.ivao.aero/authorize";
        private string tokenEndpoint = "https://api.ivao.aero/v2/oauth/token";
        private string client_id = "737ac794-ede2-4162-aba6-7890caa9a055";
        private string client_secret = "FeET2TnosPWkWxTWzFS11duUvwS1voR0";
        private string redirect_uri = "http://localhost:5000/callback";
        private string scopes = "openid profile email";
        private string state = "12345678";

        private HttpListener listener;
        private readonly int[] allowedAccess = { 493962 }; // Replace 'xxxxxx' with additional allowed IDs

        public Login()
        {
            InitializeComponent();
            this.Icon = new Icon("./tools.ico");
            this.FormClosed += (s, e) => Application.Exit();  // Close app when login form is closed
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            // Start listening for the callback from IVAO SSO
            StartListener();

            // Construct the SSO URL and open it in the user's default browser
            string ssoUrl = $"{authorizationEndpoint}?response_type=code&client_id={client_id}&redirect_uri={redirect_uri}&scope={scopes}&state={state}";
            Process.Start(new ProcessStartInfo { FileName = ssoUrl, UseShellExecute = true });
        }

        private void StartListener()
        {
            // Start an HTTP listener to capture the callback from IVAO
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/callback/");
            listener.Start();
            listener.BeginGetContext(OnRequestReceived, listener);
        }

        private async void OnRequestReceived(IAsyncResult result)
        {
            var context = listener.EndGetContext(result);
            var request = context.Request;

            // Get the authorization code from the request URL
            string code = request.QueryString["code"];
            string stateReceived = request.QueryString["state"];
            var response = context.Response;

            if (!string.IsNullOrEmpty(code))
            {
                // Exchange the authorization code for an access token
                string token = await ExchangeCodeForToken(code);

                if (!string.IsNullOrEmpty(token))
                {
                    // Fetch the user data using the access token
                    string userInfoString = await FetchUserData(token);
                    if (!string.IsNullOrEmpty(userInfoString))
                    {
                        JObject userInfo = JObject.Parse(userInfoString); // Parse the user information JSON
                        int userId = userInfo.Value<int>("id"); // Correctly extract user ID
                        string firstName = userInfo.Value<string>("firstName"); // Extract the first name
                        string lastName = userInfo.Value<string>("lastName"); // Extract the last name

                        // Automatically close the browser window after login
                        var buffer = System.Text.Encoding.UTF8.GetBytes("<html><body><script>window.close();</script><p>If the window did not close, please close this tab.</p></body></html>");
                        response.ContentType = "text/html";
                        response.ContentLength64 = buffer.Length;
                        var output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        output.Close();

                        // Check if the user is a staff member
                        bool isStaff = userInfo.Value<bool>("isStaff"); // Extract the "isStaff" field as a boolean
                        if (isStaff || allowedAccess.Contains(userId))
                        {
                            // Use Invoke to ensure the UI updates are performed on the main thread
                            this.Invoke((MethodInvoker)delegate
                            {
                                MessageBox.Show($"Welcome {firstName} {lastName} ({userId}). You are a staff member! You can click OK to continue.",
                                    "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                this.Hide(); // Hide the login form
                                Choose chooseForm = new Choose(userId); // Pass the user ID to Form1
                                chooseForm.Show(); // Show Form1
                            });
                        }
                        else
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                MessageBox.Show("You cannot access this application because you are not a staff member",
                                    "Login successful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            });
                        }
                    }

                    else
                    {
                        MessageBox.Show("Error fetching user information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Error exchanging code for token.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Stop the listener
            listener.Stop();
        }

        private async Task<string> ExchangeCodeForToken(string code)
        {
            using (var client = new HttpClient())
            {
                var values = new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("client_id", client_id),
                    new KeyValuePair<string, string>("client_secret", client_secret),
                    new KeyValuePair<string, string>("redirect_uri", redirect_uri)
                };

                var content = new FormUrlEncodedContent(values);

                try
                {
                    var response = await client.PostAsync(tokenEndpoint, content);
                    string responseString = await response.Content.ReadAsStringAsync();

                    // Parse the JSON response to extract the access token
                    JObject tokenResponse = JObject.Parse(responseString);
                    string accessToken = tokenResponse["access_token"]?.ToString();

                    if (accessToken == null)
                    {
                        MessageBox.Show("Failed to retrieve access token.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    return accessToken;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while logging in. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
        }

        private async Task<string> FetchUserData(string accessToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                try
                {
                    var userInfoEndpoint = "https://api.ivao.aero/v2/users/me";
                    var response = await client.GetStringAsync(userInfoEndpoint);

                    // Parse the user information JSON
                    JObject userInfoJson = JObject.Parse(response);

                    // Return formatted user data
                    return userInfoJson.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to retrieve user information at this time. Please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
        }

        // To adjust the layout of the form dynamically
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // Center the login button when form resizes
            this.LoginButton.Location = new System.Drawing.Point(
                (this.ClientSize.Width - this.LoginButton.Width) / 2,
                (this.ClientSize.Height - this.LoginButton.Height) / 2
            );
        }
    }
}
