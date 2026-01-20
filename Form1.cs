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
            this.Icon = new Icon("./tools.ico");
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
