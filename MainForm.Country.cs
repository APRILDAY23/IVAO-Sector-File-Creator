using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Sector_File
{
    // ─────────────────────────────────────────────────────────────────────────
    //  Country Data tool logic  - embedded panel in MainForm
    //
    //  Country list:   airac.net /countries  (paginated, cached after first load)
    //  Boundary data:  datahub.io countries GeoJSON  (cached after first fetch)
    //  Output format:  Aurora .artcc  T;ISO_CODE;LAT;LON;
    // ─────────────────────────────────────────────────────────────────────────
    partial class MainForm
    {
        // ── Constants ─────────────────────────────────────────────────────────
        private const string CountryGeoUrl =
            "https://r2.datahub.io/clvyjaryy0000la0cxieg4o8o/main/raw/data/countries.geojson";

        // ── State ─────────────────────────────────────────────────────────────
        private string _countryGeoJson  = null;   // cached GeoJSON blob
        private string _countryOutput   = "";     // last generated artcc text
        private string _selectedCountry = "";     // human name, e.g. "Thailand"
        private string _selectedCode    = "";     // ISO-2, e.g. "TH"
        private bool   _cntDropdownSuppressChange = false;

        // Country list loaded from airac.net (name, ISO-2 code)
        private List<(string Name, string Code)> _apiCountries = null;

        // ─────────────────────────────────────────────────────────────────────
        //  Page load
        // ─────────────────────────────────────────────────────────────────────

        internal void OnCountryPageShown()
        {
            if (_apiCountries != null) return;

            // Seed with built-in list immediately so the dropdown works right away
            _apiCountries = CntBuiltInCountries();
            CntStatus($"{_apiCountries.Count} countries loaded - type to search");

            // Try to enrich from airac.net in the background (silently, no blocking)
            _ = CntEnrichFromApiAsync();
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Background API enrichment (replaces list if API returns more entries)
        // ─────────────────────────────────────────────────────────────────────

        private async Task CntEnrichFromApiAsync()
        {
            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(8) };
                client.DefaultRequestHeaders.Add("User-Agent", "IVAO-SectorFileCreator/1.0");

                var countries = new List<(string Name, string Code)>();
                int page     = 1;
                bool hasMore = true;

                while (hasMore)
                {
                    string url  = $"{ApApiBase}/countries?per_page=100&page={page}";
                    string json = await client.GetStringAsync(url);
                    var root    = JObject.Parse(json);
                    var data    = root["data"] as JArray;
                    if (data == null) break;

                    foreach (JObject c in data)
                    {
                        string name = c["name"]?.ToString() ?? "";
                        string code = c["code"]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(code))
                            countries.Add((name, code));
                    }

                    hasMore = root["pagination"]?["has_more"]?.Value<bool>() == true;
                    page++;
                }

                if (countries.Count > _apiCountries.Count)
                {
                    countries.Sort((a, b) =>
                        string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
                    _apiCountries = countries;
                    CntStatus($"{_apiCountries.Count} countries loaded - type to search");
                }
            }
            catch { /* silent - built-in list is sufficient */ }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Built-in country list (instant fallback)
        // ─────────────────────────────────────────────────────────────────────

        private static List<(string Name, string Code)> CntBuiltInCountries()
        {
            var list = new List<(string Name, string Code)>
            {
                ("Afghanistan","AF"),("Albania","AL"),("Algeria","DZ"),("Angola","AO"),
                ("Argentina","AR"),("Armenia","AM"),("Australia","AU"),("Austria","AT"),
                ("Azerbaijan","AZ"),("Bahrain","BH"),("Bangladesh","BD"),("Belarus","BY"),
                ("Belgium","BE"),("Bolivia","BO"),("Bosnia and Herzegovina","BA"),
                ("Botswana","BW"),("Brazil","BR"),("Bulgaria","BG"),("Cambodia","KH"),
                ("Cameroon","CM"),("Canada","CA"),("Chile","CL"),("China","CN"),
                ("Colombia","CO"),("Croatia","HR"),("Cuba","CU"),("Czech Republic","CZ"),
                ("Denmark","DK"),("Ecuador","EC"),("Egypt","EG"),("Estonia","EE"),
                ("Ethiopia","ET"),("Finland","FI"),("France","FR"),("Georgia","GE"),
                ("Germany","DE"),("Ghana","GH"),("Greece","GR"),("Guatemala","GT"),
                ("Honduras","HN"),("Hungary","HU"),("Iceland","IS"),("India","IN"),
                ("Indonesia","ID"),("Iran","IR"),("Iraq","IQ"),("Ireland","IE"),
                ("Israel","IL"),("Italy","IT"),("Japan","JP"),("Jordan","JO"),
                ("Kazakhstan","KZ"),("Kenya","KE"),("Kuwait","KW"),("Kyrgyzstan","KG"),
                ("Latvia","LV"),("Lebanon","LB"),("Libya","LY"),("Lithuania","LT"),
                ("Luxembourg","LU"),("Malaysia","MY"),("Mexico","MX"),("Moldova","MD"),
                ("Mongolia","MN"),("Morocco","MA"),("Myanmar","MM"),("Namibia","NA"),
                ("Nepal","NP"),("Netherlands","NL"),("New Zealand","NZ"),("Nigeria","NG"),
                ("North Korea","KP"),("Norway","NO"),("Oman","OM"),("Pakistan","PK"),
                ("Peru","PE"),("Philippines","PH"),("Poland","PL"),("Portugal","PT"),
                ("Qatar","QA"),("Romania","RO"),("Russia","RU"),("Saudi Arabia","SA"),
                ("Serbia","RS"),("Singapore","SG"),("Slovakia","SK"),("Slovenia","SI"),
                ("South Africa","ZA"),("South Korea","KR"),("Spain","ES"),
                ("Sri Lanka","LK"),("Sudan","SD"),("Sweden","SE"),("Switzerland","CH"),
                ("Syria","SY"),("Taiwan","TW"),("Tajikistan","TJ"),("Tanzania","TZ"),
                ("Thailand","TH"),("Tunisia","TN"),("Turkey","TR"),("Turkmenistan","TM"),
                ("Uganda","UG"),("Ukraine","UA"),("United Arab Emirates","AE"),
                ("United Kingdom","GB"),("United States","US"),("Uzbekistan","UZ"),
                ("Venezuela","VE"),("Vietnam","VN"),("Yemen","YE"),
                ("Zambia","ZM"),("Zimbabwe","ZW"),
            };
            list.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            return list;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Populate dropdown list
        // ─────────────────────────────────────────────────────────────────────

        private void CntPopulateDropdown(string filter)
        {
            if (_apiCountries == null) return;

            cntDropdownListBox.BeginUpdate();
            cntDropdownListBox.Items.Clear();
            foreach (var (name, code) in _apiCountries)
            {
                if (name.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    code.Contains(filter,  StringComparison.OrdinalIgnoreCase))
                {
                    cntDropdownListBox.Items.Add($"{name}  ({code})");
                }
            }
            cntDropdownListBox.EndUpdate();
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Dropdown visibility
        // ─────────────────────────────────────────────────────────────────────

        private void CntShowDropdown()
        {
            // Position overlay just below the dropdown wrapper, relative to contentPanel
            var pt   = cntDropdownWrapper.PointToScreen(Point.Empty);
            var pt2  = contentPanel.PointToScreen(Point.Empty);
            int x    = pt.X - pt2.X;
            int y    = pt.Y - pt2.Y + cntDropdownWrapper.Height + 2;
            int w    = cntDropdownWrapper.Width;
            int rows = Math.Min(cntDropdownListBox.Items.Count, 10);
            int h    = rows * cntDropdownListBox.ItemHeight + 4;
            cntDropdownOverlay.SetBounds(x, y, w, h);
            cntDropdownOverlay.Visible = true;
            cntDropdownOverlay.BringToFront();
        }

        private void CntHideDropdown()
        {
            cntDropdownOverlay.Visible = false;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Event: dropdown TextBox changed → filter list
        // ─────────────────────────────────────────────────────────────────────

        private void CntDropdown_TextChanged(object sender, EventArgs e)
        {
            if (_cntDropdownSuppressChange) return;

            string text = cntDropdown.Text.Trim();
            if (string.IsNullOrEmpty(text) || _apiCountries == null)
            {
                CntHideDropdown();
                return;
            }

            CntPopulateDropdown(text);

            if (cntDropdownListBox.Items.Count > 0)
                CntShowDropdown();
            else
                CntHideDropdown();
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Event: item clicked in dropdown list → select country
        // ─────────────────────────────────────────────────────────────────────

        private void CntDropdownList_MouseClick(object sender, MouseEventArgs e)
        {
            if (cntDropdownListBox.SelectedItem is not string item) return;

            int paren = item.IndexOf("  (");
            _selectedCountry = paren > 0 ? item[..paren].Trim() : item;
            _selectedCode    = (paren > 0 && item.EndsWith(")"))
                                   ? item[(paren + 3)..^1].Trim()
                                   : "";

            _cntDropdownSuppressChange = true;
            cntDropdown.Text = item;
            _cntDropdownSuppressChange = false;
            cntDropdown.SelectionStart  = item.Length;

            CntHideDropdown();
            cntDropdown.Focus();
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Event: Fetch button clicked
        // ─────────────────────────────────────────────────────────────────────

        private async void CntSearch_Click(object sender, EventArgs e)
        {
            // Resolve country from current selection or typed text
            if (string.IsNullOrEmpty(_selectedCountry) || string.IsNullOrEmpty(_selectedCode))
            {
                // Try to match typed text against loaded list
                string typed = cntDropdown.Text.Trim();
                if (string.IsNullOrEmpty(typed))
                {
                    MessageBox.Show("Please type a country name or ISO code and select it from the list.",
                        "No Country Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_apiCountries == null)
                {
                    MessageBox.Show("Country list is still loading. Please wait a moment and try again.",
                        "Loading", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var exact = _apiCountries.FirstOrDefault(x =>
                    x.Name.Equals(typed, StringComparison.OrdinalIgnoreCase) ||
                    x.Code.Equals(typed, StringComparison.OrdinalIgnoreCase));
                if (exact.Name != null)
                {
                    _selectedCountry = exact.Name;
                    _selectedCode    = exact.Code;
                }
                else
                {
                    // Partial match
                    var partial = _apiCountries.FirstOrDefault(x =>
                        x.Name.Contains(typed, StringComparison.OrdinalIgnoreCase) ||
                        x.Code.Contains(typed,  StringComparison.OrdinalIgnoreCase));
                    if (partial.Name != null)
                    {
                        _selectedCountry = partial.Name;
                        _selectedCode    = partial.Code;
                    }
                    else
                    {
                        MessageBox.Show("No matching country found. Please select from the dropdown list.",
                            "Country Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            CntHideDropdown();

            // Reset UI
            cntLogBox.Clear();
            cntDownloadButton.Enabled = false;
            _countryOutput = "";

            CntShowProgress(true);
            CntProgress(5, $"Fetching boundary for {_selectedCountry}  ({_selectedCode})…");
            CntLog($"Country:  {_selectedCountry}  [{_selectedCode}]\n");

            var sw = Stopwatch.StartNew();

            try
            {
                // Download GeoJSON (cached after first fetch)
                if (_countryGeoJson == null)
                {
                    CntProgress(15, "Downloading country GeoJSON dataset…");
                    CntLog("Downloading country GeoJSON…");
                    using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(90) };
                    client.DefaultRequestHeaders.Add("User-Agent", "IVAO-SectorFileCreator/1.0");
                    _countryGeoJson = await client.GetStringAsync(CountryGeoUrl);
                    CntLog("  done.\n");
                }

                CntProgress(45, $"Parsing geometry for {_selectedCode}…");

                using var doc = JsonDocument.Parse(_countryGeoJson);
                var root = doc.RootElement;

                // ── Pass 1: standard Natural Earth property names ──────────────
                JsonElement match = default;
                string dbgFirst   = "";

                foreach (var f in root.GetProperty("features").EnumerateArray())
                {
                    if (!f.TryGetProperty("properties", out var props)) continue;

                    // Try uppercase then lowercase variants for admin name
                    string admin = CntGetProp(props,
                        "ADMIN", "admin", "NAME", "name",
                        "SOVEREIGNT", "sovereignt", "COUNTRY", "country");

                    // Try uppercase then lowercase for ISO code
                    string iso = CntGetProp(props, "ISO_A2", "iso_a2");
                    if (string.IsNullOrEmpty(iso) || iso == "-99")
                        iso = CntGetProp(props, "ISO_A2_EH", "iso_a2_eh");

                    if (string.IsNullOrEmpty(dbgFirst)) dbgFirst = admin;

                    if ((!string.IsNullOrEmpty(admin) &&
                         admin.Contains(_selectedCountry, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(_selectedCode) &&
                         iso.Equals(_selectedCode, StringComparison.OrdinalIgnoreCase)))
                    {
                        match = f;
                        break;
                    }
                }

                // ── Pass 2: exhaustive scan of all string properties ───────────
                if (match.ValueKind == JsonValueKind.Undefined)
                {
                    CntLog($"  Pass 1 found no match (first feature admin: \"{dbgFirst}\").\n" +
                           $"  Trying exhaustive property scan…\n");

                    foreach (var f in root.GetProperty("features").EnumerateArray())
                    {
                        if (!f.TryGetProperty("properties", out var props)) continue;
                        foreach (var prop in props.EnumerateObject())
                        {
                            if (prop.Value.ValueKind != JsonValueKind.String) continue;
                            string val = prop.Value.GetString() ?? "";
                            if (val.Equals(_selectedCountry, StringComparison.OrdinalIgnoreCase) ||
                                (!string.IsNullOrEmpty(_selectedCode) &&
                                 val.Equals(_selectedCode, StringComparison.OrdinalIgnoreCase)))
                            {
                                match = f;
                                break;
                            }
                        }
                        if (match.ValueKind != JsonValueKind.Undefined) break;
                    }
                }

                if (match.ValueKind == JsonValueKind.Undefined)
                {
                    CntLog($"\n  No match found for \"{_selectedCountry}\" ({_selectedCode}).\n" +
                           $"  First feature's admin value was: \"{dbgFirst}\"",
                           color: System.Drawing.Color.Tomato);
                    CntShowProgress(false);
                    return;
                }

                if (!match.TryGetProperty("geometry", out var geom) ||
                    !geom.TryGetProperty("coordinates", out var coords))
                {
                    CntLog("\n  No geometry data available for this country.",
                           color: System.Drawing.Color.Tomato);
                    CntShowProgress(false);
                    return;
                }

                string geoType = geom.TryGetProperty("type", out var typeEl)
                    ? typeEl.GetString() ?? "" : "";

                CntProgress(70, "Converting coordinates to Aurora format…");

                _countryOutput = geoType == "Polygon"
                    ? CntPolygonToArtcc(coords,      _selectedCode)
                    : CntMultiPolygonToArtcc(coords, _selectedCode);

                sw.Stop();
                int lines = _countryOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                          .Count(l => l.StartsWith("T;"));

                CntProgress(100, $"Done - {lines} coordinate points");
                CntLog($"\n✔  Conversion complete in {sw.Elapsed.TotalSeconds:F2}s  " +
                       $"({lines} points)",
                       bold: true,
                       color: System.Drawing.Color.FromArgb(46, 140, 86));

                cntDownloadButton.Enabled = true;

                await Task.Delay(1200);
                CntShowProgress(false);
            }
            catch (Exception ex)
            {
                CntLog($"\nError: {ex.Message}", color: System.Drawing.Color.Tomato);
                _countryGeoJson = null;   // invalidate cache on failure
                CntShowProgress(false);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GeoJSON → Aurora .artcc conversion
        //
        //  Polygon coords:      [ ring, ring, … ]
        //  MultiPolygon coords: [ polygon, polygon, … ]  where polygon = [ ring, … ]
        // ─────────────────────────────────────────────────────────────────────

        private string CntPolygonToArtcc(JsonElement coordsArray, string code)
        {
            var sb  = new StringBuilder();
            sb.Append(CntFileHeader(code));

            var pts = new List<(double lat, double lon)>();
            foreach (var ring in coordsArray.EnumerateArray())
                foreach (var pt in ring.EnumerateArray())
                    pts.Add((pt[1].GetDouble(), pt[0].GetDouble()));

            CntAppendArtcc(sb, pts, code);
            return sb.ToString();
        }

        private string CntMultiPolygonToArtcc(JsonElement coordsArray, string code)
        {
            var sb = new StringBuilder();
            sb.Append(CntFileHeader(code));

            foreach (var polygon in coordsArray.EnumerateArray())
            {
                var pts = new List<(double lat, double lon)>();
                foreach (var ring in polygon.EnumerateArray())
                    foreach (var pt in ring.EnumerateArray())
                        pts.Add((pt[1].GetDouble(), pt[0].GetDouble()));
                CntAppendArtcc(sb, pts, code);
            }

            return sb.ToString();
        }

        private static void CntAppendArtcc(StringBuilder sb,
            List<(double lat, double lon)> pts, string code)
        {
            if (pts.Count == 0) return;
            foreach (var (lat, lon) in pts)
                sb.AppendLine($"T;{code};{CntToN(lat)};{CntToE(lon)};");
            sb.AppendLine($"\nT;Dummy;N000.00.00.000;E000.00.00.000");
        }

        // ─────────────────────────────────────────────────────────────────────
        //  File header
        // ─────────────────────────────────────────────────────────────────────

        private string CntFileHeader(string code)
        {
            string cycle = SplashForm.AiracDaysLeft >= 0
                ? $"  ·  AIRAC {SplashForm.AiracCycle}" : "";
            return
                $"// IVAO Aurora Sector File - Country Boundary (.artcc)\n" +
                $"// Country: {_selectedCountry}  ({code})\n" +
                $"// Format: T;IDENTIFIER;LAT;LON;\n" +
                $"//   IDENTIFIER : ISO-2 country code\n" +
                $"//   LAT        : N/S degrees.minutes.seconds.milliseconds\n" +
                $"//   LON        : E/W degrees.minutes.seconds.milliseconds\n" +
                $"// Generated: {DateTime.UtcNow:yyyy-MM-dd}{cycle}\n" +
                $"// Tool: IVAO Sector File Creator\n" +
                $"// ⚠  NOT FOR REAL WORLD USE\n\n";
        }

        // ─────────────────────────────────────────────────────────────────────
        //  DMS coordinate formatters
        // ─────────────────────────────────────────────────────────────────────

        // ─────────────────────────────────────────────────────────────────────
        //  GeoJSON property helper - tries each name in order, returns first hit
        // ─────────────────────────────────────────────────────────────────────

        private static string CntGetProp(JsonElement props, params string[] names)
        {
            foreach (string n in names)
                if (props.TryGetProperty(n, out var el) && el.ValueKind == JsonValueKind.String)
                {
                    string v = el.GetString() ?? "";
                    if (!string.IsNullOrEmpty(v)) return v;
                }
            return "";
        }

        private static string CntToN(double lat)
        {
            string h = lat >= 0 ? "N" : "S";
            lat = Math.Abs(lat);
            int d = (int)lat;
            double md = (lat - d) * 60; int m = (int)md;
            double sd = (md - m) * 60;  int s = (int)sd;
            int ms = (int)((sd - s) * 1000) % 1000;
            return $"{h}{d:00}.{m:00}.{s:00}.{ms:000}";
        }

        private static string CntToE(double lon)
        {
            string h = lon >= 0 ? "E" : "W";
            lon = Math.Abs(lon);
            int d = (int)lon;
            double md = (lon - d) * 60; int m = (int)md;
            double sd = (md - m) * 60;  int s = (int)sd;
            int ms = (int)((sd - s) * 1000) % 1000;
            return $"{h}{d:000}.{m:00}.{s:00}.{ms:000}";
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Download button
        // ─────────────────────────────────────────────────────────────────────

        private void CntDownload_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_countryOutput))
            {
                MessageBox.Show("No data to save.", "No Data",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string safeName = _selectedCountry.Replace(" ", "_");
            using var dlg = new SaveFileDialog
            {
                Filter   = "ARTCC Files (*.artcc)|*.artcc|All Files (*.*)|*.*",
                Title    = $"Save Country Boundary - {_selectedCountry}",
                FileName = $"{safeName}_boundary.artcc",
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(dlg.FileName, _countryOutput);
                    MessageBox.Show($"Saved to {dlg.FileName}", "Saved",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    CntLog($"\nError saving: {ex.Message}",
                           color: System.Drawing.Color.Tomato);
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Progress / status helpers
        // ─────────────────────────────────────────────────────────────────────

        private void CntShowProgress(bool visible)
        {
            if (cntProgressPanel.InvokeRequired)
                cntProgressPanel.Invoke((MethodInvoker)(() => CntShowProgress(visible)));
            else
            {
                cntProgressPanel.Visible = visible;
                if (!visible) { cntProgressBar.Value = 0; cntStatusLabel.Text = ""; }
            }
        }

        private void CntProgress(int pct, string status)
        {
            if (cntProgressBar.InvokeRequired)
                cntProgressBar.Invoke((MethodInvoker)(() => CntProgress(pct, status)));
            else
            {
                cntProgressPanel.Visible  = true;
                cntProgressBar.Value      = Math.Clamp(pct, 0, 100);
                cntStatusLabel.Text       = status;
            }
        }

        private void CntStatus(string text)
        {
            if (cntStatusLabel.InvokeRequired)
                cntStatusLabel.Invoke((MethodInvoker)(() => CntStatus(text)));
            else
            {
                cntStatusLabel.Text = text;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Rich-text log helpers
        // ─────────────────────────────────────────────────────────────────────

        private void CntLog(string text,
                             bool bold  = false,
                             System.Drawing.Color? color = null)
        {
            if (cntLogBox.InvokeRequired)
                cntLogBox.Invoke((MethodInvoker)(() => CntLogDirect(text, bold, color)));
            else
                CntLogDirect(text, bold, color);
        }

        private void CntLogDirect(string text, bool bold, System.Drawing.Color? color)
        {
            int start = cntLogBox.TextLength;
            cntLogBox.AppendText(text);
            cntLogBox.Select(start, text.Length);
            cntLogBox.SelectionFont  = new System.Drawing.Font(cntLogBox.Font,
                bold ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular);
            cntLogBox.SelectionColor = color ?? System.Drawing.Color.FromArgb(30, 40, 65);
            cntLogBox.Select(cntLogBox.TextLength, 0);
            cntLogBox.ScrollToCaret();
        }
    }
}
