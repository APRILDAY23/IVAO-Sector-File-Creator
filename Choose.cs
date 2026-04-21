using System;
using System.Windows.Forms;

namespace Sector_File
{
    public partial class Choose : Form
    {
        private int    userId;
        private string firstName;
        private string lastName;

        public Choose(int userId, string firstName = "", string lastName = "")
        {
            this.userId    = userId;
            this.firstName = firstName;
            this.lastName  = lastName;

            InitializeComponent();
            try { this.Icon = new System.Drawing.Icon("./tools.ico"); } catch { }
            this.FormClosed += (s, e) => Application.Exit();

            // Personalise the welcome greeting
            string name = string.IsNullOrEmpty(firstName) ? $"#{userId}" : $"{firstName} {lastName}".Trim();
            welcomeLabel.Text = $"Welcome back, {name}";
            userIdLabel.Text  = $"IVAO ID: {userId}";
        }

        private void flightOperationsButton_Click(object sender, EventArgs e)
        {
            var form = new flightOperationOptions(userId);
            this.Hide();
            form.ShowDialog();
            this.Show();
        }

        private void atcOperationsButton_Click(object sender, EventArgs e)
        {
            var form = new Form1(userId);
            this.Hide();
            form.ShowDialog();
            this.Show();
        }

        private void creditsButton_Click(object sender, EventArgs e)
        {
            new CreditsForm().ShowDialog(this);
        }

        // Recalculate card positions so they always fill the available width evenly
        private void LayoutCards()
        {
            int w         = cardArea.ClientSize.Width;
            int padding   = 20;
            int cardW     = (w - padding * 3) / 2;
            int cardH     = 200;
            int top       = 24;

            flightCard.Location = new System.Drawing.Point(padding, top);
            flightCard.Size     = new System.Drawing.Size(cardW, cardH);

            atcCard.Location    = new System.Drawing.Point(padding * 2 + cardW, top);
            atcCard.Size        = new System.Drawing.Size(cardW, cardH);
        }
    }
}
