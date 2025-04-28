using static MoreLinq.Extensions.SplitExtension;
using static System.Linq.Enumerable;
using Tools;
using static Tools.Neighbourhoods;

namespace Days.Y2021.Day20;

using Coords = Coord2D;


public class Solution : ISolution<(char[], Dictionary<Coords, char>)>
{
    public async Task<(char[], Dictionary<Coords, char>)> LoadInput()
    {
        var lines = (await File.ReadAllLinesAsync(this.GetInputFile("input"))).Split("");

        return (lines.First().Single().ToCharArray(), lines.Last().SelectMany((l, r) => l.Select((i, c) => (new  Coord2D(r, c), i)))
            .ToDictionary(kv => kv.Item1, kv => kv.Item2));
    }

    public object Part1((char[], Dictionary<Coords, char>) input)
    {
        var (table, image) = input;

        image = Enhance(table, image, 2);

        Console.WriteLine(Render(image));
        Console.WriteLine();
        return image.Count(kv => kv.Value is '#');
    }

    private Dictionary<Coords, char> Enhance(char[] table, Dictionary<Coords, char> image, int times)
    {
        var near9 = Near9.OrderBy(p => p.r).ThenBy(p => p.c).ToArray();

        int infinity = 0;

        for (int i = 0; i < times; i++)
        {
            var toEnhance = (from px in image
                             from n in GetNear(px.Key, Near9)
                             select n).ToHashSet();

            var ehanced = from px in toEnhance
                          let n = from n in GetNear(px, near9) select image.GetValueOrDefault(n, table[infinity])
                          select KeyValuePair.Create(px, table[ToIndex(n)]);

            infinity = ToIndex(Repeat(table[infinity], 9));
            image = new(ehanced);
        }

        return image;
    }

    private int ToIndex(IEnumerable<char> pxs)
    {
        var s = new string(pxs.Select(px => px is '#' ? '1' : '0').ToArray());
        var idx = Convert.ToInt16(s, 2);

        return idx;
    }

    public object Part2((char[], Dictionary<Coords, char>) input)
    {
        var (table, image) = input;

        image = Enhance(table, image, 50);

        Console.WriteLine(Render(image));
        Console.WriteLine();
        return image.Count(kv => kv.Value is '#');
    }

    private static string Render(Dictionary<Coords, char> coords)
    {
        var minX = (int)coords.Keys.Min(cc => cc.c) - 5;
        var maxX = (int)coords.Keys.Max(cc => cc.c) + 5;
        var minY = (int)coords.Keys.Min(cc => cc.r) - 5;
        var maxY = (int)coords.Keys.Max(cc => cc.r) + 5;

        return string.Join('\n', Range(0, (maxY - minY + 1)).Select(y =>
            new string(Range(0, maxX - minX + 1).Select(x => coords.GetValueOrDefault(new Coords(y + minY, x + minX), '.')).ToArray())
            ));
    }
}