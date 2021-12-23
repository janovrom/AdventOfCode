using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day15
{
    internal class SolutionPart1 : IntSolution
    {

        int count = 0;

        public int Solve()
        {
            string[] lines = File.ReadAllLines("./Content/Day15/1.txt").ToArray();

            int[,] weightedMap = new int[lines.Length, lines.Length];
            int[,] solution = new int[lines.Length, lines.Length];
            for (int i = 0; i < lines.Length; ++i)
            {
                for (int j = 0; j < lines.Length; ++j)
                {
                    weightedMap[i, j] = lines[i][j] - '0';
                    solution[i,j] = int.MaxValue;
                }
            }

            Discover(weightedMap, solution, 0, 0, 0);

            return solution[lines.Length - 1, lines.Length - 1] - weightedMap[0,0];
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
                    // Writing it down first is faster
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
