using Sprache;
using System.Linq;
using Tools;
using static Tools.Neighbourhoods;

namespace Days.Y2024.Day18;

using static Tools.Neighbourhoods;
using Input = Coord2D[];

file class Solution : ISolution<Input>
{
    //readonly string file = "test";
    //Coord2D maxCoord = new Coord2D(6, 6);
    //int firstBytes = 12;

    readonly string file = "input";
    Coord2D maxCoord = new Coord2D(70, 70);
    int firstBytes = 1024;

    public async Task<Input> LoadInput()
    {
        return (from l in await File.ReadAllLinesAsync(this.GetInputFile(file))
                         let r = l.Split(',').SelectArray(long.Parse)
                         select new Coord2D(r[1], r[0])).ToArray();
    }

    public object Part1(Input input)
    {
        var map = new Grid<char>(input.Take(firstBytes).Select(i => KeyValuePair.Create(i, '#')));

        Console.WriteLine(map.ToVoidString('.', 0, maxCoord.c, 0, maxCoord.r));
        var distance = GetDistance(map, Coord2D.Zero, Cost, Next);

        return distance[maxCoord];
    }

    private IEnumerable<Coord2D> Next(Grid<char> map, Coord2D f)
    {
        return from n in GetNear4(f)
               where 0 <= n.c && n.c <= maxCoord.c 
               && 0 <= n.r && n.r <= maxCoord.r
               && !map.ContainsKey(n)
               select n;
    }

    private long? Cost(Grid<char> map, Coord2D from, Coord2D to)
    {
        return from.ManhatanDistance(to);
    }

    public object Part2(Input input)
    {
        var map = new Grid<char>(input.Take(firstBytes).Select(i => KeyValuePair.Create(i, '#')));

        for (var i = firstBytes + 1; i < input.Length; i++)
        {
            var fall = input[i];
            map.Add(fall, '#');
            var distance = GetDistance(map, Coord2D.Zero, Cost, Next);
            if (!distance.ContainsKey(maxCoord))
            {
                return $"{fall.c},{fall.r}";
            }
        }

        return null;
    }
}