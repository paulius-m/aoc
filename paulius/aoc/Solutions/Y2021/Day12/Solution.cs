using System.Linq;
using System.Text.RegularExpressions;
using Tools;
using Grapth = System.Collections.Generic.Dictionary<string, string[]>;

namespace Days.Y2021.Day12;

public class Solution : ISolution<Grapth>
{
    public async Task<Grapth> LoadInput()
    {
        var r = new Regex(@"(?<item1>\w+)-(?<item2>\w+)");

        return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .Select(r.Match<(string, string)>).SelectMany(p => new[] { p, (p.Item2, p.Item1) })
            .GroupBy(p => p.Item1)
            .ToDictionary(g => g.Key, g => g.Select(p => p.Item2).ToArray());
    }

    public object Part1(Grapth caves)
    {
        return GetPaths(caves).Count;
    }

    public object Part2(Grapth caves)
    {
        return caves.Keys.Where(k => k.ToLower() == k).SelectMany(k => GetPaths(caves, k)).Distinct().Count();
    }

    private static HashSet<string> GetPaths(Grapth caves, string? countTwo = null)
    {
        Queue<(string, string[])> toVisit = new();
        toVisit.Enqueue(("start", new[] { "start" }));
        HashSet<string> paths = new();

        while (toVisit.Count > 0)
        {
            var (visit, path) = toVisit.Dequeue();

            foreach (var next in caves[visit])
            {
                var newpath = path.Append(next).ToArray();

                if (next is "start")
                {
                    continue;
                }
                if (next is "end")
                {
                    var s = string.Join(',', newpath);
                    paths.Add(s);
                    continue;
                }
                if (next.ToLower() == next && path.Count(p => p == next) == (next == countTwo ? 2 : 1))
                {
                    continue;
                }

                toVisit.Enqueue((next, newpath));
            }
        }

        return paths;
    }
}