using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AdventOfCode.Solutions.Day20
{
    internal class SolutionPart2 : IntSolution
    {

        public int Solve()
        {
            IEnumerable<string> lines = File.ReadAllLines("./Content/Day20/1.txt");

            bool[] imageEnhancement = lines.First().Select(IsLit).ToArray();
            List<string> inputImage = lines.Skip(2).ToList();

            int width = inputImage[0].Length;
            int height = inputImage.Count;

            int iterations = 50;

            // Add padding for iterations.
            // When evaluation the first new image we already need padding:
            // 1. padding 1 for current border
            // 2. we will have new border => padding 2
            // 3. borderPadding has the same padding as iterations (each iteration we add 2 pixels on each side)
            int borderPadding = iterations;
            width += iterations * 2 + borderPadding * 2;
            height += iterations * 2 + borderPadding * 2;
            bool[,] image = new bool[width, height];

            int offset = iterations;
            for (int i = 0; i < inputImage.Count; ++i)
            {
                for (int j = 0; j < inputImage[i].Length; ++j)
                {
                    char c = inputImage[i][j];
                    image[i + offset + borderPadding, j + offset + borderPadding] = IsLit(c);
                }
            }

            while (offset > 0)
            {
                bool[,] output = new bool[width, height];
                for (int i = 1; i < height - 1; ++i)
                {
                    for (int j = 1; j < width - 1; ++j)
                    {

                        output[i, j] = imageEnhancement[GetEnhancedIndex(image, i, j)];
                    }
                }
                image = output;
                --offset;
            }


            int count = 0;
            for (int i = borderPadding; i < height - borderPadding; ++i)
            {
                for (int j = borderPadding; j < width - borderPadding; ++j)
                {
                    count += PixelToInt(image[i, j]);
                }
            }

            return count;
        }

        private static int GetEnhancedIndex(bool[,] image, int i, int j)
        {
            int index = 0;
            for (int ii = -1; ii <= 1; ++ii)
            {
                for (int jj = -1; jj <= 1; ++jj)
                {
                    // # to 1
                    // . to 0
                    index += PixelToInt(image[i + ii, j + jj]);
                    index <<= 1;
                }
            }

            // Remove the last move
            index >>= 1;

            return index;
        }

        private static bool IsLit(char c)
        {
            return c == '#';
        }

        private static int PixelToInt(bool isLit)
        {
            return isLit ? 1 : 0;
        }

        private static char PixelToChar(bool isLit)
        {
            return isLit ? '#' : '.';
        }

        private static void PrintLine(bool[,] image, int i)
        {
            for (int j = 0; j < image.GetLength(1); ++j)
            {
                Console.Write(PixelToChar(image[i, j]));
            }
        }

        private static void Print(bool[,] image)
        {
            for (int i = 0; i < image.GetLength(0); ++i)
            {
                for (int j = 0; j < image.GetLength(1); ++j)
                {
                    Console.Write(PixelToChar(image[i, j]));
                }
                Console.WriteLine();
            }
        }

    }

}
