using MoreLinq.Extensions;
using System.Collections;
using Tools;
using Rule = (long, long);
using Input = ((long, long)[] Rules, long[][] Updates);

namespace Days.Y2024.Day05;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var text = (await File.ReadAllLinesAsync(this.GetInputFile("input"))).Split("").ToArray();

        return text switch
        {
        [var rules, var updates] => (
            rules.Select(s => s.Split('|').SelectArray(long.Parse) switch
            {
            [var u1, var u2] => (u1, u2)
            }).ToArray(),
            updates.Select(s => s.Split(',').SelectArray(long.Parse)).ToArray()
        )
        };
    }

    public object Part1(Input input)
    {
        return (from update in input.Updates
                let comb = update.SelectMany((u1, i) => update.Skip(i + 1).Select(u2 => (u1, u2)))
                where comb.All(c => input.Rules.Contains(c))
                select update[update.Length / 2]).Sum();
    }

    public object Part2(Input input)
    {
        var cmp = new Comparer(input.Rules);

        var invalid = from update in input.Updates
                      let comb = update.SelectMany((u1, i) => update.Skip(i + 1).Select(u2 => (u1, u2)))
                      where comb.Any(c => !input.Rules.Contains(c))
                      select update;

        return (from update in invalid select update.Order(cmp).Skip(update.Length / 2).First()).Sum();
    }

    class Comparer (Rule[] Rules) : IComparer<long>
    {
        public int Compare(long x, long y)
        {
            if (Rules.Contains((x, y))) return -1;
            if (Rules.Contains((y, x))) return 1;
            return 0;
        }
    }
}