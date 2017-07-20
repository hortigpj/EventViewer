using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.Charting;
using Telerik.WinControls.UI;
using UnitsNet;

namespace MichaelsDataManipulator
{
    public partial class Form1 : Form
    {
        List<CDataFile> data_files = new List<CDataFile>();

        bool ignore_events = true;

        LinearAxis horizontalAxis;
        LinearAxis verticalAxis;

        int n_min;
        int n_max;

        int n_index;

        public Form1()
        {
            InitializeComponent();

            CartesianArea area = this.radChartView_speed_over_time.GetArea<CartesianArea>();

            area.ShowGrid = true;
            CartesianGrid grid = area.GetGrid<CartesianGrid>();
            grid.DrawHorizontalStripes = true;
            grid.DrawVerticalStripes = true;

            area.Axes.Clear();

            horizontalAxis = new LinearAxis();
            horizontalAxis.LabelFitMode = AxisLabelFitMode.MultiLine;


            horizontalAxis.Title = "Time";

            horizontalAxis.Minimum = 0;
            horizontalAxis.Maximum = 1;

            horizontalAxis.MajorStep = 1;


            area.Axes.Add(horizontalAxis);

            verticalAxis = new LinearAxis();
            verticalAxis.AxisType = AxisType.Second;
            verticalAxis.Title = "Speed";
            verticalAxis.Minimum = 0;
            verticalAxis.Maximum = 200;

            area.Axes.Add(verticalAxis);

        }

