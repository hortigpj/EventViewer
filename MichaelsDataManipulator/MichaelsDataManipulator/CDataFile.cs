using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Telerik.Charting;
using Telerik.WinControls.UI;
using UnitsNet;

namespace MichaelsDataManipulator
{
    public class CDataFile 
    {
        public enum SCAN_TYPE
        {
            STD_DEV,
            MAX_SPEED,
            RUNNING_AVR,
            MULTIPLE_RUNNING_AVR, 
            BAD_SET
        };

        public SCAN_TYPE scan_type { get; private set; }


        public List<string> time_data = new List<string>();
        public List<DateTime> time_stamp_data = new List<DateTime>();
        public List<double> relative_time_data = new List<double>();
        public List<double> delta_time_data = new List<double>();


        public List<double>[] data;
        public int n_of_data_points;

        public List<int> event_index_list
        { get; set; } = new List<int>();

        public List<double> event_time_list
        { get; set; } = new List<double>();


        public List<double> event_incline_list
        { get; set; } = new List<double>();


        public int running_avr_event_index
        {
            get; set;
        } = -1;


        public int allow_1_event_per_seconds
        {
            get;private set;
        }



        public bool IsValid
        {
            get
            {
                return maximum > minimum;
            }
        }


        public string percentage_of_good_data
        {
            get
            {
                return (100 * (double)n_of_good_data_points / (double)n_of_data_points).ToString() + "%";
            }
        }


        public List<double> tail_speed = new List<double>();
        public List<double> mid_speed = new List<double>();
        public List<double> head_speed = new List<double>();
        

        public CRunningAverageList tail_speed_running_avr;
        public CRunningAverageList mid_speed_running_avr;
        public CRunningAverageList head_speed_running_avr;

        public List<double> average_running_average_incline  = new List<double>();


        public List<double> sum = new List<double>();
        public List<double> upper = new List<double>();
        public List<double> lower = new List<double>();

        public List<double> average_speed = new List<double>();

        public List<double> mid_speed_peek_frequency = new List<double>();
        public double[,] spectrum;
        public double spectrum_max = double.MinValue;

        public CRunningAverageList running_average;

        public List<double> band_width = new List<double>();
        public CLocalStandardDeviation local_std_dev;
        public List<double> filtered_local_std_dev = new List<double>();
        public List<double> differential_filtered_local_std_dev = new List<double>();

        public List<double> head_speed_std_dev = new List<double>();
        public List<double> mid_speed_std_dev = new List<double>();
        public List<double> tail_speed_std_dev = new List<double>();


        public string filename { get; set; }
        public double total_average {
            get
            {
                if (average_speed.Count > 0)
                {
                    return average_speed.Average();
                }
                return 0;
                
            }
                
        }
        public double total_average_ft_per_min
        {
            get
            {
                return Speed.FromMetersPerSecond(total_average).FeetPerSecond * 60;
            }
        }

        public double sum_maximum;
        
        public double std_dev_of_average { get; set; }

        public int n_of_good_data_points
        {
            get
            {
                return head_speed.Count;
            }
        }

        public double SamplingRate
        {
            get
            {
                return 1 / dt;
            }
        }



        double? _dt = null;
        public double dt
        {
            get
            {
                if (!_dt.HasValue)
                {
                    _dt = delta_time_data.Average();
                }

                return _dt.Value;
            }
        }

        public double maximum { get; set; }
        public double maximum_ft_per_min
        {
            get
            {
                return Speed.FromMetersPerSecond(maximum).FeetPerSecond * 60;
            }
        }

        public double minimum { get; set; }
        public double minimum_ft_per_min
        {
            get
            {
                return Speed.FromMetersPerSecond(minimum).FeetPerSecond * 60;
            }
        }

        public double LocalStdDevMaximum
        {
            get
            {
                return local_std_dev.Max();
            }
        }


        public int IndexOfLocalStdDevMax
        {
            get
            {
                return local_std_dev.IndexOf(LocalStdDevMaximum);
            }
        }

        public int IndexOfSumMaximum
        {
            get
            {
                return sum.IndexOf(sum_maximum);
                
            }
        }

        public int IndexOfMaximum
        {
            get
            {

                int tail_index = tail_speed.IndexOf(maximum);
                int head_index = head_speed.IndexOf(maximum);
                int mid_index = mid_speed.IndexOf(maximum);


                if (tail_index >= 0)
                {
                    return tail_index;
                }
                if (head_index >= 0)
                {
                    return head_index;
                }
                if (mid_index >= 0)
                {
                    return mid_index;
                }

                return -1;

            }
        }



        public DateTime DateStampOfSumMaximum
        {
            get
            {
                if (IndexOfSumMaximum >= 0)
                {
                    return time_stamp_data[IndexOfSumMaximum];
                }
                return new DateTime();
            }
        }

        public DateTime DateStampOfMaximum
        {
            get
            {
                if (IndexOfMaximum >= 0)
                {
                    return time_stamp_data[IndexOfMaximum];
                }
                return new DateTime();
            }
        }

        public string conveyor

        {
            get
            {
                int i = filename.IndexOf("\\CB");

                return filename.Substring(i+1,6); 
            }

        }

        public double start_time_code
        {
            get
            {
                return double.Parse(time_data[0]);
            }
        }

        public double end_time_code
        {
            get
            {
                return double.Parse(time_data.Last());
            }
        }

        public DateTime start_time_stamp
        {
            get
            {
                return time_stamp_data.First();
            }
        }
        public DateTime end_time_stamp
        {
            get
            {
                return time_stamp_data.Last();
            }
        }


        public string duration
        {
            get
            {
                return (relative_time_data.Last().ToString());
            }
        }

        public DateTime video_start_date_time

        {
            get
            {
                return GetVideoTimeStamp(0);
            }
        }

