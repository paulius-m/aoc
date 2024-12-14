using Sprache;
using Tools;
using static Tools.Neighbourhoods;
using Input = Tools.Grid<char>;

namespace Days.Y2024.Day12
{
    file class Solution : ISolution<Input>
    {
        public async Task<Input> LoadInput()
        {
            return new Input((await File.ReadAllLinesAsync(this.GetInputFile("input"))).SelectMany((r, ri) => r.Select((c, ci) => Input.Coord(ri, ci, c))));
        }

        public object Part1(Input input)
        {
            var fences = new Dictionary<Coord2D, HashSet<Coord2D>>();
            var areas = new Dictionary<Coord2D, Coord2D>();
            Trace(input, fences, areas);

            var stats = areas.GroupBy(a => a.Value).Select(g => KeyValuePair.Create(g.Key, (g.Count(), g.Sum(gg => fences[gg.Key].Count)))).ToDictionary();
            return stats.Sum(s => s.Value.Item1 * s.Value.Item2);
        }

        private static void Trace(Input input, Dictionary<Coord2D, HashSet<Coord2D>> fences, Dictionary<Coord2D, Coord2D> areas)
        {
            var q = new Queue<(KeyValuePair<Coord2D, char>, Coord2D)>();
            foreach (var start in input)
            {
                q.Enqueue((start, start.Key));
                while (q.Count > 0)
                {
                    var (current, id) = q.Dequeue();
                    var currentFences = new HashSet<Coord2D>();
                    if (areas.ContainsKey(current.Key))
                    {
                        continue;
                    }
                    areas.Add(current.Key, id);

                    foreach (var n in Near4)
                    {
                        var nn = n + current.Key;

                        if (!input.TryGetValue(nn, out char value) || value != current.Value)
                        {
                            currentFences.Add(n);
                            continue;
                        }
                        else
                        {
                            q.Enqueue((KeyValuePair.Create(nn, value), id));
                        }
                    }

                    fences[current.Key] = currentFences;
                }
            }
        }

        public object Part2(Input input)
        {
            var fences = new Dictionary<Coord2D, HashSet<Coord2D>>();
            var areas = new Dictionary<Coord2D, Coord2D>();
            Trace(input, fences, areas);

            var stats = areas.GroupBy(a => a.Value, a => a.Key)
                .Select(g => (g.Key, g.ToArray()))
                .Select(g => KeyValuePair.Create(g.Key, (g.Item2.Length, JoinFences(g.Item2, fences)))).ToDictionary();

            return stats.Sum(s => s.Value.Item1 * s.Value.Item2);
        }

        private long JoinFences(Coord2D[] area, Dictionary<Coord2D, HashSet<Coord2D>> fences)
        {
            var fenceElements = from a in area
                                join f in fences on a equals f.Key
                                where f.Value.Count > 0
                                from ff in f.Value
                                select (Location: a, Direction: ff);

            var fenceDirections = from fe in fenceElements
                                  group fe.Location by fe.Direction;

            return (from f in fenceDirections
                    let byR = f.OrderBy(ff => ff.r).ThenBy(ff => ff.c)
                    let byC = f.OrderBy(ff => ff.c).ThenBy(ff => ff.r)
                    select  (f.Key.c is 0)
                    ? CountSegments(byR, E)
                    : CountSegments(byC, S)).Sum();
        }

        private static int CountSegments(IOrderedEnumerable<Coord2D> ordered, Coord2D inc)
        {
            var segmentCount = 1;
            Coord2D? prev = null;
            foreach (var item in ordered)
            {
                if (prev is not null)
                {
                    if (item - prev != inc)
                    {
                        segmentCount++;
                    }
                }

                prev = item;
            }

            return segmentCount;
        }
    }
}