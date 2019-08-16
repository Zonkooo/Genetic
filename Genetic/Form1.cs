using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Genetic
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            chart1.Series.Clear();

            var rand = new Random(0);
            var solvers = new ISolver[]
            {
                new RandomSolver(Runner.PopSize, Runner.NbCities, new Random(rand.Next())),
                new AnnealingSolver(Runner.PopSize, 600, 1, new Random(rand.Next())),
            };

            var solversWithCallbacks = new List<Tuple<ISolver, Action<int, float>>>();
            Parallel.ForEach(solvers, solver =>
            {
                var series = new Series
                {
                    ChartType = SeriesChartType.StepLine,
                    BorderWidth = 3,
                };
                chart1.Series.Add(series);
                solversWithCallbacks.Add(new Tuple<ISolver, Action<int, float>>(solver, (int gen, float score) =>
                {
                    Invoke((MethodInvoker) (() =>
                    {
                        series.Points.AddXY(gen, score);
                        series.Name = solver.Name + $" ({score:0,0.0})";
                        chart1.Invalidate();
                    }));
                }));
            });

            var runner = new Runner(solversWithCallbacks);
            Task.Run(() => runner.Run(rand));
        }
    }
}
