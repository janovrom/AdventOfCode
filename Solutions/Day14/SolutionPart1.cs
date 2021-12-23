using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day14
{
    internal class SolutionPart1 : IntSolution
    {

        public int Solve()
        {
            IEnumerable<string> lines = File.ReadAllLines("./Content/Day14/1.txt");

            // Too tired to think, brute force it
            List<int> polymer = lines.First().ToList().Select(c => (int)c).ToList();
            Dictionary<int, int> polymerTemplates = new();
            foreach (string line in lines.Skip(2))
            {
                string[] template = line.Split(" -> ");
                polymerTemplates.Add(template[0][0] * 100 + template[0][1], (int)template[1][0]);
            }
            
            // Run 10 steps of polymer pair insertion
            for (int step = 0; step < 10; ++step)
            {
                int polymerLength = polymer.Count;
                for (int i = polymerLength - 2; i >= 0; --i)
                {
                    int p0 = polymer[i];
                    int p1 = polymer[i + 1];
                    int key = p0 * 100 + p1;
                    if (polymerTemplates.TryGetValue(key, out int value))
                        polymer.Insert(i + 1, value);
                }
            }

            IEnumerable<IGrouping<int, int>> groups = polymer.GroupBy(x => x);
            int min = groups.Min(group => group.Count());
            int max = groups.Max(group => group.Count());

            return max - min;
        }

    }
}
