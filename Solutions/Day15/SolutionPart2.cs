using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day15
{
    internal class SolutionPart2 : IntSolution
    {

        int count = 0;

        public int Solve()
        {
            string[] lines = File.ReadAllLines("./Content/Day15/1.txt").ToArray();

            int l = lines.Length;
            int[,] weightedMap = new int[l * 5, l * 5];
            int[,] solution = new int[l * 5, l * 5];
            for (int i = 0; i < lines.Length; ++i)
            {
                for (int j = 0; j < lines.Length; ++j)
                {
                    int v = lines[i][j] - '0';

                    for (int x = 0; x < 5; ++x)
                    {
                        for (int y = 0; y < 5; ++y)
                        {
                            weightedMap[i + l * x, j + l * y] = (v + x + y) > 9 ? (v + x + y) % 9 : (v + x + y);
                        }
                    }
                }
            }

            for (int i = 0; i < solution.GetLength(0); ++i)
                for (int j = 0; j < solution.GetLength(1); ++j)
                    solution[i, j] = int.MaxValue;


            Discover(weightedMap, solution, 0, 0, 0);

            return solution[solution.GetLength(0) - 1, solution.GetLength(1) - 1] - weightedMap[0, 0];
        }

        private void Discover(int[,] weightedMap, int[,] solution, int startx, int starty, int pathWeight)
        {
            Queue<(int, int, int)> openedPaths = new();
            openedPaths.Enqueue((startx, starty, pathWeight));

            while (openedPaths.Count > 0)
            {
                (int x, int y, int distance) = openedPaths.Dequeue();
                distance += weightedMap[x, y];
                if (solution[x, y] >= distance)
                {
                    ++count;
                    solution[x, y] = distance;

                    // Heuristic - first left, then down
                    // Down
                    if (x < solution.GetLength(0) - 1 && distance + weightedMap[x + 1, y] < solution[x + 1, y])
                    {
                        solution[x + 1, y] = distance + weightedMap[x + 1, y];
                        openedPaths.Enqueue((x + 1, y, distance));
                    }

                    // Left
                    if (y > 0 && distance + weightedMap[x, y - 1] < solution[x, y - 1])
                    {
                        solution[x, y - 1] = distance + weightedMap[x, y - 1];
                        openedPaths.Enqueue((x, y - 1, distance));
                    }

                    // Up
                    if (x > 0 && distance + weightedMap[x - 1, y] < solution[x - 1, y])
                    {
                        solution[x - 1, y] = distance + weightedMap[x - 1, y];
                        openedPaths.Enqueue((x - 1, y, distance));
                    }

                    // Right
                    if (y < solution.GetLength(1) - 1 && distance + weightedMap[x, y + 1] < solution[x, y + 1])
                    {
                        solution[x, y + 1] = distance + weightedMap[x, y + 1];
                        openedPaths.Enqueue((x, y + 1, distance));
                    }
                }
            }
        }

    }
}
