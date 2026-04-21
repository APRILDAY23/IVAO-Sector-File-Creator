using System;
using System.Diagnostics;
using System.Net.Http;
using System.Windows.Forms;

namespace Sector_File
{
    public partial class CreditsForm : Form
    {
        public CreditsForm()
        {
            InitializeComponent();
            try { this.Icon = new System.Drawing.Icon("./tools.ico"); } catch { }
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await TryLoadIvaoLogoAsync();
        }

        // Try to load the IVAO PNG logo from CDN; fall back to local ivao_blue.png
        private async System.Threading.Tasks.Task TryLoadIvaoLogoAsync()
        {
            // Try local file first (ships with the project, fastest)
            if (System.IO.File.Exists("./ivao_blue.png"))
            {
                try
                {
                    logoPictureBox.Image    = System.Drawing.Image.FromFile("./ivao_blue.png");
                    logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                    return;
                }
                catch { }
            }

            // Fall back to remote PNG
            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
                byte[] data = await client.GetByteArrayAsync(
                    "https://static.ivao.aero/img/logos/logo.png");
                using var ms = new System.IO.MemoryStream(data);
                logoPictureBox.Image    = System.Drawing.Image.FromStream(ms);
                logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            }
            catch { /* keep the placeholder text */ }
        }

        private void vedaLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) =>
            Process.Start(new ProcessStartInfo
            {
                FileName        = "https://ivao.aero/Member.aspx?Id=656077",
                UseShellExecute = true
            });

        private void nilayLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) =>
            Process.Start(new ProcessStartInfo
            {
                FileName        = "https://ivao.aero/Member.aspx?Id=709833",
                UseShellExecute = true
            });

        private void airacLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) =>
            Process.Start(new ProcessStartInfo
            {
                FileName        = "https://airac.net",
                UseShellExecute = true
            });

        private void closeButton_Click(object sender, EventArgs e) =>
            this.Close();
    }
}
