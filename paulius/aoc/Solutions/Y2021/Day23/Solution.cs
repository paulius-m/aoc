using static MoreLinq.Extensions.PairwiseExtension;
using Tools;
using Input = System.Object;
using Sprache;

namespace Days.Y2021.Day23;

public class Solution : ISolution<char[]>
{
    public async Task<char[]> LoadInput()
    {
        return new char[] { 'D', 'B', 'D', 'A', 'C', 'B', 'C', 'A' };
        //return new char[] { 'B', 'A', 'C', 'D', 'B', 'C', 'D', 'A' }; // test
    }

    public object Part1(char[] input)
    {
        GenerateLayout(2, out var nodes, out var trans, out var rooms);

        var amphipods = input.Select((c, i) => new Pod(c, rooms[i / 2][i % 2])).ToArray();
        Draw(nodes, amphipods);
        var c = MinCost(nodes, trans, amphipods, 0);

        foreach (var ap in c.Item2)
        {
            Draw(nodes, ap);
        }

        return c;
    }

    public object Part2(char[] input)
    {
        GenerateLayout(4, out var nodes, out var trans, out var rooms);

        var insert = new char[] { 'D', 'D', 'C', 'B', 'B', 'A', 'A', 'C' };

        var amphipods = input.Select((c, i) => new Pod(c, rooms[i / 2][i % 2 * 3]))
            .Concat(insert.Select((c, i) => new Pod(c, rooms[i / 2][i % 2 + 1])))
            .ToArray();

        Draw(nodes, amphipods);
        var c = MinCost(nodes, trans, amphipods, 0);

        foreach (var ap in c.Item2)
        {
            Draw(nodes, ap);
        }

        return c;
    }


    private static void GenerateLayout(int roomCount, out List<Node> nodes, out List<Transition> trans, out List<Node[]> rooms)
    {
        nodes = new List<Node>();
        trans = new List<Transition>();
        rooms = new List<Node[]>();
        var left = 1;

        foreach (var r in new[] { 'A', 'B', 'C', 'D' })
        {
            var room = Enumerable.Range(0, roomCount).Select(i => new Node { Room = r, Top = 1 + i, Left = left + 1 }).ToArray();
            nodes.AddRange(room);

            rooms.Add(room);
            trans.AddRange(room.Pairwise((a, b) => new Transition(a, b, 1)));
            left += 2;
        }

        var hallway = Enumerable.Range(0, rooms.Count + 1).Select(i => new Node { Hall = i + 1, Top = 0, Left = i * 2 + 1 }).ToList();
        nodes.AddRange(hallway);
        var hm0 = new Node { Hall = 0, Top = 0, Left = 0 };
        var hml = new Node { Hall = hallway.Count, Top = 0, Left = hallway.Count * 2 };
        nodes.Add(hm0);
        nodes.Add(hml);

        trans.AddRange(hallway.Pairwise((a, b) => new Transition(a, b, 2)));
        trans.Add(new(hallway[0], hm0, 1));
        trans.Add(new(hallway[^1], hml, 1));

        for (int i = 0; i < rooms.Count; i++)
        {
            trans.Add(new(hallway[i], rooms[i].First(), 2));
            trans.Add(new(hallway[i + 1], rooms[i].First(), 2));
        }
    }

    private static void Draw(List<Node> nodes, Pod[] ap)
    {
        var curTop = Console.CursorTop;
        var maxTop = 0;
        foreach (var n in nodes)
        {
            Console.CursorLeft = n.Left;
            Console.CursorTop = curTop + n.Top;
            Console.Write('.');
            maxTop = Math.Max(maxTop, n.Top);
        }

        foreach (var a in ap)
        {
            Console.CursorLeft = a.Current.Left;
            Console.CursorTop = curTop + a.Current.Top;
            Console.Write(a.Type);
        }

        Console.CursorLeft = 0;
        Console.CursorTop = curTop + maxTop + 3;
    }

