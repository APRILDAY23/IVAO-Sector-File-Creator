using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq; // Ensure you have the Newtonsoft.Json package installed

namespace Sector_File
{
    public partial class firDataForm : Form
    {
        private int userIdInput;
        private string firOutput; // Variable to store the FIR output

        public firDataForm(int userId)
        {
            userIdInput = userId;
            this.Icon = new Icon("./tools.ico");
            InitializeComponent();
            searchButton.Click += new EventHandler(searchButton_Click); // Register the search button event handler
            downloadFIRBoundary.Click += new EventHandler(downloadFIRBoundary_Click); // Register the download button event handler
        }

        private async void searchButton_Click(object sender, EventArgs e)
        {
            string searchTerm = searchBox.Text.Trim().ToUpper();

            AppendToDebugTextBox("Sector File Conversion made by ");
            AppendToDebugTextBox("Veda Moola (656077)", isHyperlink: true, hyperlinkUrl: "https://ivao.aero/Member.aspx?Id=656077");
            AppendToDebugTextBox("\nFully tested by ");
            AppendToDebugTextBox("Nilay Parsodkar (709833)", isHyperlink: true, hyperlinkUrl: "https://ivao.aero/Member.aspx?Id=709833");
            AppendToDebugTextBox(" Please blame him for any bugs. Haha!. \nBut if you face any problems please dm me or raise an issue on Github repo");
            AppendToDebugTextBox("\nFIR data provided by ");
            AppendToDebugTextBox("Jordan Kirkby (646483)", isHyperlink: true, hyperlinkUrl: "https://ivao.aero/Member.aspx?Id=709833");
            AppendToDebugTextBox("\nNOT FOR REAL WORLD USE", isBold: true, textColor: Color.Red);
            AppendToDebugTextBox($"\nFetching FIR data for: {searchTerm}");

            if (string.IsNullOrEmpty(searchTerm))
            {
                MessageBox.Show("Please enter a valid FIR Identifier.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (HttpClient client = new HttpClient())
                {
                    string url = "https://portal.in.ivao.aero/core/nav_data_2309019312309/data/nav_jsondata_2309019312309/tbl_fir_uir.json";
                    string jsonResponse = await client.GetStringAsync(url);

                    await Task.Run(() =>
                    {
                        for (int i = 0; i < 100; i += 10)
                        {
                            System.Threading.Thread.Sleep(50);
                            UpdateProgress(i + 10);
                        }
                    });

                    JArray firDataArray = JArray.Parse(jsonResponse);
                    StringBuilder outputBuilder = new StringBuilder();
                    bool isFirstShape = true;
                    string firstLineOfShape = "";
                    double lastLatitude = 0;
                    double lastLongitude = 0;

                    foreach (var item in firDataArray)
                    {
                        if (item["fir_uir_identifier"].ToString().Equals(searchTerm))
                        {
                            int seqno = (int)item["seqno"];
                            double latitude = (double)item["fir_uir_latitude"];
                            double longitude = (double)item["fir_uir_longitude"];
                            string latConverted = ConvertToNFormat(latitude);
                            string lonConverted = ConvertToEFormat(longitude);
                            string currentLine = $"T;{searchTerm};{latConverted};{lonConverted};";

                            double? arcBearing = item["arc_bearing"]?.ToObject<double?>();
                            double? arcDistance = item["arc_distance"]?.ToObject<double?>();
                            double? arcOriginLatitude = item["arc_origin_latitude"]?.ToObject<double?>();
                            double? arcOriginLongitude = item["arc_origin_longitude"]?.ToObject<double?>();

                            if (arcBearing.HasValue && arcDistance.HasValue &&
                                arcOriginLatitude.HasValue && arcOriginLongitude.HasValue)
                            {
                                int nextIndex = firDataArray.IndexOf(item) + 1;
                                if (nextIndex < firDataArray.Count)
                                {
                                    var nextItem = firDataArray[nextIndex];
                                    double nextLatitude = (double)nextItem["fir_uir_latitude"];
                                    double nextLongitude = (double)nextItem["fir_uir_longitude"];
                                    //AppendToDebugTextBox($"\nNext Item FIR Identifier: {nextItem["fir_uir_identifier"]}");
                                    //($"Next Item Latitude: {nextLatitude}");
                                    //AppendToDebugTextBox($"Next Item Longitude: {nextLongitude}");

                                    // Calculate and append arc points
                                    List<string> arcPoints = CalculateArcPoints(
                                        arcOriginLatitude.Value, arcOriginLongitude.Value,
                                        latitude, longitude,
                                        nextLatitude, nextLongitude,
                                        arcDistance.Value
                                    );

                                    foreach (var point in arcPoints)
                                    {
                                        outputBuilder.AppendLine(point);
                                    }
                                }
                            }
                            else
                            {
                                if (seqno == 10)
                                {
                                    if (!isFirstShape)
                                    {
                                        outputBuilder.AppendLine(firstLineOfShape);
                                        string latHemisphere = lastLatitude >= 0 ? "N" : "S";
                                        string lonHemisphere = lastLongitude >= 0 ? "E" : "W";
                                        outputBuilder.AppendLine($"\nT;Dummy;{latHemisphere}000.00.00.000;{lonHemisphere}000.00.00.000");
                                    }
                                    firstLineOfShape = currentLine;
                                }

                                outputBuilder.AppendLine(currentLine);
                                lastLatitude = latitude;
                                lastLongitude = longitude;
                                isFirstShape = false;
                            }
                        }
                    }

                    if (outputBuilder.Length > 0)
                    {
                        firOutput = outputBuilder.ToString();
                    }
                    else
                    {
                        MessageBox.Show("No FIR data found for the specified identifier.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    stopwatch.Stop();
                    string timeMessage = stopwatch.Elapsed.TotalSeconds > 1
                        ? $"\nPROCESS COMPLETED in {stopwatch.Elapsed.TotalSeconds:F2} seconds"
                        : $"\nPROCESS COMPLETED in {stopwatch.Elapsed.TotalMilliseconds:F2} ms";

                    AppendToDebugTextBox(timeMessage, isBold: true, textColor: Color.Green);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<string> CalculateArcPoints(
     double originLat, double originLon,
     double startLat, double startLon,
     double nextLat, double nextLon,
     double arcRadiusNm)
        {
            const double EarthRadiusNm = 3440.0;
            List<string> arcPoints = new List<string>();

            double originLatRad = DegreesToRadians(originLat);
            double originLonRad = DegreesToRadians(originLon);
            double startLatRad = DegreesToRadians(startLat);
            double startLonRad = DegreesToRadians(startLon);
            double nextLatRad = DegreesToRadians(nextLat);
            double nextLonRad = DegreesToRadians(nextLon);

            int numPoints = 50; // Set the number of points for a smooth arc
            double startBearing = CalculateBearing(originLat, originLon, startLat, startLon);
            double nextBearing = CalculateBearing(originLat, originLon, nextLat, nextLon);

            for (int i = 0; i <= numPoints; i++)
            {
                double fraction = i / (double)numPoints;
                double currentBearing = startBearing + fraction * (nextBearing - startBearing);
                double distanceRatio = arcRadiusNm / EarthRadiusNm;

                double arcLat = Math.Asin(Math.Sin(originLatRad) * Math.Cos(distanceRatio) +
                                          Math.Cos(originLatRad) * Math.Sin(distanceRatio) * Math.Cos(DegreesToRadians(currentBearing)));

                double arcLon = originLonRad + Math.Atan2(Math.Sin(DegreesToRadians(currentBearing)) * Math.Sin(distanceRatio) * Math.Cos(originLatRad),
                                                          Math.Cos(distanceRatio) - Math.Sin(originLatRad) * Math.Sin(arcLat));

                double arcLatDeg = RadiansToDegrees(arcLat);
                double arcLonDeg = RadiansToDegrees(arcLon);

                string latFormatted = ConvertToNFormat(arcLatDeg);
                string lonFormatted = ConvertToEFormat(arcLonDeg);

                arcPoints.Add($"T;ArcPoint;{latFormatted};{lonFormatted};");
            }

            // The last point should connect to the next object's latitude and longitude
            string nextLatFormatted = ConvertToNFormat(nextLat);
            string nextLonFormatted = ConvertToEFormat(nextLon);
            arcPoints.Add($"T;ArcPoint;{nextLatFormatted};{nextLonFormatted};");

            return arcPoints;
        }


        private double CalculateBearing(double lat1, double lon1, double lat2, double lon2)
        {
            double lat1Rad = DegreesToRadians(lat1);
            double lat2Rad = DegreesToRadians(lat2);
            double deltaLonRad = DegreesToRadians(lon2 - lon1);

            double y = Math.Sin(deltaLonRad) * Math.Cos(lat2Rad);
            double x = Math.Cos(lat1Rad) * Math.Sin(lat2Rad) - Math.Sin(lat1Rad) * Math.Cos(lat2Rad) * Math.Cos(deltaLonRad);
            return (RadiansToDegrees(Math.Atan2(y, x)) + 360) % 360;
        }

        // Conversion methods
        private double DegreesToRadians(double degrees) => degrees * (Math.PI / 180);
        private double RadiansToDegrees(double radians) => radians * (180 / Math.PI);


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

        private async void downloadFIRBoundary_Click(object sender, EventArgs e)
        {
            string searchTerm = searchBox.Text.Trim().ToUpper();

            if (string.IsNullOrEmpty(searchTerm))
            {
                MessageBox.Show("Please enter a valid FIR Identifier before downloading.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Use SaveFileDialog to allow the user to choose where to save the file
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "ARTCC Files (*.artcc)|*.artcc|All Files (*.*)|*.*";
                saveFileDialog.FileName = $"{searchTerm}_FIR.artcc"; // Default file name

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Save the output to the selected file
                        File.WriteAllText(saveFileDialog.FileName, firOutput);
                        MessageBox.Show($"FIR Boundary file has been saved to {saveFileDialog.FileName} successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while saving the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
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

        private void downloadWebeyeShapes_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is still a WORK IN PROGRESS for Webeye shapes.",
                            "Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
        }

    }
}
