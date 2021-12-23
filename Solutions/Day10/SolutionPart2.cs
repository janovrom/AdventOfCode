using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode.Solutions.Day10
{
    internal class SolutionPart2 : LongSolution
    {

        public long Solve()
        {
            IEnumerable<string> lines = File.ReadAllLines("./Content/Day10/1.txt");

            List<long> scores = new List<long>();
            foreach (string line in lines)
            {
                if (IsIncomplete(line, out string remain))
                {
                    long score = 0;
                    for (int i = 0; i < remain.Length; ++i)
                    {
                        char c = remain[i];
                        score *= 5;
                        score += Score(c);
                    }
                    scores.Add(score);
                }
            }

            scores.Sort();
            // Always odd
            return scores[scores.Count / 2];

            static int Score(char c) => c switch
            {
                '(' => 1,
                '[' => 2,
                '{' => 3,
                '<' => 4,
                _ => throw new NotSupportedException()
            };
        }

        private static bool IsIncomplete(string line, out string remain)
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
                            remain = null;
                            return false;
                        }
                    }
                    else
                    {
                        // We have to pop, but can't => incomplete, but not corrupted
                        remain = null;
                        return false;
                    }
                }
                else
                {
                    tokens.Push(c);
                }
            }

            if (tokens.Count == 0)
            {
                remain = null;
                return false;
            }

            remain = new string(tokens.ToArray());
            return true;

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
                ('(', ')') => true,
                ('[', ']') => true,
                ('{', '}') => true,
                ('<', '>') => true,
                _ => false
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
