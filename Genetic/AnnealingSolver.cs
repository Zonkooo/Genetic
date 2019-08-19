using System;
using System.Collections.Generic;
using System.Linq;

namespace Genetic
{
    class AnnealingSolver : ISolver
    {
        private readonly double _k;
        private double _temperature;
        private readonly Random _rand;
        public string Name { get; }
        public int NbChildrenWanted { get; }

        /// <param name="initialTemperature">number of iterations before we switch to full greedy</param>
        /// <param name="k">coef applied to temperature</param>
        public AnnealingSolver(int popSize, double initialTemperature, double k, Random rand = null)
        {
            _temperature = initialTemperature;
            _k = k;
            _rand = rand ?? new Random();
            NbChildrenWanted = popSize;
            if (initialTemperature == 0 || k == 0)
                Name = "Greedy Swap";
            else
                Name = $"Annealing {initialTemperature}-{_k}";
        }

        private int i = 0;
        public List<Solution> SelectParents(List<Solution> population)
        {
            return new List<Solution> {population[(i++) % population.Count]};
        }

        public int[] Crossover(List<Solution> parents)
        {
            var x = parents[0].Path;
            var x2 = (int[])x.Clone();

            //choose 2 distinct random numbers
            var from = _rand.Next(x2.Length);
            var to = _rand.Next(x2.Length - 1);
            if (to >= from) to++;

            //swap elements in x2
            var tmp = x2[to];
            x2[to] = x2[from];
            x2[from] = tmp;

            //compute scores
            var scoreOld = parents[0].Score;
            var scoreNew = Runner._problem.Score(x2);

            bool keep = Math.Exp((scoreOld - scoreNew) / (_k * _temperature)) > _rand.NextDouble();

            if (keep)
                return x2;
            else
                return x;
        }

        public void Mutate(int[] x)
        {
        }

        public List<Solution> Extinction(IReadOnlyList<Solution> population, int currentGen)
        {
            if(_temperature > 0)
                _temperature--;
            return population.Where(s => s.Generation == currentGen).ToList();
        }
    }
}