        public string data_filename
        {
            get
            {
                return Path.GetFileName(this.filename);
            }
        }


        public double running_average_size_in_seconds
        {
            get; set;
        }

        public double local_standard_deviation_size_in_seconds
        {
            get; set;
        }

        public int running_average_size
        {
            get
            {
                return (int)(SamplingRate * running_average_size_in_seconds);
            }
        }

        public int local_standard_deviation_size
        {
            get
            {
                return (int)(SamplingRate * local_standard_deviation_size_in_seconds);
            }
        }

        public double local_standard_deviation_trigger
        {
            get; private set;
        }


        public double running_average_drop_trigger
        {
            get;set;
        }


        public CDataFile(string file_name, bool spike_filter, double accel_trigger, bool low_pass_filter, double low_pass_frequency,
            double running_avr_size_in_seconds, double local_std_dev_size_in_seconds, double local_std_dev_trigger, double running_avr_trigger,
            bool test_for_file_integrity,
            string logfile,
            SCAN_TYPE scan_type,
            bool do_fft, 
            int allow_1_event_per_seconds
            )
        {
            this.filename = file_name;
            this.running_average_size_in_seconds = running_avr_size_in_seconds;
            this.local_standard_deviation_size_in_seconds = local_std_dev_size_in_seconds;
            this.local_standard_deviation_trigger = local_std_dev_trigger;

            this.running_average_drop_trigger = running_avr_trigger;
            this.scan_type = scan_type;

            this.allow_1_event_per_seconds = allow_1_event_per_seconds;

            Read(this.filename, logfile);

            if (n_of_good_data_points > 0 && running_average_size > 0 && (!test_for_file_integrity))
            {

                running_average = new CRunningAverageList(running_average_size);

                tail_speed_running_avr = new CRunningAverageList(running_average_size);
                mid_speed_running_avr = new CRunningAverageList(running_average_size);
                head_speed_running_avr = new CRunningAverageList(running_average_size);

                local_std_dev = new CLocalStandardDeviation(local_standard_deviation_size);

                if (spike_filter)
                {
                    SpikeFilterData(accel_trigger);
                }

                if (low_pass_filter)
                {
                    LowPassFilterData(100, low_pass_frequency, 100);
                }

                if (n_of_good_data_points > 0)
                {
                    switch (scan_type)
                    {
                        case SCAN_TYPE.MULTIPLE_RUNNING_AVR:
                            {
                                CreateDerivedData2(do_fft);
                                break;
                            }
                        case SCAN_TYPE.RUNNING_AVR:
                            {
                                CreateDerivedData2(do_fft);
                                break;
                            }
                        case SCAN_TYPE.STD_DEV:
                            {
                                CreateDerivedData2(do_fft);
                                break;
                            }
                        case SCAN_TYPE.MAX_SPEED:
                            {
                                CreateDerivedData_MaxSpeed(do_fft);
                                break;
                            }
                    }


                }
            }
        }


        public int GetIndexForTime(double time)
        {
            int index = 0;

            foreach (double t in relative_time_data)
            {
                if (t < time)
                {
                    index++;
                }
            }

            return index;
        }


