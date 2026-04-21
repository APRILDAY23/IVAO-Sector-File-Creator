using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Sector_File
{
    public partial class SidStarDataForm : Form
    {
        private int userIdInput;
        private const string ApiBaseUrl = "https://airac.net/api/v1";

        // Sector file output buffers
        private string sidOutput  = "";
        private string starOutput = "";

        // ────────────────────────────────────────────────────────────────────
        // Constructor
        // ────────────────────────────────────────────────────────────────────

        public SidStarDataForm(int userId)
        {
            userIdInput = userId;
            this.Icon = new System.Drawing.Icon("./tools.ico");
            InitializeComponent();

            searchButton.Click      += SearchButton_Click;
            downloadSidButton.Click  += DownloadSidButton_Click;
            downloadStarButton.Click += DownloadStarButton_Click;
        }

        // ────────────────────────────────────────────────────────────────────
        // Form load — fetch current AIRAC cycle and display it in the header
        // ────────────────────────────────────────────────────────────────────

        private async void SidStarDataForm_Load(object sender, EventArgs e)
        {
            await LoadAiracCycleAsync();
        }

        private async Task LoadAiracCycleAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync($"{ApiBaseUrl}/airac/current");
                    if (!response.IsSuccessStatusCode) return;

                    string json = await response.Content.ReadAsStringAsync();
                    JObject obj  = JObject.Parse(json);
                    JObject data = obj["data"] as JObject;
                    if (data == null) return;

                    string cycle        = data["cycle"]?.ToString() ?? "?";
                    int    daysLeft     = data["days_remaining"]?.Value<int>() ?? 0;
                    string effectiveRaw = data["effective_date"]?.ToString() ?? "";
                    string expiryRaw    = data["expiration_date"]?.ToString() ?? "";

                    // Parse dates for display
                    string effectiveDate = DateTime.TryParse(effectiveRaw, out DateTime eff)
                        ? eff.ToString("dd MMM yyyy") : effectiveRaw;
                    string expiryDate = DateTime.TryParse(expiryRaw, out DateTime exp)
                        ? exp.ToString("dd MMM yyyy") : expiryRaw;

                    // Colour-code days remaining: green > 14, orange 7-14, red < 7
                    System.Drawing.Color daysColor =
                        daysLeft > 14 ? System.Drawing.Color.LightBlue :
                        daysLeft > 6  ? System.Drawing.Color.Orange :
                                        System.Drawing.Color.Tomato;

                    airacCycleLabel.Text      = $"AIRAC  {cycle}";
                    airacDaysLabel.Text       = $"{effectiveDate} → {expiryDate}  ({daysLeft}d left)";
                    airacDaysLabel.ForeColor  = daysColor;
                }
            }
            catch
            {
                // Non-critical — if the cycle fetch fails, the tool still works fine
                airacCycleLabel.Text     = "AIRAC  ─ ─ ─";
                airacDaysLabel.Text      = "Cycle info unavailable";
                airacDaysLabel.ForeColor = System.Drawing.Color.Gray;
            }
        }

        // ────────────────────────────────────────────────────────────────────
        // Search button — orchestrates SID + STAR fetch
        // ────────────────────────────────────────────────────────────────────

        private async void SearchButton_Click(object sender, EventArgs e)
        {
            string icaoCode = searchBox.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(icaoCode))
            {
                MessageBox.Show("Please enter an ICAO airport code.", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Reset state
            loadingProgressBar.Value = 0;
            sidOutput  = "";
            starOutput = "";
            logRichTextBox.Clear();

            // Attribution header
            AppendLog("Sector File Conversion made by ");
            AppendLog("Veda Moola (656077)", isHyperlink: true, url: "https://ivao.aero/Member.aspx?Id=656077");
            AppendLog("\nFully tested by ");
            AppendLog("Nilay Parsodkar (709833)", isHyperlink: true, url: "https://ivao.aero/Member.aspx?Id=709833");
            AppendLog(". Please blame him for any bugs — or raise an issue on the GitHub repo.");
            AppendLog("\n⚠  NOT FOR REAL WORLD USE", bold: true, color: System.Drawing.Color.Tomato);
            AppendLog($"\n\nData sourced from a free, publicly available API.");
            AppendLog($"\nAirport     : {icaoCode}\n");

            // Ask user options up-front (SID then STAR)
            bool sidAlt   = Ask("Include altitude in SID file?",   "SID Options");
            bool sidSpd   = Ask("Include speed in SID file?",       "SID Options");
            bool sidCoord = Ask("Include coordinates in SID file?", "SID Options");

            bool starAlt   = Ask("Include altitude in STAR file?",   "STAR Options");
            bool starSpd   = Ask("Include speed in STAR file?",       "STAR Options");
            bool starCoord = Ask("Include coordinates in STAR file?", "STAR Options");

            searchButton.Enabled = false;
            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // ── SID ──────────────────────────────────────────────────
                    SetStatus("Fetching SID procedure list...");
                    AppendLog("\n── SID ─────────────────────────────────────────");
                    await FetchAndFormatProcedures(client, icaoCode, "SID",
                        sidAlt, sidSpd, sidCoord);
                    UpdateProgress(50);

                    // ── STAR ─────────────────────────────────────────────────
                    SetStatus("Fetching STAR procedure list...");
                    AppendLog("\n── STAR ────────────────────────────────────────");
                    await FetchAndFormatProcedures(client, icaoCode, "STAR",
                        starAlt, starSpd, starCoord);
                    UpdateProgress(100);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching data for '{icaoCode}':\n{ex.Message}",
                    "Network Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                searchButton.Enabled = true;
                SetStatus("Ready");
            }

            sw.Stop();
            string elapsed = sw.Elapsed.TotalSeconds > 1
                ? $"{sw.Elapsed.TotalSeconds:F2}s"
                : $"{sw.Elapsed.TotalMilliseconds:F0}ms";

            AppendLog($"\n\n✔  Done in {elapsed}", bold: true, color: System.Drawing.Color.LightGreen);
        }

        // ────────────────────────────────────────────────────────────────────
        // Fetch all procedures of a given type (SID / STAR) for an airport
        // ────────────────────────────────────────────────────────────────────

        private async Task FetchAndFormatProcedures(
            HttpClient client, string icaoCode, string type,
            bool inclAlt, bool inclSpd, bool inclCoord)
        {
            // Step 1 — collect unique identifiers (list endpoint duplicates per transition)
            var identifiers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int  page    = 1;
            bool hasMore = true;

            while (hasMore)
            {
                string url = $"{ApiBaseUrl}/procedures?airport={icaoCode}&type={type}&page={page}&per_page=100";
                HttpResponseMessage res = await client.GetAsync(url);

                if (!res.IsSuccessStatusCode)
                {
                    AppendLog($"\n  No {type} procedures found (HTTP {(int)res.StatusCode})");
                    return;
                }

                JObject listObj = JObject.Parse(await res.Content.ReadAsStringAsync());
                JArray  data    = listObj["data"] as JArray;
                if (data == null || !data.Any()) break;

                foreach (JObject proc in data)
                {
                    string id = proc["identifier"]?.ToString();
                    if (!string.IsNullOrEmpty(id)) identifiers.Add(id);
                }

                hasMore = listObj["pagination"]?["has_more"]?.Value<bool>() ?? false;
                page++;
            }

            if (!identifiers.Any())
            {
                AppendLog($"\n  No {type} procedures found for {icaoCode}");
                return;
            }

            AppendLog($"\n  Found {identifiers.Count} candidate(s) in {type} list");

            // Step 2 — fetch each procedure detail and format it
            // NOTE: The airac.net list endpoint sometimes returns wrong-type procedures
            // (e.g. STARs appear in the SID list). We verify type.code in the detail
            // response and skip any procedure that doesn't match the requested type.
            int done = 0, skipped = 0;
            foreach (string id in identifiers)
            {
                done++;
                SetStatus($"Processing {type}  {done}/{identifiers.Count}  —  {id}");

                string detailUrl = $"{ApiBaseUrl}/procedures/{icaoCode}/{id}";
                HttpResponseMessage detailRes = await client.GetAsync(detailUrl);

                if (!detailRes.IsSuccessStatusCode)
                {
                    AppendLog($"\n  Skipped {id} (HTTP {(int)detailRes.StatusCode})");
                    continue;
                }

                JObject detail   = JObject.Parse(await detailRes.Content.ReadAsStringAsync());
                JObject procData = detail["data"] as JObject;
                if (procData == null) continue;

                // Guard against mis-categorised procedures in the list endpoint
                string actualType = procData["type"]?["code"]?.ToString() ?? "";
                if (!actualType.Equals(type, StringComparison.OrdinalIgnoreCase))
                {
                    skipped++;
                    AppendLog($"\n  Skipped {id} (is {actualType}, not {type})");
                    continue;
                }

                if (type == "SID")
                    FormatSidProcedure(procData, icaoCode, id, inclAlt, inclSpd, inclCoord);
                else
                    FormatStarProcedure(procData, icaoCode, id, inclAlt, inclSpd, inclCoord);
            }

            int correct = identifiers.Count - skipped;
            AppendLog($"\n  Processed {correct} {type} procedure(s)" +
                      (skipped > 0 ? $"  ({skipped} wrong-type skipped)" : ""));
        }

        // ────────────────────────────────────────────────────────────────────
        // Format a single SID procedure into the IVAO sector file format
        //
        // Output structure (unchanged from original):
        //   // SID Start
        //   ICAO;RUNWAY;PROCNAME;MATCHFIX;MATCHFIX; //coord
        //   FIX;FIX;altFormat | speedFormat; //coord
        //   ...
        //   // TRANSITION START
        //   ICAO;RUNWAY;TRANSNAME;TRANSNAME;TRANSNAME;1;
        //   FIX;FIX;
        //   ...
        //   // TRANSITION END TRANSNAME
        //   // TRANSITION END
        //   // SID ended
        // ────────────────────────────────────────────────────────────────────

        private void FormatSidProcedure(
            JObject data, string icaoCode, string identifier,
            bool inclAlt, bool inclSpd, bool inclCoord)
        {
            try
            {
                JObject runwayTrans = data["runway_transitions"] as JObject;
                JObject transitions = data["transitions"] as JObject;
                if (runwayTrans == null || !runwayTrans.HasValues) return;

                string prefix = ProcedurePrefix(identifier);

                foreach (var rwEntry in runwayTrans)
                {
                    string runway  = rwEntry.Key;
                    JArray wpts    = rwEntry.Value as JArray;
                    if (wpts == null || !wpts.Any()) continue;

                    string matchFix  = FindMatchingFix(wpts, transitions, prefix, identifier);
                    string hdrCoord  = FirstWaypointCoord(wpts, inclCoord);

                    sidOutput += "// SID Start\n";
                    sidOutput += $"{icaoCode};{runway};{identifier};{matchFix};{matchFix};{hdrCoord}\n";

                    foreach (JObject wp in wpts)
                        sidOutput += WaypointLine(wp, inclAlt, inclSpd, inclCoord) ?? "";

                    sidOutput += "\n";

                    // Transitions (entry fixes that merge into the main route)
                    if (transitions != null && transitions.HasValues)
                    {
                        sidOutput += "// TRANSITION START\n";
                        foreach (var trEntry in transitions)
                        {
                            string trName = trEntry.Key;
                            JArray trWpts = trEntry.Value as JArray;

                            sidOutput += $"{icaoCode};{runway};{trName};{trName};{trName};1;\n";
                            if (trWpts != null)
                                foreach (JObject wp in trWpts)
                                    sidOutput += WaypointLine(wp, inclAlt, inclSpd, inclCoord) ?? "";

                            sidOutput += $"// TRANSITION END {trName}\n";
                        }
                        sidOutput += "// TRANSITION END\n";
                    }

                    sidOutput += "// SID ended\n\n";
                }
            }
            catch (Exception ex)
            {
                logRichTextBox.AppendText($"  Error parsing SID {identifier}: {ex.Message}\n");
            }
        }

        // ────────────────────────────────────────────────────────────────────
        // Format a single STAR procedure into the IVAO sector file format
        //
        // Output structure mirrors the SID format above, using STAR Start /
        // STAR ended markers.
        // ────────────────────────────────────────────────────────────────────

        private void FormatStarProcedure(
            JObject data, string icaoCode, string identifier,
            bool inclAlt, bool inclSpd, bool inclCoord)
        {
            try
            {
                JObject runwayTrans = data["runway_transitions"] as JObject;
                JObject transitions = data["transitions"] as JObject;
                if (runwayTrans == null || !runwayTrans.HasValues) return;

                string prefix = ProcedurePrefix(identifier);

                foreach (var rwEntry in runwayTrans)
                {
                    string runway = rwEntry.Key;
                    JArray wpts   = rwEntry.Value as JArray;
                    if (wpts == null || !wpts.Any()) continue;

                    string matchFix = FindMatchingFix(wpts, transitions, prefix, identifier);
                    string hdrCoord = FirstWaypointCoord(wpts, inclCoord);

                    starOutput += "// STAR Start\n";
                    starOutput += $"{icaoCode};{runway};{identifier};{matchFix};{matchFix};{hdrCoord}\n";

                    foreach (JObject wp in wpts)
                        starOutput += WaypointLine(wp, inclAlt, inclSpd, inclCoord) ?? "";

                    starOutput += "\n";

                    if (transitions != null && transitions.HasValues)
                    {
                        starOutput += "// TRANSITION START\n";
                        foreach (var trEntry in transitions)
                        {
                            string trName = trEntry.Key;
                            JArray trWpts = trEntry.Value as JArray;

                            starOutput += $"{icaoCode};{runway};{trName};{trName};{trName};1;\n";
                            if (trWpts != null)
                                foreach (JObject wp in trWpts)
                                    starOutput += WaypointLine(wp, inclAlt, inclSpd, inclCoord) ?? "";

                            starOutput += $"// TRANSITION END {trName}\n";
                        }
                        starOutput += "// TRANSITION END\n";
                    }

                    starOutput += "// STAR ended\n\n";
                }
            }
            catch (Exception ex)
            {
                logRichTextBox.AppendText($"  Error parsing STAR {identifier}: {ex.Message}\n");
            }
        }

        // ────────────────────────────────────────────────────────────────────
        // Helpers — waypoint formatting
        // ────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the 3- or 4-character prefix used to locate the "matching"
        /// waypoint whose name begins with the same characters as the procedure.
        /// </summary>
        private static string ProcedurePrefix(string name) =>
            name.Length >= 6 ? name[..4] :
            name.Length >= 3 ? name[..3] : name;

        /// <summary>
        /// Scans main-route waypoints then transition waypoints for the first
        /// fix whose identifier begins with <paramref name="prefix"/>.
        /// Falls back to <paramref name="fallback"/> if nothing matches.
        /// </summary>
        private static string FindMatchingFix(
            JArray mainWpts, JObject transitions, string prefix, string fallback)
        {
            foreach (JObject wp in mainWpts)
            {
                string n = wp["fix_identifier"]?.ToString();
                if (n != null && n.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    return n;
            }

            if (transitions != null)
            {
                foreach (var tr in transitions)
                {
                    if (tr.Value is JArray trWpts)
                        foreach (JObject wp in trWpts)
                        {
                            string n = wp["fix_identifier"]?.ToString();
                            if (n != null && n.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                                return n;
                        }
                }
            }

            return fallback;
        }

        /// <summary>
        /// Builds the optional " //N00.00.00.000;E000.00.00.000" coordinate
        /// comment from the first waypoint in a route.
        /// </summary>
        private string FirstWaypointCoord(JArray wpts, bool include)
        {
            if (!include) return "";
            double lat = wpts.First["fix_coordinates"]?["lat"]?.Value<double>() ?? 0;
            double lon = wpts.First["fix_coordinates"]?["lon"]?.Value<double>() ?? 0;
            return $" //{ToNFormat(lat)};{ToEFormat(lon)}";
        }

        /// <summary>
        /// Formats a single waypoint into a semicolon-delimited sector-file line.
        /// Returns null for waypoints that should be skipped (anonymous fixes).
        /// </summary>
        private string WaypointLine(JObject wp, bool inclAlt, bool inclSpd, bool inclCoord)
        {
            string name = wp["fix_identifier"]?.ToString();
            if (string.IsNullOrEmpty(name) || name.StartsWith("(")) return null;

            double alt    = wp["altitude_ft"]?.Value<double>() ?? 0;
            string rstr   = wp["altitude_restriction"]?.ToString();
            double speed  = wp["speed_kts"]?.Value<double>() ?? 0;
            double lat    = wp["fix_coordinates"]?["lat"]?.Value<double>() ?? 0;
            double lon    = wp["fix_coordinates"]?["lon"]?.Value<double>() ?? 0;

            string altFmt   = inclAlt ? FormatAltitude(alt, rstr) : "";
            string spdFmt   = inclSpd && speed > 0 ? $"{(int)speed}kt" : "";
            string coordCmt = inclCoord ? $" //{ToNFormat(lat)};{ToEFormat(lon)}" : "";

            bool hasAlt = !string.IsNullOrEmpty(altFmt);
            bool hasSpd = !string.IsNullOrEmpty(spdFmt);

            if (!hasAlt && !hasSpd) return $"{name};{name};{coordCmt}\n";
            if ( hasAlt &&  hasSpd) return $"{name};{name};{altFmt} | {spdFmt};{coordCmt}\n";
            if ( hasAlt)            return $"{name};{name};{altFmt};{coordCmt}\n";
            /*  hasSpd only */      return $"{name};{name};{spdFmt};{coordCmt}\n";
        }

        // ────────────────────────────────────────────────────────────────────
        // Altitude formatting  (airac.net restriction → IVAO sector file notation)
        //
        //   at_or_below  →  -FL200   (at or below)
        //   at_or_above  →  +FL200   (at or above)
        //   at           →  =FL200   (exactly at)
        //   between      →  =FL200   (single value returned by API; treat as exact)
        //   null / other →  ""       (no constraint)
        // ────────────────────────────────────────────────────────────────────

        private static string FormatAltitude(double altitude, string restriction)
        {
            if (altitude == 0 || string.IsNullOrEmpty(restriction))
                return string.Empty;

            // Altitudes > 4 000 ft are expressed as flight levels (÷100)
            string altStr = altitude > 4000
                ? $"FL{(int)(altitude / 100)}"
                : ((int)altitude).ToString();

            return restriction.ToLowerInvariant() switch
            {
                "at"          => $"={altStr}",
                "at_or_below" => $"-{altStr}",
                "at_or_above" => $"+{altStr}",
                "between"     => $"={altStr}",  // only one altitude value available
                _             => string.Empty
            };
        }

        // ────────────────────────────────────────────────────────────────────
        // Coordinate conversion  (decimal degrees → DMS with milliseconds)
        // Format: N/S DD.MM.SS.mmm   E/W DDD.MM.SS.mmm
        // ────────────────────────────────────────────────────────────────────

        private static string ToNFormat(double lat)
        {
            string h = lat >= 0 ? "N" : "S";
            lat = Math.Abs(lat);
            int    deg = (int)lat;
            double mDec = (lat - deg) * 60;
            int    min = (int)mDec;
            double sDec = (mDec - min) * 60;
            int    sec = (int)sDec;
            int    ms  = (int)((sDec - sec) * 1000) % 1000;
            return $"{h}{deg:00}.{min:00}.{sec:00}.{ms:000}";
        }

        private static string ToEFormat(double lon)
        {
            string h = lon >= 0 ? "E" : "W";
            lon = Math.Abs(lon);
            int    deg = (int)lon;
            double mDec = (lon - deg) * 60;
            int    min = (int)mDec;
            double sDec = (mDec - min) * 60;
            int    sec = (int)sDec;
            int    ms  = (int)((sDec - sec) * 1000) % 1000;
            return $"{h}{deg:000}.{min:00}.{sec:00}.{ms:000}";
        }

        // ────────────────────────────────────────────────────────────────────
        // Download buttons
        // ────────────────────────────────────────────────────────────────────

        private void DownloadSidButton_Click(object sender, EventArgs e) =>
            SaveToFile(sidOutput, "SID Files (*.SID)|*.SID", "Save SID Data", "SID");

        private void DownloadStarButton_Click(object sender, EventArgs e) =>
            SaveToFile(starOutput, "STAR Files (*.STR)|*.STR", "Save STAR Data", "STR");

        private void SaveToFile(string data, string filter, string title, string ext)
        {
            if (string.IsNullOrEmpty(data))
            {
                MessageBox.Show("No data to save. Search for an airport first.",
                    "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using SaveFileDialog dlg = new SaveFileDialog
            {
                Filter    = filter,
                Title     = $"{title}  —  {searchBox.Text.Trim().ToUpper()}",
                FileName  = $"{searchBox.Text.Trim().ToUpper()}.{ext}"
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(dlg.FileName, data);
                    MessageBox.Show($"Saved to {dlg.FileName}", "Saved",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    logRichTextBox.AppendText($"Error saving: {ex.Message}\n");
                }
            }
        }

        // ────────────────────────────────────────────────────────────────────
        // UI helpers
        // ────────────────────────────────────────────────────────────────────

        private static bool Ask(string question, string title) =>
            MessageBox.Show(question, title,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

        private void UpdateProgress(int value)
        {
            if (loadingProgressBar.InvokeRequired)
                loadingProgressBar.Invoke((MethodInvoker)(() => loadingProgressBar.Value = value));
            else
                loadingProgressBar.Value = value;
        }

        private void SetStatus(string text)
        {
            if (statusLabel.InvokeRequired)
                statusLabel.Invoke((MethodInvoker)(() => statusLabel.Text = text));
            else
                statusLabel.Text = text;
        }

        /// <summary>Appends styled text to the dark terminal log box.</summary>
        private void AppendLog(
            string text,
            bool   bold      = false,
            System.Drawing.Color? color = null,
            bool   isHyperlink = false,
            string url         = null)
        {
            if (logRichTextBox.InvokeRequired)
            {
                logRichTextBox.Invoke((MethodInvoker)(() =>
                    AppendLogDirect(text, bold, color, isHyperlink, url)));
            }
            else
            {
                AppendLogDirect(text, bold, color, isHyperlink, url);
            }
        }

        private void AppendLogDirect(
            string text, bool bold,
            System.Drawing.Color? color,
            bool isHyperlink, string url)
        {
            int start = logRichTextBox.TextLength;
            logRichTextBox.AppendText(text);
            logRichTextBox.Select(start, text.Length);

            if (isHyperlink)
            {
                logRichTextBox.SelectionColor = System.Drawing.Color.CornflowerBlue;
                logRichTextBox.SelectionFont  = new System.Drawing.Font(
                    logRichTextBox.Font, System.Drawing.FontStyle.Underline);
                logRichTextBox.Tag = url;
            }
            else
            {
                logRichTextBox.SelectionFont = bold
                    ? new System.Drawing.Font(logRichTextBox.Font, System.Drawing.FontStyle.Bold)
                    : new System.Drawing.Font(logRichTextBox.Font, System.Drawing.FontStyle.Regular);
                logRichTextBox.SelectionColor = color ?? System.Drawing.Color.FromArgb(220, 220, 220);
            }

            logRichTextBox.Select(logRichTextBox.TextLength, 0);
            logRichTextBox.ScrollToCaret();
        }

        private void debugTextBox_LinkClicked(object sender, LinkClickedEventArgs e) =>
            System.Diagnostics.Process.Start(e.LinkText);

        private void backButton_Click(object sender, EventArgs e) =>
            this.Hide();
    }
}
