using System.Numerics;
using Tools;

namespace Days.Y2022.Day18;

using Volume = HashSet<Vector3>;

file class Solution : ISolution<Volume>
{
    public static int[] NCoords = new[] { -1, 0, 1 };

    private static Vector3[] Near6 = (from x in NCoords
                                      from y in NCoords
                                      from z in NCoords
                                      where Math.Abs(x) + Math.Abs(y) + Math.Abs(z) is 1
                                      select new Vector3(x, y, z)).ToArray();

    public static IEnumerable<Vector3> GetNear(Vector3 center, Vector3[] near) => from nc in near select center + nc;

    public async Task<Volume> LoadInput()
    {
        return (from c in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                select c.Split(",").SelectArray(int.Parse) switch
                {
                [var x, var y, var z] => new Vector3(x, y, z)
                }).ToHashSet();
    }

    public object Part1(Volume volume)
    {
        var touching = volume.Aggregate(0, (a, c) => a + GetNear(c, Near6).Count(volume.Contains));

        return volume.Count * 6 - touching;
    }

    public object Part2(Volume volume)
    {
        Volume shell = FindShell(volume);

        var touching = volume.Aggregate(0, (a, c) => a + GetNear(c, Near6).Count(shell.Contains));

        return touching;
    }

    private static Volume FindShell(Volume volume)
    {
        var min = new Vector3(volume.Min(v => v.X) - 1, volume.Min(v => v.Y) - 1, volume.Min(v => v.Z) - 1);
        var max = new Vector3(volume.Max(v => v.X) + 1, volume.Max(v => v.Y) + 1, volume.Max(v => v.Z) + 1);

        var shell = new Volume();
        var q = new Queue<Vector3>();
        q.Enqueue(min);
        shell.Add(min);

        while (q.Count > 0)
        {
            var current = q.Dequeue();

            foreach (var n in GetNear(current, Near6).Where(nn => !shell.Contains(nn) && !volume.Contains(nn)))
            {
                if (min.X <= n.X && n.X <= max.X
                    && min.Y <= n.Y && n.Y <= max.Y
                    && min.Z <= n.Z && n.Z <= max.Z)
                {
                    q.Enqueue(n);
                    shell.Add(n);
                }
            }
        }

        return shell;
    }
}