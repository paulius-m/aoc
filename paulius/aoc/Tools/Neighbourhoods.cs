namespace Tools;

using Sprache;

public static class Neighbourhoods
{
    private static int[] ncoords = [-1, 0, 1];

    public readonly static Coord2D[] Near4 = (from c in ncoords
                                              from r in ncoords
                                              where (r is 0 || c is 0) && (r, c) is not (0, 0)
                                              select new Coord2D(r, c)).ToArray();

    public readonly static Coord2D[] Near4X = (from c in ncoords
                                               from r in ncoords
                                               where (r is not 0 && c is not 0) && (r, c) is not (0, 0)
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
    public static IEnumerable<Coord2D> GetNear4X(Coord2D center) => GetNear(center, Near4X);
    public static IEnumerable<Coord2D> GetNear8(Coord2D center) => GetNear(center, Near8);
    public static IEnumerable<Coord2D> GetNear(Coord2D center, Coord2D[] near) => from nc in near select center + nc;

    public static Coord2D N = new(-1, 0);
    public static Coord2D NW = new(-1, -1);
    public static Coord2D NE = new(-1, 1);
    public static Coord2D W = new(0, -1);
    public static Coord2D E = new(0, 1);
    public static Coord2D C = new(0, 0);
    public static Coord2D S = new(1, 0);
    public static Coord2D SW = new(1, -1);
    public static Coord2D SE = new(1, 1);

    public record Coord2D(long r, long c)
    {
        public static Coord2D Zero = new Coord2D(0, 0);

        public static Coord2D operator +(Coord2D a, Coord2D b) => new(a.r + b.r, a.c + b.c);
        public static Coord2D operator -(Coord2D a, Coord2D b) => new(a.r - b.r, a.c - b.c);
        public static Coord2D operator -(Coord2D a) => new(-a.r, -a.c);
        public static Coord2D operator *(Coord2D a, long b) => new(a.r * b, a.c * b);
        public static Coord2D operator *(Coord2D a, Coord2D b) => new(a.r * b.r, a.c * b.c);
        public Coord2D RotateRight() => new(c, -r);
        public Coord2D RotateLeft() => new(-c, r);

        public long ManhatanDistance(Coord2D b) => Math.Abs(r - b.r) + Math.Abs(c - b.c);
    }

    public delegate long? CostFunction<TMap, T>(TMap map, T from, T to) where T : notnull;
    public delegate IEnumerable<T> NextFunction<TMap, T>(TMap map, T from) where T : notnull;
    public static Dictionary<T, long> GetDistance<TMap, T>(TMap map, T start, CostFunction<TMap, T> f, NextFunction<TMap, T> nf) where T : notnull
    {
        var distance = new Dictionary<T, long> { [start] = 0 };
        var q = new PriorityQueue<T, long>();
        q.Enqueue(start, 0);

        while (q.Count > 0)
        {
            var current = q.Dequeue();
            var currentCost = distance[current];

            foreach (var (pos, cost) in from n in nf(map, current)
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

        return distance;
    }

    public static long GetDistanceTo<TMap, T>(TMap map, T start, T end, CostFunction<TMap, T> cost, NextFunction<TMap, T> next, CostFunction<TMap, T> fCost)
    {
        Dictionary<T, long> gScore = new() { [start] = 0 };

        PrioritySet<T, long> openSet = new();
        openSet.Add(start, fCost(map, start, end).Value);

        while (true)
        {
            var current = openSet.Dequeue();

            if (current.Equals(end))
            {
                return gScore[current];
            }

            foreach (var neighbor in from n in next(map, current)
                                             select n)
            {
                
                var nCost = cost(map, current, neighbor);
                if (nCost is null) continue;

                var totalCost = gScore[current] + nCost.Value;

                if (totalCost < gScore.GetValueOrDefault(neighbor, long.MaxValue))
                {
                    gScore[neighbor] = totalCost;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor, totalCost + fCost(map, neighbor, end).Value);
                    }
                }
            }
        }
    }

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
}
