using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day21
{
    internal class SolutionPart1 : IntSolution
    {

        public int Solve()
        {
            IEnumerable<string> lines = File.ReadAllLines("./Content/Day21/1.txt");
            List<Player> players = new List<Player>();
            foreach (string line in lines)
            {
                int position = int.Parse(line.Split(' ').Last());
                players.Add(new Player(position));
            }

            Dice d = new DeterministicDice();
            while (true)
            {
                foreach (Player p in players)
                {
                    p.PlayRound(d);
                    if (p.Score >= 1000)
                    {
                        int score = players.Where(player => !ReferenceEquals(p, player)).Sum(p => p.Score);
                        return score * d.RollCount;
                    }
                }
            }
        }

    }

}
