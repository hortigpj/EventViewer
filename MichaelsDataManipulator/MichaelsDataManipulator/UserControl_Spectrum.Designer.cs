namespace MichaelsDataManipulator
{
    partial class UserControl_Spectrum
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox_spectrum_canvas = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel_spectrum = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.radButton_draw_spectrum = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_spectrum_canvas)).BeginInit();
            this.tableLayoutPanel_spectrum.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radButton_draw_spectrum)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_spectrum_canvas
            // 
            this.pictureBox_spectrum_canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_spectrum_canvas.Location = new System.Drawing.Point(3, 3);
            this.pictureBox_spectrum_canvas.Name = "pictureBox_spectrum_canvas";
            this.pictureBox_spectrum_canvas.Size = new System.Drawing.Size(1264, 560);
            this.pictureBox_spectrum_canvas.TabIndex = 0;
            this.pictureBox_spectrum_canvas.TabStop = false;
            this.pictureBox_spectrum_canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_spectrum_canvas_Paint);
            // 
            // tableLayoutPanel_spectrum
            // 
            this.tableLayoutPanel_spectrum.ColumnCount = 1;
            this.tableLayoutPanel_spectrum.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_spectrum.Controls.Add(this.pictureBox_spectrum_canvas, 0, 0);
            this.tableLayoutPanel_spectrum.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel_spectrum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_spectrum.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_spectrum.Name = "tableLayoutPanel_spectrum";
            this.tableLayoutPanel_spectrum.RowCount = 2;
            this.tableLayoutPanel_spectrum.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 92.64706F));
            this.tableLayoutPanel_spectrum.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.352941F));
            this.tableLayoutPanel_spectrum.Size = new System.Drawing.Size(1270, 612);
            this.tableLayoutPanel_spectrum.TabIndex = 1;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.radButton_draw_spectrum);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 569);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1264, 40);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // radButton_draw_spectrum
            // 
            this.radButton_draw_spectrum.Location = new System.Drawing.Point(3, 3);
            this.radButton_draw_spectrum.Name = "radButton_draw_spectrum";
            this.radButton_draw_spectrum.Size = new System.Drawing.Size(110, 24);
            this.radButton_draw_spectrum.TabIndex = 0;
            this.radButton_draw_spectrum.Text = "radButton1";
            this.radButton_draw_spectrum.Click += new System.EventHandler(this.radButton_draw_spectrum_Click);
            // 
            // UserControl_Spectrum
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel_spectrum);
            this.Name = "UserControl_Spectrum";
            this.Size = new System.Drawing.Size(1270, 612);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_spectrum_canvas)).EndInit();
            this.tableLayoutPanel_spectrum.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radButton_draw_spectrum)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_spectrum_canvas;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_spectrum;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private Telerik.WinControls.UI.RadButton radButton_draw_spectrum;
    }
}
