using MoreLinq;
using Sprache;
using Tools;

namespace Days.Day02;
using Draw = (long count, string color);
using Input = (long id, (long count, string color)[][] game)[];

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                select Game.Parse(l)).ToArray();
    }

    public object Part1(Input i)
    {
        //only 12 red cubes, 13 green cubes, and 14 blue cubes;

        var totals = GetTotals(i);

        var valid = from t in totals
                    where t.totals.GetValueOrDefault("red") <= 12 &&
                    t.totals.GetValueOrDefault("green") <= 13 &&
                    t.totals.GetValueOrDefault("blue") <= 14
                    select t.id;

        return valid.Sum();
    }

    public object Part2(Input i)
    {
        var totals = GetTotals(i);

        var products = from t in totals
                       select t.totals.Values.Aggregate(Operators.Multiply);

        return products.Sum();
    }

    private static (long id, Dictionary<string, long> totals)[] GetTotals((long id, (long count, string color)[][] game)[] i)
    {
        return (from g in i
                select (g.id, totals: (from m in g.game
                                       from d in m
                                       group d by d.color into dd
                                       select (dd.Key, dd.Max(ddd => ddd.count)))
                                       .ToDictionary(kv => kv.Key, kv => kv.Item2))
                    ).ToArray();
    }

    static Parser<Draw> Draw = from count in Parse.Number.Select(long.Parse)
                               from _ in Parse.WhiteSpace
                               from color in Parse.Letter.Many().Text()
                               select (count, color);

    static Parser<Draw[]> Match = from m in Draw.DelimitedBy(Parse.String(", "))
                                  select m.ToArray();

    static Parser<(long, Draw[][])> Game =
                from _ in Parse.String("Game ")
                from id in Parse.Number.Select(long.Parse)
                from __ in Parse.String(": ")
                from m in Match.DelimitedBy(Parse.String("; "))
                select (id, m.ToArray());
}