using Tools;
using Input = System.Collections.Generic.IEnumerable<long[]>;

namespace Days.Y2024.Day02;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                select l.Split(' ').SelectArray(long.Parse));
    }

    public object Part1(Input input)
    {
        return CountSafe(input).Count();
    }

    private static IEnumerable<int> CountSafe(Input input)
    {
        return from l in input
               let safe = l.Zip(l.Skip(1), (a, b) => a - b).ToArray()
               where safe.All(s => Math.Abs(s) is >= 1 and <= 3) && safe.All(s => Math.Sign(s) == Math.Sign(safe[0]))
               select 1;
    }

    public object Part2(Input input)
    {
        return (from l in input
                let safeRemovedOne = Enumerable.Range(0, l.Length).Select(i => l.Where((n, ii) => ii != i).ToArray())
                where CountSafe(safeRemovedOne).Any()
                select 1).Count();
    }
}