using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using Newtonsoft.Json.Linq;

namespace Sector_File
{
    partial class MainForm
    {
        private const string FlAeroHost = "aerodatabox.p.rapidapi.com";
        private const string FlAeroBase = "https://aerodatabox.p.rapidapi.com/flights/airports/icao";

        private List<(string dir, JObject obj)> _lastFlResults = new();

        // ── Search ────────────────────────────────────────────────────────────
        private async void FlSearch_Click(object sender, EventArgs e)
        {
            string dep     = flDepBox.Text.Trim().ToUpper();
            string arr     = flArrBox.Text.Trim().ToUpper();
            string airline = flAirlineBox.Text.Trim().ToUpper();
            int    mode    = flStatusCombo.SelectedIndex; // 0=both 1=dep only 2=arr only
            DateTime date  = flDatePicker.Value.Date;

            if (string.IsNullOrEmpty(dep) && string.IsNullOrEmpty(arr))
            {
                MessageBox.Show(
                    "Enter at least a departure or arrival airport ICAO code (e.g. VTBS).",
                    L("error_input_required_title"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dep.Length > 0 && dep.Length != 4)
            {
                MessageBox.Show("Departure airport must be a 4-letter ICAO code.",
                    L("error_input_required_title"), MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }
            if (arr.Length > 0 && arr.Length != 4)
            {
                MessageBox.Show("Arrival airport must be a 4-letter ICAO code.",
                    L("error_input_required_title"), MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }

            // mode 0 = Departures, mode 1 = Arrivals
            // Accept whichever ICAO box is filled regardless of which the user typed in
            if (mode == 0 && string.IsNullOrEmpty(dep) && !string.IsNullOrEmpty(arr))
                dep = arr;
            if (mode == 1 && string.IsNullOrEmpty(arr) && !string.IsNullOrEmpty(dep))
                arr = dep;

            // ── Request limit check ───────────────────────────────────────────
            if (!ConfigManager.TryIncrementFlightRequest())
            {
                MessageBox.Show(
                    "You have reached the daily limit of 10 flight schedule requests.\n" +
                    "The counter resets at midnight UTC.",
                    "Daily Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FlRefreshReqLabel();
                return;
            }
            FlRefreshReqLabel();

            string apiKey = ConfigManager.AeroDataBoxApiKey;
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                FlLog("✗  API key not configured.\n", bold: true, color: Color.FromArgb(200, 60, 60));
                return;
            }

            flLogBox.Clear();
            _lastFlResults.Clear();
            flDownloadExcelBtn.Enabled = false;
            FlProgress(0);
            flSearchButton.Enabled = false;
            FlStatus("Fetching…");

            // Build DateTime range from pickers
            DateTime dtFrom = date + flFromTimePicker.Value.TimeOfDay;
            DateTime dtTo   = date + flToTimePicker.Value.TimeOfDay;
            if (dtTo <= dtFrom) dtTo = dtFrom.AddHours(12);

            bool isFuture = date.Date > DateTime.UtcNow.Date;
            bool isToday  = date.Date == DateTime.UtcNow.Date;
            string period = isFuture ? "Scheduled (future)" : isToday ? "Live / Today" : "Historical";

            FlLog("Made by ", color: Color.FromArgb(110, 130, 160));
            FlLog("Veda Moola (656077)", isLink: true);
            FlLog("  ·  Tested by ", color: Color.FromArgb(110, 130, 160));
            FlLog("Nilay Parsodkar (709833)", isLink: true);
            FlLog("\n⚠  NOT FOR REAL WORLD NAVIGATION USE\n", bold: true, color: Color.FromArgb(200, 60, 60));
            FlLog("Fetched from a free API source\n\n", color: Color.FromArgb(110, 130, 160));

            FlLog($"Date: {date:dd MMM yyyy}  [{period}]\n", color: Color.FromArgb(60, 80, 120));
            FlLog($"Time window: {dtFrom:HH:mm}z – {dtTo:HH:mm}z\n", color: Color.FromArgb(60, 80, 120));
            if (!string.IsNullOrEmpty(dep))     FlLog($"Dep: {dep}  ", color: Color.FromArgb(60, 80, 120));
            if (!string.IsNullOrEmpty(arr))     FlLog($"Arr: {arr}  ", color: Color.FromArgb(60, 80, 120));
            if (!string.IsNullOrEmpty(airline)) FlLog($"Airline: {airline}", color: Color.FromArgb(60, 80, 120));
            FlLog("\n\n");

            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
                client.DefaultRequestHeaders.Add("X-RapidAPI-Host", FlAeroHost);
                client.DefaultRequestHeaders.Add("X-RapidAPI-Key",  apiKey);

                var allFlights = new List<(string dir, JObject obj)>();

                // Departures
                if (!string.IsNullOrEmpty(dep) && mode == 0)
                {
                    FlStatus($"Fetching departures from {dep}…");
                    var (flights, err) = await FlFetchAsync(client, dep, dtFrom, dtTo, "Departure");
                    if (err != null) { FlLog($"✗  {err}\n", bold: true, color: Color.FromArgb(200, 60, 60)); }
                    foreach (var f in flights)
                    {
                        f["_queriedDepIcao"]  = dep;
                        f["_queriedDepName"]  = dep;   // name not returned by API for queried airport
                        allFlights.Add(("DEP", f));
                    }
                    FlProgress(40);
                }

                // Arrivals
                if (!string.IsNullOrEmpty(arr) && mode == 1)
                {
                    FlStatus($"Fetching arrivals at {arr}…");
                    var (flights, err) = await FlFetchAsync(client, arr, dtFrom, dtTo, "Arrival");
                    if (err != null) { FlLog($"✗  {err}\n", bold: true, color: Color.FromArgb(200, 60, 60)); }
                    foreach (var f in flights)
                    {
                        f["_queriedArrIcao"]  = arr;
                        f["_queriedArrName"]  = arr;
                        allFlights.Add(("ARR", f));
                    }
                    FlProgress(70);
                }

                FlProgress(80);
                FlStatus("Rendering…");

                // Airline filter
                if (!string.IsNullOrEmpty(airline))
                {
                    allFlights.RemoveAll(x =>
                    {
                        string icao = x.obj["airline"]?["icao"]?.ToString()?.Trim().ToUpper() ?? "";
                        string iata = x.obj["airline"]?["iata"]?.ToString()?.Trim().ToUpper() ?? "";
                        string cs   = x.obj["callSign"]?.ToString()?.Trim().ToUpper()         ?? "";
                        return !icao.Equals(airline) && !iata.Equals(airline)
                            && !cs.StartsWith(airline, StringComparison.OrdinalIgnoreCase);
                    });
                }

                if (allFlights.Count == 0)
                {
                    FlLog("No flights found for the specified criteria.\n",
                          color: Color.FromArgb(150, 160, 180));
                    FlLog("\nTips:\n", bold: true, color: Color.FromArgb(80, 100, 130));
                    FlLog("  • Use 4-letter ICAO codes (e.g. EGLL, EDDF, VTBS, WMKK)\n" +
                          "  • Airline filter: ICAO 3-letter designator (e.g. BAW, DLH, SIA)\n" +
                          "  • Future dates return scheduled airline flights\n" +
                          "  • Past dates return historical ADS-B flight data\n",
                          color: Color.FromArgb(110, 130, 160));
                    FlProgress(100);
                    return;
                }

                var colBlue  = Color.FromArgb(13,  71, 161);
                var colMuted = Color.FromArgb(110, 130, 160);
                var colGreen = Color.FromArgb(22,  163,  74);

                FlLog($"Found {allFlights.Count} flight(s)\n\n", bold: true, color: colGreen);

                foreach (var (dir, f) in allFlights)
                {
                    string flNum    = f["number"]?.ToString()?.Trim()     ?? "";
                    string callSign = f["callSign"]?.ToString()?.Trim()   ?? "";
                    string airName  = f["airline"]?["name"]?.ToString()   ?? "";
                    string airIcao  = f["airline"]?["icao"]?.ToString()   ?? "";
                    string status   = f["status"]?.ToString()             ?? "Unknown";
                    string acModel  = f["aircraft"]?["model"]?.ToString() ?? "";

                    var depNode = f["departure"] ?? f["movement"];
                    var arrNode = f["arrival"];

                    string depIcao = depNode?["airport"]?["icao"]?.ToString()
                                  ?? f["_queriedDepIcao"]?.ToString() ?? "?";
                    string arrIcao = arrNode?["airport"]?["icao"]?.ToString()
                                  ?? f["_queriedArrIcao"]?.ToString() ?? "?";
                    // Name is absent for the queried airport - fall back to ICAO
                    string depName = depNode?["airport"]?["name"]?.ToString()
                                  ?? f["_queriedDepName"]?.ToString() ?? depIcao;
                    string arrName = arrNode?["airport"]?["name"]?.ToString()
                                  ?? f["_queriedArrName"]?.ToString() ?? arrIcao;
                    string schedDep  = FlParseAeroTime(depNode?["scheduledTime"]?["utc"]?.ToString());
                    string schedArr  = FlParseAeroTime(arrNode?["scheduledTime"]?["utc"]?.ToString());
                    string actualDep = FlParseAeroTime(depNode?["actualTime"]?["utc"]?.ToString());
                    string actualArr = FlParseAeroTime(arrNode?["actualTime"]?["utc"]?.ToString());
                    string depTerminal = depNode?["terminal"]?.ToString() ?? "";
                    string depGate     = depNode?["gate"]?.ToString()     ?? "";
                    string arrTerminal = arrNode?["terminal"]?.ToString() ?? "";
                    string arrGate     = arrNode?["gate"]?.ToString()     ?? "";
                    string acReg       = f["aircraft"]?["reg"]?.ToString() ?? "";

                    string badge    = dir == "DEP" ? "↑ DEP" : "↓ ARR";
                    Color  badgeCol = dir == "DEP"
                        ? Color.FromArgb(5, 150, 105)
                        : Color.FromArgb(37, 99, 235);

                    FlLog($"━━  {flNum}", bold: true, color: colBlue);
                    if (!string.IsNullOrEmpty(callSign) && callSign != flNum.Replace(" ", ""))
                        FlLog($"  ({callSign})", color: colMuted);
                    FlLog($"  [{badge}]\n", bold: true, color: badgeCol);

                    if (!string.IsNullOrEmpty(airName))
                    {
                        FlLog("  Airline  : ", color: colMuted);
                        FlLog(airName);
                        if (!string.IsNullOrEmpty(airIcao)) FlLog($"  [{airIcao}]", color: colMuted);
                        FlLog("\n");
                    }

                    FlLog("  Route    : ", color: colMuted);
                    string depLabel = !string.IsNullOrEmpty(depName) ? $"{depIcao} ({depName})" : depIcao;
                    string arrLabel = !string.IsNullOrEmpty(arrName) ? $"{arrIcao} ({arrName})" : arrIcao;
                    FlLog($"{depLabel}  →  {arrLabel}\n");

                    if (!string.IsNullOrEmpty(acModel))
                    {
                        FlLog("  Aircraft : ", color: colMuted);
                        FlLog(acModel);
                        if (!string.IsNullOrEmpty(acReg)) FlLog($"  [{acReg}]", color: colMuted);
                        FlLog("\n");
                    }

                    // Terminal & gate
                    bool hasDepTG = !string.IsNullOrEmpty(depTerminal) || !string.IsNullOrEmpty(depGate);
                    bool hasArrTG = !string.IsNullOrEmpty(arrTerminal) || !string.IsNullOrEmpty(arrGate);
                    if (hasDepTG || hasArrTG)
                    {
                        FlLog("  Terminal : ", color: colMuted);
                        if (hasDepTG)
                        {
                            FlLog("Dep ");
                            if (!string.IsNullOrEmpty(depTerminal)) FlLog($"T{depTerminal}");
                            if (!string.IsNullOrEmpty(depGate))     FlLog($" Gate {depGate}");
                        }
                        if (hasDepTG && hasArrTG) FlLog("  ");
                        if (hasArrTG)
                        {
                            FlLog("Arr ");
                            if (!string.IsNullOrEmpty(arrTerminal)) FlLog($"T{arrTerminal}");
                            if (!string.IsNullOrEmpty(arrGate))     FlLog($" Gate {arrGate}");
                        }
                        FlLog("\n");
                    }

                    if (!string.IsNullOrEmpty(schedDep) || !string.IsNullOrEmpty(schedArr))
                    {
                        FlLog("  Sched    : ", color: colMuted);
                        if (!string.IsNullOrEmpty(schedDep)) { FlLog("Dep "); FlLog(schedDep); }
                        if (!string.IsNullOrEmpty(schedDep) && !string.IsNullOrEmpty(schedArr)) FlLog("  ");
                        if (!string.IsNullOrEmpty(schedArr)) { FlLog("Arr "); FlLog(schedArr); }
                        FlLog("\n");
                    }

                    if (!string.IsNullOrEmpty(actualDep) || !string.IsNullOrEmpty(actualArr))
                    {
                        FlLog("  Actual   : ", color: colMuted);
                        if (!string.IsNullOrEmpty(actualDep))
                        { FlLog("Dep "); FlLog(actualDep, color: colGreen); FlLog("  "); }
                        if (!string.IsNullOrEmpty(actualArr))
                        { FlLog("Arr "); FlLog(actualArr, color: colGreen); }
                        FlLog("\n");
                    }

                    FlLog("\n");
                }

                _lastFlResults = allFlights;
                flDownloadExcelBtn.Enabled = true;
                FlProgress(100);
            }
            catch (HttpRequestException hrex)
            {
                FlLog($"\n✗  Network error: {hrex.Message}\n",
                      bold: true, color: Color.FromArgb(200, 60, 60));
            }
            catch (Exception ex)
            {
                FlLog($"\n✗  Error: {ex.Message}\n", bold: true, color: Color.FromArgb(200, 60, 60));
            }
            finally
            {
                flSearchButton.Enabled = true;
                FlStatus("Done");
            }
        }

        // ── Fetch with auto-split for the 12-hour API window limit ────────────
        // Returns (flights, errorMessage). Splits transparently if window > 12h.
        private static async Task<(List<JObject> flights, string? error)> FlFetchAsync(
            HttpClient client, string icao, DateTime from, DateTime to, string direction)
        {
            var all = new List<JObject>();

            // Split the window into 12-hour chunks.
            // Delay 1.1 s between calls - free tier enforces a per-second rate limit.
            DateTime cursor = from;
            bool firstChunk = true;
            while (cursor < to)
            {
                if (!firstChunk)
                    await Task.Delay(1100);
                firstChunk = false;

                DateTime chunkEnd = cursor.AddHours(12);
                if (chunkEnd > to) chunkEnd = to;

                var (chunk, err) = await FlFetchChunkAsync(client, icao, cursor, chunkEnd, direction);
                if (err != null) return (all, err);
                all.AddRange(chunk);

                cursor = chunkEnd;
            }

            return (all, null);
        }

        private static async Task<(List<JObject> flights, string? error)> FlFetchChunkAsync(
            HttpClient client, string icao, DateTime from, DateTime to, string direction)
        {
            try
            {
                string fromStr = from.ToString("yyyy-MM-dd'T'HH:mm");
                string toStr   = to.ToString("yyyy-MM-dd'T'HH:mm");

                string url = $"{FlAeroBase}/{icao}/{fromStr}/{toStr}" +
                             $"?withLeg=true&direction={direction}" +
                             $"&withCancelled=true&withCodeshared=true" +
                             $"&withCargo=false&withPrivate=false";

                var response = await client.GetAsync(url);
                string body  = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    // Try to extract API error message
                    string apiMsg = body;
                    try { apiMsg = JObject.Parse(body)["message"]?.ToString() ?? body; } catch { }
                    return (new List<JObject>(), $"API error {(int)response.StatusCode}: {apiMsg}");
                }

                var root = JObject.Parse(body);
                var result = new List<JObject>();
                string key = direction == "Departure" ? "departures" : "arrivals";
                if (root[key] is JArray arr)
                    foreach (var item in arr)
                        if (item is JObject obj) result.Add(obj);

                return (result, null);
            }
            catch (Exception ex)
            {
                return (new List<JObject>(), ex.Message);
            }
        }

        // ── Excel download ────────────────────────────────────────────────────
        private void FlDownloadExcel_Click(object sender, EventArgs e)
        {
            if (_lastFlResults.Count == 0) return;

            using var dlg = new SaveFileDialog
            {
                Title      = "Save Flight Schedules",
                Filter     = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName   = $"FlightSchedules_{flDatePicker.Value:yyyyMMdd}.xlsx",
                DefaultExt = "xlsx",
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Flight Schedules");

                string[] headers =
                {
                    "Direction", "Flight No", "Call Sign",
                    "Airline", "Airline ICAO", "Status",
                    "Dep ICAO", "Dep Airport Name",
                    "Arr ICAO", "Arr Airport Name",
                    "Aircraft Model", "Registration",
                    "Dep Terminal", "Dep Gate",
                    "Arr Terminal", "Arr Gate",
                    "Sched Dep (UTC)", "Sched Arr (UTC)",
                    "Actual Dep (UTC)", "Actual Arr (UTC)"
                };
                for (int i = 0; i < headers.Length; i++)
                {
                    ws.Cell(1, i + 1).Value = headers[i];
                    ws.Cell(1, i + 1).Style.Font.Bold = true;
                    ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromArgb(13, 27, 75);
                    ws.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
                }

                int row = 2;
                foreach (var (dir, f) in _lastFlResults)
                {
                    var depNode = f["departure"] ?? f["movement"];
                    var arrNode = f["arrival"];

                    string exDepIcao = depNode?["airport"]?["icao"]?.ToString()
                                    ?? f["_queriedDepIcao"]?.ToString() ?? "";
                    string exArrIcao = arrNode?["airport"]?["icao"]?.ToString()
                                    ?? f["_queriedArrIcao"]?.ToString() ?? "";
                    // Name is absent for the queried airport - leave blank rather than repeating ICAO
                    string rawDepName = depNode?["airport"]?["name"]?.ToString() ?? f["_queriedDepName"]?.ToString() ?? "";
                    string rawArrName = arrNode?["airport"]?["name"]?.ToString() ?? f["_queriedArrName"]?.ToString() ?? "";
                    string exDepName  = rawDepName == exDepIcao ? "" : rawDepName;
                    string exArrName  = rawArrName == exArrIcao ? "" : rawArrName;

                    ws.Cell(row, 1).Value  = dir;
                    ws.Cell(row, 2).Value  = f["number"]?.ToString()?.Trim()     ?? "";
                    ws.Cell(row, 3).Value  = f["callSign"]?.ToString()?.Trim()   ?? "";
                    ws.Cell(row, 4).Value  = f["airline"]?["name"]?.ToString()   ?? "";
                    ws.Cell(row, 5).Value  = f["airline"]?["icao"]?.ToString()   ?? "";
                    ws.Cell(row, 6).Value  = f["status"]?.ToString()             ?? "";
                    ws.Cell(row, 7).Value  = exDepIcao;
                    ws.Cell(row, 8).Value  = exDepName;   // blank when API doesn't return queried airport name
                    ws.Cell(row, 9).Value  = exArrIcao;
                    ws.Cell(row, 10).Value = exArrName;
                    ws.Cell(row, 11).Value = f["aircraft"]?["model"]?.ToString() ?? "";
                    ws.Cell(row, 12).Value = f["aircraft"]?["reg"]?.ToString()   ?? "";
                    ws.Cell(row, 13).Value = depNode?["terminal"]?.ToString()    ?? "";
                    ws.Cell(row, 14).Value = depNode?["gate"]?.ToString()        ?? "";
                    ws.Cell(row, 15).Value = arrNode?["terminal"]?.ToString()    ?? "";
                    ws.Cell(row, 16).Value = arrNode?["gate"]?.ToString()        ?? "";
                    ws.Cell(row, 17).Value = FlParseAeroTime(depNode?["scheduledTime"]?["utc"]?.ToString());
                    ws.Cell(row, 18).Value = FlParseAeroTime(arrNode?["scheduledTime"]?["utc"]?.ToString());
                    ws.Cell(row, 19).Value = FlParseAeroTime(depNode?["actualTime"]?["utc"]?.ToString());
                    ws.Cell(row, 20).Value = FlParseAeroTime(arrNode?["actualTime"]?["utc"]?.ToString());

                    if (row % 2 == 0)
                        ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(248, 250, 252);
                    row++;
                }

                ws.Columns().AdjustToContents();
                wb.SaveAs(dlg.FileName);

                MessageBox.Show($"Saved to:\n{dlg.FileName}",
                    L("msg_export_title"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}",
                    L("error_save_title"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string FlParseAeroTime(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "";
            raw = raw.Trim().Replace(" ", "T").TrimEnd('Z');
            if (DateTime.TryParse(raw, out DateTime dt))
                return dt.ToString("HH:mm") + "z";
            return "";
        }

        // ── Request counter label refresh ─────────────────────────────────────
        private void FlRefreshReqLabel()
        {
            var lbl = flProgressPanel?.Controls["flReqCountLabel"] as Label;
            if (lbl == null) return;
            int count = ConfigManager.GetTodayFlightRequests();
            lbl.Text = $"{count} / 10 requests today";
            lbl.ForeColor = count >= 10
                ? Color.FromArgb(185, 28, 28)
                : Color.FromArgb(120, 135, 160);
        }

        // ── Log / progress / status ───────────────────────────────────────────
        private void FlLog(string text, bool bold = false,
            Color? color = null, bool isLink = false)
        {
            if (flLogBox.InvokeRequired)
                flLogBox.Invoke((MethodInvoker)(() => FlLogDirect(text, bold, color, isLink)));
            else
                FlLogDirect(text, bold, color, isLink);
        }

        private void FlLogDirect(string text, bool bold, Color? color, bool isLink)
        {
            int start = flLogBox.TextLength;
            flLogBox.AppendText(text);
            flLogBox.Select(start, text.Length);
            if (isLink)
            {
                flLogBox.SelectionColor = Color.CornflowerBlue;
                flLogBox.SelectionFont  = new Font(flLogBox.Font, FontStyle.Underline);
            }
            else
            {
                flLogBox.SelectionFont  = bold
                    ? new Font(flLogBox.Font, FontStyle.Bold)
                    : new Font(flLogBox.Font, FontStyle.Regular);
                flLogBox.SelectionColor = color ?? Color.FromArgb(30, 40, 65);
            }
            flLogBox.Select(flLogBox.TextLength, 0);
            flLogBox.ScrollToCaret();
        }

        private void FlProgress(int value)
        {
            if (flProgressBar.InvokeRequired)
                flProgressBar.Invoke((MethodInvoker)(() => flProgressBar.Value = value));
            else
                flProgressBar.Value = value;
        }

        private void FlStatus(string text)
        {
            if (flStatusLabel.InvokeRequired)
                flStatusLabel.Invoke((MethodInvoker)(() => flStatusLabel.Text = text));
            else
                flStatusLabel.Text = text;
        }
    }
}
