using System;
using System.Windows.Forms;

namespace Sector_File
{
    public partial class Form1 : Form
    {
        private int userId;

        public Form1(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            try { this.Icon = new Icon("./tools.ico"); } catch { }
            this.Load += (s, e) => LayoutToolCards();
        }

        // Lays out 5 tool cards: 3 on row 1, 2 centred on row 2
        // Called on Load and on gridPanel.Resize so nothing is hardcoded.
        private void LayoutToolCards()
        {
            int w       = gridPanel.ClientSize.Width;
            int pad     = 18;
            int cardW   = (w - pad * 4) / 3;
            int cardH   = 158;
            int row1Y   = 20;
            int row2Y   = row1Y + cardH + pad;

            var row1 = new[] { sidStarCard, airportCard, firCard };
            for (int i = 0; i < row1.Length; i++)
            {
                row1[i].Location = new System.Drawing.Point(pad + i * (cardW + pad), row1Y);
                row1[i].Size     = new System.Drawing.Size(cardW, cardH);
            }

            // Row 2: 2 cards centred
            int row2TotalW = 2 * cardW + pad;
            int row2StartX = (w - row2TotalW) / 2;
            countryCard.Location = new System.Drawing.Point(row2StartX, row2Y);
            countryCard.Size     = new System.Drawing.Size(cardW, cardH);
            kmlCard.Location     = new System.Drawing.Point(row2StartX + cardW + pad, row2Y);
            kmlCard.Size         = new System.Drawing.Size(cardW, cardH);
        }

        // Event handler for the button click to choose KML
        private void chooseKmlButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "KML Files (*.kml)|*.kml|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                if (!filePath.EndsWith(".kml", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Error: Please upload only KML files.", "Invalid File Type", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.Hide();
                LoadingForm loadingForm = new LoadingForm(filePath, userId);
                loadingForm.ShowDialog();
                this.Show();
            }
        }

        // Event handler for the button to get country data
        private void getCountryDataButton_Click(object sender, EventArgs e)
        {
            CountryDataForm countryDataForm = new CountryDataForm(userId);
            this.Hide();
            countryDataForm.ShowDialog();
            this.Show();
        }

        // Event handler for the new button to get SID and STAR data
        private void getSidAndStarDataButton_Click(object sender, EventArgs e)
        {
            SidStarDataForm sidstarDataForm = new SidStarDataForm(userId);
            this.Hide();
            sidstarDataForm.ShowDialog();
            this.Show();
        }

        private void getFirDataButton_Click(object sender, EventArgs e)
        {
            firDataForm firDataForm = new firDataForm(userId); // Initialize FIR Data form
            this.Hide();
            firDataForm.ShowDialog(); // Show the FIR Data form
            this.Show(); // Show the main form again after closing the FIR Data form
        }


        private void getAirportDataButton_Click(object sender, EventArgs e)
        {
            airportDataForm airportDataForm = new airportDataForm(userId); // Initialize FIR Data form
            this.Hide();
            airportDataForm.ShowDialog(); // Show the FIR Data form
            this.Show(); // Show the main form again after closing the FIR Data form
        }
    }
}
