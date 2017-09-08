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
        FixedSizeQueue<double> window = null;


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
                    window = new FixedSizeQueue<double>(_running_average_length);
                }
            }
        }

        public double WindowLocalMax
        {
            get
            {
                return window.Max();
            }
        }

        public double WindowLocalMin
        {
            get
            {
                return window.Min();
            }
        }

        public double WindowLocalDelta
        {
            get
            {
                return WindowLocalMax - WindowLocalMin;
            }
        }

        public double WindowLocalDeltaPercentage
        {
            get
            {
                if (WindowLocalMax != 0)
                {
                    return WindowLocalDelta / WindowLocalMax;
                }
                return 0;
            }
        }



        public CRunningAverageList(int window_size):base()
        {
            
            _running_average_length = window_size;

            queue = new FixedSizeQueue<double>(_running_average_length);
            window = new FixedSizeQueue<double>(_running_average_length);
        }


        public new void AddValue(double value)
        {
            queue.Enqueue(value);

            double average = queue.Average();

            window.Enqueue(average);

            this.Add(average);
        }



    }
}
