using System;
using System.Diagnostics;
using System.IO; // Required for file handling
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Sector_File
{
    public partial class LoadingForm : Form
    {
        private string kmlFilePath;
        private string tflOutput; // Store the output for TFL file
        private string geoOutput; // Store the output for GEO file (LineStrings)
        private string txiOutput; // Store the output for GEO file (LineStrings)
        private string rwOutput; // Store the output for GEO file (LineStrings)
        private XmlNamespaceManager nsmgr;
        private XmlDocument xmlDoc; // Class-level variable for KML document
        private int userIdInput;

        public LoadingForm(string filePath, int userId)
        {
            kmlFilePath = filePath;
            userIdInput = userId;
            InitializeComponent();
            this.Icon = new Icon("./tools.ico");
            LoadKmlFileAsync(); // Load the file asynchronously
        }

        // Click event handler for the Go Back button
        private void goBackButton_Click(object sender, EventArgs e)
        {
           this.Hide();
        }

        private void UpdateUI(Action action)
        {
            if (InvokeRequired)
            {
                // If called from a different thread, invoke on the UI thread
                this.Invoke(action);
            }
            else
            {
                // Otherwise, execute the action directly
                action();
            }
        }

        private async void LoadKmlFileAsync()
        {
            Stopwatch stopwatch = new Stopwatch(); // Create a stopwatch to measure time
            stopwatch.Start(); // Start timing

            try
            {
                // Reset progress bar
                progressBar.Value = 0; // Set initial value to 0

                // Simulate loading progress
                await Task.Run(() =>
                {
                    for (int i = 0; i < 100; i += 10)
                    {
                        System.Threading.Thread.Sleep(50); // Simulate time-consuming task
                        UpdateProgress(i + 10); // Update progress
                    }
                    ReadKmlFile(kmlFilePath); // Read the KML file after simulating progress
                });

                outputLabel.Text = "KML file loaded successfully from:\n" + kmlFilePath;
                downloadButton.Enabled = true; // Enable the download button
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading KML file: {ex.Message}");
            }
            finally
            {
                stopwatch.Stop(); // Stop timing
                                  // Check if the time is more than 1 second
                string elapsedTime;
                if (stopwatch.Elapsed.TotalSeconds >= 1)
                {
                    // Display in seconds if more than 1 second
                    elapsedTime = $"{stopwatch.Elapsed.TotalSeconds:F2} seconds";
                }
                else
                {
                    // Display in milliseconds if less than 1 second
                    elapsedTime = $"{stopwatch.Elapsed.TotalMilliseconds:F2} ms";
                }

                // Append the completion message
                AppendToDebugTextBox($"\nPROCESS COMPLETED in {elapsedTime}", isBold: true, textColor: Color.Green);// Append the completion message
            }
        }

        private void UpdateProgress(int value)
        {
            if (value > progressBar.Maximum) value = progressBar.Maximum;
            if (value < progressBar.Minimum) value = progressBar.Minimum;

            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke((MethodInvoker)delegate {
                    progressBar.Value = value;
                });
            }
            else
            {
                progressBar.Value = value;
            }
        }

        private void ReadKmlFile(string filePath)
        {
            xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            // Append complete raw KML data to the debug text box
            string rawKmlData = File.ReadAllText(filePath);
            //AppendToDebugTextBox($"Raw KML Data:\n{rawKmlData}\n");

            AppendToDebugTextBox("Sector File Conversion made by ");
            AppendToDebugTextBox("Veda Moola (656077)", isHyperlink: true, hyperlinkUrl: "https://ivao.aero/Member.aspx?Id=656077");
            AppendToDebugTextBox("\nFully tested by ");
            AppendToDebugTextBox("Nilay Parsodkar (709833)", isHyperlink: true, hyperlinkUrl: "https://ivao.aero/Member.aspx?Id=709833"); // Simulate hyperlink
            AppendToDebugTextBox(" Please blame him for any bugs. Haha!. \nBut if you face any problems please dm me or raise an issue on Github repo"); // Simulate hyperlink
            AppendToDebugTextBox("\nNOT FOR REAL WORLD USE", isBold: true, textColor: Color.Red);
            AppendToDebugTextBox($"\nReading the KML File");

            nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("kml", "http://www.opengis.net/kml/2.2");

            // Get placemarks with the correct namespace
            XmlNodeList placemarks = xmlDoc.SelectNodes("//kml:Placemark", nsmgr);
            AppendToDebugTextBox($"\nReading Lines & Polygons & Points");

            foreach (XmlNode placemark in placemarks)
            {
                XmlNode polygonNode = placemark.SelectSingleNode(".//kml:Polygon", nsmgr);
                XmlNode lineStringNode = placemark.SelectSingleNode(".//kml:LineString", nsmgr);
                XmlNode pointNode = placemark.SelectSingleNode(".//kml:Point", nsmgr);

                if (polygonNode != null)
                {
                    ProcessPlacemark(placemark, nsmgr, xmlDoc);
                }
                else if (lineStringNode != null)
                {
                    ProcessLineString(placemark, lineStringNode, nsmgr);
                    //AppendToDebugTextBox($"Processed LineString Placemark: {GetPlacemarkName(placemark, nsmgr)}\n");
                }
                else if (pointNode != null)
                {
                    ProcessPoint(placemark, pointNode, nsmgr);
                }
            }
            AppendToDebugTextBox($"\nGenerating TFL, GEO, Taxiway lables & Runway Data");
        }

        private void ProcessPlacemark(XmlNode placemark, XmlNamespaceManager nsmgr, XmlDocument xmlDoc)
        {
            string name = GetPlacemarkName(placemark, nsmgr);
            string styleUrl = GetStyleUrl(placemark, nsmgr);
            var styleProperties = GetStyleProperties(styleUrl, nsmgr, xmlDoc, placemark);
            XmlNode polygonNode = placemark.SelectSingleNode(".//kml:Polygon", nsmgr);

            if (polygonNode != null)
            {
                ProcessPolygon(polygonNode, name, styleProperties, nsmgr);
                //AppendToDebugTextBox($"Processed Placemark: {name}\nStyle Properties: {styleProperties}\n");
            }
        }

        private void ProcessLineString(XmlNode placemark, XmlNode lineStringNode, XmlNamespaceManager nsmgr)
        {
            string name = GetPlacemarkName(placemark, nsmgr);
            XmlNode coordinatesNode = lineStringNode.SelectSingleNode(".//kml:coordinates", nsmgr);

            if (coordinatesNode != null)
            {
                string coordinates = coordinatesNode.InnerText.Trim();
                string geoData = GenerateGeoFormat(name, coordinates);
                geoOutput += geoData; // Append to the GEO output
            }
        }

        private void ProcessPoint(XmlNode placemark, XmlNode pointNode, XmlNamespaceManager nsmgr)
        {
            // Extract the Placemark name
            string name = GetPlacemarkName(placemark, nsmgr);

            // Ignore placemark names that start with "Runway", "R", or are just numbers
            if (name.StartsWith("Runway") || name.StartsWith("R"))
            {
                ProcessRunwayPlacemark(name, pointNode, nsmgr); // Process runway output
                return; // Skip further processing for taxi labels
            }

            // Extract coordinates from the KML
            XmlNode coordinatesNode = pointNode.SelectSingleNode(".//kml:coordinates", nsmgr);

            if (coordinatesNode != null)
            {
                string coordinates = coordinatesNode.InnerText.Trim();

                // Generate the formatted TXI data
                string txiData = GenerateTXIFormat(name, coordinates);

                // Append the generated TXI data to the output
                // Add the comment only for the first entry
                if (string.IsNullOrEmpty(txiOutput))
                {
                    string folderName = GetFolderNameFromKml(kmlFilePath); // Implement this method
                    txiOutput += $"// {folderName} Taxi Labels\n"; // Add comment for the first entry
                }

                txiOutput += txiData;
            }
        }

        private string GenerateTXIFormat(string placemarkName, string coordinates)
        {
            string[] coordsArray = coordinates.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string txiData = "";
            string folderName = GetFolderNameFromKml(kmlFilePath); // Implement this method

            // Process each coordinate pair
            foreach (var coord in coordsArray)
            {
                var coordParts = coord.Split(',');

                if (coordParts.Length >= 2)
                {
                    // Parse latitude and longitude
                    double lon = double.Parse(coordParts[0].Trim());
                    double lat = double.Parse(coordParts[1].Trim());

                    // Convert to N and E format (N/S for latitude, E/W for longitude)
                    string latFormatted = ConvertToNFormat(lat);
                    string lonFormatted = ConvertToEFormat(lon);

                    // Generate the TXI format line for this placemark
                    txiData += $"{placemarkName};{folderName};{latFormatted};{lonFormatted};\n"; // Note: folderName needs to be accessible
                }
            }

            return txiData;
        }

        // Initialize a HashSet to track processed runways
        private HashSet<string> processedRunways = new HashSet<string>();
        private void ProcessRunwayPlacemark(string runwayName, XmlNode pointNode, XmlNamespaceManager nsmgr)
        {
            // Extract the primary runway number and designator
            string primaryRunway = ExtractRunwayNumber(runwayName, out string designator);

            // Check if this runway has already been processed
            if (processedRunways.Contains(primaryRunway))
            {
                //AppendToDebugTextBox($"Skipping already processed runway: {runwayName}\n");
                return; // Skip processing this runway
            }

            //AppendToDebugTextBox($"Processing Placemark: {runwayName}\n");

            // Extract coordinates for the primary runway
            XmlNode coordinatesNode = pointNode.SelectSingleNode(".//kml:coordinates", nsmgr);
            if (coordinatesNode != null)
            {
                string coordinates = coordinatesNode.InnerText.Trim();
                // Ensure primary runway heading has a trailing zero (i.e., "36" -> "360")
                int primaryHeading = int.Parse(primaryRunway);
                string primaryHeadingFormatted = (primaryHeading * 10).ToString("D3");

                // Calculate the opposite runway heading and runway number
                int oppositeHeading = (primaryHeading + 18) % 36; // Adding 180 for opposite direction
                string oppositeRunwayNumber = oppositeHeading.ToString("D2"); // Convert opposite heading to runway number
                string oppositeRunway = $"Runway {oppositeRunwayNumber}{designator}";

                // Split primary runway coordinates into latitude and longitude
                string[] coordParts = coordinates.Split(',');
                if (coordParts.Length >= 2)
                {
                    double primaryLon = double.Parse(coordParts[0].Trim());
                    double primaryLat = double.Parse(coordParts[1].Trim());

                    // Convert primary coordinates to DMS format
                    string primaryLatFormatted = ConvertToNFormat(primaryLat);
                    string primaryLonFormatted = ConvertToEFormat(primaryLon);

                    // Check for opposite runway placemark
                    XmlNode oppositeNode = FindOppositePlacemark(oppositeRunwayNumber, designator, nsmgr);
                    string oppositeLatFormatted = "N/A", oppositeLonFormatted = "N/A";

                    if (oppositeNode != null)
                    {
                        // Check if opposite runway has already been processed
                        if (!processedRunways.Contains(oppositeRunwayNumber))
                        {
                            XmlNode oppositeCoordinatesNode = oppositeNode.SelectSingleNode(".//kml:coordinates", nsmgr);
                            if (oppositeCoordinatesNode != null)
                            {
                                string oppositeCoordinates = oppositeCoordinatesNode.InnerText.Trim();
                                string[] oppCoordParts = oppositeCoordinates.Split(',');
                                if (oppCoordParts.Length >= 2)
                                {
                                    double oppositeLon = double.Parse(oppCoordParts[0].Trim());
                                    double oppositeLat = double.Parse(oppCoordParts[1].Trim());

                                    // Convert opposite coordinates to DMS format
                                    oppositeLatFormatted = ConvertToNFormat(oppositeLat);
                                    oppositeLonFormatted = ConvertToEFormat(oppositeLon);

                                    //AppendToDebugTextBox($"Opposite Coordinates: {oppositeLatFormatted}, {oppositeLonFormatted}\n");
                                }
                            }

                            // Mark opposite runway as processed
                            processedRunways.Add(oppositeRunwayNumber);
                        }
                    }
                    else
                    {
                        UpdateUI(() =>
                        {
                            MessageBox.Show($"Error: No opposite runway placemark found for {runwayName}. Please check if the runway name is correct.", "Runway Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            // Hide LoadingForm and show Form1
                            this.Hide(); // Assuming this is the LoadingForm
                            Form1 mainForm = new Form1(userIdInput);
                            mainForm.Show();
                        });
                    }

                    // Magnetic headings (ensure proper format for "0360" if applicable)
                    string magneticHeading = primaryHeadingFormatted;
                    string magneticHeadingOpp = (oppositeHeading * 10).ToString("D3");

                    // Generate the final output with primary and opposite runway data
                    string folderName = GetFolderNameFromKml(kmlFilePath); // Get folder name (ICAO code)
                    string rwConfigData = $"{folderName};{primaryRunway}{designator};{oppositeRunwayNumber}{designator};" +
                                          $"PRIMARY_ELEVATION;OPPOSITE_ELEVATION;" +
                                          $"{magneticHeading};{magneticHeadingOpp};" +
                                          $"{primaryLatFormatted};{primaryLonFormatted};" +
                                          $"{oppositeLatFormatted};{oppositeLonFormatted}\n";

                    // Log the generated runway configuration
                    //AppendToDebugTextBox($"Runway Configuration Data: {rwConfigData}\n");

                    // Append runway data to rwOutput for download
                    rwOutput += rwConfigData;

                    // Add primary runway to processed runways set
                    processedRunways.Add(primaryRunway);
                }
            }
        }


        // Updated function to find the opposite placemark
        private XmlNode FindOppositePlacemark(string oppositeRunwayNumber, string designator, XmlNamespaceManager nsmgr)
        {
            // Try searching for both formats: "Runway X" and "RX"
            string oppositeRunwayFormatted1 = $"Runway {oppositeRunwayNumber}{designator}";
            string oppositeRunwayFormatted2 = $"{oppositeRunwayNumber}{designator}";

            // Search for the placemark node using the two formats
            XmlNode oppositePlacemark = xmlDoc.SelectSingleNode($"//kml:Placemark[kml:name='{oppositeRunwayFormatted1}']", nsmgr)
                                    ?? xmlDoc.SelectSingleNode($"//kml:Placemark[kml:name='{oppositeRunwayFormatted2}']", nsmgr);

            return oppositePlacemark;
        }

        // Extract runway number and designator (if any)
        private string ExtractRunwayNumber(string runwayName, out string designator)
        {
            // Initialize the designator as empty (no designator)
            designator = "";

            // Use regular expression to find the runway number and optional designator
            var match = System.Text.RegularExpressions.Regex.Match(runwayName, @"(\d{1,2})([LRC]?)");

            if (match.Success)
            {
                string runwayNumber = match.Groups[1].Value; // Extract runway number
                designator = match.Groups[2].Value;          // Extract designator (if any)

                return runwayNumber; // Return the runway number (e.g., "09", "36")
            }

            throw new FormatException($"Invalid runway name format: {runwayName}");
        }


        private string GenerateGeoFormat(string placemarkName, string coordinates)
        {
            string[] coordsArray = coordinates.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string geoData = $"\n// {placemarkName}\n";

            for (int i = 0; i < coordsArray.Length - 1; i++)
            {
                var startParts = coordsArray[i].Split(',');
                var endParts = coordsArray[i + 1].Split(',');

                if (startParts.Length >= 2 && endParts.Length >= 2)
                {
                    double startLon = double.Parse(startParts[0].Trim());
                    double startLat = double.Parse(startParts[1].Trim());
                    double endLon = double.Parse(endParts[0].Trim());
                    double endLat = double.Parse(endParts[1].Trim());

                    string startLatFormatted = ConvertToNFormat(startLat);
                    string startLonFormatted = ConvertToEFormat(startLon);
                    string endLatFormatted = ConvertToNFormat(endLat);
                    string endLonFormatted = ConvertToEFormat(endLon);

                    geoData += $"{startLatFormatted};{startLonFormatted};{endLatFormatted};{endLonFormatted};TAXI_CENTER\n";
                }
            }

            return geoData;
        }

        private void ProcessPolygon(XmlNode polygonNode, string placemarkName, (string fillColour, string strokeColour, string strokeWidth, string fillColourClear) styleProperties, XmlNamespaceManager nsmgr)
        {
            XmlNode coordinatesNode = polygonNode.SelectSingleNode(".//kml:coordinates", nsmgr);
            if (coordinatesNode != null)
            {
                string coordinates = coordinatesNode.InnerText.Trim();
                // Generate TFL format
                string tflData = GenerateTflFormat(placemarkName, coordinates, styleProperties);
                tflOutput += tflData; // Append to the output
            }
        }

        private string GetPlacemarkName(XmlNode placemark, XmlNamespaceManager nsmgr)
        {
            XmlNode nameNode = placemark.SelectSingleNode("kml:name", nsmgr);
            return nameNode?.InnerText.Trim() ?? "Unnamed Placemark";
        }

        private string GetStyleUrl(XmlNode placemark, XmlNamespaceManager nsmgr)
        {
            XmlNode styleUrlNode = placemark.SelectSingleNode("kml:styleUrl", nsmgr);
            return styleUrlNode?.InnerText.Trim() ?? "No Style URL";
        }

        private (string fillColour, string strokeColour, string strokeWidth, string fillColourClear) GetStyleProperties(string styleUrl, XmlNamespaceManager nsmgr, XmlDocument xmlDoc, XmlNode placemark)
        {
            // Default style properties
            string fillColour = "FFFFFF"; // Default fill color (white)
            string strokeColour = "000000"; // Default stroke color (black)
            string strokeWidth = "1"; // Default stroke width
            string fillColourClear = "0"; // Default fill opacity

            if (!string.IsNullOrEmpty(styleUrl) && styleUrl.StartsWith("#"))
            {
                string styleId = styleUrl.Substring(1); // Remove '#' from styleUrl
                //AppendToDebugTextBox($"Looking for StyleMap or Style with ID: {styleId}\n");

                // Check for StyleMap
                XmlNode styleMapNode = xmlDoc.SelectSingleNode($"//kml:StyleMap[@id='{styleId}']", nsmgr);
                if (styleMapNode != null)
                {
                    //AppendToDebugTextBox($"Found StyleMap with ID: {styleId}\n");
                    string normalStyleUrl = styleMapNode.SelectSingleNode(".//kml:Pair[kml:key='normal']/kml:styleUrl", nsmgr)?.InnerText.Trim();
                    if (!string.IsNullOrEmpty(normalStyleUrl))
                    {
                        //AppendToDebugTextBox($"Normal Style URL: {normalStyleUrl}\n");
                        return GetStyleProperties(normalStyleUrl, nsmgr, xmlDoc, placemark); // Recursive call to get normal style
                    }
                }
                else
                {
                    //AppendToDebugTextBox($"No StyleMap found with ID: {styleId}\n");
                }

                // Check for direct style
                XmlNode styleNode = xmlDoc.SelectSingleNode($"//kml:Style[@id='{styleId}']", nsmgr);
                if (styleNode != null)
                {
                    //AppendToDebugTextBox($"Found Style with ID: {styleId}\n");

                    // Retrieve properties with explicit checks
                    fillColour = GetStylePropertyValue(styleNode, "kml:PolyStyle", "color", nsmgr, fillColour);
                    if (string.IsNullOrEmpty(fillColour))
                    {
                        //AppendToDebugTextBox($"Fill Colour not found for Style ID: {styleId}\n");
                    }
                    else
                    {
                        fillColour = "#" + fillColour; // Prefix fill color with '#'
                    }

                    // Change "outline" to "fill" for fill opacity, if needed
                    fillColourClear = GetStylePropertyValue(styleNode, "kml:PolyStyle", "fill", nsmgr, fillColourClear) == "1" ? "1" : "0";
                    strokeColour = GetStylePropertyValue(styleNode, "kml:LineStyle", "color", nsmgr, strokeColour);
                    if (string.IsNullOrEmpty(strokeColour))
                    {
                        //AppendToDebugTextBox($"Stroke Colour not found for Style ID: {styleId}\n");
                    }
                    else
                    {
                        strokeColour = "#" + strokeColour;
                    }

                    strokeWidth = GetStylePropertyValue(styleNode, "kml:LineStyle", "width", nsmgr, strokeWidth);

                    // Log the retrieved values
                    //AppendToDebugTextBox($"Retrieved Style Properties for ID: {styleId}\n");
                    //AppendToDebugTextBox($"Fill Colour: {fillColour}\n");
                    //AppendToDebugTextBox($"Stroke Colour: {strokeColour}\n");
                    //AppendToDebugTextBox($"Stroke Width: {strokeWidth}\n");
                    //AppendToDebugTextBox($"Fill Colour Clear: {fillColourClear}\n");
                }
                else
                {
                    //AppendToDebugTextBox($"No direct Style found with ID: {styleId}\n");
                }
            }
            else
            {
                //AppendToDebugTextBox($"Style URL is null or does not start with '#': {styleUrl}\n");
            }

            return (fillColour, strokeColour, strokeWidth, fillColourClear);
        }

        private string GetStylePropertyValue(XmlNode styleNode, string styleType, string propertyName, XmlNamespaceManager nsmgr, string defaultValue)
        {
            // Use a more robust method to retrieve style properties
            XmlNode propertyNode = styleNode.SelectSingleNode($"{styleType}/kml:{propertyName}", nsmgr);
            return propertyNode?.InnerText.Trim() ?? defaultValue;
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

        private string GenerateTflFormat(string placemarkName, string coordinates, (string fillColour, string strokeColour, string strokeWidth, string fillColourClear) styleProperties)
        {
            string[] coordsArray = coordinates.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string tflData = $"\n// {placemarkName}\n";
            tflData += $"STATIC;{styleProperties.fillColour};{styleProperties.strokeWidth};{styleProperties.strokeColour};{styleProperties.fillColourClear};\n";

            foreach (string coord in coordsArray)
            {
                var parts = coord.Split(',');
                if (parts.Length >= 2)
                {
                    double lon = double.Parse(parts[0].Trim());
                    double lat = double.Parse(parts[1].Trim());
                    string latFormatted = ConvertToNFormat(lat);
                    string lonFormatted = ConvertToEFormat(lon);
                    tflData += $"{latFormatted};{lonFormatted}\n"; // Format as "N;E"
                }
            }

            return tflData; // Return the complete TFL formatted data
        }

        private void SaveTflFile(string data, string placemarkName)
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TFL_Files");
            Directory.CreateDirectory(folderPath);
            string fileName = $"{placemarkName}";
            string filePath = Path.Combine(folderPath, fileName);
            File.WriteAllText(filePath, data);
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tflOutput))
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    // Extract the folder name from the KML structure
                    string folderName = GetFolderNameFromKml(kmlFilePath); // You need to implement this method

                    // Set default file name and filter
                    saveFileDialog.FileName = $"{folderName}";
                    saveFileDialog.Filter = "TFL files (*.tfl)|*.tfl|All files (*.*)|*.*";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        SaveTflFile(tflOutput, saveFileDialog.FileName);
                        MessageBox.Show("TFL file downloaded successfully!");
                    }
                }
            }
            else
            {
                MessageBox.Show("No TFL data available to download.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void downloadRunwayButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(rwOutput)) // Check if runway data exists
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    string folderName = GetFolderNameFromKml(kmlFilePath); // You need to implement this method

                    saveFileDialog.FileName = $"{folderName}";
                    saveFileDialog.Filter = "RW files (*.rw)|*.rw|All files (*.*)|*.*";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(saveFileDialog.FileName, rwOutput);
                        MessageBox.Show("Runway configurations file downloaded successfully!");
                    }
                }
            }
            else
            {
                MessageBox.Show("No runway configuration data available to download.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void downloadGeoButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(geoOutput))
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    // Extract the folder name from the KML structure
                    string folderName = GetFolderNameFromKml(kmlFilePath); // You need to implement this method

                    // Set default file name and filter
                    saveFileDialog.FileName = $"{folderName}";
                    saveFileDialog.Filter = "GEO files (*.geo)|*.geo|All files (*.*)|*.*";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(saveFileDialog.FileName, geoOutput);
                        MessageBox.Show("GEO file downloaded successfully!");
                    }
                }
            }
            else
            {
                MessageBox.Show("No GEO data available to download.","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void downloadTXIButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txiOutput))
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    // Extract the folder name from the KML structure
                    string folderName = GetFolderNameFromKml(kmlFilePath);

                    // Set default file name and filter
                    saveFileDialog.FileName = $"{folderName}.txi"; // Set default file name to folderName.txi
                    saveFileDialog.Filter = "TXI files (*.txi)|*.txi|All files (*.*)|*.*";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(saveFileDialog.FileName, txiOutput); // Use txiOutput to save the content
                        MessageBox.Show("Taxi Labels file downloaded successfully!");
                    }
                }
            }
            else
            {
                MessageBox.Show("No Taxi label data available to download.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void downloadRawXmlButton_Click(object sender, EventArgs e)
        {
            // Check if the user is Veda Moola (user ID: 656077)
            if (userIdInput == 656077)
            {
                try
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                        saveFileDialog.Title = "Save Raw KML File";
                        saveFileDialog.FileName = Path.GetFileNameWithoutExtension(kmlFilePath) + "_raw.xml"; // Default file name

                        // Show the dialog and check if the user clicked 'Save'
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            // Write the raw KML file content to the chosen path
                            File.WriteAllText(saveFileDialog.FileName, File.ReadAllText(kmlFilePath));
                            MessageBox.Show($"Raw KML file saved successfully to:\n{saveFileDialog.FileName}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving raw KML file: {ex.Message}");
                }
            }
            else
            {
                // Show an error message if the user is not Veda Moola (656077)
                MessageBox.Show("Only Veda Moola (656077) is authorized to download this file.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetFolderNameFromKml(string kmlPath)
        {
            // Load the KML file to extract the folder name
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(kmlPath);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("kml", "http://www.opengis.net/kml/2.2");

            XmlNode folderNode = xmlDoc.SelectSingleNode("//kml:Folder/kml:name", nsmgr);
            return folderNode?.InnerText.Trim() ?? "Untitled"; // Default name if none found
        }

        private void AppendToDebugTextBox(string message, bool isBold = false, Color? textColor = null, bool isHyperlink = false, string hyperlinkUrl = null)
        {
            if (debugTextBox.InvokeRequired)
            {
                debugTextBox.Invoke((MethodInvoker)delegate {
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

            int start = debugTextBox.TextLength;
            debugTextBox.AppendText(message);  // Append the message

            debugTextBox.Select(start, message.Length);

            // Apply bold formatting if requested
            debugTextBox.SelectionFont = isBold ? new Font(debugTextBox.Font, FontStyle.Bold) : new Font(debugTextBox.Font, FontStyle.Regular);

            // Apply hyperlink formatting if requested
            if (isHyperlink)
            {
                debugTextBox.SelectionColor = Color.Blue;
                debugTextBox.SelectionFont = new Font(debugTextBox.Font, FontStyle.Underline);
                // Set the hyperlink URL in the Tag for later use
                debugTextBox.Tag = hyperlinkUrl;  // Optional, if you want to store the hyperlink URL for later use
            }
            else
            {
                // Set text color
                debugTextBox.SelectionColor = colorToUse;
            }

            debugTextBox.Select(debugTextBox.TextLength, 0);
            debugTextBox.ScrollToCaret();
        }

        // LinkClicked event handler
        private void debugTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            // Open the hyperlink when clicked
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = e.LinkText,
                UseShellExecute = true // Use this to allow opening URLs correctly
            });
        }

        // Event handler to open previewLoadingForm
        private void previewButton_Click(object sender, EventArgs e)
        {
            // Instantiate PreviewForm and pass the variables
            previewGroundLayoutForm previewForm = new previewGroundLayoutForm(tflOutput, geoOutput, txiOutput);
            previewForm.ShowDialog();
        }

    }
}