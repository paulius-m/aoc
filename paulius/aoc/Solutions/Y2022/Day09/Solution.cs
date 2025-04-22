using Tools;
using static Tools.Neighbourhoods;

namespace Days.Y2022.Day09;

file record Input(Coord2D Direction, int Length);

file class Solution : ISolution<Input[]>
{
    public async Task<Input[]> LoadInput()
    {
        return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                select l.Split(' ') switch
                {
                ["R", var length] => new Input(new Coord2D(0, 1), int.Parse(length)),
                ["L", var length] => new Input(new Coord2D(0, -1), int.Parse(length)),
                ["U", var length] => new Input(new Coord2D(1, 0), int.Parse(length)),
                ["D", var length] => new Input(new Coord2D(-1, 0), int.Parse(length)),
                }).ToArray();
    }

    public object Part1(Input[] input)
    {
        var head = new Coord2D(0, 0);
        var tail = new Coord2D(0, 0);
        var visited = new HashSet<Coord2D> { tail };

        foreach (var (dir, length) in input)
        {
            for (int i = 0; i < length; i++)
            {
                var temp = head;
                head = head + dir;
                if (Distance(head, tail) > 1)
                {
                    tail = temp;
                    visited.Add(tail);
                }
            }
        }

        return visited.Count;
    }

    public object Part2(Input[] input)
    {
        var rope = Enumerable.Repeat(Coord2D.Zero, 10).ToArray();

        var visited = new HashSet<Coord2D> { rope[^1] };

        foreach (var (dir, length) in input)
        {
            for (int i = 0; i < length; i++)
            {
                rope[0] = rope[0] + dir;
                for (int ri = 1; ri < rope.Length; ri++)
                {
                    var diff = rope[ri - 1] - rope[ri];
                    if (Length(diff) > 1)
                    {
                        rope[ri] = rope[ri] + Normalized(diff);
                    }
                }
                visited.Add(rope[^1]);
            }
        }

        return visited.Count;
    }

    private long Distance(Coord2D a, Coord2D b)
    {
        return Math.Max(Math.Abs(a.c - b.c), Math.Abs(a.r - b.r));
    }

    private long Length(Coord2D a)
    {
        return Distance(a, Coord2D.Zero);
    }

    private Coord2D Normalized(Coord2D a)
    {
        return new Coord2D(Math.Sign(a.r), Math.Sign(a.c));
    }
}