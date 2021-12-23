using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode.Solutions.Day22
{
    internal class SolutionPart2 : LongSolution
    {

        public long Solve()
        {
            IEnumerable<string> lines = File.ReadAllLines("./Content/Day22/1.txt");

            // Just brute force it to get part 2 :)
            List<Command> commands = new List<Command>();
            foreach (string line in lines)
            {
                commands.Add(new Command(line));
            }

            for (int i = 0; i < commands.Count; ++i)
            {
                for (int j = 0; j < i; ++j)
                {
                    // Skip nodes, that are remove, they were already used.
                    // Remove only from additions
                    if (commands[j].IsAddition)
                        commands[j].Node.InsertRegion(in commands[i].Region, commands[i].IsAddition);
                }
            }

            long volume = 0L;
            foreach (Command c in commands)
            {
                // Removals were already used and applied
                if (c.IsAddition)
                    volume += c.Node.Volume;
            }
            
            return volume;
        }

        private struct Region
        {

            internal long MinX;
            internal long MinY;
            internal long MinZ;
            internal long MaxX;
            internal long MaxY;
            internal long MaxZ;

            public long Volume => (MaxX - MinX + 1) * (MaxY - MinY + 1) * (MaxZ - MinZ + 1);

            public bool IsInRegion(long x, long y, long z) => MinX <= x && MinY <= y && MinZ <= z && x <= MaxX && y <= MaxY && z <= MaxZ;

            public Region(long minX, long minY, long minZ, long maxX, long maxY, long maxZ)
            {
                MinX = minX; MinY = minY; MinZ = minZ;
                MaxX = maxX; MaxY = maxY; MaxZ = maxZ;
            }

            public Region(Region other)
            {
                MinX = other.MinX; MinY = other.MinY; MinZ = other.MinZ;
                MaxX = other.MaxX; MaxY = other.MaxY; MaxZ = other.MaxZ;
            }

            public static bool Intersect(in Region r0, in Region r1)
            {
                return r0.MinX <= r1.MaxX && r0.MaxX >= r1.MinX && r0.MinY <= r1.MaxY && r0.MaxY >= r1.MinY && r0.MinZ <= r1.MaxZ && r0.MaxZ >= r1.MinZ;
            }

            public bool Contains(in Region contained)
            {
                return
                    contained.MinX >= MinX && contained.MaxX <= MaxX &&
                    contained.MinY >= MinY && contained.MaxY <= MaxY &&
                    contained.MinZ >= MinZ && contained.MaxZ <= MaxZ;
            }

            public static Region Difference(in Region r0, in Region r1)
            {
                return new Region(
                    Math.Max(r0.MinX, r1.MinX),
                    Math.Max(r0.MinY, r1.MinY),
                    Math.Max(r0.MinZ, r1.MinZ),
                    Math.Min(r0.MaxX, r1.MaxX),
                    Math.Min(r0.MaxY, r1.MaxY),
                    Math.Min(r0.MaxZ, r1.MaxZ));
            }

            public static Region Merge(in Region r0, in Region r1)
            {
                return new Region(
                    Math.Min(r0.MinX, r1.MinX),
                    Math.Min(r0.MinY, r1.MinY),
                    Math.Min(r0.MinZ, r1.MinZ),
                    Math.Max(r0.MaxX, r1.MaxX),
                    Math.Max(r0.MaxY, r1.MaxY),
                    Math.Max(r0.MaxZ, r1.MaxZ));
            }

        }

        private class Node
        {
            private List<Node> _nodes = new List<Node>();
            private bool _add;
            private Region _region;

            public bool IsLeaf => _nodes.Count == 0;
            public IEnumerable<Node> Nodes => _nodes;
            public long Volume => ComputeVolume(this);

            public Node(Region region, bool add)
            {
                _region = region;
                _add = add;
            }

            public void InsertRegion(in Region r, bool add)
            {
                if (Region.Intersect(in r, in _region))
                {
                    // If intersects compute intersection
                    Region innerRegion = Region.Difference(in r, in _region);

                    // If both add then insert remove to prevent duplicities
                    // If both remove then insert add to prevent duplicate removal
                    bool isAddition = add;
                    if (add && add == _add)
                        isAddition = false;
                    else if (!add && add == _add)
                        isAddition = true;

                    // Go through children
                    foreach (Node node in _nodes)
                        node.InsertRegion(in innerRegion, add);

                    _nodes.Add(new Node(innerRegion,isAddition));
                }
            }

            private static long ComputeVolume(Node node)
            {
                if (node.IsLeaf)
                {
                    return node.SignedVolume();
                }
                
                long volume = node.SignedVolume();
                foreach (Node child in node._nodes)
                {
                    volume += ComputeVolume(child);
                }

                return volume;
            }

            private long SignedVolume()
            {
                long volume = _region.Volume;
                return volume * (_add ? 1 : -1);
            }

        }

        private class Command
        {
            public readonly bool IsAddition;
            public readonly long MinX;
            public readonly long MinY;
            public readonly long MinZ;
            public readonly long MaxX;
            public readonly long MaxY;
            public readonly long MaxZ;
            public Region Region;
            public Node Node;


            internal Command(string line)
            {
                string[] splitted = line.Split(' ');
                IsAddition = splitted[0] == "on";
                splitted = splitted[1].Split(',');
                string[] x = splitted[0].Substring(2).Split("..");
                string[] y = splitted[1].Substring(2).Split("..");
                string[] z = splitted[2].Substring(2).Split("..");

                MinX = long.Parse(x[0]);
                MaxX = long.Parse(x[1]);

                MinY = long.Parse(y[0]);
                MaxY = long.Parse(y[1]);

                MinZ = long.Parse(z[0]);
                MaxZ = long.Parse(z[1]);

                Region = new Region(MinX, MinY, MinZ, MaxX, MaxY, MaxZ);
                Node = new Node(Region, IsAddition);
            }

        }

    }

}
