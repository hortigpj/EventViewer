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

        //SortedList<double, string> events = new SortedList<double, string>();

        //List<CEvent> my_events = new List<CEvent>();

//        CDataFile data_file = null;

        bool ignore_events = true;

        LinearAxis horizontalAxis_seconds;
        LinearAxis horizontalAxis_minutes;

        LinearAxis verticalAxis;

        public int chart_sample_size
        {
            get
            {
                return (int)radTrackBar_chart_samples.Value;
            }
            set
            {
                radTrackBar_chart_samples.Value = (float)value;
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

            horizontalAxis_seconds = new LinearAxis();
            horizontalAxis_seconds.LabelFitMode = AxisLabelFitMode.MultiLine;


            horizontalAxis_seconds.Title = "Time [sec]";

            horizontalAxis_seconds.Minimum = 0;
            horizontalAxis_seconds.Maximum = 1;

            horizontalAxis_seconds.MajorStep = 1;


            area.Axes.Add(horizontalAxis_seconds);

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


                double accel_filter = 0.5 * 9.81;

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

                        //data_file.ChartData(this.radChartView_speed_over_time, horizontalAxis_seconds, verticalAxis, 0 , 500);

                        Application.DoEvents();

//                        if (false)
                        if (data_file.IndexOfSumMaximum >= 0)
                        {
                            int i_max = data_file.IndexOfSumMaximum;

                            double std = data_file.std_dev_of_average;

                            double local_max = data_file.local_std_dev.Max();

                            if (data_file.LocalStdDevMaximum > 0.1)
                            {
                                Histogram hist = new Histogram(data_file.upper.ToArray(), 10);
                                Bucket bucket = hist.GetBucketOf(data_file.maximum);

                                double c = bucket.Count;

                                int index_of_local_max = data_file.IndexOfLocalStdDevMax;
                                DateTime time_of_local_max = data_file.time_stamp_data[index_of_local_max];


                                CEvent ev = new CEvent(
                                        data_file.filename,
                                        index_of_local_max,
                                        data_file.time_data[index_of_local_max],
                                        data_file.relative_time_data[index_of_local_max],
                                        data_file.time_stamp_data[index_of_local_max]);

                                data_file.ChartData(radChartView_speed_over_time, horizontalAxis_seconds, verticalAxis, ev.index_of_event - 250, 500);



                                writer.WriteLine("Surge");
                                writer.WriteLine(data_file.filename);
                                writer.WriteLine(index_of_local_max.ToString());
                                writer.WriteLine(time_of_local_max.ToString());

                                //my_events.Sort((a, b) => a.CompareTo(b));

                                //radListView_events.Items.Clear();

                                //foreach (CEvent e in my_events)
                                //{
                                //    ListViewDataItem item = new ListViewDataItem();
                                //    radListView_events.Items.Add(item);

                                //    item[0] = e.type;
                                //    item[1] = (Speed.FromMetersPerSecond(e.sum_maximum).FeetPerSecond * 60).ToString();
                                //    item[2] = e.qt_rank_add_maximum.ToString();
                                //    item[3] = e.time_of_maximum.ToString();
                                //    item[4] = e.index_of_maximum.ToString();
                                //    item[5] = Path.GetFileName(e.filename);
                                //    item[6] = Path.GetFullPath(e.filename);

                                //    item.Tag = e;
                                //}

                                AddEventToDataBase(ev);



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
            //ReadData("F:\\DATA");
            ReadData(path + "\\data");

            ignore_events = false;
            radListView_data_files.SelectedIndex = 0;


            //CreateChart(10,20);
        }



        private void radListView_data_files_SelectedItemChanged(object sender, EventArgs e)
        {
            if (ignore_events)
                return;


        }




        private void radListView_events_ItemMouseClick(object sender, ListViewItemEventArgs e)
        {
        }

        private void radLabel10_Click(object sender, EventArgs e)
        {

        }

        private double FeetPerMinuteToMetersPerSecond(double ft_per_min)
        {
            return Speed.FromFeetPerSecond(ft_per_min / 60).MetersPerSecond;
        }

        private void AddEventToDataBase(CEvent ev)
        {
            eventsDataSet.Events.AddEventsRow(ev.filename,"video filename","conveyor",ev.index_of_event,ev.time_stamp_event,ev.time_of_event_relative_to_start_of_data_file);
            eventsDataSet.AcceptChanges();
        }

        private void radGridView_events_SelectionChanged(object sender, EventArgs e)
        {
            if (radGridView_events.SelectedRows.Count > 0)
            {

                string filename = this.radGridView_events.CurrentRow.Cells[1].Value as string;

                int index = (int)this.radGridView_events.CurrentRow.Cells[4].Value;

                CDataFile datafile = new CDataFile(filename, 0.5);

                datafile.ChartData(radChartView_speed_over_time, horizontalAxis_seconds, verticalAxis, index - chart_sample_size / 2 , chart_sample_size);

                radPropertyGrid1.SelectedObject = datafile;

            }

        }

        private void radTrackBar_chart_samples_ValueChanged(object sender, EventArgs e)
        {
            radGridView_events_SelectionChanged(null, null);
        }

    }
}
