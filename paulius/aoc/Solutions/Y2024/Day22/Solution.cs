using MoreLinq;
using Sprache;
using Tools;
using Input = long[];

namespace Days.Y2024.Day22;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input"))).SelectArray(long.Parse);
    }

    public object Part1(Input input)
    {
        return (from secret in input
                select EnumerableEx.Gen(secret, Evolve).Take(2001).Last()).Sum();
    }

    private static long Evolve(long secret)
    {
        var shifted = MixAndPrunve(secret, secret << 6);
        var unshifted = MixAndPrunve(shifted, shifted >>> 5);
        return MixAndPrunve(unshifted, unshifted * 2048);
    }

    private static long MixAndPrunve(long secret, long mix)
    {
        return (secret ^ mix) % 16777216;
    }

    //1773
    public object Part2(Input input)
    {
        var secrets = from secret in input
                      select EnumerableEx.Gen(secret, Evolve).Take(2001);

        var prices = from s in secrets
                     select (from ss in s
                             select ss % 10);

        var diffs = (from p in prices
                     select p.Zip(p.Pairwise((a, b) => b - a).Prepend(0)));

        var sequences = (from d in diffs
                         select (from w in d.Window(4)
                                 where w.Count == 4
                                 let key = string.Join(',', w.Select(ww => ww.Second))
                                 group w.Last().First by key into g
                                 select KeyValuePair.Create(g.Key, g.First())
                                 ).ToDictionary()).ToArray();

        var uniqueSequences = (from s in sequences
                               from k in s.Keys
                               select k).ToHashSet();

        return (from u in uniqueSequences
                let maxP = (from s in sequences
                            select s.GetValueOrDefault(u, 0)).Sum()
                select maxP).Max();
    }
}