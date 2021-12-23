using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Day18
{
    internal class SolutionPart2 : IntSolution
    {

        public int Solve()
        {
            List<string> numbers = new List<string>();
            foreach (string line in File.ReadAllLines("./Content/Day18/1.txt"))
            {
                numbers.Add(line);
            }

            // Very inefficient with all the multiparsing and toarrays, but I don't care, this took 
            // more then 1.5 hour altogether...
            int maxSize = 0;
            int maxi = 0;
            int maxj = 0;
            for (int i = 0; i < numbers.Count; ++i)
            {
                for (int j = 0; j < numbers.Count; ++j)
                {
                    if (i != j)
                    {
                        int ij = ReduceSize(new Pair(MakeMath(numbers[i]), MakeMath(numbers[j])));
                        int ji = ReduceSize(new Pair(MakeMath(numbers[j]), MakeMath(numbers[i])));
                        if (ij > maxSize)
                        {
                            maxSize = ij;
                            maxi = i;
                            maxj = j;
                        }

                        if (ji > maxSize)
                        {
                            maxSize = ji;
                            maxi = j;
                            maxj = i;
                        }
                    }
                }
            }

            return maxSize;
        }

        private static int ReduceSize(Pair result)
        {
            while (true)
            {
                if (!Reduce(result))
                {
                    if (!Split(result))
                    {
                        return Size(result);
                    }
                }
            }
        }

        private static void Print(INumber number)
        {
            StringBuilder stringBuilder = new StringBuilder();
            ToString(number, stringBuilder);
            Console.WriteLine(stringBuilder.ToString());
        }

        private static INumber MakeMath(string line)
        {
            Stack<INumber> stack = new();
            INumber result = null;
            for (int i = 0; i < line.Length; ++i)
            {
                switch (line[i])
                {
                    case '[':
                        stack.Push(new Pair());
                        if (i == 0)
                            result = stack.Peek();
                        break;
                    case ']':
                        INumber right = stack.Pop();
                        INumber left = stack.Pop();
                        Pair parent = (Pair)stack.Peek();
                        parent.Left = left;
                        parent.Right = right;
                        left.Parent = parent;
                        right.Parent = parent;
                        break;
                    case ',':
                        break;
                    default:
                        // a number
                        int n = line[i] - '0';
                        stack.Push(new Value(n));
                        break;
                }
            }
            return result;
        }

        private static bool Reduce(Pair pair)
        {
            (int, Value)[] pairs = GetValues(pair, 0).ToArray();
            for (int i = 0; i < pairs.Length; i++)
            {
                (int depth, Value v) = pairs[i];

                if (v.Parent is Pair p && p.IsLiteral && depth > 4)
                {
                    if (i > 0)
                        pairs[i - 1].Item2.Number += v.Number;

                    if (i < pairs.Length - 2)
                        pairs[i + 2].Item2.Number += pairs[i + 1].Item2.Number;

                    ((Pair)v.Parent).Remove();
                    ++i;
                    return true;
                }
            }

            return false;
        }

        private static bool Split(INumber pair)
        {
            foreach ((int depth, Value v) in GetValues(pair, 0))
            {
                if (v.Number >= 10)
                {
                    v.Split();
                    return true;
                }
            }

            return false;
        }

        private static int Size(INumber number)
        {
            if (number is Value v)
            {
                return v.Number;
            }
            else
            {
                Pair p = number as Pair;
                return 3 * Size(p.Left) + 2 * Size(p.Right);
            }
        }

        private static void ToString(INumber number, StringBuilder sb)
        {
            if (number is Value v)
            {
                sb.Append(v.Number);
            }
            else
            {
                Pair p = number as Pair;
                sb.Append('[');
                ToString(p.Left, sb);
                sb.Append(',');
                ToString(p.Right, sb);
                sb.Append(']');
            }
        }

        private static IEnumerable<(int, Value)> GetValues(INumber number, int depth)
        {
            if (number is Value v)
                yield return (depth, v);
            else
            {
                Pair p = number as Pair;
                foreach (var enumerable in GetValues(p.Left, depth + 1))
                    yield return enumerable;

                foreach (var enumerable in GetValues(p.Right, depth + 1))
                    yield return enumerable;
            }
        }

        private static IEnumerable<(int, Pair)> GetPairs(INumber number, int depth)
        {
            if (number is Pair p)
            {
                if (p.IsLiteral)
                    yield return (depth, p);
                else
                {
                    foreach (var enumerable in GetPairs(p.Left, depth + 1))
                        yield return enumerable;

                    foreach (var enumerable in GetPairs(p.Right, depth + 1))
                        yield return enumerable;

                }
            }
        }

        private class Pair : INumber
        {

            public INumber Left;
            public INumber Right;
            public bool IsLiteral => Left is Value && Right is Value;
            public INumber Parent { get; set; }

            public Pair() { }

            public Pair(INumber left, INumber right)
            {
                Left = left;
                Right = right;
                Left.Parent = this;
                Right.Parent = this;
            }

            public Pair(int l, int r)
            {
                Left = new Value(l);
                Right = new Value(r);
                Left.Parent = this;
                Right.Parent = this;
            }

            internal void Replace(INumber value, INumber pair)
            {
                if (Left == value)
                {
                    Left = pair;
                    Left.Parent = this;
                }
                else
                {
                    Right = pair;
                    Right.Parent = this;
                }
            }

            internal void Remove()
            {
                ((Pair)Parent).Replace(this, new Value(0));
            }
        }

        private class Value : INumber
        {
            public int Number;
            public INumber Parent { get; set; }

            public Value(int n)
            {
                Number = n;
            }

            public void Split()
            {
                int left = Number / 2;
                int right = (int)(Math.Ceiling(Number / 2.0));
                ((Pair)Parent).Replace(this, new Pair(left, right));
                Parent = null;
            }

        }

        internal interface INumber
        {
            INumber Parent { get; set; }
        }

    }

}
