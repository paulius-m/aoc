using MoreLinq;
using Tools;
using Input = string[][];

namespace Days.Y2024.Day25;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .Split("")
            .Select(s => s.ToArray())
            .ToArray();
    }

    public object Part1(Input input)
    {
        var g = (from i in input
                 group ToHeights(i) by i[0].All(static c => c is '#') into gg
                select KeyValuePair.Create(gg.Key, gg.ToArray())).ToDictionary();

        var (locks, keys) = (g[true], g[false]);
        return (from l in locks
                from k in keys
                let check = l.Zip(k, (a, b) => a.Height + b.Height <= a.Space).ToArray()
                where check.All(t => t)
                select l).Count();
    }

    private (int Height, int Space)[] ToHeights(string[] i)
    {
        return i.Transpose().Select(c => (c.Where(cc => cc is '#').Count(), c.Count())).ToArray();
    }

    public object Part2(Input i)
    {
        return 0;
    }
}