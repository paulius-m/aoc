using System.Text;
using static Tools.Neighbourhoods;

namespace Tools;

public class Grid<T> : Dictionary<Coord2D, T>
{
    public Grid() : base() { }

    public Grid(Dictionary<Coord2D, T> source): base(source){ }

    public Grid(IEnumerable<KeyValuePair<Coord2D, T>> source): base(source){ }
    public void Add(Grid<T> toAdd, Coord2D offset)
    {
        foreach (var kv in toAdd)
        {
            this[kv.Key + offset] = kv.Value;
        }
    }

    public bool Overlaps(Grid<char> check, T overlappingValue, Coord2D offset)
    {
        foreach (var key in check.Keys.Select(k => k + offset))
        {
            if (ContainsKey(key) && this[key].Equals(overlappingValue))
            {
                return true;
            }
        }
        return false;
    }

    public string ToRectString()
    {
        var rows = this
            .GroupBy(m => m.Key.r)
            .OrderBy(g => g.Key)
            .Select(g => string.Join("", g.OrderBy(c => c.Key.c).Select(c => c.Value)));

        return string.Join("\n", rows);
    }

    public string ToVoidString(T fill)
    {
        var minR = Keys.Min(k => k.r);
        var maxR = Keys.Max(k => k.r);
        var minC = Keys.Min(k => k.c);
        var maxC = Keys.Max(k => k.c);

        return ToVoidString(fill, minC, maxC, minR, maxR);
    }

    public string ToVoidString(T fill, long minC, long maxC, long minR, long maxR)
    {
        var b = new StringBuilder();

        for (var r = minR; r <= maxR; r++)
        {
            for (var c = minC; c <= maxC; c++)
            {
                var at = new Coord2D(r, c);

                b.Append(ContainsKey(at) ? this[at] : fill);
            }
            b.Append('\n');
        }

        return b.ToString();
    }

    public string ToReversedVoidString(T fill, T border, long minC, long maxC, long minR, long maxR)
    {
        var b = new StringBuilder();

        for (var r = maxR; r >= minR; r--)
        {
            b.Append(border);
            for (var c = minC; c <= maxC; c++)
            {
                var at = new Coord2D(r, c);

                b.Append(ContainsKey(at) ? this[at] : fill);
            }
            b.Append(border);
            b.Append('\n');
        }

        return b.ToString();
    }

    public string ToReversedVoidString(T fill, T border)
    {
        var minR = Keys.Min(k => k.r);
        var maxR = Keys.Max(k => k.r);
        var minC = Keys.Min(k => k.c);
        var maxC = Keys.Max(k => k.c);

        return ToReversedVoidString(fill, border, minC, maxC, minR, maxR);
    }

    public static KeyValuePair<Coord2D, T> Coord(int ri, int ci, T c)
    {
        return KeyValuePair.Create(new Coord2D(ri, ci), c);
    }
}
