using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Sector_File
{
    public partial class airportDataForm : Form
    {
        private int    userIdInput;
        private const string ApiBaseUrl = "https://airac.net/api/v1";

        private string airportDataOutput = "";
        private string runwayOutput      = "";
        private string frequencyOutput   = "";
        private string vordmeOutput      = "";
        private string ndbOutput         = "";
        private string gateOutput        = ""; // airac.net has no gate data

        // ────────────────────────────────────────────────────────────────────
        public airportDataForm(int userId)
        {
            userIdInput = userId;
            try { this.Icon = new Icon("./tools.ico"); } catch { }
            InitializeComponent();

            searchButton.Click         += SearchButton_Click;
            downloadAirportData.Click  += DownloadAirportDataFile;
            downloadRunwayData.Click   += DownloadRunwayDataFile;
            airportFrequencyData.Click += DownloadFrequencyDataFile;
            downloadVORDMEData.Click   += DownloadVORDMEDataFile;
            downloadNDBData.Click      += DownloadNDBDataFile;
            downloadGateData.Click     += DownloadGateDataFile;
            backButton.Click           += (s, e) => this.Hide();
        }

        // ────────────────────────────────────────────────────────────────────
        //  Search - orchestrates all fetches for a 2-letter FIR/region prefix
        // ────────────────────────────────────────────────────────────────────
        private async void SearchButton_Click(object sender, EventArgs e)
        {
            string region = searchBox.Text.Trim().ToUpper();
            if (region.Length != 2)
            {
                MessageBox.Show("Please enter a 2-letter ICAO region/FIR prefix (e.g. VT for Thailand).",
                    "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Reset outputs
            airportDataOutput = runwayOutput = frequencyOutput =
                vordmeOutput = ndbOutput = gateOutput = "";
            logRichTextBox.Clear();
            UpdateProgress(0);
            searchButton.Enabled = false;

            // Attribution header
            AppendLog("Sector File Conversion made by ");
            AppendLog("Veda Moola (656077)", isHyperlink: true,
                url: "https://ivao.aero/Member.aspx?Id=656077");
            AppendLog("\nFully tested by ");
            AppendLog("Nilay Parsodkar (709833)", isHyperlink: true,
                url: "https://ivao.aero/Member.aspx?Id=709833");
            AppendLog("\n⚠  NOT FOR REAL WORLD USE", bold: true, color: Color.Red);
            AppendLog($"\n\nAIRAC Cycle : {SplashForm.AiracCycle}  ·  Data sourced from a free API");
            AppendLog($"\nRegion      : {region}XX FIR\n");

            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                using HttpClient client = new HttpClient();

                // ── Airports in region (paginated, filter by ICAO prefix) ─────
                AppendLog($"\nFetching airports in region {region}...");
                var airports = await FetchAirportsInRegionAsync(client, region);
                UpdateProgress(10);

                AppendLog($"\n  Found {airports.Count} airport(s) - fetching details...");

                // ── Per-airport: runways + frequencies ────────────────────────
                var apLines  = new List<string>();
                var rwLines  = new List<string>();
                var frqLines = new List<string>();
                var seenFreq = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                int done = 0;
                foreach (var ap in airports)
                {
                    done++;
                    AppendLog($"\n  [{done}/{airports.Count}] {ap.Icao}");
                    apLines.Add(
                        $"{ap.Icao};{ap.ElevFt};{ToN(ap.Lat)};{ToE(ap.Lon)};{ap.Name};");

                    await FetchRunwaysAndFreqsAsync(
                        client, ap, rwLines, frqLines, seenFreq);

                    // Progress: 10-50% over airport detail fetches
                    UpdateProgress(10 + (int)(40.0 * done / airports.Count));
                }

                airportDataOutput = string.Join(Environment.NewLine, apLines);
                runwayOutput      = string.Join(Environment.NewLine, rwLines);
                frequencyOutput   = string.Join(Environment.NewLine, frqLines);
                UpdateProgress(50);

                // ── VOR ───────────────────────────────────────────────────────
                AppendLog($"\nFetching VOR navaids (region {region})...");
                var vorLines    = await FetchNavaidsAsync(client, region, "VOR",    vorType: 1);
                UpdateProgress(62);

                // ── VOR/DME ───────────────────────────────────────────────────
                AppendLog($"\nFetching VOR/DME navaids (region {region})...");
                var vordmeLines = await FetchNavaidsAsync(client, region, "VORDME", vorType: 2);
                UpdateProgress(74);

                // ── DME ───────────────────────────────────────────────────────
                AppendLog($"\nFetching DME navaids (region {region})...");
                var dmeLines    = await FetchNavaidsAsync(client, region, "DME",    vorType: 4);
                UpdateProgress(86);

                // ── NDB ───────────────────────────────────────────────────────
                AppendLog($"\nFetching NDB navaids (region {region})...");
                var ndbLines    = await FetchNavaidsAsync(client, region, "NDB",    vorType: -1);
                UpdateProgress(98);

                // Merge all VHF navaids into one output file
                var allVhf = new List<string>();
                allVhf.AddRange(vorLines);
                allVhf.AddRange(vordmeLines);
                allVhf.AddRange(dmeLines);
                vordmeOutput = string.Join(Environment.NewLine, allVhf);
                ndbOutput    = string.Join(Environment.NewLine, ndbLines);

                UpdateProgress(100);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching data:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                searchButton.Enabled = true;
            }

            sw.Stop();
            string elapsed = sw.Elapsed.TotalSeconds > 1
                ? $"{sw.Elapsed.TotalSeconds:F2}s"
                : $"{sw.Elapsed.TotalMilliseconds:F0}ms";
            AppendLog($"\n\n✔  Done in {elapsed}", bold: true, color: Color.LawnGreen);
        }

        // ────────────────────────────────────────────────────────────────────
        //  Fetch all airports whose ICAO code starts with the 2-letter prefix.
        //  Tries /airports?region={prefix} first; if unsupported falls back to
        //  paginating through all airports and filtering client-side.
        // ────────────────────────────────────────────────────────────────────
        private record AirportInfo(
            string Icao, string Name,
            double Lat,  double Lon,  int ElevFt);

        private async Task<List<AirportInfo>> FetchAirportsInRegionAsync(
            HttpClient client, string region)
        {
            var results = new List<AirportInfo>();

            // Try region parameter first (supported on some airac.net builds)
            string regionUrl = $"{ApiBaseUrl}/airports?region={region}&per_page=100&page=1";
            HttpResponseMessage probe = await client.GetAsync(regionUrl);
            bool hasRegionFilter = false;

            if (probe.IsSuccessStatusCode)
            {
                JObject obj  = JObject.Parse(await probe.Content.ReadAsStringAsync());
                JArray  data = obj["data"] as JArray;
                // Only treat as valid if we get actual airport objects back
                if (data != null && data.Count > 0 &&
                    (data[0]["icao"] != null || data[0]["identifier"] != null))
                {
                    hasRegionFilter = true;
                    ExtractAirports(data, region, results);

                    bool hasMore = obj["pagination"]?["has_more"]?.Value<bool>() ?? false;
                    int  page    = 2;
                    while (hasMore)
                    {
                        string url = $"{ApiBaseUrl}/airports?region={region}" +
                                     $"&per_page=100&page={page}";
                        HttpResponseMessage res = await client.GetAsync(url);
                        if (!res.IsSuccessStatusCode) break;

                        JObject pg  = JObject.Parse(await res.Content.ReadAsStringAsync());
                        JArray  pgd = pg["data"] as JArray;
                        if (pgd == null || pgd.Count == 0) break;

                        ExtractAirports(pgd, region, results);
                        hasMore = pg["pagination"]?["has_more"]?.Value<bool>() ?? false;
                        page++;
                    }
                }
            }

            // Fallback: paginate all airports and filter by ICAO prefix (max 30 pages)
            if (!hasRegionFilter)
            {
                AppendLog("\n  (region filter unsupported - scanning global list)");
                const int maxPages = 30;
                for (int p = 1; p <= maxPages; p++)
                {
                    string url = $"{ApiBaseUrl}/airports?per_page=100&page={p}";
                    HttpResponseMessage res = await client.GetAsync(url);
                    if (!res.IsSuccessStatusCode) break;

                    JObject obj  = JObject.Parse(await res.Content.ReadAsStringAsync());
                    JArray  data = obj["data"] as JArray;
                    if (data == null || data.Count == 0) break;

                    ExtractAirports(data, region, results);

                    bool hasMore = obj["pagination"]?["has_more"]?.Value<bool>() ?? false;
                    if (!hasMore) break;
                }
            }

            return results;
        }

        private static void ExtractAirports(
            JArray data, string region, List<AirportInfo> results)
        {
            foreach (JObject ap in data)
            {
                string icao = ap["icao"]?.ToString()
                           ?? ap["identifier"]?.ToString()
                           ?? "";
                if (string.IsNullOrEmpty(icao)) continue;
                if (!icao.StartsWith(region, StringComparison.OrdinalIgnoreCase)) continue;

                results.Add(new AirportInfo(
                    Icao:   icao,
                    Name:   ap["name"]?.ToString()            ?? "",
                    Lat:    ap["latitude"]?.Value<double>()   ?? 0,
                    Lon:    ap["longitude"]?.Value<double>()  ?? 0,
                    ElevFt: ap["elevation_ft"]?.Value<int>()  ?? 0));
            }
        }

        // ────────────────────────────────────────────────────────────────────
        //  Fetch runways + frequencies for one airport (embedded in detail
        //  endpoint).  Appends formatted lines into the shared output lists.
        // ────────────────────────────────────────────────────────────────────
        private async Task FetchRunwaysAndFreqsAsync(
            HttpClient client, AirportInfo ap,
            List<string> rwLines, List<string> frqLines,
            HashSet<string> seenFreq)
        {
            HttpResponseMessage res =
                await client.GetAsync($"{ApiBaseUrl}/airports/{ap.Icao}");
            if (!res.IsSuccessStatusCode) return;

            JObject root    = JObject.Parse(await res.Content.ReadAsStringAsync());
            JObject airport = root["data"] as JObject ?? root;

            // Runways
            JArray runways = airport["runways"] as JArray ?? new JArray();
            foreach (JObject rw in runways)
            {
                string baseId   = rw["base_identifier"]?.ToString()        ?? "";
                string recipId  = rw["reciprocal_identifier"]?.ToString()  ?? "";
                double baseBrng = rw["base_bearing"]?.Value<double>()      ?? 0;
                double recipBrng= rw["reciprocal_bearing"]?.Value<double>() ?? 0;
                double lengthFt = rw["length_ft"]?.Value<double>()         ?? 0;

                // Approximate threshold positions from airport reference point.
                // Each threshold is offset half the runway length from the airport
                // centre in the direction that runway end faces.
                double halfNm = (lengthFt / 2.0) / 6076.12;
                var (bLat, bLon) = GeoOffset(ap.Lat, ap.Lon, recipBrng, halfNm);
                var (rLat, rLon) = GeoOffset(ap.Lat, ap.Lon, baseBrng,  halfNm);

                rwLines.Add($"{ap.Icao};{baseId};{recipId};" +
                            $"{baseBrng:F0};{recipBrng:F0};" +
                            $"{ToN(bLat)};{ToE(bLon)};" +
                            $"{ToN(rLat)};{ToE(rLon)};");
            }

            // Frequencies
            JArray freqs = airport["frequencies"] as JArray ?? new JArray();
            foreach (JObject freq in freqs)
            {
                string ftype = (freq["type"]?.ToString() ?? "COMM").ToUpper();
                double fmhz  = freq["frequency_mhz"]?.Value<double>() ?? 0;

                if (ftype.EndsWith("ATI") && !ftype.EndsWith("ATIS"))
                    ftype += "S";

                string key = $"{ap.Icao}_{ftype}";
                if (!seenFreq.Add(key)) continue;
                frqLines.Add($"{key};{fmhz:F3};");
            }
        }

        // ────────────────────────────────────────────────────────────────────
        //  Navaids - paginated, filtered by ICAO region prefix
        //  vorType:  1=VOR  2=VOR/DME  4=DME  -1=NDB
        // ────────────────────────────────────────────────────────────────────
        private async Task<List<string>> FetchNavaidsAsync(
            HttpClient client, string region, string apiType, int vorType)
        {
            var lines = new List<string>();
            var seen  = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int  page    = 1;
            bool hasMore = true;

            while (hasMore)
            {
                string url = $"{ApiBaseUrl}/navaids?type={apiType}" +
                             $"&region={region}&per_page=100&page={page}";
                HttpResponseMessage res = await client.GetAsync(url);
                if (!res.IsSuccessStatusCode) break;

                JObject obj  = JObject.Parse(await res.Content.ReadAsStringAsync());
                JArray  data = obj["data"] as JArray;
                if (data == null || data.Count == 0) break;

                foreach (JObject nav in data)
                {
                    string ident = nav["identifier"]?.ToString()
                                ?? nav["ident"]?.ToString()
                                ?? "";
                    if (string.IsNullOrEmpty(ident) || !seen.Add(ident)) continue;

                    double lat = nav["latitude"]?.Value<double>()  ?? 0;
                    double lon = nav["longitude"]?.Value<double>() ?? 0;

                    if (vorType == -1)
                    {
                        // NDB - frequency in kHz
                        double fkhz = nav["frequency_khz"]?.Value<double>()
                                   ?? nav["frequency"]?.Value<double>()
                                   ?? 0;
                        lines.Add($"{ident};{fkhz:F1};{ToN(lat)};{ToE(lon)};");
                    }
                    else
                    {
                        // VOR / VOR/DME / DME - frequency in MHz
                        double fmhz = nav["frequency_mhz"]?.Value<double>()
                                   ?? nav["frequency"]?.Value<double>()
                                   ?? 0;
                        lines.Add($"{ident};{fmhz:F3};{ToN(lat)};{ToE(lon)};1;{vorType};");
                    }
                }

                hasMore = obj["pagination"]?["has_more"]?.Value<bool>() ?? false;
                page++;
            }

            AppendLog($"\n  {apiType,-8}: {lines.Count} navaids");
            return lines;
        }

        // ────────────────────────────────────────────────────────────────────
        //  Haversine offset - lat/lon → new point at bearing + distance (nm)
        // ────────────────────────────────────────────────────────────────────
        private static (double lat, double lon) GeoOffset(
            double lat0, double lon0, double bearingDeg, double distNm)
        {
            const double R = 3440.065; // Earth radius in nm
            double φ1 = lat0 * Math.PI / 180;
            double λ1 = lon0 * Math.PI / 180;
            double θ  = bearingDeg * Math.PI / 180;
            double δ  = distNm / R;

            double φ2 = Math.Asin(Math.Sin(φ1) * Math.Cos(δ) +
                                   Math.Cos(φ1) * Math.Sin(δ) * Math.Cos(θ));
            double λ2 = λ1 + Math.Atan2(Math.Sin(θ) * Math.Sin(δ) * Math.Cos(φ1),
                                          Math.Cos(δ) - Math.Sin(φ1) * Math.Sin(φ2));
            return (φ2 * 180 / Math.PI, λ2 * 180 / Math.PI);
        }

        // ────────────────────────────────────────────────────────────────────
        //  Coordinate conversion  →  DMS with milliseconds
        //  Format: N/S DD.MM.SS.mmm   E/W DDD.MM.SS.mmm
        // ────────────────────────────────────────────────────────────────────
        private static string ToN(double lat)
        {
            string h  = lat >= 0 ? "N" : "S";
            lat = Math.Abs(lat);
            int    deg  = (int)lat;
            double mDec = (lat - deg) * 60;
            int    min  = (int)mDec;
            double sDec = (mDec - min) * 60;
            int    sec  = (int)sDec;
            int    ms   = (int)((sDec - sec) * 1000) % 1000;
            return $"{h}{deg:00}.{min:00}.{sec:00}.{ms:000}";
        }

        private static string ToE(double lon)
        {
            string h  = lon >= 0 ? "E" : "W";
            lon = Math.Abs(lon);
            int    deg  = (int)lon;
            double mDec = (lon - deg) * 60;
            int    min  = (int)mDec;
            double sDec = (mDec - min) * 60;
            int    sec  = (int)sDec;
            int    ms   = (int)((sDec - sec) * 1000) % 1000;
            return $"{h}{deg:000}.{min:00}.{sec:00}.{ms:000}";
        }

        // ────────────────────────────────────────────────────────────────────
        //  Download handlers
        // ────────────────────────────────────────────────────────────────────
        private void DownloadAirportDataFile(object sender, EventArgs e) =>
            SaveData(airportDataOutput, "Airport Data Files (*.ap)|*.ap",
                "Save Airport Data", "ap");

        private void DownloadRunwayDataFile(object sender, EventArgs e) =>
            SaveData(runwayOutput, "Runway Data Files (*.rw)|*.rw",
                "Save Runway Data", "rw");

        private void DownloadFrequencyDataFile(object sender, EventArgs e) =>
            SaveData(frequencyOutput, "Frequency Files (*.atc)|*.atc",
                "Save Frequency Data", "atc");

        private void DownloadVORDMEDataFile(object sender, EventArgs e) =>
            SaveData(vordmeOutput, "VOR/DME Files (*.vor)|*.vor",
                "Save VOR/DME Data", "vor");

        private void DownloadNDBDataFile(object sender, EventArgs e) =>
            SaveData(ndbOutput, "NDB Files (*.ndb)|*.ndb",
                "Save NDB Data", "ndb");

        private void DownloadGateDataFile(object sender, EventArgs e) =>
            MessageBox.Show("Gate data is not currently available.",
                "Not Available", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void SaveData(string data, string filter, string title, string ext)
        {
            if (string.IsNullOrEmpty(data))
            {
                MessageBox.Show("No data to download. Search first.",
                    "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string term = searchBox.Text.Trim().ToUpper();
            using SaveFileDialog dlg = new SaveFileDialog
            {
                Filter   = filter,
                Title    = $"{title}  -  {term}XX FIR",
                FileName = $"{term}.{ext}"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(dlg.FileName, data);
                MessageBox.Show($"Saved to {dlg.FileName}", "Saved",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ────────────────────────────────────────────────────────────────────
        //  Log helpers
        // ────────────────────────────────────────────────────────────────────
        private void AppendLog(string text, bool bold = false, Color? color = null,
            bool isHyperlink = false, string url = null)
        {
            if (logRichTextBox.InvokeRequired)
                logRichTextBox.Invoke((MethodInvoker)(() =>
                    AppendLogDirect(text, bold, color, isHyperlink, url)));
            else
                AppendLogDirect(text, bold, color, isHyperlink, url);
        }

        private void AppendLogDirect(string text, bool bold, Color? color,
            bool isHyperlink, string url)
        {
            int start = logRichTextBox.TextLength;
            logRichTextBox.AppendText(text);
            logRichTextBox.Select(start, text.Length);

            if (isHyperlink)
            {
                logRichTextBox.SelectionColor = Color.CornflowerBlue;
                logRichTextBox.SelectionFont  =
                    new Font(logRichTextBox.Font, FontStyle.Underline);
                logRichTextBox.Tag = url;
            }
            else
            {
                logRichTextBox.SelectionFont = bold
                    ? new Font(logRichTextBox.Font, FontStyle.Bold)
                    : new Font(logRichTextBox.Font, FontStyle.Regular);
                logRichTextBox.SelectionColor = color ?? Color.Black;
            }

            logRichTextBox.Select(logRichTextBox.TextLength, 0);
            logRichTextBox.ScrollToCaret();
        }

        private void UpdateProgress(int value)
        {
            if (loadingProgressBar.InvokeRequired)
                loadingProgressBar.Invoke((MethodInvoker)(() =>
                    loadingProgressBar.Value = value));
            else
                loadingProgressBar.Value = value;
        }

        private void debugTextBox_LinkClicked(object sender, LinkClickedEventArgs e) =>
            System.Diagnostics.Process.Start(e.LinkText);
    }
}
