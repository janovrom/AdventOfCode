namespace AdventOfCode.Solutions.Day24
{
    internal class SolutionPart1_Math : LongSolution
    {

        public long Solve()
        {
            long maximum = MinMaxSolver.Solve().Item2;
            if (!MinMaxSolver.Verify(maximum))
                throw new System.Exception("Result not correct.");

            return maximum;
        }

    }

}
