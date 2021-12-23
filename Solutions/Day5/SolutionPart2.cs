using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day5
{
    internal class SolutionPart2 : IntSolution
    {

        public int Solve()
        {
            int lineOverlaps = 0;
            var pointDict = new Dictionary<(int, int), int>();
            foreach (string line in File.ReadLines("./Content/Day5/1.txt"))
            {
                string[] endpoints = line.Split(" -> ");
                string[] start = endpoints[0].Split(",");
                string[] end = endpoints[1].Split(",");

                int x = int.Parse(start[0]);
                int y = int.Parse(start[1]);
                int x1 = int.Parse(end[0]);
                int y1 = int.Parse(end[1]);

                int ix = Math.Sign(x1 - x);
                int iy = Math.Sign(y1 - y);

                do
                {
                    if (pointDict.TryGetValue((x, y), out int value))
                    {
                        if (value == 1)
                            lineOverlaps += 1;

                        pointDict[(x, y)] = value + 1;
                    }
                    else
                    {
                        pointDict.Add((x, y), 1);
                    }

                    x += ix;
                    y += iy;
                } while (x != x1 + ix || y != y1 + iy);

            }

            return lineOverlaps;
        }

    }
}
