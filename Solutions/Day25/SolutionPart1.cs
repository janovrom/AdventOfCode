using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions.Day25
{
    internal class SolutionPart1 : IntSolution
    {


        public int Solve()
        {
            string[] lines = File.ReadAllLines("./Content/Day25/1.txt");
            int height = lines.Length;
            int width = lines.First().Length;

            Group[,] seaCucumbers = new Group[height+1, width+1];
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    seaCucumbers[i, j] = CharToGroup(lines[i][j]);
                }
            }

            return Iterate(seaCucumbers);

            static Group CharToGroup(char c) => c switch
            {
                '.' => Group.Empty,
                '>' => Group.Right,
                'v' => Group.Down,
            };
        }

        private static int Iterate(Group[,] seaCucumbers)
        {
            int changes = 1;
            int height = seaCucumbers.GetLength(0);
            int width = seaCucumbers.GetLength(1);
            Group[,] output = new Group[height, width];
            int iteration = 0;
            while (changes > 0)
            {
                changes = 0;
                Parallel.For(0, height-1, (h) => 
                {
                    bool shifted = false;
                    output[h, width - 1] = seaCucumbers[h, 0];
                    seaCucumbers[h, width - 1] = seaCucumbers[h, 0];

                    for (int i = 0; i < width - 1; ++i)
                    {
                        output[h, i] = seaCucumbers[h, i];
                        if (seaCucumbers[h, i] != Group.Right)
                        {
                            continue;
                        }

                        int x = h;
                        int y = i + 1;
                        if (seaCucumbers[x, y] == Group.Empty)
                        {
                            output[x, y] = seaCucumbers[h, i];
                            output[h, i] = Group.Empty;
                            shifted = true;
                            if (i == width - 2)
                            {
                                output[h, 0] = output[x, y];
                            }
                            ++i;
                        }
                    }

                    if (shifted)
                        Interlocked.Increment(ref changes);
                });

                (seaCucumbers, output) = (output, seaCucumbers);
                Clean(output);

                Parallel.For(0, width-1, (w) =>
                {
                    bool shifted = false;
                    output[height - 1, w] = seaCucumbers[0, w];
                    seaCucumbers[height - 1, w] = seaCucumbers[0, w];

                    for (int i = 0; i < height - 1; ++i)
                    {
                        output[i, w] = seaCucumbers[i, w];
                        if (seaCucumbers[i, w] != Group.Down)
                        {
                            continue;
                        }

                        int x = i + 1;
                        int y = w;

                        if (seaCucumbers[x, y] == Group.Empty)
                        {
                            output[x, y] = seaCucumbers[i, w];
                            output[i, w] = Group.Empty; 
                            shifted = true;
                            if (i == height - 2)
                            {
                                output[0, w] = output[x, y];
                            }
                            ++i;
                        }
                    }

                    if (shifted)
                        Interlocked.Increment(ref changes);
                });

                (seaCucumbers, output) = (output, seaCucumbers);
                Clean(output);

                ++iteration;
            }

            return iteration;
        }

        private static void Clean(Group[,] array)
        {
            for (int i = 0; i < array.GetLength(0); ++i)
            {
                for (int j = 0; j < array.GetLength(1); ++j)
                {
                    array[i, j] = Group.Empty;
                }
            }
        }

        private static void Print(Group[,] seaCucumbers)
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < seaCucumbers.GetLength(0); ++i)
            {
                for (int j = 0; j < seaCucumbers.GetLength(1); ++j)
                {
                    Console.Write(GroupToChar(seaCucumbers[i, j]));
                }
                Console.WriteLine();
            }

            static char GroupToChar(Group g) => g switch
            {
                Group.Empty => '.',
                Group.Right => '>',
                Group.Down => 'v',
            };
        }

        private enum Group
        {
            Empty = 0,
            Right = 1,
            Down = 2
        }

    }

}
