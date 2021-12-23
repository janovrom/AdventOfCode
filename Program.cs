using System;

namespace AdventOfCode
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(new Solutions.Day1.SolutionPart1().Solve());
            //Console.WriteLine(new Solutions.Day1.SolutionPart2().Solve());
            //Console.WriteLine(new Solutions.Day2.SolutionPart1().Solve());
            //Console.WriteLine(new Solutions.Day2.SolutionPart2().Solve());
            //Console.WriteLine(new Solutions.Day3.SolutionPart1().Solve());
            //Console.WriteLine(new Solutions.Day3.SolutionPart2().Solve());
            //Console.WriteLine(new Solutions.Day4.SolutionPart1().Solve());
            //Console.WriteLine(new Solutions.Day4.SolutionPart2().Solve());
            //Console.WriteLine(new Solutions.Day5.SolutionPart1().Solve());
            //Console.WriteLine(new Solutions.Day5.SolutionPart2().Solve());
            //Console.WriteLine(new Solutions.Day6.SolutionPart1().Solve());
            //Console.WriteLine(new Solutions.Day6.SolutionPart2().Solve());

            //Solve(new Solutions.Day7.SolutionPart1());
            //Solve(new Solutions.Day7.SolutionPart2());

            //Solve(new Solutions.Day8.SolutionPart1());
            //Solve(new Solutions.Day8.SolutionPart2());

            //Solve(new Solutions.Day9.SolutionPart1());
            //Solve(new Solutions.Day9.SolutionPart2());

            //Solve(new Solutions.Day10.SolutionPart1());
            //Solve(new Solutions.Day10.SolutionPart2());

            //Solve(new Solutions.Day11.SolutionPart1());
            //Solve(new Solutions.Day11.SolutionPart2());

            //Solve(new Solutions.Day12.SolutionPart1());
            //Solve(new Solutions.Day12.SolutionPart2());

            //Solve(new Solutions.Day13.SolutionPart1());
            //Solve(new Solutions.Day13.SolutionPart2());

            //Solve(new Solutions.Day14.SolutionPart1());
            //Solve(new Solutions.Day14.SolutionPart2());

            //Solve(new Solutions.Day15.SolutionPart1());
            //Solve(new Solutions.Day15.SolutionPart2());

            //Solve(new Solutions.Day16.SolutionPart1());
            //Solve(new Solutions.Day16.SolutionPart2());

            //Solve(new Solutions.Day17.SolutionPart1());
            //Solve(new Solutions.Day17.SolutionPart2());

            //Solve(new Solutions.Day18.SolutionPart1());
            //Solve(new Solutions.Day18.SolutionPart2());

            //Solve(new Solutions.Day19.SolutionPart1());
            //Solve(new Solutions.Day19.SolutionPart2());

            //Solve(new Solutions.Day20.SolutionPart1());
            //Solve(new Solutions.Day20.SolutionPart2());

            //Solve(new Solutions.Day21.SolutionPart1());
            //Solve(new Solutions.Day21.SolutionPart2());

            //Solve(new Solutions.Day22.SolutionPart1());
            // Solve(new Solutions.Day22.SolutionPart2());

            Solve(new Solutions.Day23.SolutionPart1());
            // Solve(new Solutions.Day23.SolutionPart2());
        }

        private static void Solve(ISolution<long> solution)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            long result = solution.Solve();
            long millis = sw.ElapsedMilliseconds;
            Console.WriteLine(result);
            Console.WriteLine($"Solution found in {millis} milliseconds.");
            sw.Stop();
        }

        private static void Solve(ISolution<int> solution)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            int result = solution.Solve();
            long millis = sw.ElapsedMilliseconds;
            Console.WriteLine(result);
            Console.WriteLine($"Solution found in {millis} milliseconds.");
            sw.Stop();
        }
    }
}