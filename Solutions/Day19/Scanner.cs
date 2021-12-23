using System.Collections.Generic;
using System.Numerics;

namespace AdventOfCode.Solutions.Day19
{
    internal class Scanner
    {

        public int Id { get; }
        public List<Vector3> Points { get; } = new List<Vector3>();
        public Vector3 Center { get; internal set; }

        public List<List<Vector3>> PreTransformedPoints = new();

        internal Scanner(int id)
        {
            Id = id;
        }

        internal void AddPoint(Vector3 vector3)
        {
            Points.Add(vector3);
        }

        internal void InitializeSystems(List<CoordinateSystem> systems)
        {
            for (int i = 0; i < systems.Count; i++)
            {
                PreTransformedPoints.Add(new List<Vector3>(Points.Count));
                for (int j = 0; j < Points.Count; j++)
                {
                    PreTransformedPoints[i].Add(Vector3.TransformNormal(Points[j], systems[i].Transform));
                }
            }
        }

    }
}
