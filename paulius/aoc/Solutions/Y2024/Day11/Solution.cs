using Tools;
using Input = long[];

namespace Days.Y2024.Day11;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (await File.ReadAllTextAsync(this.GetInputFile("input"))).Split(' ').SelectArray(long.Parse);
    }

    public object Part1(Input input)
    {
        IEnumerable<long> stones = input;
        for (int i = 0; i < 25; i++)
        {
            stones = Change(stones);
        }
        return stones.Count();
    }

    private static IEnumerable<long> Change(IEnumerable<long> stones)
    {
        foreach (long stone in stones) {
            switch (stone)
            {
                case 0 :
                    yield return 1;
                    break;

                case var s when ((long)Math.Log10(s) + 1) is var log && log % 2 is 0:
                    var split = (long)Math.Pow(10, log / 2);
                    yield return stone / split;
                    yield return stone % split;
                    break;
                default:
                    yield return stone * 2024;
                    break;
            }
        }
    }

    public object Part2(Input input)
    {
        var stones = input
            .GroupBy(i => i)
            .ToDictionary(g => g.Key, g => g.LongCount());

        var work = new long[1];
        for (int i = 0; i < 75; i++)
        {
            var next = new Dictionary<long, long>();

            foreach (var (stone, count) in stones)
            {
                work[0] = stone;
                foreach (var n in Change(work))
                {
                    next[n] = next.GetValueOrDefault(n) + count;
                }
            }
            stones = next;
        }
        return stones.Sum( s => s.Value);
    }
}