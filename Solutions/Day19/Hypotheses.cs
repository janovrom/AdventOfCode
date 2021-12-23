using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions.Day19
{
    internal static class Hypotheses
    {

        // Consider right-handed coordinate systems and rotations by 90 degrees
        // 24 different states

        public static List<CoordinateSystem> Generate()
        {
            HashSet<(Vector3, Vector3, Vector3)> systems = new();
            Vector3[] vectors = new Vector3[]
            {
                new Vector3(1f, 0f, 0f),
                new Vector3(-1f, 0f, 0f),
                new Vector3(0f, 1f, 0f),
                new Vector3(0f, -1f, 0f),
                new Vector3(0f, 0f, 1f),
                new Vector3(0f, 0f, -1f),
            };

            for (int x = 0; x < 6; ++x)
            {
                for (int y = 0; y < 6; ++y)
                {
                    if (x == y || vectors[x] + vectors[y] == new Vector3())
                        continue;

                    Vector3 z = Vector3.Cross(vectors[x], vectors[y]);
                    systems.Add((vectors[x], vectors[y], z));
                }
            }

            return systems.Select((t) => new CoordinateSystem(t.Item1, t.Item2,t.Item3)).ToList();
        }

    }
}
