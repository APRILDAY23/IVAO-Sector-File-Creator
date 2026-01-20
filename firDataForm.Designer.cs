namespace Sector_File
{
    partial class firDataForm
    {
        private System.ComponentModel.IContainer components = null;
        private ProgressBar loadingProgressBar;  // Progress bar for loading status
        private RichTextBox logRichTextBox;       // Debug output box
        private TextBox searchBox;                // Search input box
        private Button searchButton;              // Search button
        private Button downloadWebeyeShapes;         // Download SID button
        private Button downloadFIRBoundary;        // Download STAR button
        private Button backButton;                // Go back button

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            loadingProgressBar = new ProgressBar();
            logRichTextBox = new RichTextBox();
            searchBox = new TextBox();
            searchButton = new Button();
            downloadWebeyeShapes = new Button();
            downloadFIRBoundary = new Button();
            backButton = new Button();
            SuspendLayout();
            // 
            // loadingProgressBar
            // 
            loadingProgressBar.Dock = DockStyle.Top;
            loadingProgressBar.Location = new Point(0, 0);
            loadingProgressBar.Name = "loadingProgressBar";
            loadingProgressBar.Size = new Size(550, 30);
            loadingProgressBar.TabIndex = 6;
            // 
            // logRichTextBox
            // 
            logRichTextBox.Dock = DockStyle.Fill;
            logRichTextBox.Location = new Point(0, 97);
            logRichTextBox.Name = "logRichTextBox";
            logRichTextBox.ReadOnly = true;
            logRichTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            logRichTextBox.Size = new Size(550, 183);
            logRichTextBox.TabIndex = 0;
            logRichTextBox.Text = "";
            // 
            // searchBox
            // 
            searchBox.Dock = DockStyle.Top;
            searchBox.Location = new Point(0, 30);
            searchBox.Name = "searchBox";
            searchBox.Size = new Size(550, 27);
            searchBox.TabIndex = 5;
            // 
            // searchButton
            // 
            searchButton.Dock = DockStyle.Top;
            searchButton.Location = new Point(0, 57);
            searchButton.Name = "searchButton";
            searchButton.Size = new Size(550, 40);
            searchButton.TabIndex = 4;
            searchButton.Text = "Search";
            searchButton.UseVisualStyleBackColor = true;
            // 
            // downloadWebeyeShapes
            // 
            downloadWebeyeShapes.Dock = DockStyle.Bottom;
            downloadWebeyeShapes.Location = new Point(0, 320);
            downloadWebeyeShapes.Name = "downloadWebeyeShapes";
            downloadWebeyeShapes.Size = new Size(550, 40);
            downloadWebeyeShapes.TabIndex = 2;
            downloadWebeyeShapes.Text = "Download Webeye Shapes File";
            downloadWebeyeShapes.UseVisualStyleBackColor = true;
            downloadWebeyeShapes.Click += downloadWebeyeShapes_Click;
            // 
            // downloadFIRBoundary
            // 
            downloadFIRBoundary.Dock = DockStyle.Bottom;
            downloadFIRBoundary.Location = new Point(0, 280);
            downloadFIRBoundary.Name = "downloadFIRBoundary";
            downloadFIRBoundary.Size = new Size(550, 40);
            downloadFIRBoundary.TabIndex = 1;
            downloadFIRBoundary.Text = "Download FIR Boundary File";
            downloadFIRBoundary.UseVisualStyleBackColor = true;
            // 
            // backButton
            // 
            backButton.Dock = DockStyle.Bottom;
            backButton.Location = new Point(0, 360);
            backButton.Name = "backButton";
            backButton.Size = new Size(550, 40);
            backButton.TabIndex = 3;
            backButton.Text = "Back to Main Menu";
            backButton.UseVisualStyleBackColor = true;
            backButton.Click += backButton_Click;
            // 
            // firDataForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(550, 400);
            Controls.Add(logRichTextBox);
            Controls.Add(downloadFIRBoundary);
            Controls.Add(downloadWebeyeShapes);
            Controls.Add(backButton);
            Controls.Add(searchButton);
            Controls.Add(searchBox);
            Controls.Add(loadingProgressBar);
            MaximizeBox = false;
            Name = "firDataForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FIR Data | IVAO ATC Utilities";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
