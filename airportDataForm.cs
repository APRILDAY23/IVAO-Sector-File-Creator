using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sector_File
{
    public partial class airportDataForm : Form
    {
        private int userIdInput;
        private string runwayOutput;
        private string airportDataOutput; // Variable to store the fetched airport data
        private string frequencyOutput; // Variable to store the fetched airport data
        private string vordmeOutput;
        private string ndbOutput;
        private string gateOutput;


        public airportDataForm(int userId)
        {
            userIdInput = userId;
            this.Icon = new Icon("./tools.ico");
            InitializeComponent();

            // Bind the Search button click event to the getAirportDataFIR function
            searchButton.Click += searchButton_Click;
            downloadAirportData.Click += DownloadAirportDataFile;
            downloadRunwayData.Click += DownloadRunwayDataFile;
            airportFrequencyData.Click += DownloadFrequencyDataFile;
            downloadVORDMEData.Click += DownloadVORDMEDataFile;
            downloadNDBData.Click += DownloadNDBDataFile;
            downloadGateData.Click += DownloadGateDataFile;
            backButton.Click += backButton_Click;

        }

        private async void searchButton_Click(object sender, EventArgs e)
        {
            string searchTerm = searchBox.Text.Trim().ToUpper();

            AppendToDebugTextBox("Sector File Conversion made by ");
            AppendToDebugTextBox("Veda Moola (656077)", isHyperlink: true, hyperlinkUrl: "https://ivao.aero/Member.aspx?Id=656077");
            AppendToDebugTextBox("\nFully tested by ");
            AppendToDebugTextBox("Nilay Parsodkar (709833)", isHyperlink: true, hyperlinkUrl: "https://ivao.aero/Member.aspx?Id=709833");
            AppendToDebugTextBox(" Please blame him for any bugs. Haha!. \nBut if you face any problems please dm me or raise an issue on Github repo");
            AppendToDebugTextBox("\nAirport data provided by ");
            AppendToDebugTextBox("Jordan Kirkby (646483)", isHyperlink: true, hyperlinkUrl: "https://ivao.aero/Member.aspx?Id=709833");
            AppendToDebugTextBox("\nNOT FOR REAL WORLD USE", isBold: true, textColor: Color.Red);
            AppendToDebugTextBox($"\nFetching Airport/Runway/VOR/DME/NDB/Frequencies/Gates data for: {searchTerm}XX FIR");
            UpdateProgress(0); // Assuming the SID processing is 25% of the total work

            if (string.IsNullOrEmpty(searchTerm))
            {
                MessageBox.Show("Please enter a 2-letter FIR ICAO code.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                await getAirportDataFIR(searchTerm);
                UpdateProgress(16); // 16.67% progress after Airport Data

                await getRunwayDataFIR(searchTerm);
                UpdateProgress(33); // 33.33% progress after Runway Data

                await getFrequencyDataFIR(searchTerm);
                UpdateProgress(50); // 50% progress after Frequency Data

                await getVORDMEDataFIR(searchTerm);
                UpdateProgress(67); // 66.67% progress after VOR/DME Data

                await GetNDBData(searchTerm);
                UpdateProgress(83); // 83.33% progress after NDB Data

                await GetGateData(searchTerm);
                UpdateProgress(100); // 100% progress after Gate Data

                stopwatch.Stop();
                string timeMessage = stopwatch.Elapsed.TotalSeconds > 1
                    ? $"\nPROCESS COMPLETED in {stopwatch.Elapsed.TotalSeconds:F2} seconds"
                    : $"\nPROCESS COMPLETED in {stopwatch.Elapsed.TotalMilliseconds:F2} ms";

                AppendToDebugTextBox(timeMessage, isBold: true, textColor: Color.Green);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task getAirportDataFIR(string searchTerm)
        {

            string url = "https://portal.in.ivao.aero/core/nav_data_2309019312309/data/nav_jsondata_2309019312309/tbl_airports.json";

            try
            {
                using HttpClient client = new HttpClient();
                string json = await client.GetStringAsync(url);

                var airportDataList = JsonSerializer.Deserialize<List<Airport>>(json);

                if (airportDataList == null)
                {
                    MessageBox.Show("No airport data found.");
                    return;
                }

                List<string> formattedData = new List<string>();

                foreach (var airport in airportDataList)
                {
                    if (airport.icao_code == searchTerm)
                    {
                        string formattedEntry = FormatAirportDataOutput(airport);
                        formattedData.Add(formattedEntry);
                    }
                }

                airportDataOutput = string.Join(Environment.NewLine, formattedData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching data: {ex.Message}");
            }
        }

        private string FormatAirportDataOutput(Airport airport)
        {
            string latitudeFormatted = ConvertToNFormat(airport.airport_ref_latitude);
            string longitudeFormatted = ConvertToEFormat(airport.airport_ref_longitude);

            return $"{airport.airport_identifier};{airport.elevation};{latitudeFormatted};{longitudeFormatted};{airport.airport_name};";
        }

        private async Task getRunwayDataFIR(string searchTerm)
        {
            string url = "https://portal.in.ivao.aero/core/nav_data_2309019312309/data/nav_jsondata_2309019312309/tbl_runways.json";

            try
            {
                using HttpClient client = new HttpClient();
                string json = await client.GetStringAsync(url);

                var runwayDataList = JsonSerializer.Deserialize<List<Runway>>(json);

                if (runwayDataList == null)
                {
                    MessageBox.Show("No runway data found.");
                    return;
                }

                HashSet<string> addedRunways = new HashSet<string>();  // To track already processed runways
                List<string> formattedData = new List<string>();

                // Group runways by airport identifier
                var airportGroups = runwayDataList
                    .Where(r => r.icao_code == searchTerm)
                    .GroupBy(r => r.airport_identifier);

                foreach (var group in airportGroups)
                {
                    foreach (var runway in group)
                    {
                        string runwayIdentifier = TrimRunwayIdentifier(runway.runway_identifier);
                        string oppositeRunwayIdentifier = GetOppositeRunway(runwayIdentifier);

                        // Find the opposite runway within the same airport group
                        var oppositeRunway = group.FirstOrDefault(r => TrimRunwayIdentifier(r.runway_identifier) == oppositeRunwayIdentifier);

                        // Check if this runway pair has already been processed
                        string runwayPairKey = $"{runway.airport_identifier};{runwayIdentifier}/{oppositeRunwayIdentifier}";
                        if (!addedRunways.Contains(runwayPairKey))
                        {
                            // Format the runway data and add it to the result list
                            string formattedEntry = FormatRunwayData(runway, oppositeRunway);
                            formattedData.Add(formattedEntry);

                            // Mark both runway directions as processed
                            addedRunways.Add(runwayPairKey);
                            addedRunways.Add($"{runway.airport_identifier};{oppositeRunwayIdentifier}/{runwayIdentifier}");
                        }
                    }
                }

                runwayOutput = string.Join(Environment.NewLine, formattedData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching runway data: {ex.Message}");
            }
        }

        private string GetOppositeRunway(string runwayIdentifier)
        {
            int runwayNumber = int.Parse(runwayIdentifier.Substring(0, 2)); // Parse the numeric part
            int oppositeNumber = (runwayNumber + 18) % 36; // Calculate opposite runway number
            return oppositeNumber.ToString("00") + (runwayIdentifier.Length > 2 ? runwayIdentifier[2].ToString() : "");
        }

        private string FormatRunwayData(Runway runway, Runway oppositeRunway)
        {
            // Trim runway identifiers
            string runwayIdentifier = TrimRunwayIdentifier(runway.runway_identifier);
            string oppositeRunwayIdentifier = TrimRunwayIdentifier(oppositeRunway?.runway_identifier ?? runway.runway_identifier);

            // Format latitude and longitude
            string runwayLat = ConvertToNFormat(runway.runway_latitude);
            string runwayLon = ConvertToEFormat(runway.runway_longitude);
            string oppositeRunwayLat = oppositeRunway != null ? ConvertToNFormat(oppositeRunway.runway_latitude) : runwayLat;
            string oppositeRunwayLon = oppositeRunway != null ? ConvertToEFormat(oppositeRunway.runway_longitude) : runwayLon;

            return $"{runway.airport_identifier};{runwayIdentifier};{oppositeRunwayIdentifier};" +
                   $"{runway.runway_magnetic_bearing:F0};{oppositeRunway?.runway_magnetic_bearing:F0};" +
                   $"{runwayLat};{runwayLon};{oppositeRunwayLat};{oppositeRunwayLon};";
        }

        private string TrimRunwayIdentifier(string runwayIdentifier)
        {
            // Remove "RW" prefix and keep direction suffix (L, C, R) if present
            string identifier = runwayIdentifier.Substring(2);
            return identifier.Length > 2 && "LCR".Contains(identifier[^1]) ? identifier : identifier.Substring(0, 2);
        }

        private async Task getFrequencyDataFIR(string searchTerm)
        {
            string url = "https://portal.in.ivao.aero/core/nav_data_2309019312309/data/nav_jsondata_2309019312309/tbl_airport_communication.json";

            try
            {
                using HttpClient client = new HttpClient();
                string json = await client.GetStringAsync(url);

                // Assuming the frequency data is returned as a list of 'Frequency' objects
                var frequencyDataList = JsonSerializer.Deserialize<List<Frequency>>(json);

                if (frequencyDataList == null)
                {
                    MessageBox.Show("No frequency data found.");
                    return;
                }

                HashSet<string> addedFrequency = new HashSet<string>();  // To track already processed frequencies
                List<string> formattedData = new List<string>();

                foreach (var group in frequencyDataList.Where(f => f.icao_code == searchTerm).GroupBy(f => f.airport_identifier))
                {
                    foreach (var frequency in group)
                    {
                        string formattedEntry = FormatFrequencyData(frequency);

                        // Check if the frequency data is already processed to avoid duplication
                        string frequencyPair = $"{frequency.airport_identifier}_{frequency.communication_type}";
                        if (!addedFrequency.Contains(frequencyPair))
                        {
                            formattedData.Add(formattedEntry);

                            // Mark the frequency as processed
                            addedFrequency.Add(frequencyPair);
                        }
                    }
                }

                frequencyOutput = string.Join(Environment.NewLine, formattedData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching frequency data: {ex.Message}");
            }
        }

        private string FormatFrequencyData(Frequency frequency)
        {
            // Check if the communication_type ends with "_ATI" and append "S" after it
            string communicationType = frequency.communication_type;
            if (communicationType != null && communicationType.EndsWith("ATI") && !communicationType.EndsWith("ATIS"))
            {
                communicationType = communicationType.Replace("ATI", "ATIS");  // Replace "_ATI" with "_ATIS"
            }

            // Format the communication_frequency to always have 3 decimal places
            string formattedFrequency = frequency.communication_frequency.ToString("F3");

            // Format the output as requested: airport_identifier_communication_type;communication_frequency
            return $"{frequency.airport_identifier}_{communicationType};{formattedFrequency};";
        }

        private async Task getVORDMEDataFIR(string searchTerm)
        {
            string url = "https://portal.in.ivao.aero/core/nav_data_2309019312309/data/nav_jsondata_2309019312309/tbl_vhfnavaids.json"; // Update URL to the correct VORDME data source

            try
            {
                using HttpClient client = new HttpClient();
                string json = await client.GetStringAsync(url);

                // Assuming the VORDME data is returned as a list of 'VORDME' objects
                var vordmeDataList = JsonSerializer.Deserialize<List<VORDME>>(json);

                if (vordmeDataList == null)
                {
                    MessageBox.Show("No VORDME data found.");
                    return;
                }

                HashSet<string> addedVORDME = new HashSet<string>();  // To track already processed VORDME entries
                List<string> formattedData = new List<string>();

                foreach (var group in vordmeDataList.Where(v => v.icao_code == searchTerm).GroupBy(v => v.airport_identifier))
                {
                    foreach (var vordme in group)
                    {
                        string formattedEntry = FormatVORDMEData(vordme);

                        // Check if the VORDME entry is already processed to avoid duplication
                        string vordmePair = $"{vordme.airport_identifier}_{vordme.vor_identifier}";
                        if (!addedVORDME.Contains(vordmePair))
                        {
                            formattedData.Add(formattedEntry);

                            // Mark the VORDME entry as processed
                            addedVORDME.Add(vordmePair);
                        }
                    }
                }

                vordmeOutput = string.Join(Environment.NewLine, formattedData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching VORDME data: {ex.Message}");
            }
        }

        private string FormatVORDMEData(VORDME vordme)
        {
            // Determine VORDME type based on presence of dme_ident and vor_identifier
            int vorType = 0;  // Default type VOR
            if (!string.IsNullOrEmpty(vordme.dme_ident) && !string.IsNullOrEmpty(vordme.vor_identifier))
            {
                vorType = 1;  // VORDME
            }
            else if (!string.IsNullOrEmpty(vordme.dme_ident) && string.IsNullOrEmpty(vordme.vor_identifier))
            {
                vorType = 4;  // DME
            }

            // Format the VOR frequency to 2 decimal places
            string formattedVORFrequency = vordme.vor_frequency.ToString("F3");

            // Convert the latitude and longitude to N/S and E/W format
            string formattedVORLat = ConvertToNFormat(vordme.vor_latitude);
            string formattedVORLon = ConvertToEFormat(vordme.vor_longitude);

            // Determine station identifier (either dme_ident or vor_identifier)
            string stationIdentifier = string.IsNullOrEmpty(vordme.dme_ident) ? vordme.vor_identifier : vordme.dme_ident;

            // Return the formatted VORDME data string
            return $"{stationIdentifier};{formattedVORFrequency};{formattedVORLat};{formattedVORLon};1;{vorType};";
        }

        private async Task GetNDBData(string searchTerm)
        {
            string url = "https://portal.in.ivao.aero/core/nav_data_2309019312309/data/nav_jsondata_2309019312309/tbl_enroute_ndbnavaids.json";

            try
            {
                using HttpClient client = new HttpClient();
                string json = await client.GetStringAsync(url);

                // Assuming the NDB data is returned as a list of 'NDB' objects
                var ndbDataList = JsonSerializer.Deserialize<List<NDB>>(json);

                if (ndbDataList == null)
                {
                    MessageBox.Show("No NDB data found.");
                    return;
                }

                List<string> formattedData = new List<string>();

                foreach (var ndb in ndbDataList.Where(n => n.icao_code == searchTerm))
                {
                    string formattedEntry = FormatNDBData(ndb);
                    formattedData.Add(formattedEntry);
                }

                ndbOutput = string.Join(Environment.NewLine, formattedData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching NDB data: {ex.Message}");
            }
        }

        private string FormatNDBData(NDB ndb)
        {
            // Format the frequency with 1 decimal place
            string formattedFrequency = ndb.ndb_frequency.ToString("F1", CultureInfo.InvariantCulture);

            // Convert latitude and longitude to N/S and E/W format
            string formattedLatitude = ConvertToNFormat(ndb.ndb_latitude);
            string formattedLongitude = ConvertToEFormat(ndb.ndb_longitude);

            // Format the output as expected: NDB_IDENTIFIER;FREQ;LATITUDE;LONGITUDE
            return $"{ndb.ndb_identifier};{formattedFrequency};{formattedLatitude};{formattedLongitude};";
        }

        private async Task GetGateData(string searchTerm)
        {
            string url = "https://portal.in.ivao.aero/core/nav_data_2309019312309/data/nav_jsondata_2309019312309/tbl_gate.json";

            try
            {
                using HttpClient client = new HttpClient();
                string json = await client.GetStringAsync(url);

                // Assuming the gate data is returned as a list of 'Gate' objects
                var gateDataList = JsonSerializer.Deserialize<List<Gate>>(json);

                if (gateDataList == null)
                {
                    MessageBox.Show("No gate data found.");
                    return;
                }

                // Filter by search term and order by airport_identifier
                var filteredAndOrderedGates = gateDataList
                    .Where(g => g.icao_code == searchTerm)
                    .OrderBy(g => g.airport_identifier);

                List<string> formattedData = new List<string>();

                foreach (var gate in filteredAndOrderedGates)
                {
                    string formattedEntry = FormatGateData(gate);
                    formattedData.Add(formattedEntry);
                }

                gateOutput = string.Join(Environment.NewLine, formattedData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching gate data: {ex.Message}");
            }
        }

        private string FormatGateData(Gate gate)
        {
            // Convert latitude and longitude to N/S and E/W format
            string formattedLatitude = ConvertToNFormat(gate.gate_latitude);
            string formattedLongitude = ConvertToEFormat(gate.gate_longitude);

            // Format the output as expected: GATE_IDENTIFIER;AIRPORT_IDENTIFIER;LATITUDE;LONGITUDE
            return $"{gate.gate_identifier};{gate.airport_identifier};{formattedLatitude};{formattedLongitude};";
        }


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


        private void DownloadAirportDataFile(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(airportDataOutput))
            {
                MessageBox.Show("No data to download. Please search for a FIR first.");
                return;
            }

            string searchTerm = searchBox.Text.Trim().ToUpper();
            string defaultFileName = $"{searchTerm}_AP.ap";

            using SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = defaultFileName,
                Filter = "Airport Data Files (*.ap)|*.ap|All Files (*.*)|*.*",
                Title = "Save Airport Data File"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, airportDataOutput);
                MessageBox.Show($"Airport Data file has been saved to {saveFileDialog.FileName} successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DownloadRunwayDataFile(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(runwayOutput))
            {
                MessageBox.Show("No data to download. Please search for a FIR first.");
                return;
            }

            string searchTerm = searchBox.Text.Trim().ToUpper();
            string defaultFileName = $"{searchTerm}_RW.rw";  // Using SEARCHTERM_RW.rw for filename

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.FileName = defaultFileName;
                saveFileDialog.Filter = "Runway Data Files (*.rw)|*.rw|All Files (*.*)|*.*";
                saveFileDialog.Title = "Save Runway Data File";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, runwayOutput);  // Saving runway data output to file
                    MessageBox.Show($"Runway Data file has been saved to {saveFileDialog.FileName} successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void DownloadFrequencyDataFile(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(frequencyOutput))
            {
                MessageBox.Show("No data to download. Please search for a FIR first.");
                return;
            }

            string searchTerm = searchBox.Text.Trim().ToUpper();
            string defaultFileName = $"{searchTerm}.atc";  // Using SEARCHTERM_RW.rw for filename

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.FileName = defaultFileName;
                saveFileDialog.Filter = "Frequency Data Files (*.atc)|*.atc|All Files (*.*)|*.*";
                saveFileDialog.Title = "Save Frequency Data File";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, frequencyOutput);  // Saving runway data output to file
                    MessageBox.Show($"Frequency Data file has been saved to {saveFileDialog.FileName} successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void DownloadVORDMEDataFile(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(vordmeOutput))
            {
                MessageBox.Show("No data to download. Please search for a FIR first.");
                return;
            }

            string searchTerm = searchBox.Text.Trim().ToUpper();
            string defaultFileName = $"{searchTerm}.vor";  // Using SEARCHTERM_RW.rw for filename

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.FileName = defaultFileName;
                saveFileDialog.Filter = "VOR/DME Data Files (*.vor)|*.vor|All Files (*.*)|*.*";
                saveFileDialog.Title = "Save VOR/DME Data File";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, vordmeOutput);  // Saving runway data output to file
                    MessageBox.Show($"VOR/DME Data file has been saved to {saveFileDialog.FileName} successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void DownloadNDBDataFile(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ndbOutput))
            {
                MessageBox.Show("No data to download. Please search for a FIR first.");
                return;
            }

            string searchTerm = searchBox.Text.Trim().ToUpper();
            string defaultFileName = $"{searchTerm}.ndb";  // Using SEARCHTERM_RW.rw for filename

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.FileName = defaultFileName;
                saveFileDialog.Filter = "NDB Data Files (*.ndb)|*.ndb|All Files (*.*)|*.*";
                saveFileDialog.Title = "Save NDB Data File";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, ndbOutput);  // Saving runway data output to file
                    MessageBox.Show($"NDB Data file has been saved to {saveFileDialog.FileName} successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void DownloadGateDataFile(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(gateOutput))
            {
                MessageBox.Show("No data to download. Please search for a FIR first.");
                return;
            }

            string searchTerm = searchBox.Text.Trim().ToUpper();
            string defaultFileName = $"{searchTerm}.gts";  // Using SEARCHTERM_RW.rw for filename

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.FileName = defaultFileName;
                saveFileDialog.Filter = "Gate Data Files (*.gts)|*.gts|All Files (*.*)|*.*";
                saveFileDialog.Title = "Save Gate Data File";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, gateOutput);  // Saving runway data output to file
                    MessageBox.Show($"Gate Data file has been saved to {saveFileDialog.FileName} successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void AppendToDebugTextBox(string message, bool isBold = false, Color? textColor = null, bool isHyperlink = false, string hyperlinkUrl = null)
        {
            if (logRichTextBox.InvokeRequired)
            {
                logRichTextBox.Invoke((MethodInvoker)delegate
                {
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

        // Define a class for deserializing airport data
        public class NDB
        {
            public string area_code { get; set; }
            public string icao_code { get; set; }
            public string id { get; set; }
            public string navaid_class { get; set; }
            public double ndb_frequency { get; set; }
            public string ndb_identifier { get; set; }
            public double ndb_latitude { get; set; }
            public double ndb_longitude { get; set; }
            public string ndb_name { get; set; }
            public int range { get; set; }
        }


        public class Airport
        {
            public string airport_identifier { get; set; }
            public string icao_code { get; set; }
            public string airport_name { get; set; }
            public double airport_ref_latitude { get; set; }
            public double airport_ref_longitude { get; set; }
            public int elevation { get; set; }
        }

        public class VORDME
        {
            public string airport_identifier { get; set; }
            public string icao_code { get; set; }
            public string dme_ident { get; set; }
            public string vor_identifier { get; set; }
            public double vor_frequency { get; set; }
            public double vor_latitude { get; set; }
            public double vor_longitude { get; set; }
            public string vor_name { get; set; }
        }


        public class Runway
        {
            public string airport_identifier { get; set; }
            public string icao_code { get; set; }
            public string runway_identifier { get; set; }
            public double runway_latitude { get; set; }
            public double runway_longitude { get; set; }
            public int landing_threshold_elevation { get; set; }
            public double runway_magnetic_bearing { get; set; }
        }

        public class Gate
        {
            public string airport_identifier { get; set; }
            public string area_code { get; set; }
            public string gate_identifier { get; set; }
            public double gate_latitude { get; set; }
            public double gate_longitude { get; set; }
            public string icao_code { get; set; }
            public string name { get; set; }
        }

        private class Frequency
        {
            public string airport_identifier { get; set; }
            public string area_code { get; set; }
            public string callsign { get; set; }
            public double communication_frequency { get; set; }
            public string communication_type { get; set; }
            public string frequency_units { get; set; }
            public string icao_code { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public string service_indicator { get; set; }
        }

        private void UpdateProgress(int value)
        {
            if (loadingProgressBar.InvokeRequired)
            {
                loadingProgressBar.Invoke((MethodInvoker)delegate
                {
                    loadingProgressBar.Value = value;
                });
            }
            else
            {
                loadingProgressBar.Value = value;
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            // Code to go back to the main menu
            this.Hide();
        }
    }
}
