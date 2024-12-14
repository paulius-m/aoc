using System.Text.RegularExpressions;
using Tools;
using static Tools.Neighbourhoods;

namespace Days.Y2024.Day14;
using Input = Robot[];

file class Solution : ISolution<Input>
{

    Coord2D room = new Coord2D(103, 101);
    // Coord2D room = new Coord2D(7, 11);
    public async Task<Input> LoadInput()
    {
        var r = new Regex(@"p=(?<Item1>-?\d+),(?<Item2>-?\d+) v=(?<Item3>-?\d+),(?<Item4>-?\d+)");

        return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                let m = r.Match<(long px, long py, long vx, long vy)>(l)
                select new Robot(new(m.py, m.px), new Coord2D(m.vy, m.vx))).ToArray();
    }

    public object Part1(Input input)
    {
        
        var half = new Coord2D(room.r / 2 , room.c / 2 );

        var m = (from r in input
                let moved = EnumerableEx.Gen(r.P, p => Pin(p + r.V, room.r, room.c)).Take(101).Last()
                select moved).ToArray();

        var g = new Grid<char>(m.GroupBy(r => r).Select(g => KeyValuePair.Create(g.Key, (char)(g.Count() + '0'))));

        Console.WriteLine(g.ToVoidString('.', 0, room.c - 1, 0, room.r - 1));

        var fit = from moved in m
                  where moved.r != half.r && moved.c != half.c
                  select moved;

        return (from moved in fit
                group moved by (moved.r * 2 / room.r, moved.c * 2 / room.c) into quadrant
                select quadrant.Count()).Product(f => f);
    }

    static Coord2D Pin(Coord2D n, long maxR, long maxC)
    {
        return new Coord2D(Mod(n.r, maxR), Mod(n.c, maxC));

        long Mod(long x, long m)
        {
            return (x % m + m) % m;
        }
    }

    public object Part2(Input input)
    {
        var corner = new Coord2D(room.r - 1, 0);
        var i = 0;
        while(true)
        {
            i++;
            input = (from r in input
                    select new Robot(Pin(r.P + r.V, room.r, room.c), r.V)).ToArray();

            var g = new Grid<char>(input.GroupBy(r => r.P).Select(g => KeyValuePair.Create(g.Key, (char)(g.Count() + '0'))));

            if (g.Where(k => GetNear8(k.Key).Where(g.ContainsKey).Count() == 5 ).Count() > 10)
            {
                Console.WriteLine(g.ToVoidString('.', 0, room.c - 1, 0, room.r - 1));
                return i;
            }

        }

        return 0;
    }
}

file record Robot(Coord2D P, Coord2D V);