    Dictionary<HashSet<Pod>, (int, Pod[][])> _cache = new Dictionary<HashSet<Pod>, (int, Pod[][])>(HashSet<Pod>.CreateSetComparer());
    private (int, Pod[][]) MinCost(List<Node> nodes, List<Transition> trans, Pod[] amphipods, int currentCost)
    {
        if (amphipods.All(a => a.Type == a.Current.Room))
        {
            return (currentCost, new[] { amphipods });
        }

        if (_cache.TryGetValue(amphipods.ToHashSet(), out var p))
        {
            return (p.Item1 is int.MaxValue ? p.Item1 : p.Item1 + currentCost, p.Item2);
        }

        var min = int.MaxValue;
        var minChain = Array.Empty<Pod[]>();

        var routesToTry = new List<(Pod, (Node, int))>();

        for (int i = 0; i < amphipods.Length; i++)
        {
            Pod? a = amphipods[i];
            var canTakeRoom = nodes.Where(n => n.Room == a.Type).All(n => !amphipods.Any(a => a.Current == n && a.Type != n.Room));

            if (canTakeRoom && a.Type == a.Current.Room)
            {
                continue;
            }

            var takenNodes = amphipods.Where(aa => aa != a).Select(aa => aa.Current).ToArray();

            var routes = FindPossibleRoutes(a, trans, takenNodes, canTakeRoom);

            routesToTry.AddRange(routes.Select(r => (a, r)));
        }

        for (int i = 0; i < routesToTry.Count; i++)
        {
            var (a, r) = routesToTry[i];

            var (cost, chain) = MinCost(nodes, trans, amphipods.Where(aa => aa != a).Append(a with { Current = r.Item1 }).ToArray(), currentCost + r.Item2);
            if (cost < min)
            {
                min = cost;
                minChain = chain;
            }
        }

        var newChain = new[] { amphipods }.Concat(minChain).ToArray();
        _cache[amphipods.ToHashSet()] = (min is int.MaxValue ? min : min - currentCost, newChain);

        return (min, newChain);
    }

    Dictionary<char, int> Energy = new()
    {
        ['A'] = 1,
        ['B'] = 10,
        ['C'] = 100,
        ['D'] = 1000
    };

    private (Node toTake, int cost)[] FindPossibleRoutes(Pod a, List<Transition> trans, Node[] taken, bool canTakeRoom)
    {
        var route = new List<(Node, int)>();
        var q = new Queue<(Node, int)>();
        q.Enqueue((a.Current, 0));

        while (q.Count > 0)
        {
            var (node, cost) = q.Dequeue();

            var candidates = trans.Where(t => t.Links(node))
                        .Select(t => t.LinkFrom(node));

            var next = candidates
                        .Where(n => !taken.Contains(n.Item1) &&
                                    !route.Any(r => r.Item1 == n.Item1)
                        )
                        .ToArray();
            for (int i = 0; i < next.Length; i++)
            {
                (Node, int) n = next[i];
                route.Add((n.Item1, (n.Item2 + cost) * Energy[a.Type]));
                q.Enqueue((n.Item1, n.Item2 + cost));
            }
        }

        if (canTakeRoom)
        {
            var toTheRoom = route.Where(r => r.Item1.Room == a.Type && canTakeRoom).OrderByDescending(r => r.Item2).Take(1).ToArray();
            if (toTheRoom.Length > 0)
                return toTheRoom;
        }

        var isCurrentRoom = a.Current.Room is not null;

        if (!isCurrentRoom)
        {
            return route.Where(r => r.Item1.Room == a.Type && canTakeRoom).OrderByDescending(r => r.Item2).Take(1).ToArray();
        }
        else
        {
            return route.Where(r => r.Item1.Hall is not null).ToArray();
        }
    }

    record Node
    {
        public char? Room { get; set; }
        public int? Hall { get; set; }

        public int Top { get; set; }
        public int Left { get; set; }

    }

    record Pod(char Type, Node Current);

    record Transition(Node From, Node To, int Cost)
    {
        public bool Links(Node n) => From == n || To == n;

        public (Node, int) LinkFrom(Node n) => (n == From ? To : From, Cost);
    }
}