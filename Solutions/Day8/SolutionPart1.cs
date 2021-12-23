using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day8
{
    internal class SolutionPart1 : IntSolution
    {

        public int Solve()
        {
            int occurences = 0;
            foreach (string line in File.ReadAllLines("./Content/Day8/1.txt"))
            {
                string[] signalsAndOutputs = line.Split(" | ");
                string[] outputs = signalsAndOutputs[1].Split(' ');

                occurences += outputs.Count(x => x.Length == 2 || x.Length == 3 || x.Length == 7 || x.Length == 2);
            }
            
            return occurences;
        }

    }
}
