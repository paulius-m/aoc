using Sprache;
using Tools;

namespace Days.Y2019.Day03;

using Input = (Segment[], Segment[]);

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var input = await File.ReadAllTextAsync(this.GetInputFile("input"));

        var wireVectors = Wires.Parse(input);

        var wireSegments = wireVectors.Select(w =>
            w.Aggregate(Enumerable.Empty<Segment>(), (accum, v) => accum.LastOrDefault() switch
            {
                { To: var t } => accum.Append(new Segment(t, t + v, v))
            }
            ).ToArray()
         );

        var (wireA, wireB) = (wireSegments.First(), wireSegments.Last());

        return (wireA, wireB);
    }

    public object Part1(Input wireSegments)
    {
        var (wireA, wireB) = wireSegments;

        var closestDistance = int.MaxValue;

        var wireALength = 0;
        foreach (var sa in wireA)
        {
            var wireBLength = 0;
            foreach (var sb in wireB)
            {
                var (wasCollision, collisionPoint, fromA, fromB) =
                      (sa.Dir.I == 0 && sb.Dir.J == 0) ? Distance(sa, sb)
                    : (sa.Dir.J == 0 && sb.Dir.I == 0) ? Distance(sb, sa)
                    : default;

                if (wasCollision)
                {
                    closestDistance = Math.Min(closestDistance, collisionPoint.Length);
                }
                wireBLength += sb.Dir.Length;

            }
            wireALength += sa.Dir.Length;
        }
        return closestDistance;
    }

    public object Part2(Input wireSegments)
    {
        var (wireA, wireB) = wireSegments;

        var fastedCollision = int.MaxValue;

        var wireALength = 0;
        foreach (var sa in wireA)
        {
            var wireBLength = 0;
            foreach (var sb in wireB)
            {
                var (wasCollision, collisionPoint, fromA, fromB) =
                      (sa.Dir.I == 0 && sb.Dir.J == 0) ? Distance(sa, sb)
                    : (sa.Dir.J == 0 && sb.Dir.I == 0) ? Distance(sb, sa)
                    : default;

                if (wasCollision)
                {
                    fastedCollision = Math.Min(fastedCollision, wireALength + wireBLength + fromA.Length + fromB.Length);
                }
                wireBLength += sb.Dir.Length;

            }
            wireALength += sa.Dir.Length;
        }
        return fastedCollision;
    }

    private static (bool, Vector, Vector, Vector) Distance(Segment sa, Segment sb)
    {
        var (a, fromA, toA) = (sa.From.I, Math.Min(sa.From.J, sa.To.J), Math.Max(sa.From.J, sa.To.J));
        var (b, fromB, toB) = (sb.From.J, Math.Min(sb.From.I, sb.To.I), Math.Max(sb.From.I, sb.To.I));
        if (fromA < b && b < toA && fromB < a && a < toB)
        {
            return (true, new Vector(a, b), new Vector(a - sb.From.I, 0), new Vector(0, b - sa.From.J));
        }

        return default;
    }

    static readonly Parser<string> VectorSeparator =
    Parse.String(",").Text();

    static readonly Parser<string> WireSeparator =
        Parse.String(Environment.NewLine).Text();

    static readonly Parser<Vector> Vector =
        from direction in Parse.Letter
        from length in Parse.Number.Select(int.Parse)
        select direction switch
        {
            'U' => new Vector(0, length),
            'D' => new Vector(0, -length),
            'L' => new Vector(-length, 0),
            'R' => new Vector(length, 0),
            _ => new Vector(0, 0)
        };

    static readonly Parser<Vector[]> Wire =
        from v in Vector.DelimitedBy(VectorSeparator)
        select v.ToArray();

    static readonly Parser<Vector[][]> Wires =
        from w in Wire.DelimitedBy(WireSeparator)
        select w.ToArray();

}

file struct Vector
{
    public readonly int I;
    public readonly int J;

    public Vector(int i, int j) => (I, J) = (i, j);
    public int Length => Math.Abs(I) + Math.Abs(J);
    public static Vector operator +(Vector a, Vector b) => new Vector(a.I + b.I, a.J + b.J);
}

file struct Segment
{
    public readonly Vector From;
    public readonly Vector To;
    public readonly Vector Dir;
    public Segment(Vector f, Vector t, Vector d) => (From, To, Dir) = (f, t, d);
}
