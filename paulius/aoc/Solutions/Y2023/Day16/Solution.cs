using Tools;

namespace Days.Y2023.Day16;
using static Tools.Neighbourhoods;
using Input = Grid<char>;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return new((await File.ReadAllLinesAsync(this.GetInputFile("input")))
                .SelectMany((r, ri) => r.Select((c, ci) => KeyValuePair.Create(new Coord2D(ri, ci), c)))
                );
    }

    public object Part1(Input grid)
    {
        var beams = new List<(Coord2D pos, Coord2D dir)>() { (C, E) };
        return GetEnergized(grid, beams);
    }

    public object Part2(Input grid)
    {
        var width = grid.Max(kv => kv.Key.c) + 1;
        var height = grid.Max(kv => kv.Key.r) + 1;
        long maxEnergy = 0;

        for (int i = 0; i < width; i++)
        {
            maxEnergy = Math.Max(maxEnergy, GetEnergized(grid, new() { (new(0, i), S) }));
            maxEnergy = Math.Max(maxEnergy, GetEnergized(grid, new() { (new(height - 1, i), N) }));
        }

        for (int i = 0; i < height; i++)
        {
            maxEnergy = Math.Max(maxEnergy, GetEnergized(grid, new() { (new(i, 0), E) }));
            maxEnergy = Math.Max(maxEnergy, GetEnergized(grid, new() { (new(i, width - 1), W) }));
        }

        return maxEnergy;
    }

    private static long GetEnergized(Input grid, List<(Coord2D pos, Coord2D dir)> beams)
    {
        var energized = new HashSet<(Coord2D pos, Coord2D dir)>();

        while (beams.Count > 0)
        {
            for (int i = 0; i < beams.Count; i++)
            {
                var t = beams[i];

                if (!grid.ContainsKey(t.pos) || energized.Contains(t))
                {
                    beams.RemoveAt(i);
                    i--;
                    continue;
                }

                energized.Add(t);

                var (pos, dir) = t;
                switch (grid[pos])
                {
                    case '\\':
                        dir = new(dir.c, dir.r);
                        break;
                    case '/':
                        dir = new Coord2D(-dir.c, -dir.r);
                        break;
                    case '|' when dir.r is 0:
                        dir = N;
                        beams.Add((pos, -dir));
                        break;
                    case '-' when dir.c is 0:
                        dir = E;
                        beams.Add((pos, -dir));
                        break;
                    default:
                        break;
                }

                beams[i] = (pos + dir, dir);
            }
        }

        return energized.Select(e => e.pos).Distinct().Count();
    }

}