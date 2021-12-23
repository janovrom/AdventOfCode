using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day16
{
    internal class SolutionPart2 : LongSolution
    {

        public long Solve()
        {
            byte[] data = File.ReadAllLines("./Content/Day16/1.txt").First().SelectMany(ToByteArray).ToArray();

            int offset = 0;
            Packet packet = PacketFactory.Create(data, ref offset);
            long result = GetValue(packet);
            return result;
        }


        private static long GetValue(Packet p)
        {
            long sum;
            if (p.Data is OperatorPacketData opd)
                sum = GetValueFromData(p.Header.Type, opd);
            else
                sum = ((LiteralValuePacketData)p.Data).Value;

            return sum;
        }

        private static long GetValueFromData(int operatorType, OperatorPacketData opd)
        {
            checked
            {
                return operatorType switch
                {
                    0 => opd.Packets.Sum(GetValue),
                    1 => opd.Packets.Aggregate(1L, (mul, a) => GetValue(a) * mul),
                    2 => opd.Packets.Min(GetValue),
                    3 => opd.Packets.Max(GetValue),
                    5 => GetValue(opd.Packets[0]) > GetValue(opd.Packets[1]) ? 1 : 0,
                    6 => GetValue(opd.Packets[0]) < GetValue(opd.Packets[1]) ? 1 : 0,
                    7 => GetValue(opd.Packets[0]) == GetValue(opd.Packets[1]) ? 1 : 0,
                };

            }
        }

        private static byte[] ToByteArray(char c) => c switch
        {
            '0' => new byte[] { 0, 0, 0, 0 },
            '1' => new byte[] { 0, 0, 0, 1 },
            '2' => new byte[] { 0, 0, 1, 0 },
            '3' => new byte[] { 0, 0, 1, 1 },
            '4' => new byte[] { 0, 1, 0, 0 },
            '5' => new byte[] { 0, 1, 0, 1 },
            '6' => new byte[] { 0, 1, 1, 0 },
            '7' => new byte[] { 0, 1, 1, 1 },
            '8' => new byte[] { 1, 0, 0, 0 },
            '9' => new byte[] { 1, 0, 0, 1 },
            'A' => new byte[] { 1, 0, 1, 0 },
            'B' => new byte[] { 1, 0, 1, 1 },
            'C' => new byte[] { 1, 1, 0, 0 },
            'D' => new byte[] { 1, 1, 0, 1 },
            'E' => new byte[] { 1, 1, 1, 0 },
            'F' => new byte[] { 1, 1, 1, 1 },
            _ => throw new NotSupportedException($"Character {c} is not supported"),
        };

        private static class PacketFactory
        {

            public static Packet Create(byte[] packetBytes, ref int offset)
            {
                Packet packet = new();
                packet.Reconstruct(packetBytes, ref offset);
                if (packet.Data is null)
                    return null;

                return packet;
            }

            public static PacketData CreatePacketData(PacketHeader packetHeader)
            {
                return packetHeader.Type switch
                {
                    4 => new LiteralValuePacketData(),
                    _ => new OperatorPacketData(),
                };
            }
        }

        private abstract class PacketData : IReconstructable
        {
            public abstract void Reconstruct(byte[] array, ref int offset);
        }

        private class LiteralValuePacketData : PacketData
        {

            public long Value;

            public override void Reconstruct(byte[] array, ref int offset)
            {
                bool lastData = false;
                Value = 0;
                while (!lastData)
                {
                    lastData = !array.NextBool(ref offset);
                    Value = (Value << 4) + array.Next(4, ref offset);
                }
            }
        }

        private class OperatorPacketData : PacketData
        {

            public List<Packet> Packets = new List<Packet>();

            public override void Reconstruct(byte[] array, ref int offset)
            {
                bool isLengthType = !array.NextBool(ref offset);

                if (isLengthType)
                {
                    int byteLength = array.Next(15, ref offset);
                    int storedOffset = offset;
                    while (storedOffset + byteLength > offset)
                    {
                        Packets.Add(PacketFactory.Create(array, ref offset));
                    }
                }
                else
                {
                    int count = array.Next(11, ref offset);
                    for (int i = 0; i < count; ++i)
                        Packets.Add(PacketFactory.Create(array, ref offset));
                }
            }
        }

        private class Packet : IReconstructable
        {

            public PacketHeader Header;
            public PacketData Data;

            public void Reconstruct(byte[] array, ref int offset)
            {
                Header = new PacketHeader();
                Header.Reconstruct(array, ref offset);
                Data = PacketFactory.CreatePacketData(Header);
                Data?.Reconstruct(array, ref offset);

            }
        }

        private class PacketHeader : IReconstructable
        {

            public int Version;
            public int Type;

            public void Reconstruct(byte[] array, ref int offset)
            {
                Version = array.Next(3, ref offset);
                Type = array.Next(3, ref offset);
            }
        }

        private interface IReconstructable
        {
            public void Reconstruct(byte[] array, ref int offset);
        }

    }

}
