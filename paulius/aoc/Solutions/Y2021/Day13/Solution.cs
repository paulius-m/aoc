using Tools;
using System.Text.RegularExpressions;
using static MoreLinq.Extensions.SplitExtension;
using static System.Linq.Enumerable;
using Coords = System.ValueTuple<int, int>;
using Fold = System.ValueTuple<string, int>;
using Input = System.ValueTuple<(int x, int y)[], (string f, int c)[]>;

namespace Days.Y2021.Day13;

public class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var split = (await File.ReadAllLinesAsync(this.GetInputFile("input"))).Split(string.Empty);
        var coords = split.First()
            .Select(new Regex(@"(?<item1>\d+),(?<item2>\d+)").Match<(int, int)>)
            .ToArray();

        var instructions = split.Last()
            .Select(new Regex(@"fold along (?<item1>\w+)=(?<item2>\d+)").Match<(string, int)>)
            .ToArray();

        return (coords, instructions);
    }

    public object Part1(Input input)
    {
        var (coords, instructions) = input;

        return Fold(coords, instructions.First()).Length;
    }

    private static Coords[] Fold(Coords[] c, Fold fold)
    {
        int TryFold(int c, int f) => f < c ? 2 * f - c : c;

        Coords Transform(Coords coords) => fold switch
        {
            ("x", var x) => (TryFold(coords.Item1, x), coords.Item2),
            ("y", var y) => (coords.Item1, TryFold(coords.Item2, y))
        };

        return c.Select(Transform).Distinct().ToArray();
    }

    private static string Render(Coords[] coords)
    {
        var minX = coords.Min(cc => cc.Item1);
        var maxX = coords.Max(cc => cc.Item1);
        var minY = coords.Min(cc => cc.Item2);
        var maxY = coords.Max(cc => cc.Item2);

        var translated = coords.Select(c => (c.Item1 - minX, c.Item2 - minY)).ToHashSet();

        return string.Join('\n', Range(0, maxY - minY + 1).Select(y =>
            new string(Range(0, maxX - minX + 1).Select(x => translated.Contains((x, y)) ? 'O' : ' ').ToArray())
            ));
    }

    public object Part2(Input input)
    {
        var (coords, instructions) = input;

        Console.WriteLine(Render(instructions.Aggregate(coords, Fold)));

        return 0;
    }
}