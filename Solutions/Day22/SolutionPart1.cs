using System.Collections.Generic;
using System.IO;

namespace AdventOfCode.Solutions.Day22
{
    internal class SolutionPart1 : IntSolution
    {

        public int Solve()
        {
            IEnumerable<string> lines = File.ReadAllLines("./Content/Day22/1.txt");

            // Just brute force it to get part 2 :)
            List<Command> commands = new List<Command>();
            foreach (string line in lines)
            {
                commands.Add(new Command(line));
            }

            int onCount = 0;

            for (int x = -50; x <= 50; ++x)
            {
                for (int y = -50; y <= 50; ++y)
                {
                    for (int z = -50; z <= 50; ++z)
                    {
                        bool isTurnedOn = false;
                        foreach (Command command in commands)
                        {
                            isTurnedOn = command.IsTurnedOn(isTurnedOn, x, y, z);
                        }

                        onCount += isTurnedOn ? 1 : 0;
                    }
                }
            }

            return onCount;
        }

        private class Command
        {
            private bool _turnOn;
            private int _minX;
            private int _minY;
            private int _minZ;
            private int _maxX;
            private int _maxY;
            private int _maxZ;


            internal Command(string line)
            {
                string[] splitted = line.Split(' ');
                _turnOn = splitted[0] == "on";
                splitted = splitted[1].Split(',');
                string[] x = splitted[0].Substring(2).Split("..");
                string[] y = splitted[1].Substring(2).Split("..");
                string[] z = splitted[2].Substring(2).Split("..");

                _minX = int.Parse(x[0]);
                _maxX = int.Parse(x[1]);

                _minY = int.Parse(y[0]);
                _maxY = int.Parse(y[1]);

                _minZ = int.Parse(z[0]);
                _maxZ = int.Parse(z[1]);
            }

            public bool IsTurnedOn(bool state, int x, int y, int z)
            {
                // Is in region, turn on/off based on our field _turnOn
                if (_minX <= x && _minY <= y && _minZ <= z && x <= _maxX && y <= _maxY && z <= _maxZ)
                    return _turnOn;

                // It's not our grid block, pass the value
                return state;
            }

        }

    }

}
