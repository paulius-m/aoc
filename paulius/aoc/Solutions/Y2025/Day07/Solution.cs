using Tools;
using static Tools.Neighbourhoods;
using Input = Tools.Grid<char>;

namespace Days.Y2025.Day07;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var lines = await File.ReadAllLinesAsync(this.GetInputFile("input"));

        var map = new Grid<char>(lines
            .SelectMany((line, y) => line.Select((c, x) => KeyValuePair.Create(new Coord2D(y, x), c)))
            );

        return map;
    }

    public object Part1(Input input)
    {
        var start = input.First(k => k.Value is 'S').Key;
        var end = input.Max(k => k.Key.r);

        var beams = new[]
        {
            start
        };

        var splits = 0;
        while (beams[0].r < end)
        {
            var newBeams = (from b in beams
                    from nb in CheckBeamSplit(input, b + S)
                    select nb).ToArray();

            splits += newBeams.Length - beams.Length;
            beams = newBeams.Distinct().ToArray();

        }
        return splits;
    }

    public object Part2(Input input)
    {
        var start = input.First(k => k.Value is 'S').Key;
        var end = input.Max(k => k.Key.r);

        var beams = new[]
        {
            (Beam: start, Count: 1L)
        };
        while (beams[0].Beam.r < end)
        {
            var newBeams = (from b in beams
                            from nb in CheckBeamSplit(input, b.Beam + S)
                            select (Beam: nb, b.Count) into rawB
                            group rawB.Count by rawB.Beam into g
                            select (g.Key, g.Sum())
                            ).ToArray();

            beams = newBeams;

        }
        return beams.Sum(b => b.Count);
    }

    private static Coord2D[] CheckBeamSplit(Input input, Coord2D nb)
    {
        return input.TryGetValue(nb, out var c) switch
        {
            true when c is '^' => [nb + W, nb + E],
            _ => [nb]
        };
    }
}