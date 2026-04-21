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
    // ─────────────────────────────────────────────────────────────────────────
    //  SID & STAR tool logic  - embedded panel in MainForm
    //
    //  Design principle:
    //   • Collect the union of both list endpoints (SID + STAR).
    //   • Fetch each detail exactly once and classify by type.code from the
    //     detail endpoint - this is authoritative even when the list endpoint
    //     misclassifies (e.g. DEEZZ6 in STAR list but detail says SID).
    //   • Cache detail responses so generation never double-fetches.
    //   • A clear inventory of all found + reclassified names is printed.
    // ─────────────────────────────────────────────────────────────────────────
    partial class MainForm
    {
        // ── Search / generate handler ─────────────────────────────────────────

        private async void SsSearch_Click(object sender, EventArgs e)
        {
            string icao = ssIcaoBox.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(icao))
            {
                MessageBox.Show("Please enter an ICAO airport code.", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Reset
            ssProgressBar.Value = 0;
            _sidOutput  = SsHeader_Sid(icao);
            _starOutput = SsHeader_Star(icao);
            downloadSidButton.Enabled  = false;
            downloadStarButton.Enabled = false;
            ssLogBox.Clear();
            _ssLogLinks.Clear();

            SsLog("\n");
            SsLog("Made by ");
            SsLog("Veda Moola (656077)", isLink: true, url: "https://www.ivao.aero/Member.aspx?Id=656077");
            SsLog("  |  Tested by ");
            SsLog("Nilay Parsodkar (709833)", isLink: true, url: "https://www.ivao.aero/Member.aspx?Id=709833");
            SsLog("\n⚠  NOT FOR REAL WORLD USE\n", bold: true,
                  color: System.Drawing.Color.FromArgb(200, 60, 60));
            SsLog($"\nAIRAC {SplashForm.AiracCycle}");
            SsLog($"\nAirport: {icao}\n");

            // Format reference block
            var _dim  = System.Drawing.Color.FromArgb(110, 130, 160);
            var _blue = System.Drawing.Color.FromArgb(13, 71, 161);
            SsLog("\n━━  Output File Formats  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━",
                  bold: true, color: _blue);
            SsLog("\n  .sid  SID header : ", color: _dim);
            SsLog("ICAO;RUNWAY;PROC_ID;MATCH_FIX;MATCH_FIX;[//LAT;LON]");
            SsLog("\n  .str  STAR header: ", color: _dim);
            SsLog("ICAO;RUNWAY;PROC_ID;MATCH_FIX;MATCH_FIX;[//LAT;LON]");
            SsLog("\n  Waypoint line    : ", color: _dim);
            SsLog("FIX;FIX;[RESTRICTION];[//LAT;LON]");
            SsLog("\n  Restriction fmt  : ", color: _dim);
            SsLog("=FL150  -FL200  +5000  250kt  =FL150 | 250kt");
            SsLog("\n  Ref: ", color: _dim);
            SsLog("wiki.ivao.aero/en/home/devops/manuals/SectorFile_Definition", isLink: true);
            SsLog("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n",
                  color: _blue);

            // Read options from checkboxes
            bool sidAlt   = chkSidAlt.Checked;
            bool sidSpd   = chkSidSpd.Checked;
            bool sidCoord = chkSidCoord.Checked;
            bool starAlt  = chkStarAlt.Checked;
            bool starSpd  = chkStarSpd.Checked;
            bool starCoord = chkStarCoord.Checked;

            ssSearchButton.Enabled = false;
            var sw = Stopwatch.StartNew();

            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(90) };

                // ── Step 1: Collect all SID and STAR identifiers upfront ──────
                SsSetStatus("Collecting SID list…");
                var sidIds  = await SsCollectIdentifiers(client, icao, "SID");
                SsUpdateProgress(15);

                SsSetStatus("Collecting STAR list…");
                var starIds = await SsCollectIdentifiers(client, icao, "STAR");
                SsUpdateProgress(30);

                // ── Step 2: Print raw API inventory ──────────────────────
                SsLog("\n━━  RAW API - SID endpoint  ━━━━━━━━━━━━━━━━━━━━━━━━━━",
                      bold: true, color: System.Drawing.Color.FromArgb(13, 71, 161));
                SsLog(sidIds.Count == 0
                    ? "\n  None."
                    : $"\n  {string.Join("   ", sidIds.OrderBy(x => x))}",
                      color: System.Drawing.Color.FromArgb(20, 100, 60));

                SsLog("\n\n━━  RAW API - STAR endpoint  ━━━━━━━━━━━━━━━━━━━━━━━━━",
                      bold: true, color: System.Drawing.Color.FromArgb(130, 60, 0));
                SsLog(starIds.Count == 0
                    ? "\n  None."
                    : $"\n  {string.Join("   ", starIds.OrderBy(x => x))}",
                      color: System.Drawing.Color.FromArgb(130, 60, 0));
                SsLog("\n");

                // ── Step 3: Classify using list source + type.code heuristics ─
                //
                //  Rules (pure list-membership - no type.code guessing):
                //   • ID in SID list ONLY  → SID file only
                //   • ID in STAR list ONLY → STAR file only
                //   • ID in BOTH lists     → included in BOTH files
                //     (a comment is written in each file block noting the overlap)
                SsLog("\n── Classifying by list membership ──────────────────────");
                SsUpdateProgress(30);
                var (sidOnlyIds, starOnlyIds, bothIds, detailCache) =
                    await SsClassifyProcedures(client, icao, sidIds, starIds);
                SsUpdateProgress(55);

                // Build per-file sets
                var sidFileIds  = new HashSet<string>(sidOnlyIds,  StringComparer.OrdinalIgnoreCase);
                foreach (string bid in bothIds) sidFileIds.Add(bid);

                var starFileIds = new HashSet<string>(starOnlyIds, StringComparer.OrdinalIgnoreCase);
                foreach (string bid in bothIds) starFileIds.Add(bid);

                SsLog("\n\n━━  SID FILE  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━",
                      bold: true, color: System.Drawing.Color.FromArgb(13, 71, 161));
                SsLog(sidOnlyIds.Count == 0 ? "" :
                      $"\n  SID-only:  {string.Join("   ", sidOnlyIds.OrderBy(x => x))}",
                      color: System.Drawing.Color.FromArgb(20, 100, 60));
                if (bothIds.Count > 0)
                    SsLog($"\n  Both lists: {string.Join("   ", bothIds.OrderBy(x => x))}",
                          color: System.Drawing.Color.FromArgb(100, 0, 160));

                SsLog("\n\n━━  STAR FILE  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━",
                      bold: true, color: System.Drawing.Color.FromArgb(130, 60, 0));
                SsLog(starOnlyIds.Count == 0 ? "" :
                      $"\n  STAR-only:  {string.Join("   ", starOnlyIds.OrderBy(x => x))}",
                      color: System.Drawing.Color.FromArgb(130, 60, 0));
                if (bothIds.Count > 0)
                    SsLog($"\n  Both lists: {string.Join("   ", bothIds.OrderBy(x => x))}",
                          color: System.Drawing.Color.FromArgb(100, 0, 160));
                SsLog("\n");

                SsLog("\n── Generating SID file ─────────────────────────────────");
                await SsFetchAndFormat(client, icao, "SID",  sidFileIds,
                                       sidAlt,  sidSpd,  sidCoord, detailCache, bothIds);
                SsUpdateProgress(78);

                SsLog("\n── Generating STAR file ────────────────────────────────");
                await SsFetchAndFormat(client, icao, "STAR", starFileIds,
                                       starAlt, starSpd, starCoord, detailCache, bothIds);
                SsUpdateProgress(100);

                // Enable only buttons that have actual data
                downloadSidButton.Enabled  = !string.IsNullOrEmpty(_sidOutput.Trim());
                downloadStarButton.Enabled = !string.IsNullOrEmpty(_starOutput.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching data:\n{ex.Message}", "Network Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ssSearchButton.Enabled = true;
                SsSetStatus("Ready");
            }

            sw.Stop();
            string elapsed = sw.Elapsed.TotalSeconds > 1
                ? $"{sw.Elapsed.TotalSeconds:F2}s"
                : $"{sw.Elapsed.TotalMilliseconds:F0}ms";
            SsLog($"\n\n✔  Done in {elapsed}",
                  bold: true, color: System.Drawing.Color.FromArgb(46, 140, 86));
        }

        // ── Classify all procedures by authoritative type.code ───────────────
        //
        //  Fetches the detail endpoint for every ID in allIds exactly once.
        //  Categorises each ID purely by which API list endpoint(s) returned it.
        //  sidOnly  → SID file only
        //  starOnly → STAR file only
        //  both     → included in BOTH files (a comment is added in each block)
        //  cache    → detail responses keyed by identifier (reused during generation)

        private async Task<(HashSet<string> sidOnlyIds, HashSet<string> starOnlyIds,
                            HashSet<string> bothIds, Dictionary<string, JObject> cache)>
            SsClassifyProcedures(HttpClient client, string icao,
                                  HashSet<string> sidListIds, HashSet<string> starListIds)
        {
            var sidOnly = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var starOnly = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var both    = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var cache   = new Dictionary<string, JObject>(StringComparer.OrdinalIgnoreCase);

            var allIds = new HashSet<string>(sidListIds, StringComparer.OrdinalIgnoreCase);
            foreach (string id in starListIds) allIds.Add(id);

            int done = 0;
            foreach (string id in allIds.OrderBy(x => x))
            {
                done++;
                SsSetStatus($"Classifying {done}/{allIds.Count}  -  {id}");

                string url = $"{ApiBaseUrl}/procedures/{icao}/{id}";
                HttpResponseMessage res = await client.GetAsync(url);

                if (!res.IsSuccessStatusCode)
                {
                    SsLog($"\n  ⚠  {id} - HTTP {(int)res.StatusCode}, skipped");
                    continue;
                }

                JObject detail   = JObject.Parse(await res.Content.ReadAsStringAsync());
                JObject procData = detail["data"] as JObject;
                if (procData == null)
                {
                    SsLog($"\n  ⚠  {id} - no data in response, skipped");
                    continue;
                }

                cache[id] = procData;

                bool inSid  = sidListIds.Contains(id);
                bool inStar = starListIds.Contains(id);

                if      (inSid && !inStar)  sidOnly.Add(id);
                else if (!inSid && inStar)  starOnly.Add(id);
                else                        both.Add(id);
            }

            return (sidOnly, starOnly, both, cache);
        }

        // ── Collect unique identifiers from the paginated list endpoint ───────
        //
        //  Returns the set of identifiers exactly as the API categorises them.
        //  Classification happens later in SsClassifyProcedures.

        private async Task<HashSet<string>> SsCollectIdentifiers(
            HttpClient client, string icao, string type)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int page = 1;

            while (true)
            {
                string url = $"{ApiBaseUrl}/procedures?airport={icao}&type={type}" +
                             $"&page={page}&per_page=50";
                SsSetStatus($"Fetching {type} list page {page}…");

                HttpResponseMessage res = await client.GetAsync(url);
                if (!res.IsSuccessStatusCode) break;

                JObject body = JObject.Parse(await res.Content.ReadAsStringAsync());
                JArray  data = body["data"] as JArray;
                if (data == null || !data.Any()) break;

                foreach (JObject proc in data)
                {
                    string id = proc["identifier"]?.ToString();
                    if (!string.IsNullOrEmpty(id)) seen.Add(id);
                }

                // Multiple pagination key shapes
                bool hasMore = body["pagination"]?["has_more"]?.Value<bool>()
                            ?? body["meta"]?["pagination"]?["has_more"]?.Value<bool>()
                            ?? false;

                // If API returned a full page but didn't set has_more, try next page
                if (!hasMore && data.Count == 50)
                    hasMore = true;

                if (!hasMore) break;
                if (++page > 30) break;   // safety cap
            }

            return seen;
        }

        // ── Fetch detail and format each procedure ────────────────────────────
        //
        //  Uses the pre-built detail cache from SsClassifyProcedures so every
        //  procedure is fetched only once.  Falls back to a live GET if a cache
        //  miss somehow occurs (shouldn't happen in normal flow).

        private async Task SsFetchAndFormat(
            HttpClient client, string icao, string type,
            HashSet<string> identifiers,
            bool inclAlt, bool inclSpd, bool inclCoord,
            Dictionary<string, JObject> cache = null,
            HashSet<string> bothIds = null)
        {
            if (!identifiers.Any())
            {
                SsLog($"\n  No {type} procedures to generate.");
                return;
            }

            int done = 0;

            foreach (string id in identifiers.OrderBy(x => x))
            {
                done++;
                SsSetStatus($"Generating {type}  {done}/{identifiers.Count}  -  {id}");

                JObject procData = null;

                if (cache != null && cache.TryGetValue(id, out JObject cached))
                    procData = cached;
                else
                {
                    string detailUrl = $"{ApiBaseUrl}/procedures/{icao}/{id}";
                    HttpResponseMessage dr = await client.GetAsync(detailUrl);
                    if (!dr.IsSuccessStatusCode)
                    {
                        SsLog($"\n  ⚠  {id} - HTTP {(int)dr.StatusCode}, skipped");
                        continue;
                    }
                    JObject detail = JObject.Parse(await dr.Content.ReadAsStringAsync());
                    procData = detail["data"] as JObject;
                }

                if (procData == null)
                {
                    SsLog($"\n  ⚠  {id} - no data, skipped");
                    continue;
                }

                bool isAlsoBoth = bothIds != null && bothIds.Contains(id);

                if (type == "SID")
                    SsFormatSid(procData,  icao, id, inclAlt, inclSpd, inclCoord, isAlsoBoth);
                else
                    SsFormatStar(procData, icao, id, inclAlt, inclSpd, inclCoord, isAlsoBoth);
            }

            SsLog($"\n\n  ✔  {type}:  generated {identifiers.Count} procedure(s)", bold: true);
        }

        // ── Format SID ────────────────────────────────────────────────────────

        private void SsFormatSid(JObject data, string icao, string id,
                                  bool inclAlt, bool inclSpd, bool inclCoord,
                                  bool alsoInStar = false)
        {
            try
            {
                JObject rwy = data["runway_transitions"] as JObject;
                JObject trn = data["transitions"] as JObject;
                string pfx  = SsProcPrefix(id);

                if (rwy != null && rwy.HasValues)
                {
                    foreach (var rwEntry in rwy)
                    {
                        string runway = rwEntry.Key;
                        JArray wpts   = rwEntry.Value as JArray;
                        if (wpts == null || !wpts.Any()) continue;

                        string mFix     = SsFindMatchFix(wpts, trn, pfx, id);
                        string hdrCoord = SsFirstCoord(wpts, inclCoord);

                        _sidOutput += "// SID Start\n";
                        if (alsoInStar)
                            _sidOutput += "// Note: also present in STAR file (found in both SID and STAR endpoints)\n";
                        _sidOutput += $"{icao};{runway};{id};{mFix};{mFix};{hdrCoord}\n";

                        foreach (JObject wp in wpts)
                            _sidOutput += SsWaypointLine(wp, inclAlt, inclSpd, inclCoord) ?? "";

                        _sidOutput += "\n";

                        if (trn != null && trn.HasValues)
                        {
                            _sidOutput += "// TRANSITION START\n";
                            foreach (var t in trn)
                            {
                                string tn   = t.Key;
                                JArray twps = t.Value as JArray;
                                _sidOutput += $"{icao};{runway};{tn};{tn};{tn};1;\n";
                                if (twps != null)
                                    foreach (JObject wp in twps)
                                        _sidOutput += SsWaypointLine(wp, inclAlt, inclSpd, inclCoord) ?? "";
                                _sidOutput += $"// TRANSITION END {tn}\n";
                            }
                            _sidOutput += "// TRANSITION END\n";
                        }

                        _sidOutput += "// SID ended\n\n";
                    }
                }
                else if (trn != null && trn.HasValues)
                {
                    // Fallback: no runway-specific legs; use transitions
                    foreach (var t in trn)
                    {
                        string transName = t.Key;
                        JArray twps      = t.Value as JArray;
                        if (twps == null || !twps.Any()) continue;

                        string mFix     = SsFindMatchFix(twps, null, pfx, id);
                        string hdrCoord = SsFirstCoord(twps, inclCoord);

                        _sidOutput += "// SID Start\n";
                        if (alsoInStar)
                            _sidOutput += "// Note: also present in STAR file (found in both SID and STAR endpoints)\n";
                        _sidOutput += $"{icao};{transName};{id};{mFix};{mFix};{hdrCoord}\n";

                        foreach (JObject wp in twps)
                            _sidOutput += SsWaypointLine(wp, inclAlt, inclSpd, inclCoord) ?? "";

                        _sidOutput += "\n// SID ended\n\n";
                    }
                }
            }
            catch (Exception ex)
            {
                ssLogBox.AppendText($"\n  Error parsing SID {id}: {ex.Message}");
            }
        }

        // ── Format STAR ───────────────────────────────────────────────────────

        private void SsFormatStar(JObject data, string icao, string id,
                                   bool inclAlt, bool inclSpd, bool inclCoord,
                                   bool alsoInSid = false)
        {
            try
            {
                JObject rwy = data["runway_transitions"] as JObject;
                JObject trn = data["transitions"] as JObject;
                string pfx  = SsProcPrefix(id);

                if (rwy != null && rwy.HasValues)
                {
                    // Normal case: STAR has runway-specific arrival legs
                    foreach (var rwEntry in rwy)
                    {
                        string runway = rwEntry.Key;
                        JArray wpts   = rwEntry.Value as JArray;
                        if (wpts == null || !wpts.Any()) continue;

                        string mFix     = SsFindMatchFix(wpts, trn, pfx, id);
                        string hdrCoord = SsFirstCoord(wpts, inclCoord);

                        _starOutput += "// STAR Start\n";
                        if (alsoInSid)
                            _starOutput += "// Note: also present in SID file (found in both SID and STAR endpoints)\n";
                        _starOutput += $"{icao};{runway};{id};{mFix};{mFix};{hdrCoord}\n";

                        foreach (JObject wp in wpts)
                            _starOutput += SsWaypointLine(wp, inclAlt, inclSpd, inclCoord) ?? "";

                        _starOutput += "\n";

                        if (trn != null && trn.HasValues)
                        {
                            _starOutput += "// TRANSITION START\n";
                            foreach (var t in trn)
                            {
                                string tn   = t.Key;
                                JArray twps = t.Value as JArray;
                                _starOutput += $"{icao};{runway};{tn};{tn};{tn};1;\n";
                                if (twps != null)
                                    foreach (JObject wp in twps)
                                        _starOutput += SsWaypointLine(wp, inclAlt, inclSpd, inclCoord) ?? "";
                                _starOutput += $"// TRANSITION END {tn}\n";
                            }
                            _starOutput += "// TRANSITION END\n";
                        }

                        _starOutput += "// STAR ended\n\n";
                    }
                }
                else if (trn != null && trn.HasValues)
                {
                    // STAR ends at a fix (vectors-to-final) - no runway-specific legs.
                    // Each named transition is a separate arrival path to the terminal fix.
                    // Use the transition name as the "runway" field in the sector file header.
                    foreach (var t in trn)
                    {
                        string transName = t.Key;
                        JArray twps      = t.Value as JArray;
                        if (twps == null || !twps.Any()) continue;

                        string mFix     = SsFindMatchFix(twps, null, pfx, id);
                        string hdrCoord = SsFirstCoord(twps, inclCoord);

                        _starOutput += "// STAR Start\n";
                        if (alsoInSid)
                            _starOutput += "// Note: also present in SID file (found in both SID and STAR endpoints)\n";
                        _starOutput += $"{icao};{transName};{id};{mFix};{mFix};{hdrCoord}\n";

                        foreach (JObject wp in twps)
                            _starOutput += SsWaypointLine(wp, inclAlt, inclSpd, inclCoord) ?? "";

                        _starOutput += "\n// STAR ended\n\n";
                    }
                }
            }
            catch (Exception ex)
            {
                ssLogBox.AppendText($"\n  Error parsing STAR {id}: {ex.Message}");
            }
        }

        // ── Waypoint helpers ──────────────────────────────────────────────────

        private static string SsProcPrefix(string name) =>
            name.Length >= 6 ? name[..4] :
            name.Length >= 3 ? name[..3] : name;

        private static string SsFindMatchFix(JArray wpts, JObject trn, string prefix, string fallback)
        {
            foreach (JObject wp in wpts)
            {
                string n = wp["fix_identifier"]?.ToString();
                if (n != null && n.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    return n;
            }
            if (trn != null)
                foreach (var t in trn)
                    if (t.Value is JArray twps)
                        foreach (JObject wp in twps)
                        {
                            string n = wp["fix_identifier"]?.ToString();
                            if (n != null && n.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                                return n;
                        }
            return fallback;
        }

        private static string SsFirstCoord(JArray wpts, bool include)
        {
            if (!include) return "";
            double lat = wpts.First["fix_coordinates"]?["lat"]?.Value<double>() ?? 0;
            double lon = wpts.First["fix_coordinates"]?["lon"]?.Value<double>() ?? 0;
            return $" //{SsToN(lat)};{SsToE(lon)}";
        }

        private static string SsWaypointLine(JObject wp, bool inclAlt, bool inclSpd, bool inclCoord)
        {
            string name = wp["fix_identifier"]?.ToString();
            if (string.IsNullOrEmpty(name) || name.StartsWith("(")) return null;

            double alt   = wp["altitude_ft"]?.Value<double>() ?? 0;
            string rstr  = wp["altitude_restriction"]?.ToString();
            double speed = wp["speed_kts"]?.Value<double>() ?? 0;
            double lat   = wp["fix_coordinates"]?["lat"]?.Value<double>() ?? 0;
            double lon   = wp["fix_coordinates"]?["lon"]?.Value<double>() ?? 0;

            string altFmt   = inclAlt ? SsFormatAlt(alt, rstr) : "";
            string spdFmt   = inclSpd && speed > 0 ? $"{(int)speed}kt" : "";
            string coordCmt = inclCoord ? $" //{SsToN(lat)};{SsToE(lon)}" : "";

            bool ha = !string.IsNullOrEmpty(altFmt);
            bool hs = !string.IsNullOrEmpty(spdFmt);

            if (!ha && !hs) return $"{name};{name};{coordCmt}\n";
            if ( ha &&  hs) return $"{name};{name};{altFmt} | {spdFmt};{coordCmt}\n";
            if ( ha)        return $"{name};{name};{altFmt};{coordCmt}\n";
                            return $"{name};{name};{spdFmt};{coordCmt}\n";
        }

        private static string SsFormatAlt(double alt, string restriction)
        {
            if (alt == 0 || string.IsNullOrEmpty(restriction)) return "";
            string a = alt > 4000 ? $"FL{(int)(alt / 100)}" : ((int)alt).ToString();
            return restriction.ToLowerInvariant() switch
            {
                "at"          => $"={a}",
                "at_or_below" => $"-{a}",
                "at_or_above" => $"+{a}",
                "between"     => $"={a}",
                _             => ""
            };
        }

        private static string SsToN(double lat)
        {
            string h = lat >= 0 ? "N" : "S";
            lat = Math.Abs(lat);
            int d = (int)lat; double md = (lat - d) * 60; int m = (int)md;
            double sd = (md - m) * 60; int s = (int)sd; int ms = (int)((sd - s) * 1000) % 1000;
            return $"{h}{d:00}.{m:00}.{s:00}.{ms:000}";
        }

        private static string SsToE(double lon)
        {
            string h = lon >= 0 ? "E" : "W";
            lon = Math.Abs(lon);
            int d = (int)lon; double md = (lon - d) * 60; int m = (int)md;
            double sd = (md - m) * 60; int s = (int)sd; int ms = (int)((sd - s) * 1000) % 1000;
            return $"{h}{d:000}.{m:00}.{s:00}.{ms:000}";
        }

        // ── Format header comments ────────────────────────────────────────────

        private static string SsHeader_Sid(string icao) =>
            $"// IVAO Aurora Sector File - SID Procedures (.sid)\n" +
            $"// Airport: {icao}\n" +
            $"// Format: ICAO;RUNWAY;PROC_ID;MATCH_FIX;MATCH_FIX;[//LAT;LON]\n" +
            $"//         FIX;FIX;[RESTRICTION];[//LAT;LON]\n" +
            $"//   RUNWAY     : runway designator (e.g. 09L) or transition name\n" +
            $"//   PROC_ID    : SID identifier (e.g. AMPA2A)\n" +
            $"//   MATCH_FIX  : first fix matching the procedure prefix\n" +
            $"//   RESTRICTION: altitude/speed  e.g. =FL150  -FL200  +5000  250kt\n" +
            $"//   LAT/LON    : optional coord comment  N/S DD.MM.SS.mmm  E/W DDD.MM.SS.mmm\n" +
            $"// Made by Veda Moola (656077)\n" +
            $"// Ref: https://wiki.ivao.aero/en/home/devops/manuals/SectorFile_Definition\n" +
            $"// AIRAC {SplashForm.AiracCycle}\n" +
            $"// ⚠  NOT FOR REAL WORLD USE\n\n";

        private static string SsHeader_Star(string icao) =>
            $"// IVAO Aurora Sector File - STAR Procedures (.str)\n" +
            $"// Airport: {icao}\n" +
            $"// Format: ICAO;RUNWAY;PROC_ID;MATCH_FIX;MATCH_FIX;[//LAT;LON]\n" +
            $"//         FIX;FIX;[RESTRICTION];[//LAT;LON]\n" +
            $"//   RUNWAY     : runway designator (e.g. 27R) or transition name\n" +
            $"//   PROC_ID    : STAR identifier (e.g. NOPMA1A)\n" +
            $"//   MATCH_FIX  : first fix matching the procedure prefix\n" +
            $"//   RESTRICTION: altitude/speed  e.g. =FL150  -FL200  +5000  250kt\n" +
            $"//   LAT/LON    : optional coord comment  N/S DD.MM.SS.mmm  E/W DDD.MM.SS.mmm\n" +
            $"// Made by Veda Moola (656077)\n" +
            $"// Ref: https://wiki.ivao.aero/en/home/devops/manuals/SectorFile_Definition\n" +
            $"// AIRAC {SplashForm.AiracCycle}\n" +
            $"// ⚠  NOT FOR REAL WORLD USE\n\n";

        // ── Save output file ──────────────────────────────────────────────────

        private void SsSaveFile(string data, string filter, string ext)
        {
            if (string.IsNullOrEmpty(data))
            {
                MessageBox.Show("No data to save. Generate first.", "No Data",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string icao = ssIcaoBox.Text.Trim().ToUpper();
            using var dlg = new SaveFileDialog
            {
                Filter   = filter,
                Title    = $"Save {ext} - {icao}",
                FileName = $"{icao}.{ext}"
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
                    ssLogBox.AppendText($"\nError saving: {ex.Message}");
                }
            }
        }

        // ── UI helpers ────────────────────────────────────────────────────────

        private void SsUpdateProgress(int value)
        {
            if (ssProgressBar.InvokeRequired)
                ssProgressBar.Invoke((MethodInvoker)(() => ssProgressBar.Value = value));
            else
                ssProgressBar.Value = value;
        }

        private void SsSetStatus(string text)
        {
            if (ssStatusLabel.InvokeRequired)
                ssStatusLabel.Invoke((MethodInvoker)(() => ssStatusLabel.Text = text));
            else
                ssStatusLabel.Text = text;
        }

        // Maps (charStart, charLength) → URL for clickable log links
        private readonly List<(int Start, int Length, string Url)> _ssLogLinks = new();

        private void SsLog(string text,
                           bool bold    = false,
                           System.Drawing.Color? color = null,
                           bool isLink  = false,
                           string url   = null)
        {
            if (ssLogBox.InvokeRequired)
                ssLogBox.Invoke((MethodInvoker)(() => SsLogDirect(text, bold, color, isLink, url)));
            else
                SsLogDirect(text, bold, color, isLink, url);
        }

        private void SsLogDirect(string text, bool bold,
                                  System.Drawing.Color? color, bool isLink, string url)
        {
            int start = ssLogBox.TextLength;
            ssLogBox.AppendText(text);
            ssLogBox.Select(start, text.Length);

            if (isLink)
            {
                ssLogBox.SelectionColor = System.Drawing.Color.FromArgb(25, 118, 210);
                ssLogBox.SelectionFont  = new System.Drawing.Font(ssLogBox.Font,
                    System.Drawing.FontStyle.Underline);
                if (!string.IsNullOrEmpty(url))
                    _ssLogLinks.Add((start, text.Length, url));
            }
            else
            {
                ssLogBox.SelectionFont  = new System.Drawing.Font(ssLogBox.Font,
                    bold ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular);
                ssLogBox.SelectionColor = color ?? System.Drawing.Color.FromArgb(30, 40, 65);
            }

            ssLogBox.Select(ssLogBox.TextLength, 0);
            ssLogBox.ScrollToCaret();
        }

        private void SsLogBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            int idx = ssLogBox.GetCharIndexFromPosition(e.Location);
            foreach (var (start, length, url) in _ssLogLinks)
            {
                if (idx >= start && idx < start + length)
                {
                    System.Diagnostics.Process.Start(
                        new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
                    return;
                }
            }
        }
    }
}
