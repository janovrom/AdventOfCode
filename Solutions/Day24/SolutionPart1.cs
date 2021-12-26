using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Day24
{
    internal class SolutionPart1 : LongSolution
    {

        public long Solve()
        {
            IEnumerable<string> lines = File.ReadAllLines("./Content/Day24/1.txt");
            Program program = new Program();
            foreach (string line in lines)
            {
                program.AddInstruction(line);
            }

            Input input = new Input();
            foreach (Input i in GetInput(0, input))
            {
                i.Reset();
                if (program.Execute(i, new State()))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (int n in input.Numbers)
                        sb.Append(n);

                    string s = sb.ToString();
                    return long.Parse(s);
                }
            }

            return 0;
        }

        private IEnumerable<Input> GetInput(int length, Input input)
        {
            input.Numbers[0] = 9;
            input.Numbers[1] = 9;
            input.Numbers[2] = 9;
            input.Numbers[3] = 2;
            input.Numbers[4] = 4;
            input.Numbers[5] = 9;
            input.Numbers[6] = 9;
            input.Numbers[7] = 5;
            input.Numbers[8] = 4;
            input.Numbers[9] = 9;
            input.Numbers[10] = 9;
            input.Numbers[11] = 3;
            input.Numbers[12] = 3;
            input.Numbers[13] = 9;
            yield return input;

            //if (length == 14)
            //    yield return input;
            //else
            //{
            //    for (int i = length == 0 ? 4 : 9; i > 0; --i)
            //    {
            //        input.Numbers[length] = i;
            //        foreach (Input childInput in GetInput(length + 1, input))
            //            yield return input;
            //    }
            //}

        }

        private class Program
        {
            private List<Instruction> _instructions = new List<Instruction>();

            public bool Execute(Input input, State s)
            {
                foreach (Instruction instruction in _instructions)
                {
                    if (!instruction.Do(s, input))
                        return false;
                }

                return s.Z == 0;
            }

            public void AddInstruction(string line)
            {
                string[] splitted = line.Split(' ');
                if (splitted.Length == 2)
                {
                    _instructions.Add(new Inp());
                }
                else
                {
                    _instructions.Add(GetInstruction(splitted[0], splitted[1], splitted[2]));
                }
            }

            private static Instruction GetInstruction(string ins, string a, string b)
            {
                int r0 = 0;
                int r1 = 0;
                if (IsReg(a))
                {
                    r0 = GetRegIdx(a);
                }
                else
                {
                    throw new NotSupportedException($"Instruction {ins} with {a} {b} is not supported");
                }

                if (IsReg(b))
                {
                    r1 = GetRegIdx(b);
                    return GetInsReg(ins, r0, r1);
                }
                else
                {
                    r1 = int.Parse(b);
                    return GetInsN(ins, r0, r1);
                }

                static bool IsReg(string s) => s switch
                {
                    "w" => true,
                    "x" => true,
                    "y" => true,
                    "z" => true,
                    _ => false
                };

                static int GetRegIdx(string s) => s switch
                {
                    "w" => 0,
                    "x" => 1,
                    "y" => 2,
                    "z" => 3,
                };

                static Instruction GetInsReg(string ins, int r0, int r1) => ins switch
                {
                    "mul" => new Mul(r0, r1),
                    "add" => new Add(r0, r1),
                    "div" => new Div(r0, r1),
                    "mod" => new Mod(r0, r1),
                    "eql" => new Eql(r0, r1),
                };

                static Instruction GetInsN(string ins, int r0, int r1) => ins switch
                {
                    "mul" => new MulN(r0, r1),
                    "add" => new AddN(r0, r1),
                    "div" => new DivN(r0, r1),
                    "mod" => new ModN(r0, r1),
                    "eql" => new EqlN(r0, r1),
                };
            }

        }

        private abstract class Instruction
        {
            public abstract bool Do(State s, Input input);
        }

        private class Inp : Instruction
        {
            public override bool Do(State s, Input input)
            {
                s.W = input.Next();
                return true;
            }
        }

        private class Mul : Instruction
        {

            private int _a;
            private int _b;

            public Mul(int idx0, int idx1)
            {
                _a = idx0;
                _b = idx1;
            }

            public override bool Do(State s, Input input)
            {
                s.Reg[_a] *= s.Reg[_b];
                return true;
            }
        }

        private class MulN : Instruction
        {

            private int _a;
            private int _b;

            public MulN(int idx0, int idx1)
            {
                _a = idx0;
                _b = idx1;
            }

            public override bool Do(State s, Input input)
            {
                s.Reg[_a] *= _b;
                return true;
            }
        }

        private class Div : Instruction
        {

            private int _a;
            private int _b;

            public Div(int idx0, int idx1)
            {
                _a = idx0;
                _b = idx1;
            }

            public override bool Do(State s, Input input)
            {
                if (s.Reg[_b] <= 0)
                    return false;

                s.Reg[_a] /= s.Reg[_b];
                return true;
            }
        }

        private class DivN : Instruction
        {

            private int _a;
            private int _b;

            public DivN(int idx0, int idx1)
            {
                _a = idx0;
                _b = idx1;
            }

            public override bool Do(State s, Input input)
            {
                if (_b <= 0)
                    return false;

                s.Reg[_a] /= _b;
                return true;
            }
        }

        private class Mod : Instruction
        {

            private int _a;
            private int _b;

            public Mod(int idx0, int idx1)
            {
                _a = idx0;
                _b = idx1;
            }

            public override bool Do(State s, Input input)
            {
                if (s.Reg[_a] < 0 || s.Reg[_b] <= 0)
                    return false;

                s.Reg[_a] %= s.Reg[_b];
                return true;
            }
        }

        private class ModN : Instruction
        {

            private int _a;
            private int _b;

            public ModN(int idx0, int idx1)
            {
                _a = idx0;
                _b = idx1;
            }

            public override bool Do(State s, Input input)
            {
                if (s.Reg[_a] < 0 || _b <= 0)
                    return false;

                s.Reg[_a] %= _b;
                return true;
            }
        }

        private class Add : Instruction
        {

            private int _a;
            private int _b;

            public Add(int idx0, int idx1)
            {
                _a = idx0;
                _b = idx1;
            }

            public override bool Do(State s, Input input)
            {
                s.Reg[_a] += s.Reg[_b];
                return true;
            }
        }

        private class AddN : Instruction
        {

            private int _a;
            private int _b;

            public AddN(int idx0, int idx1)
            {
                _a = idx0;
                _b = idx1;
            }

            public override bool Do(State s, Input input)
            {
                s.Reg[_a] += _b;
                return true;
            }
        }

        private class Eql : Instruction
        {

            private int _a;
            private int _b;

            public Eql(int idx0, int idx1)
            {
                _a = idx0;
                _b = idx1;
            }

            public override bool Do(State s, Input input)
            {
                s.Reg[_a] = s.Reg[_a] == s.Reg[_b] ? 1 : 0;
                return true;
            }
        }

        private class EqlN : Instruction
        {

            private int _a;
            private int _b;

            public EqlN(int idx0, int idx1)
            {
                _a = idx0;
                _b = idx1;
            }

            public override bool Do(State s, Input input)
            {
                s.Reg[_a] = s.Reg[_a] == _b ? 1 : 0;
                return true;
            }
        }

        private class State
        {
            public int[] Reg = new int[4];

            public int W { get => Reg[0]; set => Reg[0] = value; } 
            public int X { get => Reg[1]; set => Reg[1] = value; } 
            public int Y { get => Reg[2]; set => Reg[2] = value; } 
            public int Z { get => Reg[3]; set => Reg[3] = value; }
        }

        private class Input
        {
            public int[] Numbers = new int[14] { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 };
            private int _current = 0;

            public int Next()
            {
                return Numbers[_current++];
            }

            public void Reset()
            {
                _current = 0;
            }
        }

    }

}
