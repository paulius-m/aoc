using MoreLinq;
using Tools;
using static Tools.Neighbourhoods;

namespace Days.Y2024.Day15;
using Input = (Grid<char>, Coord2D[]);


file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input"))).Split("").ToArray() switch
        {
            [var map, var rules] => (
                new Grid<char>(map.SelectMany((r, ri) => r.Select((c, ci) => Grid<char>.Coord(ri, ci, c)))),
                (from l in rules
                 from r in l
                 select r switch
                 {
                     '^' => N,
                     '>' => E,
                     'v' => S,
                     '<' => W
                 }).ToArray()
            )
        };
    }

    public object Part1(Input input)
    {
        var (map, rules) = input;

        var walls = new HashSet<Coord2D>(map.Where(kv => kv.Value is '#').Select(kv => kv.Key));
        var boxes = new HashSet<Coord2D>(map.Where(kv => kv.Value is 'O').Select(kv => kv.Key));
        var robot = map.First(kv => kv.Value is '@').Key;


        foreach (var rule in rules)
        {
            var next = robot + rule;

            if (walls.Contains(next))
            {
                continue;
            }

            var toPush = EnumerableEx.Gen(next, nBox => nBox + rule).TakeWhile(boxes.Contains).ToArray();
            if (toPush.Length > 0)
            {
                if (walls.Contains(toPush[^1] + rule))
                {
                    continue;
                }

                toPush.ForEach(b => { boxes.Remove(b); });
                toPush.Select(t => t + rule).ForEach(b => { boxes.Add(b); });
            }
            robot = next;
        }

        return boxes.Aggregate(0L, (a, b) => a + b.r * 100 + b.c);
    }

    public object Part2(Input input)
    {
        var (map, rules) = input;
        var walls = new HashSet<Coord2D>(map.Where(kv => kv.Value is '#').SelectMany(kv => new Coord2D[] { kv.Key with { c = kv.Key.c * 2 }, kv.Key with { c = kv.Key.c * 2 + 1 } } ));
        var boxes = new HashSet<Box>(map.Where(kv => kv.Value is 'O').Select(kv => new Box(kv.Key with { c = kv.Key.c * 2 })));
        var robot = map.First(kv => kv.Value is '@').Key;
        robot = robot with { c = robot.c * 2 };

        var extendedMap = new Grid<char>(
            [
                ..walls.Select(w => KeyValuePair.Create(w, '#')),
                ..boxes.SelectMany(b => b.ToGridCoord()  ),
                KeyValuePair.Create(robot, '@')
            ]
            );

        Console.WriteLine(extendedMap.ToVoidString('.'));

        foreach (var rule in rules)
        {
            var next = robot + rule;

            var toCheck = new Queue<Coord2D>();
            toCheck.Enqueue(next);
            var toPush = new List<Box>();
            var collidedWithWall = false;
            while (toCheck.Count > 0)
            {
                var c = toCheck.Dequeue();
                if (walls.Contains(c))
                {
                    collidedWithWall = true;
                    break;
                }
                foreach(var box in boxes.Where(b => b.Contains(c)))
                {
                    toPush.Add(box);
                    box.GetCheckRays(rule).ForEach(toCheck.Enqueue);
                }
            }

            if (collidedWithWall)
            {
                continue;
            }

            toPush.ForEach(b => { boxes.Remove(b); });
            toPush.Select(t => t with { Corner = t.Corner + rule }).ForEach(b => { boxes.Add(b); });
            robot = next;
        }

        return boxes.Aggregate(0L, (a, b) => a + b.Corner.r * 100 + b.Corner.c);
    }
}
record Box(Coord2D Corner)
{
    long Width = 2;
    public bool Contains(Coord2D point)
    {
        return point.r == Corner.r && Corner.c <= point.c && point.c < Corner.c + Width;
    }

    public KeyValuePair<Coord2D, char>[] ToGridCoord()
    {
        return [KeyValuePair.Create(Corner, '['), KeyValuePair.Create(Corner with { c = Corner.c + 1 }, ']')];
    }

    public Coord2D[]  GetCheckRays(Coord2D dir)
    {
        return dir switch
        {
            _ when dir.c > 0 => [Corner + dir * 2],
            _ when dir.c < 0 => [Corner + dir],
            _ => [Corner + dir, Corner + E + dir]
        };
    }
}