        void ReadData(string path)
        {
            string[] file_names = Directory.GetFiles(path, "*.bin", SearchOption.AllDirectories);


            float i = 0;
            foreach (string file_name in file_names)
            {
                Debug.WriteLine("reading:" + file_name);

                CDataFile data_file = new CDataFile(file_name);

                float progress = ++i / (float)file_names.Count();

                radProgressBar_ReadData.Value1 = (int)(progress * 100);

                if (data_file.Count > 0)
                {
                    data_files.Add(data_file);

                    radListView_data_files.Items.Add(Path.GetFileName(file_name));

                    Application.DoEvents();

                    //if (file_name.Contains("Fake Data"))
                    //{
                    //    for (int i = 1000; i < 1200; i++)
                    //    {
                    //        data_files.Last().head_speed[i] = Speed.FromFeetPerSecond(200 / 60).MetersPerSecond;
                    //    }
                    //}

                }
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            String path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


            Debug.WriteLine("Read Data");
            ReadData(path + "\\DATA\\TEST");

            ignore_events = false;
            radListView_data_files.SelectedIndex = 0;


            //CreateChart(10,20);
        }

        private void SetNMinMax(CDataFile data_file)
        {
            n_min = n_index + (int)radSpinEditor_index_minus.Value;

            if (n_min < 0)
            {
                n_min = 0;
            }

            n_max = n_index + (int)radSpinEditor_index_plus.Value; 

            if (n_max > data_file.Count)
            {
                n_max = data_file.Count-1;
            }

        }

        private void CreateChart(CDataFile data_file)
        {
            Debug.WriteLine("Creating chart");

            

            ScatterLineSeries tail_speed_series = new ScatterLineSeries();

            //tail_speed_series.BackColor = Color.Green;
            //tail_speed_series.BackColor2 = Color.Green;
            //tail_speed_series.BackColor3 = Color.Green;
            //tail_speed_series.BackColor4 = Color.Green;

            tail_speed_series.LegendTitle = "Tail Speed";
            tail_speed_series.PointSize = new SizeF(2, 2);


            Debug.WriteLine("tail speed data");
            for (int i= n_min;i< n_max; i++)
            {
                tail_speed_series.DataPoints.Add(new ScatterDataPoint(data_file.time_data[i], Speed.FromMetersPerSecond(data_file.tail_speed[i]).FeetPerSecond * 60));
            }

            tail_speed_series.HorizontalAxis = horizontalAxis;
            tail_speed_series.VerticalAxis = verticalAxis;



            ScatterLineSeries mid_speed_series = new ScatterLineSeries();

            //mid_speed_series.BackColor = Color.Blue;
            //mid_speed_series.BackColor2 = Color.Blue;
            //mid_speed_series.BackColor3 = Color.Blue;
            //mid_speed_series.BackColor4 = Color.Blue;

            mid_speed_series.LegendTitle = "Mid Speed";
            mid_speed_series.PointSize = new SizeF(2, 2);

            Debug.WriteLine("mid speed data");
            for (int i = n_min; i < n_max; i++)
            {
                mid_speed_series.DataPoints.Add(new ScatterDataPoint(data_file.time_data[i], Speed.FromMetersPerSecond(data_file.mid_speed[i]).FeetPerSecond * 60));
            }

            mid_speed_series.HorizontalAxis = horizontalAxis;
            mid_speed_series.VerticalAxis = verticalAxis;


            ScatterLineSeries head_speed_series = new ScatterLineSeries();

            //head_speed_series.BackColor = Color.Red;
            //head_speed_series.BackColor2 = Color.Red;
            //head_speed_series.BackColor3 = Color.Red;
            //head_speed_series.BackColor4 = Color.Red;

            head_speed_series.PointSize = new SizeF(2, 2);
            head_speed_series.LegendTitle = "Head Speed";


            Debug.WriteLine("head speed data");
            for (int i = n_min; i < n_max; i++)
            {
                head_speed_series.DataPoints.Add(new ScatterDataPoint(data_file.time_data[i], Speed.FromMetersPerSecond(data_file.head_speed[i]).FeetPerSecond * 60));
            }

            head_speed_series.HorizontalAxis = horizontalAxis;
            head_speed_series.VerticalAxis = verticalAxis;


            this.radChartView_speed_over_time.Series.Clear();
            Debug.WriteLine("Adding Series:");
            Debug.WriteLine("tail speed");
            this.radChartView_speed_over_time.Series.Add(tail_speed_series);
            Debug.WriteLine("mid speed");
            this.radChartView_speed_over_time.Series.Add(mid_speed_series);
            Debug.WriteLine("head speed");
            this.radChartView_speed_over_time.Series.Add(head_speed_series);

        }

        private void InitChart()
        {
            Debug.WriteLine("Creating chart");

            // Initialize the Axes

            CartesianArea area = this.radChartView_speed_over_time.GetArea<CartesianArea>();

            area.ShowGrid = true;
            CartesianGrid grid = area.GetGrid<CartesianGrid>();
            grid.DrawHorizontalStripes = true;
            grid.DrawVerticalStripes = true;

            area.Axes.Clear();

            LinearAxis horizontalAxis = new LinearAxis();
            horizontalAxis.LabelFitMode = AxisLabelFitMode.MultiLine;

            this.radChartView_speed_over_time.Series.Clear();
            Debug.WriteLine("Adding Series:");
            Debug.WriteLine("tail speed");
            //this.radChartView_speed_over_time.Series.Add(tail_speed_series);
            Debug.WriteLine("mid speed");
            //this.radChartView_speed_over_time.Series.Add(mid_speed_series);
            Debug.WriteLine("head speed");
            //this.radChartView_speed_over_time.Series.Add(head_speed_series);

        }



        private void radListView_data_files_SelectedItemChanged(object sender, EventArgs e)
        {
            if (ignore_events)
                return;

            if (radListView_data_files.SelectedIndex >= 0)
            {

                CDataFile data_file = data_files[radListView_data_files.SelectedIndex];

                SetNMinMax(data_file);

                horizontalAxis.Minimum = data_file.time_data[n_min];
                horizontalAxis.Maximum = data_file.time_data[n_max];

                radSpinEditor_size.Value = (decimal)data_file.Count;
                radSpinEditor_average.Value = (decimal)   Speed.FromMetersPerSecond(data_file.average).FeetPerSecond*60;
                radSpinEditor_maximum.Value = (decimal)Speed.FromMetersPerSecond(data_file.maximum).FeetPerSecond * 60;
                radSpinEditor_minimum.Value = (decimal)Speed.FromMetersPerSecond(data_file.minimum).FeetPerSecond * 60;

                CreateChart(data_file);
            }

        }

        private void radSpinEditor_index_minus_ValueChanged(object sender, EventArgs e)
        {
            CDataFile data_file = data_files[radListView_data_files.SelectedIndex];

            SetNMinMax(data_file);

            horizontalAxis.Minimum = data_file.time_data[n_min];
            horizontalAxis.Maximum = data_file.time_data[n_max];

        }

        private void radSpinEditor_index_ValueChanged(object sender, EventArgs e)
        {
            CDataFile data_file = data_files[radListView_data_files.SelectedIndex];

            SetNMinMax(data_file);

            horizontalAxis.Minimum = data_file.time_data[n_min];
            horizontalAxis.Maximum = data_file.time_data[n_max];

            CreateChart(data_file);

        }

        private void radSpinEditor_index_plus_ValueChanged(object sender, EventArgs e)
        {
            CDataFile data_file = data_files[radListView_data_files.SelectedIndex];

            n_index = (int)radSpinEditor_index.Value;

            SetNMinMax(data_file);

            horizontalAxis.Minimum = data_file.time_data[n_min];
            horizontalAxis.Maximum = data_file.time_data[n_max];

        }

        private void radButton_Find_Events_Click(object sender, EventArgs e)
        {
            ignore_events = true;

            radListView_events.Items.Clear();

            double trigger_high = Speed.FromFeetPerSecond((double)radSpinEditor_event_trigger_high.Value / 60).MetersPerSecond;
            double trigger_low = Speed.FromFeetPerSecond((double)radSpinEditor_event_trigger_low.Value / 60).MetersPerSecond;

            float i=0;

            foreach (CDataFile data_file in data_files)
            {

                float progress = ++i / (float)data_files.Count();
                
                radProgressBar_events.Value1 = (int)(progress * 100);

                List<int> events = data_file.FindEvent(trigger_high, trigger_low);
                
                foreach (int ev in events)
                {
                    ListViewDataItem item = new ListViewDataItem();
                    radListView_events.Items.Add(item);

                    item[0] = ev.ToString();
                    item[1] = Path.GetFileName(data_file.filename);

                    item.Tag = data_file;

                    Application.DoEvents();
                }

                
            }

            ignore_events = false;
        }

        private void radListView_events_SelectedItemChanged(object sender, EventArgs e)
        {
            if (ignore_events)
                return;

            if (radListView_events.SelectedItem != null)
            {

                CDataFile data_file = radListView_events.SelectedItem.Tag as CDataFile;

                if (data_file != null)
                {

                    ListViewDataItem item = radListView_events.SelectedItem;

                    string index = item[0] as string;

                    if (index != null)
                    {
                        n_index = int.Parse(item[0] as string);

                        ignore_events = true;

                        radSpinEditor_index.Value = (decimal)n_index;

                        ignore_events = false;

                        SetNMinMax(data_file);

                        horizontalAxis.Minimum = data_file.time_data[n_min];
                        horizontalAxis.Maximum = data_file.time_data[n_max];

                        radSpinEditor_average.Value = (decimal)Speed.FromMetersPerSecond(data_file.average).FeetPerSecond * 60;
                        radSpinEditor_maximum.Value = (decimal)Speed.FromMetersPerSecond(data_file.maximum).FeetPerSecond * 60;
                        radSpinEditor_minimum.Value = (decimal)Speed.FromMetersPerSecond(data_file.minimum).FeetPerSecond * 60;

                        CreateChart(data_file);
                    }
                }
            }
        }

        private void radListView_events_ItemMouseClick(object sender, ListViewItemEventArgs e)
        {
            radListView_events_SelectedItemChanged(null, null);
        }
    }
}
