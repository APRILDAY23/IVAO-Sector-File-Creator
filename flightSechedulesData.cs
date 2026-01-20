using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Org.BouncyCastle.Math.Primes;
using Newtonsoft.Json;
using ClosedXML.Excel;

namespace Sector_File
{
    public partial class flightSechedulesData : Form
    {
        private string schedulesOuput;
        private List<Dictionary<string, string>> flightDataList;
        private int userId;

        public flightSechedulesData(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            this.Icon = new Icon("./tools.ico");
            searchButton.Click += new EventHandler(searchButton_Click); // Register the search button event handler
            downloadFlightScheduleButton.Click += new EventHandler(downloadFlightScheduleButton_Click); // Register the download button event handler
        }

        private async void searchButton_Click(object sender, EventArgs e)
        {
            string searchTerm = searchBox.Text.Trim().ToUpper();
            UpdateProgress(0);

            AppendToDebugTextBox("Flight Schedule Module made by ");
            AppendToDebugTextBox("Veda Moola (656077)", isHyperlink: true, hyperlinkUrl: "https://ivao.aero/Member.aspx?Id=656077");
            AppendToDebugTextBox("\nFully tested by ");
            AppendToDebugTextBox("Nilay Parsodkar (709833)", isHyperlink: true, hyperlinkUrl: "https://ivao.aero/Member.aspx?Id=709833");
            AppendToDebugTextBox(" Please blame him for any bugs. Haha!. \nBut if you face any problems please dm me or raise an issue on Github repo");
            AppendToDebugTextBox("\nNOT FOR REAL WORLD USE", isBold: true, textColor: Color.Red);
            AppendToDebugTextBox($"\nFetching Flight Schedule data for airport: {searchTerm}");

            if (string.IsNullOrEmpty(searchTerm) || searchTerm.Length > 4)
            {
                MessageBox.Show("Please enter a valid ICAO Code Identifier (up to 4 letters).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                string apiResponse = await FetchDataAsync(searchTerm);
                var decodedResponse = JsonConvert.DeserializeObject<dynamic>(apiResponse);
                //AppendToDebugTextBox($"Decoded Response: {decodedResponse?.data?.airport?.pluginData?.schedule}"); // Log the decoded response to inspect its structure

                // Declare flightDataList and newFlightId here
                flightDataList = new List<Dictionary<string, string>>();
                int newFlightId = 1; // Start ID for each flight

                // Check for the presence of schedule and log it to see the structure
                var schedule = decodedResponse?.data?.airport?.pluginData?.schedule;
                if (schedule == null)
                {
                    AppendToDebugTextBox("No schedule data found.");
                    return;
                }
             
                var arrivals = schedule.arrivals?.data;
                if (arrivals != null)
                {
                    ProcessFlightData(arrivals, flightDataList, ref newFlightId, decodedResponse);
                    UpdateProgress(50);
                }
                else
                {
                    AppendToDebugTextBox("\nNo Arrivals Data Found:", isBold: true, textColor: Color.Red);
                }

                var departures = schedule.departures?.data;
                if (departures != null)
                {
                    ProcessFlightData(departures, flightDataList, ref newFlightId, decodedResponse);
                    UpdateProgress(100);
                }
                else
                {
                    AppendToDebugTextBox("\nNo Departures Data Found:", isBold: true, textColor: Color.Red);
                }

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

        private void ProcessFlightData(dynamic flightsData, List<Dictionary<string, string>> flightDataList, ref int newFlightId, dynamic decodedResponse)
        {
            if (flightsData != null && flightsData is IEnumerable<dynamic>)
            {
                foreach (var flightWrapper in flightsData)
                {
                    // Log to understand the structure of each flightWrapper

                    if (flightWrapper?.flight != null)
                    {
                        var flight = flightWrapper.flight; // Access the flight data

                        // Convert timestamps to DateTime and format to HH:mm or N/A if null
                        string departureTime = flight.time.scheduled.departure != null
                            ? DateTimeOffset.FromUnixTimeSeconds((long)flight.time.scheduled.departure).ToString("HH:mm")
                            : "N/A";

                        string arrivalTime = flight.time.scheduled.arrival != null
                            ? DateTimeOffset.FromUnixTimeSeconds((long)flight.time.scheduled.arrival).ToString("HH:mm")
                            : "N/A";

                        // Calculate flight duration if both times are available
                        string flightDurationStr = "N/A";
                        if (flight.time.scheduled.departure != null && flight.time.scheduled.arrival != null)
                        {
                            var departureDateTime = DateTimeOffset.FromUnixTimeSeconds((long)flight.time.scheduled.departure).DateTime;
                            var arrivalDateTime = DateTimeOffset.FromUnixTimeSeconds((long)flight.time.scheduled.arrival).DateTime;
                            TimeSpan flightDuration = arrivalDateTime - departureDateTime;
                            flightDurationStr = flightDuration.ToString(@"hh\:mm");
                        }

                        // Initialize variables with safe null handling
                        string flightId = newFlightId++.ToString();
                        string callSign = flight.identification?.callsign ?? "N/A";
                        string aircraftType = flight.aircraft?.model?.code ?? "N/A";
                        string departureIcao = flight.airport?.origin?.code?.icao ?? "N/A";
                        string arrivalIcao = flight.airport?.destination?.code?.icao ?? "N/A";
                        string departureCity = flight.airport?.origin?.position?.region?.city ?? "N/A";
                        string arrivalCity = flight.airport?.destination?.position?.region?.city ?? "N/A";
                        string status = "Unknown";

                        // Build flight row data dictionary
                        var rowData = new Dictionary<string, string>
                        {
                            ["Flight_Id"] = flightId,
                            ["Call_Sign"] = callSign,
                            ["Number"] = (string)flight.identification?.number?["default"] ?? "N/A", // Safely access number["default"]
                            ["Aircraft_Type"] = aircraftType,
                            ["Departure_Icao"] = flight.airport?.origin?.code?["icao"]?.ToString()
                                 ?? decodedResponse.data?.airport?.pluginData?.details?.code?["icao"]?.ToString()
                                 ?? "N/A",
                            ["Arrival_Icao"] = flight.airport?.destination?.code?["icao"]?.ToString()
                               ?? decodedResponse.data?.airport?.pluginData?.details?.code?["icao"]?.ToString()
                               ?? "N/A",
                            ["Departure_City"] = flight.airport?.origin?.position?.region?["city"]?.ToString()
                                 ?? decodedResponse.data?.airport?.pluginData?.details?.position?.region?["city"]?.ToString()
                                 ?? "N/A",
                            ["Arrival_City"] = flight.airport?.destination?.position?.region?["city"]?.ToString()
                               ?? decodedResponse.data?.airport?.pluginData?.details?.position?.region?["city"]?.ToString()
                               ?? "N/A",
                            ["Departure_Time"] = departureTime,
                            ["Arrival_Time"] = arrivalTime,
                            ["Flight_Duration"] = flightDurationStr,
                            ["Status"] = status
                        };

                        // Add row data to list
                        flightDataList.Add(rowData);
                    }
                }
            }
        }


        private async Task<string> FetchDataAsync(string airportId)
        {
            using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(120) }) // Set to 120 seconds
            {
                client.BaseAddress = new Uri("https://flightradar24-com.p.rapidapi.com");
                client.DefaultRequestHeaders.Add("x-rapidapi-host", "flightradar24-com.p.rapidapi.com");
                client.DefaultRequestHeaders.Add("x-rapidapi-key", "cc04b039a6msh3bace4737eb7256p1523a8jsn0041b081ec4e");

                try
                {
                    HttpResponseMessage response = await client.GetAsync($"/airports/routes?airport_id={airportId}&page=1");
                    response.EnsureSuccessStatusCode(); // Throws if status code is not 2xx

                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException e)
                {
                    return $"Request error: {e.Message}";
                }
                catch (TaskCanceledException)
                {
                    return "Request timed out. Please try again later.";
                }
            }
        }


        private async void downloadFlightScheduleButton_Click(object sender, EventArgs e)
        {
            if (flightDataList == null || !flightDataList.Any())
            {
                MessageBox.Show("No data to download. Please perform a search first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string searchTerm = searchBox.Text.Trim().ToUpper();

            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                saveFileDialog.Title = "Save Flight Schedules";
                saveFileDialog.FileName = $"FlightSchedules_{searchTerm.ToUpper()}.xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Flight Schedules");

                        // Define headers based on ESP2 format
                        string[] headers = { "Flight ID", "Call Sign", "Flight Number", "Aircraft Type", "Departure ICAO", "Arrival ICAO", "Departure City", "Arrival City", "Departure Time", "Arrival Time", "Flight Duration", "Status" };

                        // Insert headers
                        for (int i = 0; i < headers.Length; i++)
                        {
                            worksheet.Cell(1, i + 1).Value = headers[i];
                            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                        }

                        // Populate data
                        for (int i = 0; i < flightDataList.Count; i++)
                        {
                            var row = flightDataList[i];
                            worksheet.Cell(i + 2, 1).Value = row["Flight_Id"];
                            worksheet.Cell(i + 2, 2).Value = row["Call_Sign"];
                            worksheet.Cell(i + 2, 3).Value = row["Number"];
                            worksheet.Cell(i + 2, 4).Value = row["Aircraft_Type"];
                            worksheet.Cell(i + 2, 5).Value = row["Departure_Icao"];
                            worksheet.Cell(i + 2, 6).Value = row["Arrival_Icao"];
                            worksheet.Cell(i + 2, 7).Value = row["Departure_City"];
                            worksheet.Cell(i + 2, 8).Value = row["Arrival_City"];
                            worksheet.Cell(i + 2, 9).Value = row["Departure_Time"];
                            worksheet.Cell(i + 2, 10).Value = row["Arrival_Time"];
                            worksheet.Cell(i + 2, 11).Value = row["Flight_Duration"];
                            worksheet.Cell(i + 2, 12).Value = row["Status"];
                        }

                        // Adjust column widths for readability
                        worksheet.Columns().AdjustToContents();

                        // Save the workbook to the selected file path
                        workbook.SaveAs(saveFileDialog.FileName);

                        MessageBox.Show($"Flight schedule data successfully saved to {saveFileDialog.FileName}", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
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
