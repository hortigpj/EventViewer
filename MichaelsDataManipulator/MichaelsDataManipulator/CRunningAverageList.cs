using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichaelsDataManipulator
{
    public class CRunningAverageList: List<double>
    {


        FixedSizeQueue<double> queue = null;

        int _running_average_length;
        
        public int RunningAverageLength
        {
            get
            {
                return _running_average_length;
            }

            set
            {
                if (_running_average_length != value)
                {
                    _running_average_length = value;

                    queue = new FixedSizeQueue<double>(_running_average_length);
                }
            }
        }

        public double LocalMax
        {
            get
            {
                return queue.Max();
            }
        }

        public double LocalMin
        {
            get
            {
                return queue.Min();
            }
        }

        public double LocalDelta
        {
            get
            {
                return LocalMax - LocalMin;
            }
        }

        public double LocalDeltaPercentage
        {
            get
            {
                if (LocalMax != 0)
                {
                    return LocalDelta / LocalMax;
                }
                return 0;
            }
        }



        public CRunningAverageList(int window_size):base()
        {
            
            _running_average_length = window_size;

            queue = new FixedSizeQueue<double>(_running_average_length);
        }


        public new void AddValue(double value)
        {
            queue.Enqueue(value);

            this.Add(queue.Average());
        }



    }
}
