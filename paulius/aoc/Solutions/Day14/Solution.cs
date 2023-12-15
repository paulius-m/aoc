using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Tools;

namespace Days.Day14;

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

    public object Part1(Input input)
    {
        var south = input.Max(kv => kv.Key.r);
        var east = input.Max(kv => kv.Key.c);
        var size = new Coord2D(east, south);
        var moved = Move(new(input, N, size));

        return moved.Sum(m => south - m.Key.r + 1);
    }

    private static Input Move(Params p)
    {
        var (input, dir, size) = p;
        var stable = input.Where(c => c.Value is '#').Select(c => c.Key).ToHashSet();
        var moving = input.Where(c => c.Value is 'O').Select(c => c.Key).ToHashSet();
        var moved = moving;
        do
        {
            moving = moved;
            moved = (from c in moving
                     let nc = c + dir
                     select !stable.Contains(nc) && !moving.Contains(nc) && Inside(nc, size)? nc : c).ToHashSet();
        } while (!moved.SetEquals(moving));


        var movedInput = new Input(
            moved.Select(c => KeyValuePair.Create(c, 'O')).Concat(stable.Select(c => KeyValuePair.Create(c, '#')))
            );
        return movedInput;
    }

    private static bool Inside(Coord2D nc, Coord2D size)
    {
        return 0 <= nc.r && nc.r <= size.r &&  0 <= nc.c && nc.c <= size.c;
    }

    public object Part2(Input input)
    {
        Coord2D[] cycle = [N, W, S, E];
        var south = input.Max(kv => kv.Key.r);
        var east = input.Max(kv => kv.Key.c);
        var size = new Coord2D(east, south);
        var moved = input;
        List<string> previous = new();
        List<Input> previousInput = new();
        var cycles = 1000000000;
        for(long i = 0; i <= cycles; i++)
        {
            for (long j = 0; j < cycle.Length; j++)
            moved = Move(new(moved, cycle[j], size));
            if (previous.LastIndexOf(moved.ToVoidString('.')) is var previousI && previousI > -1)
            {
                var repeat = i - previousI;

                var lastI = (cycles - previousI - 1) % repeat;

                moved = previousInput[(int)lastI + previousI];
                break; 
            }         

            previousInput.Add(moved);
            previous.Add(moved.ToVoidString('.'));
        }

        return moved.Where(m => m.Value is 'O').Sum(m => south - m.Key.r + 1);
    }

    record Params(Input input, Coord2D dir, Coord2D size)
    {
        public string InputString {get;} = input.ToVoidString('.');
        public HashSet<Coord2D> coords {get;} =  input.Keys.ToHashSet();
    }
}