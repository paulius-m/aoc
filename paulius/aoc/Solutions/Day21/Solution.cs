using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Tools;
using static Tools.Neighbourhoods;

namespace Days.Day21;
using Input = Tools.Grid<char>;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return new((await File.ReadAllLinesAsync(this.GetInputFile("input")))
                .SelectMany((r, ri) => r.Select((c, ci) => KeyValuePair.Create(new Coord2D(ri, ci), c)))
                );
    }

    public object Part1(Input grid)
    {
        return GetCount2(grid, grid.First(kv => kv.Value is 'S').Key, 64);
    }

    public object Part2(Input grid)
    {
        Test();
        return GetCount2(grid, grid.First(kv => kv.Value is 'S').Key, 26501365);
    }

    public delegate long? CostFunction(Input map, Coord2D from, Coord2D to);
    public static int GetCount(Input map, Coord2D start, int steps)
    {
        var q = new HashSet<Coord2D>() { start };

        for (int i = 0; i < steps; i++)
        {
            q = new(from current in q
                    from n in GetNear4(current)
                    where map.ContainsKey(n) && (map[n] is '.' or 'S')
                    select n);
        }

        return q.Count;
    }

    public static long GetCount2(Input map, Coord2D start, int steps)
    {
        var rmap = (from kv in map
                   where kv.Value is '.' or 'S'
                   select kv.Key).ToHashSet();

        var maxR = map.Keys.Max(k => k.r) + 1;
        var maxC = map.Keys.Max(k => k.c) + 1;

        HashSet<Coord2D> q = [start];
        HashSet<Coord2D>[] prev =
        [
            [], []
        ];
        long[] prevL = new long[2];

        for (int i = 0; i < steps; i++)
        {
            prev[i % 2] = q;
            var preI = (i + 1) % 2;
            prevL[preI] += prev[preI].Count;

            var t = new HashSet<Coord2D>();
            foreach (var current in q)
            {
                foreach (var n in GetNear4(current))
                {
                    if (rmap.Contains(Pin(n, maxR, maxC)) && !prev[preI].Contains(n))
                    {
                        t.Add(n);
                    }
                }
            }
            q = t;

        }

        return q.Count + prevL[steps % 2];
    }

    static Coord2D Pin(Coord2D n, long maxR, long maxC)
    {
        return new Coord2D(Mod(n.r, maxR), Mod(n.c, maxC));

        long Mod(long x, long m)
        {
            return (x % m + m) % m;
        }
    }

    private void Test()
    {
        Debug.Assert(Pin(new Coord2D(1, 1), 2, 2) == new Coord2D(1, 1));
        Debug.Assert(Pin(new Coord2D(-1, -1), 2, 2) == new Coord2D(1, 1));
        Debug.Assert(Pin(new Coord2D(3, 3), 2, 2) == new Coord2D(1, 1));
    }
}