using System.Numerics;

namespace AdventOfCode.Solutions.Day19
{
    internal struct CoordinateSystem
    {

        public Vector3 AxisX;
        public Vector3 AxisY;
        public Vector3 AxisZ;
        public Matrix4x4 Transform;

        public CoordinateSystem(Vector3 x, Vector3 y, Vector3 z)
        {
            Transform = new Matrix4x4()
            {
                M11 = x.X,
                M12 = y.X,
                M13 = z.X,
                M21 = x.Y,
                M22 = y.Y,
                M23 = z.Y,
                M31 = x.Z,
                M32 = y.Z,
                M33 = z.Z,
            };

            AxisX = x;
            AxisY = y;
            AxisZ = z;
        }

    }

}
