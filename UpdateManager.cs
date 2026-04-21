using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Sector_File
{
    internal record UpdateInfo(
        string Version,
        string DownloadUrl,
        string ReleaseNotes,
        bool   IsPreRelease);

    internal static class UpdateManager
    {
        private const string ReleasesApi =
            "https://api.github.com/repos/APRILDAY23/IVAO-Sector-File-Creator/releases";
        private const string UserAgent = "IVAO-Sector-File-Creator-Updater";

        public static string Channel
        {
            get => ConfigManager.UpdateChannel;
            set => ConfigManager.UpdateChannel = value;
        }

        // Check GitHub for a newer release on the current channel.
        // Returns null if already up-to-date or on a network error.
        public static async Task<UpdateInfo?> CheckAsync()
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                client.Timeout = TimeSpan.FromSeconds(15);

                string json     = await client.GetStringAsync(ReleasesApi);
                var    releases = JArray.Parse(json);
                bool   wantBeta = Channel == "beta";
                var    current  = AppInfo.AssemblyVersion;

                foreach (JObject rel in releases)
                {
                    bool isPreRelease = rel["prerelease"]?.Value<bool>() ?? false;
                    if (isPreRelease && !wantBeta) continue; // stable channel skips beta

                    string tag        = rel["tag_name"]?.ToString() ?? "";
                    string verStr     = tag.TrimStart('v').Split('-')[0]; // "1.2.0-beta.1" → "1.2.0"
                    if (!Version.TryParse(verStr, out var latest)) continue;
                    if (latest <= current) continue; // already newest

                    // Find the installer .exe asset
                    string? url = null;
                    foreach (JObject asset in rel["assets"] as JArray ?? new JArray())
                    {
                        string name = asset["name"]?.ToString() ?? "";
                        if (name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                        { url = asset["browser_download_url"]?.ToString(); break; }
                    }
                    if (string.IsNullOrEmpty(url)) continue;

                    return new UpdateInfo(tag, url!, rel["body"]?.ToString() ?? "", isPreRelease);
                }
                return null;
            }
            catch { return null; }
        }

        // Download the installer to %TEMP% and launch it silently.
        // Inno Setup /SILENT closes the running app and restarts after install.
        public static async Task DownloadAndInstallAsync(
            UpdateInfo info, IProgress<int>? progress = null)
        {
            string tmp = Path.Combine(Path.GetTempPath(),
                $"IVAO-SFC-Setup-{info.Version}.exe");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            client.Timeout = TimeSpan.FromMinutes(10);

            using var response = await client.GetAsync(
                info.DownloadUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            long total = response.Content.Headers.ContentLength ?? -1;
            long received = 0;

            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var file   = File.Create(tmp))
            {
                byte[] buf = new byte[81920];
                int    read;
                while ((read = await stream.ReadAsync(buf)) > 0)
                {
                    await file.WriteAsync(buf.AsMemory(0, read));
                    received += read;
                    if (total > 0) progress?.Report((int)(received * 100 / total));
                }
            }

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName        = tmp,
                Arguments       = "/SILENT /CLOSEAPPLICATIONS /RESTARTAPPLICATIONS",
                UseShellExecute = true,
            });
            Application.Exit();
        }
    }
}
