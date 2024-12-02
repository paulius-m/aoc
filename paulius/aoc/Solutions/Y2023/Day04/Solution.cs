using MoreLinq.Extensions;
using Tools;

namespace Days.Y2023.Day04;
using Input = Card[];

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var lines = await File.ReadAllLinesAsync(this.GetInputFile("input"));

        return (from l in lines
                select l.Split(':', '|').SelectArray(a => a.Split(" ")) switch
                {
                [[.., var id], var winning, var available] => new Card(
                    long.Parse(id),
                    new(winning.Where(s => !string.IsNullOrWhiteSpace(s)).Select(long.Parse)),
                    new(available.Where(s => !string.IsNullOrWhiteSpace(s)).Select(long.Parse))
                )
                }).ToArray();
    }

    public object Part1(Input i)
    {
        var scores = from card in i
                     select GetScore(card.Winning.Intersect(card.Available).Count());
        return scores.Sum();
    }

    public object Part2(Input input)
    {
        var counts = from card in input
                     select card.Winning.Intersect(card.Available).Count();

        long[] copies = input.SelectArray(_ => 1L);

        counts.ForEach((c, i) => Enumerable
            .Range(i + 1, Math.Min(c, copies.Length - i))
            .ForEach(ii => copies[ii] += copies[i])
        );

        return copies.Sum();
    }

    static double GetScore(int count) => count switch
    {
        0 => 0,
        var s => Math.Pow(2, s - 1)
    };
}
file record Card(long Id, HashSet<long> Winning, HashSet<long> Available);