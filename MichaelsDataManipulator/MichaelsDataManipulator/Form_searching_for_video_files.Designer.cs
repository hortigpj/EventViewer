namespace MichaelsDataManipulator
{
    partial class Form_searching_for_video_files
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
            this.radProgressBar_search_for_video_files_drive_letters = new Telerik.WinControls.UI.RadProgressBar();
            this.radProgressBar_search_for_video_files_files = new Telerik.WinControls.UI.RadProgressBar();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.radProgressBar_search_for_video_files_drive_letters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radProgressBar_search_for_video_files_files)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            this.SuspendLayout();
            // 
            // radProgressBar_search_for_video_files_drive_letters
            // 
            this.radProgressBar_search_for_video_files_drive_letters.Location = new System.Drawing.Point(12, 37);
            this.radProgressBar_search_for_video_files_drive_letters.Name = "radProgressBar_search_for_video_files_drive_letters";
            this.radProgressBar_search_for_video_files_drive_letters.Size = new System.Drawing.Size(517, 24);
            this.radProgressBar_search_for_video_files_drive_letters.TabIndex = 0;
            // 
            // radProgressBar_search_for_video_files_files
            // 
            this.radProgressBar_search_for_video_files_files.Location = new System.Drawing.Point(13, 67);
            this.radProgressBar_search_for_video_files_files.Name = "radProgressBar_search_for_video_files_files";
            this.radProgressBar_search_for_video_files_files.Size = new System.Drawing.Size(517, 24);
            this.radProgressBar_search_for_video_files_files.TabIndex = 0;
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(13, 13);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(126, 18);
            this.radLabel1.TabIndex = 1;
            this.radLabel1.Text = "Searching for video files";
            // 
            // Form_searching_for_video_files
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 103);
            this.ControlBox = false;
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.radProgressBar_search_for_video_files_files);
            this.Controls.Add(this.radProgressBar_search_for_video_files_drive_letters);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_searching_for_video_files";
            this.Text = "Form_wait";
            ((System.ComponentModel.ISupportInitialize)(this.radProgressBar_search_for_video_files_drive_letters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radProgressBar_search_for_video_files_files)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Telerik.WinControls.UI.RadLabel radLabel1;
        public Telerik.WinControls.UI.RadProgressBar radProgressBar_search_for_video_files_drive_letters;
        public Telerik.WinControls.UI.RadProgressBar radProgressBar_search_for_video_files_files;
    }
}