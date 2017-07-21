using MathNet.Numerics.Statistics;
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

        SortedList<double, string> events = new SortedList<double, string>();

        List<CEvent> my_events = new List<CEvent>();

        CDataFile data_file = null;

        bool ignore_events = true;

        LinearAxis horizontalAxis;
        LinearAxis verticalAxis;

        int n_min;
        int n_max;

        int _n_index;
        int n_index
        {
            get
            {
                return _n_index;
            }
            set
            {
                _n_index = value;

                ignore_events = true;

                radSpinEditor_index.Value = (decimal)_n_index;

                ignore_events = false;
            }
        }


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
            String mydocs_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            using (TextWriter writer = File.CreateText(mydocs_path + "\\eventlog.txt")) 
            {

                string[] file_names = Directory.GetFiles(path, "*.bin", SearchOption.AllDirectories);

                List<string> fn = new List<string>();
                fn.AddRange(file_names);

                fn.Sort();

                file_names = fn.ToArray();


                double accel_filter = (double)radSpinEditor_accel_trigger.Value * 9.81;

                ignore_events = true;

                float i = 0;

                long aver_ticks = 0;
                long old_ticks = 0;

                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();

                long start_ticks = stopwatch.ElapsedMilliseconds;

                foreach (string file_name in file_names)
                {
                    Debug.WriteLine("reading:" + file_name);

                    long now_ticks = stopwatch.ElapsedMilliseconds;

                    long ticks_passed = now_ticks - old_ticks;

                    long ticks_passed_total = now_ticks - start_ticks;

                    old_ticks = now_ticks;

                    aver_ticks += ticks_passed;

                    aver_ticks /= 2;

                    long ticks_total = aver_ticks * file_names.Count();

                    long ticks_left = ticks_total - ticks_passed_total;

                    if (ticks_left < 0)
                        ticks_left = 0;

                    Duration time_left = Duration.FromMilliseconds((double)(ticks_left));
                    Duration time_per = Duration.FromMilliseconds((double)(ticks_passed));


                    CDataFile data_file = new CDataFile(file_name, accel_filter);


                    float progress = ++i / (float)file_names.Count();

                    radProgressBar_ReadData.Value1 = (int)(progress * 100);
                    radProgressBar_ReadData.Text = i.ToString() + "/" + file_names.Count().ToString() + " "
                        + ((int)time_left.ToTimeSpan().TotalMinutes).ToString() + " min left" +
                        " per : " + ((int)time_per.ToTimeSpan().TotalSeconds).ToString() + " sec";


                    if (data_file.Count > 0)
                    {

                        ignore_events = true;
                        radListView_data_files.Items.Add(Path.GetFileName(file_name));
                        ignore_events = false;

                        Application.DoEvents();

                        if (data_file.IndexOfSumMaximum >= 0)
                        {
                            int i_max = data_file.IndexOfSumMaximum;

                            double std = data_file.std_dev_of_average;

                            double local_max = data_file.local_std_dev.Max();

                            if (local_max > 0.1)
                            {
                                Histogram hist = new Histogram(data_file.upper.ToArray(), 10);
                                Bucket bucket = hist.GetBucketOf(data_file.maximum);

                                double c = bucket.Count;

                                int index_of_local_max = data_file.local_std_dev.IndexOf(local_max);
                                DateTime time_of_local_max = data_file.time_stamp_data[index_of_local_max];


                                CEvent ev0 = new CEvent(
                                        "Surge",
                                        data_file.maximum,
                                        data_file.filename,
                                        index_of_local_max,
                                        time_of_local_max,
                                        bucket.Count);

                                my_events.Add(ev0);

                                writer.WriteLine("Surge");
                                writer.WriteLine(data_file.filename);
                                writer.WriteLine(index_of_local_max.ToString());
                                writer.WriteLine(time_of_local_max.ToString());
                                



                                if (data_file.DetectDriveBarSkipping(
                                    FeetPerMinuteToMetersPerSecond(40),
                                    FeetPerMinuteToMetersPerSecond(10),
                                    FeetPerMinuteToMetersPerSecond(40)))
                                {
                                    CEvent ev1 = new CEvent(
                                            "Drivebar",
                                            data_file.maximum,
                                            data_file.filename,
                                            data_file.drive_bar_skipping_index,
                                            data_file.drive_bar_skipping_date_time,
                                            0);

                                    my_events.Add(ev1);

                                    writer.WriteLine("Drive Bar");
                                    writer.WriteLine(data_file.filename);
                                    writer.WriteLine(data_file.drive_bar_skipping_index.ToString());
                                    writer.WriteLine(data_file.drive_bar_skipping_date_time.ToString());


                                }

                                my_events.Sort((a, b) => a.CompareTo(b));

                                radListView_events.Items.Clear();

                                foreach (CEvent e in my_events)
                                {
                                    ListViewDataItem item = new ListViewDataItem();
                                    radListView_events.Items.Add(item);

                                    item[0] = e.type;
                                    item[1] = (Speed.FromMetersPerSecond(e.sum_maximum).FeetPerSecond * 60).ToString();
                                    item[2] = e.qt_rank_add_maximum.ToString();
                                    item[3] = e.time_of_maximum.ToString();
                                    item[4] = e.index_of_maximum.ToString();
                                    item[5] = Path.GetFileName(e.filename);
                                    item[6] = Path.GetFullPath(e.filename);

                                    item.Tag = e;
                                }


                                Application.DoEvents();
                            }
                        }


                    }

                    data_file = null;
                }
                ignore_events = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            String path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


            Debug.WriteLine("Read Data");
            ReadData("F:\\");
            //ReadData(path + "\\data");

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

            head_speed_series.PointSize = new SizeF(2, 2);
            head_speed_series.LegendTitle = "Head Speed";


            Debug.WriteLine("head speed data");
            for (int i = n_min; i < n_max; i++)
            {
                head_speed_series.DataPoints.Add(new ScatterDataPoint(data_file.time_data[i], Speed.FromMetersPerSecond(data_file.head_speed[i]).FeetPerSecond * 60));
            }

            head_speed_series.HorizontalAxis = horizontalAxis;
            head_speed_series.VerticalAxis = verticalAxis;


            ScatterLineSeries upper_series = new ScatterLineSeries();

            upper_series.PointSize = new SizeF(2, 2);
            upper_series.LegendTitle = "Upper Speed";
            
            for (int i = n_min; i < n_max; i++)
            {
                upper_series.DataPoints.Add(new ScatterDataPoint(data_file.time_data[i], Speed.FromMetersPerSecond(data_file.upper[i]).FeetPerSecond * 60));
            }

            upper_series.HorizontalAxis = horizontalAxis;
            upper_series.VerticalAxis = verticalAxis;


            ScatterLineSeries lower_series = new ScatterLineSeries();

            lower_series.PointSize = new SizeF(2, 2);
            lower_series.LegendTitle = "Lower Speed";

            for (int i = n_min; i < n_max; i++)
            {
                lower_series.DataPoints.Add(new ScatterDataPoint(data_file.time_data[i], Speed.FromMetersPerSecond(data_file.lower[i]).FeetPerSecond * 60));
            }

            lower_series.HorizontalAxis = horizontalAxis;
            lower_series.VerticalAxis = verticalAxis;


            ScatterLineSeries running_average_series = new ScatterLineSeries();

            running_average_series.PointSize = new SizeF(2, 2);
            running_average_series.LegendTitle = "Running Avr. Speed";

            for (int i = n_min; i < n_max; i++)
            {
                running_average_series.DataPoints.Add(new ScatterDataPoint(data_file.time_data[i], Speed.FromMetersPerSecond(data_file.running_average[i]).FeetPerSecond * 60));
            }

            running_average_series.HorizontalAxis = horizontalAxis;
            running_average_series.VerticalAxis = verticalAxis;


            ScatterLineSeries band_width_series = new ScatterLineSeries();

            band_width_series.PointSize = new SizeF(2, 2);
            band_width_series.LegendTitle = "Band Width";

            for (int i = n_min; i < n_max; i++)
            {
                band_width_series.DataPoints.Add(new ScatterDataPoint(data_file.time_data[i], Speed.FromMetersPerSecond(data_file.band_width[i]).FeetPerSecond * 60));
            }

            band_width_series.HorizontalAxis = horizontalAxis;
            band_width_series.VerticalAxis = verticalAxis;

            ScatterLineSeries local_std_dev_series = new ScatterLineSeries();

            local_std_dev_series.PointSize = new SizeF(2, 2);
            local_std_dev_series.LegendTitle = "Band Width";

            for (int i = n_min; i < n_max; i++)
            {
                local_std_dev_series.DataPoints.Add(new ScatterDataPoint(data_file.time_data[i], data_file.local_std_dev[i] * 100));
            }

            local_std_dev_series.HorizontalAxis = horizontalAxis;
            local_std_dev_series.VerticalAxis = verticalAxis;


            this.radChartView_speed_over_time.Series.Clear();
            Debug.WriteLine("Adding Series:");
            Debug.WriteLine("tail speed");
            this.radChartView_speed_over_time.Series.Add(tail_speed_series);
            Debug.WriteLine("mid speed");
            this.radChartView_speed_over_time.Series.Add(mid_speed_series);
            Debug.WriteLine("head speed");
            this.radChartView_speed_over_time.Series.Add(head_speed_series);

            this.radChartView_speed_over_time.Series.Add(running_average_series);
            this.radChartView_speed_over_time.Series.Add(band_width_series);
            this.radChartView_speed_over_time.Series.Add(local_std_dev_series);

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


        }

        private void radSpinEditor_index_minus_ValueChanged(object sender, EventArgs e)
        {
            

            SetNMinMax(data_file);

            horizontalAxis.Minimum = data_file.time_data[n_min];
            horizontalAxis.Maximum = data_file.time_data[n_max];

        }

        private void radSpinEditor_index_ValueChanged(object sender, EventArgs e)
        {
            if (ignore_events)
                return;

            n_index = (int)radSpinEditor_index.Value;


            SetNMinMax(data_file);

            horizontalAxis.Minimum = data_file.time_data[n_min];
            horizontalAxis.Maximum = data_file.time_data[n_max];

            CreateChart(data_file);

            ignore_events = false;

        }

        private void radSpinEditor_index_plus_ValueChanged(object sender, EventArgs e)
        {

            n_index = (int)radSpinEditor_index.Value;

            SetNMinMax(data_file);

            horizontalAxis.Minimum = data_file.time_data[n_min];
            horizontalAxis.Maximum = data_file.time_data[n_max];

        }

        private void radButton_Find_Events_Click(object sender, EventArgs e)
        {
        }

        private void radListView_events_SelectedItemChanged(object sender, EventArgs e)
        {
            if (ignore_events)
                return;

            if (radListView_events.SelectedItem != null)
            {
                CEvent ev = radListView_events.SelectedItem.Tag as CEvent;

                if (ev != null)
                {

                    CDataFile data_file = new CDataFile(ev.filename, (double)radSpinEditor_accel_trigger.Value * 9.81);

                    if (data_file != null)
                        {

                            ListViewDataItem item = radListView_events.SelectedItem;

                            string index = item[4] as string;

                            if (index != null)
                            {
                                n_index = int.Parse(item[4] as string);

                                SetNMinMax(data_file);

                                horizontalAxis.Minimum = data_file.time_data[n_min];
                                horizontalAxis.Maximum = data_file.time_data[n_max];

                                radSpinEditor_average.Value = (decimal)Speed.FromMetersPerSecond(data_file.total_average).FeetPerSecond * 60;
                                radSpinEditor_maximum.Value = (decimal)Speed.FromMetersPerSecond(data_file.maximum).FeetPerSecond * 60;
                                radSpinEditor_minimum.Value = (decimal)Speed.FromMetersPerSecond(data_file.minimum).FeetPerSecond * 60;

                                CreateChart(data_file);

                                Application.DoEvents();
                            }
                        }
                    }
            }
        }

        private void radListView_events_ItemMouseClick(object sender, ListViewItemEventArgs e)
        {
            radListView_events_SelectedItemChanged(null, null);
        }

        private void radLabel10_Click(object sender, EventArgs e)
        {

        }

        private double FeetPerMinuteToMetersPerSecond(double ft_per_min)
        {
            return Speed.FromFeetPerSecond(ft_per_min / 60).MetersPerSecond;
        }
    }
}
