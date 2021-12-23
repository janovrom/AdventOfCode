using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day9
{
    internal class SolutionPart1 : IntSolution
    {

        public int Solve()
        {
            IEnumerable<string> lines = File.ReadAllLines("./Content/Day9/1.txt");
            int height = lines.Count();
            int width = lines.First().Length;

            int[,] heights = new int[height, width];

            // Build the heightmap
            int h = 0;
            foreach (string line in lines)
            {
                for (int w = 0; w < width; ++w)
                {
                    heights[h, w] = line[w] - 48;
                }
                ++h;
            }

            return GetRisk(heights);
        }

        private int GetRisk(int[,] heights)
        {
            int risk = 0;
            for (int h = 0; h < heights.GetLength(0); ++h)
            {
                for (int w = 0; w < heights.GetLength(1); ++w)
                {
                    if (IsMinimum(heights, h, w))
                    {
                        risk += 1 + heights[h, w];
                    }
                }
            }

            return risk;
        }

        private static bool IsMinimum(int[,] heights, int h, int w)
        {
            int candidate = heights[h, w];
            bool isMin = true;
            // Look up
            if (h > 0)
                isMin = isMin && heights[h - 1, w] - candidate > 0;

            // Look down
            if (h < heights.GetLength(0) - 1)
                isMin = isMin && heights[h + 1, w] - candidate > 0;

            // Look left
            if (w > 0)
                isMin = isMin && heights[h, w - 1] - candidate > 0;

            // Look right
            if (w < heights.GetLength(1) - 1)
                isMin = isMin && heights[h, w + 1] - candidate > 0;

            return isMin;
        }

    }
}
