using MathNet.Numerics.Statistics;
using MichaelsDataManipulator.SimplotDatabaseDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
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
        enum SCAN_TYPE
        {
            STD_DEV,
            RUNNING_AVR
        };



        bool ignore_events = true;


        public int running_average_size
        {
            get
            {
                return (int)radSpinEditor_running_avr_size.Value;
            }
        }

        public int local_standard_deviation_size
        {
            get
            {
                return (int)radSpinEditor_local_standard_deviation_size.Value;
            }
        }



        public double speed_drop_in_percent
        {
            get
            {
                return (double)radSpinEditor_running_avr_decrease_trigger_value.Value * 0.01;
            }
        }


        public double spike_filter_accel
        {
            get
            {
                return (double)radSpinEditor_spike_filter_trigger_accel.Value * 9.81;

            }
            set
            {
                radSpinEditor_spike_filter_trigger_accel.Value = (decimal)(value / 9.81);
            }
        }
        public double low_pass_filter_frequency
        {
            get
            {
                return (double)radSpinEditor_low_pass_filter_frequency.Value;

            }
            set
            {
                radSpinEditor_low_pass_filter_frequency.Value = (decimal)value;
            }
        }



        public CDataFile current_datafile
        {
            get; set;
        }


        public int event_file_index
        {
            get
            {
                if (current_datafile != null)
                {
                    int index = (int)this.radGridView_data.CurrentRow.Cells["EVENT_INDEX"].Value;

                    if (index >= 0 && index <current_datafile.n_of_good_data_points)
                    {
                        return index;
                    }
                }

                return -1;
            }
        }

        public int current_file_index
        {
            get
            {
                radTextBox_position.Text = (radHScrollBar_index.Value * 0.01).ToString();

                return (int)radHScrollBar_index.Value;
            }
            set
            {
                radHScrollBar_index.Maximum = current_datafile.n_of_good_data_points;
                radHScrollBar_index.Minimum = 0;

                radHScrollBar_index.Value = value;
                radTextBox_position.Text = (value * 0.01).ToString();
            }
        }



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
                radTextBox_range.Text = (radTrackBar_chart_samples.Value *0.01).ToString();
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

            RadMenuItem radmenuitem_std_dev_scan = new RadMenuItem("Std. Dev. Scan");
            radmenuitem_std_dev_scan.Click += Radmenuitem_std_dev_scan_Click;

            RadMenuItem radmenuitem_running_average_scan = new RadMenuItem("Run. Avr. Scan");
            radmenuitem_running_average_scan.Click += Radmenuitem_running_average_scan_Click;



            radDropDownButton_scans.Items.Add(radmenuitem_std_dev_scan);
            radDropDownButton_scans.Items.Add(radmenuitem_running_average_scan);

            /// tests
            /// 


        }

        private void Radmenuitem_running_average_scan_Click(object sender, EventArgs e)
        {
            RunningAverageScan();
        }

        private void Radmenuitem_std_dev_scan_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
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

        private void radGridView_events_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void radTrackBar_chart_samples_ValueChanged(object sender, EventArgs e)
        {
            
            this.radGridView_data_SelectionChanged(null, null);
        }

        private void radButton_StartRead_Click(object sender, EventArgs e)
        {
            PopulateDatabase();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'eventDatabaseDataSet.Events' table. You can move, or remove it, as needed.

            ignore_events = true;

            // do the sql stuff

            //string strConnection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\\\prod\\root\\S_Drive\\USGR - Shared\\SimPlot DATA\\SimplotDatabase.accdb";
            //string strConnection = "Data Source=\\\\prod\\root\\S_Drive\\USGR - Shared\\SimPlot DATA\\SimplotDatabase.accdb";

            //SqlConnection con = new SqlConnection(strConnection);


            //SqlCommand sqlCmd = new SqlCommand();
            //sqlCmd.Connection = con;
            //sqlCmd.CommandType = CommandType.Text;
            //sqlCmd.CommandText = "Select * from data";

            //SqlDataAdapter sqlDataAdap = new SqlDataAdapter(sqlCmd);

            //DataTable dtRecord = new DataTable();
            //sqlDataAdap.Fill(dtRecord);
            //radGridView_data.DataSource = dtRecord;

            




            SimplotDatabaseDataSetTableAdapters.dataTableAdapter a = new SimplotDatabaseDataSetTableAdapters.dataTableAdapter();
            a.Fill(simplotDatabaseDataSet.data);


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

        private void ListVideo(double event_time, string filename)
        {
            this.radListView_video.Items.Clear();

            this.radSpinEditor_event_time_code.Value = (decimal)event_time;

            string path = Path.GetPathRoot(filename);

            List<string> all_file_names = new List<string>();

            Form_searching_for_video_files search_form = new Form_searching_for_video_files();

            if (drive_letters.Count > 0)
            {

                search_form.radProgressBar_search_for_video_files_drive_letters.Maximum = drive_letters.Count - 1;

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

                        if (start_time_of_video > time_min && start_time_of_video < time_max)
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

   
        void PopulateDatabase()
        {
            string[] file_names = Directory.GetFiles(@"\\prod\root\S_Drive\USGR-Shared\SimPlot DATA\DATA", "*.bin", SearchOption.AllDirectories);

            List<string> fn = new List<string>();
            fn.AddRange(file_names);

            fn.Sort();

            file_names = fn.ToArray();

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


                CDataFile data_file = new CDataFile(file_name, radCheckBox_spike_filter.Checked, spike_filter_accel, radCheckBox_low_pass_filter.Checked, low_pass_filter_frequency, running_average_size, local_standard_deviation_size);

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

                    //radTextBox_counted_data_points.Text = counted_data_points.ToString();

                    //radTextBox_expected_data_points.Text = expected_data_points.ToString();

                    //radTextBox_data_point_ratio.Text = (100 * (double)counted_data_points / (double)expected_data_points).ToString() + "%";

                    AddDataFileToDataBase(data_file, file_name, SCAN_TYPE.STD_DEV, 0);

                    ignore_events = false;

                    Application.DoEvents();

                }
                else
                {
                    AddDataFileToDataBase(data_file, file_name, SCAN_TYPE.STD_DEV,0);
                }
                data_file = null;
            }
            ignore_events = false;

        }

        void AddDataFileToDataBase(CDataFile datafile, string filename, SCAN_TYPE scan_type, double running_avr_drop_trigger_value)
        {
            if (scan_type != SCAN_TYPE.STD_DEV && scan_type != SCAN_TYPE.RUNNING_AVR)
            {
                Debugger.Break();
            }


            SimplotDatabaseDataSetTableAdapters.dataTableAdapter adapter = new SimplotDatabaseDataSetTableAdapters.dataTableAdapter();

            if (datafile.n_of_good_data_points > 0)
            {

                int index_of_event=0;

                if (scan_type == SCAN_TYPE.STD_DEV)
                {
                    index_of_event = datafile.IndexOfLocalStdDevMax;
                }

                if (scan_type == SCAN_TYPE.STD_DEV)
                {
                    index_of_event = datafile.IndexOfRunningAverageDropForAllSpeeds(running_avr_drop_trigger_value);
                }



                double t_window_min = -10;
                double t_window_max = 10;

                double window_min = double.MaxValue;
                double window_max = double.MinValue;
                double window_average = 0;

                int window_min_index = index_of_event + (int)(100 * t_window_min);
                int window_max_index = index_of_event + (int)(100 * t_window_max);

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
                   index_of_event,
                   datafile.time_stamp_data[index_of_event],
                   datafile.relative_time_data[index_of_event],
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
                   datafile.n_of_data_points, scan_type.ToString());
            }
            else
            {
                simplotDatabaseDataSet.data.AdddataRow(filename, Path.GetFileName(filename), "", "",
                   -1,
                   new DateTime(),
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
                   datafile.n_of_data_points, "BAD_SET");

            }

//            adapter.Update(simplotDatabaseDataSet);

//            simplotDatabaseDataSet.AcceptChanges();

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

                if (File.Exists(changed_filename))
                {

                    current_datafile = new CDataFile(changed_filename, radCheckBox_spike_filter.Checked, spike_filter_accel, radCheckBox_low_pass_filter.Checked, low_pass_filter_frequency, running_average_size, local_standard_deviation_size);

                    if (event_file_index != -1)
                    {

                        if (radCheckBox_jump_to_event.Checked)
                        {
                            current_file_index = event_file_index;
                        }

                        Application.DoEvents();
                        current_datafile.ChartData(radChartView_speed_over_time, horizontalAxis_seconds, verticalAxis, current_file_index - chart_sample_size / 2, chart_sample_size);
                        ChartLogic();


                        radPropertyGrid1.SelectedObject = current_datafile;

                        // create the video list

                        double event_time = double.Parse(current_datafile.time_data[current_file_index]);

                        ListVideo(event_time, filename);
                    }
                }
                else
                {
                    this.radDesktopAlert1.CaptionText = "File Error";
                    this.radDesktopAlert1.ContentText = "Data file does not exist at indicated path.\nConsider changing the drive letter.";
                    this.radDesktopAlert1.Show();
                }

               
            }

            Cursor = Cursors.Default;
            Application.DoEvents();
        }

        private void radGridView_data_CellValueChanged(object sender, GridViewCellEventArgs e)
        {

            SimplotDatabaseDataSetTableAdapters.dataTableAdapter a = new SimplotDatabaseDataSetTableAdapters.dataTableAdapter();
            a.Update(simplotDatabaseDataSet);
            simplotDatabaseDataSet.AcceptChanges();

        }

        private void radHScrollBar_index_ValueChanged(object sender, EventArgs e)
        {
            if (current_datafile != null)
            {
                current_datafile.ChartData(radChartView_speed_over_time, horizontalAxis_seconds, verticalAxis, current_file_index - chart_sample_size / 2, chart_sample_size);
                ChartLogic();
            }
        }

        private void radButton_transfer_Click(object sender, EventArgs e)
        {

            SimplotDatabaseDataSetTableAdapters.EventsTableAdapter ae = new EventsTableAdapter();

            ae.Fill(simplotDatabaseDataSet.Events);


            radProgressBar_ReadData.Maximum = simplotDatabaseDataSet.Events.Rows.Count;

            SimplotDatabaseDataSetTableAdapters.dataTableAdapter a = new SimplotDatabaseDataSetTableAdapters.dataTableAdapter();

            int i = 0;
            foreach (SimplotDatabaseDataSet.EventsRow row in simplotDatabaseDataSet.Events.Rows)
            {
                radProgressBar_ReadData.Value1 = i++;

                Application.DoEvents();

                if (!row.Is_EVENT_TYPENull())
                {
                    if (row._EVENT_TYPE != "")
                    {
                        // find equivalent row in main database

                        string filename = row._DATA_FILENAME;
                        int event_index = row._EVENT_INDEX;

                        foreach (SimplotDatabaseDataSet.dataRow data_row in simplotDatabaseDataSet.data.Rows)
                        {
                            if (data_row.DATA_FILENAME == filename)
                              
                            {
                                if (row._FILENAME.Contains(data_row.CONVEYOR))
                                {
                                    data_row.BeginEdit();

                                    data_row.EVENT_TYPE = row._EVENT_TYPE;

                                    data_row.EndEdit();

                                    a.Update(simplotDatabaseDataSet);
                                    simplotDatabaseDataSet.AcceptChanges();
                                }
                            }
                        }
                    }
                }


            }
        }

        private void radCheckBox_spike_filter_CheckStateChanged(object sender, EventArgs e)
        {
            radGridView_data_SelectionChanged(null, null);
        }

        private void radSpinEditor_spike_filter_trigger_accel_ValueChanged(object sender, EventArgs e)
        {
            radGridView_data_SelectionChanged(null, null);
        }

        private void radCheckBox_low_pass_filter_CheckStateChanged(object sender, EventArgs e)
        {
            radGridView_data_SelectionChanged(null, null);
        }

        private void radSpinEditor_low_pass_filter_frequency_ValueChanged(object sender, EventArgs e)
        {
            radGridView_data_SelectionChanged(null, null);
        }

        private void radButton_script_Click(object sender, EventArgs e)
        {
            SimplotDatabaseDataSetTableAdapters.dataTableAdapter a = new SimplotDatabaseDataSetTableAdapters.dataTableAdapter();

            int i = 0;
            ignore_events = true;

            radProgressBar_ReadData.Maximum = simplotDatabaseDataSet.data.Rows.Count;
       
            foreach (SimplotDatabaseDataSet.dataRow data_row in simplotDatabaseDataSet.data.Rows)
            {
                string filename = data_row.FILENAME;
                string datafilename = data_row.DATA_FILENAME;
                string conveyor = data_row.CONVEYOR;

                radProgressBar_ReadData.Value1 = i++;

                Application.DoEvents();


                if (conveyor != "")
                {
                    string n_filename = filename.Substring(0, filename.IndexOf("\\DATA\\")) + "\\DATA\\" + conveyor + "\\LabVIEW Data\\" + datafilename;

                    data_row.BeginEdit();

                    data_row.FILENAME = n_filename;

                    data_row.EndEdit();

                    a.Update(simplotDatabaseDataSet);
                    simplotDatabaseDataSet.AcceptChanges();
                }

            }

            ignore_events = false;
        }

        public void FillInSTDDEVScript()
        {
            SimplotDatabaseDataSetTableAdapters.dataTableAdapter a = new SimplotDatabaseDataSetTableAdapters.dataTableAdapter();

            int i = 0;
            ignore_events = true;

            radProgressBar_ReadData.Maximum = simplotDatabaseDataSet.data.Rows.Count;

            foreach (SimplotDatabaseDataSet.dataRow data_row in simplotDatabaseDataSet.data.Rows)
            {

                radProgressBar_ReadData.Value1 = i++;
                radProgressBar_ReadData.Text = "processing " + i.ToString() + "/" + simplotDatabaseDataSet.data.Rows.Count.ToString();


                Application.DoEvents();
                
                data_row.BeginEdit();

                data_row.SCAN_TYPE = "STD_DEV";

                data_row.EndEdit();

                a.Update(simplotDatabaseDataSet);
                simplotDatabaseDataSet.AcceptChanges();

            }

            ignore_events = false;

        }

        public void RunningAverageScan()
        {
            string[] file_names = Directory.GetFiles(@"\\prod\root\S_Drive\USGR-Shared\SimPlot DATA\DATA", "*.bin", SearchOption.AllDirectories);

            List<string> fn = new List<string>();
            fn.AddRange(file_names);

            fn.Sort();

            file_names = fn.ToArray();

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


                CDataFile data_file = new CDataFile(file_name, radCheckBox_spike_filter.Checked, spike_filter_accel, radCheckBox_low_pass_filter.Checked, low_pass_filter_frequency, running_average_size, local_standard_deviation_size);

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

                    AddDataFileToDataBase(data_file, file_name, SCAN_TYPE.RUNNING_AVR, speed_drop_in_percent);

                    ignore_events = false;

                    Application.DoEvents();

                }

                data_file = null;
            }
            ignore_events = false;


        }

        public void ChartLogic()
        {

            if (current_datafile != null)
            {
                if (radChartView_speed_over_time != null)
                {
                    radChartView_speed_over_time.Series[0].IsVisible = radCheckBox_head_speed.Checked;
                    radChartView_speed_over_time.Series[1].IsVisible = radCheckBox_mid_speed.Checked;
                    radChartView_speed_over_time.Series[2].IsVisible = radCheckBox_tail_speed.Checked;

                    radChartView_speed_over_time.Series[3].IsVisible = radCheckBox_head_speed_running_avr.Checked;
                    radChartView_speed_over_time.Series[4].IsVisible = radCheckBox_mid_speed_running_avr.Checked;
                    radChartView_speed_over_time.Series[5].IsVisible = radCheckBox_tail_speed_running_avr.Checked;

                    radChartView_speed_over_time.Series[6].IsVisible = radCheckBox_all_speeds_running_avr.Checked;
                    radChartView_speed_over_time.Series[7].IsVisible = radCheckBox_all_speeds_local_std_dev.Checked;
                }
            }
        }

        private void radCheckBox_head_speed_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            ChartLogic();
        }

        private void radCheckBox_mid_speed_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            ChartLogic();
        }

        private void radCheckBox_tail_speed_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            ChartLogic();
        }

        private void radCheckBox_head_speed_running_avr_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            ChartLogic();
        }

        private void radCheckBox_mid_speed_running_avr_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            ChartLogic();
        }

        private void radCheckBox_tail_speed_running_avr_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            ChartLogic();
        }

        private void radCheckBox_all_speeds_running_avr_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            ChartLogic();
        }

        private void radCheckBox_all_speeds_local_std_dev_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            ChartLogic();
        }
    }
}
