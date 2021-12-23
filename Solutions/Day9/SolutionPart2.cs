using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day9
{
    internal class SolutionPart2 : IntSolution
    {

        public int Solve()
        {
            IEnumerable<string> lines = File.ReadAllLines("./Content/Day9/1.txt");
            int height = lines.Count();
            int width = lines.First().Length;

            int[,] heights = new int[height, width];
            // We get by with only one shared array since the basins are
            // separated by 9. Also the task explicitly says, that each
            // height belongs to exactly one basin, otherwise we would
            // need to stop in saddle points or local maxima.
            bool[,] basins = new bool[height, width];

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

            return GetBasinSize(heights, basins);
        }

        private int GetBasinSize(int[,] heights, bool[,] basins)
        {
            List<int> basinSizes = new();
            for (int h = 0; h < heights.GetLength(0); ++h)
            {
                for (int w = 0; w < heights.GetLength(1); ++w)
                {
                    if (IsMinimum(heights, h, w))
                    {
                        basinSizes.Add(GetBasinSize(heights, basins, h, w));
                    }
                }
            }

            basinSizes.Sort();
            return basinSizes.TakeLast(3).Aggregate((x,y) => x * y);
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

        private static int GetBasinSize(int[,] heights, bool[,] basins, int h, int w)
        {
            if (heights[h, w] == 9)
                return 0;

            int size = 1;
            basins[h, w] = true;
            // We can possibly skip all the height checks based on an observation 
            // of the data - each basin is surrounded by 9
            // Look up
            if (h > 0)
            {
                if (!basins[h - 1, w] && heights[h - 1, w] > heights[h, w])
                    size += GetBasinSize(heights, basins, h - 1, w);
            }

            // Look down
            if (h < heights.GetLength(0) - 1)
            {
                if (!basins[h + 1, w] && heights[h + 1, w] > heights[h, w])
                    size += GetBasinSize(heights, basins, h + 1, w);
            }

            // Look left
            if (w > 0)
            {
                if (!basins[h, w - 1] && heights[h, w - 1] > heights[h, w])
                    size += GetBasinSize(heights, basins, h, w - 1);
            }

            // Look right
            if (w < heights.GetLength(1) - 1)
            {
                if (!basins[h, w + 1] && heights[h, w + 1] > heights[h, w])
                    size += GetBasinSize(heights, basins, h, w + 1);
            }

            return size;
        }

    }
}
