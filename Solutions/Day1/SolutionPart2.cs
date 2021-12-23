using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day1
{
    internal class SolutionPart2 : IntSolution
    {

        private int[] _depths;

        public SolutionPart2()
        {
            LoadFile();
        }

        public int Solve()
        {
            int increases = 0;
            // Compute sliding average first
            for (int i = 0; i < _depths.Length - 2; ++i)
            {
                _depths[i] += _depths[i + 1] + _depths[i + 2];
            }

            for (int i = 1; i < _depths.Length - 2; ++i)
            {
                increases += _depths[i - 1] < _depths[i] ? 1 : 0;
            }

            return increases;
        }

        private void LoadFile()
        {
            _depths = File.ReadLines("./Content/Day1/1.txt").Select(line => int.Parse(line)).ToArray();
        }

    }
}
