using System.Diagnostics.Metrics;
using System.Linq;
using System.Text.RegularExpressions;
using Tools;

using static MoreLinq.Extensions.SplitExtension;
using Input = System.ValueTuple<string, System.Collections.Generic.Dictionary<string, char>>;

namespace Days.Y2021.Day14;

public class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var split = (await File.ReadAllLinesAsync(this.GetInputFile("input"))).Split(string.Empty);
        var polymer = split.First().Single();
        var rules = split.Last()
            .Select(new Regex(@"(?<item1>\w+) -> (?<item2>\w+)").Match<(string, char)>)
            .ToArray();


        return (polymer, rules.ToDictionary(kv => kv.Item1, kv => kv.Item2));
    }

    public object Part1(Input input)
    {
        var (polymer, r) = input;

        for (var i = 0; i < 10; i++)
        {
            var pairs = polymer.Prepend('#').Zip(polymer.Append('#'), (a, b) => new string(new[] { a, b }));

            polymer = string.Join("", pairs.Select(p => p switch
            {
                var s when r.ContainsKey(s) => new string(new[] { s[0], r[s] }),
                var s => s.Substring(0, 1)
            }))[1..];
        }

        var stats = polymer.GroupBy(Operators.Identity).Select(g => g.Count()).OrderByDescending(Operators.Identity).ToArray();

        return stats.First() - stats.Last();
    }

    public object Part2(Input input)
    {
        var (polymer, r) = input;

        var pairs = polymer.Prepend('#').Zip(polymer.Append('#'), (a, b) => new string(new[] { a, b }))
            .GroupBy(Operators.Identity).Select(g => (g.Key, Count: g.LongCount())).ToArray();

        (string Key, long Count)[] SplitPair((string, long) p, char atom)
        {
            return new[]
            {
                (new string(new[] { p.Item1[0], atom }), p.Item2 ),
                (new string(new[] { atom, p.Item1[1] }), p.Item2 )
            };
        }

        for (var i = 0; i < 40; i++)
        {
            pairs = pairs.SelectMany(p => r.ContainsKey(p.Key) && r[p.Key] is char i ? SplitPair(p, i) : new[] { p })
                 .GroupBy(p => p.Key)
                 .Select(g => (g.Key, Count: g.Sum(p => p.Count))).ToArray();
        }

        var unziped = pairs.Select(p => (Key: p.Key[0], p.Count))
            .Where(p => p.Key is not '#')
            .GroupBy(p => p.Key)
            .Select(g => (g.Key, Count: g.Sum(p => p.Count))).ToArray();

        var stats = unziped.Select(u => u.Count).OrderByDescending(Operators.Identity).ToArray();

        return stats.First() - stats.Last();
    }
}