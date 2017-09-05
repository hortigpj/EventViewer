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
        public List<string> time_data = new List<string>();
        public List<DateTime> time_stamp_data = new List<DateTime>();
        public List<double> relative_time_data = new List<double>();

        public List<double>[] data;
        public int n_of_data_points;

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

        public List<double> sum = new List<double>();
        public List<double> upper = new List<double>();
        public List<double> lower = new List<double>();

        public List<double> average = new List<double>();

        public CRunningAverageList running_average;

        public List<double> band_width = new List<double>();
        public CLocalStandardDeviation local_std_dev;

        public string filename { get; set; }
        public double total_average { get; set; }
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
                return relative_time_data.Count;
            }
        }

        public double SamplingRate
        {
            get
            {
                return 1 / dt;
            }
        }


        public double dt
        {
            get
            {
                return relative_time_data[1];
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


        public CDataFile(string file_name, bool spike_filter, double accel_trigger, bool low_pass_filter, double low_pass_frequency, double running_avr_size_in_seconds, int local_std_dev_size_in_seconds)
        {
            this.filename = file_name;
            this.running_average_size_in_seconds = running_avr_size_in_seconds;
            this.local_standard_deviation_size_in_seconds = local_std_dev_size_in_seconds;


            Read();

            if (n_of_good_data_points > 0)
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
                    CreateDerivedData();
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


        void Read()
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                string line;

                // Read and display lines from the file until 
                // the end of the file is reached. 
                while ((line = sr.ReadLine()) != null)
                {
                    if (line != "")
                    {
                        n_of_data_points++;

                        string[] tokens = line.Split('\t');

                        try
                        {

                            if (tokens[6] != "NULL" && tokens[7] != "NULL" && tokens[8] != "NULL")
                            {
                                double seconds_since_1_1_1970 = double.Parse(tokens[0]);

                                UnitsNet.Duration time = UnitsNet.Duration.FromSeconds(seconds_since_1_1_1970);

                                DateTime date_time = new DateTime((long)(time.Nanoseconds * 0.01));

                                date_time = date_time.AddYears(1969);
                                date_time = date_time.AddDays(-1);

                                double tail_speed_ft_min = 0;
                                double mid_speed_ft_min = 0;
                                double head_speed_ft_min = 0;


                                tail_speed_ft_min = double.Parse(tokens[8]);
                                mid_speed_ft_min = double.Parse(tokens[7]);
                                head_speed_ft_min = double.Parse(tokens[6]);

                                if (tail_speed_ft_min > 0 && mid_speed_ft_min > 0 && head_speed_ft_min > 0)
                                {
                                    time_data.Add(tokens[0]);
                                    relative_time_data.Add(time.Seconds);
                                    time_stamp_data.Add(date_time);

                                    tail_speed.Add(Speed.FromFeetPerSecond(tail_speed_ft_min / 60).MetersPerSecond);
                                    mid_speed.Add(Speed.FromFeetPerSecond(mid_speed_ft_min / 60).MetersPerSecond);
                                    head_speed.Add(Speed.FromFeetPerSecond(head_speed_ft_min / 60).MetersPerSecond);
                                }
                            }
                        }
                        catch
                        {

                        }

                    }
                }
            }
        }




        public void CreateDerivedData()
        {
            double t_min = relative_time_data.Min();

            for (int i = 0; i < relative_time_data.Count; i++)
            {
                relative_time_data[i] -= t_min;
            }

            //total_average = (head_speed.Average() + mid_speed.Average() + tail_speed.Average()) / 3;

            for (int i = 0; i < relative_time_data.Count(); i++)
            {
                double s = head_speed[i] + tail_speed[i] + mid_speed[i];

                sum.Add(s);

                upper.Add(Math.Max(Math.Max(head_speed[i], mid_speed[i]), tail_speed[i]));
                lower.Add(Math.Min(Math.Min(head_speed[i], mid_speed[i]), tail_speed[i]));

                double a = s / 3;


                average.Add(a);

                running_average.AddValue(a);

                band_width.Add(upper[i] - lower[i]);

                local_std_dev.AddValue(a);

                tail_speed_running_avr.AddValue(tail_speed[i]);
                mid_speed_running_avr.AddValue(mid_speed[i]);
                head_speed_running_avr.AddValue(head_speed[i]);

                if (tail_speed_running_avr.LocalDeltaPercentage > 0.3)
                {
                    
                }


            }

            sum_maximum = sum.Max();
            maximum = Math.Max(Math.Max(head_speed.Max(), mid_speed.Max()), tail_speed.Max());
            minimum = Math.Min(Math.Min(head_speed.Min(), mid_speed.Min()), tail_speed.Min());

            std_dev_of_average = ArrayStatistics.StandardDeviation(average.ToArray());

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
            //var lowPass = MathNet.Filtering.IIR.IirCoefficients.LowPass(sampling_rate, cutoff, band_width);
            //MathNet.Filtering.IIR.OnlineIirFilter filter = new MathNet.Filtering.IIR.OnlineIirFilter(lowPass);

            //double[] result;

            //result = filter.ProcessSamples(value_list.ToArray());


            //result_list.AddRange(result);

            

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



        public void ChartData(RadChartView rad_chart_view, LinearAxis horizontalAxis, LinearAxis verticalAxis, int index , int width)
        {
            Debug.WriteLine("Creating chart");

            int max_data_points = width;

            ScatterLineSeries tail_speed_series = new ScatterLineSeries();
            ScatterLineSeries mid_speed_series = new ScatterLineSeries();
            ScatterLineSeries head_speed_series = new ScatterLineSeries();

            ScatterLineSeries tail_speed_running_avr_series = new ScatterLineSeries();
            ScatterLineSeries mid_speed_running_avr_series = new ScatterLineSeries();
            ScatterLineSeries head_speed_running_avr_series = new ScatterLineSeries();


            //ScatterLineSeries upper_series = new ScatterLineSeries();
            //ScatterLineSeries lower_series = new ScatterLineSeries();

            ScatterLineSeries running_average_series = new ScatterLineSeries();
            //ScatterLineSeries band_width_series = new ScatterLineSeries();
            ScatterLineSeries local_std_dev_series = new ScatterLineSeries();


            tail_speed_series.HorizontalAxis = horizontalAxis;
            tail_speed_series.VerticalAxis = verticalAxis;

            mid_speed_series.HorizontalAxis = horizontalAxis;
            mid_speed_series.VerticalAxis = verticalAxis;

            head_speed_series.HorizontalAxis = horizontalAxis;
            head_speed_series.VerticalAxis = verticalAxis;

            tail_speed_running_avr_series.HorizontalAxis = horizontalAxis;
            tail_speed_running_avr_series.VerticalAxis = verticalAxis;

            mid_speed_running_avr_series.HorizontalAxis = horizontalAxis;
            mid_speed_running_avr_series.VerticalAxis = verticalAxis;

            head_speed_running_avr_series.HorizontalAxis = horizontalAxis;
            head_speed_running_avr_series.VerticalAxis = verticalAxis;

            //upper_series.HorizontalAxis = horizontalAxis;
            //upper_series.VerticalAxis = verticalAxis;
            //lower_series.HorizontalAxis = horizontalAxis;
            //lower_series.VerticalAxis = verticalAxis;
            running_average_series.HorizontalAxis = horizontalAxis;
            running_average_series.VerticalAxis = verticalAxis;
            //band_width_series.HorizontalAxis = horizontalAxis;
            //band_width_series.VerticalAxis = verticalAxis;
            local_std_dev_series.HorizontalAxis = horizontalAxis;
            local_std_dev_series.VerticalAxis = verticalAxis;


            tail_speed_series.LegendTitle = "Tail Speed";
            mid_speed_series.LegendTitle = "Mid Speed";
            head_speed_series.LegendTitle = "Head Speed";

            tail_speed_running_avr_series.LegendTitle = "Tail Speed Running Avr.";
            mid_speed_running_avr_series.LegendTitle = "Mid Speed Running Avr.";
            head_speed_running_avr_series.LegendTitle = "Head Speed Running Avr.";


            //upper_series.LegendTitle = "Upper Speed";
            //lower_series.LegendTitle = "Lower Speed";
            running_average_series.LegendTitle = "Running Avr. Speed";
            //band_width_series.LegendTitle = "Band Width";
            local_std_dev_series.LegendTitle = "local Std dev";

            


            double t_min = double.MaxValue;
            double t_max = double.MinValue;


            for (int i = 0; i < max_data_points; i++)
            {
                int j = index + i;

                if (j>=0 && j < n_of_good_data_points)
                {

                    tail_speed_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(tail_speed[j]).FeetPerSecond * 60));
                    mid_speed_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(mid_speed[j]).FeetPerSecond * 60));
                    head_speed_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(head_speed[j]).FeetPerSecond * 60));

                    tail_speed_running_avr_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(tail_speed_running_avr[j]).FeetPerSecond * 60));
                    mid_speed_running_avr_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(mid_speed_running_avr[j]).FeetPerSecond * 60));
                    head_speed_running_avr_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(head_speed_running_avr[j]).FeetPerSecond * 60));



                    //upper_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(upper[j]).FeetPerSecond * 60));
                    //lower_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(lower[j]).FeetPerSecond * 60));
                    running_average_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(running_average[j]).FeetPerSecond * 60));
                    //band_width_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(band_width[j]).FeetPerSecond * 60));
                    local_std_dev_series.DataPoints.Add(new ScatterDataPoint(relative_time_data[j], Speed.FromMetersPerSecond(local_std_dev[j]).FeetPerSecond * 60));

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

            verticalAxis.Minimum = 0;
            verticalAxis.Maximum = 250;
            verticalAxis.MajorStep = 50;


            rad_chart_view.Series.Clear();
            rad_chart_view.Series.Add(tail_speed_series);
            rad_chart_view.Series.Add(mid_speed_series);
            rad_chart_view.Series.Add(head_speed_series);

            rad_chart_view.Series.Add(tail_speed_running_avr_series);
            rad_chart_view.Series.Add(mid_speed_running_avr_series);
            rad_chart_view.Series.Add(head_speed_running_avr_series);


            rad_chart_view.Series.Add(running_average_series);
            //rad_chart_view.Series.Add(band_width_series);
            rad_chart_view.Series.Add(local_std_dev_series);

            foreach (ScatterSeries series in rad_chart_view.Series)
            {
                series.PointSize = new SizeF(0, 0);
            }

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

        public int IndexOfRunningAverageDropForAllSpeeds(double trigger_value)
        {

            FixedSizeQueue<double> q_head = new FixedSizeQueue<double>(running_average_size);
            FixedSizeQueue<double> q_mid = new FixedSizeQueue<double>(running_average_size);
            FixedSizeQueue<double> q_tail = new FixedSizeQueue<double>(running_average_size);

            for (int i=0;i<this.n_of_good_data_points - running_average_size;i++)
            {
                q_head.Enqueue(head_speed_running_avr[i]);
                q_mid.Enqueue(mid_speed_running_avr[i]);
                q_tail.Enqueue(tail_speed_running_avr[i]);


                double head_delta = q_head.Max() - q_head.Min();
                double mid_delta = q_mid.Max() - q_mid.Min();
                double tail_delta = q_tail.Max() - q_tail.Min();


                if (q_head.Max()  / head_delta > trigger_value)
                {
                    if (q_mid.Max() / mid_delta > trigger_value)
                    {
                        if (q_tail.Max() / tail_delta > trigger_value)
                        {
                            return i;
                        }
                    }
                }


            }

            return -1;

        }


    }
}
