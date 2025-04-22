using System.Text.RegularExpressions;
using Tools;
using static Tools.Neighbourhoods;

namespace Days.Y2022.Day15;

using Input = ValueTuple<Coord2D, Coord2D>;


file class Solution : ISolution<Input[]>
{
    public async Task<Input[]> LoadInput()
    {
        var regex = new Regex(@"Sensor at x=(?<Item1>-?\d+), y=(?<Item2>-?\d+): closest beacon is at x=(?<Item3>-?\d+), y=(?<Item4>-?\d+)");

        var sensors = from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                      let coord = regex.Match<(int, int, int, int)>(l)
                      select (new Coord2D(coord.Item2, coord.Item1), new Coord2D(coord.Item4, coord.Item3));

        return sensors.ToArray();
    }

    public object Part1(Input[] input)
    {
        var rowToCheck = 2_000_000;
        var columnRanges = new List<CoordRange>();
        foreach (var (sensor, beacon) in input)
        {
            var radius = Distance(sensor, beacon);

            var range = new CoordRange { From = sensor.r - radius, To = sensor.r + radius };

            if (range.Contains(rowToCheck))
            {
                var rDist = Math.Abs(sensor.r - rowToCheck);
                var cDist = radius - rDist;
                var columnRange = new CoordRange() { From = sensor.c - cDist, To = sensor.c + cDist };
                AddToRanges(columnRanges, columnRange);
            }
        }

        var beaconsInRow = input.Where(i => i.Item2.r == rowToCheck).Select(i => i.Item2).Distinct().Count();


        return columnRanges.Sum(r => r.To - r.From + 1) - beaconsInRow;
    }

    public object Part2(Input[] input)
    {
        var rowToCheck = 4_000_000L;

        var allGaps = new List<(long, long)>();
        for (long i = 0; i < rowToCheck; i++)
        {
            var columnRanges = new List<CoordRange>();

            foreach (var (sensor, beacon) in input)
            {
                var radius = Distance(sensor, beacon);

                var range = new CoordRange { From = sensor.r - radius, To = sensor.r + radius };

                if (range.Contains(i))
                {
                    var rDist = Math.Abs(sensor.r - i);
                    var cDist = radius - rDist;
                    var columnRange = new CoordRange() { From = sensor.c - cDist, To = sensor.c + cDist };
                    AddToRanges(columnRanges, columnRange);
                }
            }
            columnRanges = columnRanges.OrderBy(r => r.To).ToList();
            var gaps = columnRanges.Zip(columnRanges.Skip(1)).Where(p => p.First.To + 1 != p.Second.From).Select(p => (p.First.To + 1, i)).ToArray();
            allGaps.AddRange(gaps);
        }

        return allGaps[0].Item1 * rowToCheck + allGaps[0].Item2;
    }

    private long Distance(Coord2D a, Coord2D b)
    {
        return Math.Abs(a.c - b.c) + Math.Abs(a.r - b.r);
    }

    private static void AddToRanges(List<CoordRange> columnRanges, CoordRange columnRange)
    {
        var toAdd = true;
        var toDelete = new List<CoordRange>();
        foreach (var existing in columnRanges)
        {
            if (existing.Contains(columnRange))
            {
                toAdd = false;
                break;
            }

            if (columnRange.Contains(existing))
            {
                toDelete.Add(existing);
                continue;
            }

            if (columnRange.Overlaps(existing))
            {
                existing.Without(columnRange);
            }
        }
        if (toAdd)
            columnRanges.Add(columnRange);

        columnRanges.RemoveAll(toDelete.Contains);
    }
}