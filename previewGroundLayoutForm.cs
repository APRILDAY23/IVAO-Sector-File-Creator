using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Sector_File
{
    public partial class previewGroundLayoutForm : Form
    {
        private string tflOutput;
        private string geoOutput;
        private string txiOutput;

        private double zoomFactor = 1.0;
        private const double ZoomIncrement = 0.1;
        private double scaleFactor = 1.0;

        private Dictionary<string, string> predefinedColors = new Dictionary<string, string>
        {
            { "TAXIWAY", "#FF0000" },
            { "RUNWAY", "Black" },
            { "MARKINGS", "White" },
            { "APRON", "Brown" },
            { "BOUNDARY", "#475440" },
            { "BUILDING", "#FF8800" },
            { "TAXI_CENTER", "White" }, // Add GEO layer color for TAXI_CENTER
            { "TXI", "Yellow" } // Add GEO layer color for TAXI_CENTER

        };

        private Dictionary<string, List<PointF>> layerPoints = new Dictionary<string, List<PointF>>();
        private Dictionary<string, (Brush fill, float strokeWidth, Pen stroke)> layerStyles = new Dictionary<string, (Brush, float, Pen)>();
        private List<string> layerOrder = new List<string>();

        // Panning state variables
        private bool isPanning = false;
        private Point previousMousePosition;

        public previewGroundLayoutForm(string tflOutput, string geoOutput, string txiOutput)
        {
            InitializeComponent();
            this.Icon = new Icon("./tools.ico");

            this.tflOutput = tflOutput;
            this.geoOutput = geoOutput;
            this.txiOutput = txiOutput;

            plotCanvas.Paint += new PaintEventHandler(plotCanvas_Paint);
            plotCanvas.MouseWheel += new MouseEventHandler(plotCanvas_MouseWheel);
            plotCanvas.MouseDown += new MouseEventHandler(plotCanvas_MouseDown);
            plotCanvas.MouseMove += new MouseEventHandler(plotCanvas_MouseMove);
            plotCanvas.MouseUp += new MouseEventHandler(plotCanvas_MouseUp);

            ProcessTFLFile(tflOutput);
            ProcessGEOFile(geoOutput); // Call the function to process GEO data
            ProcessTXIFile(txiOutput); // Call the function to process Taxi Labels
        }

        private void ProcessTFLFile(string tflOutput)
        {
            string currentLayerType = string.Empty;
            int runwayCount = 0, boundaryCount = 0, taxiwayCount = 0, buildingCount = 0, apronCount = 0;

            // Split the tflOutput string by newlines to simulate reading lines from a file
            string[] lines = tflOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (line.StartsWith("//"))
                {
                    string layerType = IdentifyLayerType(line);
                    switch (layerType)
                    {
                        case "RUNWAY": currentLayerType = $"RUNWAY_{++runwayCount}"; break;
                        case "BOUNDARY": currentLayerType = $"BOUNDARY_{++boundaryCount}"; break;
                        case "TAXIWAY": currentLayerType = $"TAXIWAY_{++taxiwayCount}"; break;
                        case "BUILDING": currentLayerType = $"BUILDING_{++buildingCount}"; break;
                        case "APRON": currentLayerType = $"APRON_{++apronCount}"; break;
                        default: currentLayerType = layerType; break;
                    }

                    if (!layerPoints.ContainsKey(currentLayerType))
                    {
                        layerPoints[currentLayerType] = new List<PointF>();
                        layerOrder.Add(currentLayerType);
                    }
                    continue;
                }

                if (line.StartsWith("STATIC") || line.StartsWith("DYNAMIC"))
                {
                    string[] parts = line.Split(';');
                    if (parts.Length >= 4)
                    {
                        string fillColorCode = parts[1].Trim();
                        float strokeWidth = float.TryParse(parts[2], out float result) ? result : 1.0f;
                        string strokeColorCode = parts[3].Trim();

                        Pen strokeColor = new Pen(GetLayerColor(strokeColorCode));
                        Brush fillColor = new SolidBrush(GetLayerColor(fillColorCode));

                        if (!layerStyles.ContainsKey(currentLayerType))
                        {
                            layerStyles[currentLayerType] = (fillColor, strokeWidth, strokeColor);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(currentLayerType))
                {
                    var coordinates = ExtractCoordinates(line);
                    if (coordinates != null)
                    {
                        layerPoints[currentLayerType].Add(coordinates.Value);
                    }
                }
            }

            // Redraw after processing
            plotCanvas.Invalidate();
        }
        // Function to process GEO data and start new segments based on empty or comment lines
        private void ProcessGEOFile(string geoOutput)
        {
            string currentLayerType = "TAXI_CENTER"; // Type of the layer (can be changed based on your needs)
            string[] lines = geoOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.None); // Split input into lines

            List<PointF> currentLayerPoints = new List<PointF>(); // List to store points for the current segment
            Pen strokeColor = null; // Declare a Pen object to store the color for the segment lines

            // Iterate through each line in the geoOutput
            foreach (var line in lines)
            {
                string trimmedLine = line.Trim(); // Trim leading/trailing spaces for accurate checking

                // Check if the line is empty or a comment
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("//"))
                {
                    // If we have points in the current segment, finalize and add it to the layer
                    if (currentLayerPoints.Count > 0)
                    {
                        AddSegmentToLayer(currentLayerType, currentLayerPoints, strokeColor);
                        currentLayerPoints.Clear(); // Clear the points for the next segment
                    }
                    continue; // Skip this line and move to the next one
                }

                // Process lines with valid data (coordinates and color)
                var parts = trimmedLine.Split(';');
                if (parts.Length >= 5)
                {
                    // Extract coordinates (start and end points)
                    var startCoordinates = ExtractCoordinates($"{parts[0]};{parts[1]}");
                    var endCoordinates = ExtractCoordinates($"{parts[2]};{parts[3]}");

                    if (startCoordinates != null && endCoordinates != null)
                    {
                        // Add the start and end points to the current segment
                        currentLayerPoints.Add(startCoordinates.Value);
                        currentLayerPoints.Add(endCoordinates.Value);

                        // Store the color for the segment if not already done
                        if (strokeColor == null)
                        {
                            strokeColor = new Pen(GetLayerColor(parts[4].Trim())); // Get the color based on GEO data
                        }
                    }
                }
            }

            // After processing all lines, ensure the last segment is added (if there were no empty lines at the end)
            if (currentLayerPoints.Count > 0)
            {
                AddSegmentToLayer(currentLayerType, currentLayerPoints, strokeColor);
            }

            // Redraw the canvas to reflect the changes
            plotCanvas.Invalidate();
        }
        // Declare the layerColors dictionary at the class level
        private Dictionary<string, Color> layerColors = new Dictionary<string, Color>();

        public void ProcessTXIFile(string txiOutput)
        {
            string[] lines = txiOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.None); // Split into lines

            foreach (var line in lines)
            {
                string trimmedLine = line.Trim(); // Trim spaces
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("//"))
                {
                    continue; // Skip empty or comment lines
                }

                var parts = trimmedLine.Split(';');
                if (parts.Length >= 4)
                {
                    string label = parts[0].Trim(); // Taxi label (e.g., "A", "B")
                    string airportIcao = parts[1].Trim(); // ICAO code (not used for drawing)
                    var coordinates = ExtractCoordinates($"{parts[2]};{parts[3]}");

                    if (coordinates != null)
                    {
                        // Ensure we are storing each label with its coordinates correctly
                        if (!layerPoints.ContainsKey($"TXI_{label}"))
                        {
                            layerPoints[$"TXI_{label}"] = new List<PointF>(); // Initialize list if not already present
                        }

                        // Add each point for the label
                        layerPoints[$"TXI_{label}"].Add(coordinates.Value);

                        // Get the color for the label from predefinedColors dictionary
                        string colorName = predefinedColors.ContainsKey("TXI") ? predefinedColors["TXI"] : "White"; // Default to white if not found

                        // Convert the named color string to a Color object using FromName (for standard colors like "Yellow", "Red")
                        Color labelColor = Color.FromName(colorName);

                        // In case the name isn't recognized, you could fallback to a default color
                        if (labelColor.IsKnownColor == false)
                        {
                            labelColor = Color.White; // Fallback to white if the color is not known
                        }

                        // Store the color for later use in drawing (each label will get its own color)
                        if (!layerColors.ContainsKey($"TXI_{label}"))
                        {
                            layerColors[$"TXI_{label}"] = labelColor; // Store color for each label
                        }

                        // Add unique label to the order list if it doesn't exist already
                        if (!layerOrder.Contains($"TXI_{label}"))
                        {
                            layerOrder.Add($"TXI_{label}");
                        }
                    }
                }
            }

            plotCanvas.Invalidate(); // Redraw after processing the taxiway labels
        }


        // Helper method to add a segment to the layer and draw it
        private void AddSegmentToLayer(string layerType, List<PointF> points, Pen strokeColor)
        {
            // Add points to the corresponding layer
            if (!layerPoints.ContainsKey(layerType))
            {
                layerPoints[layerType] = new List<PointF>(); // Initialize list if not already present
                layerOrder.Add(layerType); // Add layer type to the order list
            }
            layerPoints[layerType].AddRange(points); // Add all points of the current segment

            // Store the style for the layer (this is used when drawing the segment)
            if (!layerStyles.ContainsKey(layerType))
            {
                layerStyles[layerType] = (Brushes.Transparent, 2.0f, strokeColor); // Set default style (transparent fill, 2px line width)
            }

            // Use the DrawLayer function to draw the current segment on the canvas
            DrawLayer(plotCanvas.CreateGraphics(), layerType, points, scale: 1, offsetX: 0, offsetY: 0, globalMinX: 0, globalMinY: 0, style: (Brushes.Transparent, 2.0f, strokeColor));
        }

        private void DrawLayer(Graphics g, string layerType, List<PointF> points, float scale, float offsetX, float offsetY, float globalMinX, float globalMinY, (Brush fill, float strokeWidth, Pen stroke) style)
        {
            if (layerType.StartsWith("TXI_"))
            {
                // Draw text for taxi labels (no lines, just text)
                foreach (var point in points)
                {
                    string label = layerType.Substring(4); // Get the label (excluding 'TXI_')
                    PointF transformedPoint = new PointF((point.X - globalMinX) * scale + offsetX, (point.Y - globalMinY) * scale + offsetY);
                    g.DrawString(label, new Font("Arial", 10), Brushes.Black, transformedPoint); // Draw label text in black
                }
            }
            else
            {
                // For layers like GEO, TFL, etc., draw polygons or lines
                if (points.Count > 1)
                {
                    PointF[] transformedPoints = points.Select(p => new PointF((p.X - globalMinX) * scale + offsetX, (p.Y - globalMinY) * scale + offsetY)).ToArray();

                    if (style.fill != Brushes.Transparent)
                    {
                        g.FillPolygon(style.fill, transformedPoints); // Fill polygons (for areas like Taxiway)
                    }

                    g.DrawPolygon(style.stroke, transformedPoints); // Draw stroke (for lines)
                }
                else if (points.Count == 2)
                {
                    g.DrawLine(style.stroke, points[0].X * scale + offsetX, points[0].Y * scale + offsetY, points[1].X * scale + offsetX, points[1].Y * scale + offsetY);
                }
            }
        }



        private Color GetLayerColor(string colorKey)
        {
            if (Regex.IsMatch(colorKey, "^#[0-9A-Fa-f]{8}$"))
            {
                // Extract the alpha, red, green, and blue components
                int alpha = Convert.ToInt32(colorKey.Substring(1, 2), 16);
                int red = Convert.ToInt32(colorKey.Substring(3, 2), 16);
                int green = Convert.ToInt32(colorKey.Substring(5, 2), 16);
                int blue = Convert.ToInt32(colorKey.Substring(7, 2), 16);
                return Color.FromArgb(alpha, red, green, blue); // Include alpha for transparency
            }

            if (Regex.IsMatch(colorKey, "^#[0-9A-Fa-f]{6}$"))
            {
                // Handle solid colors (without opacity)
                return ColorTranslator.FromHtml(colorKey);
            }

            if (predefinedColors.TryGetValue(colorKey, out string colorString))
            {
                return ColorTranslator.FromHtml(colorString);
            }

            return Color.Transparent;
        }


        private string IdentifyLayerType(string line)
        {
            if (line.Contains("Boundary", StringComparison.OrdinalIgnoreCase)) return "BOUNDARY";
            if (line.Contains("Taxiway", StringComparison.OrdinalIgnoreCase)) return "TAXIWAY";
            if (line.Contains("Runway", StringComparison.OrdinalIgnoreCase)) return "RUNWAY";
            if (line.Contains("Building", StringComparison.OrdinalIgnoreCase)) return "BUILDING";
            if (line.Contains("Apron", StringComparison.OrdinalIgnoreCase)) return "APRON";
            return string.Empty;
        }

        private PointF? ExtractCoordinates(string line)
        {
            // Regex to match coordinates with N/S for latitude and E/W for longitude
            var match = Regex.Match(line, @"([NS])(\d{1,3})\.(\d{2})\.(\d{2})\.(\d{3});([EW])(\d{1,3})\.(\d{2})\.(\d{2})\.(\d{3})");

            if (match.Success)
            {
                // Extracting the DMS components for latitude and longitude
                string latDirection = match.Groups[1].Value;  // N/S
                string latDegrees = match.Groups[2].Value;
                string latMinutes = match.Groups[3].Value;
                string latSeconds = match.Groups[4].Value;
                string latMilliseconds = match.Groups[5].Value;

                string lonDirection = match.Groups[6].Value;  // E/W
                string lonDegrees = match.Groups[7].Value;
                string lonMinutes = match.Groups[8].Value;
                string lonSeconds = match.Groups[9].Value;
                string lonMilliseconds = match.Groups[10].Value;

                // Convert to decimal degrees
                float lat = ConvertDMS(latDirection, latDegrees, latMinutes, latSeconds, latMilliseconds);
                float lon = ConvertDMS(lonDirection, lonDegrees, lonMinutes, lonSeconds, lonMilliseconds);

                // Return coordinates as PointF (longitude, latitude)
                return new PointF(lon, -lat);  // We keep lat as it is, longitude can be negative for W
            }

            return null;  // Return null if the regex does not match
        }

        private float ConvertDMS(string direction, string degrees, string minutes, string seconds, string milliseconds)
        {
            // Parse the degree, minute, second, and millisecond values to floats
            float deg = Convert.ToSingle(degrees);
            float min = Convert.ToSingle(minutes);
            float sec = Convert.ToSingle(seconds);
            float milli = Convert.ToSingle(milliseconds);

            // Convert to decimal degrees
            float decimalDegrees = deg + (min / 60) + (sec / 3600) + (milli / 3600000);

            // Adjust for N/S and E/W directions
            if ((direction == "S" || direction == "W") && !string.IsNullOrEmpty(direction))
            {
                decimalDegrees = -decimalDegrees;  // Negative for South and West
            }

            return decimalDegrees;
        }

        private void plotCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Clear the background with the same color as the panel's background
            g.Clear(Color.FromArgb(0, 24, 36));

            // Define the center offset based on the canvas size
            float centerX = plotCanvas.Width / 2;
            float centerY = plotCanvas.Height / 2;

            // Draw layers from TFL data
            DrawTFLData(g, centerX, centerY);
        }

        private void DrawTFLData(Graphics g, float centerX, float centerY)
        {
            // Find the global min/max for all coordinates
            float globalMinX = layerPoints.Values.SelectMany(points => points).Min(p => p.X);
            float globalMinY = layerPoints.Values.SelectMany(points => points).Min(p => p.Y);
            float globalMaxX = layerPoints.Values.SelectMany(points => points).Max(p => p.X);
            float globalMaxY = layerPoints.Values.SelectMany(points => points).Max(p => p.Y);

            // Calculate scale factor and offsets based on the global min/max values
            float width = globalMaxX - globalMinX;
            float height = globalMaxY - globalMinY;

            scaleFactor = Math.Min(
                (float)(plotCanvas.Width / width) * (float)zoomFactor,
                (float)(plotCanvas.Height / height) * (float)zoomFactor
            );

            float offsetX = (float)(plotCanvas.Width - (width * scaleFactor)) / 2;
            float offsetY = (float)(plotCanvas.Height - (height * scaleFactor)) / 2;

            // Draw each layer
            foreach (var layer in layerOrder)
            {
                var style = layerStyles.ContainsKey(layer) ? layerStyles[layer] : (Brushes.Transparent, 1.0f, new Pen(Color.Black));
                DrawLayer(g, layer, layerPoints[layer], (float)scaleFactor, offsetX, offsetY, globalMinX, globalMinY, style);
            }
        }

        // Mouse wheel for zooming
        private void plotCanvas_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                zoomFactor += ZoomIncrement;  // Zoom In
            else if (e.Delta < 0)
                zoomFactor = Math.Max(zoomFactor - ZoomIncrement, ZoomIncrement);  // Zoom Out

            plotCanvas.Invalidate();  // Redraw
        }

        // Mouse down for panning
        private void plotCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isPanning = true;
                previousMousePosition = e.Location;
            }
        }

        // Mouse move for panning
        private void plotCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPanning)
            {
                var dx = e.X - previousMousePosition.X;
                var dy = e.Y - previousMousePosition.Y;

                // Adjust the canvas offset here (for simplicity, this part can be improved)
                // For example, you could adjust a global offset variable.

                plotCanvas.Invalidate();  // Redraw after move
                previousMousePosition = e.Location;
            }
        }

        // Mouse up to stop panning
        private void plotCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isPanning = false;
            }
        }
    }
}