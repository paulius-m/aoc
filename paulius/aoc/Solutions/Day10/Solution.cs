namespace Days.Day10;

using System.Collections.Frozen;
using Tools;
using static Tools.Neighbourhoods;
using Input = Tools.Grid<char>;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return new((await File.ReadAllLinesAsync(this.GetInputFile("input")))
                .SelectMany((r, ri) => r.Select((c, ci) => KeyValuePair.Create(new Coord2D(ri, ci), c)))
                );
    }

    public object Part1(Input t)
    {
        var g = t.GroupBy(tt => tt.Value != '.').ToDictionary(g => g.Key);
        var pipes = new Input(g[true]);

        return Traverse(pipes).Max(kv => kv.Value);
    }

    private Grid<long> Traverse(Input grid)
    {
        var start = grid.First(kv => kv.Value is 'S').Key;
        Grid<long> lenghts = new() { [start] = 0 };
        HashSet<Coord2D> visited = new();

        Queue<Coord2D> next = new();
        next.Enqueue(start);

        while (next.Count > 0)
        {
            var current = next.Dequeue();
            visited.Add(current);

            var connected = (from n in Near4
                             let nn = n + current
                             where !visited.Contains(nn) && grid.ContainsKey(nn) && Connects(grid[current], grid[nn], n)
                             select nn).ToArray();

            foreach (var c in connected)
            {
                lenghts[c] = lenghts[current] + 1;
                next.Enqueue(c);
            }
        }

        return lenghts;
    }


    private HashSet<Coord2D> Fill(HashSet<Coord2D> grid)
    {
        var start = new Coord2D(-1, -1);
        HashSet<Coord2D> visited = new();
        
        HashSet<Coord2D> filled = new();
        Queue<Coord2D> next = new();
        next.Enqueue(start);

        while (next.Count > 0)
        {
            var current = next.Dequeue();
            visited.Add(current);
            var connected = (from n in GetNear4(current)
                             where !visited.Contains(n) && grid.Contains(n) && !filled.Contains(n)
                             select n).ToArray();

            foreach (var c in connected)
            {
                filled.Add(c);
                next.Enqueue(c);
            }
        }

        return filled;
    }


    private FrozenDictionary<char, Pipe> _pipes = new Dictionary<char, Pipe>
    {
        ['S'] = new(true, true, true, true),
        ['|'] = new(true, true, false, false),
        ['-'] = new(false, false, true, true),
        ['L'] = new(true, false, false, true),
        ['J'] = new(true, false, true, false),
        ['7'] = new(false, true, true, false),
        ['F'] = new(false, true, false, true)
    }.ToFrozenDictionary();

    private FrozenDictionary<char, Coord2D[]> _pipesZoomed = new Dictionary<char, Coord2D[]>
    {
        ['S'] = [C, N, E, W, S],
        ['|'] = [N, C, S],
        ['-'] = [W, C, E],
        ['L'] = [N, C, E],
        ['J'] = [N, C, W],
        ['7'] = [S, C, W],
        ['F'] = [S, C, E]
    }.ToFrozenDictionary();

    private bool Connects(char current, char check, Coord2D n)
    {
        var currentPipe = _pipes[current];
        var checkPipe = _pipes[check];
        return n switch
        {
            (-1, 0) => currentPipe.n && checkPipe.s,
            (1, 0) => currentPipe.s && checkPipe.n,
            (0, 1) => currentPipe.e && checkPipe.w,
            (0, -1) => currentPipe.w && checkPipe.e,
        };
    }

    public object Part2(Input grid)
    {
        var group = grid.GroupBy(tt => tt.Value != '.').ToDictionary(g => g.Key);
        var pipes = new Input(group[true]);
        var ground = new Input(group[false]);
        var loop = Traverse(pipes);
        var loopPipes = pipes.Where(p => loop.ContainsKey(p.Key)).ToArray();
        var rest = pipes.Where(p => !loop.ContainsKey(p.Key)).Concat(ground).ToArray();
        
        var x3Zoomed =  (from p in loopPipes
                              let pp = p.Key * 3
                              from n in Near9
                              where !_pipesZoomed[p.Value].Contains(n)
                              select pp + n).Concat(from g in rest
                                                    let gg = g.Key * 3
                                                    from n in GetNear(gg, Near9)
                                                    select n).ToHashSet();
        
        var filled = Fill(x3Zoomed);
        
        return rest.Where(g => !filled.Contains(g.Key * 3) ).Count();
    }
    record Pipe(bool n, bool s, bool w, bool e);
}