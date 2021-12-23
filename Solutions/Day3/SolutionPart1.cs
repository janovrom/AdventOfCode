using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day3
{
    internal class SolutionPart1 : IntSolution
    {

        public int Solve()
        {
            IEnumerable<string> lines = File.ReadLines("./Content/Day3/1.txt");
            int length = lines.First().Length;
            int[] values = lines.Select(x => Convert.ToInt32(x, 2)).ToArray();
            int[] zeroCount = new int[length];
            int[] oneCount = new int[length];
            for (int i = 0; i < values.Length; ++i)
            {
                for (int j = 0; j < length; ++j)
                {
                    int index = 1 << j;
                    if ((values[i] & index) == 0)
                    {
                        zeroCount[j] += 1;
                    }
                    else
                    {
                        oneCount[j] += 1;
                    }
                }
            }

            int gammaRate = 0;
            int epsilonRate = 0;
            for (int i = length - 1; i >= 0; --i)
            {
                if (oneCount[i] > zeroCount[i])
                    gammaRate += 1;
                
                if (oneCount[i] < zeroCount[i])
                    epsilonRate += 1;

                gammaRate <<= 1;
                epsilonRate <<= 1;
            }
            gammaRate /= 2; // Remove the last shift
            epsilonRate /= 2; // Remove the last shift

            return gammaRate * epsilonRate;
        }

    }
}
