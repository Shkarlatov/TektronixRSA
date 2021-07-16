using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

using Tektronix.TekRSA;


namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для TektronixControl.xaml
    /// </summary>
    public partial class TektronixControl : UserControl, INotifyPropertyChanged, IDisposable
    {
        private bool disposedValue;

        public TekRSA TekRSA { get; } = new TekRSA();

        private const int UpdateInterval = 100;
        private readonly Timer _timer;
        volatile object _lock = new object();
        public TektronixControl()
        {
            _timer = new Timer(new TimerCallback(RefreshPlot), null, Timeout.Infinite, Timeout.Infinite);


            TekRSA.Error += (o, e) => MessageBox.Show($"{e.Source}: {e.Message}");
            TekRSA.PropertyChanged += (o, e) => FunctionManager.Reset();
            TekRSA.PropertyChanged += (o, e) => Debug.WriteLine(e.PropertyName);

            TekRSA.Connect();

            //TekRSA.Configure.Preset();
            TekRSA.Configure.RefLevel = -80;
            TekRSA.Configure.CenterFreq = 100.0e6;
            //TekRSA.Spectrum.SetDefault();
            TekRSA.Spectrum.RBW = 1e3;
            //TekRSA.Spectrum.VBW = 1e3;
            TekRSA.Spectrum.Span = 4e6;

            TekRSA.Spectrum.Enable = true;
            TekRSA.Spectrum.Traces[0].Enable = true;

            InitializeComponent();

            specWidnows.ItemsSource = Enum.GetValues(typeof(SpectrumWindows)).Cast<SpectrumWindows>();
            specUnits.ItemsSource = Enum.GetValues(typeof(SpectrumVerticalUnits)).Cast<SpectrumVerticalUnits>();
            traceDetector.ItemsSource = Enum.GetValues(typeof(SpectrumDetectors)).Cast<SpectrumDetectors>();
            funcCombobox.ItemsSource = Enum.GetValues(typeof(SpectrumTraceFunction)).Cast<SpectrumTraceFunction>();

            PlotSetup();

            BindingOperations.EnableCollectionSynchronization(Points, _lock);
            DataContext = this;
            _timer.Change(1000, UpdateInterval);

            //AvgManager = new AvgManager();
            //AvgManager.Count = 20;
            //AvgManager.DataAvalible += AvgManager_DataAvalible;
            _ = Task.Factory.StartNew(doWork, TaskCreationOptions.LongRunning).ConfigureAwait(false);
            //_ = Task.Run(doWork)
        }

        private void AvgManager_DataAvalible(object sender, AvgManagerDataEventArgs e)
        {
            UpdatePlotData(e.Data);
        }

        public AvgManager AvgManager { get; private set; }

        private async Task doWork()
        {
            while (true)
            {
                TekRSA.Spectrum.AcquireTrace();
                if (!TekRSA.Spectrum.WaitForTraceReady(100))
                    continue;

                var data = TekRSA.Spectrum.Traces[0].GetData(TekRSA.Spectrum.TraceLength);

                var tmp = FunctionManager.Apply(data);
                // await Task.Yield();
                await Task.Delay(0);
                UpdatePlotData(tmp);

            }
        }

        public SpectrumTraceFunctionManager FunctionManager { get; } = new SpectrumTraceFunctionManager() { Function = SpectrumTraceFunction.Avg };


        void UpdatePlotData(IEnumerable<float> data)
        {
            if (data == null)
                return;
            var step = TekRSA.Spectrum.Span / TekRSA.Spectrum.TraceLength;
            var f_start = TekRSA.Configure.CenterFreq - TekRSA.Spectrum.Span / 2;
            var parallelData = data.AsParallel().AsOrdered();
            var points = parallelData
                .Select((val, index) => new DataPoint((f_start + index * step) / 1e6, val));
            var average = parallelData.Average();



            lock (PlotModel.SyncRoot)
            {
                Points.Clear();
                //foreach (var point in points)
                //{
                //    Points.Add(point);
                //}
                Points.AddRange(points);
                UpdateCenterAnnotation(TekRSA.Configure.CenterFreq);
                UpdateAverageAnnotation(average);
            }
        }

        private void RefreshPlot(object state)
        {
            lock (PlotModel.SyncRoot)
            {
                PlotModel.InvalidatePlot(true);
            }

        }

        public RangeObservableCollection<DataPoint> Points { get; } = new RangeObservableCollection<DataPoint>();

        void UpdateAverageAnnotation(double val)
        {
            var average = (LineAnnotation)PlotModel.Annotations.First(x => (string)x.Tag == "averageLine");
            var averageSignal= (LineAnnotation)PlotModel.Annotations.First(x => (string)x.Tag == "averageSignalLine");

            if(average!=null && averageSignal!=null)
            {
                average.Y = val;
                averageSignal.Y = val+6;
            }

        }
        void UpdateCenterAnnotation(double cf_mhz)
        {
            var annotation = (LineAnnotation)PlotModel.Annotations.First(x => (string)x.Tag == "centerFreqLine");
            if (annotation == null)
                return;
            annotation.X = cf_mhz / 1e6;
        }

        private void PlotSetup()
        {

            var plotModel = new PlotModel();
            //plotModel.Title = "TekRSA";
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Amplitude" });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Unit = "MHz", Title = "Frequency" });
            var series = new LineSeries { LineStyle = LineStyle.Solid, MinimumSegmentLength = 2 };
            plotModel.Series.Add(series);
            //var series2 = new ScatterSeries { MarkerType = MarkerType.Plus, MarkerStroke = OxyColors.Red };
            //plotModel.Series.Add(series2);
            //points = series.Points;

            //signalPoints = series2.Points;
            //PlotModel = plotModel;

            var centerFreqLine = new LineAnnotation()
            {
                Type = LineAnnotationType.Vertical,
                // Color = OxyColors.Black,
                StrokeThickness = 1,
                TextOrientation = AnnotationTextOrientation.Horizontal,
                //Text="Center",
                // TextColor = OxyColors.Gold,
                X = 0,
                Tag = "centerFreqLine"
            };

            var averageLine = new LineAnnotation()
            {
                Type = LineAnnotationType.Horizontal,
                // Color = OxyColors.Black,
                StrokeThickness = 1,
                TextOrientation = AnnotationTextOrientation.Horizontal,
                //Text="Center",
                // TextColor = OxyColors.Gold,
                //X = 100
                Y = 0,
                Tag = "averageLine"
            };

            var averageSignalLine = new LineAnnotation()
            {
                Type = LineAnnotationType.Horizontal,
                // Color = OxyColors.Black,
                StrokeThickness = 1,
                TextOrientation = AnnotationTextOrientation.Horizontal,
                //Text="Center",
                // TextColor = OxyColors.Gold,
                //X = 100
                Y = 0,
                Tag= "averageSignalLine"
            };

            plotModel.Annotations.Add(centerFreqLine);
            plotModel.Annotations.Add(averageLine);
            plotModel.Annotations.Add(averageSignalLine);



            series.ItemsSource = Points;

            PlotModel = plotModel;
        }


        public PlotModel PlotModel { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты)
                    TekRSA?.Dispose();
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                // TODO: установить значение NULL для больших полей
                disposedValue = true;
            }
        }

        // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        // ~TektronixControl()
        // {
        //     // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
