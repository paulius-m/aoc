using Tools;
using static Tools.Neighbourhoods;

namespace Days.Y2019.Day20;
using Input = (HashSet<Coord2D> Passages, (Coord2D Location, string Name)[] PortalNames);

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var lines = await File.ReadAllLinesAsync(this.GetInputFile("input"));

        var map = new Grid<char>(lines.SelectMany((line, y) => line.Select((c, x) => KeyValuePair.Create(new Coord2D(y, x), c))));

        Coord2D[] letterN = [N * 2, N, W * 2, W, S, S * 2, E, E * 2];

        var passages = (from m in map
                        where m.Value is '.'
                        select m.Key).ToHashSet();

        var portalNames = (from m in passages
                           let portal = new string((from n in letterN
                                                    let nn = n + m
                                                    let c = map.TryGetValue(nn, out var cc) ? cc : ' '
                                                    where char.IsLetter(c)
                                                    select c).ToArray())
                           where portal.Length == 2
                           select (m, portal)).ToArray();

        var portals = (from formP in portalNames
                       join toP in portalNames on formP.portal equals toP.portal
                       where formP.m != toP.m
                       select KeyValuePair.Create(formP.m, toP.m)).ToDictionary();

        return (passages, portalNames);
    }

    public object Part1(Input input)
    {
        var (passages, names) = input;

        var portals = (from formP in names
                       join toP in names on formP.Name equals toP.Name
                       where formP.Location != toP.Location
                       select KeyValuePair.Create(formP.Location, toP.Location)).ToDictionary();

        var start = names.FirstOrDefault(n => n.Name is "AA").Location;
        var end = names.FirstOrDefault(n => n.Name is "ZZ").Location;

        var distances = GetDistance(passages, start, CostFunction, NextFunction);

        return distances[end];

        long? CostFunction(HashSet<Coord2D> map, Coord2D from, Coord2D to)
        {
            return 1;
        }
        IEnumerable<Coord2D> NextFunction(HashSet<Coord2D> map, Coord2D from)
        {
            if (portals.TryGetValue(from, out var to))
            {
                yield return to;
            }

            foreach (var l in from n in GetNear4(@from)
                              where map.Contains(n)
                              select n)
            {
                yield return l;
            }
        }
    }

    public object Part2(Input input)
    {
        var (passages, names) = input;

        var max = passages.Aggregate((a, b) => new Coord2D(Math.Max(a.r, b.r), Math.Max(a.c, b.c)));
        var min = passages.Aggregate((a, b) => new Coord2D(Math.Min(a.r, b.r), Math.Min(a.c, b.c)));

        var portals = (from formP in names
                       join toP in names on formP.Name equals toP.Name
                       where formP.Location != toP.Location
                       select KeyValuePair.Create(formP.Location, (toP.Location, GetRecursionDirection(formP.Location)))).ToDictionary();

        var start = names.FirstOrDefault(n => n.Name is "AA").Location;
        var end = names.FirstOrDefault(n => n.Name is "ZZ").Location;

        NextFunction<HashSet<Coord2D>, (Coord2D, long)> f = NextFunction;

        return GetDistanceTo(passages, (start, 0), (end, 0),  CostFunction, f, FCostFunction);

        long? CostFunction(HashSet<Coord2D> map, (Coord2D, long) from, (Coord2D, long) to)
        {
            return 1;
        }

        long? FCostFunction(HashSet<Coord2D> map, (Coord2D, long) from, (Coord2D, long) end)
        {
            var recursionDistance = Math.Abs(from.Item2 - end.Item2);

            return from.Item1.ManhatanDistance(end.Item1) * recursionDistance;
        }

        IEnumerable<(Coord2D, long)> NextFunction(HashSet<Coord2D> map, (Coord2D, long) from)
        {
            var currentRec = from.Item2;

            if (portals.TryGetValue(from.Item1, out var to) && to.Item2 + currentRec >= 0)
            {
                yield return (to.Item1, to.Item2 + currentRec);
            }

            foreach (var l in from n in GetNear4(@from.Item1)
                              where map.Contains(n)
                              select n)
            {
                yield return (l, currentRec);
            }
        }

        long GetRecursionDirection(Coord2D fromPortal)
        {
            if (fromPortal.r == min.r || fromPortal.r == max.r)
            {
                return -1;
            }

            if (fromPortal.c == min.c || fromPortal.c == max.c)
            {
                return -1;
            }

            return 1;
        }
    }
}
