using System.Data;
using System.Diagnostics;
using System.Text;
using Tools;
using static Tools.Neighbourhoods;
using System.Linq;

namespace Days.Y2023.Day21;
using Input = Grid<char>;
using Rule = (int, int, int, int, int);
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
        return GetCount3(grid, grid.First(kv => kv.Value is 'S').Key, 5000 /* 26501365*/);
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

    public static long GetCount3(Input map, Coord2D start, int steps)
    {
        var rmap = (from kv in map
                    where kv.Value is '.' or 'S'
                    select kv.Key).ToHashSet();

        var rules = new Dictionary<Rule, int>();

        var specialRules = new Dictionary<Rule, int>();

        var setRef = new Dictionary<HashSet<Coord2D>, int>(HashSet<Coord2D>.CreateSetComparer());

        var ruleLoops = new Dictionary<Coord2D, List<Rule>>();
        var quadrants = new Dictionary<Coord2D, int>();

        var maxR = map.Keys.Max(k => k.r) + 1;
        var maxC = map.Keys.Max(k => k.c) + 1;
        var empty2 = 0;
        var q = new HashSet<Coord2D>() { start };
        setRef[q] = 1;
        quadrants[new(0, 0)] = 1;

        var i = 0;
        var reference = 1;
        for (; i < steps; i++)
        {
            var uniqueCount = rules.Count;

            q = new(from current in q
                    from n in GetNear4(current)
                    where map.ContainsKey(Pin(n, maxR, maxC))
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

                var rule = GetRule(quadrants, empty2, quad, quadrants.TryGetValue(quad, out var quadSet) ? quadSet : empty2);

                rules[rule] = cached;

                nQuadrants[quad] = cached;
            }
            quadrants = nQuadrants;

            if (uniqueCount == rules.Count)
                break;
        }

        var groups = rules.GroupBy(x => x.Value).ToDictionary(g => g.Key, g => g.ToArray());
        var specialgroups = specialRules.GroupBy(x => x.Value).ToDictionary(g => g.Key, g => g.ToArray());

        for (; i < steps; i++)
        {
            var area = (from current in quadrants
                        from n in GetNear4(current.Key)
                        select n).ToHashSet();

            Console.WriteLine(ToVoidString(quadrants, -10, 10, 0, 0));
            Console.ReadLine();

            quadrants = new(from current in area
                             let rule = GetRule(quadrants, empty2, current, quadrants.TryGetValue(current, out var quadSet) ? quadSet : empty2)
                             where rules.TryGetValue(rule, out var next) && next != empty2
                             select KeyValuePair.Create(current, rules[rule]));
        }

        var reverseRef = (from kv in setRef
                          select KeyValuePair.Create(kv.Value, kv.Key)).ToDictionary();

        var qCount = quadrants.Sum(q => reverseRef[q.Value].Count);

        return qCount;
    }

    static string ToVoidString(Dictionary<Coord2D, int> dict, long minC, long maxC, long minR, long maxR)
    {
        var b = new StringBuilder();

        for (var r = minR; r <= maxR; r++)
        {
            for (var c = minC; c <= maxC; c++)
            {
                var at = new Coord2D(r, c);

                if (dict.TryGetValue(at, out int value))
                {
                    b.Append(value.ToString().PadLeft(4));

                }
                else { b.Append("    "); }
            }
            b.Append('\n');
        }

        return b.ToString();
    }

    private static Rule GetRule(Dictionary<Coord2D, int> quadrants, int empty, Coord2D quad, int set)
    {
        var quadN = (
                from n in GetNear4(quad)
                select quadrants.TryGetValue(n, out var quadSet) ? quadSet : empty).ToArray();

        var rule = (quadN[0], quadN[1], quadN[2], quadN[3], set);
        return rule;
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
