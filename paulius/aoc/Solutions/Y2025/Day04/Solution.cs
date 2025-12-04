using Tools;
using static Tools.Neighbourhoods;

namespace Days.Y2025.Day04;
using Input = System.Collections.Generic.HashSet<Coord2D>;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var lines = await File.ReadAllLinesAsync(this.GetInputFile("input"));

        var map = lines
            .SelectMany((line, y) => line.Select((c, x) => (new Coord2D(y, x), c)))
            .Where(c => c.c is '@')
            .Select(c => c.Item1).ToHashSet();

        return map;
    }

    public object Part1(Input map)
    {
        return (from c in map
                let rolls = from nn in GetNear8(c)
                            where map.Contains(nn)
                            select nn
                where rolls.Count() < 4
                select c).Count();

    }

    public object Part2(Input map)
    {
        var removed = 0L;
        var totalRemoved = 0L;

        do
        {
            var toRemove = (from c in map
                            let rolls = from nn in GetNear8(c)
                                        where map.Contains(nn)
                                        select nn
                            where rolls.Count() < 4
                            select c).ToArray();

            totalRemoved += removed = toRemove.LongLength;

            map.ExceptWith(toRemove);
        } while (removed > 0);

        return totalRemoved;
    }
}