using Tools;
using static Tools.Neighbourhoods;

namespace Days.Y2021.Day09;

public class Solution : ISolution<Dictionary<Coord2D, int>>
{
    public async Task<Dictionary<Coord2D, int>> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .SelectMany((l, r) => l.Select((i, c) => (new Coord2D(r, c), i - '0'))).ToDictionary(kv => kv.Item1, kv => kv.Item2);
    }

    public object Part1(Dictionary<Coord2D, int> heights)
    {
        IEnumerable<KeyValuePair<Coord2D, int>> lowPoints = GetLowPoints(heights);

        return lowPoints.Sum(l => l.Value + 1);
    }

    private static IEnumerable<KeyValuePair<Coord2D, int>> GetLowPoints(Dictionary<Coord2D, int> heights)
    {
        return from h in heights
               let nh = GetNeibhours(h.Key, heights)
               where nh.All(nhh => nhh.height > h.Value)
               select h;
    }

    public object Part2(Dictionary<Coord2D, int> heights)
    {
        IEnumerable<KeyValuePair<Coord2D, int>> lowPoints = GetLowPoints(heights);

        var basins = from lp in lowPoints
                     select Fill(lp, heights).Count();

        return basins.OrderByDescending(Operators.Identity).Take(3).Aggregate(Operators.Multiply);
    }

    private static IEnumerable<(Coord2D, int)> Fill(KeyValuePair<Coord2D, int> lp, Dictionary<Coord2D, int> map)
    {
        var basin = new HashSet<(Coord2D, int h)>();
        basin.Add((lp.Key, lp.Value));
        var toVisit = new Queue<Coord2D>();
        toVisit.Enqueue(lp.Key);

        while (toVisit.Count > 0)
        {
            var h = toVisit.Dequeue();

            foreach (var basinCandidate in GetNeibhours(h, map).Where(n => n.height < 9 && !basin.Contains(n)))
            {
                toVisit.Enqueue(basinCandidate.Item1);
                basin.Add(basinCandidate);
            }
        }

        return basin;
    }

    private static IEnumerable<(Coord2D, int height)> GetNeibhours(Coord2D h, Dictionary<Coord2D, int> map) =>
            from n in GetNear4(h)
            where map.ContainsKey(n)
            select (n, map[n]);
}