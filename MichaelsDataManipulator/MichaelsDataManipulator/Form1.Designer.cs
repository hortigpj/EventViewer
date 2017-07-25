namespace MichaelsDataManipulator
{
    partial class Form1
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
            Telerik.WinControls.UI.CartesianArea cartesianArea1 = new Telerik.WinControls.UI.CartesianArea();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn1 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn2 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn3 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn4 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn2 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewDateTimeColumn gridViewDateTimeColumn1 = new Telerik.WinControls.UI.GridViewDateTimeColumn();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn3 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn5 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition1 = new Telerik.WinControls.UI.TableViewDefinition();
            this.radChartView_speed_over_time = new Telerik.WinControls.UI.RadChartView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.radPageView1 = new Telerik.WinControls.UI.RadPageView();
            this.radPageViewPage1 = new Telerik.WinControls.UI.RadPageViewPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.radTrackBar_chart_samples = new Telerik.WinControls.UI.RadTrackBar();
            this.radPageViewPage2 = new Telerik.WinControls.UI.RadPageViewPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.radGridView_events = new Telerik.WinControls.UI.RadGridView();
            this.eventsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.radSplitContainer1 = new Telerik.WinControls.UI.RadSplitContainer();
            this.splitPanel1 = new Telerik.WinControls.UI.SplitPanel();
            this.radPropertyGrid1 = new Telerik.WinControls.UI.RadPropertyGrid();
            this.splitPanel2 = new Telerik.WinControls.UI.SplitPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.radListView_data_files = new Telerik.WinControls.UI.RadListView();
            this.radProgressBar_ReadData = new Telerik.WinControls.UI.RadProgressBar();
            this.radButton_StartRead = new Telerik.WinControls.UI.RadButton();
            this.eventDatabaseDataSetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.eventDatabaseDataSet = new MichaelsDataManipulator.EventDatabaseDataSet();
            this.eventsTableAdapter = new MichaelsDataManipulator.EventDatabaseDataSetTableAdapters.EventsTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.radChartView_speed_over_time)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radPageView1)).BeginInit();
            this.radPageView1.SuspendLayout();
            this.radPageViewPage1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radTrackBar_chart_samples)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView_events)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView_events.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radSplitContainer1)).BeginInit();
            this.radSplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel1)).BeginInit();
            this.splitPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radPropertyGrid1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel2)).BeginInit();
            this.splitPanel2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radListView_data_files)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radProgressBar_ReadData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton_StartRead)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventDatabaseDataSetBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventDatabaseDataSet)).BeginInit();
            this.SuspendLayout();
            // 
            // radChartView_speed_over_time
            // 
            cartesianArea1.ShowGrid = true;
            this.radChartView_speed_over_time.AreaDesign = cartesianArea1;
            this.radChartView_speed_over_time.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radChartView_speed_over_time.Location = new System.Drawing.Point(3, 3);
            this.radChartView_speed_over_time.Name = "radChartView_speed_over_time";
            this.radChartView_speed_over_time.ShowLegend = true;
            this.radChartView_speed_over_time.ShowPanZoom = true;
            this.radChartView_speed_over_time.ShowToolTip = true;
            this.radChartView_speed_over_time.ShowTrackBall = true;
            this.radChartView_speed_over_time.Size = new System.Drawing.Size(875, 591);
            this.radChartView_speed_over_time.TabIndex = 0;
            this.radChartView_speed_over_time.Text = "radChartView1";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 800F));
            this.tableLayoutPanel1.Controls.Add(this.radPageView1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 688F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1708, 688);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // radPageView1
            // 
            this.radPageView1.Controls.Add(this.radPageViewPage1);
            this.radPageView1.Controls.Add(this.radPageViewPage2);
            this.radPageView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPageView1.Location = new System.Drawing.Point(3, 3);
            this.radPageView1.Name = "radPageView1";
            this.radPageView1.SelectedPage = this.radPageViewPage1;
            this.radPageView1.Size = new System.Drawing.Size(902, 682);
            this.radPageView1.TabIndex = 2;
            this.radPageView1.Text = "radPageView1";
            // 
            // radPageViewPage1
            // 
            this.radPageViewPage1.Controls.Add(this.tableLayoutPanel2);
            this.radPageViewPage1.ItemSize = new System.Drawing.SizeF(98F, 24F);
            this.radPageViewPage1.Location = new System.Drawing.Point(10, 33);
            this.radPageViewPage1.Name = "radPageViewPage1";
            this.radPageViewPage1.Size = new System.Drawing.Size(881, 638);
            this.radPageViewPage1.Text = "Speed Over Time";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.radChartView_speed_over_time, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.radTrackBar_chart_samples, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(881, 638);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // radTrackBar_chart_samples
            // 
            this.radTrackBar_chart_samples.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radTrackBar_chart_samples.LargeTickFrequency = 500;
            this.radTrackBar_chart_samples.Location = new System.Drawing.Point(3, 600);
            this.radTrackBar_chart_samples.Maximum = 1000F;
            this.radTrackBar_chart_samples.Minimum = 19F;
            this.radTrackBar_chart_samples.Name = "radTrackBar_chart_samples";
            this.radTrackBar_chart_samples.Size = new System.Drawing.Size(875, 34);
            this.radTrackBar_chart_samples.SmallTickFrequency = 10;
            this.radTrackBar_chart_samples.TabIndex = 3;
            this.radTrackBar_chart_samples.Text = "radTrackBar1";
            this.radTrackBar_chart_samples.Value = 100F;
            this.radTrackBar_chart_samples.ValueChanged += new System.EventHandler(this.radTrackBar_chart_samples_ValueChanged);
            // 
            // radPageViewPage2
            // 
            this.radPageViewPage2.ItemSize = new System.Drawing.SizeF(105F, 24F);
            this.radPageViewPage2.Location = new System.Drawing.Point(10, 37);
            this.radPageViewPage2.Name = "radPageViewPage2";
            this.radPageViewPage2.Size = new System.Drawing.Size(881, 634);
            this.radPageViewPage2.Text = "Speed Distribution";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.radGridView_events, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.radSplitContainer1, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(911, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(794, 682);
            this.tableLayoutPanel3.TabIndex = 7;
            // 
            // radGridView_events
            // 
            this.radGridView_events.AutoSizeRows = true;
            this.radGridView_events.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radGridView_events.Location = new System.Drawing.Point(3, 344);
            // 
            // 
            // 
            this.radGridView_events.MasterTemplate.AllowColumnChooser = false;
            this.radGridView_events.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
            gridViewDecimalColumn1.DataType = typeof(int);
            gridViewDecimalColumn1.FieldName = "ID";
            gridViewDecimalColumn1.HeaderText = "ID";
            gridViewDecimalColumn1.IsAutoGenerated = true;
            gridViewDecimalColumn1.Name = "ID";
            gridViewDecimalColumn1.Width = 85;
            gridViewTextBoxColumn1.FieldName = "FILENAME";
            gridViewTextBoxColumn1.HeaderText = "FILENAME";
            gridViewTextBoxColumn1.IsAutoGenerated = true;
            gridViewTextBoxColumn1.Name = "FILENAME";
            gridViewTextBoxColumn1.Width = 85;
            gridViewTextBoxColumn2.FieldName = "DATA_FILENAME";
            gridViewTextBoxColumn2.HeaderText = "DATA_FILENAME";
            gridViewTextBoxColumn2.IsAutoGenerated = true;
            gridViewTextBoxColumn2.Name = "DATA_FILENAME";
            gridViewTextBoxColumn2.Width = 85;
            gridViewTextBoxColumn3.FieldName = "VIDEO_FILENAME";
            gridViewTextBoxColumn3.HeaderText = "VIDEO_FILENAME";
            gridViewTextBoxColumn3.IsAutoGenerated = true;
            gridViewTextBoxColumn3.Name = "VIDEO_FILENAME";
            gridViewTextBoxColumn3.Width = 85;
            gridViewTextBoxColumn4.FieldName = "CONVEYOR";
            gridViewTextBoxColumn4.HeaderText = "CONVEYOR";
            gridViewTextBoxColumn4.IsAutoGenerated = true;
            gridViewTextBoxColumn4.Name = "CONVEYOR";
            gridViewTextBoxColumn4.Width = 85;
            gridViewDecimalColumn2.DataType = typeof(int);
            gridViewDecimalColumn2.FieldName = "EVENT_INDEX";
            gridViewDecimalColumn2.HeaderText = "EVENT_INDEX";
            gridViewDecimalColumn2.IsAutoGenerated = true;
            gridViewDecimalColumn2.Name = "EVENT_INDEX";
            gridViewDecimalColumn2.Width = 85;
            gridViewDateTimeColumn1.FieldName = "EVENT_TIME_STAMP";
            gridViewDateTimeColumn1.HeaderText = "EVENT_TIME_STAMP";
            gridViewDateTimeColumn1.IsAutoGenerated = true;
            gridViewDateTimeColumn1.Name = "EVENT_TIME_STAMP";
            gridViewDateTimeColumn1.Width = 85;
            gridViewDecimalColumn3.DataType = typeof(double);
            gridViewDecimalColumn3.FieldName = "EVENT_TIME";
            gridViewDecimalColumn3.HeaderText = "EVENT_TIME";
            gridViewDecimalColumn3.IsAutoGenerated = true;
            gridViewDecimalColumn3.Name = "EVENT_TIME";
            gridViewDecimalColumn3.Width = 85;
            gridViewTextBoxColumn5.FieldName = "EVENT_TYPE";
            gridViewTextBoxColumn5.HeaderText = "EVENT_TYPE";
            gridViewTextBoxColumn5.IsAutoGenerated = true;
            gridViewTextBoxColumn5.Name = "EVENT_TYPE";
            gridViewTextBoxColumn5.Width = 86;
            this.radGridView_events.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewDecimalColumn1,
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewTextBoxColumn3,
            gridViewTextBoxColumn4,
            gridViewDecimalColumn2,
            gridViewDateTimeColumn1,
            gridViewDecimalColumn3,
            gridViewTextBoxColumn5});
            this.radGridView_events.MasterTemplate.DataSource = this.eventsBindingSource;
            this.radGridView_events.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.radGridView_events.Name = "radGridView_events";
            this.radGridView_events.Size = new System.Drawing.Size(788, 335);
            this.radGridView_events.TabIndex = 7;
            this.radGridView_events.Text = "radGridView1";
            this.radGridView_events.SelectionChanged += new System.EventHandler(this.radGridView_events_SelectionChanged);
            this.radGridView_events.LayoutLoaded += new Telerik.WinControls.UI.LayoutLoadedEventHandler(this.radGridView_events_LayoutLoaded);
            // 
            // eventsBindingSource
            // 
            this.eventsBindingSource.DataMember = "Events";
            this.eventsBindingSource.DataSource = this.eventDatabaseDataSetBindingSource;
            // 
            // radSplitContainer1
            // 
            this.radSplitContainer1.Controls.Add(this.splitPanel1);
            this.radSplitContainer1.Controls.Add(this.splitPanel2);
            this.radSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radSplitContainer1.Location = new System.Drawing.Point(0, 0);
            this.radSplitContainer1.Name = "radSplitContainer1";
            // 
            // 
            // 
            this.radSplitContainer1.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.radSplitContainer1.Size = new System.Drawing.Size(794, 340);
            this.radSplitContainer1.TabIndex = 8;
            this.radSplitContainer1.TabStop = false;
            this.radSplitContainer1.Text = "radSplitContainer1";
            // 
            // splitPanel1
            // 
            this.splitPanel1.Controls.Add(this.radPropertyGrid1);
            this.splitPanel1.Location = new System.Drawing.Point(0, 0);
            this.splitPanel1.Name = "splitPanel1";
            // 
            // 
            // 
            this.splitPanel1.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.splitPanel1.Size = new System.Drawing.Size(395, 340);
            this.splitPanel1.TabIndex = 0;
            this.splitPanel1.TabStop = false;
            this.splitPanel1.Text = "splitPanel1";
            // 
            // radPropertyGrid1
            // 
            this.radPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPropertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.radPropertyGrid1.Name = "radPropertyGrid1";
            this.radPropertyGrid1.Size = new System.Drawing.Size(395, 340);
            this.radPropertyGrid1.TabIndex = 0;
            this.radPropertyGrid1.Text = "radPropertyGrid1";
            // 
            // splitPanel2
            // 
            this.splitPanel2.Controls.Add(this.tableLayoutPanel4);
            this.splitPanel2.Location = new System.Drawing.Point(399, 0);
            this.splitPanel2.Name = "splitPanel2";
            // 
            // 
            // 
            this.splitPanel2.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.splitPanel2.Size = new System.Drawing.Size(395, 340);
            this.splitPanel2.TabIndex = 1;
            this.splitPanel2.TabStop = false;
            this.splitPanel2.Text = "splitPanel2";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.radListView_data_files, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.radProgressBar_ReadData, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.radButton_StartRead, 0, 2);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(395, 340);
            this.tableLayoutPanel4.TabIndex = 5;
            // 
            // radListView_data_files
            // 
            this.radListView_data_files.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radListView_data_files.Location = new System.Drawing.Point(3, 3);
            this.radListView_data_files.Name = "radListView_data_files";
            this.radListView_data_files.Size = new System.Drawing.Size(389, 294);
            this.radListView_data_files.TabIndex = 3;
            this.radListView_data_files.Text = "radListView1";
            this.radListView_data_files.SelectedItemChanged += new System.EventHandler(this.radListView_data_files_SelectedItemChanged);
            // 
            // radProgressBar_ReadData
            // 
            this.radProgressBar_ReadData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radProgressBar_ReadData.Location = new System.Drawing.Point(3, 303);
            this.radProgressBar_ReadData.Name = "radProgressBar_ReadData";
            this.radProgressBar_ReadData.Size = new System.Drawing.Size(389, 14);
            this.radProgressBar_ReadData.TabIndex = 5;
            this.radProgressBar_ReadData.Text = "Read Progress";
            // 
            // radButton_StartRead
            // 
            this.radButton_StartRead.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radButton_StartRead.Location = new System.Drawing.Point(3, 323);
            this.radButton_StartRead.Name = "radButton_StartRead";
            this.radButton_StartRead.Size = new System.Drawing.Size(389, 14);
            this.radButton_StartRead.TabIndex = 4;
            this.radButton_StartRead.Text = "Read Data";
            this.radButton_StartRead.Click += new System.EventHandler(this.radButton_StartRead_Click);
            // 
            // eventDatabaseDataSetBindingSource
            // 
            this.eventDatabaseDataSetBindingSource.DataSource = this.eventDatabaseDataSet;
            this.eventDatabaseDataSetBindingSource.Position = 0;
            // 
            // eventDatabaseDataSet
            // 
            this.eventDatabaseDataSet.DataSetName = "EventDatabaseDataSet";
            this.eventDatabaseDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // eventsTableAdapter
            // 
            this.eventsTableAdapter.ClearBeforeFill = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1708, 688);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radChartView_speed_over_time)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radPageView1)).EndInit();
            this.radPageView1.ResumeLayout(false);
            this.radPageViewPage1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radTrackBar_chart_samples)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radGridView_events.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView_events)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radSplitContainer1)).EndInit();
            this.radSplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel1)).EndInit();
            this.splitPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radPropertyGrid1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel2)).EndInit();
            this.splitPanel2.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radListView_data_files)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radProgressBar_ReadData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton_StartRead)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventDatabaseDataSetBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventDatabaseDataSet)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadChartView radChartView_speed_over_time;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Telerik.WinControls.UI.RadPageView radPageView1;
        private Telerik.WinControls.UI.RadPageViewPage radPageViewPage1;
        private Telerik.WinControls.UI.RadPageViewPage radPageViewPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private Telerik.WinControls.UI.RadListView radListView_data_files;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private Telerik.WinControls.UI.RadProgressBar radProgressBar_ReadData;
        private Telerik.WinControls.UI.RadGridView radGridView_events;
        private Telerik.WinControls.UI.RadTrackBar radTrackBar_chart_samples;
        private Telerik.WinControls.UI.RadSplitContainer radSplitContainer1;
        private Telerik.WinControls.UI.SplitPanel splitPanel1;
        private Telerik.WinControls.UI.RadPropertyGrid radPropertyGrid1;
        private Telerik.WinControls.UI.SplitPanel splitPanel2;
        //private System.Windows.Forms.BindingSource eventsDataSet1BindingSource;
        
        //private System.Windows.Forms.BindingSource eventsBindingSource;
        
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private Telerik.WinControls.UI.RadButton radButton_StartRead;
        
        
        
        
        private System.Windows.Forms.BindingSource eventDatabaseDataSetBindingSource;
        private EventDatabaseDataSet eventDatabaseDataSet;
        private System.Windows.Forms.BindingSource eventsBindingSource;
        private EventDatabaseDataSetTableAdapters.EventsTableAdapter eventsTableAdapter;
    }
}

