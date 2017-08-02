using MathNet.Numerics.Statistics;
using MichaelsDataManipulator.SimplotDatabaseDataSetTableAdapters;
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

        public List<string> drive_letters
        {
            get
            {
                List<string> list = new List<string>();

                foreach (RadCheckedListDataItem item in radCheckedDropDownList_drives.Items)
                {
                    if (item.Checked)
                    {
                        list.Add(item.Text);
                    }
                }

                return list;
            }
        }


        LinearAxis horizontalAxis_seconds;
        LinearAxis horizontalAxis_minutes;

        LinearAxis verticalAxis;

        public double delta_t_min
        {
            get
            {
                return (double)radSpinEditor_delta_t_min.Value * 60;
            }
        }
        public double delta_t_max
        {
            get
            {
                return (double)radSpinEditor_delta_t_max.Value * 60;
            }
        }


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

        dataTableAdapter adapter = new dataTableAdapter();


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


                    if (data_file.n_of_good_data_points > 0)
                    {

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
            EventDatabaseDataSet.EventsRow[] rows =
                (EventDatabaseDataSet.EventsRow[])  eventDatabaseDataSet.Events.Select("EVENT_INDEX = " + ev.index_of_event + "AND DATA_FILENAME = '"+ev.data_filename + "'");

            if (rows.Count() == 0)
            {
                eventDatabaseDataSet.Events.AddEventsRow(ev.filename, ev.data_filename, "video filename", ev.conveyor, ev.index_of_event, ev.time_stamp_event, ev.time_of_event_relative_to_start_of_data_file, "");

                this.eventsTableAdapter.Update(eventDatabaseDataSet);

                eventDatabaseDataSet.AcceptChanges();
            }


        }

        private void radGridView_events_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void radTrackBar_chart_samples_ValueChanged(object sender, EventArgs e)
        {
            
            this.radGridView_data_SelectionChanged(null, null);
        }

        private void radButton_StartRead_Click(object sender, EventArgs e)
        {
            //FolderBrowserDialog folder_browser_dialog = new FolderBrowserDialog();


            //DialogResult result = folder_browser_dialog.ShowDialog();

            //if (result == DialogResult.OK)
            //{
            //    Debug.WriteLine("Read Data");
            //    //ReadData("F:\\DATA");
                PopulateDatabase();

            //    ignore_events = false;



            //}


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'eventDatabaseDataSet.Events' table. You can move, or remove it, as needed.

            ignore_events = true;
            this.eventsTableAdapter.Fill(this.eventDatabaseDataSet.Events);
            this.adapter.Fill(simplotDatabaseDataSet.data);


            List<string> drives = GetAvailableDriveLetters();

            foreach (string drive in drives)
            {
                radCheckedDropDownList_drives.Items.Add(drive);

                if (drive.Contains("E:"))
                {
                    (radCheckedDropDownList_drives.Items.Last as RadCheckedListDataItem).Checked = true;
                }
                if (drive.Contains("F:"))
                {
                    (radCheckedDropDownList_drives.Items.Last as RadCheckedListDataItem).Checked = true;
                }
                if (drive.Contains("G:"))
                {
                    (radCheckedDropDownList_drives.Items.Last as RadCheckedListDataItem).Checked = true;
                }

            }
            ignore_events = false;
        }

        private void radGridView_events_LayoutLoaded(object sender, LayoutLoadedEventArgs e)
        {

        }

        private void ListVideo(double event_time, string filename)
        {
            this.radListView_video.Items.Clear();

            this.radSpinEditor_event_time_code.Value = (decimal)event_time;

            string path = Path.GetPathRoot(filename);

            List<string> all_file_names = new List<string>();

            Form_searching_for_video_files search_form = new Form_searching_for_video_files();

            search_form.radProgressBar_search_for_video_files_drive_letters.Maximum = drive_letters.Count-1;

            search_form.Show();

            int n_letters = 0;
            foreach (string drive_letter in drive_letters)
            {
                try
                {
                    string[] file_names = Directory.GetFiles(drive_letter + "\\DATA", "*.mp4", SearchOption.AllDirectories);
                    all_file_names.AddRange(file_names.ToList<string>());

                    search_form.radProgressBar_search_for_video_files_drive_letters.Text = "scanning drive letter: " + drive_letter;
                    search_form.radProgressBar_search_for_video_files_drive_letters.Value1 = ++n_letters;

                    Application.DoEvents();
                }
                catch (Exception ex)
                { }
            }

            int n_files = 0;

            search_form.radProgressBar_search_for_video_files_files.Maximum = all_file_names.Count;

            foreach (string video_file in all_file_names)
            {
                if (n_files % 10 == 0)
                {
                    search_form.radProgressBar_search_for_video_files_files.Text = "scanning file " + n_files.ToString() + " of " + all_file_names.Count().ToString();
                    search_form.radProgressBar_search_for_video_files_files.Value1 = ++n_files;

                    Application.DoEvents();
                }

                string fn = Path.GetFileNameWithoutExtension(video_file);

                string[] subs = fn.Split('-');

                if (subs.GetUpperBound(0) == 5)
                {
                    string time_code = subs[subs.GetUpperBound(0)];

                    time_code = time_code.Substring(0, 10);

                    double start_time_of_video = double.Parse(time_code);

                    double time_min = event_time + delta_t_min;
                    double time_max = event_time + delta_t_max;

                    if ( start_time_of_video > time_min && start_time_of_video < time_max)
                    {
                        double event_delta_t = event_time - start_time_of_video;

                        TimeSpan event_delta_time_span = new TimeSpan(0, 0, (int)event_delta_t);

                        ListViewDataItem item = new ListViewDataItem();
                        this.radListView_video.Items.Add(item);
                        item.Text = video_file;

                        item[0] = video_file;
                        item[1] = start_time_of_video.ToString();
                        item[2] = event_delta_time_span.ToString();
                    }
                }
            }

            search_form.Hide();
        }

        private void radSpinEditor_delta_t_min_ValueChanged(object sender, EventArgs e)
        {
            radGridView_events_SelectionChanged(null, null);
        }

        private void radSpinEditor_delta_t_max_ValueChanged(object sender, EventArgs e)
        {
            radGridView_events_SelectionChanged(null, null);
        }

        private void radListView_video_DoubleClick(object sender, EventArgs e)
        {
            if (ignore_events)
                return;

            if (radListView_video.SelectedIndex >= 0)
            {

                ListViewDataItem item = radListView_video.Items[radListView_video.SelectedIndex];

                string filename = item.Text;

                Process.Start(filename);
            }
        }

        private List<string> GetAvailableDriveLetters()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            List<string> list = new List<string>();

            foreach (DriveInfo d in allDrives)
            {
                list.Add(d.Name);
            }

            return list;
        }

        private void radGridView_events_CellEndEdit(object sender, GridViewCellEventArgs e)
        {
            this.eventsTableAdapter.Update(eventDatabaseDataSet);

            eventDatabaseDataSet.AcceptChanges();

        }

        private void radButton_count_data_points_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog folder_browser_dialog = new FolderBrowserDialog();


            DialogResult result = folder_browser_dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                Debug.WriteLine("Count Data");
                //ReadData("F:\\DATA");
                CountDataPoints(folder_browser_dialog.SelectedPath);

                ignore_events = false;
                

            }


        }

        void CountDataPoints(string path)
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

                double min_start_time_code = double.MaxValue;
                double max_end_time_code = double.MinValue;

                double min_max_time_span_in_seconds =0;

                long counted_data_points = 0;
                long expected_data_points = 0;

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


                    if (data_file.n_of_good_data_points > 0)
                    {
                        counted_data_points += data_file.n_of_good_data_points;

                        ignore_events = true;
                        
                        if (data_file.start_time_code < min_start_time_code)
                        {
                            min_start_time_code = data_file.start_time_code;
                        }
                        if (max_end_time_code < data_file.start_time_code + 60*60)
                        {
                            max_end_time_code = data_file.start_time_code + 60 * 60;
                        }

                        min_max_time_span_in_seconds = max_end_time_code - min_start_time_code;

                        expected_data_points = (int)(min_max_time_span_in_seconds * 100);

                        radTextBox_counted_data_points.Text = counted_data_points.ToString();



                        radTextBox_expected_data_points.Text = expected_data_points.ToString();

                        radTextBox_data_point_ratio.Text = (100 * (double)counted_data_points / (double)expected_data_points).ToString() + "%";

                        ignore_events = false;

                        Application.DoEvents();
                     
                    }
                    data_file = null;
                }
                ignore_events = false;
            }

        }

        void PopulateDatabase()
        {
            simplotDatabaseDataSet.ToString();


            string[] file_names = Directory.GetFiles(@"\\prod\root\S_Drive\USGR-Shared\SimPlot DATA\DATA", "*.bin", SearchOption.AllDirectories);

            List<string> fn = new List<string>();
            fn.AddRange(file_names);

            fn.Sort();

            file_names = fn.ToArray();

            double accel_filter = 0.5 * 9.81;

            double min_start_time_code = double.MaxValue;
            double max_end_time_code = double.MinValue;

            double min_max_time_span_in_seconds = 0;

            long counted_data_points = 0;
            long expected_data_points = 0;

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


                if (data_file.n_of_good_data_points > 0)
                {
                    counted_data_points += data_file.n_of_good_data_points;

                    ignore_events = true;

                    if (data_file.start_time_code < min_start_time_code)
                    {
                        min_start_time_code = data_file.start_time_code;
                    }
                    if (max_end_time_code < data_file.end_time_code)
                    {
                        max_end_time_code = data_file.end_time_code;
                    }

                    min_max_time_span_in_seconds = max_end_time_code - min_start_time_code;

                    expected_data_points = (int)(min_max_time_span_in_seconds * 100);

                    radTextBox_counted_data_points.Text = counted_data_points.ToString();

                    radTextBox_expected_data_points.Text = expected_data_points.ToString();

                    radTextBox_data_point_ratio.Text = (100 * (double)counted_data_points / (double)expected_data_points).ToString() + "%";


                    AddDataFileToDataBase(data_file, file_name);

                    ignore_events = false;

                    Application.DoEvents();

                }
                else
                {
                    AddDataFileToDataBase(data_file, file_name);
                }
                data_file = null;
            }
            ignore_events = false;

        }

        void AddDataFileToDataBase(CDataFile datafile, string filename)
        {
            if (datafile.n_of_good_data_points > 0)
            {
                int index_of_local_max = datafile.IndexOfLocalStdDevMax;

                double t_window_min = -10;
                double t_window_max = 10;

                double window_min = double.MaxValue;
                double window_max = double.MinValue;
                double window_average = 0;

                int window_min_index = index_of_local_max + (int)(100 * t_window_min);
                int window_max_index = index_of_local_max + (int)(100 * t_window_max);

                if (window_min_index < 0)
                {
                    window_min_index = 0;
                }

                if (window_max_index > datafile.n_of_good_data_points - 1)
                {
                    window_max_index = datafile.n_of_good_data_points - 1;
                }

                int n = 0;

                for (int i = window_min_index; i <= window_max_index; i++)
                {
                    if (window_min > datafile.lower[i])
                    {
                        window_min = datafile.lower[i];
                    }

                    if (window_max < datafile.upper[i])
                    {
                        window_max = datafile.upper[i];
                    }

                    window_average += datafile.average[i];

                    n++;
                }

                window_average /= (double)n;

                double window_min_ft_per_min = Speed.FromMetersPerSecond(window_min).FeetPerSecond * 60;
                double window_max_ft_per_min = Speed.FromMetersPerSecond(window_max).FeetPerSecond * 60;
                double window_average_ft_per_min = Speed.FromMetersPerSecond(window_average).FeetPerSecond * 60;


                simplotDatabaseDataSet.data.AdddataRow(datafile.filename, datafile.data_filename, "", datafile.conveyor,
                   index_of_local_max,
                   datafile.time_stamp_data[index_of_local_max],
                   datafile.relative_time_data[index_of_local_max],
                   "",
                   window_min_ft_per_min,
                   window_max_ft_per_min,
                   window_average_ft_per_min,
                   datafile.local_std_dev.Max(),
                   datafile.minimum_ft_per_min,
                   datafile.maximum_ft_per_min,
                   datafile.total_average_ft_per_min,
                   datafile.std_dev_of_average,
                   datafile.n_of_good_data_points,
                   datafile.n_of_data_points);
            }
            else
            {
                simplotDatabaseDataSet.data.AdddataRow(filename,Path.GetFileName(filename), "","",
                   -1,
                   new DateTime() ,
                   0,
                   "",
                   0,
                   0,
                   0,
                   0,
                   0,
                   0,
                   0,
                   0,
                   datafile.n_of_good_data_points,
                   datafile.n_of_data_points);

            }

            adapter.Update(simplotDatabaseDataSet);

            simplotDatabaseDataSet.AcceptChanges();


            
            //eventDatabaseDataSet.Events.AddEventsRow(ev.filename, ev.data_filename, "video filename", ev.conveyor, ev.index_of_event, ev.time_stamp_event, ev.time_of_event_relative_to_start_of_data_file, "");

            //this.eventsTableAdapter.Update(eventDatabaseDataSet);

            //eventDatabaseDataSet.AcceptChanges();


        }

        private void radGridView_data_SelectionChanged(object sender, EventArgs e)
        {
            if (ignore_events)
                return;


            if (radGridView_data.SelectedRows.Count > 0)
            {
                Cursor = Cursors.WaitCursor;
                Application.DoEvents();


                this.radDesktopAlert1.Hide();

                string filename = this.radGridView_data.CurrentRow.Cells[1].Value as string;


                string changed_filename = filename;

                Debug.WriteLine(changed_filename);


                int index = (int)this.radGridView_data.CurrentRow.Cells["EVENT_INDEX"].Value;

                if (File.Exists(changed_filename))
                {

                    CDataFile datafile = new CDataFile(filename, 0.5);

                    datafile.ChartData(radChartView_speed_over_time, horizontalAxis_seconds, verticalAxis, index - chart_sample_size / 2, chart_sample_size);

                    radPropertyGrid1.SelectedObject = datafile;

                    // create the video list

                    double event_time = double.Parse(datafile.time_data[index]);

                    ListVideo(event_time, filename);
                }
                else
                {
                    this.radDesktopAlert1.CaptionText = "File Error";
                    this.radDesktopAlert1.ContentText = "Data file does not exist at indicated path.\nConsider changing the drive letter.";
                    this.radDesktopAlert1.Show();
                }

                Cursor = Cursors.Default;
            }


            Application.DoEvents();
        }
    }
}
