using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Genetic
{
    class GeneticSolver : ISolver
    {
        protected readonly Random _rand;
        public string Name { get; protected set; }
        public int NbChildrenWanted { get; }
        protected int _nbCities;

        public GeneticSolver(Random rand, int nbCities)
        {
            _rand = rand;
            _nbCities = nbCities;
            NbChildrenWanted = _nbCities;
            Name = "Genetic v1";
        }

        public List<Solution> SelectParents(List<Solution> population)
        {
            List<Solution> ret = new List<Solution>();
            
            double maxScore = population.Select(p => p.Score).Max();
            double total = population.Sum(p => maxScore - p.Score);
            double criteria = _rand.NextDouble() * total;

            double totalScoreSoFar = 0;
            int j = 0;
            for (int i = 0; i < population.Count; i++)
            {
                totalScoreSoFar += maxScore - population[i].Score;
                if (totalScoreSoFar >= criteria)
                {
                    ret.Add(population[i]);
                    total -= maxScore - population[i].Score;
                    j = i;
                    break;
                }
            }

            criteria = _rand.NextDouble() * total;

            totalScoreSoFar = 0;
            for (int i = 0; i < population.Count; i++)
            {
                if(j == i) continue;

                totalScoreSoFar += maxScore - population[i].Score;
                if (totalScoreSoFar >= criteria)
                {
                    ret.Add(population[i]);
                    break;
                }
            }

            return ret;
        }

        public int[] Crossover(List<Solution> parents)
        {
            var seen = new HashSet<int>();
            var idxPar1 = _rand.Next(_nbCities);

            var child = new List<int>();
            for (int i = idxPar1; i < idxPar1 + _nbCities/2; i++)
            {
                var city = parents[0].Path[i % _nbCities];
                child.Add(city);
                seen.Add(city);
            }

            var idxPar2 = parents[1].Path.First(x => x == child.Last());
            for (int i = idxPar2; i < idxPar2 + _nbCities; i++)
            {
                var city = parents[1].Path[i % _nbCities];
                if (!seen.Contains(city))
                {
                    child.Add(city);
                }
            }

            return child.ToArray();
        }

        public virtual void Mutate(int[] x)
        {
            ;
        }

        public List<Solution> Extinction(IReadOnlyList<Solution> population, int currentGen)
        {
            return population.Where(s => s.Generation == currentGen).ToList();
        }
    }

    class GeneticV2 : GeneticSolver
    {
        public GeneticV2(Random rand, int nbCities)
        : base(rand, nbCities)
        {
            Name = "Genetic V2";
        }
        public override void Mutate(int[] x)
        {
            if (_rand.NextDouble() < 0.9)
            {
                return;
            }

            int idx1 = _rand.Next(_nbCities);
            int idx2 = _rand.Next(_nbCities);

            int temp = x[idx1];
            x[idx1] = x[idx2];
            x[idx2] = temp;
        }

        public List<Solution> Extinction(IReadOnlyList<Solution> population, int currentGen)
        {
            return population.OrderByDescending(x => x.Score).Skip((int)(0.9*population.Count)).ToList();
        }
    }
}
