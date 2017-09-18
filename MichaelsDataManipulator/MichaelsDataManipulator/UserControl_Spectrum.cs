using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Telerik.WinControls.UI;
using Telerik.Charting;

namespace MichaelsDataManipulator
{
    public partial class UserControl_Spectrum : UserControl
    {
        Bitmap spectrum_bitmap;


        CDataFile _datafile = null;
        public CDataFile datafile
        {
            get
            {
                return _datafile;
            }
            set
            {
                _datafile = value;
            }
        }

        int _index_from = 1026;
        public int index_from
        {
            get
            {
                return _index_from;
            }
            set
            {
                if (datafile != null)
                {
                    if (value < 513)
                    {
                        value = 513;
                    }
                    if (value > datafile.n_of_good_data_points)
                    {
                        value = datafile.n_of_good_data_points;
                    }

                    _index_from = value;
                }
            }
        }

        int _index_to = 0;
        public int index_to
        {
            get
            {
                return _index_to;
            }
            set
            {
                if (datafile != null)
                {
                    if (value < 0)
                    {
                        value = 0;
                    }
                    if (value > datafile.n_of_good_data_points - 513)
                    {
                        value = datafile.n_of_good_data_points - 513;
                    }

                    _index_to = value;
                }




            }
        }



        public UserControl_Spectrum()
        {
            InitializeComponent();

            //CartesianArea area = this.radChartView_spectrum.GetArea<CartesianArea>();

            //area.ShowGrid = true;
            //CartesianGrid grid = area.GetGrid<CartesianGrid>();
            //grid.DrawHorizontalStripes = true;
            //grid.DrawVerticalStripes = true;


            //area.Axes.Clear();

            //LinearAxis horizontalAxis_seconds = new LinearAxis();
            //horizontalAxis_seconds.LabelFitMode = AxisLabelFitMode.MultiLine;


            //horizontalAxis_seconds.Title = "Time [sec]";

            //horizontalAxis_seconds.Minimum = 0;
            //horizontalAxis_seconds.Maximum = 1;

            //horizontalAxis_seconds.MajorStep = 1;


            //area.Axes.Add(horizontalAxis_seconds);

            //LinearAxis verticalAxis_speed_in_ft_min = new LinearAxis();
            //verticalAxis_speed_in_ft_min.AxisType = AxisType.Second;
            //verticalAxis_speed_in_ft_min.HorizontalLocation = AxisHorizontalLocation.Left;
            //verticalAxis_speed_in_ft_min.Title = "Speed";
            //verticalAxis_speed_in_ft_min.Minimum = 0;
            //verticalAxis_speed_in_ft_min.Maximum = 200;


            //area.Axes.Add(verticalAxis_speed_in_ft_min);

            //ScatterLineSeries test_series = new ScatterLineSeries();

            //test_series.DataPoints.Add(new ScatterDataPoint(0,0));
            //test_series.DataPoints.Add(new ScatterDataPoint(1, 1));


            //radChartView_spectrum.Series.Add(test_series);


            //foreach (ScatterSeries series in radChartView_spectrum.Series)
            //{
            //    series.PointSize = new SizeF(0, 0);
            //    series.IsVisible = true;

            //}

        }

        public void Draw()
        {
            pictureBox_spectrum_canvas.Refresh();
        }


        private void pictureBox_spectrum_canvas_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);


            if (spectrum_bitmap != null)
            {
                e.Graphics.DrawImage(spectrum_bitmap, 0, 0);
            }
        }

        private void radButton_draw_spectrum_Click(object sender, EventArgs e)
        {
            //var a =  radChartView_spectrum.Area;

            if (datafile != null)
            {

                


                spectrum_bitmap = new Bitmap(pictureBox_spectrum_canvas.Width, pictureBox_spectrum_canvas.Height);

                pictureBox_spectrum_canvas.Refresh();

                Application.DoEvents();

                double max = double.MinValue;

                for (int i = index_from;i<index_to;i++)
                {
                    Tuple<double, double>[] spectrum = datafile.GetSpectrumAtIndex(i);

                    foreach (Tuple<double, double> t in spectrum)
                    {
                        if (max < t.Item2)
                        {
                            max = t.Item2;
                        }
                    }
                }

                double scale_x = (double)(index_to - index_from) / (double)spectrum_bitmap.Width;
                double scale_y = (double)(513) / (double)spectrum_bitmap.Width;

                for (int x = 0; x< spectrum_bitmap.Width; x++)
                {
                    int i = index_from + (int)((double)x * scale_x);

                    Tuple<double, double>[] spectrum = datafile.GetSpectrumAtIndex(i);

                    for (int y = 0; y < spectrum_bitmap.Height; y++)
                    {
                        int j = (int)((double)y * scale_y);

                        int s = (int)(255 * spectrum[j].Item2 / max);

                        Color c = Color.FromArgb( 255-s,255-s, 255-s);
                        spectrum_bitmap.SetPixel(x, y, c);

                    }
                    if (x % 10 == 0)
                    {
                        pictureBox_spectrum_canvas.Refresh();
                        Application.DoEvents();
                    }
                }
            }




            pictureBox_spectrum_canvas.Refresh();
        }
    }
}
