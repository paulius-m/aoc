using Tools;

namespace Days.Y2022.Day14;

using static Tools.Neighbourhoods;
using Grid = Grid<char>;

file class Solution : ISolution<Grid>
{
    public async Task<Grid> LoadInput()
    {
        var input = await File.ReadAllLinesAsync(this.GetInputFile("input"));

        var scans = from l in input
                    let coords = l.Split(" -> ").Select(c => c.Split(",") switch { [var x, var y] => new Coord2D(int.Parse(y), int.Parse(x)) }).ToArray()
                    select coords;

        var map = new Grid();

        foreach (var line in scans)
        {
            foreach (var (from, to) in line.Zip(line.Skip(1)))
            {
                var v = to - from;
                var normal = Normalized(v);
                for (int i = 0; i <= Length(v); i++)
                {
                    map[from + normal * i] = '#';
                }
            }
        }

        return map;
    }

    Coord2D Source = new(0, 500);

    Coord2D[] FallPath = new Coord2D[]
    {
            new (1, 0),
            new (1, -1),
            new (1, 1),
    };

    private long Distance(Coord2D a, Coord2D b)
    {
        return Math.Abs(a.c - b.c) + Math.Abs(a.r - b.r);
    }

    private long Length(Coord2D a)
    {
        return Distance(a, Coord2D.Zero);
    }

    private Coord2D Normalized(Coord2D a)
    {
        return new Coord2D(Math.Sign(a.r), Math.Sign(a.c));
    }

    public object Part1(Grid map)
    {
        var maxR = map.Keys.Max(k => k.r);

        var inBounds = true;
        while (inBounds)
        {
            var current = Source;

            while (true)
            {
                var next = FallPath.Select(p => current + p).FirstOrDefault(p => !map.ContainsKey(p));

                if (next is null)
                {
                    map[current] = 'o';
                    break;
                }
                else if (next.r == maxR)
                {
                    inBounds = false;
                    break;
                }
                current = next;
            }
        }

        return map.Values.Count(v => v is 'o');
    }

    public object Part2(Grid map)
    {
        var maxR = map.Keys.Max(k => k.r) + 2;

        var spawnNew = true;
        while (spawnNew)
        {
            var current = Source;

            while (true)
            {
                var next = FallPath.Select(p => current + p).FirstOrDefault(p => !map.ContainsKey(p) && p.r < maxR);

                if (next is null)
                {
                    map[current] = 'o';
                    if (current == Source)
                    {
                        spawnNew = false;
                    }
                    break;
                }
                current = next;
            }
        }
        return map.Values.Count(v => v is 'o');
    }
}