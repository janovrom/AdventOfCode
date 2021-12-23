using System.IO;

namespace AdventOfCode.Solutions.Day2
{
    internal class SolutionPart2 : IntSolution
    {

        public int Solve()
        {
            int depth = 0;
            int forward = 0;
            int aim = 0;
            foreach (string line in File.ReadLines("./Content/Day2/1.txt"))
            {
                string[] split = line.Split(" ");
                int v = int.Parse(split[1]);
                switch (split[0])
                {
                    case "forward":
                        forward += v;
                        depth += aim * v;
                        break;
                    case "down":
                        aim += v;
                        break;
                    case "up":
                        aim -= v;
                        break;
                }
            }

            return depth * forward;
        }

    }
}
