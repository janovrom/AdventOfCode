using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day13
{
    internal class SolutionPart2 : IntSolution
    {

        public int Solve()
        {
            string[] lines = File.ReadAllLines("./Content/Day13/1.txt").ToArray();
            int emptyLineIndex = GetIndex(lines);
            int[,] indices = ToIndices(lines, emptyLineIndex);
            GetMaxima(indices, out int right, out int down);

            bool[,] marks = new bool[down + 1, right + 1];
            FillInMarks(indices, marks);

            int i = down + 1;
            int j = right + 1;
            foreach (Instruction instruction in ReadInstructions(lines, emptyLineIndex + 1))
            {
                if (instruction.Up)
                {
                    FlipUp(marks, instruction.Index, i, j);
                    i = instruction.Index;
                }
                else
                {
                    FlipLeft(marks, instruction.Index, i, j);
                    j = instruction.Index;
                }
            }

            return SumRegion(marks, i, j);
        }

        private int SumRegion(bool[,] marks, int endDown, int endRight)
        {
            int count = 0;
            for (int i = 0; i < endDown; ++i)
            {
                for (int j = 0; j < endRight; ++j)
                {
                    if (marks[i, j])
                        ++count;

                    if (marks[i, j])
                        Console.Write("#");
                    else
                        Console.Write(".");
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            return count;
        }

        private void FlipUp(bool[,] marks, int index, int endDown, int endRight)
        {
            for (int i = index + 1; i < endDown; ++i)
            {
                for (int j = 0; j < endRight; ++j)
                {
                    marks[endDown - i - 1, j] |= marks[i, j];
                }
            }
        }

        private void FlipLeft(bool[,] marks, int index, int endDown, int endRight)
        {
            for (int i = 0; i < endDown; ++i)
            {
                for (int j = index + 1; j < endRight; ++j)
                {
                    marks[i, endRight - j - 1] |= marks[i, j];
                }
            }
        }

        private int GetIndex(string[] lines)
        {
            for (int i = 0; i < lines.Length; ++i)
            {
                if (String.IsNullOrEmpty(lines[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        private int[,] ToIndices(string[] lines, int count)
        {
            int[,] indices = new int[count, 2];
            for (int i = 0; i < count; ++i)
            {
                string[] splitted = lines[i].Split(",");
                indices[i, 0] = int.Parse(splitted[0]);
                indices[i, 1] = int.Parse(splitted[1]);
            }

            return indices;
        }

        private void GetMaxima(int[,] indices, out int right, out int down)
        {
            right = 0;
            down = 0;

            for (int i = 0; i < indices.GetLength(0); ++i)
            {
                right = Math.Max(indices[i, 0], right);
                down = Math.Max(indices[i, 1], down);
            }
        }

        private void FillInMarks(int[,] indices, bool[,] marks)
        {
            for (int i = 0; i < indices.GetLength(0); ++i)
            {
                marks[indices[i, 1], indices[i, 0]] = true;
            }

            //for (int i = 0; i < marks.GetLength(0); ++i)
            //{
            //    for (int j = 0; j < marks.GetLength(1); ++j)
            //    {
            //        if (marks[i, j])
            //            Console.Write("#");
            //        else
            //            Console.Write(".");
            //    }
            //    Console.WriteLine();
            //}    
            //Console.WriteLine();
        }

        private IEnumerable<Instruction> ReadInstructions(string[] lines, int start)
        {
            Instruction instruction;
            for (int i = start; i < lines.Length; ++i)
            {
                Parse(lines[i], out instruction);
                yield return instruction;
            }
        }

        private void Parse(string line, out Instruction instruction)
        {
            string[] inst = line.Split(" ").Last().Split("=");
            instruction = new Instruction(inst[0] == "y", int.Parse(inst[1]));
        }

        private struct Instruction
        {
            public bool Up;
            public int Index;

            public Instruction(bool down, int index)
            {
                Up = down;
                Index = index;
            }
        }

    }
}
