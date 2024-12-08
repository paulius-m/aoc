using MoreLinq;
using Sprache;
using Tools;

namespace Days.Y2024.Day07;

using Input = Equation[];
using Op = Func<long, long, long>;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                let s1 = l.Split(": ")
                let result = long.Parse(s1[0])
                let values = s1[1].Split(" ").SelectArray(long.Parse)
                select new Equation(result, values)
                ).ToArray();
    }

    public object Part1(Input input)
    {
        return (
            from e in input
            where e.Solvable(
                (a, b) => a + b,
                (a, b) => a * b
                )
            select e.Result).Sum();
    }

    public object Part2(Input input)
    {
        return (
            from e in input
            where e.Solvable(
                (a, b) => a + b,
                (a, b) => a * b,
                (a, b) => a * (long)Math.Pow(10, (long)Math.Log10(b) + 1) + b
            )
            select e.Result).Sum();
    }
}

file record Equation(long Result, long[] Values)
{
    public bool Solvable(params Op[] operators)
    {
        return Combinations(operators, Values.Length)
            .Any(c => Values[1..].Zip(c).Aggregate(Values[0], (a, b) => b.Second(a, b.First)) == Result);
    }

    private static IEnumerable<Op[]> Combinations(Op[] operators, int length)
    {
        var combinationCount = Math.Pow(operators.Length, length);

        var combination = new Op[length];

        for (var i = 0; i < combinationCount; i++)
        {
            var v = i;
            for (var j = 0; j < length; j++)
            {
                var opIdx = v % operators.Length;
                v = v / operators.Length;
                combination[j] = operators[opIdx];
            }
            yield return combination;
        }
    }
}