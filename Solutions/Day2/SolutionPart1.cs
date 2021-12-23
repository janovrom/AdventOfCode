using System.IO;

namespace AdventOfCode.Solutions.Day2
{
    internal class SolutionPart1 : IntSolution
    {

        public int Solve()
        {
            int depth = 0;
            int forward = 0;
            foreach (string line in File.ReadLines("./Content/Day2/1.txt"))
            {
                string[] split = line.Split(" ");
                switch (split[0])
                {
                    case "forward":
                        forward += int.Parse(split[1]);
                        break;
                    case "down":
                        depth += int.Parse(split[1]);
                        break;
                    case "up":
                        depth -= int.Parse(split[1]);
                        break;
                }
            }

            return depth * forward;
        }

    }
}
