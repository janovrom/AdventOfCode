using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day7
{
    internal class SolutionPart1 : IntSolution
    {

        public int Solve()
        {
            IEnumerable<int> positions = File.ReadAllLines("./Content/Day7/1.txt").First().Split(",").Select(x => int.Parse(x));
            int maxPosition = positions.Max();
            int[] crabCounts = new int[maxPosition + 1];

            foreach (int position in positions)
            {
                crabCounts[position]++;
            }

            int minMovementCost = int.MaxValue;
            int minPosition = int.MaxValue;
            for (int position = 0; position <= maxPosition; ++position)
            {
                int movementCost = 0;
                for (int i = 0; i <= maxPosition; ++i)
                {
                    movementCost += Math.Abs(i - position) * crabCounts[i];
                }
                
                if (minMovementCost > movementCost)
                {
                    minMovementCost = movementCost;
                    minPosition = position;
                }
            }

            return minMovementCost;
        }

    }
}
