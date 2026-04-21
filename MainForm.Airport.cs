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
    // ─────────────────────────────────────────────────────────────────────────
    //  Airport Data tool logic  - embedded panel in MainForm
    //
    //  Input:
    //   • 4-letter ICAO  (e.g. VABB)  - single airport + navaids for its region
    //   • 2-letter prefix (e.g. VA)   - all airports in that FIR + all navaids
    //
    //  Airport listing strategy (2-letter mode):
    //   1. Try /airports?region={prefix}          (may work on some API builds)
    //   2. Try /airports?country={ISO}            (lookup table for known regions)
    //   3. Paginate all airports, stop smartly    (reliable fallback)
    // ─────────────────────────────────────────────────────────────────────────
    partial class MainForm
    {
        // ── State ─────────────────────────────────────────────────────────────
        private const string ApApiBase = "https://airac.net/api/v1";

        private string _apAirportOutput  = "";
        private string _apRunwayOutput   = "";
        private string _apFreqOutput     = "";
        private string _apVorOutput      = "";
        private string _apNdbOutput      = "";
        private string _apLastCountryIso = "";  // cached from ApDetectCountryAsync for OpenAIP fallback

        // (country code is detected dynamically from the API - no hardcoded table)

        // ── Search / orchestrate ──────────────────────────────────────────────
        private async void ApSearch_Click(object sender, EventArgs e)
        {
            string input = apRegionBox.Text.Trim().ToUpper();

            bool isFullIcao = input.Length == 4;
            bool isFirMode  = input.Length == 2;

            if (!isFullIcao && !isFirMode)
            {
                MessageBox.Show(
                    "Enter a 4-letter airport ICAO (e.g. VABB) or 2-letter FIR prefix (e.g. VA).",
                    "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string region = isFullIcao ? input.Substring(0, 2) : input;

            // Reset
            _apAirportOutput = _apRunwayOutput = _apFreqOutput =
                _apVorOutput = _apNdbOutput = "";
            apDownloadAirportBtn.Enabled = apDownloadRunwayBtn.Enabled =
            apDownloadFreqBtn.Enabled   = apDownloadVorBtn.Enabled = apDownloadNdbBtn.Enabled = false;
            apLogBox.Clear();
            ApProgress(0);
            apSearchButton.Enabled = false;
            ApStatus("Initialising…");

            // Attribution
            ApLog("Made by ");
            ApLog("Veda Moola (656077)", isLink: true);
            ApLog("  |  Tested by ");
            ApLog("Nilay Parsodkar (709833)", isLink: true);
            ApLog("\n⚠  NOT FOR REAL WORLD USE\n",
                  bold: true, color: Color.FromArgb(200, 60, 60));
            ApLog($"\nAIRAC {SplashForm.AiracCycle}");

            if (isFullIcao)
                ApLog($"\nAirport     : {input}  (region {region})\n");
            else
                ApLog($"\nRegion      : {region}XX FIR\n");

            // Format reference block
            var dim  = Color.FromArgb(110, 130, 160);
            var blue = Color.FromArgb(13, 71, 161);
            ApLog("\n━━  Output File Formats  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━",
                  bold: true, color: blue);
            ApLog("\n  .ap   Airport    : ", color: dim);
            ApLog("ICAO;ELEV_FT;LAT;LON;NAME;");
            ApLog("\n  .rw   Runway     : ", color: dim);
            ApLog("ICAO;BASE;RECIP;BASE_BRG;RECIP_BRG;BASE_LAT;BASE_LON;RECIP_LAT;RECIP_LON;");
            ApLog("\n  .atc  ATC Freq   : ", color: dim);
            ApLog("CALLSIGN;FREQ_MHZ;VIS_POINTS;ALIAS;VOICE_ATIS;FLAGS;;ATIS_FILE;");
            ApLog("\n           CALLSIGN suffixes: ", color: dim);
            ApLog("TWR  GND  APP  DEL  DEP  CTR  FSS  UNICOM");
            ApLog("\n  .vor  VHF Navaid : ", color: dim);
            ApLog("IDENT;FREQ_MHZ;LAT;LON;1;TYPE;");
            ApLog("  (TYPE: 1=VOR  2=VOR/DME  4=DME)", color: dim);
            ApLog("\n  .ndb  NDB Navaid : ", color: dim);
            ApLog("IDENT;FREQ_KHZ;LAT;LON;");
            ApLog("\n  Ref: ", color: dim);
            ApLog("wiki.ivao.aero/en/home/devops/manuals/SectorFile_Definition", isLink: true);
            ApLog("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n",
                  color: blue);

            var sw = Stopwatch.StartNew();

            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(120) };

                if (isFullIcao)
                {
                    // ── Single airport mode ───────────────────────────────────
                    ApStatus($"Fetching airport {input}…");
                    ApLog($"\nFetching airport {input}…");
                    await ApFetchSingleAirportAsync(client, input);
                    ApProgress(40);
                }
                else
                {
                    // ── FIR mode: all airports + runways + frequencies ─────────
                    ApStatus($"Fetching airports in {region}XX FIR…");
                    ApLog($"\nFetching airports in {region}XX FIR…");
                    var airports = await ApFetchAirportsInRegionAsync(client, region);
                    ApProgress(15);
                    ApLog($"\n  Found {airports.Count} airport(s) - fetching details…");

                    var apLines  = new List<string>();
                    var rwLines  = new List<string>();
                    var frqLines = new List<string>();
                    var seenFreq = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    int done = 0;
                    foreach (var ap in airports)
                    {
                        done++;
                        ApStatus($"Fetching details {done}/{airports.Count}  -  {ap.Icao}");
                        apLines.Add(
                            $"{ap.Icao};{ap.ElevFt};{ApToN(ap.Lat)};{ApToE(ap.Lon)};{ap.Name};");

                        var (rwCount, frqCount) = await ApFetchDetailAsync(client, ap, rwLines, frqLines, seenFreq);

                        string frqNote = frqCount == 0
                            ? "  ⚠ no frequencies"
                            : $"  freq:{frqCount}";
                        ApLog($"\n  [{done}/{airports.Count}] {ap.Icao}  rwy:{rwCount}{frqNote}");

                        // Progress 15 → 40 % over all airport detail fetches
                        ApProgress(15 + (int)(25.0 * done / Math.Max(1, airports.Count)));
                    }

                    // ── FIR CTR / FSS positions ───────────────────────────────
                    //   Check if any CTR/FSS was found among airport frequencies.
                    //   If not, try fetching the FIR center airport directly.
                    bool hasCtr = frqLines.Any(l =>
                        l.Contains("_CTR;") || l.Contains("_FSS;"));
                    if (!hasCtr)
                    {
                        ApStatus("Looking for FIR CTR/FSS position…");
                        ApLog("\n  No CTR/FSS found in airport data - probing FIR center…");
                        await ApFetchFirCtrAsync(client, region, frqLines, seenFreq);
                    }

                    _apAirportOutput = ApHeader_Airport() + string.Join(Environment.NewLine, apLines);
                    _apRunwayOutput  = ApHeader_Runway()  + string.Join(Environment.NewLine, rwLines);
                    _apFreqOutput    = ApHeader_Atc()     + string.Join(Environment.NewLine, frqLines);
                    ApProgress(40);
                }

                // ── Navaids (same for both modes) ─────────────────────────────
                ApStatus("Fetching VOR navaids…");
                ApLog($"\nFetching VOR navaids (region {region})…");
                var vorLines    = await ApFetchNavaidsAsync(client, region, "VOR",    1);
                ApProgress(55);

                ApStatus("Fetching VOR/DME navaids…");
                ApLog($"\nFetching VOR/DME navaids (region {region})…");
                var vordmeLines = await ApFetchNavaidsAsync(client, region, "VORDME", 2);
                ApProgress(70);

                ApStatus("Fetching DME navaids…");
                ApLog($"\nFetching DME navaids (region {region})…");
                var dmeLines    = await ApFetchNavaidsAsync(client, region, "DME",    4);
                ApProgress(83);

                ApStatus("Fetching NDB navaids…");
                ApLog($"\nFetching NDB navaids (region {region})…");
                var ndbLines    = await ApFetchNavaidsAsync(client, region, "NDB",   -1);
                ApProgress(98);

                // Merge all VHF navaids into one output file
                var allVhf = new List<string>();
                allVhf.AddRange(vorLines);
                allVhf.AddRange(vordmeLines);
                allVhf.AddRange(dmeLines);
                _apVorOutput = ApHeader_Vor() + string.Join(Environment.NewLine, allVhf);
                _apNdbOutput = ApHeader_Ndb() + string.Join(Environment.NewLine, ndbLines);

                ApProgress(100);

                // Enable only buttons that have actual data
                apDownloadAirportBtn.Enabled = !string.IsNullOrEmpty(_apAirportOutput);
                apDownloadRunwayBtn.Enabled  = !string.IsNullOrEmpty(_apRunwayOutput);
                apDownloadFreqBtn.Enabled    = !string.IsNullOrEmpty(_apFreqOutput);
                apDownloadVorBtn.Enabled     = !string.IsNullOrEmpty(_apVorOutput);
                apDownloadNdbBtn.Enabled     = !string.IsNullOrEmpty(_apNdbOutput);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching data:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                apSearchButton.Enabled = true;
                ApStatus("Done");
            }

            sw.Stop();
            string elapsed = sw.Elapsed.TotalSeconds > 1
                ? $"{sw.Elapsed.TotalSeconds:F2}s"
                : $"{sw.Elapsed.TotalMilliseconds:F0}ms";
            ApLog($"\n\n✔  Done in {elapsed}", bold: true, color: Color.FromArgb(22, 163, 74));
        }

        // ────────────────────────────────────────────────────────────────────
        //  List all airports whose ICAO starts with the 2-letter prefix.
        //
        //  Strategy (tries in order until airports are found):
        //   1. /airports?region={prefix}          API-native filter
        //   2. /airports?country={ISO}            country-code lookup + prefix filter
        //   3. Full paginated scan                unlimited pages, smart stop
        // ────────────────────────────────────────────────────────────────────
        private record ApInfo(string Icao, string Name, double Lat, double Lon, int ElevFt);

        private async Task<List<ApInfo>> ApFetchAirportsInRegionAsync(
            HttpClient client, string region)
        {
            var results = new List<ApInfo>();

            // ── Strategy 1: API-native ?region= filter ────────────────────────
            await ApPagedAirports(client, $"?region={region}", region, results);
            if (results.Count > 0)
            {
                ApLog($"\n  (used region filter - {results.Count} airports)");
                return results;
            }

            // ── Strategy 2: probe likely ICAO codes to detect country ─────────
            ApLog("\n  (detecting country from API - probing ICAO candidates…)");
            string iso = await ApDetectCountryAsync(client, region);
            if (!string.IsNullOrEmpty(iso))
            {
                _apLastCountryIso = iso;   // cache for OpenAIP fallback
                ApLog($"\n  (country {iso} detected - fetching all {region}* airports)");
                await ApPagedAirports(client, $"?country={iso}", region, results);
                if (results.Count > 0)
                {
                    ApLog($"\n  ({results.Count} airports matched {region}* in country {iso})");
                    return results;
                }
            }

            // ── Strategy 3: full scan with smart stop (reliable fallback) ─────
            ApLog("\n  (full scan - paginating all airports)");
            bool seenAny        = false;
            int  emptyAfterMatch = 0;

            for (int p = 1; ; p++)
            {
                var res = await client.GetAsync(
                    $"{ApApiBase}/airports?per_page=100&page={p}");
                if (!res.IsSuccessStatusCode) break;

                JObject obj  = JObject.Parse(await res.Content.ReadAsStringAsync());
                JArray  data = obj["data"] as JArray;
                if (data == null || data.Count == 0) break;

                int before   = results.Count;
                ApExtract(data, region, results);
                bool foundAny = results.Count > before;

                if (foundAny)     { seenAny = true; emptyAfterMatch = 0; }
                else if (seenAny) emptyAfterMatch++;

                bool hasMore = obj["pagination"]?["has_more"]?.Value<bool>() ?? false;
                if (!hasMore || (seenAny && emptyAfterMatch >= 8)) break;
            }

            return results;
        }

        // ── Auto-detect ISO country code by probing likely ICAO codes ──────────
        //
        //  Instead of scanning the global list (which may not return all airports
        //  or may paginate them in non-alphabetical order), we directly request
        //  a handful of plausible ICAO codes for this prefix and read the
        //  iso_country / country_code / country field from the detail response.
        //  This is fast (max ~12 requests) and works for any valid ICAO prefix.
        private async Task<string> ApDetectCountryAsync(HttpClient client, string prefix)
        {
            // Common suffix patterns found in airport ICAO codes across all regions.
            // For prefix "VA": tries VABB (Mumbai), VAAB, VABA, VAAA, etc.
            string[] suffixes = { "BB", "AB", "BA", "AA", "CB", "NB", "IB", "DB",
                                   "HB", "OB", "PB", "RB", "SB", "TB" };

            foreach (string suffix in suffixes)
            {
                string candidate = prefix + suffix;
                var res = await client.GetAsync($"{ApApiBase}/airports/{candidate}");
                if (!res.IsSuccessStatusCode) continue;

                JObject root    = JObject.Parse(await res.Content.ReadAsStringAsync());
                JObject airport = root["data"] as JObject ?? root;

                // Confirm the returned airport actually belongs to this prefix
                string apIcao = airport["icao"]?.ToString()
                             ?? airport["identifier"]?.ToString() ?? "";
                if (!apIcao.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                // Try every common field name for ISO-3166-1 alpha-2 country code
                string country = airport["iso_country"]?.ToString()
                              ?? airport["country_code"]?.ToString()
                              ?? airport["country"]?.ToString()
                              ?? airport["countryCode"]?.ToString()
                              ?? "";

                if (country.Length >= 2)
                {
                    string iso = country.Substring(0, 2).ToUpper();
                    ApLog($"\n  → country {iso} detected from {apIcao}");
                    return iso;
                }

                // Airport exists but API returned no country field -
                // log it and keep trying other candidates
                ApLog($"\n  → {apIcao} found but no country field in API response");
            }

            return null;   // detection failed; caller will fall back to full scan
        }

        /// <summary>
        /// Paginates one endpoint pattern, extracts airports matching the prefix.
        /// Returns true if the HTTP calls succeeded (even if 0 match).
        /// </summary>
        private async Task<bool> ApPagedAirports(
            HttpClient client, string queryString,
            string prefix, List<ApInfo> results)
        {
            int  page    = 1;
            bool hasMore = true;

            while (hasMore)
            {
                var res = await client.GetAsync(
                    $"{ApApiBase}/airports{queryString}&per_page=100&page={page}");
                if (!res.IsSuccessStatusCode) return false;

                JObject obj  = JObject.Parse(await res.Content.ReadAsStringAsync());
                JArray  data = obj["data"] as JArray;
                if (data == null || data.Count == 0) break;

                ApExtract(data, prefix, results);

                hasMore = obj["pagination"]?["has_more"]?.Value<bool>() ?? false;
                page++;
            }
            return true;
        }

        private static void ApExtract(JArray data, string prefix, List<ApInfo> results)
        {
            foreach (JObject ap in data)
            {
                string icao = ap["icao"]?.ToString()
                           ?? ap["identifier"]?.ToString() ?? "";
                if (!icao.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) continue;
                results.Add(new ApInfo(
                    icao,
                    ap["name"]?.ToString()           ?? "",
                    ap["latitude"]?.Value<double>()  ?? 0,
                    ap["longitude"]?.Value<double>() ?? 0,
                    ap["elevation_ft"]?.Value<int>() ?? 0));
            }
        }

        // ────────────────────────────────────────────────────────────────────
        //  Fetch per-airport runways + frequencies (embedded in detail endpoint)
        //  Returns (runwayCount, frequencyCount) for logging.
        // ────────────────────────────────────────────────────────────────────
        private async Task<(int rwCount, int frqCount)> ApFetchDetailAsync(
            HttpClient client, ApInfo ap,
            List<string> rwOut, List<string> frqOut, HashSet<string> seenFreq)
        {
            var res = await client.GetAsync($"{ApApiBase}/airports/{ap.Icao}");
            if (!res.IsSuccessStatusCode) return (0, 0);

            JObject root    = JObject.Parse(await res.Content.ReadAsStringAsync());
            JObject airport = root["data"] as JObject ?? root;

            // ── Runways ───────────────────────────────────────────────────────
            var runwayArray = airport["runways"] as JArray ?? new JArray();
            foreach (JObject rw in runwayArray)
            {
                string baseId    = rw["base_identifier"]?.ToString()         ?? "";
                string recipId   = rw["reciprocal_identifier"]?.ToString()   ?? "";
                double baseBrng  = rw["base_bearing"]?.Value<double>()       ?? 0;
                double recipBrng = rw["reciprocal_bearing"]?.Value<double>() ?? 0;
                double lengthFt  = rw["length_ft"]?.Value<double>()          ?? 0;

                double halfNm = (lengthFt / 2.0) / 6076.12;
                var (bLat, bLon) = ApGeoOffset(ap.Lat, ap.Lon, recipBrng, halfNm);
                var (rLat, rLon) = ApGeoOffset(ap.Lat, ap.Lon, baseBrng,  halfNm);

                rwOut.Add($"{ap.Icao};{baseId};{recipId};" +
                          $"{baseBrng:F0};{recipBrng:F0};" +
                          $"{ApToN(bLat)};{ApToE(bLon)};" +
                          $"{ApToN(rLat)};{ApToE(rLon)};");
            }

            string rwyType  = runwayArray.Count >= 2 ? "dual" : "single";
            var freqArray   = airport["frequencies"] as JArray ?? new JArray();
            int frqBefore   = frqOut.Count;

            // ── Pass 1: map known ATC position types ──────────────────────────
            foreach (JObject freq in freqArray)
            {
                string ftype = (freq["type"]?.ToString() ?? "").ToUpper().Trim();
                double fmhz  = freq["frequency_mhz"]?.Value<double>() ?? 0;

                if (ftype.EndsWith("ATI") && !ftype.EndsWith("ATIS")) ftype += "S";

                string suffix = ftype switch
                {
                    "TOWER"       => "TWR",
                    "GROUND"      => "GND",
                    "APPROACH"    => "APP",
                    "DELIVERY"    => "DEL",
                    "DEPARTURE"   => "DEP",
                    "CENTER"      => "CTR",
                    "CONTROL"     => "CTR",
                    "ACC"         => "CTR",
                    "RADAR"       => "APP",
                    "CLEARANCE"   => "DEL",
                    "FIS"         => "FSS",
                    "AFIS"        => "FSS",
                    "FSS"         => "FSS",
                    "INFORMATION" => "FSS",
                    "RADIO"       => "FSS",
                    "UNICOM"      => "UNICOM",
                    "MULTICOM"    => "UNICOM",
                    _             => null,   // ATIS and unrecognised - skip in pass 1
                };
                if (suffix == null) continue;

                string callsign = $"{ap.Icao}_{suffix}";
                if (!seenFreq.Add(callsign)) continue;

                frqOut.Add($"{callsign};{fmhz:F3};{ap.Icao};;ATIS\\voice{rwyType}.atis;0;;ATIS\\{rwyType}rwy.atis;");
            }

            // ── Pass 2: FIR/CTR fallback - if no ATC freqs found at all ──────
            //   Some small airports have no dedicated ATC positions. Include any
            //   non-ATIS frequency as a FSS (Flight Service Station) fallback so
            //   the airport at least appears in the .atc output.
            if (frqOut.Count == frqBefore && freqArray.Count > 0)
            {
                foreach (JObject freq in freqArray)
                {
                    string ftype = (freq["type"]?.ToString() ?? "").ToUpper().Trim();
                    double fmhz  = freq["frequency_mhz"]?.Value<double>() ?? 0;
                    if (fmhz <= 0) continue;

                    // Skip ATIS-type entries
                    if (ftype.Contains("ATIS")) continue;

                    string callsign = $"{ap.Icao}_FSS";
                    if (!seenFreq.Add(callsign)) continue;

                    frqOut.Add($"{callsign};{fmhz:F3};{ap.Icao};;ATIS\\voice{rwyType}.atis;0;;ATIS\\{rwyType}rwy.atis;");
                    break;   // one FSS entry per airport is enough
                }
            }

            return (runwayArray.Count, frqOut.Count - frqBefore);
        }

        // ────────────────────────────────────────────────────────────────────
        //  FIR CTR / FSS probe - called when no CTR or FSS was found in the
        //  regular airport frequency sweep.
        //
        //  Strategy:
        //   1. Try common FIR suffix patterns on airac.net
        //      ({prefix}F, {prefix}CF, …)
        //   2. If nothing found, fall back to OpenAIP
        // ────────────────────────────────────────────────────────────────────
        private async Task ApFetchFirCtrAsync(
            HttpClient client, string region,
            List<string> frqOut, HashSet<string> seenFreq)
        {
            // ── Step 1: try suffix patterns on airac.net ──────────────────────
            ApStatus("Probing for FIR CTR/FSS…");
            ApLog("\n  Probing navigation database for FIR CTR/FSS…");

            string[] suffixes = { "F", "CF", "MF", "BF", "DF", "RF", "NF", "SF", "TF", "AF" };
            foreach (string suf in suffixes)
            {
                string firIcao = region + suf;
                var res = await client.GetAsync($"{ApApiBase}/airports/{firIcao}");
                if (!res.IsSuccessStatusCode) continue;

                JObject root    = JObject.Parse(await res.Content.ReadAsStringAsync());
                JObject airport = root["data"] as JObject ?? root;

                string apIcao = airport["icao"]?.ToString()
                             ?? airport["identifier"]?.ToString() ?? "";
                if (!apIcao.StartsWith(region, StringComparison.OrdinalIgnoreCase)) continue;

                bool added = false;
                foreach (JObject freq in airport["frequencies"] as JArray ?? new JArray())
                {
                    string ftype = (freq["type"]?.ToString() ?? "").ToUpper().Trim();
                    double fmhz  = freq["frequency_mhz"]?.Value<double>() ?? 0;
                    if (fmhz <= 0) continue;

                    string suffix = ftype switch
                    {
                        "CENTER"      => "CTR",
                        "CONTROL"     => "CTR",
                        "ACC"         => "CTR",
                        "FIS"         => "FSS",
                        "AFIS"        => "FSS",
                        "FSS"         => "FSS",
                        "INFORMATION" => "FSS",
                        "RADIO"       => "FSS",
                        _             => null,
                    };
                    if (suffix == null) continue;

                    string callsign = $"{apIcao}_{suffix}";
                    if (!seenFreq.Add(callsign)) continue;

                    frqOut.Add($"{callsign};{fmhz:F3};{region};;ATIS\\voicesingle.atis;0;;ATIS\\singlerwy.atis;");
                    ApLog($"\n  FIR {suffix,-3}: {callsign}  {fmhz:F3} MHz",
                          color: Color.FromArgb(13, 71, 161));
                    added = true;
                }

                if (added) return;
            }

            ApLog("\n  No FIR CTR/FSS found - checking secondary source…",
                  color: Color.FromArgb(100, 120, 150));

            // ── Step 2: OpenAIP fallback ──────────────────────────────────────
            bool oaipFound = await ApFetchFirCtrFromOpenAipAsync(client, region, frqOut, seenFreq);
            if (!oaipFound)
                ApLog("\n  ⚠ No FIR CTR/FSS position found - may need to be added manually",
                      color: Color.FromArgb(200, 80, 0));
        }

        // ────────────────────────────────────────────────────────────────────
        //  OpenAIP fallback - queries api.openaip.net for airports whose ICAO
        //  starts with the FIR prefix and extracts CENTER / FIS / FSS freqs.
        //  Requires OpenAIP API key in Settings.
        // ────────────────────────────────────────────────────────────────────
        private async Task<bool> ApFetchFirCtrFromOpenAipAsync(
            HttpClient _, string region,
            List<string> frqOut, HashSet<string> seenFreq)
        {
            if (string.IsNullOrEmpty(_openAipApiKey))
            {
                ApLog("\n  ℹ Secondary data source not configured - set it in Settings to enable this fallback",
                      color: Color.FromArgb(130, 140, 160));
                return false;
            }

            try
            {
                using var oaip = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
                oaip.DefaultRequestHeaders.Add("x-openaip-api-key", _openAipApiKey);

                // Prefer country-scoped query; fall back to un-scoped 200-item page
                string url = !string.IsNullOrEmpty(_apLastCountryIso)
                    ? $"https://api.openaip.net/api/airports?country={_apLastCountryIso}&page=1&limit=200"
                    : $"https://api.openaip.net/api/airports?page=1&limit=200";

                var res = await oaip.GetAsync(url);
                if (!res.IsSuccessStatusCode)
                {
                    ApLog($"\n  ⚠ Secondary source returned HTTP {(int)res.StatusCode}",
                          color: Color.FromArgb(200, 80, 0));
                    return false;
                }

                JObject obj   = JObject.Parse(await res.Content.ReadAsStringAsync());
                JArray  items = obj["items"] as JArray
                             ?? obj["data"]  as JArray
                             ?? new JArray();

                bool found = false;
                foreach (JObject ap in items)
                {
                    string icao = ap["icaoCode"]?.ToString()
                               ?? ap["icao"]?.ToString() ?? "";
                    if (!icao.StartsWith(region, StringComparison.OrdinalIgnoreCase)) continue;

                    var freqs = ap["frequencies"] as JArray ?? new JArray();
                    foreach (JObject freq in freqs)
                    {
                        string fname = (freq["name"]?.ToString() ?? "").ToUpper();
                        string fval  = freq["value"]?.ToString()
                                    ?? freq["frequency"]?.ToString() ?? "0";
                        if (!double.TryParse(fval,
                            System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out double fmhz)
                            || fmhz <= 0) continue;

                        string suffix =
                            fname.Contains("CENTER")  || fname.Contains("CONTROL") || fname.Contains("ACC") ? "CTR" :
                            fname.Contains("FIS")     || fname.Contains("FSS")     ||
                            fname.Contains("INFORMA") || fname.Contains("RADIO")   ? "FSS" : null;
                        if (suffix == null) continue;

                        string callsign = $"{icao}_{suffix}";
                        if (!seenFreq.Add(callsign)) continue;

                        frqOut.Add($"{callsign};{fmhz:F3};{region};;ATIS\\voicesingle.atis;0;;ATIS\\singlerwy.atis;");
                        ApLog($"\n  FIR {suffix,-3}: {callsign}  {fmhz:F3} MHz",
                              color: Color.FromArgb(13, 71, 161));
                        found = true;
                    }
                }
                return found;
            }
            catch (Exception ex)
            {
                ApLog($"\n  ⚠ Secondary source error: {ex.Message}", color: Color.FromArgb(200, 80, 0));
                return false;
            }
        }

        // ────────────────────────────────────────────────────────────────────
        //  Single airport fetch (4-letter ICAO mode)
        // ────────────────────────────────────────────────────────────────────
        private async Task ApFetchSingleAirportAsync(HttpClient client, string icao)
        {
            var res = await client.GetAsync($"{ApApiBase}/airports/{icao}");
            if (!res.IsSuccessStatusCode)
            {
                ApLog($"\n  ✗ Airport {icao} not found (HTTP {(int)res.StatusCode})",
                      color: Color.FromArgb(200, 80, 0));
                return;
            }

            JObject root    = JObject.Parse(await res.Content.ReadAsStringAsync());
            JObject airport = root["data"] as JObject ?? root;

            string airIcao = airport["icao"]?.ToString()          ?? icao;
            string airName = airport["name"]?.ToString()           ?? "";
            double airLat  = airport["latitude"]?.Value<double>()  ?? 0;
            double airLon  = airport["longitude"]?.Value<double>() ?? 0;
            int    airElev = airport["elevation_ft"]?.Value<int>() ?? 0;

            _apAirportOutput = ApHeader_Airport() +
                $"{airIcao};{airElev};{ApToN(airLat)};{ApToE(airLon)};{airName};";
            ApLog($"\n  Airport  : {airIcao} - {airName}  elev={airElev}ft");

            var rwLines  = new List<string>();
            var frqLines = new List<string>();
            var seenFreq = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var apInfo = new ApInfo(airIcao, airName, airLat, airLon, airElev);
            var (rwCount, frqCount) = await ApFetchDetailAsync(client, apInfo, rwLines, frqLines, seenFreq);

            _apRunwayOutput = ApHeader_Runway() + string.Join(Environment.NewLine, rwLines);
            _apFreqOutput   = ApHeader_Atc()   + string.Join(Environment.NewLine, frqLines);

            ApLog($"\n  Runways  : {rwCount}");
            if (frqCount == 0)
                ApLog($"\n  Freqs    : 0  ⚠ no ATC frequencies found - FSS fallback applied if any freq existed",
                      color: Color.FromArgb(200, 120, 0));
            else
                ApLog($"\n  Freqs    : {frqCount}");
        }

        // ────────────────────────────────────────────────────────────────────
        //  Navaid fetch - paginated, filtered by ICAO region prefix
        //  vorType: 1=VOR  2=VOR/DME  4=DME  -1=NDB
        // ────────────────────────────────────────────────────────────────────
        private async Task<List<string>> ApFetchNavaidsAsync(
            HttpClient client, string region, string apiType, int vorType)
        {
            var lines = new List<string>();
            var seen  = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int  page    = 1;
            bool hasMore = true;

            while (hasMore)
            {
                string url = $"{ApApiBase}/navaids?type={apiType}" +
                             $"&region={region}&per_page=100&page={page}";
                var res = await client.GetAsync(url);
                if (!res.IsSuccessStatusCode) break;

                JObject obj  = JObject.Parse(await res.Content.ReadAsStringAsync());
                JArray  data = obj["data"] as JArray;
                if (data == null || data.Count == 0) break;

                foreach (JObject nav in data)
                {
                    string ident = nav["identifier"]?.ToString()
                                ?? nav["ident"]?.ToString() ?? "";
                    if (string.IsNullOrEmpty(ident) || !seen.Add(ident)) continue;

                    double lat = nav["latitude"]?.Value<double>()  ?? 0;
                    double lon = nav["longitude"]?.Value<double>() ?? 0;

                    if (vorType == -1)
                    {
                        double fkhz = nav["frequency_khz"]?.Value<double>()
                                   ?? nav["frequency"]?.Value<double>() ?? 0;
                        lines.Add($"{ident};{fkhz:F1};{ApToN(lat)};{ApToE(lon)};");
                    }
                    else
                    {
                        double fmhz = nav["frequency_mhz"]?.Value<double>()
                                   ?? nav["frequency"]?.Value<double>() ?? 0;
                        lines.Add($"{ident};{fmhz:F3};{ApToN(lat)};{ApToE(lon)};1;{vorType};");
                    }
                }

                hasMore = obj["pagination"]?["has_more"]?.Value<bool>() ?? false;
                page++;
            }

            ApLog($"\n  {apiType,-8}: {lines.Count} navaids");
            return lines;
        }

        // ────────────────────────────────────────────────────────────────────
        //  Haversine offset - centre lat/lon → new point at bearing + dist (nm)
        // ────────────────────────────────────────────────────────────────────
        private static (double lat, double lon) ApGeoOffset(
            double lat0, double lon0, double bearingDeg, double distNm)
        {
            const double R = 3440.065;
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
        //  DMS conversion  - decimal degrees → N/S DD.MM.SS.mmm
        // ────────────────────────────────────────────────────────────────────
        private static string ApToN(double lat)
        {
            string h   = lat >= 0 ? "N" : "S"; lat = Math.Abs(lat);
            int    deg = (int)lat;
            double mD  = (lat - deg) * 60; int min = (int)mD;
            double sD  = (mD - min)  * 60; int sec = (int)sD;
            int    ms  = (int)((sD - sec) * 1000) % 1000;
            return $"{h}{deg:00}.{min:00}.{sec:00}.{ms:000}";
        }

        private static string ApToE(double lon)
        {
            string h   = lon >= 0 ? "E" : "W"; lon = Math.Abs(lon);
            int    deg = (int)lon;
            double mD  = (lon - deg) * 60; int min = (int)mD;
            double sD  = (mD - min)  * 60; int sec = (int)sD;
            int    ms  = (int)((sD - sec) * 1000) % 1000;
            return $"{h}{deg:000}.{min:00}.{sec:00}.{ms:000}";
        }

        // ────────────────────────────────────────────────────────────────────
        //  Format header comments - prepended to each output file
        // ────────────────────────────────────────────────────────────────────
        private static string ApHeader_Airport() =>
            $"// IVAO Aurora Sector File - Airport Data (.ap)\n" +
            $"// Format: ICAO;ELEVATION_FT;LAT;LON;NAME;\n" +
            $"//   ELEVATION_FT : airport elevation in feet\n" +
            $"//   LAT/LON      : DMS  N/S DD.MM.SS.mmm  /  E/W DDD.MM.SS.mmm\n" +
            $"// Made by Veda Moola (656077)\n" +
            $"// Ref: https://wiki.ivao.aero/en/home/devops/manuals/SectorFile_Definition\n" +
            $"// AIRAC {SplashForm.AiracCycle}\n" +
            $"// ⚠  NOT FOR REAL WORLD USE\n\n";

        private static string ApHeader_Runway() =>
            $"// IVAO Aurora Sector File - Runway Data (.rw)\n" +
            $"// Format: ICAO;BASE_ID;RECIP_ID;BASE_BRG;RECIP_BRG;BASE_THRESH_LAT;BASE_THRESH_LON;RECIP_THRESH_LAT;RECIP_THRESH_LON;\n" +
            $"//   BASE_ID / RECIP_ID  : runway designators (e.g. 09, 27)\n" +
            $"//   BASE_BRG / RECIP_BRG: magnetic bearing in degrees\n" +
            $"//   THRESH coords       : computed from airport reference ± (length/2) along heading\n" +
            $"//   LAT/LON             : DMS  N/S DD.MM.SS.mmm  /  E/W DDD.MM.SS.mmm\n" +
            $"// Made by Veda Moola (656077)\n" +
            $"// Ref: https://wiki.ivao.aero/en/home/devops/manuals/SectorFile_Definition\n" +
            $"// AIRAC {SplashForm.AiracCycle}\n" +
            $"// ⚠  NOT FOR REAL WORLD USE\n\n";

        private static string ApHeader_Atc() =>
            $"// IVAO Aurora Sector File - ATC Frequencies (.atc)\n" +
            $"// Format: CALLSIGN;FREQ_MHZ;VIS_POINTS;ALIAS;VOICE_ATIS;FLAGS;RESERVED;ATIS_FILE;\n" +
            $"//   CALLSIGN  : ICAO_SUFFIX  (e.g. VABB_TWR, VABB_GND, VABB_APP, VABB_DEL, VABB_DEP, VABB_CTR)\n" +
            $"//   FREQ_MHZ  : frequency in MHz to 3 decimal places\n" +
            $"//   VIS_POINTS: space-separated ICAO codes visible to this position\n" +
            $"//   ALIAS     : (empty)\n" +
            $"//   VOICE_ATIS: ATIS\\voicesingle.atis  or  ATIS\\voicedual.atis\n" +
            $"//   FLAGS     : 0 = primary position\n" +
            $"//   RESERVED  : (empty)\n" +
            $"//   ATIS_FILE : ATIS\\singlerwy.atis  or  ATIS\\dualrwy.atis\n" +
            $"// Made by Veda Moola (656077)\n" +
            $"// Ref: https://wiki.ivao.aero/en/home/devops/manuals/SectorFile_Definition\n" +
            $"// AIRAC {SplashForm.AiracCycle}\n" +
            $"// ⚠  NOT FOR REAL WORLD USE\n\n";

        private static string ApHeader_Vor() =>
            $"// IVAO Aurora Sector File - VHF Navaids (.vor)\n" +
            $"// Includes: VOR, VOR/DME, DME\n" +
            $"// Format: IDENT;FREQ_MHZ;LAT;LON;1;TYPE;\n" +
            $"//   IDENT   : navaid identifier\n" +
            $"//   FREQ_MHZ: frequency in MHz to 3 decimal places\n" +
            $"//   LAT/LON : DMS  N/S DD.MM.SS.mmm  /  E/W DDD.MM.SS.mmm\n" +
            $"//   1       : always 1\n" +
            $"//   TYPE    : 1=VOR  2=VOR/DME  4=DME\n" +
            $"// Made by Veda Moola (656077)\n" +
            $"// Ref: https://wiki.ivao.aero/en/home/devops/manuals/SectorFile_Definition\n" +
            $"// AIRAC {SplashForm.AiracCycle}\n" +
            $"// ⚠  NOT FOR REAL WORLD USE\n\n";

        private static string ApHeader_Ndb() =>
            $"// IVAO Aurora Sector File - NDB Navaids (.ndb)\n" +
            $"// Format: IDENT;FREQ_KHZ;LAT;LON;\n" +
            $"//   IDENT   : navaid identifier\n" +
            $"//   FREQ_KHZ: frequency in kHz to 1 decimal place\n" +
            $"//   LAT/LON : DMS  N/S DD.MM.SS.mmm  /  E/W DDD.MM.SS.mmm\n" +
            $"// Made by Veda Moola (656077)\n" +
            $"// Ref: https://wiki.ivao.aero/en/home/devops/manuals/SectorFile_Definition\n" +
            $"// AIRAC {SplashForm.AiracCycle}\n" +
            $"// ⚠  NOT FOR REAL WORLD USE\n\n";

        // ────────────────────────────────────────────────────────────────────
        //  Save helper
        // ────────────────────────────────────────────────────────────────────
        private void ApSave(string data, string filter, string ext)
        {
            if (string.IsNullOrEmpty(data))
            {
                MessageBox.Show("No data to download. Fetch data first.",
                    "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string term = apRegionBox.Text.Trim().ToUpper();
            using var dlg = new SaveFileDialog
            {
                Filter   = filter,
                Title    = $"Save  -  {term}",
                FileName = $"{term}.{ext}",
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(dlg.FileName, data);
                MessageBox.Show($"Saved to {dlg.FileName}", "Saved",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ────────────────────────────────────────────────────────────────────
        //  Log / progress / status helpers
        // ────────────────────────────────────────────────────────────────────
        private void ApLog(string text, bool bold = false,
            Color? color = null, bool isLink = false)
        {
            if (apLogBox.InvokeRequired)
                apLogBox.Invoke((MethodInvoker)(() =>
                    ApLogDirect(text, bold, color, isLink)));
            else
                ApLogDirect(text, bold, color, isLink);
        }

        private void ApLogDirect(string text, bool bold, Color? color, bool isLink)
        {
            int start = apLogBox.TextLength;
            apLogBox.AppendText(text);
            apLogBox.Select(start, text.Length);

            if (isLink)
            {
                apLogBox.SelectionColor = Color.CornflowerBlue;
                apLogBox.SelectionFont  = new Font(apLogBox.Font, FontStyle.Underline);
            }
            else
            {
                apLogBox.SelectionFont = bold
                    ? new Font(apLogBox.Font, FontStyle.Bold)
                    : new Font(apLogBox.Font, FontStyle.Regular);
                apLogBox.SelectionColor = color ?? Color.FromArgb(30, 40, 65);
            }

            apLogBox.Select(apLogBox.TextLength, 0);
            apLogBox.ScrollToCaret();
        }

        private void ApProgress(int value)
        {
            if (apProgressBar.InvokeRequired)
                apProgressBar.Invoke((MethodInvoker)(() => apProgressBar.Value = value));
            else
                apProgressBar.Value = value;
        }

        private void ApStatus(string text)
        {
            if (apStatusLabel.InvokeRequired)
                apStatusLabel.Invoke((MethodInvoker)(() => apStatusLabel.Text = text));
            else
                apStatusLabel.Text = text;
        }
    }
}
