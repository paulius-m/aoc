using MoreLinq.Extensions;
using Tools;
using Input = System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<long>>;

namespace Days.Y2024.Day01;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var input = from l in (await File.ReadAllLinesAsync(this.GetInputFile("input")))
                let s = l.Split(' ')
                select new[] { Convert.ToInt64(s.First()), Convert.ToInt64(s.Last()) };

        return input.Transpose();
    }

    public object Part1(Input input)
    {
        return input.First().Order().Zip(input.Last().Order(), (a, b) => Math.Abs(a - b)).Sum();
    }

    public object Part2(Input input)
    {
        return (from f in Group(input.First())
                join l in Group(input.Last()) on f.Key equals l.Key
                select f.Key * f.Count * l.Count).Sum();

        static IEnumerable<(long Key, int Count)> Group(IEnumerable<long> e) => e.GroupBy(f => f).Select(f => (f.Key, f.Count()));
    }
}