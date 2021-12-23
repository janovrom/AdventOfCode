using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Day19
{
    internal class SolutionPart1 : IntSolution
    {

        private List<Scanner> _scanners = new List<Scanner>();

        public int Solve()
        {
            Scanner scanner = null;
            int id = 0;
            foreach (string line in File.ReadAllLines("./Content/Day19/1.txt"))
            {
                if (scanner is null)
                {
                    // Start scanner on first line
                    scanner = new Scanner(id++);

                }
                else
                {
                    if (String.IsNullOrEmpty(line))
                    {
                        // End of scanner
                        _scanners.Add(scanner);
                        scanner = null;
                    }
                    else
                    {
                        string[] splitted = line.Split(',');
                        int x = int.Parse(splitted[0]);
                        int y = int.Parse(splitted[1]);
                        int z = int.Parse(splitted[2]);
                        scanner.AddPoint(new Vector3(x, y, z));
                    }
                }
            }

            if (scanner is not null)
            {
                _scanners.Add(scanner);
                scanner = null;
            }

            return RansacScanners();
        }

        private int RansacScanners()
        {
            List<CoordinateSystem> systems = Hypotheses.Generate();
            // Scanner 0 is the relative system
            Scanner scanner0 = _scanners[0];
            _scanners.RemoveAt(0);

            HashSet<Vector3> beacons = new HashSet<Vector3>(scanner0.Points, new DistanceComparer());

            foreach (Scanner sc in _scanners)
                sc.InitializeSystems(systems);



            while (_scanners.Count > 0)
            {
                ConcurrentQueue<(Scanner, List<Vector3>)> foundModels = new();

                Parallel.ForEach(_scanners, scanner =>
                {
                    // Select hypothesis - pair of points potentially matching and coordinate system
                    // We have to pick all possible variants
                    (Vector3 scannerCenter, List<Vector3> addedPoints) = GetValidModel(scanner0, scanner, systems);
                    if (addedPoints is not null)
                        foundModels.Enqueue((scanner, addedPoints));
                });

                foreach ((Scanner sc, List<Vector3> addedPoints) in foundModels)
                {
                    _scanners.Remove(sc);
                    foreach (Vector3 point in addedPoints)
                    {
                        // Even when not used in the model, they could already be there
                        if (!beacons.Contains(point))
                        {
                            beacons.Add(point);
                            scanner0.Points.Add(point);
                        }
                    }

                }
            }

            return beacons.Count;
        }

        private (Vector3, List<Vector3>) GetValidModel(Scanner s0, Scanner s1, List<CoordinateSystem> systems)
        {
            for (int s = 0; s < systems.Count; s++)
            {
                List<Vector3> points = s1.PreTransformedPoints[s];

                foreach ((int i, int j) in GetPairs(s0, s1))
                {
                    // Hypothesis for this pair of points
                    // v0 + translation = CoordinateSystem * v
                    Vector3 translation = points[j] - s0.Points[i];
                    // Gather all matching points
                    HashSet<int> maybeModelFromI = new();
                    HashSet<int> maybeModelFromJ = new();

                    // Try to fit all points to a model.
                    foreach ((int maybeI, int maybeJ) in GetPairs(s0, s1))
                    {
                        // If one point is in the model, it cannot be used again.
                        // Disqualify such pairs.
                        if (maybeModelFromI.Contains(maybeI) || maybeModelFromJ.Contains(maybeJ))
                            continue;

                        // Test the hypothesis
                        Vector3 v0 = s0.Points[maybeI];
                        Vector3 v = points[maybeJ];
                        Vector3 t = v - v0;
                        if (Vector3.DistanceSquared(t, translation) < 0.01f)
                        {
                            maybeModelFromI.Add(maybeI);
                            maybeModelFromJ.Add(maybeJ);
                        }
                    }

                    if (maybeModelFromJ.Count >= 12)
                    {
                        List<Vector3> addedPoints = new();
                        // This is valid model
                        for (int p = 0; p < points.Count; ++p)
                        {
                            if (!maybeModelFromJ.Contains(p))
                            {
                                addedPoints.Add(points[p] - translation);
                            }
                        }

                        return (-translation, addedPoints);
                    }
                }
            }

            return (new Vector3(), null);
        }

        private IEnumerable<(int, int)> GetPairs(Scanner s0, Scanner s1)
        {
            for (int i = 0; i < s0.Points.Count; ++i)
                for (int j = 0; j < s1.Points.Count; ++j)
                    yield return (i, j);
        }

    }

    internal class DistanceComparer : IEqualityComparer<Vector3>
    {

        public bool Equals(Vector3 x, Vector3 y)
        {
            return Vector3.DistanceSquared(x, y) < 0.001f;
        }

        public int GetHashCode([DisallowNull] Vector3 obj)
        {
            return Vector3.DistanceSquared(new Vector3(), obj).GetHashCode();
        }
    }
}
