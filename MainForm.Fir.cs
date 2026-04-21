using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Sector_File
{
    // ─────────────────────────────────────────────────────────────────────────
    //  FIR & Airspace Data  — embedded panel in MainForm
    //
    //  All airspace types (FIR / UIR / TMA / CTR / CTA / R / D / P):
    //    1. airac.net  GET /airspaces?fir={ID}  (paginated)
    //    2. OpenAIP    GET /airspaces?country={ISO}&type={N}  (fallback, API-key auth)
    //
    //  Output format (Aurora .artcc):
    //    T;IDENTIFIER;LAT;LON;
    // ─────────────────────────────────────────────────────────────────────────
    partial class MainForm
    {
        // ── Output buffers ────────────────────────────────────────────────────
        private string _firOutput           = "";
        private string _firUirOutput        = "";
        private string _firTmaOutput        = "";
        private string _firCtrOutput        = "";
        private string _firCtaOutput        = "";
        private string _firRestrictedOutput = "";
        private string _firDangerOutput     = "";
        private string _firProhibitedOutput = "";

        // ── Cached shapes (cleared on new FIR term) ───────────────────────────
        private string _lastFirTerm = "";

        // Cached airac.net / OpenAIP shapes, keyed by normalised type string
        private readonly Dictionary<string, List<FirShape>> _airspaceCache = new();

        // ── airac.net base (shared with Airport / Country pages) ──────────────
        // ApApiBase is defined in MainForm.Airport.cs as const string

        // ── OpenAIP base ──────────────────────────────────────────────────────
        private const string OpenAipBase = "https://api.core.openaip.net/api";

        // ── OpenAIP type numbers (verified by live API probe) ────────────────
        // https://docs.openaip.net  → Airspace → type enum
        //  0 = AIRWAY/UNDEFINED   1 = RESTRICTED     2 = DANGER
        //  3 = PROHIBITED         4 = CTR            5 = TMZ
        //  6 = RMZ                7 = TMA            8 = GLIDER/TRA
        //  9 = TRA               10 = FIR ← actual FIR boundary type in OpenAIP
        // 12 = ADIZ              13 = ATZ            14 = MATZ
        // 15 = AIRWAY            18 = ALERT          21 = MOA
        // 23 = ACC               24 = CTA            25 = FIR-P
        // 26 = CTA/WARNING       27 = ACC SECTOR     28 = TRNG
        // 29 = OTHER             30 = MCTR (Military CTR)
        //
        // NOTE: type=11 has 0 entries globally in OpenAIP — FIR is actually type=10.
        // UIR is not a separate type; UIR boundaries are included in type=10.
        //
        // FIR entries in OpenAIP use city/ICAO names (e.g. "MUMBAI", "FIR LFBB", "EDGG").
        // Matching is done via the human name returned by airac.net (e.g. "MUMBAI" for VABF).
        //
        // ICAO class numbers (icaoClass param):
        //  0 = A   1 = B   2 = C   3 = D   4 = E   5 = F   6 = G
        private static readonly Dictionary<string, int[]> OpenAipTypes = new()
        {
            ["FIR"]        = new[] { 10 },       // 10 = FIR boundary (verified)
            ["UIR"]        = new[] { 10 },       // UIR included in type=10
            ["TMA"]        = new[] { 7  },       //  7 = TMA (Terminal Maneuvering Area)
            ["CTR"]        = new[] { 4, 30 },    //  4 = CTR, 30 = MCTR (Military CTR)
            ["CTA"]        = new[] { 24, 26 },   // 24 = CTA, 26 = WARNING
            ["RESTRICTED"] = new[] { 1  },       //  1 = RESTRICTED
            ["DANGER"]     = new[] { 2  },       //  2 = DANGER
            ["PROHIBITED"] = new[] { 3  },       //  3 = PROHIBITED
        };

        // ── ICAO 2-letter prefix → ISO-3166 alpha-2 country code ─────────────
        private static readonly Dictionary<string, string> IcaoToIso = new(StringComparer.OrdinalIgnoreCase)
        {
            ["AG"] = "SB", ["AN"] = "NR",
            ["AY"] = "PG",
            // Africa
            ["DA"] = "DZ", ["DB"] = "BJ", ["DF"] = "BF", ["DG"] = "GH",
            ["DI"] = "CI", ["DN"] = "NG", ["DR"] = "NE", ["DT"] = "TN", ["DX"] = "TG",
            ["FA"] = "ZA", ["FB"] = "BW", ["FC"] = "CG", ["FD"] = "SZ",
            ["FE"] = "CF", ["FG"] = "GQ", ["FH"] = "SH", ["FI"] = "MG",
            ["FK"] = "CM", ["FL"] = "ZM", ["FN"] = "AO", ["FO"] = "GA",
            ["FP"] = "ST", ["FQ"] = "MZ", ["FS"] = "SC", ["FT"] = "TD",
            ["FV"] = "ZW", ["FW"] = "MW", ["FX"] = "LS", ["FY"] = "NA", ["FZ"] = "CD",
            ["GA"] = "ML", ["GB"] = "GM", ["GC"] = "ES", ["GF"] = "SL",
            ["GG"] = "GW", ["GL"] = "LR", ["GM"] = "MA", ["GO"] = "SN",
            ["GQ"] = "MR", ["GS"] = "EH", ["GU"] = "GN", ["GV"] = "CV",
            ["HA"] = "ET", ["HB"] = "BI", ["HC"] = "SO", ["HD"] = "DJ",
            ["HE"] = "EG", ["HH"] = "ER", ["HK"] = "KE", ["HL"] = "LY",
            ["HR"] = "RW", ["HS"] = "SD", ["HT"] = "TZ", ["HU"] = "UG",
            // Europe
            ["EB"] = "BE", ["ED"] = "DE", ["EE"] = "EE", ["EF"] = "FI",
            ["EG"] = "GB", ["EH"] = "NL", ["EI"] = "IE", ["EK"] = "DK",
            ["EL"] = "LU", ["EN"] = "NO", ["EP"] = "PL", ["ES"] = "SE",
            ["EV"] = "LV", ["EY"] = "LT",
            ["LA"] = "AL", ["LB"] = "BG", ["LC"] = "CY", ["LD"] = "HR",
            ["LE"] = "ES", ["LF"] = "FR", ["LG"] = "GR", ["LH"] = "HU",
            ["LI"] = "IT", ["LJ"] = "SI", ["LK"] = "CZ", ["LL"] = "IL",
            ["LM"] = "MT", ["LO"] = "AT", ["LP"] = "PT", ["LQ"] = "BA",
            ["LR"] = "RO", ["LS"] = "CH", ["LT"] = "TR", ["LU"] = "MD",
            ["LW"] = "MK", ["LX"] = "GI", ["LY"] = "RS", ["LZ"] = "SK",
            // Middle East
            ["OA"] = "AF", ["OB"] = "BH", ["OE"] = "SA", ["OI"] = "IR",
            ["OJ"] = "JO", ["OK"] = "KW", ["OL"] = "LB", ["OM"] = "AE",
            ["OO"] = "OM", ["OP"] = "PK", ["OR"] = "IQ", ["OS"] = "SY",
            ["OT"] = "QA", ["OY"] = "YE",
            // South / Southeast Asia
            ["VA"] = "IN", ["VB"] = "MM", ["VC"] = "LK", ["VD"] = "KH",
            ["VE"] = "IN", ["VG"] = "BD", ["VH"] = "HK", ["VI"] = "IN",
            ["VL"] = "LA", ["VM"] = "MO", ["VN"] = "NP", ["VO"] = "IN",
            ["VQ"] = "BT", ["VR"] = "MV", ["VT"] = "TH", ["VV"] = "VN",
            ["VY"] = "MM",
            // Southeast Asia (western)
            ["WA"] = "ID", ["WB"] = "MY", ["WI"] = "ID", ["WM"] = "MY",
            ["WP"] = "TL", ["WR"] = "ID", ["WS"] = "SG",
            // East Asia
            ["RC"] = "TW", ["RJ"] = "JP", ["RK"] = "KR", ["RP"] = "PH",
            ["RO"] = "PH",
            // Russia / CIS
            ["UA"] = "KZ", ["UB"] = "AZ", ["UC"] = "KG", ["UD"] = "AM",
            ["UG"] = "GE", ["UK"] = "UA", ["UM"] = "BY",
            ["UR"] = "RU", ["UW"] = "RU",
            ["UT"] = "UZ",
            // Americas
            ["K"]  = "US", ["PA"] = "US", ["PH"] = "US",
            ["C"]  = "CA",
            ["MM"] = "MX",
            ["SA"] = "AR", ["SB"] = "BR", ["SC"] = "CL", ["SD"] = "BR",
            ["SE"] = "EC", ["SG"] = "PY", ["SK"] = "CO", ["SL"] = "BO",
            ["SM"] = "SR", ["SO"] = "GF", ["SP"] = "PE", ["SS"] = "BR",
            ["SU"] = "UY", ["SV"] = "VE", ["SW"] = "BR", ["SY"] = "GY",
            ["MG"] = "GT", ["MH"] = "HN", ["MK"] = "JM", ["MN"] = "NI",
            ["MP"] = "PA", ["MR"] = "CR", ["MS"] = "SV", ["MT"] = "HT",
            ["MU"] = "CU", ["MY"] = "BS", ["MZ"] = "BZ",
            ["MB"] = "TC", ["MD"] = "DO",
            // Pacific
            ["NF"] = "FJ", ["NL"] = "WF", ["NT"] = "PF", ["NV"] = "VU",
            ["NW"] = "NC", ["NZ"] = "NZ",
            ["PG"] = "GU", ["PK"] = "FM", ["PT"] = "PW",
            // Australia
            ["Y"]  = "AU",
            // China / Mongolia
            ["ZB"] = "CN", ["ZG"] = "CN", ["ZH"] = "CN", ["ZJ"] = "CN",
            ["ZK"] = "KP", ["ZL"] = "CN", ["ZM"] = "MN", ["ZP"] = "CN",
            ["ZS"] = "CN", ["ZT"] = "CN", ["ZU"] = "CN", ["ZW"] = "CN",
            ["ZY"] = "CN",
        };

        // ── Simple shape holder ────────────────────────────────────────────────
        private record FirShape(string Name, List<(double Lat, double Lon)> Points);

        // ────────────────────────────────────────────────────────────────────
        //  Search button
        // ────────────────────────────────────────────────────────────────────
        private async void FirSearch_Click(object sender, EventArgs e)
        {
            string term = firSearchBox.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(term))
            {
                MessageBox.Show("Enter a FIR identifier (e.g. VTBB, WSSS, VABF).",
                    "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool doFir        = chkFirBoundary.Checked;
            bool doUir        = chkFirUir.Checked;
            bool doTma        = chkFirTma.Checked;
            bool doCtr        = chkFirCtr.Checked;
            bool doCta        = chkFirCta.Checked;
            bool doRestricted = chkFirRestricted.Checked;
            bool doDanger     = chkFirDanger.Checked;
            bool doProhibited = chkFirProhibited.Checked;

            if (!doFir && !doUir && !doTma && !doCtr && !doCta &&
                !doRestricted && !doDanger && !doProhibited)
            {
                MessageBox.Show("Select at least one airspace type to fetch.",
                    "Nothing Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string prefix = term.Length >= 2 ? term[..2] : term;

            // Clear cache if FIR term changed
            if (!term.Equals(_lastFirTerm, StringComparison.OrdinalIgnoreCase))
            {
                _airspaceCache.Clear();
                _lastFirTerm = term;
            }

            _firOutput = _firUirOutput = _firTmaOutput = _firCtrOutput = "";
            _firCtaOutput = _firRestrictedOutput = _firDangerOutput = _firProhibitedOutput = "";
            FirSetDownloadButtons(false);

            firLogBox.Clear();
            FirProgress(0);
            firSearchButton.Enabled = false;
            FirStatus("Fetching…");

            FirLog("\n");
            FirLog("Made by ");
            FirLog("Veda Moola (656077)", isLink: true);
            FirLog("\n⚠  NOT FOR REAL WORLD USE\n", bold: true, color: Color.FromArgb(200, 60, 60));
            FirLog($"\nFIR: {term}  ·  Prefix: {prefix}");

            // Resolve ISO country code for OpenAIP fallback
            string isoCode = ResolveIso(prefix);
            FirLog($"  ·  Country: {(isoCode != "" ? isoCode : "unknown")}\n",
                color: Color.FromArgb(60, 80, 120));

            var sw = Stopwatch.StartNew();

            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
                client.DefaultRequestHeaders.Add("User-Agent", "IVAO-SectorFileCreator/1.0");

                var shapes = await FetchAllAirspaces(client, term, prefix, isoCode);
                FirProgress(80);

                if (doFir)        _firOutput           = BuildOutputFromShapes(shapes, "FIR",        term, "FIR Boundary");
                if (doUir)        _firUirOutput        = BuildOutputFromShapes(shapes, "UIR",        term, "UIR Boundary");
                if (doTma)        _firTmaOutput        = BuildOutputFromShapes(shapes, "TMA",        term, "TMA");
                if (doCtr)        _firCtrOutput        = BuildOutputFromShapes(shapes, "CTR",        term, "CTR / CTZ");
                if (doCta)        _firCtaOutput        = BuildOutputFromShapes(shapes, "CTA",        term, "CTA");
                if (doRestricted) _firRestrictedOutput = BuildOutputFromShapes(shapes, "RESTRICTED", term, "Restricted Airspace");
                if (doDanger)     _firDangerOutput     = BuildOutputFromShapes(shapes, "DANGER",     term, "Danger Areas");
                if (doProhibited) _firProhibitedOutput = BuildOutputFromShapes(shapes, "PROHIBITED", term, "Prohibited Areas");

                FirProgress(100);
                sw.Stop();
                FirLog($"\n\n✔  Done in {sw.Elapsed.TotalSeconds:F2}s",
                    bold: true, color: Color.FromArgb(22, 163, 74));

                FirLog("\n\n");
                void Row(string label, string data)
                {
                    if (string.IsNullOrEmpty(data)) return;
                    FirLog($"  {label,-18}", bold: true, color: Color.FromArgb(30, 64, 175));
                    FirLog($"{CountShapes(data),4} shape(s)   {CountLines(data),5} pt(s)\n",
                        color: Color.FromArgb(55, 65, 81));
                }
                Row("FIR Boundary",  _firOutput);
                Row("UIR Boundary",  _firUirOutput);
                Row("TMA",           _firTmaOutput);
                Row("CTR/CTZ",       _firCtrOutput);
                Row("CTA",           _firCtaOutput);
                Row("Restricted",    _firRestrictedOutput);
                Row("Danger",        _firDangerOutput);
                Row("Prohibited",    _firProhibitedOutput);

                FirSetDownloadButtons(true);
            }
            catch (Exception ex)
            {
                FirLog($"\n✗ Error: {ex.Message}", color: Color.FromArgb(200, 60, 60));
                _airspaceCache.Clear();
            }
            finally
            {
                firSearchButton.Enabled = true;
                FirStatus("Done");
            }
        }

        // ────────────────────────────────────────────────────────────────────
        //  Fetch all airspace shapes for a region
        //  Priority: airac.net per type → OpenAIP fallback for missing types
        // ────────────────────────────────────────────────────────────────────
        private async Task<Dictionary<string, List<FirShape>>> FetchAllAirspaces(
            HttpClient client, string term, string prefix, string isoCode)
        {
            var result = new Dictionary<string, List<FirShape>>(StringComparer.OrdinalIgnoreCase);

            // ── 1. airac.net (priority source) ────────────────────────────
            FirLog("\nQuerying airspace data…");
            var (airacShapes, firHumanName) = await FetchAiracNetAirspaces(client, term, prefix);

            int airacTotal = airacShapes.Values.Sum(v => v.Count);
            if (airacTotal > 0)
            {
                FirLog($" {airacTotal} shape(s) found.", color: Color.FromArgb(22, 163, 74));
                foreach (var (typeKey, shapes) in airacShapes)
                {
                    if (!result.ContainsKey(typeKey)) result[typeKey] = new();
                    result[typeKey].AddRange(shapes);
                }
            }
            else
            {
                FirLog(" no data.", color: Color.FromArgb(150, 130, 0));
            }

            if (!string.IsNullOrEmpty(firHumanName))
                FirLog($"  FIR name: {firHumanName}", color: Color.FromArgb(80, 100, 150));

            // ── 2. OpenAIP fallback — only for types missing from airac.net ──
            var missingTypes = new[] { "FIR", "UIR", "TMA", "CTR", "CTA", "RESTRICTED", "DANGER", "PROHIBITED" }
                .Where(t => !result.ContainsKey(t) || result[t].Count == 0)
                .ToList();

            if (missingTypes.Count > 0 && !string.IsNullOrEmpty(_openAipApiKey))
            {
                FirLog($"\nQuerying additional sources for: {string.Join(", ", missingTypes)}…");
                var openAipShapes = await FetchOpenAIPAirspaces(
                    client, isoCode, missingTypes, term, firHumanName);

                if (openAipShapes.Count > 0)
                {
                    FirLog($" {openAipShapes.Values.Sum(v => v.Count)} shape(s) found.",
                        color: Color.FromArgb(22, 163, 74));
                    foreach (var (typeKey, shapes) in openAipShapes)
                    {
                        if (!result.ContainsKey(typeKey)) result[typeKey] = new();
                        result[typeKey].AddRange(shapes);
                    }
                }
                else
                {
                    FirLog(" no data.", color: Color.FromArgb(150, 130, 0));
                }
            }
            else if (missingTypes.Count > 0)
            {
                FirLog("\nAdditional airspace source not configured — skipping.",
                    color: Color.FromArgb(150, 130, 0));
            }

            return result;
        }

        // ────────────────────────────────────────────────────────────────────
        //  Fetch from airac.net  GET /airspace/fir?identifier={term}
        //  Returns: (shapes, firHumanName)
        //    firHumanName — the human city/region name from the "FIR" type entry
        //                   (e.g. "MUMBAI" for VABF, used to match OpenAIP results)
        // ────────────────────────────────────────────────────────────────────
        private async Task<(Dictionary<string, List<FirShape>> Shapes, string FirHumanName)>
            FetchAiracNetAirspaces(HttpClient client, string term, string prefix)
        {
            var result = new Dictionary<string, List<FirShape>>(StringComparer.OrdinalIgnoreCase);
            string firHumanName = "";

            // Primary: correct airac.net FIR endpoint
            string url = $"{ApApiBase}/airspace/fir?identifier={term}";
            string json;
            try   { json = await client.GetStringAsync(url); }
            catch { return (result, firHumanName); }

            JToken root;
            try   { root = JToken.Parse(json); }
            catch { return (result, firHumanName); }

            // Response may be a raw array, a {data:[...]} wrapper, or a single object
            JArray items = null;
            if (root is JArray arr)
            {
                items = arr;
            }
            else if (root is JObject obj)
            {
                items = obj["data"]      as JArray
                     ?? obj["items"]     as JArray
                     ?? obj["airspaces"] as JArray
                     ?? obj["results"]   as JArray;

                if (items == null)
                    items = new JArray(obj);
            }

            if (items == null || items.Count == 0)
                return (result, firHumanName);

            foreach (JObject item in items.OfType<JObject>())
            {
                // type may be a string OR an object {"code":"FIR","description":"..."}
                string rawType = (item["type"] is JObject typeObj)
                    ? typeObj["code"]?.ToString() ?? ""
                    : item["type"]?.ToString()
                   ?? item["airspace_type"]?.ToString() ?? "";

                string typeKey = NormaliseAiracType(rawType);

                // Capture the human FIR name from the first true "FIR" entry
                // (not sub-regions / ACC sectors) — strip "FIR"/"UIR" suffix
                if (string.IsNullOrEmpty(firHumanName) &&
                    rawType.Equals("FIR", StringComparison.OrdinalIgnoreCase) &&
                    item["is_subregion"]?.Value<bool>() != true)
                {
                    string n = item["name"]?.ToString() ?? "";
                    // Strip trailing " FIR" / " UIR" / " ACC" etc.
                    foreach (string suffix in new[] { " FIR", " UIR", " ACC", " CONTROL" })
                        if (n.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                            n = n[..^suffix.Length].Trim();
                    firHumanName = n;
                }

                if (string.IsNullOrEmpty(typeKey)) continue;

                string name = (item["name"] ?? item["icao"] ?? item["identifier"])
                                  ?.ToString() ?? term;
                name = SanitiseName(name);

                var pts = ExtractPoints(item);
                if (pts.Count < 2) continue;  // airac.net metadata has no coords → falls to OpenAIP

                if (!result.ContainsKey(typeKey)) result[typeKey] = new();
                result[typeKey].Add(new FirShape(name, pts));
            }

            return (result, firHumanName);
        }

        // ────────────────────────────────────────────────────────────────────
        //  Fetch from OpenAIP  /airspaces
        //  firTerm:      4-letter ICAO FIR code (e.g. "VABF")
        //  firHumanName: city name from airac.net (e.g. "MUMBAI") — used to
        //                match OpenAIP type=10 FIR entries by name
        // ────────────────────────────────────────────────────────────────────
        private async Task<Dictionary<string, List<FirShape>>> FetchOpenAIPAirspaces(
            HttpClient client, string isoCode, List<string> typeKeys,
            string firTerm = "", string firHumanName = "")
        {
            var result = new Dictionary<string, List<FirShape>>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(isoCode)) return result;

            string apiKey = _openAipApiKey;

            foreach (string typeKey in typeKeys)
            {
                if (!OpenAipTypes.TryGetValue(typeKey.ToUpper(), out int[] typeNums)) continue;

                // ── Phase 1: query by OpenAIP type number + country ──────────
                // For FIR/UIR: a country can have multiple FIRs (e.g. India has 4).
                // We must also filter by name so we return only the searched FIR,
                // not every FIR in the country.
                bool isFirUirP1 = typeKey.Equals("FIR", StringComparison.OrdinalIgnoreCase) ||
                                  typeKey.Equals("UIR", StringComparison.OrdinalIgnoreCase);
                string prefix2p1 = firTerm.Length >= 2 ? firTerm[..2] : firTerm;

                foreach (int typeNum in typeNums)
                {
                    int page  = 1;
                    int limit = 100;
                    bool more = true;

                    while (more)
                    {
                        string url = $"{OpenAipBase}/airspaces?country={isoCode}" +
                                     $"&type={typeNum}&page={page}&limit={limit}";
                        string json;
                        try
                        {
                            using var req = new HttpRequestMessage(HttpMethod.Get, url);
                            req.Headers.Add("x-openaip-api-key", apiKey);
                            var resp = await client.SendAsync(req);
                            if (!resp.IsSuccessStatusCode) break;
                            json = await resp.Content.ReadAsStringAsync();
                        }
                        catch { break; }

                        JObject root;
                        try   { root = JObject.Parse(json); }
                        catch { break; }

                        var items = root["items"] as JArray ?? root["data"] as JArray;
                        if (items == null || items.Count == 0) break;

                        if (!result.ContainsKey(typeKey)) result[typeKey] = new();

                        foreach (JObject item in items.OfType<JObject>())
                        {
                            string rawName = item["name"]?.ToString() ?? "UNK";

                            // For FIR/UIR: only keep the entry whose name matches this FIR.
                            // A country can have many FIRs (e.g. India has MUMBAI/DELHI/CHENNAI/KOLKATA).
                            if (isFirUirP1)
                            {
                                bool humanMatch = !string.IsNullOrEmpty(firHumanName) &&
                                    rawName.IndexOf(firHumanName, StringComparison.OrdinalIgnoreCase) >= 0;
                                bool codeMatch = !string.IsNullOrEmpty(firTerm) &&
                                    rawName.IndexOf(firTerm, StringComparison.OrdinalIgnoreCase) >= 0;
                                bool prefixMatch = !string.IsNullOrEmpty(prefix2p1) &&
                                    rawName.StartsWith(prefix2p1, StringComparison.OrdinalIgnoreCase);
                                if (!humanMatch && !codeMatch && !prefixMatch) continue;
                            }

                            // For FIR/UIR: use the ICAO FIR code as the shape identifier
                            // (e.g. "VABF" rather than the city name "MUMBAI")
                            string shapeName = isFirUirP1 && !string.IsNullOrEmpty(firTerm)
                                ? SanitiseName($"{firTerm}_FIR")
                                : SanitiseName(rawName);
                            var pts = ExtractOpenAipPoints(item);
                            if (pts.Count < 2) continue;
                            result[typeKey].Add(new FirShape(shapeName, pts));
                        }

                        int totalCount = root["totalCount"]?.Value<int>()
                                      ?? root["total"]?.Value<int>() ?? 0;
                        page++;
                        more = ((page - 1) * limit) < totalCount;
                        if (page > 50) break;
                    }
                }

                // ── Phase 2 (FIR / UIR only): OpenAIP FIR entries (type=10) have a
                //    country field, so Phase 1 works for many regions. But if it
                //    returned 0, fall back to a global search matched by name.
                //    Matching priority:
                //      1. firHumanName (e.g. "MUMBAI" from airac.net) — city name match
                //      2. firTerm (e.g. "VABF") — ICAO code in name (e.g. "FIR LFBB")
                //      3. 2-letter prefix (e.g. "VA") — broad prefix fallback
                bool isFirUir = typeKey.Equals("FIR", StringComparison.OrdinalIgnoreCase) ||
                                typeKey.Equals("UIR", StringComparison.OrdinalIgnoreCase);

                if (isFirUir &&
                    (!result.ContainsKey(typeKey) || result[typeKey].Count == 0) &&
                    (!string.IsNullOrEmpty(firTerm) || !string.IsNullOrEmpty(firHumanName)))
                {
                    string prefix2 = firTerm.Length >= 2 ? firTerm[..2] : firTerm;

                    FirLog($"\n  {typeKey}: no results with country filter — searching globally…",
                        color: Color.FromArgb(150, 130, 0));

                    foreach (int typeNum in typeNums)
                    {
                        int page  = 1;
                        int limit = 100;
                        bool more = true;

                        while (more)
                        {
                            string url = $"{OpenAipBase}/airspaces?type={typeNum}" +
                                         $"&page={page}&limit={limit}";
                            string json;
                            try
                            {
                                using var req = new HttpRequestMessage(HttpMethod.Get, url);
                                req.Headers.Add("x-openaip-api-key", apiKey);
                                var resp = await client.SendAsync(req);
                                if (!resp.IsSuccessStatusCode) break;
                                json = await resp.Content.ReadAsStringAsync();
                            }
                            catch { break; }

                            JObject root2;
                            try   { root2 = JObject.Parse(json); }
                            catch { break; }

                            var items2 = root2["items"] as JArray ?? root2["data"] as JArray;
                            if (items2 == null || items2.Count == 0) break;

                            foreach (JObject item in items2.OfType<JObject>())
                            {
                                string itemName = item["name"]?.ToString() ?? "";

                                // Match by human name (e.g. "MUMBAI" for VABF)
                                bool humanMatch = !string.IsNullOrEmpty(firHumanName) &&
                                    itemName.IndexOf(firHumanName, StringComparison.OrdinalIgnoreCase) >= 0;

                                // Match by ICAO code in name (e.g. "FIR LFBB" contains "LFBB")
                                bool codeMatch = !string.IsNullOrEmpty(firTerm) &&
                                    itemName.IndexOf(firTerm, StringComparison.OrdinalIgnoreCase) >= 0;

                                // Prefix fallback (e.g. "VA" for India)
                                bool prefixMatch = !string.IsNullOrEmpty(prefix2) &&
                                    itemName.StartsWith(prefix2, StringComparison.OrdinalIgnoreCase);

                                if (!humanMatch && !codeMatch && !prefixMatch) continue;

                                // Use ICAO FIR code as shape name
                                string safeName = !string.IsNullOrEmpty(firTerm)
                                    ? SanitiseName($"{firTerm}_FIR")
                                    : SanitiseName(itemName.Length > 0 ? itemName : "UNK");
                                var pts = ExtractOpenAipPoints(item);
                                if (pts.Count < 2) continue;
                                if (!result.ContainsKey(typeKey)) result[typeKey] = new();
                                result[typeKey].Add(new FirShape(safeName, pts));
                            }

                            int totalCount2 = root2["totalCount"]?.Value<int>()
                                           ?? root2["total"]?.Value<int>() ?? 0;
                            page++;
                            more = ((page - 1) * limit) < totalCount2;
                            if (page > 100) break;
                        }
                    }

                    // Phase 3: if still nothing, scan ICAO classes A–G globally
                    if (!result.ContainsKey(typeKey) || result[typeKey].Count == 0)
                    {
                        FirLog($"\n  Still 0 — scanning ICAO classes A–G globally for '{firTerm}'…",
                            color: Color.FromArgb(150, 130, 0));

                        for (int icaoClass = 0; icaoClass <= 6; icaoClass++)
                        {
                            int page  = 1;
                            int limit = 100;
                            bool more = true;

                            while (more)
                            {
                                string url = $"{OpenAipBase}/airspaces?icaoClass={icaoClass}" +
                                             $"&page={page}&limit={limit}";
                                string json;
                                try
                                {
                                    using var req = new HttpRequestMessage(HttpMethod.Get, url);
                                    req.Headers.Add("x-openaip-api-key", apiKey);
                                    var resp = await client.SendAsync(req);
                                    if (!resp.IsSuccessStatusCode) break;
                                    json = await resp.Content.ReadAsStringAsync();
                                }
                                catch { break; }

                                JObject root3;
                                try   { root3 = JObject.Parse(json); }
                                catch { break; }

                                var items3 = root3["items"] as JArray ?? root3["data"] as JArray;
                                if (items3 == null || items3.Count == 0) break;

                                foreach (JObject item in items3.OfType<JObject>())
                                {
                                    string itemName    = item["name"]?.ToString() ?? "";
                                    int    itemTypeNum = item["type"]?.Value<int>() ?? -1;

                                    bool nameMatch =
                                        itemName.IndexOf(firTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                        itemName.StartsWith(prefix2, StringComparison.OrdinalIgnoreCase);
                                    bool typeMatch = itemTypeNum == 11 || itemTypeNum == 12;
                                    if (!nameMatch && !typeMatch) continue;

                                    string matchedKey = itemTypeNum == 12 ? "UIR" : "FIR";
                                    if (!matchedKey.Equals(typeKey, StringComparison.OrdinalIgnoreCase))
                                        continue;

                                    string safeName = !string.IsNullOrEmpty(firTerm)
                                        ? SanitiseName($"{firTerm}_FIR")
                                        : SanitiseName(itemName.Length > 0 ? itemName : "UNK");
                                    var pts = ExtractOpenAipPoints(item);
                                    if (pts.Count < 2) continue;
                                    if (!result.ContainsKey(typeKey)) result[typeKey] = new();
                                    result[typeKey].Add(new FirShape(safeName, pts));
                                }

                                int totalCount3 = root3["totalCount"]?.Value<int>()
                                              ?? root3["total"]?.Value<int>() ?? 0;
                                page++;
                                more = ((page - 1) * limit) < totalCount3;
                                if (page > 100) break;
                            }
                        }
                    }

                    int found = result.TryGetValue(typeKey, out var found2) ? found2.Count : 0;
                    if (found > 0)
                        FirLog($" {found} shape(s) found.",
                            color: Color.FromArgb(22, 163, 74));
                    else
                        FirLog($" still 0 — {typeKey} not found in OpenAIP for '{firTerm}'.",
                            color: Color.FromArgb(200, 60, 60));
                }
            }

            return result;
        }

        // ────────────────────────────────────────────────────────────────────
        //  Aurora file header
        // ────────────────────────────────────────────────────────────────────
        private static string FirFileHeader(string label, string firId) =>
            $"// IVAO Aurora Sector File — {label} (.artcc)\n" +
            $"// FIR: {firId}\n" +
            $"// Format: T;IDENTIFIER;LAT;LON;\n" +
            $"//   IDENTIFIER : airspace name or ID\n" +
            $"//   LAT        : N/S DD.MM.SS.mmm\n" +
            $"//   LON        : E/W DDD.MM.SS.mmm\n" +
            $"//   T;Dummy;N000.00.00.000;E000.00.00.000  ← shape separator\n" +
            $"// Made by Veda Moola (656077)\n" +
            $"// Ref: https://wiki.ivao.aero/en/home/devops/manuals/SectorFile_Definition\n" +
            $"// AIRAC {SplashForm.AiracCycle}\n" +
            $"// ⚠  NOT FOR REAL WORLD USE\n\n";

        // ────────────────────────────────────────────────────────────────────
        //  Build Aurora .artcc output from a shape dictionary
        // ────────────────────────────────────────────────────────────────────
        private static string BuildOutputFromShapes(
            Dictionary<string, List<FirShape>> all, string typeKey,
            string firId = "", string label = "")
        {
            if (!all.TryGetValue(typeKey, out var shapes) || shapes.Count == 0)
                return "";

            string lbl = string.IsNullOrEmpty(label) ? typeKey : label;
            var sb = new StringBuilder();
            sb.Append(FirFileHeader(lbl, firId));

            foreach (var shape in shapes)
            {
                foreach (var (lat, lon) in shape.Points)
                    sb.AppendLine($"T;{shape.Name};{FirToN(lat)};{FirToE(lon)};");
                sb.AppendLine($"\nT;Dummy;N000.00.00.000;E000.00.00.000");
            }
            return sb.ToString();
        }

        // ────────────────────────────────────────────────────────────────────
        //  Coordinate extraction helpers
        // ────────────────────────────────────────────────────────────────────

        // Extract points from an airac.net airspace item (various possible formats)
        private static List<(double Lat, double Lon)> ExtractPoints(JObject item)
        {
            var pts = new List<(double, double)>();

            // Format 1: "points": [{lat, lng}] or [{lat, lon}] or [{latitude, longitude}]
            JArray points = item["points"] as JArray
                         ?? item["boundary"] as JArray
                         ?? item["coordinates"] as JArray;
            if (points != null)
            {
                foreach (JToken p in points)
                {
                    double? lat = p["lat"]?.Value<double>() ?? p["latitude"]?.Value<double>();
                    double? lon = p["lng"]?.Value<double>() ?? p["lon"]?.Value<double>()
                               ?? p["longitude"]?.Value<double>();
                    if (lat.HasValue && lon.HasValue)
                        pts.Add((lat.Value, lon.Value));
                    else if (p is JArray arr && arr.Count >= 2)
                    {
                        // Could be [lon, lat] (GeoJSON) or [lat, lon]
                        // Heuristic: longitude is typically larger absolute value for most of Asia/Europe
                        pts.Add((arr[1].Value<double>(), arr[0].Value<double>()));
                    }
                }
                return pts;
            }

            // Format 2: GeoJSON geometry
            var geom = item["geometry"] as JObject;
            if (geom != null)
                return ExtractGeoJsonPoints(geom);

            return pts;
        }

        // Extract points from an OpenAIP item (always GeoJSON)
        private static List<(double Lat, double Lon)> ExtractOpenAipPoints(JObject item)
        {
            var geom = item["geometry"] as JObject;
            return geom != null ? ExtractGeoJsonPoints(geom) : new();
        }

        private static List<(double Lat, double Lon)> ExtractGeoJsonPoints(JObject geom)
        {
            var pts  = new List<(double, double)>();
            string gtype = geom["type"]?.ToString() ?? "";
            var coords = geom["coordinates"];
            if (coords == null) return pts;

            // Polygon: coordinates[0] is the outer ring [[lon,lat],...]
            if (gtype == "Polygon")
            {
                var ring = coords[0] as JArray;
                if (ring != null)
                    foreach (JArray pt in ring.OfType<JArray>())
                        if (pt.Count >= 2)
                            pts.Add((pt[1].Value<double>(), pt[0].Value<double>())); // [lon,lat]→(lat,lon)
            }
            // MultiPolygon: coordinates[poly][ring][pt]
            else if (gtype == "MultiPolygon")
            {
                foreach (var poly in coords.OfType<JArray>())
                {
                    var ring = poly[0] as JArray;
                    if (ring == null) continue;
                    if (pts.Count > 0)
                    {
                        // Separator between polygons — caller handles via Dummy lines
                    }
                    foreach (JArray pt in ring.OfType<JArray>())
                        if (pt.Count >= 2)
                            pts.Add((pt[1].Value<double>(), pt[0].Value<double>()));
                }
            }

            return pts;
        }

        // ────────────────────────────────────────────────────────────────────
        //  Type normalisation — airac.net type string → our canonical key
        // ────────────────────────────────────────────────────────────────────
        private static string NormaliseAiracType(string raw)
        {
            string t = raw.Trim().ToUpper();
            if (t is "TMA" or "TMA/CTR" or "TMA/CTA") return "TMA";
            if (t is "CTR" or "CTZ" or "CTA/CTR" or "MCTR") return "CTR";
            if (t is "CTA" or "CTA/TMA") return "CTA";
            if (t is "R" or "RESTRICTED" or "RESTRICT") return "RESTRICTED";
            if (t is "D" or "DANGER") return "DANGER";
            if (t is "P" or "PROHIBITED" or "PROHIBIT") return "PROHIBITED";
            if (t is "FIR") return "FIR";
            if (t is "UIR") return "UIR";
            return "";
        }

        // ────────────────────────────────────────────────────────────────────
        //  ICAO prefix → ISO country code
        // ────────────────────────────────────────────────────────────────────
        private static string ResolveIso(string prefix)
        {
            if (IcaoToIso.TryGetValue(prefix, out string iso)) return iso;
            // Try single-letter prefix (K→US, C→CA, Y→AU, Z→CN)
            if (prefix.Length > 0 && IcaoToIso.TryGetValue(prefix[..1], out string iso1)) return iso1;
            return "";
        }

        // ────────────────────────────────────────────────────────────────────
        //  DMS formatters
        // ────────────────────────────────────────────────────────────────────
        private static string FirToN(double lat)
        {
            string h = lat >= 0 ? "N" : "S"; lat = Math.Abs(lat);
            int d = (int)lat; double mD = (lat - d) * 60; int m = (int)mD;
            double sD = (mD - m) * 60; int s = (int)sD; int ms = (int)((sD - s) * 1000) % 1000;
            return $"{h}{d:00}.{m:00}.{s:00}.{ms:000}";
        }

        private static string FirToE(double lon)
        {
            string h = lon >= 0 ? "E" : "W"; lon = Math.Abs(lon);
            int d = (int)lon; double mD = (lon - d) * 60; int m = (int)mD;
            double sD = (mD - m) * 60; int s = (int)sD; int ms = (int)((sD - s) * 1000) % 1000;
            return $"{h}{d:000}.{m:00}.{s:00}.{ms:000}";
        }

        // ────────────────────────────────────────────────────────────────────
        //  Utilities
        // ────────────────────────────────────────────────────────────────────
        private static string SanitiseName(string s)
        {
            var filtered = new string(s.Select(c => c is ' ' or '/' or '\\' ? '_' : c)
                                       .Where(c => c != ',' && c != ';')
                                       .ToArray());
            return filtered[..Math.Min(filtered.Length, 32)];
        }

        private static int CountLines(string s)
            => string.IsNullOrEmpty(s) ? 0
               : s.Split('\n').Count(l => l.TrimStart().StartsWith("T;") &&
                                          !l.Contains("T;Dummy;"));

        private static int CountShapes(string s)
            => string.IsNullOrEmpty(s) ? 0
               : Math.Max(1, s.Split('\n').Count(l => l.Contains("T;Dummy;")));

        // ────────────────────────────────────────────────────────────────────
        //  Enable/disable download buttons
        // ────────────────────────────────────────────────────────────────────
        private void FirSetDownloadButtons(bool enableBasedOnData)
        {
            if (firDlFirBtn.InvokeRequired)
            {
                firDlFirBtn.Invoke((MethodInvoker)(() => FirSetDownloadButtons(enableBasedOnData)));
                return;
            }
            firDlFirBtn.Enabled        = enableBasedOnData && !string.IsNullOrEmpty(_firOutput);
            firDlUirBtn.Enabled        = enableBasedOnData && !string.IsNullOrEmpty(_firUirOutput);
            firDlTmaBtn.Enabled        = enableBasedOnData && !string.IsNullOrEmpty(_firTmaOutput);
            firDlCtrBtn.Enabled        = enableBasedOnData && !string.IsNullOrEmpty(_firCtrOutput);
            firDlCtaBtn.Enabled        = enableBasedOnData && !string.IsNullOrEmpty(_firCtaOutput);
            firDlRestrictedBtn.Enabled = enableBasedOnData && !string.IsNullOrEmpty(_firRestrictedOutput);
            firDlDangerBtn.Enabled     = enableBasedOnData && !string.IsNullOrEmpty(_firDangerOutput);
            firDlProhibitedBtn.Enabled = enableBasedOnData && !string.IsNullOrEmpty(_firProhibitedOutput);

            // Combined button: enabled if ANY type has data
            bool anyData = enableBasedOnData && (
                !string.IsNullOrEmpty(_firOutput) || !string.IsNullOrEmpty(_firUirOutput) ||
                !string.IsNullOrEmpty(_firTmaOutput) || !string.IsNullOrEmpty(_firCtrOutput) ||
                !string.IsNullOrEmpty(_firCtaOutput) || !string.IsNullOrEmpty(_firRestrictedOutput) ||
                !string.IsNullOrEmpty(_firDangerOutput) || !string.IsNullOrEmpty(_firProhibitedOutput));
            firDlCombinedBtn.Enabled = anyData;
        }

        // ────────────────────────────────────────────────────────────────────
        //  Save / download
        // ────────────────────────────────────────────────────────────────────
        private void FirSave(string data, string label)
        {
            if (string.IsNullOrEmpty(data))
            {
                MessageBox.Show($"No {label} data to save — run a search first.",
                    "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string term = firSearchBox.Text.Trim().ToUpper();
            string tag  = label.Replace("/", "").Replace(" ", "_");
            using var dlg = new SaveFileDialog
            {
                Filter   = "ARTCC Files (*.artcc)|*.artcc|All Files (*.*)|*.*",
                FileName = $"{term}_{tag}.artcc",
                Title    = $"Save {label}  —  {term}",
            };
            if (dlg.ShowDialog() == DialogResult.OK)
                File.WriteAllText(dlg.FileName, data);
        }

        private void FirSaveCombined()
        {
            string term = firSearchBox.Text.Trim().ToUpper();
            var sb = new StringBuilder();

            // Single file header
            sb.Append(FirFileHeader("Combined Airspace", term));

            // Each section strips its own header (already has one from BuildOutputFromShapes)
            // and appends a section comment instead
            void Append(string label, string data)
            {
                if (string.IsNullOrEmpty(data)) return;
                // Strip the per-type header (everything up to the first blank line after headers)
                int bodyStart = data.IndexOf("\n\n");
                string body = bodyStart >= 0 ? data[(bodyStart + 2)..] : data;
                sb.AppendLine($"// ── {label} ──────────────────────────────────────────────");
                sb.Append(body);
                sb.AppendLine();
            }

            Append("FIR Boundary",       _firOutput);
            Append("UIR Boundary",       _firUirOutput);
            Append("TMA",                _firTmaOutput);
            Append("CTR / CTZ",          _firCtrOutput);
            Append("CTA",                _firCtaOutput);
            Append("Restricted Airspace",_firRestrictedOutput);
            Append("Danger Areas",       _firDangerOutput);
            Append("Prohibited Areas",   _firProhibitedOutput);

            string combined = sb.ToString();
            if (string.IsNullOrEmpty(combined.Trim()))
            {
                MessageBox.Show("No airspace data to save — run a search first.",
                    "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var dlg = new SaveFileDialog
            {
                Filter   = "ARTCC Files (*.artcc)|*.artcc|All Files (*.*)|*.*",
                FileName = $"{term}_Airspace.artcc",
                Title    = $"Save Combined Airspace  —  {term}",
            };
            if (dlg.ShowDialog() == DialogResult.OK)
                File.WriteAllText(dlg.FileName, combined);
        }

        private void FirDownload_Click(object sender, EventArgs e)
            => FirSave(_firOutput, "FIR_Boundary");

        // ────────────────────────────────────────────────────────────────────
        //  Log / progress / status
        // ────────────────────────────────────────────────────────────────────
        private void FirLog(string text, bool bold = false,
            Color? color = null, bool isLink = false)
        {
            if (firLogBox.InvokeRequired)
                firLogBox.Invoke((MethodInvoker)(() => FirLogDirect(text, bold, color, isLink)));
            else
                FirLogDirect(text, bold, color, isLink);
        }

        private void FirLogDirect(string text, bool bold, Color? color, bool isLink)
        {
            int start = firLogBox.TextLength;
            firLogBox.AppendText(text);
            firLogBox.Select(start, text.Length);
            if (isLink)
            {
                firLogBox.SelectionColor = Color.FromArgb(25, 118, 210);
                firLogBox.SelectionFont  = new Font(firLogBox.Font, FontStyle.Underline);
            }
            else
            {
                firLogBox.SelectionFont  = bold
                    ? new Font(firLogBox.Font, FontStyle.Bold)
                    : new Font(firLogBox.Font, FontStyle.Regular);
                firLogBox.SelectionColor = color ?? Color.FromArgb(30, 40, 65);
            }
            firLogBox.Select(firLogBox.TextLength, 0);
            firLogBox.ScrollToCaret();
        }

        private void FirProgress(int value)
        {
            if (firProgressBar.InvokeRequired)
                firProgressBar.Invoke((MethodInvoker)(() => firProgressBar.Value = value));
            else
                firProgressBar.Value = Math.Min(100, Math.Max(0, value));
        }

        private void FirStatus(string text)
        {
            if (firStatusLabel.InvokeRequired)
                firStatusLabel.Invoke((MethodInvoker)(() => firStatusLabel.Text = text));
            else
                firStatusLabel.Text = text;
        }
    }
}
