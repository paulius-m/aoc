using System;

namespace Tools;
public record CoordRange
{
    public long From { get; set; }
    public long To { get; set; }

    public CoordRange() { }
    public CoordRange(long from, long to)
    {
        From = from;
        To = to;
    }

    public bool Contains(CoordRange r)
    {
        return From <= r.From && r.To <= To;
    }

    public bool Contains(long point)
    {
        return From <= point && point <= To;
    }

    public bool Contains(float point)
    {
        return From <= point && point <= To;
    }

    public bool Overlaps(CoordRange r)
    {
        return r.From <= To && r.To >= From;
    }

    public IEnumerable<long> Coords(int min, int max)
    {
        for (var i = Math.Max(From, min); i <= Math.Min(To, max); i++)
        {
            yield return i;
        }
    }

    public IEnumerable<long> Coords()
    {
        for (var i = From; i <= To; i++)
        {
            yield return i;
        }
    }

    public void Without(CoordRange r)
    {
        if (Contains(r.From))
        {
            To = r.From - 1;
        }
        if (Contains(r.To))
        {
            From = r.To + 1;
        }
    }

    public IEnumerable<CoordRange> Partition(CoordRange by)
    {
        var c = this;

        if (!c.Overlaps(by))
        {
            return [c];
        }

        var cuts = new[]
        {
                    new CoordRange { From = Math.Min(by.From, c.From), To = Math.Max(by.From, c.From) - 1 },
                    new CoordRange { From = Math.Max(by.From, c.From), To = Math.Min(by.To, c.To) },
                    new CoordRange { From = Math.Min(by.To, c.To) + 1, To = Math.Max(by.To, c.To) }
        };

        cuts = cuts.Where(r => r.From <= r.To)
        .Where(c.Contains).ToArray();
        return cuts;
    }
}
