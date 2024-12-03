using System.Text.RegularExpressions;
using Tools;
using Input = string;

namespace Days.Y2024.Day03;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return await File.ReadAllTextAsync(this.GetInputFile("input"));
    }

    public object Part1(Input input)
    {
        var r = new Regex(@"(?<Name>mul)\((?<A>\d+)\,(?<B>\d+)\)");

        var m = r.Matches<Command>(input);

        return m.Sum(mm => mm.A * mm.B);
    }

    public object Part2(Input input)
    {
        var r = new Regex(@"(?<Name>mul|don't|do)\((?:(?<A>\d+)\,(?<B>\d+))?\)");
        var m = r.Matches<Command>(input);

        return m.Aggregate((Enabled: true, Sum: 0L), (a, command) => command.Name switch
        {
            "do" => a with { Enabled = true },
            "don't" => a with { Enabled = false },
            "mul" when a.Enabled => a with { Sum = a.Sum + command.A * command.B },
            _ => a
        }).Sum;
    }

    private struct Command
    {
        public string Name;
        public long A;
        public long B;

        public override string ToString()
        {
            return $"{Name}({A},{B})";
        }
    }
}