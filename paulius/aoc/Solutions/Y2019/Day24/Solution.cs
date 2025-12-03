using Tools;
using static Tools.Neighbourhoods;

namespace Days.Y2019.Day24;

file class Solution : ISolution<HashSet<Coord2D>>
{
    public async Task<HashSet<Coord2D>> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .SelectMany((l, r) => l.Select((i, c) => (new Coord2D(r, c), i)))
            .Where(c => c.i is '#').Select(c => c.Item1).ToHashSet();

    }

    public object Part1(HashSet<Coord2D> input)
    {
        var previous = new HashSet<HashSet<Coord2D>>(HashSet<Coord2D>.CreateSetComparer());

        var coords = (from r in Enumerable.Range(0, 5)
                      from c in Enumerable.Range(0, 5)
                      select new Coord2D(r, c)).ToArray();


        while (!previous.Contains(input))
        {
            var newInput = (from c in coords
                            let n = from nn in GetNear4(c)
                                    where input.Contains(nn)
                                    select nn
                            where input.Contains(c) ? n.Count() is 1 : n.Count() is 1 or 2
                            select c).ToHashSet();
            previous.Add(input);
            input = newInput;
        }

        return input.Sum(c => Math.Pow(2, c.r * 5 + c.c));
    }

    public object Part2(HashSet<Coord2D> input)
    {
        var state = (from i in input
                    select (Coord: i, Depth: 0)).ToHashSet();

        for (int i = 0; i < 200; i++)
        {
            var coords = from s in state
                         from n in GetNearRec(s)
                         select n;

            state = (from c in coords
                     let n = from nn in GetNearRec(c)
                             where state.Contains(nn)
                             select nn
                     where state.Contains(c) ? n.Count() is 1 : n.Count() is 1 or 2
                     select c).ToHashSet();

            //for (int j = state.Min(s => s.Depth); j <= state.Max(s => s.Depth); j++)
            //{
            //    var g = new Grid<char>();
            //    foreach (var s in state.Where(s => s.Depth == j))
            //    {
            //        g[s.Coord] = '#';
            //    }
            //    Console.WriteLine(j);
            //    Console.WriteLine(g.ToVoidString('.', 0, 4, 0, 4));
            //}
        }

        return state.Count();
    }

    private IEnumerable<(Coord2D Coord, int Depth)> GetNearRec((Coord2D Coord, int Depth) s)
    {
        var (c, d) = s;

        return (from n in GetNear4(c)
               from nn in n switch
               {
                   (2, 2) when c.c < 2 => from i in Enumerable.Range(0, 5) select (new Coord2D(i, 0), d + 1),
                   (2, 2) when c.c > 2 => from i in Enumerable.Range(0, 5) select (new Coord2D(i, 4), d + 1),
                   (2, 2) when c.r < 2 => from i in Enumerable.Range(0, 5) select (new Coord2D(0, i), d + 1),
                   (2, 2) when c.r > 2 => from i in Enumerable.Range(0, 5) select (new Coord2D(4, i), d + 1),
                   (-1, _) => [(new Coord2D(1, 2), d - 1)],
                   (_, -1) => [(new Coord2D(2, 1), d - 1)],
                   (5, _) => [(new Coord2D(3, 2), d - 1)],
                   (_, 5) => [(new Coord2D(2, 3), d - 1)],
                   _ => [(n, d)]
               }
               select nn).Distinct();
    }
}
