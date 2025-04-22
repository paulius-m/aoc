using MoreLinq.Extensions;
using Sprache;
using System.Text.RegularExpressions;
using Tools;

namespace Days.Y2022.Day05;

file record Input(Dictionary<char, Stack<char>> crates, Instruction[] instructions);

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var lines = await File.ReadAllLinesAsync(this.GetInputFile("input"));
        var instuctionsRegex = new Regex(@"move (?<count>\d+) from (?<from>\d) to (?<to>\d)", RegexOptions.Compiled);

        return lines.Split("").ToArray() switch
        {
        [var crates, var instructions] =>
        new(
            crates
                .Transpose()
                .Select(c => new string(c.Reverse().ToArray()).Trim())
                .Skip(1)
                .TakeEvery(4).ToDictionary(c => c[0], c => new Stack<char>(c[1..])),
            instructions.Select(instuctionsRegex.Match<Instruction>).ToArray()
        )
        };
    }

    public object Part1(Input input)
    {
        var (crates, instructions) = input;

        foreach (var (count, from, to) in instructions)
        {
            for (var i = 0; i < count; i++)
            {
                crates[to].Push(crates[from].Pop());
            }
        }

        return new string(crates.OrderBy(kv => kv.Key).Select(kv => kv.Value.Peek()).ToArray());
    }

    public object Part2(Input input)
    {
        var (crates, instructions) = input;

        var temp = new Stack<char>();
        foreach (var (count, from, to) in instructions)
        {
            for (var i = 0; i < count; i++)
            {
                temp.Push(crates[from].Pop());
            }
            for (var i = 0; i < count; i++)
            {
                crates[to].Push(temp.Pop());
            }
        }

        return new string(crates.OrderBy(kv => kv.Key).Select(kv => kv.Value.Peek()).ToArray());
    }

}

file record Instruction(int Count, char From, char To);