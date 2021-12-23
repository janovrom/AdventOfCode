using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day1
{
    internal class SolutionPart1 : IntSolution
    {

        private int[] _depths;

        public SolutionPart1()
        {
            LoadFile();
        }

        public int Solve()
        {
            int increases = 0;
            for (int i = 1; i < _depths.Length; ++i)
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
