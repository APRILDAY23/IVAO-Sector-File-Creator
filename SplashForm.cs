using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Sector_File
{
    public partial class SplashForm : Form
    {
        // Shared across the app so the Login/other forms can show it without re-fetching
        public static string AiracCycle       { get; private set; } = "─ ─ ─";
        public static string AiracEffective   { get; private set; } = "";
        public static string AiracExpiry      { get; private set; } = "";
        public static int    AiracDaysLeft    { get; private set; } = -1;

        public SplashForm()
        {
            InitializeComponent();
            try { this.Icon = new System.Drawing.Icon("./tools.ico"); } catch { }
        }

        private async void SplashForm_Load(object sender, EventArgs e)
        {
            await RunStartupAsync();
            this.Close();   // Returns control to Program.cs → Application.Run(new Login())
        }

        // Colours reused across steps
        private static readonly System.Drawing.Color ColActive = System.Drawing.Color.FromArgb(13, 71, 161);
        private static readonly System.Drawing.Color ColDone   = System.Drawing.Color.FromArgb(22, 163, 74);
        private static readonly System.Drawing.Color ColFail   = System.Drawing.Color.FromArgb(220, 38, 38);
        private static readonly System.Drawing.Color ColMuted  = System.Drawing.Color.FromArgb(140, 155, 175);

        private async Task RunStartupAsync()
        {
            // Step 1 - init
            await MarkStep(step1Label, "Initializing application...", 15);
            await Task.Delay(300);
            step1Label.Text      = "✔   Application initialized";
            step1Label.ForeColor = ColDone;

            // Step 2 - network check
            await MarkStep(step2Label, "Checking network connection...", 40);
            bool online = await CheckNetworkAsync();
            step2Label.Text      = online ? "✔   Network connection OK" : "✖   No network - working offline";
            step2Label.ForeColor = online ? ColDone : ColFail;

            // Step 3 - AIRAC cycle
            await MarkStep(step3Label, "Fetching AIRAC cycle data...", 70);
            if (online) await FetchAiracCycleAsync();
            step3Label.Text      = AiracDaysLeft >= 0
                ? $"✔   AIRAC {AiracCycle}  ({AiracDaysLeft} days left)"
                : "✖   AIRAC data unavailable";
            step3Label.ForeColor = AiracDaysLeft >= 0 ? ColDone : ColMuted;

            // Step 4 - ready
            await MarkStep(step4Label, "Ready - opening login...", 100);
            step4Label.Text      = "✔   Ready";
            step4Label.ForeColor = ColDone;
            await Task.Delay(700);
        }

        // Marks a step label as active (dark blue) and animates the progress bar
        private async Task MarkStep(Label lbl, string text, int targetProgress)
        {
            lbl.ForeColor = ColActive;
            lbl.Text      = $"●   {text}";

            int current = progressBar.Value;
            for (int v = current; v <= targetProgress; v += 2)
            {
                progressBar.Value = v;
                await Task.Delay(12);
            }
            progressBar.Value = targetProgress;
        }

        private static async Task<bool> CheckNetworkAsync()
        {
            try
            {
                using HttpClient c = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
                var res = await c.GetAsync("https://airac.net/api/v1/airac/current");
                return res.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        private static async Task FetchAiracCycleAsync()
        {
            try
            {
                using HttpClient c = new HttpClient { Timeout = TimeSpan.FromSeconds(8) };
                string json = await c.GetStringAsync("https://airac.net/api/v1/airac/current");
                JObject data = (JObject.Parse(json)["data"] as JObject)!;

                AiracCycle     = data["cycle"]?.ToString()            ?? "─";
                AiracDaysLeft  = data["days_remaining"]?.Value<int>() ?? -1;

                string eff = data["effective_date"]?.ToString()   ?? "";
                string exp = data["expiration_date"]?.ToString()  ?? "";
                AiracEffective = DateTime.TryParse(eff, out DateTime e1) ? e1.ToString("dd MMM yyyy") : eff;
                AiracExpiry    = DateTime.TryParse(exp, out DateTime e2) ? e2.ToString("dd MMM yyyy") : exp;
            }
            catch { /* leave defaults */ }
        }
    }
}
