using MoreLinq;
using System.Linq;
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
        var seen = new Dictionary<(string, long, int), Stat>();
        var top = 3L;
        var currentWind = 0;
        var gravity = new Coord2D(-1, 0);
        long i = 0;
        for (; i < rockCount; i++)
        {
            var rockId = i % Rocks.Length;
            var rock = Rocks[rockId];
            var position = new Coord2D(top, 2);
            var f = 0;
            for (; true; f++)
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

            top = Math.Max(top, position.r + rock.Keys.Max(k => k.r) + 3 + 1);

            var rowId = new string(Enumerable.Range(0, 7).Select(c => chamber.ContainsKey(new Coord2D(position.r, c)) ? '1' : '0').ToArray());
            var seenId = (rowId, rockId, f);
            var metric = top;

            var prev = seen.TryGetValue(seenId, out var p) ? p : new(new List<(long, long)>());
            prev.Stats.Add((i, metric));
            seen[seenId] = prev;

            if (prev.Stats.Count > 1000)
            {
                break;
            }
        }
        if (i == rockCount)
        {
            return top - 3;
        }

        var lastRock = (rockCount - 1) % Rocks.Length;
        var minTop = long.MaxValue;
        foreach (var (seenId, value) in seen.Where(kv => kv.Key.Item2 == lastRock))
        {
            var deltaI = value.Stats.Pairwise((a, b) => b.I - a.I).ToArray();
            var deltaTop = value.Stats.Pairwise((a, b) => b.Metric - a.Metric).ToArray();

            var loop = GetLoopRange(deltaI);
            if (!loop.HasValue)
                continue;

            var (start, length) = loop.Value.GetOffsetAndLength(deltaI.Length);

            var subranges = Enumerable.Range(start, length)
                .Select(r => KeyValuePair.Create(deltaI[start..r].Sum(), deltaTop[start..r].Sum()))
                .ToDictionary();

            var startI = value.Stats[start].I;
            var startTop = value.Stats[start].Metric;
            var loopI = deltaI[loop.Value].Sum();
            var loopTop = deltaTop[loop.Value].Sum();

            var loops = (rockCount - 1 - startI) / loopI;
            var lefts = (rockCount - 1 - startI) % loopI;

            minTop = Math.Min(minTop,
                lefts is 0 ? startTop + loopTop * loops
                : subranges.ContainsKey(lefts) ? startTop + loopTop * loops + subranges[lefts]
                : long.MaxValue);
        }

        return minTop - 3;
    }

    private Range? GetLoopRange(long[] arr)
    {
        for (var i = 0; i < arr.Length - 1; i++)
        {
            for (var j = i + 1; j < arr.Length;)
            {
                var nextLoop = Array.IndexOf(arr, arr[i], j);
                if (nextLoop is -1)
                {
                    break;
                }
                var loopLength = nextLoop - i;
                if (nextLoop + loopLength > arr.Length)
                {
                    break;
                }
                var loopRange = i..nextLoop;
                if (arr[loopRange].SequenceEqual(arr[nextLoop..(nextLoop + loopLength)]))
                {
                    return loopRange;
                }
                else
                {
                    j = nextLoop + 1;
                }
            }
        }
        return null;
    }
}

internal record struct Stat(List<(long I, long Metric)> Stats)
{
}