        void Read(string f_name, string logfile)
        {
            using (StreamWriter log = File.AppendText(logfile))
            {
                using (StreamReader sr = new StreamReader(f_name))
                {
                    string line;

                    UnitsNet.Duration? first_time_value = null;
                    UnitsNet.Duration? relative_time = null;
                    UnitsNet.Duration? last_relative_time = null;

                    // Read and display lines from the file until 
                    // the end of the file is reached. 
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line != "")
                        {

                            try
                            {

                                n_of_data_points++;

                                string[] tokens = line.Split('\t');


                                double seconds_since_1_1_1970 = double.Parse(tokens[0]);

                                UnitsNet.Duration time = UnitsNet.Duration.FromSeconds(seconds_since_1_1_1970);

                                if (!first_time_value.HasValue)
                                {
                                    first_time_value = time;
                                }


                                DateTime date_time = new DateTime((long)(time.Nanoseconds * 0.01));

                                date_time = date_time.AddYears(1969);
                                date_time = date_time.AddDays(-1);

                                time_data.Add(tokens[0]);

                                last_relative_time = relative_time;

                                relative_time = time - first_time_value;

                                UnitsNet.Duration? delta_t = relative_time - last_relative_time;

                                

                                if (delta_t.HasValue)
                                {
                                    delta_time_data.Add(delta_t.Value.Seconds);
                                }
                                else
                                {
                                    delta_time_data.Add(0);
                                }

                                time_stamp_data.Add(date_time);

                                double tail_speed_ft_min = 0;
                                double mid_speed_ft_min = 0;
                                double head_speed_ft_min = 0;

                                if (tokens[6] != "NULL" && tokens[7] != "NULL" && tokens[8] != "NULL")
                                {
                                    try
                                    {
                                        tail_speed_ft_min = double.Parse(tokens[8]);
                                        mid_speed_ft_min = double.Parse(tokens[7]);
                                        head_speed_ft_min = double.Parse(tokens[6]);
                                    }
                                    catch (Exception ex)
                                    {
                                        log.WriteLine("datafile:" + f_name + " bad speed data -> ex = "+ex.ToString() );
                                    }
                                }

                                tail_speed.Add(Speed.FromFeetPerSecond(tail_speed_ft_min / 60).MetersPerSecond);
                                mid_speed.Add(Speed.FromFeetPerSecond(mid_speed_ft_min / 60).MetersPerSecond);
                                head_speed.Add(Speed.FromFeetPerSecond(head_speed_ft_min / 60).MetersPerSecond);

                                relative_time_data.Add(relative_time.Value.Seconds);

                            }

                            catch (Exception ex)
                            {
                                log.WriteLine("datafile:" + f_name + " error -> ex = " + ex.ToString());
                            }
                        }
                    }
                }
            }
        }




        public void CreateDerivedData()
        {
            int n = running_average_size;

            FixedSizeQueue<double> q_time = new FixedSizeQueue<double>(n);

            FixedSizeQueue<double> q_head = new FixedSizeQueue<double>(n);
            FixedSizeQueue<double> q_mid = new FixedSizeQueue<double>(n);
            FixedSizeQueue<double> q_tail = new FixedSizeQueue<double>(n);

            FixedSizeQueue<double> q_average = new FixedSizeQueue<double>(n);


            for (int i = 0; i < relative_time_data.Count(); i++)
            {
                double s = head_speed[i] + tail_speed[i] + mid_speed[i];

                sum.Add(s);

                upper.Add(Math.Max(Math.Max(head_speed[i], mid_speed[i]), tail_speed[i]));
                lower.Add(Math.Min(Math.Min(head_speed[i], mid_speed[i]), tail_speed[i]));

                double a = s / 3;
                
                average_speed.Add(a);

                running_average.AddValue(a);

                band_width.Add(upper[i] - lower[i]);

                double p0 = local_std_dev.Last();

                local_std_dev.AddValue(a);

                double p1 = local_std_dev.Last();

                double diff = (p1 - p0) / dt;

                differential_filtered_local_std_dev.Add(diff);
                
                tail_speed_running_avr.AddValue(tail_speed[i]);
                mid_speed_running_avr.AddValue(mid_speed[i]);
                head_speed_running_avr.AddValue(head_speed[i]);


                if (running_avr_event_index == -1)
                {

                    q_time.Enqueue(relative_time_data[i]);

                    q_head.Enqueue(head_speed_running_avr[i]);
                    q_mid.Enqueue(mid_speed_running_avr[i]);
                    q_tail.Enqueue(tail_speed_running_avr[i]);

                    q_average.Enqueue((head_speed_running_avr[i] + mid_speed_running_avr[i] + tail_speed_running_avr[i]) / 3);


                    double head_delta = q_head.Max() - q_head.Min();
                    double mid_delta = q_mid.Max() - q_mid.Min();
                    double tail_delta = q_tail.Max() - q_tail.Min();


                    if (i > n && a > 0.1)
                    {

                        double[] x = q_time.ToArray();
                        double[] y = q_average.ToArray();

                        Tuple<double, double> p = Fit.Line(x, y);

                        if (p.Item2 < 0)
                        {
                            if (head_delta / q_head.Max() > running_average_drop_trigger)
                            {
                                if (mid_delta / q_mid.Max() > running_average_drop_trigger)
                                {
                                    if (tail_delta / q_tail.Max() > running_average_drop_trigger)
                                    {
                                        running_avr_event_index = i;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            sum_maximum = sum.Max();
            maximum = Math.Max(Math.Max(head_speed.Max(), mid_speed.Max()), tail_speed.Max());
            minimum = Math.Min(Math.Min(head_speed.Min(), mid_speed.Min()), tail_speed.Min());

            std_dev_of_average = ArrayStatistics.StandardDeviation(average_speed.ToArray());

        }


        public void SpikeFilterData(double accel_trigger)
        {
            Debug.WriteLine("Filtering Data");

            head_speed = SpikeFilterSpeedData(relative_time_data, head_speed, accel_trigger);
            mid_speed = SpikeFilterSpeedData(relative_time_data, mid_speed, accel_trigger);
            tail_speed = SpikeFilterSpeedData(relative_time_data, tail_speed, accel_trigger);

        }
        public void LowPassFilterData(double sampling_rate, double cutoff, double bandwidth)
        {
            Debug.WriteLine("Low Pass Filtering Data");

            head_speed = LowPassFilter(head_speed, sampling_rate, cutoff, bandwidth);
            mid_speed = LowPassFilter(mid_speed, sampling_rate, cutoff, bandwidth);
            tail_speed = LowPassFilter(tail_speed, sampling_rate, cutoff, bandwidth);

        }


        List<int> ListOfBadValueIndices(List<double> time_list, List<double> list, double max_accel)
        {
            Debug.WriteLine("Create list of bad indices");

            List<int> bad_values = new List<int>();

            for (int i = 1; i < relative_time_data.Count() - 1; i++)
            {
                double dt0 = time_list[i] - time_list[i - 1];
                double dt1 = time_list[i + 1] - time_list[i];

                double accel0 = (list[i] - list[i - 1]) / dt0;
                double accel1 = (list[i + 1] - list[i]) / dt1;


                if (Math.Sign(accel0) != Math.Sign(accel1))
                {
                    if (Math.Abs(accel0) > max_accel && Math.Abs(accel1) > max_accel)
                    {
                        bad_values.Add(i);
                    }
                }


            }

            return bad_values;

        }


        List<double> SpikeFilterSpeedData(List<double> time_list, List<double> list, double max_accel)
        {

            Debug.WriteLine("Filtering");

            List<int> bad_values = ListOfBadValueIndices(time_list, list, max_accel);

            List<double> return_list = list.ToList();


            foreach (int j in bad_values)
            {
                // find previous good index
                int? j0 = j;

                while (bad_values.Contains(j0.Value))
                {
                    if (j0 > 0)
                    {
                        j0--;
                    }
                    else
                    {
                        j0 = null;
                    }
                }

                // find next good index
                int? j1 = j;

                while (bad_values.Contains(j1.Value))
                {
                    if (j1 < list.Count - 1)
                    {
                        j1++;
                    }
                    else
                    {
                        j1 = null;
                    }
                }

                if (j0.HasValue && j1.HasValue)
                {
                    return_list[j] = (list[j0.Value] + list[j1.Value]) / 2;
                }
            }


            return return_list;
        }


        public List<int> FindEvent(double trigger_high, double trigger_low)
        {
            List<int> events = new List<int>();

            int max=0;

            for (int i = 0; i < relative_time_data.Count; i++)
            {

                if (events.Count > 0)
                {
                    max = events.Max();
                }


                if (head_speed[i] > trigger_high || head_speed[i] < trigger_low)
                {

                    if (max + 100 < i)
                        events.Add(i);
                }
                if (mid_speed[i] > trigger_high || mid_speed[i] < trigger_low)
                {
                    if (max + 100 < i)
                        events.Add(i);
                }
                if (tail_speed[i] > trigger_high || tail_speed[i] < trigger_low)
                {
                    if (max + 100 < i)
                        events.Add(i);
                }
            }
            return events;
        }

        public List<double> LowPassFilter(List<double> x, double sampling_rate, double cutoff, double band_width)
        {
            double[] y = new double[x.Count];

            double RC = 1 / cutoff;
            double dt = 1 / sampling_rate;
            double alpha = dt / (RC + dt);

            y[0] = alpha * x[0];

            for (int i = 1;i<x.Count;i++)
            {
                y[i] = alpha * x[i] + (1 - alpha) * y[i - 1];
            }

            List<double> result = new List<double>();

            result.AddRange(y);


            return result;
        }



        public void ChartData(RadChartView rad_chart_view, LinearAxis horizontalAxis, LinearAxis verticalAxis1, LinearAxis verticalAxis2, LinearAxis verticalAxis3, int index , int width)
        {
            Debug.WriteLine("Creating chart");

            int data_points = width;

            int increment = data_points / 500;

            if (increment < 1)
            {
                increment = 1;
            }



            ScatterLineSeries tail_speed_series = new ScatterLineSeries();
            ScatterLineSeries mid_speed_series = new ScatterLineSeries();
            ScatterLineSeries head_speed_series = new ScatterLineSeries();

            ScatterLineSeries head_speed_std_dev_series = new ScatterLineSeries();
            ScatterLineSeries mid_speed_std_dev_series = new ScatterLineSeries();
            ScatterLineSeries tail_speed_std_dev_series = new ScatterLineSeries();


            ScatterLineSeries average_speed_series = new ScatterLineSeries();
            

            ScatterLineSeries tail_speed_running_avr_series = new ScatterLineSeries();
            ScatterLineSeries mid_speed_running_avr_series = new ScatterLineSeries();
            ScatterLineSeries head_speed_running_avr_series = new ScatterLineSeries();


            ScatterLineSeries mid_speed_fft_series = new ScatterLineSeries();
            

            ScatterLineSeries running_average_series = new ScatterLineSeries();
            //ScatterLineSeries band_width_series = new ScatterLineSeries();

            ScatterLineSeries local_std_dev_series = new ScatterLineSeries();
            ScatterLineSeries filtered_local_std_dev_series = new ScatterLineSeries();


            ScatterLineSeries diff_local_std_dev_series = new ScatterLineSeries();

            ScatterLineSeries average_running_average_incline_series = new ScatterLineSeries();


            tail_speed_series.HorizontalAxis = horizontalAxis;
            tail_speed_series.VerticalAxis = verticalAxis1;

            mid_speed_series.HorizontalAxis = horizontalAxis;
            mid_speed_series.VerticalAxis = verticalAxis1;

            head_speed_series.HorizontalAxis = horizontalAxis;
            head_speed_series.VerticalAxis = verticalAxis1;

            average_speed_series.HorizontalAxis = horizontalAxis;
            average_speed_series.VerticalAxis = verticalAxis1;

            head_speed_std_dev_series.HorizontalAxis = horizontalAxis;
            head_speed_std_dev_series.VerticalAxis = verticalAxis3;

            mid_speed_std_dev_series.HorizontalAxis = horizontalAxis;
            mid_speed_std_dev_series.VerticalAxis = verticalAxis3;

            tail_speed_std_dev_series.HorizontalAxis = horizontalAxis;
            tail_speed_std_dev_series.VerticalAxis = verticalAxis3;


            mid_speed_fft_series.HorizontalAxis = horizontalAxis;
            mid_speed_fft_series.VerticalAxis = verticalAxis2;
            


            tail_speed_running_avr_series.HorizontalAxis = horizontalAxis;
            tail_speed_running_avr_series.VerticalAxis = verticalAxis1;

            mid_speed_running_avr_series.HorizontalAxis = horizontalAxis;
            mid_speed_running_avr_series.VerticalAxis = verticalAxis1;

            head_speed_running_avr_series.HorizontalAxis = horizontalAxis;
            head_speed_running_avr_series.VerticalAxis = verticalAxis1;

            //upper_series.HorizontalAxis = horizontalAxis;
            //upper_series.VerticalAxis = verticalAxis;
            //lower_series.HorizontalAxis = horizontalAxis;
            //lower_series.VerticalAxis = verticalAxis;
            running_average_series.HorizontalAxis = horizontalAxis;
            running_average_series.VerticalAxis = verticalAxis1;
            //band_width_series.HorizontalAxis = horizontalAxis;
            //band_width_series.VerticalAxis = verticalAxis;
            local_std_dev_series.HorizontalAxis = horizontalAxis;
            local_std_dev_series.VerticalAxis = verticalAxis3;

            filtered_local_std_dev_series.HorizontalAxis = horizontalAxis;
            filtered_local_std_dev_series.VerticalAxis = verticalAxis3;


            diff_local_std_dev_series.HorizontalAxis = horizontalAxis;
            diff_local_std_dev_series.VerticalAxis = verticalAxis3;



            average_running_average_incline_series.HorizontalAxis = horizontalAxis;
            average_running_average_incline_series.VerticalAxis = verticalAxis1;


            tail_speed_series.LegendTitle = "Tail Speed";
            mid_speed_series.LegendTitle = "Mid Speed";
            head_speed_series.LegendTitle = "Head Speed";

            average_speed_series.LegendTitle = "Average Speed";

            head_speed_std_dev_series.LegendTitle = "Head Speed Std. Dev.";
            mid_speed_std_dev_series.LegendTitle = "Mid Speed Std. Dev.";
            tail_speed_std_dev_series.LegendTitle = "Tail Speed Std. Dev.";

            mid_speed_fft_series.LegendTitle = "mid speed peek freq.";

            tail_speed_running_avr_series.LegendTitle = "Tail Speed Running Avr.";
            mid_speed_running_avr_series.LegendTitle = "Mid Speed Running Avr.";
            head_speed_running_avr_series.LegendTitle = "Head Speed Running Avr.";


            //upper_series.LegendTitle = "Upper Speed";
            //lower_series.LegendTitle = "Lower Speed";
            running_average_series.LegendTitle = "Running Avr. Speed";
            //band_width_series.LegendTitle = "Band Width";
            local_std_dev_series.LegendTitle = "local Std dev";
            filtered_local_std_dev_series.LegendTitle = "filtered local Std dev";
            diff_local_std_dev_series.LegendTitle = "diff local Std dev";

            average_running_average_incline_series.LegendTitle = "Avr. Run. Avr. Incline";



            double t_min = double.MaxValue;
            double t_max = double.MinValue;


            for (int i = 0; i < data_points; i+= increment)
            {
                int j = index + i;

                if (j>=0 && j < n_of_good_data_points)
                {

                    if (tail_speed.Count - 1 >= j)
                    {
                        tail_speed_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(tail_speed[j]).FeetPerSecond * 60));
                    }
                    if (mid_speed.Count - 1 >= j)
                    {
                        mid_speed_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(mid_speed[j]).FeetPerSecond * 60));
                    }
                    if (head_speed.Count - 1 >= j)
                    {
                        head_speed_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(head_speed[j]).FeetPerSecond * 60));
                    }

                    if (average_speed.Count - 1 >= j)
                    {
                        average_speed_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(average_speed[j]).FeetPerSecond * 60));
                    }

                    if (head_speed_std_dev.Count - 1 >= j)
                    {
                        head_speed_std_dev_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(head_speed_std_dev[j]).FeetPerSecond * 60));
                    }
                    if (mid_speed_std_dev.Count - 1 >= j)
                    {
                        mid_speed_std_dev_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(mid_speed_std_dev[j]).FeetPerSecond * 60));
                    }
                    if (tail_speed_std_dev.Count - 1 >= j)
                    {
                        tail_speed_std_dev_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(tail_speed_std_dev[j]).FeetPerSecond * 60));
                    }



                    if (mid_speed_peek_frequency.Count > 0 && j < mid_speed_peek_frequency.Count && j>=0)
                    {
                        mid_speed_fft_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], mid_speed_peek_frequency[j]));
                    }

                    if (tail_speed_running_avr.Count - 1 >= j)
                    {
                        tail_speed_running_avr_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(tail_speed_running_avr[j]).FeetPerSecond * 60));
                    }

                    if (mid_speed_running_avr.Count - 1 >= j)
                    {
                        mid_speed_running_avr_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(mid_speed_running_avr[j]).FeetPerSecond * 60));
                    }

                    if (head_speed_running_avr.Count - 1 >= j)
                    {
                        head_speed_running_avr_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(head_speed_running_avr[j]).FeetPerSecond * 60));
                    }


                    if (running_average.Count - 1 >= j)
                    {
                        running_average_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(running_average[j]).FeetPerSecond * 60));
                    }

                    if (local_std_dev.Count - 1 >= j)
                    {
                        local_std_dev_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(local_std_dev[j]).FeetPerSecond * 60) );
                    }

                    if (filtered_local_std_dev.Count - 1 >= j)
                    {
                        filtered_local_std_dev_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], filtered_local_std_dev[j]));
                    }

                    if (differential_filtered_local_std_dev.Count - 1 >= j)
                    {
                        diff_local_std_dev_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], differential_filtered_local_std_dev[j]));
                    }

                    if (average_running_average_incline.Count - 1 >= j)
                    {
                        average_running_average_incline_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(average_running_average_incline[j]).FeetPerSecond * 60));
                    }

                    if (relative_time_data[j] > t_max)
                    {
                        t_max = relative_time_data[j];
                    }

                    if (relative_time_data[j] < t_min)
                    {
                        t_min = relative_time_data[j];
                    }
                }
            }

            horizontalAxis.Minimum = t_min;
            horizontalAxis.Maximum = t_max;

            horizontalAxis.MajorStep = 0;

            verticalAxis1.Minimum = 0;
            verticalAxis1.Maximum = 250;
            verticalAxis1.MajorStep = 50;


            rad_chart_view.Series.Clear();

            if (tail_speed_series.DataPoints.Count > 0)
            {
                rad_chart_view.Series.Add(tail_speed_series);
            }
            if (mid_speed_series.DataPoints.Count > 0)
            {
                rad_chart_view.Series.Add(mid_speed_series);
            }
            if (head_speed_series.DataPoints.Count > 0)
            {
                rad_chart_view.Series.Add(head_speed_series);
            }

            if (average_speed_series.DataPoints.Count > 0)
            {
                rad_chart_view.Series.Add(average_speed_series);
            }

            if (tail_speed_std_dev_series.DataPoints.Count > 0)
            {
                rad_chart_view.Series.Add(tail_speed_std_dev_series);
            }
            if (mid_speed_std_dev_series.DataPoints.Count > 0)
            {
                rad_chart_view.Series.Add(mid_speed_std_dev_series);
            }
            if (head_speed_std_dev_series.DataPoints.Count > 0)
            {
                rad_chart_view.Series.Add(head_speed_std_dev_series);
            }



            if (mid_speed_fft_series.DataPoints.Count > 0)
            {
                rad_chart_view.Series.Add(mid_speed_fft_series);
            }
            if (tail_speed_running_avr_series.DataPoints.Count > 0)
            {
                rad_chart_view.Series.Add(tail_speed_running_avr_series);
            }
            if (mid_speed_running_avr_series.DataPoints.Count > 0)
            {
                rad_chart_view.Series.Add(mid_speed_running_avr_series);
            }
            if (head_speed_running_avr_series.DataPoints.Count > 0)
            {
                rad_chart_view.Series.Add(head_speed_running_avr_series);
            }
            if (running_average_series.DataPoints.Count > 0)
            {
                rad_chart_view.Series.Add(running_average_series);
            }
            if (local_std_dev_series.DataPoints.Count > 0)
            {
                rad_chart_view.Series.Add(local_std_dev_series);
            }
            if (filtered_local_std_dev_series.DataPoints.Count > 0)
            {
                rad_chart_view.Series.Add(filtered_local_std_dev_series);
            }
            if (diff_local_std_dev_series.DataPoints.Count > 0)
            {
                rad_chart_view.Series.Add(diff_local_std_dev_series);
            }
            if (average_running_average_incline_series.DataPoints.Count > 0)
            {
                rad_chart_view.Series.Add(average_running_average_incline_series);
            }

            foreach (ScatterSeries series in rad_chart_view.Series)
            {
                series.PointSize = new SizeF(0, 0);
                series.IsVisible = true;
            }

            rad_chart_view.ShowTitle = true;
            rad_chart_view.Title = filename;

        }

        public void QuickChart(Graphics g)
        {
            double t_min = relative_time_data.Min();
            double t_max = relative_time_data.Max();

            double y_min = 0;
            double y_max = 200;

            PointF old_point = new PointF(0, 0);


            for (int i = 0;i<n_of_good_data_points;i++)
            {
                double x = relative_time_data[i] - t_min;
                double y = y_max - head_speed[i];
            }
        }

        public DateTime GetVideoTimeStamp(int i)
        {
            DateTime dt = time_stamp_data[i];

            ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();

            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
            DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(dt, cstZone);

            return cstTime;
        }

        //public int IndexOfRunningAverageDropForAllSpeeds(double trigger_value)
        //{
        //    int n = running_average_size;

        //    FixedSizeQueue<double> q_head = new FixedSizeQueue<double>(n);
        //    FixedSizeQueue<double> q_mid = new FixedSizeQueue<double>(n);
        //    FixedSizeQueue<double> q_tail = new FixedSizeQueue<double>(n);

        //    for (int i=0;i<this.n_of_good_data_points - n;i++)
        //    {
        //        q_head.Enqueue(head_speed_running_avr[i]);
        //        q_mid.Enqueue(mid_speed_running_avr[i]);
        //        q_tail.Enqueue(tail_speed_running_avr[i]);


        //        double head_delta = q_head.Max() - q_head.Min();
        //        double mid_delta = q_mid.Max() - q_mid.Min();
        //        double tail_delta = q_tail.Max() - q_tail.Min();


        //        if (i > n)
        //        {
        //            if (head_delta / q_head.Max() > trigger_value)
        //            {
        //                if (mid_delta/ q_mid.Max() > trigger_value)
        //                {
        //                    if (tail_delta / q_tail.Max() > trigger_value)
        //                    {
        //                        return i;
        //                    }
        //                }
        //            }
        //        }

        //    }

        //    return -1;

        //}

        public static DateTime DateEncodedInFilename(string filename)
        {
            string s = Path.GetFileName(filename);

            string[] parts = s.Split('=');

            string d = parts[0];

            d = d.Remove(0, 3);

            return DateTime.Parse(d);
        }

        public void CreateDerivedData2(bool do_fft)
        {
            double[] frequency_scale = MathNet.Numerics.IntegralTransforms.Fourier.FrequencyScale(1024, 50);

            FixedSizeQueue<double> q_average_fft = new FixedSizeQueue<double>(1026);

            bool scanning_for_running_average_event = true;

            spectrum_max = double.MinValue;

            int n = running_average_size;

            FixedSizeQueue<double> q_time = new FixedSizeQueue<double>(n);

            FixedSizeQueue<double> q_head = new FixedSizeQueue<double>(n);
            FixedSizeQueue<double> q_mid = new FixedSizeQueue<double>(n);
            FixedSizeQueue<double> q_tail = new FixedSizeQueue<double>(n);

            FixedSizeQueue<double> q_average = new FixedSizeQueue<double>(n);

            for (int i = 0; i < relative_time_data.Count(); i++)
            {
                double s = head_speed[i] + tail_speed[i] + mid_speed[i];

                sum.Add(s);

                upper.Add(Math.Max(Math.Max(head_speed[i], mid_speed[i]), tail_speed[i]));
                lower.Add(Math.Min(Math.Min(head_speed[i], mid_speed[i]), tail_speed[i]));

                double a = s / 3;

                average_speed.Add(a);

                // FFT
                if (do_fft)
                {
                    q_average_fft.Enqueue(mid_speed[i]);

                    if (q_average_fft.Count >= 1026)
                    {
                        double[] samples = q_average_fft.ToArray();

                        //samples = MathNet.Numerics.Generate.Sinusoidal(1026, 100, 20, 1.0);

                        MathNet.Numerics.IntegralTransforms.Fourier.ForwardReal(samples, 1024);

                        

                        List<double> my_spectrum = new List<double>();
                                               

                        samples[0] = 0;

                        for (int j = 0; j < 513; j++)
                        {
                            my_spectrum.Add(Math.Abs(samples[j]));

                            if (samples[j] > spectrum_max)
                            {
                                spectrum_max = samples[j];
                            }

                            //spectrum[i, j] = samples[j];

                            //if (bw!= null)
                            //{
                            //    bw.Write((float)frequency_scale[j]);
                            //    bw.Write((float)samples[j]);
                            //}
                        }

                        

                        double peek = my_spectrum.Max();

                        int peek_index = my_spectrum.IndexOf(peek);

                        double peek_freq = frequency_scale[peek_index];

                        mid_speed_peek_frequency.Add(peek_freq);
                    }
                }

                running_average.AddValue(a);

                band_width.Add(upper[i] - lower[i]);


                local_std_dev.AddValue(a);

                tail_speed_running_avr.AddValue(tail_speed[i]);
                mid_speed_running_avr.AddValue(mid_speed[i]);
                head_speed_running_avr.AddValue(head_speed[i]);

                if (running_avr_event_index == -1)
                {

                    q_time.Enqueue(relative_time_data[i]);

                    q_head.Enqueue(head_speed_running_avr[i]);
                    q_mid.Enqueue(mid_speed_running_avr[i]);
                    q_tail.Enqueue(tail_speed_running_avr[i]);

                    q_average.Enqueue((head_speed_running_avr[i] + mid_speed_running_avr[i] + tail_speed_running_avr[i]) / 3);


                    double head_delta = q_head.Max() - q_head.Min();
                    double mid_delta = q_mid.Max() - q_mid.Min();
                    double tail_delta = q_tail.Max() - q_tail.Min();

                    
                    // only done if scanning for multiple running avr
                    if (i > n)
                    {
                        double[] x = q_time.ToArray();
                        double[] y = q_average.ToArray();

                        Tuple<double, double> p = Fit.Line(x, y);

                        double incline = p.Item2;

                        average_running_average_incline.Add(Math.Abs(incline));

                        if (incline > -0.01 && incline < 0.01)
                        {
                            scanning_for_running_average_event = true;
                        }

                        if (incline < 0 && scanning_for_running_average_event && a > 0.1)
                        {
                            if (head_delta / q_head.Max() > running_average_drop_trigger)
                            {
                                if (mid_delta / q_mid.Max() > running_average_drop_trigger)
                                {
                                    if (tail_delta / q_tail.Max() > running_average_drop_trigger)
                                    {
                                        if (scan_type == SCAN_TYPE.MULTIPLE_RUNNING_AVR)
                                        {
                                            if (IsIntervalFree(i))
                                            {
                                                event_index_list.Add(i);
                                                event_incline_list.Add(incline);

                                                scanning_for_running_average_event = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        average_running_average_incline.Add(0);
                    }

                }
            }




            sum_maximum = sum.Max();
            maximum = Math.Max(Math.Max(head_speed.Max(), mid_speed.Max()), tail_speed.Max());
            minimum = Math.Min(Math.Min(head_speed.Min(), mid_speed.Min()), tail_speed.Min());

            std_dev_of_average = ArrayStatistics.StandardDeviation(average_speed.ToArray());

            filtered_local_std_dev = LowPassFilter(local_std_dev, 100, 2, 100);
          
            differential_filtered_local_std_dev = SimpleDifferentiate(filtered_local_std_dev, dt);

            differential_filtered_local_std_dev = LowPassFilter(differential_filtered_local_std_dev, 100, 2, 100);

            List<int> differential_filtered_local_std_dev_roots = FindDownwardCrossRootIndices(differential_filtered_local_std_dev);

            SortedDictionary<double,int> maxima = new SortedDictionary<double,int>(Comparer<double>.Create((x, y) => y.CompareTo(x))); 

            foreach (int i in differential_filtered_local_std_dev_roots)
            {
                if (!maxima.ContainsKey(filtered_local_std_dev[i]))
                {
                    if (filtered_local_std_dev[i] > local_standard_deviation_trigger)
                        maxima.Add(filtered_local_std_dev[i], i);
                }
            }

            maxima.Reverse();

            foreach (KeyValuePair<double, int>  t in maxima)
            {
                if ( IsIntervalFree(t.Value) )
                {
                    event_index_list.Add(t.Value);
                }
            }



        }

        public int NumberOfEventsInIntervall(int intervall_min, int intervall_max)
        {
            int count = 0;

            foreach (int i in event_index_list)
            {
                if (i>= intervall_min && i<= intervall_max)
                {
                    count++;
                }
            }


            return count;
        }

        Tuple<int, int> IntervalForEventIndex(int index)
        {
            int size =  allow_1_event_per_seconds * (int)SamplingRate ;
            int size2 = size / 2;

            return new Tuple<int, int>(index - size2, index + size2);

        }

        bool IsIntervalFree(int index)
        {
            return NumberOfEventsInIntervall(IntervalForEventIndex(index).Item1, IntervalForEventIndex(index).Item2) == 0;
        }


        List<int> FindDownwardCrossRootIndices(List<double> values)
        {
            List<int> result = new List<int>();

            for (int i = 1; i < values.Count(); i++)
            {
                if (values[i-1] >= 0 && values[i] < 0)
                {
                    result.Add(i);
                }
            }

            return result;
        }


        List<double> SimpleDifferentiate(List<double> values, double dt)
        {
            List<double> result = new List<double>();

            result.Add(0);

            for (int i = 1; i<values.Count();i++)
            {
                result.Add((values[i] - values[i - 1]) / dt);
            }

            return result;
        }


        public Tuple<double, double>[] GetSpectrumAtIndex(int index)
        {
            int n = 1026;
            int n2 = n/2;

            if (index < n2)
            {
                index = n2;
            }

            if (index > relative_time_data.Count() - n2)
            {
                index = relative_time_data.Count() - n2;
            }

            double[] frequency_scale = MathNet.Numerics.IntegralTransforms.Fourier.FrequencyScale(1024, 50);

            FixedSizeQueue<double> q_average_fft = new FixedSizeQueue<double>(1026);

            for (int i = index - n2; i < index + n2; i++)
            {
                q_average_fft.Enqueue(mid_speed[i]);
            }

            double[] samples = q_average_fft.ToArray();

            MathNet.Numerics.IntegralTransforms.Fourier.ForwardReal(samples, 1024);

            List<double> my_spectrum = new List<double>();

            samples[0] = 0;

            for (int j = 0; j < 513; j++)
            {
                my_spectrum.Add(Math.Abs(samples[j]));
            }

            Tuple<double, double>[] result = new Tuple<double, double>[513];

            for (int j = 0; j < 513; j++)
            {
                result[j] = new Tuple<double, double>(frequency_scale[j], my_spectrum[j]);
            }

            return result;
        }

        public void CreateDerivedData_MaxSpeed(bool do_fft)
        {
            double[] frequency_scale = MathNet.Numerics.IntegralTransforms.Fourier.FrequencyScale(1024, 50);

            FixedSizeQueue<double> q_average_fft = new FixedSizeQueue<double>(1026);
            
            spectrum_max = double.MinValue;

            int n = running_average_size;

            //FixedSizeQueue<double> q_time = new FixedSizeQueue<double>(n);

            //FixedSizeQueue<double> q_head = new FixedSizeQueue<double>(n);
            //FixedSizeQueue<double> q_mid = new FixedSizeQueue<double>(n);
            //FixedSizeQueue<double> q_tail = new FixedSizeQueue<double>(n);

            //FixedSizeQueue<double> q_average = new FixedSizeQueue<double>(n);

            int std_dev_spike_fiter_size = 5;
            int std_dev_spike_fiter_size2 = std_dev_spike_fiter_size /2;

            FixedSizeQueue<double> std_dev_spike_filter = new FixedSizeQueue<double>(std_dev_spike_fiter_size);


            double file_maximum_speed = double.MinValue;

            int file_maximum_speed_index = -1;

            double threshold = (Speed.FromFeetPerSecond(15.0/60.0)).MetersPerSecond;

            for (int i = 0; i < relative_time_data.Count(); i++)
            {
                double s = head_speed[i] + tail_speed[i] + mid_speed[i];

                upper.Add(Math.Max(Math.Max(head_speed[i], mid_speed[i]), tail_speed[i]));
                lower.Add(Math.Min(Math.Min(head_speed[i], mid_speed[i]), tail_speed[i]));


                sum.Add(s);

                double a = s / 3;

                average_speed.Add(a);
                local_std_dev.AddValue(a);

                head_speed_std_dev.Add(GetStdDevForIndexCenteredWindowInList(head_speed, i, std_dev_spike_fiter_size));
                mid_speed_std_dev.Add(GetStdDevForIndexCenteredWindowInList(mid_speed, i, std_dev_spike_fiter_size));
                tail_speed_std_dev.Add(GetStdDevForIndexCenteredWindowInList(tail_speed, i, std_dev_spike_fiter_size));

                double head = 0, mid = 0, tail = 0;

                
                if (head_speed_std_dev.Last() < threshold)
                {
                    head = head_speed[i];
                }
                if (mid_speed_std_dev.Last() < threshold)
                {
                    mid = mid_speed[i];
                }
                if (tail_speed_std_dev.Last() < threshold)
                {
                    tail = tail_speed[i];
                }

                double max = Math.Max(head, Math.Max(mid, tail));

                if (max > file_maximum_speed)
                {
                    file_maximum_speed = max;

                    file_maximum_speed_index = i;
                }

                if (do_fft)
                {
                    q_average_fft.Enqueue(mid_speed[i]);

                    if (q_average_fft.Count >= 1026)
                    {
                        double[] samples = q_average_fft.ToArray();

                        //samples = MathNet.Numerics.Generate.Sinusoidal(1026, 100, 20, 1.0);

                        MathNet.Numerics.IntegralTransforms.Fourier.ForwardReal(samples, 1024);

                        List<double> my_spectrum = new List<double>();

                        samples[0] = 0;

                        for (int j = 0; j < 513; j++)
                        {
                            my_spectrum.Add(Math.Abs(samples[j]));

                            if (samples[j] > spectrum_max)
                            {
                                spectrum_max = samples[j];
                            }
                        }

                        double peek = my_spectrum.Max();

                        int peek_index = my_spectrum.IndexOf(peek);

                        double peek_freq = frequency_scale[peek_index];

                        mid_speed_peek_frequency.Add(peek_freq);
                    }
                }
            }

            sum_maximum = sum.Max();
            maximum = Math.Max(Math.Max(head_speed.Max(), mid_speed.Max()), tail_speed.Max());
            minimum = Math.Min(Math.Min(head_speed.Min(), mid_speed.Min()), tail_speed.Min());

            std_dev_of_average = ArrayStatistics.StandardDeviation(average_speed.ToArray());

            if (file_maximum_speed_index != -1)
            {
                event_index_list.Add(file_maximum_speed_index);
                event_time_list.Add(relative_time_data[file_maximum_speed_index]);
            }


        }

        double GetStdDevForIndexCenteredWindowInList(List<double> array,int index, int window_size)
        {
            if (window_size % 2 == 0)
            {
                Debugger.Break();
            }

            double[] s = new double[window_size];

            int window_size2 = window_size / 2;

            int n = 0;

            for (int k = -window_size2; k <= window_size2; k++)
            {
                int j = index + k;

                if (j < 0)
                {
                    j = 0;
                }

                if (j > array.Count - 1)
                {
                    j = array.Count - 1;
                }

                s[n++] = array[j];
            }

            return ArrayStatistics.StandardDeviation(s);

        }

    }
}
