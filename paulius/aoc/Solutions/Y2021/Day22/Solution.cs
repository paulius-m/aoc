using static MoreLinq.Extensions.PairwiseExtension;
using Sprache;
using System;
using Tools;
using static Days.Y2021.Day22.Solution;
using Cube = System.ValueTuple<int, int, int>;
using System.Collections.Concurrent;

namespace Days.Y2021.Day22;

public class Solution : ISolution<Step[]>
{
    public async Task<Step[]> LoadInput()
    {
        return Steps.Parse(await File.ReadAllTextAsync(this.GetInputFile("input")));
    }

    public object Part1(Step[] steps)
    {
        var cubes = (CuboidRange[] ranges) =>
                    from x in ranges[0].Coords(-50, 50)
                    from y in ranges[1].Coords(-50, 50)
                    from z in ranges[2].Coords(-50, 50)
                    select (x, y, z);

        var space = new HashSet<Cube>();

        foreach (var s in steps)
        {
            var c = cubes(s.Cuboid.Ranges);
            if (s.TurnedOn)
                space.UnionWith(c);
            else
                space.ExceptWith(c);
        }

        return space.Count();
    }

    public object Part2(Step[] steps)
    {
        var space = new List<Cuboid>();

        foreach (var s in steps)
        {
            var newSpace = new List<Cuboid>();

            if (s.TurnedOn)
            {
                newSpace.Add(s.Cuboid);
            }

            foreach (var sp in space)
            {
                if (s.Cuboid.Overlaps(sp))
                {
                    newSpace.AddRange(sp.Partition(s.Cuboid).Where(p => !s.Cuboid.Contains(p)));
                }
                else
                {
                    newSpace.Add(sp);
                }
            }

            space = newSpace;
        }
        checked
        {
            return space.Sum(p => p.Volume());
        }
    }

    static Parser<bool> OnOff = from s in Parse.String("on").Or(Parse.String("off")).Text()
                                select s is "on";

    static Parser<int> Int = from s in Parse.Char('-').Optional()
                             from f in Parse.Number
                             select int.Parse(s.GetOrElse('0') + f);

    static Parser<CuboidRange> Range = from d in Parse.Chars('x', 'y', 'z')
                                       from _ in Parse.Char('=')
                                       from f in Int
                                       from __ in Parse.String("..")
                                       from t in Int
                                       select new CuboidRange { From = f, To = t };


    static Parser<Step> StepParser = from turnedOn in OnOff
                                     from _ in Parse.WhiteSpace
                                     from r in Range.DelimitedBy(Parse.Char(',')).Select(r => r.ToArray())
                                     select new Step
                                     {
                                         TurnedOn = turnedOn,
                                         Cuboid = new Cuboid { Ranges = r }
                                     };

    static Parser<Step[]> Steps = from s in StepParser.DelimitedBy(Parse.LineTerminator)
                                  select s.ToArray();

    public record Step
    {
        public bool TurnedOn { get; set; }
        public Cuboid Cuboid { get; set; }
    }

    public record Cuboid
    {
        public CuboidRange[] Ranges { get; set; }

        public Cuboid[] Partition(CuboidRange r, int dim)
        {
            var c = Ranges[dim];

            if (!c.Overlaps(r))
            {
                return new[] { this };
            }

            var cuts = new[]
            {
                new CuboidRange { From = Math.Min(r.From, c.From), To = Math.Max(r.From, c.From) - 1 },
                new CuboidRange { From = Math.Max(r.From, c.From), To = Math.Min(r.To, c.To) },
                new CuboidRange { From = Math.Min(r.To, c.To) + 1, To = Math.Max(r.To, c.To) }
            }
            .Where(r => r.From <= r.To)
            .Where(c.Contains).ToArray();

            return cuts.Select(c =>
            {
                var copy = Ranges.ToArray();
                copy[dim] = c;
                return new Cuboid { Ranges = copy };
            }).ToArray();
        }

        public Cuboid[] Partition(Cuboid c)
        {
            var partitions = new List<Cuboid> { this };
            for (int i = 0; i < c.Ranges.Length; i++)
            {
                var newPartitions = new List<Cuboid>();

                foreach (var p in partitions)
                {
                    if (p.Overlaps(c))
                    {
                        newPartitions.AddRange(p.Partition(c.Ranges[i], i));
                    }
                    else
                    {
                        newPartitions.Add(p);
                    }
                }

                partitions = newPartitions;
            }

            return partitions.ToArray();
        }

        public bool Contains(Cuboid c)
        {
            return Ranges.Zip(c.Ranges, (s, r) => s.Contains(r)).All(Operators.Identity);
        }

        public bool Overlaps(Cuboid c)
        {
            return Ranges.Zip(c.Ranges, (s, r) => s.Overlaps(r)).All(Operators.Identity);
        }

        public long Volume()
        {
            var v = Ranges.Product(cr => cr.To - cr.From + 1);
            return v;
        }
    }

    public record CuboidRange
    {
        public int From { get; set; }
        public int To { get; set; }

        public bool Contains(CuboidRange r)
        {
            return From <= r.From && r.To <= To;
        }

        public IEnumerable<int> Coords(int min, int max)
        {
            for (int i = Math.Max(From, min); i <= Math.Min(To, max); i++)
            {
                yield return i;
            }
        }

        public bool Overlaps(CuboidRange r)
        {
            return r.From <= To && r.To >= From;
        }
    }

}