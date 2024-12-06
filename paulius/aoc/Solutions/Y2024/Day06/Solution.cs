using Sprache;
using Tools;
using Input = Tools.Grid<char>;

namespace Days.Y2024.Day06;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return new Input((await File.ReadAllLinesAsync(this.GetInputFile("input"))).SelectMany((r, ri) => r.Select((c, ci) => Input.Coord(ri, ci, c))));
    }

    public object Part1(Input input)
    {
        HashSet<Neighbourhoods.Coord2D> visited = Visit(input);

        return visited.Count;
    }

    private static HashSet<Neighbourhoods.Coord2D> Visit(Input input)
    {
        HashSet<Neighbourhoods.Coord2D> visited = new();
        var current = input.First(kv => kv.Value is '^').Key;
        var dir = Neighbourhoods.N;

        while (input.Keys.Contains(current))
        {
            visited.Add(current);
            var next = current + dir;
            while (input.Keys.Contains(next) && input[next] is '#')
            {
                dir = dir.RotateRight();
                next = current + dir;
            }

            current = next;
        }

        return visited;
    }

    public object Part2(Input input)
    {
        HashSet<Neighbourhoods.Coord2D> visited = Visit(input);

        return visited.Where( v => CheckForLoop(input, v)).Count();
    }

    private static bool CheckForLoop(Input input, Neighbourhoods.Coord2D insert)
    {
        HashSet<Neighbourhoods.Coord2D> visited = new();
        HashSet<(Neighbourhoods.Coord2D Pos, Neighbourhoods.Coord2D Dir)> hits = new();

        var current = input.First(kv => kv.Value is '^').Key;
        var dir = Neighbourhoods.N;

        while (input.Keys.Contains(current))
        {
            visited.Add(current);
            var next = current + dir;
            while (input.Keys.Contains(next) && input[next] is '#' || next == insert)
            {
                var hit = (next, dir);
                if (hits.Contains(hit))
                {
                    return true;
                }
                hits.Add(hit);

                dir = dir.RotateRight();
                next = current + dir;
            }

            current = next;
        }

        return false;
    }
}