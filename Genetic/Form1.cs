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
//                new RandomSolver(Runner.PopSize, Runner.NbCities, new Random(rand.Next())),
//                new AnnealingSolver(Runner.PopSize, 0, 1, new Random(rand.Next())),
                //new AnnealingSolver(Runner.PopSize, 500, 10, new Random(rand.Next())),
                new GeneticSolver(new Random(rand.Next()), Runner.NbCities),
                new GeneticV2(new Random(rand.Next()), Runner.NbCities), 
            };

            var solversWithCallbacks = new List<Tuple<ISolver, Action<int, float, float>>>();
            foreach (var solver in solvers)
            {
                var series = new Series
                {
                    ChartType = SeriesChartType.StepLine,
                    BorderWidth = 3,
                };
                chart1.Series.Add(series);
                solversWithCallbacks.Add(new Tuple<ISolver, Action<int, float, float>>(solver, (int gen, float score, float time) =>
                {
                    Invoke((MethodInvoker) (() =>
                    {
                        series.Points.AddXY(gen, score);
                        series.Name = solver.Name + $" (s{score:0,0} t{time:0})";
                        chart1.Invalidate();
                    }));
                }));
            }

            var runner = new Runner(solversWithCallbacks);
            Task.Run(() => runner.Run(rand));
        }
    }
}
