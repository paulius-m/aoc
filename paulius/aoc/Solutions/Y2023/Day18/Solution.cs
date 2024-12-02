using System.Text.RegularExpressions;
using Pastel;
using Tools;
using static Tools.Neighbourhoods;
using Sprache;


namespace Days.Y2023.Day18;
using Input = (char, int, string)[];

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var r = new Regex(@"(?<Item1>-?\w) (?<Item2>-?\d+) \(#(?<Item3>-?\w+)\)");
        return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                select r.Match<(char, int, string)>(l)).ToArray();
    }

    public object Part1(Input input)
    {
        var grid = new Grid<string>();
        var head = new Coord2D(0, 0);

        foreach (var (d, distance, color) in input)
        {
            var dir = d switch
            {
                'U' => N,
                'D' => S,
                'R' => E,
                'L' => W
            };

            for (int i = 0; i < distance; i++)
            {
                grid.Add(head, "#".Pastel(color));
                head += dir;
            }
        }

        FillIn(grid);

        return grid.Keys.Count;
    }

    public Grid<string> FillIn(Grid<string> grid)
    {
        var minR = grid.Keys.Min(k => k.r);
        var maxR = grid.Keys.Max(k => k.r);
        var minC = grid.Keys.Min(k => k.c);

        Coord2D? start = null;
        var edgeCount = 0;

        for (var i = minR; i <= maxR; i++)
        {
            var at = new Coord2D(i, minC + 1);
            if (grid.ContainsKey(at))
            {
                edgeCount++;
            }
            else if (edgeCount == 1)
            {
                start = at;
                break;
            }
        }

        var q = new Queue<Coord2D>();
        q.Enqueue(start);
        var enqueued = new HashSet<Coord2D>
        {
            start
        };

        while (q.Count > 0)
        {
            var current = q.Dequeue();
            grid[current] = "#";

            foreach (var pos in from n in GetNear4(current)
                                where !grid.ContainsKey(n) && !enqueued.Contains(n)
                                select n)
            {
                q.Enqueue(pos);
                enqueued.Add(pos);
            }
        }

        return grid;
    }

    public object Part2(Input input)
    {
        var polygon = new List<(Coord2D, Coord2D, long)>();
        var head = new Coord2D(0, 0);

        foreach (var (_, _, color) in input)
        {
            var distance = Convert.ToInt64(color[0..^1], 16);
            var d = color[^1];

            var dir = d switch
            {
                '3' => N,
                '1' => S,
                '0' => E,
                '2' => W
            };

            var next = head + dir * distance;

            polygon.Add((head, next, distance));
            head = next;
        }

        return Area(polygon);
    }

    private long Area(List<(Coord2D, Coord2D, long)> polygon)
    {
        var perimeter = polygon.Aggregate(0L, (a, b) => a + b.Item3);
        var area = polygon.Aggregate(0L, (a, b) => a + b.Item1.c * b.Item2.r - b.Item2.c * b.Item1.r) / 2;
        return area + 1 - perimeter / 2 + perimeter;
    }
}