using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Sector_File
{
    // ─────────────────────────────────────────────────────────────────────────
    //  Ground Layout tool  — embedded panel in MainForm
    //
    //  Reads a KML file exported from Google Earth / QGIS and produces:
    //    .tfl  — IVAO Aurora filled polygons (aprons, taxiways, buildings)
    //    .geo  — IVAO Aurora geo lines (taxiway centre-lines, stand lines)
    //    .txi  — IVAO Aurora taxiway labels
    //    .rw   — IVAO Aurora runway config (from Point placemarks named "Runway XX")
    //
    //  Conversion format identical to the standalone LoadingForm, targeting
    //  IVAO Aurora sector file specification.
    // ─────────────────────────────────────────────────────────────────────────
    partial class MainForm
    {
        // ── State ─────────────────────────────────────────────────────────────
        private string _grTflOutput = "";
        private string _grGeoOutput = "";
        private string _grTxiOutput = "";
        private string _grRwOutput  = "";
        private string _grKmlPath   = "";
        private readonly HashSet<string> _grProcessedRunways = new(StringComparer.OrdinalIgnoreCase);

        // ────────────────────────────────────────────────────────────────────
        //  Import button — select KML then process
        // ────────────────────────────────────────────────────────────────────
        private async void GrImport_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "KML/KMZ Files (*.kml;*.kmz)|*.kml;*.kmz|KML Files (*.kml)|*.kml|KMZ Files (*.kmz)|*.kmz|All Files (*.*)|*.*",
                Title  = "Select KML or KMZ Ground Layout File",
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            string ext = Path.GetExtension(ofd.FileName).ToLowerInvariant();
            if (ext != ".kml" && ext != ".kmz")
            {
                MessageBox.Show("Please select a KML or KMZ file.", "Invalid File",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _grKmlPath   = ofd.FileName;
            _grTflOutput = _grGeoOutput = _grTxiOutput = _grRwOutput = "";
            _grProcessedRunways.Clear();

            grLogBox.Clear();
            GrProgress(0);
            grImportButton.Enabled = false;
            GrStatus("Loading…");

            // Attribution header
            GrLog("Made by ");
            GrLog("Veda Moola (656077)", isLink: true);
            GrLog("  |  Tested by ");
            GrLog("Nilay Parsodkar (709833)", isLink: true);
            GrLog("\n⚠  NOT FOR REAL WORLD USE\n",
                  bold: true, color: Color.FromArgb(200, 60, 60));
            GrLog($"\nAIRAC {SplashForm.AiracCycle}");
            string fileExt = Path.GetExtension(_grKmlPath).ToUpperInvariant().TrimStart('.');
            GrLog($"\n{fileExt,-4} : {Path.GetFileName(_grKmlPath)}\n");

            // Format reference
            var dim  = Color.FromArgb(110, 130, 160);
            var blue = Color.FromArgb(13, 71, 161);
            GrLog("\n━━  Output File Formats  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━",
                  bold: true, color: blue);
            GrLog("\n  .tfl  Polygons  : ", color: dim);
            GrLog("STATIC;FILL;LINE_W;LINE_COLOUR;FILL_OPACITY;  then lat;lon rows");
            GrLog("\n  .geo  Lines     : ", color: dim);
            GrLog("startLat;startLon;endLat;endLon;TAXI_CENTER");
            GrLog("\n  .txi  Labels    : ", color: dim);
            GrLog("LABEL;ICAO;LAT;LON;");
            GrLog("\n  .rw   Runways   : ", color: dim);
            GrLog("ICAO;RWY;OPP;PRI_ELEV;OPP_ELEV;HDG;HDG_OPP;PRI_LAT;PRI_LON;OPP_LAT;OPP_LON");
            GrLog("\n  Ref: ", color: dim);
            GrLog("wiki.ivao.aero/en/home/devops/manuals/SectorFile_Definition", isLink: true);
            GrLog("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n",
                  color: blue);

            var sw = Stopwatch.StartNew();
            try
            {
                GrLog("\nProcessing KML geometry…");
                await Task.Run(() => GrReadKml(_grKmlPath));

                GrProgress(100);
                GrLog("\n\nOutput summary:", bold: true, color: blue);
                GrLog($"\n  TFL polygons : {(_grTflOutput.Length > 0 ? "✔" : "✗  none")}");
                GrLog($"\n  GEO lines    : {(_grGeoOutput.Length > 0 ? "✔" : "✗  none")}");
                GrLog($"\n  TXI labels   : {(_grTxiOutput.Length > 0 ? "✔" : "✗  none")}");
                GrLog($"\n  RW runways   : {(_grRwOutput.Length  > 0 ? "✔" : "✗  none")}");

                sw.Stop();
                string elapsed = sw.Elapsed.TotalSeconds > 1
                    ? $"{sw.Elapsed.TotalSeconds:F2}s"
                    : $"{sw.Elapsed.TotalMilliseconds:F0}ms";
                GrLog($"\n\n✔  Done in {elapsed}", bold: true, color: Color.FromArgb(22, 163, 74));
            }
            catch (Exception ex)
            {
                GrLog($"\n✗ Error: {ex.Message}", color: Color.FromArgb(200, 60, 60));
            }
            finally
            {
                grImportButton.Enabled = true;
                GrStatus("Done");

                // Update file label and enable download buttons
                if (!string.IsNullOrEmpty(_grKmlPath))
                {
                    grFileLabel.Text      = Path.GetFileName(_grKmlPath);
                    grFileLabel.ForeColor = Color.FromArgb(30, 40, 65);
                }
                bool any = !string.IsNullOrEmpty(_grTflOutput) || !string.IsNullOrEmpty(_grGeoOutput)
                        || !string.IsNullOrEmpty(_grTxiOutput) || !string.IsNullOrEmpty(_grRwOutput);
                grDownloadTflBtn.Enabled = any;
                grDownloadGeoBtn.Enabled = any;
                grDownloadTxiBtn.Enabled = any;
                grDownloadRwBtn .Enabled = any;
            }
        }

        // ────────────────────────────────────────────────────────────────────
        //  KML reading — runs on background thread.
        //  Handles both .kml (plain XML) and .kmz (ZIP containing doc.kml).
        // ────────────────────────────────────────────────────────────────────
        private void GrReadKml(string filePath)
        {
            var xmlDoc = new XmlDocument();

            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            if (ext == ".kmz")
            {
                // KMZ = ZIP archive; extract the first .kml entry (usually doc.kml)
                using var zip = ZipFile.OpenRead(filePath);
                ZipArchiveEntry? kmlEntry = null;
                foreach (var entry in zip.Entries)
                {
                    if (entry.FullName.EndsWith(".kml", StringComparison.OrdinalIgnoreCase))
                    {
                        kmlEntry = entry; break;
                    }
                }
                if (kmlEntry == null)
                    throw new InvalidDataException("No .kml file found inside the KMZ archive.");

                using var stream = kmlEntry.Open();
                xmlDoc.Load(stream);
            }
            else
            {
                xmlDoc.Load(filePath);
            }

            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("kml", "http://www.opengis.net/kml/2.2");

            GrLog("\n  Reading placemarks…");
            GrProgress(20);

            XmlNodeList placemarks = xmlDoc.SelectNodes("//kml:Placemark", nsmgr);
            if (placemarks == null || placemarks.Count == 0)
            {
                GrLog("\n  ⚠ No placemarks found", color: Color.FromArgb(200, 80, 0));
                return;
            }

            GrLog($"\n  Found {placemarks.Count} placemarks");
            GrProgress(40);

            string folderName = GrGetFolderName(xmlDoc, nsmgr);

            foreach (XmlNode pm in placemarks)
            {
                XmlNode poly  = pm.SelectSingleNode(".//kml:Polygon",   nsmgr);
                XmlNode line  = pm.SelectSingleNode(".//kml:LineString", nsmgr);
                XmlNode point = pm.SelectSingleNode(".//kml:Point",      nsmgr);

                if      (poly  != null) GrProcessPolygon(pm, poly,  nsmgr, xmlDoc);
                else if (line  != null) GrProcessLine   (pm, line,  nsmgr);
                else if (point != null) GrProcessPoint  (pm, point, nsmgr, folderName);
            }

            GrProgress(90);
            GrLog("\n  KML processing complete");
        }

        // ── Polygon → TFL ─────────────────────────────────────────────────────
        private void GrProcessPolygon(XmlNode pm, XmlNode poly,
            XmlNamespaceManager ns, XmlDocument xmlDoc)
        {
            string  name     = GrGetName(pm, ns);
            string  styleUrl = pm.SelectSingleNode("kml:styleUrl", ns)?.InnerText.Trim() ?? "";
            var     style    = GrGetStyle(styleUrl, ns, xmlDoc);
            XmlNode coords   = poly.SelectSingleNode(".//kml:coordinates", ns);
            if (coords == null) return;

            string data = $"\n// {name}\n" +
                          $"STATIC;{style.fill};{style.lineW};{style.line};{style.fillOp};\n";

            foreach (string c in coords.InnerText.Trim()
                .Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var p = c.Split(',');
                if (p.Length < 2) continue;
                if (!TryParseDouble(p[0], out double lon)) continue;
                if (!TryParseDouble(p[1], out double lat)) continue;
                data += $"{GrToN(lat)};{GrToE(lon)}\n";
            }
            _grTflOutput += data;
        }

        // ── LineString → GEO ─────────────────────────────────────────────────
        private void GrProcessLine(XmlNode pm, XmlNode line, XmlNamespaceManager ns)
        {
            string  name   = GrGetName(pm, ns);
            XmlNode coords = line.SelectSingleNode(".//kml:coordinates", ns);
            if (coords == null) return;

            string[] pts  = coords.InnerText.Trim()
                .Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            string   data = $"\n// {name}\n";

            for (int i = 0; i < pts.Length - 1; i++)
            {
                var s = pts[i].Split(',');
                var e = pts[i + 1].Split(',');
                if (s.Length < 2 || e.Length < 2) continue;
                if (!TryParseDouble(s[0], out double sLon)) continue;
                if (!TryParseDouble(s[1], out double sLat)) continue;
                if (!TryParseDouble(e[0], out double eLon)) continue;
                if (!TryParseDouble(e[1], out double eLat)) continue;
                data += $"{GrToN(sLat)};{GrToE(sLon)};{GrToN(eLat)};{GrToE(eLon)};TAXI_CENTER\n";
            }
            _grGeoOutput += data;
        }

        // ── Point → TXI label or RW runway ────────────────────────────────────
        private void GrProcessPoint(XmlNode pm, XmlNode point,
            XmlNamespaceManager ns, string folderName)
        {
            string  name   = GrGetName(pm, ns);
            XmlNode coords = point.SelectSingleNode(".//kml:coordinates", ns);
            if (coords == null) return;

            var parts = coords.InnerText.Trim().Split(',');
            if (parts.Length < 2) return;
            if (!TryParseDouble(parts[0], out double lon)) return;
            if (!TryParseDouble(parts[1], out double lat)) return;

            // Runway placemarks start with "Runway" or "R\d"
            if (name.StartsWith("Runway", StringComparison.OrdinalIgnoreCase) ||
                Regex.IsMatch(name, @"^R\d", RegexOptions.IgnoreCase))
            {
                GrProcessRunway(name, lat, lon, pm.OwnerDocument!, ns, folderName);
                return;
            }

            // Taxi label
            if (string.IsNullOrEmpty(_grTxiOutput))
                _grTxiOutput = $"// {folderName} Taxi Labels\n";
            _grTxiOutput += $"{name};{folderName};{GrToN(lat)};{GrToE(lon)};\n";
        }

        // ── Runway processing ─────────────────────────────────────────────────
        private void GrProcessRunway(string name, double lat, double lon,
            XmlDocument xmlDoc, XmlNamespaceManager ns, string folderName)
        {
            var m = Regex.Match(name, @"(\d{1,2})([LRC]?)");
            if (!m.Success) return;

            string primary    = m.Groups[1].Value;
            string designator = m.Groups[2].Value;
            if (_grProcessedRunways.Contains(primary)) return;

            int    oppNum = (int.Parse(primary) + 18) % 36;
            string oppStr = oppNum.ToString("D2");

            XmlNode? oppNode =
                xmlDoc.SelectSingleNode($"//kml:Placemark[kml:name='Runway {oppStr}{designator}']", ns) ??
                xmlDoc.SelectSingleNode($"//kml:Placemark[kml:name='{oppStr}{designator}']", ns);

            double oppLat = lat, oppLon = lon;
            if (oppNode != null)
            {
                XmlNode? oppCoords = oppNode.SelectSingleNode(".//kml:coordinates", ns);
                if (oppCoords != null)
                {
                    var op = oppCoords.InnerText.Trim().Split(',');
                    if (op.Length >= 2)
                    {
                        TryParseDouble(op[0], out oppLon);
                        TryParseDouble(op[1], out oppLat);
                    }
                }
                _grProcessedRunways.Add(oppStr);
            }

            string hdg    = (int.Parse(primary) * 10).ToString("D3");
            string hdgOpp = (oppNum * 10).ToString("D3");

            _grRwOutput +=
                $"{folderName};{primary}{designator};{oppStr}{designator};" +
                $"PRIMARY_ELEVATION;OPPOSITE_ELEVATION;" +
                $"{hdg};{hdgOpp};" +
                $"{GrToN(lat)};{GrToE(lon)};" +
                $"{GrToN(oppLat)};{GrToE(oppLon)}\n";

            _grProcessedRunways.Add(primary);
        }

        // ── Style lookup ──────────────────────────────────────────────────────
        private (string fill, string line, string lineW, string fillOp)
            GrGetStyle(string styleUrl, XmlNamespaceManager ns, XmlDocument xmlDoc)
        {
            string fill = "FFFFFF", line = "000000", lineW = "1", fillOp = "0";
            if (string.IsNullOrEmpty(styleUrl) || !styleUrl.StartsWith("#"))
                return (fill, line, lineW, fillOp);

            string id = styleUrl.Substring(1);

            // Check for StyleMap → recurse with normal style URL
            XmlNode? sm = xmlDoc.SelectSingleNode($"//kml:StyleMap[@id='{id}']", ns);
            if (sm != null)
            {
                string? normalUrl = sm.SelectSingleNode(
                    ".//kml:Pair[kml:key='normal']/kml:styleUrl", ns)?.InnerText.Trim();
                if (!string.IsNullOrEmpty(normalUrl))
                    return GrGetStyle(normalUrl, ns, xmlDoc);
            }

            XmlNode? sty = xmlDoc.SelectSingleNode($"//kml:Style[@id='{id}']", ns);
            if (sty == null) return (fill, line, lineW, fillOp);

            string rawFill = sty.SelectSingleNode("kml:PolyStyle/kml:color", ns)?.InnerText ?? fill;
            fill   = "#" + KmlColorToRgb(rawFill);
            fillOp = sty.SelectSingleNode("kml:PolyStyle/kml:fill",          ns)?.InnerText == "1" ? "1" : "0";
            string rawLine = sty.SelectSingleNode("kml:LineStyle/kml:color",  ns)?.InnerText ?? line;
            line   = "#" + KmlColorToRgb(rawLine);
            lineW  = sty.SelectSingleNode("kml:LineStyle/kml:width",          ns)?.InnerText ?? lineW;
            return (fill, line, lineW, fillOp);
        }

        // KML encodes color as AABBGGRR — convert to RRGGBB for Aurora
        private static string KmlColorToRgb(string kml)
        {
            kml = kml.Trim();
            if (kml.Length == 8)
                return kml.Substring(6, 2) + kml.Substring(4, 2) + kml.Substring(2, 2);
            if (kml.Length == 6)
                return kml.Substring(4, 2) + kml.Substring(2, 2) + kml.Substring(0, 2);
            return kml;
        }

        private static string GrGetName(XmlNode pm, XmlNamespaceManager ns)
            => pm.SelectSingleNode("kml:name", ns)?.InnerText.Trim() ?? "Unnamed";

        private static string GrGetFolderName(XmlDocument doc, XmlNamespaceManager ns)
            => doc.SelectSingleNode("//kml:Folder/kml:name", ns)?.InnerText.Trim() ?? "Airport";

        private static bool TryParseDouble(string s, out double result) =>
            double.TryParse(s.Trim(),
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out result);

        // ── DMS conversions ───────────────────────────────────────────────────
        private static string GrToN(double lat)
        {
            string h = lat >= 0 ? "N" : "S"; lat = Math.Abs(lat);
            int deg = (int)lat; double mD = (lat - deg) * 60; int min = (int)mD;
            double sD = (mD - min) * 60; int sec = (int)sD; int ms = (int)((sD - sec) * 1000) % 1000;
            return $"{h}{deg:00}.{min:00}.{sec:00}.{ms:000}";
        }
        private static string GrToE(double lon)
        {
            string h = lon >= 0 ? "E" : "W"; lon = Math.Abs(lon);
            int deg = (int)lon; double mD = (lon - deg) * 60; int min = (int)mD;
            double sD = (mD - min) * 60; int sec = (int)sD; int ms = (int)((sD - sec) * 1000) % 1000;
            return $"{h}{deg:000}.{min:00}.{sec:00}.{ms:000}";
        }

        // ── Download helpers ─────────────────────────────────────────────────
        private void GrDownloadTfl_Click(object s, EventArgs e)
            => GrSave(_grTflOutput, "TFL Files (*.tfl)|*.tfl", "tfl");
        private void GrDownloadGeo_Click(object s, EventArgs e)
            => GrSave(_grGeoOutput, "GEO Files (*.geo)|*.geo", "geo");
        private void GrDownloadTxi_Click(object s, EventArgs e)
            => GrSave(_grTxiOutput, "TXI Files (*.txi)|*.txi", "txi");
        private void GrDownloadRw_Click(object s, EventArgs e)
            => GrSave(_grRwOutput,  "RW Files (*.rw)|*.rw",    "rw");

        private void GrSave(string data, string filter, string ext)
        {
            if (string.IsNullOrEmpty(data))
            {
                MessageBox.Show("No data to save — import a KML file first.",
                    "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string name = string.IsNullOrEmpty(_grKmlPath)
                ? "output" : Path.GetFileNameWithoutExtension(_grKmlPath);
            using var dlg = new SaveFileDialog
            {
                Filter   = filter,
                FileName = $"{name}.{ext}",
                Title    = $"Save  —  {name}.{ext}",
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(dlg.FileName, data);
                MessageBox.Show($"Saved to {dlg.FileName}", "Saved",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ── Log / progress / status ──────────────────────────────────────────
        private void GrLog(string text, bool bold = false,
            Color? color = null, bool isLink = false)
        {
            if (grLogBox.InvokeRequired)
                grLogBox.Invoke((MethodInvoker)(() => GrLogDirect(text, bold, color, isLink)));
            else
                GrLogDirect(text, bold, color, isLink);
        }

        private void GrLogDirect(string text, bool bold, Color? color, bool isLink)
        {
            int start = grLogBox.TextLength;
            grLogBox.AppendText(text);
            grLogBox.Select(start, text.Length);
            if (isLink)
            {
                grLogBox.SelectionColor = Color.CornflowerBlue;
                grLogBox.SelectionFont  = new Font(grLogBox.Font, FontStyle.Underline);
            }
            else
            {
                grLogBox.SelectionFont  = bold
                    ? new Font(grLogBox.Font, FontStyle.Bold)
                    : new Font(grLogBox.Font, FontStyle.Regular);
                grLogBox.SelectionColor = color ?? Color.FromArgb(30, 40, 65);
            }
            grLogBox.Select(grLogBox.TextLength, 0);
            grLogBox.ScrollToCaret();
        }

        private void GrProgress(int value)
        {
            if (grProgressBar.InvokeRequired)
                grProgressBar.Invoke((MethodInvoker)(() => grProgressBar.Value = value));
            else
                grProgressBar.Value = value;
        }

        private void GrStatus(string text)
        {
            if (grStatusLabel.InvokeRequired)
                grStatusLabel.Invoke((MethodInvoker)(() => grStatusLabel.Text = text));
            else
                grStatusLabel.Text = text;
        }
    }
}
