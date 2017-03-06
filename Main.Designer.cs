namespace Interactive_Photobooth
{
    partial class Main
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
            this.SuspendLayout();
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.MaximumSize = new System.Drawing.Size(1920, 1080);

            //TODO: Change to "System.Windows.Forms.FormBorderStyle.None" for production
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            
            //TODO: Change to 1920x1080 for production
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.MinimumSize = new System.Drawing.Size(640, 480);

            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = "Interactive Photobooth - Group 13";
            this.ResumeLayout(false);

        }

        #endregion
    }
}

