using Tools;
using static Tools.Neighbourhoods;

namespace Days.Y2023.Day17;
using Map = Grid<int>;

file class Solution : ISolution<Map>
{
    public async Task<Map> LoadInput()
    {
        return new((await File.ReadAllLinesAsync(this.GetInputFile("input")))
                .SelectMany((r, ri) => r.Select((c, ci) => KeyValuePair.Create(new Coord2D(ri, ci), c - '0')))
                );
    }

    public object Part1(Map i)
    {
        return GetDistance(i, new Coord2D(0, 0), new Coord2D(i.Keys.Max(k => k.r), i.Keys.Max(k => k.c)), Cost);
    }

    public object Part2(Map i)
    {
        return GetDistanceUltra(i, new Coord2D(0, 0), new Coord2D(i.Keys.Max(k => k.r), i.Keys.Max(k => k.c)), Cost);
    }

    int? Cost(Map map, Coord2D from, Coord2D to)
    {
        return map.TryGetValue(to, out var v) ? v : null;
    }

    public delegate int? CostFunction(Map map, Coord2D from, Coord2D to);

    public static long GetDistance(Map map, Coord2D start, Coord2D end, CostFunction f)
    {
        var distance = new Dictionary<(Coord2D, Coord2D, int), long> { [(start, C, 0)] = 0 };
        var q = new PriorityQueue<(Coord2D, Coord2D, int), long>();
        q.Enqueue((start, C, 0), 0);

        while (q.Count > 0)
        {
            var (current, dir, straight) = q.Dequeue();

            var currentCost = distance[(current, dir, straight)];

            foreach (var (pos, cost) in from ndir in Near4
                                        let n = current + ndir
                                        where map.ContainsKey(n)
                                        select ((n, ndir), f(map, current, n)))
            {
                var nstraight = pos.ndir == dir ? straight + 1 : 0;

                if (cost is not null && pos.ndir + dir != C && nstraight < 3)
                {
                    var totalCost = currentCost + cost.Value;
                    if (totalCost < distance.GetValueOrDefault((pos.n, pos.ndir, nstraight), long.MaxValue))
                    {
                        q.Enqueue((pos.n, pos.ndir, nstraight), totalCost);
                        distance[(pos.n, pos.ndir, nstraight)] = totalCost;
                    }
                }
            }
        }

        return distance.Where(kv => kv.Key.Item1 == end).Min(kv => kv.Value);
    }

    public static long GetDistanceUltra(Map map, Coord2D start, Coord2D end, CostFunction f)
    {
        var startNode = new Node(start, 0, E, 0);
        var distance = new Dictionary<Node, long> { [startNode] = 0 };
        var q = new PriorityQueue<Path, long>();
        q.Enqueue(new(startNode, null), 0);
        Path? path = null;

        while (q.Count > 0)
        {
            var currentPath = q.Dequeue();
            var (currentNode, prev) = currentPath;
            var (current, cost2, dir, straight) = currentNode;
            var currentCost = distance[currentNode];

            if (current == end && straight > 2)
            {
                path = currentPath;
                break;
            }

            foreach (var (pos, cost) in from ndir in Near4
                                        let n = current + ndir
                                        where map.ContainsKey(n) && (ndir == dir || straight > 2)
                                        select ((n, ndir), f(map, current, n)))
            {
                var nstraight = pos.ndir == dir ? straight + 1 : 0;

                if (cost is not null && pos.ndir + dir != C && nstraight < 10)
                {
                    var totalCost = currentCost + cost.Value;

                    var nNode = new Node(pos.n, 0, pos.ndir, nstraight);

                    if (totalCost < distance.GetValueOrDefault(nNode, long.MaxValue))
                    {
                        q.Enqueue(new(nNode, currentPath), totalCost);
                        distance[nNode] = totalCost;
                    }
                }
            }
        }

        return distance[path.Head];

    }

    record Node(Coord2D Head, long Cost, Coord2D Dir, int Straight);
    record Path(Node Head, Path? Prev);

    //public static long GetDistanceMany(Map map, Coord2D start, Coord2D end, CostFunction f)
    //{
    //    List<Path> paths = new List<Path>();
    //    var count = new Dictionary<Coord2D, int>();

    //    var B = new PriorityQueue<Path, long>();
    //    B.Enqueue(new Path(start, 0, C, 0, null), 0);

    //    //Console.CursorTop = 0;
    //    //Console.CursorLeft = 0;
    //    //Console.WriteLine(map.ToRectString());
    //    var K = 10000;
    //    while (B.Count > 0)
    //    {
    //        var p = B.Dequeue();

    //        var (current, currentCost, dir, straight, prev) = p;

    //        if (current == end)
    //        {
    //            paths.Add(p);
    //            if (paths.Count > K)
    //                break;
    //            continue;
    //        }

    //        var counted = count.GetValueOrDefault(current, 0);
    //        if (counted >= K)
    //            continue;

    //        count[current] = counted + 1;

    //        //if (currentCost % 100 == 0)
    //        //{
    //        //    Console.CursorTop = (int)current.r;
    //        //    Console.CursorLeft = (int)current.c;
    //        //    Console.Write("#".PastelBg("FF0000"));
    //        //}

    //        foreach (var (pos, cost) in from ndir in Near4
    //                                    let n = current + ndir
    //                                    where map.ContainsKey(n)
    //                                    select ((n, ndir), f(map, current, n)))
    //        {
    //            var nstraight = pos.ndir == dir ? straight + 1 : 0;
    //            if (cost is not null && pos.ndir + dir != C && nstraight < 3)
    //            {
    //                var totalCost = currentCost + cost.Value;
    //                B.Enqueue(new Path(pos.n, totalCost, pos.ndir, nstraight, p), totalCost);
    //            }
    //        }
    //    }
    //    var shortest = paths.MinBy(p => p.Cost);
    //    {
    //        Console.CursorTop = 0;
    //        Console.CursorLeft = 0;
    //        Console.WriteLine(map.ToRectString());
    //        var cursor = Console.CursorTop;
    //        for (var i = shortest; i != null; i = i.Prev)
    //        {
    //            var c = i.Head;
    //            Console.CursorTop = (int)c.r;
    //            Console.CursorLeft = (int)c.c;
    //            Console.Write("#".Pastel("FF0000"));
    //        }
    //        Console.CursorTop = cursor;
    //        Console.CursorLeft = 0;
    //    }

    //    return shortest.Cost;
    //}

}