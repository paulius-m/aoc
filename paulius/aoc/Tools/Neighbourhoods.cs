namespace Tools;
using Map = Dictionary<Neighbourhoods.Coord2D, char>;
using MapPoint = KeyValuePair<Neighbourhoods.Coord2D, char>;
using MapQueue = PriorityQueue<Neighbourhoods.Coord2D, int>;

public static class Neighbourhoods
{
    private static int[] ncoords = new[] { -1, 0, 1 };

    public readonly static Coord2D[] Near4 = (from c in ncoords
                                                     from r in ncoords
                                                     where (r is 0 || c is 0) && (r, c) is not (0, 0)
                                                     select new Coord2D(r, c)).ToArray();

    public readonly static Coord2D[] Near8 = (from c in ncoords
                                                     from r in ncoords
                                                     where (r, c) is not (0, 0)
                                                     select new Coord2D(r, c)).ToArray();

    public readonly static Coord2D[] Near9 = (from c in ncoords
                                                     from r in ncoords
                                                     select new Coord2D(r, c)
                                                     ).ToArray();
    public static IEnumerable<Coord2D> GetNear4(Coord2D center) => GetNear(center, Near4);
    public static IEnumerable<Coord2D> GetNear8(Coord2D center) => GetNear(center, Near8);
    public static IEnumerable<Coord2D> GetNear(Coord2D center, Coord2D[] near) => from nc in near select center + nc;

    public static Coord2D N = new (-1, 0);
    
    public static Coord2D NW = new (-1, -1);
    
    public static Coord2D NE = new (-1, 1);
    public static Coord2D W = new (0, -1);
    public static Coord2D E = new (0, 1);
    public static Coord2D C = new (0, 0);
    public static Coord2D S = new (1, 0);
    public static Coord2D SW = new (1, -1);
    public static Coord2D SE = new (1, 1);

    public record Coord2D(long r, long c)
    {
        public static Coord2D Zero = new Coord2D(0, 0);

        public static Coord2D operator +(Coord2D a, Coord2D b) => new(a.r + b.r, a.c + b.c);
        public static Coord2D operator -(Coord2D a, Coord2D b) => new(a.r - b.r, a.c - b.c);
        public static Coord2D operator -(Coord2D a) => new(-a.r, -a.c);
        public static Coord2D operator *(Coord2D a, long b) => new(a.r * b, a.c * b);

        public Coord2D RotateRight() => new(c, -r);
        public Coord2D RotateLeft() => new(-c, r);
    }

    public delegate int? CostFunction(Map map, Coord2D from, Coord2D to);
    public static int GetDistance(Map map, MapPoint start, MapPoint end, CostFunction f)
    {
        var distance = new Dictionary<Coord2D, int> { [start.Key] = 0 };
        var q = new MapQueue();
        q.Enqueue(start.Key, 0);

        while (q.Count > 0)
        {
            var current = q.Dequeue();
            var currentCost = distance[current];

            foreach (var (pos, cost) in from n in GetNear4(current)
                                        where map.ContainsKey(n)
                                        select (n, f(map, current, n)))
            {
                if (cost is not null)
                {
                    var totalCost = currentCost + cost.Value;

                    if (totalCost < distance.GetValueOrDefault(pos, int.MaxValue))
                    {
                        q.Enqueue(pos, totalCost);
                        distance[pos] = totalCost;
                    }
                }
            }
        }

        return distance.GetValueOrDefault(end.Key, int.MaxValue);
    }
}
