using Tools;
using static Tools.Neighbourhoods;

namespace Days.Y2021.Day15;
using static Tools.Neighbourhoods;
using Map = System.Collections.Generic.Dictionary<Coord2D, long>;
using MapQueue = System.Collections.Generic.PriorityQueue<Coord2D, long>;


public class Solution : ISolution<Map>
{
    public async Task<Map> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .SelectMany((l, r) => l.Select((i, c) => (new Coord2D(r, c), i - '0')))
            .ToDictionary(kv => kv.Item1, kv => (long)kv.Item2);
    }

    public object Part1(Map map) => GetRisk(map);

    public object Part2(Map map) => GetRisk(Enlarge(map, 5));

    private static object GetRisk(Map map)
    {
        var risks = new Map { [new (0, 0)] = 0 };
        var q = new MapQueue([(new Coord2D(0, 0), 0L)]);

        while (q.Count > 0)
        {
            var current = q.Dequeue();
            var currentRisk = risks[current];

            foreach (var (pos, risk) in from n in GetNear4(current)
                                        where map.ContainsKey(n)
                                        select (n, currentRisk + map[n]))
            {
                if (risk < risks.GetValueOrDefault(pos, int.MaxValue))
                {
                    q.Enqueue(pos, risk);
                    risks[pos] = risk;
                }
            }
        }

        return risks[new (risks.Max(p => p.Key.r), risks.Max(p => p.Key.c))];
    }

    private Map Enlarge(Map map, int scale)
    {
        var enlarged = new Map();
        var a = map.Max(p => p.Key.r) + 1;
        var b = map.Max(p => p.Key.c) + 1;

        for (var i = 0; i < a * scale; i++)
            for (var j = 0; j < b * scale; j++)
            {
                var risk = map[new (i % a, j % b)] + i / a + j / b;
                enlarged[new (i, j)] = risk > 9 ? risk % 9 : risk;
            }
        return enlarged;
    }
}