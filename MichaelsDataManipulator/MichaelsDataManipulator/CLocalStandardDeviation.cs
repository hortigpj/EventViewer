using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichaelsDataManipulator
{
    public class CLocalStandardDeviation : List<double>
    {
        FixedSizeQueue<double> queue = null;

        int _local_std_dev_length;

        public int LocalStandardDeviationLength
        {
            get
            {
                return _local_std_dev_length;
            }

            set
            {
                if (_local_std_dev_length != value)
                {
                    _local_std_dev_length = value;

                    queue = new FixedSizeQueue<double>(_local_std_dev_length);
                }
            }
        }


        public CLocalStandardDeviation(int window_size) :base()
        {
            _local_std_dev_length = window_size;

            queue = new FixedSizeQueue<double>(_local_std_dev_length);
        }


        public new void AddValue(double value)
        {
            queue.Enqueue(value);
            this.Add(ArrayStatistics.StandardDeviation(queue.ToArray()));
        }




    }
}
