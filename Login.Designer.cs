namespace Sector_File
{
    partial class Login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.LoginButton = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // 
            // LoginButton
            // 
            this.LoginButton.Size = new System.Drawing.Size(200, 50); // Set the size of the button
            this.LoginButton.Text = "Login with IVAO SSO"; // Set button text
            this.LoginButton.UseVisualStyleBackColor = true;
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click); // Add event handler for button click

            // Center the button in the form
            this.LoginButton.Location = new System.Drawing.Point(
                (this.ClientSize.Width - this.LoginButton.Width) / 2,
                (this.ClientSize.Height - this.LoginButton.Height) / 2
            );

            // 
            // Login Form
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 150); // Form size
            this.Controls.Add(this.LoginButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle; // Prevent resizing
            this.MaximizeBox = false; // Disable maximize button
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen; // Center the form on screen
            this.Text = "Login | IVAO Utilities";
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button LoginButton; // Declare the button
    }
}
