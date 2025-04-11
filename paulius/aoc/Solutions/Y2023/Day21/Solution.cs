using System.Data;
using System.Diagnostics;
using Tools;
using static Tools.Neighbourhoods;
using MoreLinq;

namespace Days.Y2023.Day21;
using Input = Grid<char>;
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
                    where map.ContainsKey(n) && map[n] is '.' or 'S'
                    select n);
        }

        return q.Count;
    }

    public static long GetCount2(Input map, Coord2D start, long steps)
    {
        var rmap = (from kv in map
                    where kv.Value is '.' or 'S'
                    select kv.Key).ToHashSet();

        var setRef = new Dictionary<HashSet<Coord2D>, int>(HashSet<Coord2D>.CreateSetComparer());
        var statsDic = new Dictionary<HashSet<int>, List<Stat>>(HashSet<int>.CreateSetComparer());
        var quadrants = new Dictionary<Coord2D, int>();

        var maxR = map.Keys.Max(k => k.r) + 1;
        var maxC = map.Keys.Max(k => k.c) + 1;

        var q = new HashSet<Coord2D>() { start };
        setRef[q] = 1;
        quadrants[new(0, 0)] = 1;
        var reference = 1;
        for (var i = 0; i < steps; i++)
        {
            var uniqueCount = setRef.Count;

            q = new(from current in q
                    from n in GetNear4(current)
                    where rmap.Contains(Pin(n, maxR, maxC))
                    select n);

            var nQuadrants = new Dictionary<Coord2D, int>();

            foreach (var (quad, set) in from c in q
                                        group Pin(c, maxR, maxC) by Quad(c, maxR, maxC) into quads
                                        select (quads.Key, quads.ToHashSet()))
            {
                if (!setRef.TryGetValue(set, out var cached))
                {
                    reference++;
                    setRef[set] = reference;
                    cached = reference;
                }

                nQuadrants[quad] = cached;
            }

            quadrants = nQuadrants;

            var stats = quadrants.GroupBy(kv => kv.Value).Select(g => KeyValuePair.Create(g.Key, g.LongCount())).OrderBy(g => g.Key).ToArray();
            var statsKey = stats.Select(kv => kv.Key).ToHashSet();

            var statsList = statsDic.TryGetValue(statsKey, out var l) ? l : new List<Stat>();
            statsList.Add(new Stat(i, stats));
            statsDic[statsKey] = statsList;

            if (statsList.Count > 3)
            {
                break;
            }
        }
        var reverseRef = (from kv in setRef
                          select KeyValuePair.Create(kv.Value, (long)kv.Key.Count)).ToDictionary();

        {
            var cyclic = statsDic.Where(kv => kv.Value.Count > 1).ToArray();
            var minIter = cyclic.Min(kv => kv.Value.Min(s => s.Iteration));

            var cycle = cyclic.SelectMany(kv => kv.Value.Pairwise((a, b) => b.Iteration - a.Iteration)).Distinct().Single();

            var startIter = (steps - 1) % cycle;
            for (; startIter < minIter; startIter += cycle) ;
            var loops = (steps - startIter - 1) / cycle;

            var stats = cyclic.First(s => s.Value.Any(ss => ss.Iteration == startIter)).Value;
            var startStat = stats.First(s => s.Iteration == startIter);

            var multipliers = new long[stats[0].Stats.Length];

            for (int i1 = 0; i1 < stats[0].Stats.Length; i1++)
            {
                var d1 = stats[1].Stats[i1].Value - stats[0].Stats[i1].Value;
                var d2 = stats[2].Stats[i1].Value - stats[1].Stats[i1].Value - d1;

                multipliers[i1] = startStat.Stats[i1].Value + loops * (loops - 1) / 2 * d2 + loops * d1;
            }

            return startStat.Stats.Zip(multipliers, (s, m) => reverseRef[s.Key] * m).Sum();
        }
    }

    static Coord2D Pin(Coord2D n, long maxR, long maxC)
    {
        return new Coord2D(Mod(n.r, maxR), Mod(n.c, maxC));

        long Mod(long x, long m)
        {
            return (x % m + m) % m;
        }
    }

    static Coord2D Quad(Coord2D n, long maxR, long maxC)
    {
        return new(Div(n.r, maxR), Div(n.c, maxC));

        long Div(long a, long b) { return a >= 0 ? a / b : (a - b + 1) / b; }
    }

    private void Test()
    {
        Debug.Assert(Pin(new Coord2D(1, 1), 2, 2) == new Coord2D(1, 1));
        Debug.Assert(Pin(new Coord2D(-1, -1), 2, 2) == new Coord2D(1, 1));
        Debug.Assert(Pin(new Coord2D(3, 3), 2, 2) == new Coord2D(1, 1));

        Debug.Assert(Quad(new Coord2D(1, 1), 2, 2) == new Coord2D(0, 0));
        Debug.Assert(Quad(new Coord2D(-1, -1), 2, 2) == new Coord2D(-1, -1));
        Debug.Assert(Quad(new Coord2D(-1, 1), 2, 2) == new Coord2D(-1, 0));
        Debug.Assert(Quad(new Coord2D(3, 3), 2, 2) == new Coord2D(1, 1));

        Debug.Assert(Quad(new Coord2D(1, 1), 2, 2) * 2 + Pin(new Coord2D(1, 1), 2, 2) == new Coord2D(1, 1));
        Debug.Assert(Quad(new Coord2D(-10, -10), 2, 2) * 2 + Pin(new Coord2D(-10, -10), 2, 2) == new Coord2D(-10, -10));
    }
}


file record Stat(long Iteration, KeyValuePair<int, long>[] Stats);