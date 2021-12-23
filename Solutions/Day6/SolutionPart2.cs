using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day6
{
    internal class SolutionPart2 : LongSolution
    {

        public long Solve()
        {
            string[] spawnRates = File.ReadAllLines("./Content/Day6/1.txt").First().Split(",");
            long[] spawnCount = new long[9];
            foreach (string spawnRate in spawnRates)
            {
                long rate = long.Parse(spawnRate);
                spawnCount[rate] += 1;
            }

            for (int day = 0; day < 256; ++day)
            {
                long first = spawnCount[0];
                for (int i = 1; i < 9; ++i)
                {
                    spawnCount[i - 1] = spawnCount[i];
                }

                // Spawn new fish
                spawnCount[8] = first;
                // Reset the cycle for breeding fish
                spawnCount[6] += first;
            }

            return spawnCount.Sum();
        }

    }
}
