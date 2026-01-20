using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sector_File
{

    public partial class flightOperationOptions : Form
    {
        private int userId;

        public flightOperationOptions(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            this.Icon = new Icon("./tools.ico");
        }

        private void getSchedulesButton_Click(object sender, EventArgs e)
        {
            // Initialize the new form
            flightSechedulesData schedulesForm = new flightSechedulesData(  userId);
            this.Hide();
            schedulesForm.ShowDialog();
            this.Show(); // Show the main form again after closing the FIR Data form
        }
    }
}
