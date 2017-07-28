using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichaelsDataManipulator
{
    public class CEvent //: IComparable<CEvent>
    {

        public string filename { get; set; }
        public string data_filename
        {
            get
            {
                return Path.GetFileName(filename);
            }
        }

        public string conveyor

        {
            get
            {
                int i = filename.IndexOf("\\CB");

                return filename.Substring(i + 1, 6);
            }

        }

        public int index_of_event { get; set; }

        public string time { get; set; }
        public double time_of_event_relative_to_start_of_data_file { get; set; } // seconds
        public DateTime time_stamp_event { get; set; }


        public CEvent(string file_name, int index_of_event,string time,  double time_relative_to_start_of_data_file, DateTime time_stamp)
        {
            
            this.filename = file_name;
            this.index_of_event = index_of_event;
            this.time = time;
            this.time_of_event_relative_to_start_of_data_file = time_relative_to_start_of_data_file;
            this.time_stamp_event = time_stamp;
        }

        //public int CompareTo(CEvent other)
        //{
        //    if (this.sum_maximum > other.sum_maximum)
        //    {
        //        return -1;
        //    }
        //    else
        //    {
        //        return 1;
        //    }

        //}
    }
}
