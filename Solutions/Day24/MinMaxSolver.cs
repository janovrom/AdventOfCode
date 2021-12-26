using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions.Day24
{
    internal class MinMaxSolver
    {

        // D is the divisor of Z
        // M is the number added to modulo
        // A is the number pushed to stack
        // What is the stack? It's the z that is non-zero:
        // If you think what is going in the code then the method Verify
        // describes it well. There is 7 divisions by 26. The result is
        // suppossed to be 0, that can happen only when the previous Zi-1 
        // was 0 or <1-25>. But if it was zero, then the previous Zi-2 was
        // also 0 => Z1 is zero, but in this case it's w1 + p (non-zero).
        // so at all times Zi has to be positive. Which means that at all
        // times (until the end) there has to be the same number of muls
        // and divs. 7 divs means 7 muls by 26. That's 13 operations, we can
        // safely ignore the first one, since it's always a push.
        // Multiplication is done, when pi == 1 => Zi-1 % 26 + Mi != wi.
        // Observe the Ms. When it's positive, it's always larger then 9 =>
        // it is never true (always at least 10) => pi for such Ms is 1.
        // This will give you the 7 muls, which means we cannot add more
        // multiplications by 26 => any division that's there has to have pi = 0.
        // If you consider 10 instead of 26, then multiplication/division is 
        // shifting the number up/down. The same goes for 26. You actually store
        // the value on Zi to some kind of stack where mul pushes the input wi + A[i]
        // and div pops the stack and gives you an equation Zi-1%26 + M[i] = wi.
        // From this you can also compute the Zi-1 popped from stack and its wi-1.
        private static readonly int[] _D = new int[14] { 1, 1, 1, 26, 26, 1, 1, 26, 1, 26, 1, 26, 26, 26 };
        private static readonly int[] _M = new int[14] { 12, 12, 12, -9, -9, 14, 14, -10, 15, -2, 11, -15, -9, -3 };
        private static readonly int[] _A = new int[14] { 9, 4, 2, 5, 1, 6, 11, 15, 7, 12, 15, 9, 12, 12 };

        private const int _InputLength = 14;

        public static bool Verify(long input)
        {
            long z = 0;
            for (int i = 0; i < _InputLength; ++i)
            {
                int w = GetInput(input, i);
                int p = (z % 26) + _M[i] == w ? 0 : 1;
                z = (z / _D[i]) * (25 * p + 1) + p * (w + _A[i]);
            }

            return z == 0;
        }

        public static (long, long) Solve()
        {
            Stack<(int, int, int)> stack = new();
            Dictionary<int, (int, int, int)> indexedMinMaxs = new();
            for (int i = 0; i < _InputLength; ++i)
            {
                if (_D[i] > 1)
                {
                    // stack pop
                    (int j, int min, int max) = stack.Pop();
                    indexedMinMaxs.Add(i, (j, Math.Max(min + _M[i], 1), Math.Min(max + _M[i], 9)));
                }
                else
                {
                    // add to stack
                    stack.Push((i, 1 + _A[i], 9 + _A[i]));
                }
            }

            int[] solutionMin = new int[_InputLength];
            int[] solutionMax = new int[_InputLength];
            foreach ((int i, (int j, int min, int max)) in indexedMinMaxs)
            {
                solutionMin[i] = min;
                solutionMax[i] = max;
                solutionMin[j] = min - _M[i] - _A[j];
                solutionMax[j] = max - _M[i] - _A[j];
            }

            return (ToInput(solutionMin), ToInput(solutionMax));
        }

        private static long ToInput(int[] numbers)
        {
            long result = 0;
            for (int i = 0; i < numbers.Length; ++i)
            {
                result += numbers[i];
                result *= 10;
            }

            return result / 10; // Remove the last multiplication
        }

        private static int GetInput(long input, int position)
        {
            for (int i = position; i < _InputLength - 1; ++i)
                input /= 10;

            return (int)(input % 10);
        }

    }
}
