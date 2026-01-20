using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Sector_File
{
    public partial class CountryDataForm : Form
    {
        private string filteredCoordinates = string.Empty;
        private int userIdInput;

        public CountryDataForm(int userId)
        {
            userIdInput = userId;
            this.Icon = new Icon("./tools.ico");
            InitializeComponent();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            // Call the async function to fetch country data
            _ = GetCountryDataAsync();
        }

        private async Task GetCountryDataAsync()
        {
            string searchQuery = searchBox.Text;
            if (string.IsNullOrEmpty(searchQuery))
            {
                MessageBox.Show("Please enter a valid country name or ISO code.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Reset progress bar
            loadingProgressBar.Value = 0;

            // Append processing completion information
            AppendToDebugTextBox("Sector File Conversion made by ");
            AppendToDebugTextBox("Veda Moola (656077)", isHyperlink: true, hyperlinkUrl: "https://ivao.aero/Member.aspx?Id=656077");
            AppendToDebugTextBox("\nFully tested by ");
            AppendToDebugTextBox("Nilay Parsodkar (709833)", isHyperlink: true, hyperlinkUrl: "https://ivao.aero/Member.aspx?Id=709833");
            AppendToDebugTextBox(" Please blame him for any bugs. Haha!. \nBut if you face any problems please dm me or raise an issue on Github repo");
            AppendToDebugTextBox("\nNOT FOR REAL WORLD USE", isBold: true, textColor: Color.Red);
            AppendToDebugTextBox($"\nFetching data for country: {searchQuery}");

            string url = "https://r2.datahub.io/clvyjaryy0000la0cxieg4o8o/main/raw/data/countries.geojson";

            try
            {
                // Start timing the process
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);

                    // Simulate loading progress
                    await Task.Run(() =>
                    {
                        for (int i = 0; i < 100; i += 10)
                        {
                            System.Threading.Thread.Sleep(50); // Simulate time-consuming task
                            UpdateProgress(i + 10); // Update progress
                        }
                    });

                    // Parse the response
                    JsonDocument jsonDoc = JsonDocument.Parse(response);
                    var features = jsonDoc.RootElement.GetProperty("features").EnumerateArray();

                    // Filter the features by ADMIN or ISO_A2
                    var filtered = features
                        .Where(f => f.GetProperty("properties").GetProperty("ADMIN").GetString().Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                                    f.GetProperty("properties").GetProperty("ISO_A2").GetString().Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault();

                    if (filtered.ValueKind != JsonValueKind.Undefined)
                    {
                        var coordinatesArray = filtered.GetProperty("geometry").GetProperty("coordinates");
                        filteredCoordinates = ConvertCoordinatesToDMS(coordinatesArray);

                        AppendToDebugTextBox("\nCountry data fetched successfully.");
                    }
                    else
                    {
                        MessageBox.Show("No matching country found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    // Stop the stopwatch and log the time taken
                    stopwatch.Stop();
                    string timeMessage = stopwatch.Elapsed.TotalSeconds > 1
                        ? $"\nPROCESS COMPLETED in {stopwatch.Elapsed.TotalSeconds:F2} seconds"
                        : $"\nPROCESS COMPLETED in {stopwatch.Elapsed.TotalMilliseconds:F2} ms";

                    AppendToDebugTextBox(timeMessage, isBold: true, textColor: Color.Green); // Append the completion message

                    downloadButton.Enabled = true; // Enable the download button
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ConvertCoordinatesToDMS(JsonElement coordinatesArray)
        {
            var sb = new StringBuilder();

            foreach (var polygon in coordinatesArray.EnumerateArray())
            {
                var coordinatesList = new List<(double Latitude, double Longitude)>();

                // Collecting all points in the polygon
                foreach (var ring in polygon.EnumerateArray())
                {
                    foreach (var point in ring.EnumerateArray())
                    {
                        double longitude = point[0].GetDouble();
                        double latitude = point[1].GetDouble();
                        coordinatesList.Add((latitude, longitude));
                    }
                }

                // Create lines between start and end points
                for (int i = 0; i < coordinatesList.Count; i++)
                {
                    // Get the current point
                    var startPoint = coordinatesList[i];

                    // Get the next point (with wrap-around to the first point)
                    var endPoint = coordinatesList[(i + 1) % coordinatesList.Count];

                    // Convert to DMS format
                    string latStartDMS = ConvertToNFormat(startPoint.Latitude);
                    string lonStartDMS = ConvertToEFormat(startPoint.Longitude);
                    string latEndDMS = ConvertToNFormat(endPoint.Latitude);
                    string lonEndDMS = ConvertToEFormat(endPoint.Longitude);

                    // Log raw and converted coordinates
                    sb.AppendLine($"{latStartDMS};{lonStartDMS};{latEndDMS};{lonEndDMS};COAST;");
                }
            }

            return sb.ToString();
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
        private void downloadButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(filteredCoordinates))
            {
                MessageBox.Show("No coordinates to download.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Use the search query as the file name
            string fileName = $"{searchBox.Text}_CountryCoordinates.geo"; // Append .geo extension

            // Code to save the filteredCoordinates to a file
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Geo Files (*.geo)|*.geo|Text Files (*.txt)|*.txt",
                Title = "Save Coordinates File",
                FileName = fileName // Set the initial file name based on the search query
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, filteredCoordinates);
                MessageBox.Show("Coordinates downloaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void backButton_Click(object sender, EventArgs e)
        {
            // Code to go back to the main menu
            this.Hide();
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
    }
}
