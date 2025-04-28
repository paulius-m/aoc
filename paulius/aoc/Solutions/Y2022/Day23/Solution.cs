using Tools;
using static Tools.Neighbourhoods;

namespace Days.Y2022.Day23;
using Input = HashSet<Coord2D>;

file class Solution : ISolution<Input>
{

    (Coord2D[], Coord2D)[] Check8 = [
        (Near8, Coord2D.Zero)
    ];

    (Coord2D[], Coord2D)[] Checks3 = [
        ([N, NE, NW], N),
        ([S, SE, SW], S),
        ([W, NW, SW], W),
        ([E, NE, SE], E)
        ];

    (Coord2D[], Coord2D)[] NoMove = [
            ([], Coord2D.Zero)
    ];

    public async Task<Input> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .SelectMany((r, ri) => r.Select((c, ci) => KeyValuePair.Create(new Coord2D(ri, ci), c)))
            .Where(kv => kv.Value is '#').Select(kv => kv.Key).ToHashSet();
    }

    public object Part1(Input elfs)
    {
        for (int i = 0; i < 10; i++)
        {
            elfs = Move(elfs, i);
        }
        var w = elfs.Max(e => e.c) - elfs.Min(e => e.c) + 1;
        var h = elfs.Max(e => e.r) - elfs.Min(e => e.r) + 1;

        return w * h - elfs.Count;
    }

    public object Part2(Input elfs)
    {
        for (int i = 0; true; i++)
        {
            var moved = Move(elfs, i);

            if (moved.SetEquals(elfs))
            {
                return i + 1;
            }
            elfs = moved;
        }
    }

    private Input Move(Input elfs, int i)
    {
        var checks = Check8
            .Concat(Enumerable.Range(i, Checks3.Length).Select(i => Checks3[i % Checks3.Length]))
            .Concat(NoMove);

        elfs = (from e in elfs
                let direction = from c in checks
                                where GetNear(e, c.Item1).All(cc => !elfs.Contains(cc))
                                select c.Item2
                select (e, next: e + direction.First()) into chances
                group chances by chances.next into collisions
                let stay = from c in collisions
                               select c.e
                let move = from c in collisions
                               select c.next
                from cr in collisions.Count() == 1 ? move : stay
                select cr).ToHashSet();
        return elfs;
    }
}