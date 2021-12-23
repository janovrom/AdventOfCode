using System.Collections.Generic;
using System.IO;

namespace AdventOfCode.Solutions.Day10
{
    internal class SolutionPart1 : IntSolution
    {

        public int Solve()
        {
            IEnumerable<string> lines = File.ReadAllLines("./Content/Day10/1.txt");

            int score = 0;
            foreach (string line in lines)
            {
                if (IsCorrupted(line, out Corruption corruption))
                {
                    score += corruption.Score;
                }
            }

            return score;
        }

        private static bool IsCorrupted(string line, out Corruption corruption)
        {
            Stack<char> tokens = new Stack<char>();

            for (int i = 0; i < line.Length; ++i)
            {
                char c = line[i];
                if (IsClosing(c))
                {
                    if (tokens.TryPop(out char top))
                    {
                        if (!Match(top, c))
                        {
                            // Do not match, this is corruption
                            corruption = new Corruption(c, i, Score(c));
                            return true;
                        }
                    }
                    else
                    {
                        // We have to pop, but can't => incomplete, but not corrupted
                        corruption = Corruption.Incomplete;
                        return false;
                    }
                }
                else
                {
                    tokens.Push(c);
                }
            }

            corruption = Corruption.NoCorruption;
            return false;

            static bool IsClosing(char c) => c switch
            {
                ')' => true,
                ']' => true,
                '}' => true,
                '>' => true,
                _ => false
            };

            static bool Match(char open, char close) => (open, close) switch
            {
                ('(',')') => true,
                ('[',']') => true,
                ('{','}') => true,
                ('<','>') => true,
                _ => false
            };

            static int Score(char c) => c switch
            {
                ')' => 3,
                ']' => 57,
                '}' => 1197,
                '>' => 25137,
                _ => 0
            };
        }

        private readonly struct Corruption
        {

            public static readonly Corruption NoCorruption = new Corruption();
            public static readonly Corruption Incomplete = new Corruption();

            public readonly char CorruptedChar;
            public readonly int Position;
            public readonly int Score;

            public Corruption(char corruptedChar, int position, int score)
            {
                CorruptedChar = corruptedChar;
                Position = position;
                Score = score;
            }
        }

    }
}
