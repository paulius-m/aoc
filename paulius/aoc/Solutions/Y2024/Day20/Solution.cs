using MoreLinq;
using Sprache;
using Tools;
using static Tools.Neighbourhoods;
using Pastel;

namespace Days.Y2024.Day20;

using static Tools.Neighbourhoods;
using Input = Grid<char>;
using PosPrev = (Coord2D Pos, Coord2D? Prev);

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return new Input((await File.ReadAllLinesAsync(this.GetInputFile("input"))).SelectMany((r, ri) => r.Select((c, ci) => Input.Coord(ri, ci, c))));
    }

    public object Part1(Input input)
    {
        var s = input.First(i => i.Value is 'S').Key;
        PosPrev start = (s, null);
        Coord2D end = input.First(i => i.Value is 'E').Key;

        var distance = GetDistance(input, start, Cost, Next);

        var path = GetPath(end, distance);

        return (from kv in input
                where kv.Value is '#'
                let near = (from n in GetNear4(kv.Key)
                            where path.ContainsKey(n)
                            select n).ToArray()
                where near.Length > 0
                from cheat in GetCombinations(near)
                let dist = Math.Abs(path[cheat.a] - path[cheat.b]) - 2
                where dist >= 100
                select dist).Count();
    }

    static long? Cost(Dictionary<Coord2D, char> map, PosPrev from, PosPrev to) => from.Pos.ManhatanDistance(to.Pos);

    static IEnumerable<PosPrev> Next(Dictionary<Coord2D, char> map, PosPrev f) => from next in GetNear4(f.Pos)
                                                                                  where map.TryGetValue(next, out var c) && c is not '#'
                                                                                  select (next, f.Pos);

    private static IEnumerable<(Coord2D a, Coord2D b)> GetCombinations(Coord2D[] near)
    {
        for (int i = 0; i < near.Length - 1; i++)
        {
            for (int j = i; j < near.Length; j++)
            {
                yield return (near[i], near[j]);
            }
        }
    }

    private static Dictionary<Coord2D, long> GetPath(Coord2D end, Dictionary<(Coord2D Pos, Coord2D? Prev), long> distance)
    {
        var q = new Queue<PosPrev>();
        var path = new Dictionary<Coord2D, long>();
        var visited = new HashSet<PosPrev>();
        distance.Where(d => d.Key.Pos == end).GroupBy(d => d.Value).MinBy(g => g.Key)!.ForEach(d => q.Enqueue(d.Key));

        while (q.Count > 0)
        {
            var current = q.Dequeue();

            if (!visited.Add(current))
            {
                continue;
            }
            var (pos, prev) = current;
            path.Add(pos, distance[current]);

            if (prev is null)
            {
                continue;
            }

            distance.Where(d => d.Key.Pos == prev).GroupBy(d => d.Value).MinBy(g => g.Key)!.ForEach(d => q.Enqueue(d.Key));
        }

        return path;
    }

    public object Part2(Input input)
    {
        var near = (from r in Enumerable.Range(-20, 41)
                    from c in Enumerable.Range(-20, 41)
                    let rc = new Coord2D(r, c)
                    let dist = rc.ManhatanDistance(Coord2D.Zero)
                    where dist is > 0 and <= 20
                    select (rc, dist)).ToArray();

        var s = input.First(i => i.Value is 'S').Key;
        PosPrev start = (s, null);
        Coord2D end = input.First(i => i.Value is 'E').Key;

        var distance = GetDistance(input, start, Cost, Next);

        var path = GetPath(end, distance);

        var cheatDistances = (from p in path
                              from n in near
                              let cheat = p.Key + n.rc
                              where path.ContainsKey(cheat)
                              let dist = Math.Abs(path[cheat] - p.Value) - n.dist
                              where dist >= 100
                              select dist).ToArray();

        return cheatDistances.Length / 2;
    }
}