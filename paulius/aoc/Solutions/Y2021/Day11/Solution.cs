using Pastel;
using Tools;
using static Tools.Neighbourhoods;

namespace Days.Y2021.Day11;
using Map = System.Collections.Generic.Dictionary<Coord2D, char>;
using MapPair = System.Collections.Generic.KeyValuePair<Coord2D, char>;
using MapSet = System.Collections.Generic.HashSet<Coord2D>;


public class Solution : ISolution<Map>
{
    public async Task<Map> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .SelectMany((l, r) => l.Select((i, c) => (new Coord2D(r, c), i))).ToDictionary(kv => kv.Item1, kv => kv.Item2);
    }

    public object Part1(Map map)
    {
        var flashCount = 0;
        for (int i = 0; i < 100; i++)
        {
            map = MakeStep(map);

            flashCount += map.Count(m => m.Value is '0');
        }
        return flashCount;
    }

    public object Part2(Map map)
    {
        for (int i = 0; true; i++)
        {
            if (map.All(m => m.Value is '0'))
            {
                return i;
            }

            map = MakeStep(map);
        }
    }

    private static Map MakeStep(Map map)
    {
        map = new(map.Select(Inc));

        var flashing = new MapSet();
        var previousFlashed = new MapSet();

        do
        {
            flashing = new(map.Where(m => m.Value > '9').Select(m => m.Key).Except(previousFlashed));
            if (flashing.Count is 0)
            {
                break;
            }

            map = new(map.Select(m => Add(m, GetNeibhours(m.Key, map).Count(flashing.Contains))));
            previousFlashed.UnionWith(flashing);

        } while (true);

        map = new(map.Select(Fade));
        Console.WriteLine(ToString(map, previousFlashed));
        Console.WriteLine();
        return map;

        static MapPair Fade(MapPair pair) => new(pair.Key, pair.Value > '9' ? '0' : pair.Value);
        static MapPair Inc(MapPair pair) => Add(pair, 1);
        static MapPair Add(MapPair pair, int a) => new(pair.Key, (char)(pair.Value + a));
    }

    private static string ToString(Map map, MapSet? flash = null)
    {
        string ToFlashNumber(MapPair p)
        {
            var s = p.Value.ToString();
            return flash?.Contains(p.Key) == true ? s.Pastel("#FFFF00") : s;
        }

        var rows = map
            .GroupBy(m => m.Key.r)
            .OrderBy(g => g.Key)
            .Select(g => string.Join("", g.OrderBy(c => c.Key.c).Select(ToFlashNumber)));

        return string.Join("\n", rows);
    }

    private static IEnumerable<Coord2D> GetNeibhours(Coord2D h, Map map) =>
    from n in GetNear8(h)
    where map.ContainsKey(n)
    select n;
}