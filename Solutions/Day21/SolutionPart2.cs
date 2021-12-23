using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day21
{
    internal class SolutionPart2 : LongSolution
    {

        public long Solve()
        {
            string[] lines = File.ReadAllLines("./Content/Day21/1.txt").ToArray();
            Player player0 = new Player(int.Parse(lines[0].Split(' ').Last()));
            Player player1 = new Player(int.Parse(lines[1].Split(' ').Last()));

            DimensionalDice dice0 = new DimensionalDice(player0.StartingPosition);
            DimensionalDice dice1 = new DimensionalDice(player1.StartingPosition);

            long universesForWin = 0;
            int run = 0;
            List<long> rounds0 = new();
            List<long> rounds1 = new();
            while (dice0.CanRun() || dice1.CanRun())
            {
                dice0.RunRound();
                (long c0, long w0) = dice0.GetWinners();
                (long c1, long w1) = dice1.GetWinners();
                dice1.RunRound();
                universesForWin += w0 * (c1 + w1);
                rounds0.Add(w0);
                rounds1.Add(w1);
                ++run;

                dice0.ResetWinners();
                dice1.ResetWinners();
            }

            return universesForWin;
        }

        private class DimensionalDice
        {
            private long[] _throwProbabilities = new long[10];
            // All possible positions we can be in (consider the position to be the score)
            // Since we can overshoot (20, throw is 9 and position 1 => 30) we have larger array.
            private const int _MaxScore = 31;
            private const int _MaxPosition = 10;
            private const int _PositionStart = 0;
            private long[,] _scoreState = new long[_MaxScore, _MaxPosition];

            internal DimensionalDice(int startPosition)
            {
                CreateThrowOutcomes();
                Initialize(startPosition);
            }

            private void Initialize(int startPosition)
            {
                // Example: startPosition is 4, but indexing starts
                // from 0 and from 1 in array and the game respectively
                startPosition -= 1;

                // With score 0 we have 1 state in position startPosition
                _scoreState[0, startPosition] = 1;
            }

            internal void RunRound()
            {
                // From states 0 till 21, we can actually get to pass condition
                // All beyond is already possibly won.

                long[,] scoreState = new long[_MaxScore, _MaxPosition];

                for (int score = 0; score < 21; ++score)
                {
                    // For each non-winning score, move positions
                    for (int p = _PositionStart; p < _MaxPosition; ++p)
                    {
                        // If for given score, we have no state in this position, skip it
                        long currentStatesInPosition = _scoreState[score, p];
                        if (currentStatesInPosition == 0)
                            continue;

                        // Model transitions
                        for (int t = 3; t < 10; ++t)
                        {
                            int newPosition = (p + t) % 10;
                            long newScore = score + newPosition + 1; // NewPosition is an index
                            //int newPosition = (((p + t) - 1) % 10) + 1;
                            //long newScore = score + newPosition;
                            scoreState[newScore, newPosition] += currentStatesInPosition * _throwProbabilities[t];
                        }

                    }
                }

                _scoreState = scoreState;
            }

            internal void Print()
            {
                Console.SetCursorPosition(0, 0);
                Console.Write("\t\t");
                for (int p = _PositionStart; p < _MaxPosition; ++p)
                {
                    Console.Write($"{p}\t");
                }
                Console.WriteLine();
                for (int score = 0; score < _MaxScore; ++score)
                {
                    Console.Write($"Score {score}:\t");
                    // For each non-winning score, move positions
                    for (int p = _PositionStart; p < _MaxPosition; ++p)
                    {
                        // If for given score, we have no state in this position, skip it
                        long currentStatesInPosition = _scoreState[score, p];
                        Console.Write(currentStatesInPosition);
                        Console.Write("\t");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }

            internal void Flatten()
            {
                for (int score = 0; score < _MaxScore; ++score)
                {
                    // For each non-winning score, move positions
                    for (int p = _PositionStart; p < _MaxPosition; ++p)
                    {
                        _scoreState[score, p] = Math.Sign(_scoreState[score, p]);
                    }
                }
            }

            internal (long, long) GetWinners()
            {
                long ends = 0;
                long continues = 0;
                for (int i = 0; i < 21; ++i)
                {
                    for (int j = _PositionStart; j < _MaxPosition; ++j)
                    {
                        continues += _scoreState[i, j];
                    }
                }

                // Count all occurrences of 21 and more, those are the possible winners in this step.
                for (int i = 21; i < _MaxScore; ++i)
                {
                    for (int j = _PositionStart; j < _MaxPosition; ++j)
                    {
                        ends += _scoreState[i, j];
                        _scoreState[i, j] = 0;
                    }
                }

                return (continues, ends);
            }

            internal void ResetWinners()
            {
                // Count all occurrences of 21 and more, those are the possible winners in this step.
                for (int i = 21; i < _MaxScore; ++i)
                {
                    for (int j = _PositionStart; j < _MaxPosition; ++j)
                    {
                        // These states will no longer contribute (they are sinks)
                        _scoreState[i, j] = 0;
                    }
                }
            }

            internal bool CanRun()
            {
                for (int i = 0; i < 21; ++i)
                {
                    for (int j = _PositionStart; j < _MaxPosition; ++j)
                    {
                        if (_scoreState[i, j] > 0)
                        {
                            // At least one state remains
                            return true;
                        }
                    }
                }

                return false;
            }

            private void CreateThrowOutcomes()
            {
                for (int i = 1; i <= 3; ++i)
                {
                    for (int j = 1; j <= 3; ++j)
                    {
                        for (int k = 1; k <= 3; ++k)
                        {
                            _throwProbabilities[i + j + k] += 1;
                        }
                    }
                }
            }

        }

    }

}
