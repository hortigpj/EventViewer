using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichaelsDataManipulator
{
    public class CEvent : IComparable<CEvent>
    {
        public string type { get; set; }
        public double sum_maximum { get; set; }

        public double qt_rank_add_maximum { get; set; }

        public string filename { get; set; }
        public int index_of_maximum { get; set; }
        public DateTime time_of_maximum { get; set; }

        public CEvent(string type,  double max, string file_name, int index_of_max, DateTime time_of_max, double qt_rank_sum_max)
        {
            this.type = type;
            this.sum_maximum = max;
            this.filename = file_name;
            this.index_of_maximum = index_of_max;
            this.time_of_maximum = time_of_max;
            this.qt_rank_add_maximum = qt_rank_sum_max;
        }

        public int CompareTo(CEvent other)
        {
            if (this.sum_maximum > other.sum_maximum)
            {
                return -1;
            }
            else
            {
                return 1;
            }

        }
    }
}
