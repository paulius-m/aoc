using System.Numerics;
using Tools;
using static Tools.Matrices;
using static System.Numerics.Matrix4x4;
using static Days.Y2021.Day19.Solution;
using static MoreLinq.Extensions.SplitExtension;
using System.Collections.Generic;

namespace Days.Y2021.Day19;

public class Solution : ISolution<(Vector3[] beacons, Dictionary<Scanner, Vector3> positions)>
{
    private static Matrix4x4[] Orientations = (from m1 in Rotations
                                               from m2 in Rotations
                                               select Multiply(m1, m2)).Distinct().ToArray();
    public async Task<(Vector3[] beacons, Dictionary<Scanner, Vector3> positions)> LoadInput()
    {
        var lines = await File.ReadAllLinesAsync(this.GetInputFile("input"));

        return Connect(lines.Split("")
            .Select(g => new Scanner(
                g.First(),
                g.Skip(1).Select(s => s.Split(',')
                                       .Select(int.Parse)
                                       .ToArray() switch
                                       {
                                           int[] a => new Vector3(a[0], a[1], a[2])
                                       }).ToArray()
                ))
            .ToArray());
    }

    public object Part1((Vector3[] beacons, Dictionary<Scanner, Vector3> positions) scanners)
    {
        return scanners.beacons.Length;
    }

    public object Part2((Vector3[] beacons, Dictionary<Scanner, Vector3> positions) scanners)
    {
        var positions = scanners.positions;

        return (from p1 in positions.Values
                from p2 in positions.Values
                select Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y) + Math.Abs(p1.Z - p2.Z)).Max();
    }


    private (Vector3[] beacons, Dictionary<Scanner, Vector3> positions) Connect(Scanner[] scanners)
    {
        Scanner uberScanner = scanners[0];

        Dictionary<Scanner, Vector3> distances = new()
        {
            [uberScanner] = new Vector3(0)
        };

        var unconnected = scanners.Skip(1).ToArray();
        while (unconnected.Any())
        {
            var connected = new List<Scanner>();

            foreach (var s1 in unconnected)
            {
                if (s1.Match(uberScanner) is var (a, of) && a.Length > 0)
                {
                    connected.Add(s1);
                    uberScanner = new Scanner(uberScanner.Name, uberScanner.Beacons.Concat(a).Distinct().ToArray());
                    distances[s1] = of;
                }
            }

            unconnected = unconnected.Except(connected).ToArray();
        }
        return (uberScanner.Beacons, distances);
    }

    public class Scanner
    {
        public string Name { get; }
        public Vector3[] Beacons { get; }

        private readonly Vector3[][] _rotated;

        public Scanner(string name, Vector3[] beacons)
        {
            Name = name;
            Beacons = beacons;
            _rotated = Orientations.Select(o => beacons.Select(b => Vector3.Transform(b, o)).ToArray()).ToArray();
        }

        public (Vector3[] beacons, Vector3 offset) Match(Scanner s)
        {
            Vector3[] beacons = s.Beacons;
            for (int i = 0; i < _rotated.Length; i++)
            {
                var r = _rotated[i];
                var match = from sb in beacons
                            from rb in r
                            select sb - rb into d
                            group d by d into g
                            select (g.Key, Count: g.Count());

                if (match.MaxBy(m => m.Count) is (var dist, Count: >= 12))
                {
                    return (r.Select(rb => rb + dist).ToArray(), dist);
                }
            }

            return (Array.Empty<Vector3>(), default);
        }
    }
}