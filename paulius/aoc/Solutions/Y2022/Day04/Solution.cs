using System.Text.RegularExpressions;
using Tools;

namespace Days.Y2022.Day04;

file class Solution : ISolution<CoordRange[][]>
{
    public async Task<CoordRange[][]> LoadInput()
    {
        var regex = new Regex(@"(?<from>\d+)-(?<to>\d+)");
        return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
               .Select(l => l.Split(',').Select(regex.Match<CoordRange>).ToArray())
               .ToArray();
    }

    public object Part1(CoordRange[][] i)
    {
        return i
            .Where(r => r.Zip(r.Skip(1)).All(p => p.First.Contains(p.Second) || p.Second.Contains(p.First)))
            .Count();
    }

    public object Part2(CoordRange[][] i)
    {
        return i
            .Where(r => r.Zip(r.Skip(1)).All(p => p.First.Overlaps(p.Second)))
            .Count();
    }
}
