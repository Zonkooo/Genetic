using System;
using System.Collections.Generic;
using System.Linq;

namespace Genetic
{
    class RandomSolver : ISolver
    {
        private readonly int _nbCities;
        private readonly Random _rand;
        public string Name => "Random" + new Random().Next(100);
        public int NbChildrenWanted { get; }

        public RandomSolver(int popSize, int nbCities, Random rand = null)
        {
            _nbCities = nbCities;
            _rand = rand ?? new Random();
            NbChildrenWanted = popSize;
        }

        public List<Solution> SelectParents(List<Solution> population)
        {
            return new List<Solution>();
        }

        public int[] Crossover(List<Solution> parents)
        {
            var ordered = new int[_nbCities];
            for (int i = 0; i < _nbCities; i++)
                ordered[i] = i;
            return ordered;
        }

        public void Mutate(int[] x)
        {
            x.Shuffle(_rand);
        }

        public List<Solution> Extinction(IReadOnlyList<Solution> population, int currentGen)
        {
            return population.Where(s => s.Generation == currentGen).ToList();
        }
    }
}
