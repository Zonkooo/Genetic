using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Genetic
{
    public partial class Form1 : Form
    {
        private readonly Series _series1;
        private volatile int _current = 0;
        private readonly DateTime _start;

        public Form1()
        {
            InitializeComponent();
            chart1.Series.Clear();
            _series1 = new Series
            {
                Name = "Series1",
                ChartType = SeriesChartType.StepLine,
                BorderWidth = 3,
            };

            chart1.Series.Add(_series1);
            _series1.Points.AddXY(0, 0);

            _start = DateTime.UtcNow;
            Task.Run(() => Walk());
        }

        private void Walk()
        {
            var rand = new Random();
            var sum = 0;
            for (int i = 0; i < 10000; i++)
            {
                Thread.Sleep(300);
                sum += rand.Next(-10, 11);
                _current = sum;

                Invoke((MethodInvoker)UpdateChart);
            }
        }

        private void UpdateChart()
        {
            var now = DateTime.UtcNow;
            _series1.Points.AddXY((now - _start).TotalSeconds, _current);
            chart1.Invalidate();
        }
    }
}
