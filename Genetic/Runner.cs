using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Genetic
{
    class Runner
    {
        public const int NbCities = 1000;
        public const int PopSize = 1000;
        const int WindowSize = 10; //for time measurement

        private readonly List<Tuple<ISolver, Action<int, float, float>>> _solvers;
        public volatile bool DoRun = true;
        public static Problem _problem; //todo refactor

        public Runner(List<Tuple<ISolver, Action<int, float, float>>> solvers)
        {
            _solvers = solvers;
        }

        private class Instance
        {
            public ISolver Solver { get; }
            public List<Solution> Pop { get; set; }
            public Action<int, float, float> ScoreCallback { get; }
            public float Best { get; set; }

            public Stopwatch Watch { get; }
            public Queue<long> LastTimes { get; }

            public Instance(ISolver solver, List<Solution> pop, Action<int, float, float> scoreCallback)
            {
                Solver = solver;
                Pop = pop;
                ScoreCallback = scoreCallback;
                Best = float.MaxValue;

                Watch = Stopwatch.StartNew();
                LastTimes = new Queue<long>(new long[WindowSize]);
            }
        }

        public void Run(Random rand)
        {
            _problem = Problem.Generate(rand, NbCities);
            var initialPop = GeneratePop(_problem, rand);
            var instances = _solvers.Select(s => new Instance(s.Item1, initialPop, s.Item2)).ToList();
            int generation = 0;
            while (DoRun)
            {
                generation++;
                Parallel.ForEach(instances, instance =>
                {
                    instance.Watch.Restart();
                    var score = instance.Pop.Min(s => s.Score);
                    if (score < instance.Best)
                    {
                        instance.Best = score;
                        instance.ScoreCallback(generation - 1, score, (float) instance.LastTimes.Sum() / WindowSize);
                    }

                    var newBorns = new List<Solution>(instance.Solver.NbChildrenWanted);
                    for (int i = 0; i < instance.Solver.NbChildrenWanted; i++)
                    {
                        var parents = instance.Solver.SelectParents(instance.Pop);
                        var child = instance.Solver.Crossover(parents);
                        instance.Solver.Mutate(child);
                        newBorns.Add(new Solution(child, generation, _problem));
                    }

                    instance.Pop.AddRange(newBorns);
                    instance.Pop = instance.Solver.Extinction(instance.Pop, generation);

                    instance.LastTimes.Enqueue(instance.Watch.ElapsedMilliseconds);
                    instance.LastTimes.Dequeue();
                });
            }
        }

        private List<Solution> GeneratePop(Problem p, Random r)
        {
            var ordered = new int[NbCities];
            for (int i = 0; i < NbCities; i++)
            {
                ordered[i] = i;
            }

            var pop = new List<Solution>(PopSize);
            for (int i = 0; i < PopSize; i++)
            {
                var copy = new int[NbCities];
                Array.Copy(ordered, copy, NbCities);
                copy.Shuffle(r);
                pop.Add(new Solution(copy, 0, p));
            }

            return pop;
        }
    }
}
