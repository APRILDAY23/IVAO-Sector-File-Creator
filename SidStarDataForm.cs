using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Windows.Forms;
using System.Xml.Linq;
using static Org.BouncyCastle.Math.Primes;

namespace Sector_File
{
    public partial class SidStarDataForm : Form
    {
        private int userIdInput;
        private string airacDirectory = "https://portal.in.ivao.aero/core/nav_data_2309019312309/data/nav_data_2309019312309";
        private string sidOutput;
        private string starOutput;

        public SidStarDataForm(int userId)
        {
            userIdInput = userId;
            this.Icon = new Icon("./tools.ico");
            InitializeComponent();
            searchButton.Click += SearchButton_Click;
            downloadSidButton.Click += DownloadSidButton_Click;
            downloadStarButton.Click += DownloadStarButton_Click;
        }

        private async void SearchButton_Click(object sender, EventArgs e)
        {
            loadingProgressBar.Value = 0;

            string icaoCode = searchBox.Text.Trim().ToLower(); // Get the ICAO code
            string filePath = $"{airacDirectory}/{icaoCode}.xml";
            string icaoCode_upper = searchBox.Text.Trim().ToUpper(); // Get the ICAO code

            // Append processing completion information
            AppendToDebugTextBox("Sector File Conversion made by ");
            AppendToDebugTextBox("Veda Moola (656077)", isHyperlink: true, hyperlinkUrl: "https://ivao.aero/Member.aspx?Id=656077");
            AppendToDebugTextBox("\nFully tested by ");
            AppendToDebugTextBox("Nilay Parsodkar (709833)", isHyperlink: true, hyperlinkUrl: "https://ivao.aero/Member.aspx?Id=709833");
            AppendToDebugTextBox(" Please blame him for any bugs. Haha!. \nBut if you face any problems please dm me or raise an issue on Github repo");
            AppendToDebugTextBox("\nNOT FOR REAL WORLD USE", isBold: true, textColor: Color.Red);
            AppendToDebugTextBox($"\nFetching SID and STAR data for airport: {icaoCode_upper}");

            // Start timing the process
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(filePath); // Use filePath as the URL
                    response.EnsureSuccessStatusCode();

                    string xmlContent = await response.Content.ReadAsStringAsync();

                    UpdateProgress(25); // Assuming the SID processing is 25% of the total work
                    // Process SID and STAR data
                    FormatSidData(xmlContent, icaoCode_upper);
                    UpdateProgress(50); // 50% after SID, STAR starts

                    FormatStarData(xmlContent, icaoCode_upper);
                    UpdateProgress(100); // Completed processing
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching data for ICAO code '{icaoCode}': {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Stop the stopwatch and log the time taken
            stopwatch.Stop();
            string timeMessage = stopwatch.Elapsed.TotalSeconds > 1
                ? $"\nPROCESS COMPLETED in {stopwatch.Elapsed.TotalSeconds:F2} seconds"
                : $"\nPROCESS COMPLETED in {stopwatch.Elapsed.TotalMilliseconds:F2} ms";

            AppendToDebugTextBox(timeMessage, isBold: true, textColor: Color.Green); // Append the completion message
        }


        private void UpdateProgress(int value)
        {
            if (loadingProgressBar.InvokeRequired)
            {
                loadingProgressBar.Invoke((MethodInvoker)delegate {
                    loadingProgressBar.Value = value;
                });
            }
            else
            {
                loadingProgressBar.Value = value;
            }
        }

        private void FormatSidData(string xmlContent, string icaoCode)
        {
            sidOutput = ""; // Reset the SID output

            try
            {
                XDocument doc = XDocument.Parse(xmlContent);

                if (doc == null)
                {
                    MessageBox.Show($"Failed to load SID data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                XNamespace ns = "http://tempuri.org/XMLSchema.xsd";

                var airport = doc.Descendants(ns + "Airport")
                                 .FirstOrDefault(a => a.Attribute("ICAOcode").Value.Equals(icaoCode, StringComparison.OrdinalIgnoreCase));

                if (airport == null)
                {
                    MessageBox.Show($"No airport found for '{icaoCode}'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool includeAltitude = MessageBox.Show("Do you want altitude in the SID file?", "Altitude Option for SID", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                bool includeSpeed = MessageBox.Show("Do you want speed in the SID file?", "Speed Option for SID", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                bool includeCoordinates = MessageBox.Show("Do you want coordinates in the SID file?", "Coordinate Option for SID", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

                var sids = airport.Descendants(ns + "Sid")
                                       .Select(s => new
                                       {
                                           Runway = s.Attribute("Runways").Value.Replace(",", ":"), // Replace commas with colons
                                           Name = s.Attribute("Name").Value,
                                           Waypoints = s.Elements(ns + "Sid_Waypoint")
                                                       .Select(w => new
                                                       {
                                                           Name = (string)w.Element(ns + "Name"),
                                                           Altitude = (int)w.Element(ns + "Altitude"),
                                                           AltitudeCons = (int?)w.Element(ns + "AltitudeCons") ?? 0,
                                                           AltitudeRestriction = (string)w.Element(ns + "AltitudeRestriction"),
                                                           Speed = (int?)w.Element(ns + "Speed") ?? 0,
                                                           Latitude = (double?)w.Element(ns + "Latitude") ?? 0,
                                                           Longitude = (double?)w.Element(ns + "Longitude") ?? 0
                                                       }).ToList(),
                                           Transitions = s.Elements(ns + "Sid_Transition")
                                                          .Select(t => new
                                                          {
                                                              Name = (string)t.Attribute("Name"),
                                                              Waypoints = t.Elements(ns + "SidTr_Waypoint")
                                                                          .Select(w => new
                                                                          {
                                                                              Name = (string)w.Element(ns + "Name"),
                                                                              Altitude = (int)w.Element(ns + "Altitude"),
                                                                              AltitudeCons = (int?)w.Element(ns + "AltitudeCons") ?? 0,
                                                                              AltitudeRestriction = (string)w.Element(ns + "AltitudeRestriction"),
                                                                              Speed = (int?)w.Element(ns + "Speed") ?? 0,
                                                                              Latitude = (double?)w.Element(ns + "Latitude") ?? 0,
                                                                              Longitude = (double?)w.Element(ns + "Longitude") ?? 0
                                                                          }).ToList()
                                                          }).ToList()
                                       }).ToList();

                if (!sids.Any())
                {
                    AppendToDebugTextBox($"\nNo SID for airport: {icaoCode}");
                    return;
                }

                foreach (var sid in sids)
                {
                    // Start of SID section
                    sidOutput += "// SID Start\n";

                    string sidPrefix = sid.Name.Length >= 6 ? sid.Name.Substring(0, 4) :
                                       sid.Name.Length > 0 && sid.Name.Length <= 5 ? sid.Name.Substring(0, 3) : sid.Name;

                    string matchingWaypointName = sid.Waypoints
                        .FirstOrDefault(w => w.Name.StartsWith(sidPrefix, StringComparison.OrdinalIgnoreCase))?.Name
                        ?? sid.Transitions.SelectMany(t => t.Waypoints)
                                          .FirstOrDefault(w => w.Name.StartsWith(sidPrefix, StringComparison.OrdinalIgnoreCase))?.Name
                        ?? sid.Name;

                    string firstLatLon = includeCoordinates ? $" //{ConvertToNFormat(sid.Waypoints.First().Latitude)};{ConvertToEFormat(sid.Waypoints.First().Longitude)}" : "";
                    // Format SID output
                    string formattedSid = $"{icaoCode};{sid.Runway};{sid.Name};{matchingWaypointName};{matchingWaypointName};{firstLatLon}";
                    sidOutput += formattedSid + "\n"; // Add SID header on a new line

                    foreach (var waypoint in sid.Waypoints)
                    {
                        if (waypoint.Name.StartsWith("(")) // Skip waypoints with names starting with '('
                            continue;

                        // Get formatted altitude using the FormatAltitudeWithConstraints method
                        string altitudeFormat = includeAltitude ? FormatAltitudeWithConstraints(waypoint.Altitude, waypoint.AltitudeCons, waypoint.AltitudeRestriction) : "";
                        string speedFormat = includeSpeed && waypoint.Speed > 0 ? $"{waypoint.Speed}kt" : "";
                        string latLonComment = includeCoordinates ? $" //{ConvertToNFormat(waypoint.Latitude)};{ConvertToEFormat(waypoint.Longitude)}" : "";

                        // Constructing the output format based on conditions with comment for latitude and longitude
                        if (string.IsNullOrEmpty(altitudeFormat) && string.IsNullOrEmpty(speedFormat))
                        {
                            sidOutput += $"{waypoint.Name};{waypoint.Name};{latLonComment}\n";
                        }
                        else if (!string.IsNullOrEmpty(altitudeFormat) && !string.IsNullOrEmpty(speedFormat))
                        {
                            sidOutput += $"{waypoint.Name};{waypoint.Name};{altitudeFormat} | {speedFormat};{latLonComment}\n";
                        }
                        else if (!string.IsNullOrEmpty(altitudeFormat))
                        {
                            sidOutput += $"{waypoint.Name};{waypoint.Name};{altitudeFormat};{latLonComment}\n";
                        }
                        else if (!string.IsNullOrEmpty(speedFormat))
                        {
                            sidOutput += $"{waypoint.Name};{waypoint.Name};{speedFormat};{latLonComment}\n";
                        }
                    }

                    sidOutput += "\n";

                    if (sid.Transitions != null && sid.Transitions.Any())
                    {
                        sidOutput += "// TRANSITION START\n";

                        foreach (var transition in sid.Transitions)
                        {
                            string transitionSid = $"{icaoCode};{sid.Runway};{transition.Name};{transition.Name};{transition.Name};1;";
                            sidOutput += transitionSid + "\n";

                            // Insert the matching waypoint name at the start of each transition
                            sidOutput += $"{matchingWaypointName};{matchingWaypointName};\n";
                            foreach (var waypoint in transition.Waypoints)
                            {
                                string transitionAltitudeFormat = includeAltitude ? FormatAltitudeWithConstraints(waypoint.Altitude, waypoint.AltitudeCons, waypoint.AltitudeRestriction) : "";
                                string transitionSpeedFormat = includeSpeed && waypoint.Speed > 0 ? $"{waypoint.Speed}kt" : "";
                                string transitionLatLonComment = includeCoordinates ? $" //{ConvertToNFormat(waypoint.Latitude)};{ConvertToEFormat(waypoint.Longitude)}" : "";

                                // Constructing the output format based on conditions with comment for latitude and longitude
                                if (string.IsNullOrEmpty(transitionAltitudeFormat) && string.IsNullOrEmpty(transitionSpeedFormat))
                                {
                                    sidOutput += $"{waypoint.Name};{waypoint.Name};{transitionLatLonComment}\n";
                                }
                                else if (!string.IsNullOrEmpty(transitionAltitudeFormat) && !string.IsNullOrEmpty(transitionSpeedFormat))
                                {
                                    sidOutput += $"{waypoint.Name};{waypoint.Name};{transitionAltitudeFormat} | {transitionSpeedFormat};{transitionLatLonComment}\n";
                                }
                                else if (!string.IsNullOrEmpty(transitionAltitudeFormat))
                                {
                                    sidOutput += $"{waypoint.Name};{waypoint.Name};{transitionAltitudeFormat};{transitionLatLonComment}\n";
                                }
                                else if (!string.IsNullOrEmpty(transitionSpeedFormat))
                                {
                                    sidOutput += $"{waypoint.Name};{waypoint.Name};{transitionSpeedFormat};{transitionLatLonComment}\n";
                                }
                            }

                            sidOutput += $"// TRANSITION END {transition.Name}\n";
                        }

                        sidOutput += "// TRANSITION END\n";
                    }

                    sidOutput += "// SID ended\n\n";
                }

                // logRichTextBox.AppendText(sidOutput);
            }
            catch (Exception ex)
            {
                logRichTextBox.AppendText($"Error parsing SID data: {ex.Message}\n");
            }
        }

        // Convert latitude to the desired DMS format with milliseconds
        private string ConvertToNFormat(double latitude)
        {
            string hemisphere = latitude >= 0 ? "N" : "S";
            latitude = Math.Abs(latitude);
            int degrees = (int)Math.Floor(latitude);
            double minutesDecimal = (latitude - degrees) * 60;
            int minutes = (int)Math.Floor(minutesDecimal);
            double secondsDecimal = (minutesDecimal - minutes) * 60;
            int seconds = (int)Math.Floor(secondsDecimal);
            int milliseconds = (int)((secondsDecimal - seconds) * 1000) % 1000; // Ensure exactly 3 digits for milliseconds
            return $"{hemisphere}{degrees:00}.{minutes:00}.{seconds:00}.{milliseconds:000}";
        }

        // Convert longitude to the desired DMS format with milliseconds
        private string ConvertToEFormat(double longitude)
        {
            string hemisphere = longitude >= 0 ? "E" : "W";
            longitude = Math.Abs(longitude);
            int degrees = (int)Math.Floor(longitude);
            double minutesDecimal = (longitude - degrees) * 60;
            int minutes = (int)Math.Floor(minutesDecimal);
            double secondsDecimal = (minutesDecimal - minutes) * 60;
            int seconds = (int)Math.Floor(secondsDecimal);
            int milliseconds = (int)((secondsDecimal - seconds) * 1000) % 1000; // Ensure exactly 3 digits for milliseconds
            return $"{hemisphere}{degrees:00}.{minutes:00}.{seconds:00}.{milliseconds:000}";
        }

        private void FormatStarData(string xmlContent, string icaoCode)
        {
            starOutput = ""; // Reset the STAR output

            try
            {
                XDocument doc = XDocument.Parse(xmlContent);

                if (doc == null)
                {
                    MessageBox.Show($"Failed to load STAR data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                XNamespace ns = "http://tempuri.org/XMLSchema.xsd";

                var airport = doc.Descendants(ns + "Airport")
                                  .FirstOrDefault(a => a.Attribute("ICAOcode").Value.Equals(icaoCode, StringComparison.OrdinalIgnoreCase));

                if (airport == null)
                {
                    MessageBox.Show($"No airport found for '{icaoCode}'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool includeAltitude = MessageBox.Show("Do you want altitude in the STAR file?", "Altitude Option for STAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                bool includeSpeed = MessageBox.Show("Do you want speed in the STAR file?", "Speed Option for STAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                bool includeCoordinates = MessageBox.Show("Do you want coordinates in the STAR file?", "Coordinate Option for STAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

                var stars = airport.Descendants(ns + "Star")
                                    .Select(s => new
                                    {
                                        Runway = s.Attribute("Runways").Value.Replace(",", ":"), // Replace commas with colons
                                        Name = s.Attribute("Name").Value,
                                        Waypoints = s.Elements(ns + "Star_Waypoint")
                                                    .Select(w => new
                                                    {
                                                        Name = (string)w.Element(ns + "Name"),
                                                        Altitude = (int)w.Element(ns + "Altitude"),
                                                        AltitudeCons = (int?)w.Element(ns + "AltitudeCons") ?? 0,
                                                        AltitudeRestriction = (string)w.Element(ns + "AltitudeRestriction"),
                                                        Speed = (int?)w.Element(ns + "Speed") ?? 0,
                                                        Latitude = (double?)w.Element(ns + "Latitude") ?? 0,
                                                        Longitude = (double?)w.Element(ns + "Longitude") ?? 0
                                                    }).ToList(),
                                        Transitions = s.Elements(ns + "Star_Transition")
                                                       .Select(t => new
                                                       {
                                                           Name = (string)t.Attribute("Name"),
                                                           Waypoints = t.Elements(ns + "StarTr_Waypoint")
                                                                       .Select(w => new
                                                                       {
                                                                           Name = (string)w.Element(ns + "Name"),
                                                                           Altitude = (int)w.Element(ns + "Altitude"),
                                                                           AltitudeCons = (int?)w.Element(ns + "AltitudeCons") ?? 0,
                                                                           AltitudeRestriction = (string)w.Element(ns + "AltitudeRestriction"),
                                                                           Speed = (int?)w.Element(ns + "Speed") ?? 0,
                                                                           Latitude = (double?)w.Element(ns + "Latitude") ?? 0,
                                                                           Longitude = (double?)w.Element(ns + "Longitude") ?? 0
                                                                       }).ToList()
                                                       }).ToList()
                                    }).ToList();

                if (!stars.Any())
                {
                    AppendToDebugTextBox($"\nNo STARS for airport: {icaoCode}");
                    return;
                }

                foreach (var star in stars)
                {
                    // Start of STAR section
                    starOutput += "// STAR Start\n";

                    string starPrefix = star.Name.Length >= 6 ? star.Name.Substring(0, 4) :
                                        star.Name.Length > 0 && star.Name.Length <= 5 ? star.Name.Substring(0, 3) : star.Name;

                    string matchingWaypointName = star.Waypoints
                        .FirstOrDefault(w => w.Name.StartsWith(starPrefix, StringComparison.OrdinalIgnoreCase))?.Name
                        ?? star.Transitions.SelectMany(t => t.Waypoints)
                                           .FirstOrDefault(w => w.Name.StartsWith(starPrefix, StringComparison.OrdinalIgnoreCase))?.Name
                        ?? star.Name;

                    // Format STAR output
                    string firstLatLon = includeCoordinates ? $" //{ConvertToNFormat(star.Waypoints.First().Latitude)};{ConvertToEFormat(star.Waypoints.First().Longitude)}" : "";
                    string formattedStar = $"{icaoCode};{star.Runway};{star.Name};{matchingWaypointName};{matchingWaypointName};{firstLatLon}";
                    starOutput += formattedStar + "\n"; // Add STAR header on a new line

                    foreach (var waypoint in star.Waypoints)
                    {
                        if (waypoint.Name.StartsWith("(")) // Skip waypoints with names starting with '('
                            continue;

                        string formattedAltitude = includeAltitude ? FormatAltitudeWithConstraints(waypoint.Altitude, waypoint.AltitudeCons, waypoint.AltitudeRestriction) : "";
                        string speedFormat = includeSpeed && waypoint.Speed > 0 ? $"{waypoint.Speed}kt" : "";
                        string latLonComment = includeCoordinates ? $" //{ConvertToNFormat(waypoint.Latitude)};{ConvertToEFormat(waypoint.Longitude)}" : "";

                        // Constructing the output format based on conditions
                        if (string.IsNullOrEmpty(formattedAltitude) && string.IsNullOrEmpty(speedFormat))
                        {
                            starOutput += $"{waypoint.Name};{waypoint.Name};{latLonComment}\n"; // Only name with one semicolon
                        }
                        else if (!string.IsNullOrEmpty(formattedAltitude) && !string.IsNullOrEmpty(speedFormat))
                        {
                            starOutput += $"{waypoint.Name};{waypoint.Name};{formattedAltitude} | {speedFormat};{latLonComment}\n"; // Both altitude and speed
                        }
                        else if (!string.IsNullOrEmpty(formattedAltitude))
                        {
                            starOutput += $"{waypoint.Name};{waypoint.Name};{formattedAltitude};{latLonComment}\n"; // Only altitude
                        }
                        else if (!string.IsNullOrEmpty(speedFormat))
                        {
                            starOutput += $"{waypoint.Name};{waypoint.Name};{speedFormat};{latLonComment}\n"; // Only speed
                        }
                    }

                    starOutput += "\n";

                    if (star.Transitions != null && star.Transitions.Any())
                    {
                        starOutput += "// TRANSITION START\n";

                        foreach (var transition in star.Transitions)
                        {
                            string transitionStar = $"{icaoCode};{star.Runway};{transition.Name};{transition.Name};{transition.Name};1;";
                            starOutput += transitionStar + "\n";

                            starOutput += $"{matchingWaypointName};{matchingWaypointName};\n";

                            foreach (var waypoint in transition.Waypoints)
                            {
                                string transitionAltitudeFormat = includeAltitude ? FormatAltitudeWithConstraints(waypoint.Altitude, waypoint.AltitudeCons, waypoint.AltitudeRestriction) : "";
                                string transitionSpeedFormat = includeSpeed && waypoint.Speed > 0 ? $"{waypoint.Speed}kt" : "";
                                string transitionLatLonComment = includeCoordinates ? $" //{ConvertToNFormat(waypoint.Latitude)};{ConvertToEFormat(waypoint.Longitude)}" : "";

                                // Constructing the output format based on conditions
                                if (string.IsNullOrEmpty(transitionAltitudeFormat) && string.IsNullOrEmpty(transitionSpeedFormat))
                                {
                                    starOutput += $"{waypoint.Name};{waypoint.Name};{transitionLatLonComment}\n"; // Only name with one semicolon
                                }
                                else if (!string.IsNullOrEmpty(transitionAltitudeFormat) && !string.IsNullOrEmpty(transitionSpeedFormat))
                                {
                                    starOutput += $"{waypoint.Name};{waypoint.Name};{transitionAltitudeFormat} | {transitionSpeedFormat};{transitionLatLonComment}\n"; // Both altitude and speed
                                }
                                else if (!string.IsNullOrEmpty(transitionAltitudeFormat))
                                {
                                    starOutput += $"{waypoint.Name};{waypoint.Name};{transitionAltitudeFormat};{transitionLatLonComment}\n"; // Only altitude
                                }
                                else if (!string.IsNullOrEmpty(transitionSpeedFormat))
                                {
                                    starOutput += $"{waypoint.Name};{waypoint.Name};{transitionSpeedFormat};{transitionLatLonComment}\n"; // Only speed
                                }
                            }

                            starOutput += $"// TRANSITION END {transition.Name}\n";
                        }

                        starOutput += "// TRANSITION END\n";
                    }

                    starOutput += "// STAR ended\n\n";
                }

                // logRichTextBox.AppendText(starOutput);
            }
            catch (Exception ex)
            {
                logRichTextBox.AppendText($"Error parsing STAR data: {ex.Message}\n");
            }
        }



        private string FormatAltitudeWithConstraints(int altitude, int altitudeCons, string restriction)
        {
            // Check if both altitudes are zero
            if (altitude == 0 && altitudeCons == 0)
            {
                return string.Empty; // Return an empty string if both altitudes are zero
            }

            string altitudeStr = altitude > 4000 ? $"FL{altitude / 100}" : altitude.ToString();
            string altitudeConsStr = altitudeCons > 4000 ? $"FL{altitudeCons / 100}" : altitudeCons.ToString();

            if (altitudeCons == 0)
            {
                if (restriction.Equals("at", StringComparison.OrdinalIgnoreCase))
                {
                    return altitude > 0 ? $"={altitudeStr}" : $"={altitude}";
                }
                else if (restriction.Equals("below", StringComparison.OrdinalIgnoreCase))
                {
                    return altitude > 4000 ? $"-{altitudeStr}" : $"-{altitude}";
                }
                else if (restriction.Equals("above", StringComparison.OrdinalIgnoreCase))
                {
                    return altitude > 4000 ? $"+{altitudeStr}" : $"+{altitude}";
                }
            }
            else
            {
                if (restriction.Equals("above", StringComparison.OrdinalIgnoreCase))
                {
                    return $"-{altitudeStr}/+{altitudeConsStr}";
                }
                else if (restriction.Equals("below", StringComparison.OrdinalIgnoreCase))
                {
                    return $"+{altitudeStr}/-{altitudeConsStr}";
                }
            }

            return string.Empty; // Return an empty string if conditions are not met
        }


        private void DownloadSidButton_Click(object sender, EventArgs e)
        {
            SaveToFile(sidOutput, "SID Files (*.SID)|*.SID", "Save SID Data", "SID");
        }

        private void DownloadStarButton_Click(object sender, EventArgs e)
        {
            SaveToFile(starOutput, "STAR Files (*.STR)|*.STR", "Save STAR Data", "STR");
        }

        private void SaveToFile(string outputData, string filter, string title, string fileType)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                string icaoCode = searchBox.Text.Trim().ToUpper();
                saveFileDialog.Filter = filter;
                saveFileDialog.Title = $"{title} for {icaoCode}";
                saveFileDialog.FileName = $"{icaoCode}.{fileType}";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(saveFileDialog.FileName, outputData);
                        MessageBox.Show($"{fileType} data saved to {saveFileDialog.FileName}", "Save Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        logRichTextBox.AppendText($"Error saving file: {ex.Message}\n");
                    }
                }
            }
        }

        private void AppendToDebugTextBox(string message, bool isBold = false, Color? textColor = null, bool isHyperlink = false, string hyperlinkUrl = null)
        {
            if (logRichTextBox.InvokeRequired)
            {
                logRichTextBox.Invoke((MethodInvoker)delegate {
                    AppendFormattedText(message, isBold, textColor, isHyperlink, hyperlinkUrl);
                });
            }
            else
            {
                AppendFormattedText(message, isBold, textColor, isHyperlink, hyperlinkUrl);
            }
        }


        private void AppendFormattedText(string message, bool isBold, Color? textColor, bool isHyperlink, string hyperlinkUrl)
        {
            Color colorToUse = textColor ?? Color.Black;

            int start = logRichTextBox.TextLength;
            logRichTextBox.AppendText(message);  // Append the message

            logRichTextBox.Select(start, message.Length);

            // Apply bold formatting if requested
            logRichTextBox.SelectionFont = isBold ? new Font(logRichTextBox.Font, FontStyle.Bold) : new Font(logRichTextBox.Font, FontStyle.Regular);

            // Apply hyperlink formatting if requested
            if (isHyperlink)
            {
                logRichTextBox.SelectionColor = Color.Blue;
                logRichTextBox.SelectionFont = new Font(logRichTextBox.Font, FontStyle.Underline);
                // Set the hyperlink URL in the Tag for later use
                logRichTextBox.Tag = hyperlinkUrl;  // Optional, if you want to store the hyperlink URL for later use
            }
            else
            {
                // Set text color
                logRichTextBox.SelectionColor = colorToUse;
            }

            logRichTextBox.Select(logRichTextBox.TextLength, 0);
            logRichTextBox.ScrollToCaret();
        }

        // LinkClicked event handler
        private void debugTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            // Open the hyperlink when clicked
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            // Code to go back to the main menu
            this.Hide();
        }
    }
}