using MoreLinq;
using Sprache;
using System.Collections.Concurrent;
using Tools;

namespace Days.Y2025.Day05;

using Input = (CoordRange[], long[]);

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return Parsers.Input.Parse(await File.ReadAllTextAsync(this.GetInputFile("input")));
    }

    public object Part1(Input input)
    {
        var (ranges, ids) = input;

        return ranges.Cartesian(ids, (r, id) => (id, Fresh: r.Contains(id)))
            .Where(i => i.Fresh)
            .Distinct()
            .Count();
    }

    public object Part2(Input input)
    {
        var (ranges, _) = input;

        var minimal = new List<CoordRange>();

        for (var i1 = 0; i1 < ranges.Length; i1++)
        {
            var partitioned = new[] { ranges[i1] };

            for (var i2 = i1 + 1; i2 < ranges.Length; i2++)
            {
                var r2 = ranges[i2];
                partitioned = (from r in partitioned
                               from p in r.Partition(r2)
                               where !r2.Contains(p)
                               select p).ToArray();
            }
            minimal.AddRange(partitioned);
        }

        return (from r in minimal
                select r.To - r.From + 1).Sum();
    }
}

file class Parsers
{
    private static Parser<CoordRange> Range = from f in Parse.Number.Select(long.Parse)
                                              from _ in Parse.Char('-')
                                              from t in Parse.Number.Select(long.Parse)
                                              select new CoordRange(f, t);

    public static Parser<CoordRange[]> Ranges = from r in Range.DelimitedBy(Parse.LineTerminator)
                                                select r.ToArray();

    public static Parser<Input> Input = from r in Ranges
                                        from _ in Parse.LineTerminator.Repeat(2)
                                        from ids in Parse.Number.Select(long.Parse).DelimitedBy(Parse.LineTerminator)
                                        select (r, ids.ToArray());
}