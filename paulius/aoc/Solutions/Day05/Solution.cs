using MoreLinq;
using Sprache;
using Tools;

namespace Days.Day05;
using Input = (long[] Seeds, Map[] Maps);

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return Input.Parse(await File.ReadAllTextAsync(this.GetInputFile("input")));
    }

    public object Part1(Input i)
    {
        var initial = (Name: "seed", Values: i.Seeds);
        var final = i.Maps.Aggregate(initial, (a, b) => i.Maps.First(m => m.Name.From == a.Name) switch
        {
            var map => (map.Name.To, (from v in a.Values
                                      let r = map.Values.FirstOrDefault(mr => mr.Source.Contains(v))
                                      select r is null ? v :
                                      r.Destination.From + v - r.Source.From).ToArray()
                                     )
        });

        return final.Values.Min();
    }

    public object Part2(Input i)
    {
        var seeds = i.Seeds.TakeEvery(2).Zip(i.Seeds.Skip(1).TakeEvery(2), (f, t) => new CoordRange(f, t + f - 1)).ToArray();
        var min = long.MaxValue;
        foreach (var seed in seeds)
        {
            var a = "seed";
            CoordRange[] acc = [seed];
            while (true)
            {
                var map = i.Maps.FirstOrDefault(m => m.Name.From == a);

                if (map is null)
                    break;

                acc = (from v in acc
                       let r = map.Values.Where(mr => mr.Source.Overlaps(v) || mr.Source.Contains(v) || v.Contains(mr.Source)).ToArray()
                       from rr in r.Length is 0 ? [v] : Split(v, r)
                       select rr).Distinct().ToArray();

                a = map.Name.To;
            }
            min = Math.Min(min, acc.Min(a => a.From));
        }

        return min;
    }

    private CoordRange[] Split(CoordRange seed, FromToRange[] ranges)
    {
        CoordRange[] acc = [seed];

        foreach(var r in ranges)
        {
            acc = (from a in acc 
                from p in a.Partition(r.Source)
                   select Map(r.Source, r.Destination, p)).ToArray();
        }
        return acc;

        static CoordRange Map(CoordRange source, CoordRange dest, CoordRange p)
        {
            if (source.Contains(p))
            {
                var mapped = new CoordRange(dest.From + p.From - source.From, dest.From - source.From + p.To);
                return mapped;
            }
            return p;
        }
    }


    static Parser<long[]> Ids = from id in Parse.Number.Select(long.Parse).DelimitedBy(Parse.Char(' '))
                                select id.ToArray();

    static Parser<FromTo> FromTo = from f in Parse.Letter.Many().Text()
                                   from _ in Parse.String("-to-")
                                   from t in Parse.Letter.Many().Text()
                                   from __ in Parse.String(" map:")
                                   select new FromTo(f, t);

    static Parser<long[]> Seeds = from _ in Parse.String("seeds: ")
                                  from ids in Ids
                                  select ids;

    static Parser<FromToRange> Range = from ids in Ids
                                       select ids switch
                                       {
                                       [var dStart, var sStart, var length] => new FromToRange(
                                           Source: new() { From = sStart, To = sStart + length - 1 },
                                           Destination: new() { From = dStart, To = dStart + length - 1 }
                                           )
                                       };

    static Parser<Map> Map = from name in FromTo
                             from _ in Parse.LineTerminator
                             from ranges in Range.DelimitedBy(Parse.LineTerminator)
                             select new Map(name, ranges.ToArray());

    static Parser<Input> Input = from seeds in Seeds
                                 from _ in Parse.LineTerminator.Repeat(2)
                                 from maps in Map.DelimitedBy(Parse.LineTerminator.Repeat(2))
                                 select (seeds, maps.ToArray());

}

file record FromTo(string From, string To);
file record FromToRange(CoordRange Source, CoordRange Destination);
file record Map(FromTo Name, FromToRange[] Values);
