namespace AdventOfCode.Solutions.Day24
{
    internal class SolutionPart2_Math : LongSolution
    {

        public long Solve()
        {
            long minimum = MinMaxSolver.Solve().Item1;
            if (!MinMaxSolver.Verify(minimum))
                throw new System.Exception("Result not correct.");

            return minimum;
        }

    }

}
