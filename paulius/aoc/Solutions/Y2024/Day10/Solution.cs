using Sprache;
using Tools;
using static Tools.Neighbourhoods;
using Input = Tools.Grid<int>;

namespace Days.Y2024.Day10;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return new Input((await File.ReadAllLinesAsync(this.GetInputFile("input"))).SelectMany((r, ri) => r.Select((c, ci) => Input.Coord(ri, ci, (c - '0')))));
    }

    public object Part1(Input input)
    {
        var scores = new Dictionary<Coord2D, HashSet<Coord2D>>();

        foreach (var k in input.Where(kv => kv.Value == 9))
        {
            scores[k.Key] = new HashSet<Coord2D>([k.Key]);
        }

        for (var i = 8; i >= 0; i--)
        {
            foreach (var k in input.Where(kv => kv.Value == i))
            {
                scores[k.Key] = (from n in GetNear4(k.Key)
                                 where input.ContainsKey(n) && input[n] == i + 1
                                 from s in scores[n]
                                 select s).ToHashSet();
            }
        }

        return input.Where(kv => kv.Value == 0).Sum(k => scores[k.Key].Count);
    }

    public object Part2(Input input)
    {
        var scores = new Dictionary<Coord2D, long>();

        foreach (var k in input.Where(kv => kv.Value == 9))
        {
            scores[k.Key] = 1;
        }

        for (var i = 8; i >= 0; i--)
        {
            foreach (var k in input.Where(kv => kv.Value == i))
            {
                scores[k.Key] = (from n in GetNear4(k.Key)
                                 where input.ContainsKey(n) && input[n] == i + 1
                                 select scores[n]).Sum();
            }
        }

        return input.Where(kv => kv.Value == 0).Sum(k => scores[k.Key]);
    }
}