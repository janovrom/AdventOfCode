using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day3
{
    internal class SolutionPart2 : IntSolution
    {

        public int Solve()
        {
            IEnumerable<string> lines = File.ReadLines("./Content/Day3/1.txt");
            int length = lines.First().Length;
            string[] lineArray = lines.ToArray();
            
            int oxygen = GetOxygen(lineArray, length);
            int co2 = GetCO2(lineArray, length);

            return oxygen * co2;
        }

        private int GetOxygen(string[] lineArray, int length)
        {
            BitComparer bitComparer = new();
            int start = 0;
            int end = lineArray.Length - 1;
            for (int i = 0; i < length; ++i)
            {
                Array.Sort(lineArray, start, end - start + 1, bitComparer);

                // Count zeros
                int count = 0;
                for (; count < end - start; ++count)
                {
                    if (lineArray[count + start][i] == '1')
                        break;
                }

                // more zeroes or ones?
                if (count > end - start - count + 1)
                {
                    // Keep zeros
                    end = start + count - 1;
                }
                else
                {
                    start += count;
                }

                if (start == end)
                {
                    // Last number
                    return Convert.ToInt32(lineArray[start], 2);
                }

                bitComparer.Bit += 1;
            }

            return -1;
        }

        private int GetCO2(string[] lineArray, int length)
        {
            BitComparer bitComparer = new();
            int start = 0;
            int end = lineArray.Length - 1;
            for (int i = 0; i < length; ++i)
            {
                Array.Sort(lineArray, start, end - start + 1, bitComparer);

                // Count zeros
                int count = 0;
                for (; count < end - start; ++count)
                {
                    if (lineArray[count + start][i] == '1')
                        break;
                }

                // more zeroes or ones?
                if (count > end - start - count + 1)
                {
                    // More zeroes, keep ones
                    start += count;
                }
                else
                {
                    // Keep zeros
                    end = start + count - 1;
                }

                if (start == end)
                {
                    // Last number
                    return Convert.ToInt32(lineArray[start], 2);
                }

                bitComparer.Bit += 1;
            }

            return -1;
        }

        private class BitComparer : IComparer<string>
        {

            public int Bit = 0;

            public int Compare(string x, string y)
            {
                return x[Bit].CompareTo(y[Bit]);
            }
        }

    }
}
