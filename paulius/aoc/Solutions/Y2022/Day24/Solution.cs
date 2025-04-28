using System.Diagnostics;
using Tools;
using static Tools.Neighbourhoods;

namespace Days.Y2022.Day24;

using Input = (Blizzard[] blizzards, Coord2D walls, Coord2D start, Coord2D end);
using Coord2DTime = (Coord2D Location, long Time);
file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var grid = new Grid<char>((await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .SelectMany((r, ri) => r.Select((c, ci) => KeyValuePair.Create(new Coord2D(ri, ci), c))));

        var blizzards = from c in grid
                        where c.Value is not '.' and not '#'
                        select new Blizzard(c.Key, c.Value switch
                        {
                            '^' => N,
                            '>' => E,
                            'v' => S,
                            '<' => W
                        });

        var maxR = grid.Keys.Max(c => c.r);
        var maxC = grid.Keys.Max(c => c.c);

        var start = grid.First(kv => kv.Key.r is 0 && kv.Value is '.').Key;
        var end = grid.First(kv => kv.Key.r == maxR && kv.Value is '.').Key;

        return (blizzards.ToArray(), new Coord2D(maxR, maxC), start, end);
    }

    public object Part1(Input input)
    {
        Debug.Assert(Pin(new Coord2D(0, 0), 3, 3) == new Coord2D(2, 2));
        Debug.Assert(Pin(new Coord2D(2, 2), 3, 3) == new Coord2D(2, 2));
        Debug.Assert(Pin(new Coord2D(3, 3), 3, 3) == new Coord2D(1, 1));

        var (intitialBlizzard, walls, start, end) = input;
        return GetTravelTime(intitialBlizzard, walls, start, end);
    }

    public object Part2(Input input)
    {
        var (intitialBlizzard, walls, start, end) = input;
        var time1 = GetTravelTime(intitialBlizzard, walls, start, end);
        var time2 = GetTravelTime(intitialBlizzard, walls, end, start, time1);
        return GetTravelTime(intitialBlizzard, walls, start, end, time2);
    }

    private static long GetTravelTime(Blizzard[] intitialBlizzard, Coord2D walls, Coord2D start, Coord2D end, long initialTime = 0)
    {
        var blizzardAt = FuncEx.Memoize((long t) => (from b in intitialBlizzard
                                                     select Pin(b.Pos + b.Dir * t, walls.r, walls.c)).ToHashSet());
        var initial = (start, initialTime);

        Dictionary<Coord2DTime, long> gScore = new() { [initial] = initialTime };

        PrioritySet<Coord2DTime, long> openSet = new();
        openSet.Add(initial, initialTime + start.ManhatanDistance(end));

        while (true)
        {
            var current = openSet.Dequeue();

            if (current.Location == end)
            {
                return gScore[current];
            }

            var blizzard = blizzardAt(current.Time + 1);

            foreach (var (neighbor, cost) in from n in GetNear4(current.Location).Append(current.Location)
                                             where !blizzard.Contains(n) && (n.r > 0 && n.c > 0 && n.r < walls.r && n.c < walls.c || n == start || n == end)
                                             select (n, 1))
            {
                var totalCost = gScore[current] + cost;

                var next = (neighbor, current.Time + 1);

                if (totalCost < gScore.GetValueOrDefault(next, long.MaxValue))
                {
                    gScore[next] = totalCost;

                    if (!openSet.Contains(next))
                    {
                        openSet.Add(next, totalCost + neighbor.ManhatanDistance(end));
                    }
                }
            }
        }
    }

    static Coord2D Pin(Coord2D n, long maxR, long maxC)
    {
        return new Coord2D(Mod(n.r, maxR), Mod(n.c, maxC));

        long Mod(long x, long m)
        {
            return ((x - 1) % (m - 1) + m - 1) % (m - 1) + 1;
        }
    }

}

file record Blizzard(Coord2D Pos, Coord2D Dir);

class PrioritySet<T1, T2> 
{
    HashSet<T1> _set = new();
    PriorityQueue<T1, T2> _queue = new PriorityQueue<T1, T2>();


    public void Add(T1 t1, T2 t2)
    {
        _queue.Enqueue(t1, t2);
        _set.Add(t1);
    }
    
    public bool Contains(T1 t1) { return _set.Contains(t1); }
    public T1 Dequeue()
    {
        var t1 = _queue.Dequeue();
        _set.Remove(t1);
        return t1;
    }
}