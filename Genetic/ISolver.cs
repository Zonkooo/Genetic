using System;
using System.Collections.Generic;

namespace Genetic
{
    public static class Helper
    {
        public static void Shuffle(this int[] t, Random r)
        {
            int n = t.Length;
            while (n > 1)
            {
                int k = r.Next(n--);
                var tmp = t[n];
                t[n] = t[k];
                t[k] = tmp;
            }
        }
    }

    class Point2D
    {
        public readonly float X;
        public readonly float Y;

        public Point2D(float x, float y)
        {
            X = x;
            Y = y;
        }
        public Point2D(double x, double y)
        {
            X = (float)x;
            Y = (float)y;
        }

        public float Distance(Point2D other)
        {
            var diffx = X - other.X;
            var diffy = Y - other.Y;
            return (float)Math.Sqrt(diffx * diffx + diffy * diffy);
        }
    }

    class Problem
    {
        public IReadOnlyList<Point2D> Cities { get; }
        
        public Problem(IReadOnlyList<Point2D> cities)
        {
            Cities = cities;
        }

        public static Problem Generate(Random r, int size)
        {
            var positions = new List<Point2D>(size);
            for(int i = 0; i < size; i++)
            {
                positions.Add(new Point2D(r.NextDouble()*1000, r.NextDouble()*1000));
            }
            return new Problem(positions);
        }

        public float Score(int[] path)
        {
            float cost = 0;
            var prev = Cities[path[0]];
            for (int i = 1; i < path.Length; i++)
            {
                var current = Cities[path[i]];
                cost += prev.Distance(current);
                prev = current;
            }

            return cost;
        }
    }

    class Solution
    {
        public int[] Path { get; }
        public int Generation { get; }
        public float Score { get; }

        public Solution(int[] path, int generation, Problem p)
        {
            Path = path;
            Generation = generation;
            Score = p.Score(Path);
        }
    }

    interface ISolver
    {
        string Name { get; }
        int NbChildrenWanted { get; }
        /// <summary>
        /// generate one set of parents that are going to generate one child
        /// this method is called several times, each list returned will yield one child
        /// </summary>
        List<Solution> SelectParents(List<Solution> population);

        /// <summary> create a child from the list of parents </summary>
        int[] Crossover(List<Solution> parents);

        /// <summary> optionally apply a mutation on the newly generated child </summary>
        /// <param name="x"></param>
        void Mutate(int[] x);

        /// <summary> kill some of the population </summary>
        /// <returns> the list of survivors </returns>
        List<Solution> Extinction(IReadOnlyList<Solution> population, int currentGen);
    }
}
