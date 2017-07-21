using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using UnitsNet;

namespace MichaelsDataManipulator
{
    public class CDataFile 
    {
        public List<DateTime> time_stamp_data = new List<DateTime>();
        public List<double> time_data = new List<double>();
        public List<double> tail_speed = new List<double>();
        public List<double> mid_speed = new List<double>();
        public List<double> head_speed = new List<double>();
        public List<double> sum = new List<double>();
        public List<double> upper = new List<double>();
        public List<double> lower = new List<double>();

        public List<double> average = new List<double>();
        public List<double> running_average = new List<double>();
        public List<double> band_width = new List<double>();
        public List<double> local_std_dev = new List<double>();

        public string filename;
        public double total_average;
        public double maximum;
        public double sum_maximum;
        public double minimum;
        public double std_dev_of_average;

        public int drive_bar_skipping_index=0;
        public DateTime drive_bar_skipping_date_time;

        public int Count
        {
            get
            {
                return time_data.Count;
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


        public CDataFile(string file_name, double accel_trigger )
        {
            this.filename = file_name;
            Read();

            if (Count > 0)
            {
                FilterData(accel_trigger);
            }
        }

        public int GetIndexForTime(double time)
        {
            int index = 0;

            foreach (double t in time_data)
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

                        string[] tokens = line.Split('\t');

                        try
                        {

                            if (tokens[6] != "NULL" && tokens[7] != "NULL" && tokens[8] != "NULL")
                            {

                                double seconds_since_1_1_1970 = double.Parse(tokens[0]);

                                UnitsNet.Duration time = UnitsNet.Duration.FromSeconds(seconds_since_1_1_1970);

                                //time += UnitsNet.Duration.FromYears(1970);

                                double years = time.Years;

                                TimeSpan time_stamp = time.ToTimeSpan();

                                DateTime date_time = new DateTime((long)(time.Nanoseconds * 0.01));

                                date_time = date_time.AddYears(1970);

                                double tail_speed_ft_min = 0;
                                double mid_speed_ft_min = 0;
                                double head_speed_ft_min = 0;


                                tail_speed_ft_min = double.Parse(tokens[8]);
                                mid_speed_ft_min = double.Parse(tokens[7]);
                                head_speed_ft_min = double.Parse(tokens[6]);

                                if (tail_speed_ft_min > 10 && mid_speed_ft_min > 10 && head_speed_ft_min > 10)
                                {
                                    time_data.Add(time.Seconds);
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

        public void FilterData(double accel_trigger)
        {
            Debug.WriteLine("Filtering Data");

            double t_min = time_data.Min();

            for (int i = 0; i< time_data.Count; i++)
            {
                time_data[i] -= t_min;
            }

            //List<double>[] speed_data = new List<double>[3];

            //speed_data[0] = head_speed.ToList<double>();
            //speed_data[1] = mid_speed.ToList<double>();
            //speed_data[2] = tail_speed.ToList<double>();


            

            head_speed = FilterSpeedData(time_data, head_speed, accel_trigger);
            mid_speed = FilterSpeedData(time_data, mid_speed, accel_trigger);
            tail_speed = FilterSpeedData(time_data, tail_speed, accel_trigger);

            total_average = (head_speed.Average() + mid_speed.Average() + tail_speed.Average()) / 3;

            for (int i = 0; i < time_data.Count(); i++)
            {
                double s = head_speed[i] + tail_speed[i] + mid_speed[i];

                sum.Add(s);

                upper.Add(Math.Max(Math.Max(head_speed[i], mid_speed[i]), tail_speed[i]));
                lower.Add(Math.Min(Math.Min(head_speed[i], mid_speed[i]), tail_speed[i]));

                average.Add(s/3);

                
            }

            double[] std_dev = new double[20];

            // running average
            for (int i = 0; i < time_data.Count(); i++)
            {

                double r_avr_sum = 0;

                int l = 0;
                for (int j = -10; j < 10; j++)
                {
                    int k = i + j;

                    if (k < 0)
                        k = 0;

                    if (k > this.Count - 1)
                        k = Count - 1;

                    r_avr_sum += average[k];

                    std_dev[l] = average[k];

                    l++;
                }

                running_average.Add(r_avr_sum / (double)l);

                band_width.Add(upper[i] - lower[i]);

                local_std_dev.Add(ArrayStatistics.StandardDeviation(std_dev));
                
            }

            sum_maximum = sum.Max();
            maximum = Math.Max(Math.Max(head_speed.Max(), mid_speed.Max()),tail_speed.Max());
            minimum = Math.Min(Math.Min(head_speed.Min(), mid_speed.Min()), tail_speed.Min());

            std_dev_of_average = ArrayStatistics.StandardDeviation(average.ToArray());
        }


        List<int> ListOfBadValueIndices(List<double> time_list, List<double> list, double max_accel)
        {
            Debug.WriteLine("Create list of bad indices");

            List<int> bad_values = new List<int>();

            for (int i = 1; i < time_data.Count() - 1; i++)
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


        List<double> FilterSpeedData(List<double> time_list, List<double> list, double max_accel)
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

            for (int i = 0; i < time_data.Count; i++)
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


        public bool DetectDriveBarSkipping(double avr_threshold, double abs_min, double band_width_threshold)
        {
            for (int i = 0; i < time_data.Count; i++)
            {
                if (running_average[i] < avr_threshold && running_average[i] > abs_min && band_width[i] > band_width_threshold)
                {
                    drive_bar_skipping_index = i;
                    drive_bar_skipping_date_time = time_stamp_data[i];
                    return true;
                }

            }
            return false;

        }

    }
}
