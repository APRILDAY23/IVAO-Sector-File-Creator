using Microsoft.VisualBasic.ApplicationServices;
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
    public partial class Choose : Form
    {
        private int userId;

        public Choose(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            this.Icon = new Icon("./tools.ico");
            this.FormClosed += (s, e) => Application.Exit(); // Close the application when this form is closed
        }

        // Event handler for the button click to choose KML
        private void flightOperationsButton_Click(object sender, EventArgs e)
        {

            flightOperationOptions flightOperationOptions = new flightOperationOptions(userId);
            this.Hide();
            flightOperationOptions.ShowDialog();
            this.Show();
        }

        // Event handler for the button to get country data
        private void atcOperationsButton_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1(userId);
            this.Hide();
            form1.ShowDialog();
            this.Show();
        }
    }
}
