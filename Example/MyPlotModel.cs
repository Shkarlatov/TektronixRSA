using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Tektronix.TekRSA;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows;
using System.Collections.ObjectModel;
using OxyPlot.Annotations;

namespace WpfApp1
{
    public class MyPlotModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public PlotModel PlotModel { get; private set; }
        public ObservableCollection<ScatterPoint> SignalPoints { get; } = new ObservableCollection<ScatterPoint>();

        private const int UpdateInterval = 100;
        private readonly Timer _timer;
        private bool disposed = false;

        private readonly List<DataPoint> points;
        private readonly List<ScatterPoint> signalPoints;


        Timer _timerFreq;
        public MyPlotModel()
        {
           
            _timer = new Timer(new TimerCallback(Refresh), null, Timeout.Infinite, Timeout.Infinite);

            var plotModel = new PlotModel();
            plotModel.Title = "TekRSA";
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Unit = "dBm", Title = "Amplitude" });
            //plotModel.Axes.Add(new LogarithmicAxis { Position = AxisPosition.Right, Unit = "dBuV", Title = "Amplitude" });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Unit = "MHz", Title = "Frequency" });
            var series = new LineSeries { LineStyle = LineStyle.Solid, MinimumSegmentLength = 2 };
            plotModel.Series.Add(series);

            var series2 = new ScatterSeries { MarkerType = MarkerType.Plus, MarkerStroke = OxyColors.Red };
            plotModel.Series.Add(series2);
            points = series.Points;

            signalPoints = series2.Points;
            PlotModel = plotModel;
            _timer.Change(1000, UpdateInterval);

            _ = Task.Run(doWork);
        }


        private readonly TekRSA tek = new TekRSA();
        private void doWork()
        {
            tek.Connect();
            tek.Configure.Preset();
            tek.Configure.RefLevel = -80;
            tek.Configure.CenterFreq = 100.0e6;
            //tek.Configure.CenterFreq = 433e6;
            tek.Spectrum.SetDefault();

            var trace = tek.Spectrum.Traces[0];
            // trace.Detector = SpectrumDetectors.PosPeak;
            //trace.Enable = true;

            //t.Spectrum.Traces[1].Enable = false;
            //t.Spectrum.Traces[2].Enable = false;

            tek.Spectrum.VerticalUnits = SpectrumVerticalUnits.dBm;

            tek.Spectrum.RBW = 10e3;
            tek.Spectrum.VBW = 1e3;
           // tek.Spectrum.EnableVBW = true;
            //tek.Spectrum.TraceLength = 6001;
            tek.Spectrum.Span = 10e6;
            tek.Spectrum.Enable = true;
            // tek.Spectrum.AcquireTrace();


            // cts = new CancellationTokenSource();
            //  Task.Run(DoWork, cts.Token);
            //// DoWork();

            // // dBmV to dbuV      dBmV+60=dBuV
            // //  var data = trace.GetData(900);

            _ = Task.Run(() =>
            {

               // OnlineFilter filter =
               // new OnlineMedianFilter(9);
                //new OnlineFirFilter(new List<double> { 0.14,0.88});
              
               



                var s = tek.Spectrum;
                var t = tek.Spectrum.Traces[0];
                var f_start = tek.Configure.CenterFreq - (tek.Spectrum.Span / 2);
                var f_stop = tek.Configure.CenterFreq + (tek.Spectrum.Span / 2);
                while (true)
                {
                    s.AcquireTrace();
                    if (!s.WaitForTraceReady())
                        continue;

                    var data = t.GetData(s.TraceLength);
                    //  for (int i = 0; i < data.Length; i++)
                    //      data[i] += 60; //  dBmV to dbuV      dBmV+60=dBuV
                    var d = data.Select(x => (double)x).ToArray();


                    //d = filter.ProcessSamples(d);
                    

                    avg2(d, 10);

                    //d = avg(d, 20);
                    //if (d != null)
                    //{
                    //    Update(f_start, f_stop, d);
                    //    //Thread.Sleep(100);
                    //}


                }
            });
        }


        double[][] tmp2;
        int tmp2_index;
        void avg2(double[] data, int avg = 0)
        {
            var res = new double[data.Length];
            if (avg == 0)
            {
                res = data;
                goto skip;
            }



            if (tmp2 == null)
                tmp2 = new double[avg][];
            if (tmp2_index < avg)
            {
                if (tmp2[tmp2_index] == null)
                    tmp2[tmp2_index] = new double[data.Length];

                data.CopyTo(tmp2[tmp2_index], 0);
                tmp2_index++;
                return;
            }
            tmp2_index = 0;



            for (int y = 0; y < data.Length; y++)
            {
                for (int i = 0; i < avg; i++)
                {
                    res[y] += tmp2[i][y];
                }
                res[y] /= avg;
            }
            skip:
            var f_start = tek.Configure.CenterFreq - (tek.Spectrum.Span / 2);
            var f_stop = tek.Configure.CenterFreq + (tek.Spectrum.Span / 2);

            Update(f_start, f_stop, res);

        }


        double[] tmp;
        int avg_current;

        double[] avg(double[] data, int avg = 0)
        {
            if (avg == 0)
                return data;
            if (tmp == null || tmp.Length != data.Length)
            {
                tmp = new double[data.Length];
                data.CopyTo(tmp, 0);
            }



            for (int i = 0; i < data.Length; i++)
            {
                tmp[i] = (tmp[i] + data[i]) / 2;
            }
            if (avg_current < avg)
            {
                avg_current++;
                return null;
            }
            avg_current = 0;
            return tmp;

        }


        private void Refresh(object state)
        {
            lock (this.PlotModel.SyncRoot)
            {
                // this.Update();
                PlotModel.Annotations.Clear();
                var annotation = new LineAnnotation();
                annotation.Color = OxyColors.Blue;
                // annotation.MinimumY = 10;
                // annotation.MaximumY = 40;
                // annotation.X = 5;
                annotation.Y = means;
                annotation.Text = "AVG: " + means.ToString();
                annotation.LineStyle = LineStyle.Solid;
                annotation.Type = LineAnnotationType.Horizontal;
                PlotModel.Annotations.Add(annotation);


                var annotation2 = new LineAnnotation();
                annotation2.Color = OxyColors.Gray;
                // annotation.MinimumY = 10;
                // annotation.MaximumY = 40;
                // annotation.X = 5;
                annotation2.Y = means + 6;
                annotation2.Text = "AVG +6: " + (means + 6).ToString();
                annotation2.LineStyle = LineStyle.Solid;
                annotation2.Type = LineAnnotationType.Horizontal;
                PlotModel.Annotations.Add(annotation2);


            }
            this.PlotModel.InvalidatePlot(true);
            Application.Current.Dispatcher.Invoke(() =>
            {
                SignalPoints.Clear();

                for (int i = 0; i < signalPoints.Count; i++)
                {
                    SignalPoints.Add(signalPoints[i]);
                }
            });

        }

        object _lock = new object();
        double means;
        public void Update(double f_start, double f_stop, double[] data)
        {
           
            
            lock (_lock)
            {
                var step = (f_stop - f_start) / data.Length;

                var p = data.Select((val, index) => new DataPoint((f_start + index * step) / 1e6, val)).ToArray();


                points.Clear();
                points.AddRange(p);

                
                   means = p.Average(x => x.Y);
                //means = -Math.Sqrt(p.Select(x=>x.Y*x.Y).Sum()/p.Length);
                var signal = p.Where(x => x.Y > means + 6).Select(x => new ScatterPoint(x.X, x.Y)).ToArray();

                signalPoints.Clear();
                signalPoints.AddRange(signal);




            }
          

        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this._timer.Dispose();
                    tek.Dispose();
                }
            }

            this.disposed = true;
        }
    }
}
