using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day8
{
    internal class SolutionPart2 : IntSolution
    {

        public int Solve()
        {
            int outputSum = 0;
            foreach (string line in File.ReadAllLines("./Content/Day8/1.txt"))
            {
                string[] signalsAndOutputs = line.Split(" | ");
                string[] signals = signalsAndOutputs[0].Split(' ');
                string[] outputs = signalsAndOutputs[1].Split(' ');
                string[] decoders = DecodeSignals(signals);

                for (int i = 0; i < 4; ++i)
                {
                    for (int j = 0; j < 10; ++j)
                    {
                        if (MatchesCharacters(decoders[j], outputs[i]))
                        {
                            outputSum += (int)Math.Pow(10, 3 - i) * j;
                            break;
                        }
                    }
                }
            }

            return outputSum;
        }

        private string[] DecodeSignals(string[] signals)
        {

            Array.Sort(signals, (x, y) => x.Length.CompareTo(y.Length));
            string one = signals[0];
            string seven = signals[1];
            string four = signals[2];
            string eight = signals[9];
            string three = null, nine = null;

            // Indices 3,4,5 represent numbers 2,3,5
            for (int i = 3; i < 6; ++i)
            {
                if (SignalContains(signals[i], one))
                {
                    // Then it's three
                    three = signals[i];
                    // Move 3 as first
                    signals[i] = signals[3];
                    break;
                }
            }

            // Indices 6,7,8 are numbers 0, 6, 9
            for (int i = 6; i < 9; ++i)
            {
                if (SignalContains(signals[i], four))
                {
                    // Then it's nine
                    nine = signals[i];
                    // Move 9 as first
                    signals[i] = signals[6];
                    break;
                }
            }

            // 7,8 are numbers 0 or 6
            // If it has one, then it's zero
            (string zero, string six) = SignalContains(signals[7], one) ? (signals[7], signals[8]) : (signals[8], signals[7]);

            // Only 2 and 5 remains
            // If it is contained in 6, it's 5
            (string five, string two) = SignalContains(six, signals[4]) ? (signals[4], signals[5]) : (signals[5], signals[4]);

            return new string[]
            {
                zero,
                one,
                two,
                three,
                four,
                five,
                six,
                seven,
                eight,
                nine
            };
        }

        private static bool MatchesCharacters(string s0, string s1)
        {
            if (s0.Length != s1.Length)
                return false;

            return SignalContains(s0, s1);
        }

        private static bool SignalContains(string s0, string s1)
        {
            foreach (char c in s1)
            {
                if (!s0.Contains(c))
                    return false;
            }

            return true;
        }

    }
}
