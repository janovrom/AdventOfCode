using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day11
{
    internal class SolutionPart2 : IntSolution
    {

        public int Solve()
        {
            string[] lines = File.ReadAllLines("./Content/Day11/1.txt").ToArray();

            int[,] energy = new int[12, 12];// Add padding to ignore boundary conditions

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    energy[i + 1, j + 1] = lines[i][j] - 48;
                }
            }

            // 10000 should be enough
            for (int step = 1; step < 10000; ++step)
            {
                bool[,] flashed = new bool[12, 12];
                CleanBoundary(energy);
                UpdateEnergyLevel(energy);
                PropagateFlash(energy, flashed);
                ResetFlashed(energy);
                int flashCount = CountFlashed(flashed);
                if (flashCount == 100)
                    return step;
            }


            return -1;
        }

        private static void UpdateEnergyLevel(int[,] energy)
        {
            for (int i = 1; i < 11; ++i)
            {
                for (int j = 1; j < 11; ++j)
                {
                    energy[i, j] += 1;
                }
            }
        }

        private static void PropagateFlash(int[,] energy, bool[,] flashed)
        {
            for (int i = 1; i < 11; ++i)
            {
                for (int j = 1; j < 11; ++j)
                {
                    if (energy[i, j] > 9 && !flashed[i, j])
                    {
                        Flash(energy, i, j, flashed);
                    }
                }
            }
        }

        private static void Flash(int[,] energy, int x, int y, bool[,] flashed)
        {
            flashed[x, y] = true;
            for (int i = x - 1; i <= x + 1; ++i)
            {
                for (int j = y - 1; j <= y + 1; ++j)
                {
                    if (i == x && j == y)
                        continue;

                    energy[i, j] += 1;
                    if (energy[i, j] > 9 && !flashed[i, j])
                    {
                        Flash(energy, i, j, flashed);
                    }
                }
            }
        }

        private static void CleanBoundary(int[,] energy)
        {
            for (int i = 0; i < 12; ++i)
                energy[0, i] = 0;

            for (int i = 0; i < 12; ++i)
                energy[11, i] = 0;

            for (int i = 0; i < 12; ++i)
            {
                energy[i, 0] = 0;
                energy[i, 11] = 0;
            }
        }

        private static int CountFlashed(bool[,] flashed)
        {
            int count = 0;
            for (int i = 1; i < 11; ++i)
            {
                for (int j = 1; j < 11; ++j)
                {
                    if (flashed[i, j])
                    {
                        ++count;
                    }
                }
            }

            return count;
        }

        private static void ResetFlashed(int[,] energy)
        {
            for (int i = 1; i < 11; ++i)
            {
                for (int j = 1; j < 11; ++j)
                {
                    if (energy[i, j] > 9)
                        energy[i, j] = 0;
                }
            }
        }

    }
}
