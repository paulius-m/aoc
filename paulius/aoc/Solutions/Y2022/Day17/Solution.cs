using MoreLinq;
using Tools;
using static Tools.Neighbourhoods;

namespace Days.Y2022.Day17;

file class Solution : ISolution<Coord2D[]>
{
    private Grid<char>[] Rocks =
    """
    ####

    .#.
    ###
    .#.

    ..#
    ..#
    ###

    #
    #
    #
    #

    ##
    ##
    """.Split("\r\n")
        .Split("")
        .Select(rock =>
            new Grid<char>(rock.Reverse()
                .SelectMany((r, ri) => r.Select((c, ci) => (ri, ci, c)))
                .Where(p => p.c is '#')
                .ToDictionary(p => new Coord2D(p.ri, p.ci), p => p.c))).ToArray()
                ;

    public async Task<Coord2D[]> LoadInput()
    {
        var left = new Coord2D(0, -1);
        var right = new Coord2D(0, 1);
        return (from row in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                from c in row
                select c switch
                {
                    '>' => right,
                    '<' => left,
                }).ToArray();
    }

    public object Part1(Coord2D[] windPattern)
    {
        return GetHeight(windPattern, 2022);
    }

    public object Part2(Coord2D[] windPattern)
    {
        return GetHeight(windPattern, 1000000000000L);
    }

    private object GetHeight(Coord2D[] windPattern, long rockCount)
    {
        var chamber = new Grid<char>();

        var top = 3L;
        var currentWind = 0;
        var gravity = new Coord2D(-1, 0);
        for (long i = 0; i < rockCount; i++)
        {
            var rock = Rocks[i % Rocks.Length];
            var position = new Coord2D(top, 2);

            for (var f = 0; true; f++)
            {
                var wind = windPattern[currentWind++];
                if (currentWind == windPattern.Length) currentWind = 0;

                if (position.c + wind.c >= 0
                    && position.c + wind.c + rock.Keys.Max(k => k.c) < 7
                    && !chamber.Overlaps(rock, '#', position + wind)
                    )
                {
                    position = position + wind;
                }

                if (!chamber.Overlaps(rock, '#', position + gravity)
                    && position.r + gravity.r >= 0
                    )
                {
                    position = position + gravity;
                }
                else
                {
                    break;
                }
            }

            chamber.Add(rock, position);

            if (Enumerable.Range(0, 7).Select(c => new Coord2D(position.r, c)).All(chamber.ContainsKey))
            {
                var toMove = chamber.Keys.Where(k => k.r >= position.r).ToArray();

                chamber.Clear();
                foreach (var f in toMove)
                {
                    chamber.Add(f, '#');
                }
            }

            top = Math.Max(top, position.r + rock.Keys.Max(k => k.r) + 3 + 1);


            if (i > 0 && i % 10_000_000 is 0)
            {
                var eval = rockCount / i * ((top - 3) * 1d);
                Console.WriteLine(eval);
            }

            //File.WriteAllText("chamber.txt", chamber.ToReversedVoidString('.', '|', 0, 6, 0, top));

            //Console.WriteLine();

            //Console.ReadLine();
        }

        return top - 3;
    }
}