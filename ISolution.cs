namespace AdventOfCode
{
    internal interface ISolution<T>
    {

        T Solve();

    }

    internal interface IntSolution : ISolution<int> { }
    internal interface LongSolution : ISolution<long> { }
}
