using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day4
{
    internal class SolutionPart1 : IntSolution
    {

        public int Solve()
        {
            string[] lines = File.ReadLines("./Content/Day4/1.txt").ToArray();

            string[] randomNumbers = lines[0].Split(",");

            List<Board> boards = new List<Board>((lines.Length - 2) / 6);
            for (int i = 2; i < lines.Length; i+=6)
            {
                boards.Add(new Board(lines, i));
            }

            foreach (string randomNumber in randomNumbers)
            {
                foreach (Board board in boards)
                {
                    if (board.InputNumber(randomNumber))
                    {
                        return board.SumUnmarked() * int.Parse(randomNumber);
                    }
                }
            }

            return 0;
        }

        private class Board
        {

            private Dictionary<string, (int, int)> _valueToIndex = new Dictionary<string, (int,int)>();
            private int[] _countRows = new int[5];
            private int[] _countCols = new int[5];

            internal Board(string[] lines, int offset)
            {
                for (int i = 0; i < 5; ++i)
                {
                    string[] splitted = lines[offset + i].Trim().Replace("  ", ",").Replace(" ", ",").Split(",");
                    for (int j = 0; j < 5; ++j)
                        _valueToIndex.Add(splitted[j], (i, j));
                }
            }

            internal bool InputNumber(string number)
            {
                if (_valueToIndex.TryGetValue(number, out (int, int) pair))
                {
                    _valueToIndex.Remove(number);
                    _countRows[pair.Item1] += 1;
                    _countCols[pair.Item2] += 1;

                    if (_countRows[pair.Item1] == 5)
                        return true;

                    if (_countCols[pair.Item2] == 5)
                        return true;
                }

                return false;
            }

            internal int SumUnmarked()
            {
                int sum = 0;
                foreach (string v in _valueToIndex.Keys)
                {
                    sum += int.Parse(v);
                }

                return sum;
            }

        }

    }
}
