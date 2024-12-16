using MoreLinq;
using Sprache;
using Tools;
using static Tools.Neighbourhoods;

namespace Days.Y2024.Day16;

using static Tools.Neighbourhoods;
using Input = Grid<char>;
using PosDir = (Coord2D Pos, Coord2D Dir);

using PosDirPrev = (Coord2D Pos, Coord2D Dir, (Coord2D Pos, Coord2D Dir)? Prev);

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return new Input((await File.ReadAllLinesAsync(this.GetInputFile("input"))).SelectMany((r, ri) => r.Select((c, ci) => Input.Coord(ri, ci, c))));
    }

    public object Part1(Input input)
    {
        PosDir start = (input.FirstOrDefault(i => i.Value is 'S').Key, E);
        Coord2D end = input.FirstOrDefault(i => i.Value is 'E').Key;

        return GetDistance(input, start, Cost, Next).Where(d => d.Key.Pos == end ).Min(e => e.Value );

        static long? Cost(Dictionary<Coord2D, char> map, PosDir from, PosDir to)
        {
            var turnCost = from.Dir == to.Dir ? 0 : 1000;
            var moveCost = from.Pos.ManhatanDistance(to.Pos);
            return turnCost + moveCost;
        }

        static IEnumerable<PosDir> Next(Dictionary<Coord2D, char> map, PosDir from)
        {
            var steps = new[]
            {
                (from.Dir,from.Dir, from.Dir),
                (Coord2D.Zero, from.Dir.RotateLeft(), from.Dir.RotateLeft()),
                (Coord2D.Zero, from.Dir.RotateRight(), from.Dir.RotateRight())
            };

            foreach (var step in steps)
            {
                var move = from.Pos + step.Item1;
                var check = from.Pos + step.Item2;
                if (map.TryGetValue(check, out var c) && c is not '#')
                {
                    yield return (move, step.Item2);
                }
            }
        }
    }

    public object Part2(Input input)
    {
        var s = input.FirstOrDefault(i => i.Value is 'S').Key;
        PosDirPrev start = (s, E, null);
        Coord2D end = input.FirstOrDefault(i => i.Value is 'E').Key;

        var distance = GetDistance(input, start, Cost, Next);

        var q = new Queue<PosDirPrev>();
        var unique = new HashSet<Coord2D>();
        var visited = new HashSet<PosDirPrev>();
        distance.Where(d => d.Key.Pos == end).GroupBy( d => d.Value).MinBy( g=> g.Key)!.ForEach(d => q.Enqueue(d.Key));

        while (q.Count > 0)
        {
            var current = q.Dequeue();

            if (!visited.Add(current))
            {
                continue;
            }
            var (pos, dir, prev) = current;

            unique.Add(pos);
            if (prev is null)
            {
                continue;
            }

            distance.Where(d => d.Key.Pos == prev.Value.Pos && d.Key.Dir == prev.Value.Dir ).GroupBy(d => d.Value).MinBy(g => g.Key)!.ForEach(d => q.Enqueue(d.Key));
        }

        return unique.Count;


        static long? Cost(Dictionary<Coord2D, char> map, PosDirPrev from, PosDirPrev to)
        {
            var turnCost = from.Dir == to.Dir ? 0 : 1000;
            var moveCost = from.Pos.ManhatanDistance(to.Pos);
            return turnCost + moveCost;
        }

        static IEnumerable<PosDirPrev> Next(Dictionary<Coord2D, char> map, PosDirPrev from)
        {
            var prev = (from.Pos, from.Dir);
            var steps = new[]
            {
                (from.Dir,from.Dir, from.Dir),
                (Coord2D.Zero, from.Dir.RotateLeft(), from.Dir.RotateLeft()),
                (Coord2D.Zero, from.Dir.RotateRight(), from.Dir.RotateRight())
            };

            foreach (var step in steps)
            {
                var move = from.Pos + step.Item1;
                var check = from.Pos + step.Item2;
                if (map.TryGetValue(check, out var c) && c is not '#')
                {
                    yield return (move, step.Item2, prev);
                }
            }
        }
    }
}
