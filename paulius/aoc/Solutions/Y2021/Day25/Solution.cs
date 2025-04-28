using Tools;
using Pastel;
using static Days.Y2021.Day25.Solution;
using Coord = System.ValueTuple<int, int>;
using System.Drawing;

namespace Days.Y2021.Day25;

public class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var lines = await File.ReadAllLinesAsync(this.GetInputFile("input"));

        var cucumbers = lines
            .SelectMany((l, r) => l.Select((i, c) => ((r, c), i)))
            .Where(c => c.i is not '.').ToArray();

        return new Input(
            lines[0].Length,
            lines.Length,
            cucumbers.Where(c => c.i is '>').Select(c => c.Item1).ToHashSet(),
            cucumbers.Where(c => c.i is 'v').Select(c => c.Item1).ToHashSet()
            );
    }

    public object Part1(Input map)
    {
        var (width, height, east, south) = map;

        for (int i = 0; true; i++)
        {
            //Console.CursorTop = 0;
            //Console.WriteLine(Render(width , height, east, south));

            var prev = east;
            east = east.Select(e => (e.r, (e.c + 1) % width) switch
            {
                var newC when !(east.Contains(newC) || south.Contains(newC)) => newC,
                _ => e
            }).ToHashSet();

            var moved = !east.SetEquals(prev);

            prev = south;
            south = south.Select(e => ((e.r + 1) % height, e.c) switch
            {
                var newC when !(east.Contains(newC) || south.Contains(newC)) => newC,
                _ => e
            }).ToHashSet();

            moved = moved || !south.SetEquals(prev);

            if (!moved)
            {
                return i + 1;
            }
        }
    }

    public object Part2(Input map)
    {
        return 0;
    }


    private string Render(int width, int height, HashSet<Coord> east, HashSet<Coord> south)
    {
        return string.Join('\n', Enumerable.Range(0, height).Select(r =>
            string.Join("", Enumerable.Range(0, width).Select(c => (r, c) switch
            {
                var e when east.Contains(e) => ">".Pastel(Color.DarkGreen),
                var e when south.Contains(e) => "v".Pastel(Color.DarkOliveGreen),
                _ => ".".Pastel(Color.LightBlue)
            }).ToArray()
            )).ToArray());
    }

    public record Input(int width, int height, HashSet<(int r, int c)> east, HashSet<(int r, int c)> south);
}