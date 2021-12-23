using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day14
{
    internal class SolutionPart2 : LongSolution
    {

        private long[] _counts = new long['Z' - 'A' + 1];

        public const int BuildStep = 5;

        public long Solve()
        {
            IEnumerable<string> lines = File.ReadAllLines("./Content/Day14/1.txt");

            // Too tired to think, brute force it
            List<int> polymer = lines.First().ToList().Select(c => (int)c).ToList();
            Dictionary<int, PolymerTemplate> templates = new();

            foreach (string line in lines.Skip(2))
            {
                var template = new PolymerTemplate(line);
                templates.Add(template.Key, template);
            }

            foreach (PolymerTemplate polymerTemplate in templates.Values)
            {
                polymerTemplate.BuildFullTemplate(templates);
            }

            int polymerLength = polymer.Count;
            for (int i = polymerLength - 2; i >= 0; --i)
            {
                int p0 = polymer[i];
                int p1 = polymer[i + 1];
                int key = p0 * 100 + p1;
                templates[key].FillCounts(_counts, 8, templates);
            }

            foreach (int p in polymer)
                _counts[p - 'A'] += 1;

            long min = _counts.Where(x => x > 0).Min();
            long max = _counts.Max();

            return max - min;
        }

        private class PolymerTemplate
        {

            public readonly int FirstPart;
            public readonly int SecondPart;
            public readonly int Key;
            public readonly int Output;
            public List<int> OutputPolymer;

            Dictionary<int, long[]> DepthToCounts = new();


            internal PolymerTemplate(string template)
            {
                string[] splitted = template.Split(" -> ");
                FirstPart = splitted[0][0];
                SecondPart = splitted[0][1];
                Key = FirstPart * 100 + SecondPart;
                Output = splitted[1][0];
            }

            internal void BuildFullTemplate(Dictionary<int, PolymerTemplate> hashedTemplates)
            {
                OutputPolymer = new List<int>() { FirstPart, SecondPart };
                long[] counts = new long['Z' - 'A' + 1];
                for (int step = 0; step < BuildStep; ++step)
                {
                    int polymerLength = OutputPolymer.Count;
                    for (int i = polymerLength - 2; i >= 0; --i)
                    {
                        int p0 = OutputPolymer[i];
                        int p1 = OutputPolymer[i + 1];
                        int key = p0 * 100 + p1;
                        if (hashedTemplates.TryGetValue(key, out PolymerTemplate template))
                        {
                            counts[template.Output - 'A'] += 1;
                            OutputPolymer.Insert(i + 1, template.Output);
                        }
                    }
                }
                DepthToCounts.Add(BuildStep, counts);
            }

            internal void FillCounts(long[] globalCounts, int depth, Dictionary<int, PolymerTemplate> templates)
            {
                if (depth == 0)
                    return;

                if (!DepthToCounts.TryGetValue(BuildStep * depth, out long[] counts))
                {
                    counts = (long[])DepthToCounts[BuildStep].Clone();
                    for (int i = 0; i < OutputPolymer.Count - 1; ++i)
                    {
                        int p0 = OutputPolymer[i];
                        int p1 = OutputPolymer[i + 1];
                        int key = p0 * 100 + p1;
                        if (templates.TryGetValue(key, out PolymerTemplate template))
                            template.FillCounts(counts, depth - 1, templates);
                    }
                    DepthToCounts[BuildStep * depth] = counts;
                }

                for (int i = 0; i < counts.Length; ++i)
                {
                    globalCounts[i] += counts[i];
                }

                
            }

            public override int GetHashCode()
            {
                return Key.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is not PolymerTemplate other) return false;
                return Key == other.Key;
            }


        }

    }
}
