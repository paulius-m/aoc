using System.Collections.Immutable;
using Tools;
using static Tools.Neighbourhoods;

namespace Days.Y2023.Day23;
using Map = Grid<char>;


file class Solution : ISolution<Map>
{
    public async Task<Map> LoadInput()
    {
        return new((await File.ReadAllLinesAsync(this.GetInputFile("input")))
                .SelectMany((r, ri) => r.Select((c, ci) => KeyValuePair.Create(new Coord2D(ri, ci), c)))
                );
    }

    public object Part1(Map map)
    {
        var maxR = map.Keys.Max(k => k.r);
        var maxC = map.Keys.Max(k => k.c);

        var start = new Coord2D(0, 1);
        var end = new Coord2D(maxR, maxC - 1);
        return GetDistance(map, start, end, CostFunction);

        int? CostFunction(Map map, Coord2D from, Coord2D to)
        {
            if (map[to] is '#') return null;

            if (map[from] is '.') return 1;

            var dir = to - from;

            return map[from] switch
            {
                '>' when dir == E => 1,
                'v' when dir == S => 1,
                '^' when dir == N => 1,
                '<' when dir == W => 1,
                _ => null
            };
        }
    }

    public delegate int? CostFunction(Map map, Coord2D from, Coord2D to);
    public static long GetDistance(Map map, Coord2D start, Coord2D end, CostFunction f)
    {
        var startNode = new Node(start, 0);
        var distance = new Dictionary<Coord2D, Path> { [start] = new(startNode, []) };
        var q = new Queue<Path>();
        q.Enqueue(new(startNode, []));

        while (q.Count > 0)
        {
            var currentPath = q.Dequeue();
            var (currentNode, prev) = currentPath;
            var current = currentNode.Head;
            var currentCost = currentNode.Cost;

            var nVisited = currentPath.Visited.Add(current);

            if (current == end)
            {
                continue;
            }

            foreach (var (pos, cost) in from n in GetNear4(current)
                                        where map.ContainsKey(n) && !currentPath.Contains(n)
                                        select (n, f(map, current, n)))
            {
                if (cost is not null)
                {
                    var totalCost = currentCost + cost.Value;

                    var nNode = new Node(pos, totalCost);
                    q.Enqueue(new(nNode, nVisited));
                    if (!distance.ContainsKey(pos) || totalCost > distance[pos].Head.Cost)
                    {
                        distance[pos] = new(nNode, nVisited);
                    }
                }
            }
        }

        return distance[end].Head.Cost;
    }

    public static long GetSubDistance(Map map, Coord2D start, Coord2D end, HashSet<Coord2D> crossings, CostFunction f)
    {
        var startNode = new Node(start, 0);
        var distance = new Dictionary<Coord2D, Path> { [start] = new(startNode, []) };
        var q = new Queue<Path>();
        q.Enqueue(new(startNode, []));

        while (q.Count > 0)
        {
            var currentPath = q.Dequeue();
            var (currentNode, prev) = currentPath;
            var current = currentNode.Head;
            var currentCost = currentNode.Cost;

            var nVisited = currentPath.Visited.Add(current);

            if (current != start && crossings.Contains(current))
            {
                continue;
            }

            foreach (var (pos, cost) in from n in GetNear4(current)
                                        where map.ContainsKey(n) && !currentPath.Contains(n)
                                        select (n, f(map, current, n)))
            {
                if (cost is not null)
                {
                    var totalCost = currentCost + cost.Value;

                    var nNode = new Node(pos, totalCost);
                    q.Enqueue(new(nNode, nVisited));
                    if (!distance.ContainsKey(pos) || totalCost > distance[pos].Head.Cost)
                    {
                        distance[pos] = new(nNode, nVisited);
                    }
                }
            }
        }

        return distance[end].Head.Cost;
    }


    private static List<Coord2D> GetNearCrossroads(Map map, Coord2D start, HashSet<Coord2D> crossRoads, CostFunction f)
    {
        var nearCrossRoads = new List<Coord2D>();

        HashSet<Coord2D> visited = new();

        HashSet<Coord2D> filled = new();
        Queue<Coord2D> next = new();
        next.Enqueue(start);

        while (next.Count > 0)
        {
            var current = next.Dequeue();
            visited.Add(current);

            if (current != start && crossRoads.Contains(current))
            {
                nearCrossRoads.Add(current);
                continue;
            }

            var connected = (from n in GetNear4(current)
                             where !visited.Contains(n) && map.ContainsKey(n) && f(map, current, n) is not null && !filled.Contains(n)
                             select n).ToArray();

            foreach (var c in connected)
            {
                filled.Add(c);
                next.Enqueue(c);
            }
        }

        return nearCrossRoads;
    }

    public static long GetDistance3(Map map, Coord2D start, Coord2D end, CostFunction f)
    {
        var crossRoads = (from k in map.Keys
                          let nn = from n in GetNear4(k)
                                   where map.ContainsKey(n)
                                   select (n, f(map, k, n)) into nc
                                   where nc.Item2 is not null
                                   select nc.n
                          where nn.Count() > 2
                          select k).Concat([start, end]).ToHashSet();

        var nears = crossRoads.Select(cr => KeyValuePair.Create(cr, GetNearCrossroads(map, cr, crossRoads, f))).ToDictionary();

        var distance = new Dictionary<(Coord2D, Coord2D), long>();
        var q = new Queue<Coord2D>();
        var visited = new HashSet<Coord2D>();
        q.Enqueue(start);

        while (q.Count > 0)
        {
            var current = q.Dequeue();

            if (visited.Contains(current))
                continue;

            visited.Add(current);

            foreach (var n in nears[current])
            {
                distance[(current, n)] = GetSubDistance(map, current, n, crossRoads, f);
            }

            foreach (var n in nears[current].OrderByDescending(nn => distance[(current, nn)]))
            {
                q.Enqueue(n);
            }
        }

        return GetDistance5(nears, distance, new Path(new(start, 0), []), end);

        static long GetDistance5(Dictionary<Coord2D, List<Coord2D>> nears, Dictionary<(Coord2D, Coord2D), long> distance, Path path, Coord2D end)
        {
            long max = 0;
            var ((current, currentCost), visited) = path;

            if (current == end)
            {
                return currentCost;
            }

            var nVisited = visited.Add(current);

            foreach (var n in nears[current].Where(c => !visited.Contains(c)).OrderBy(c => distance[(current, c)]))
            {
                var total = currentCost + distance[(current, n)];
                var nCost = GetDistance5(nears, distance, new(new(n, total), nVisited), end);
                if (nCost > max)
                    max = nCost;
            }

            return max;
        }
    }



    record Node(Coord2D Head, long Cost);
    record Path(Node Head, ImmutableHashSet<Coord2D> Visited)
    {
        public bool Contains(Coord2D c)
        {
            return Visited.Contains(c);
        }
    }

    public object Part2(Map map)
    {
        var maxR = map.Keys.Max(k => k.r);
        var maxC = map.Keys.Max(k => k.c);

        var start = new Coord2D(0, 1);
        var end = new Coord2D(maxR, maxC - 1);

        return GetDistance3(map, start, end, CostFunction);

        int? CostFunction(Map map, Coord2D from, Coord2D to)
        {
            if (map[to] is '#') return null;
            if (map[from] is '#') return null;

            return 1;
        }
    }
}
