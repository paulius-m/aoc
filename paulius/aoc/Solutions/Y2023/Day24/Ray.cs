using Numerics;
using System.Numerics;
using Tools;

namespace Days.Y2023.Day24;

record class Ray(long[] Origin, long[] Direction)
{
    private long[] b = Origin.Zip(Direction, (a, b) => a + b).ToArray();

    public float[] OriginF = Origin.SelectArray(o => (float)o / 10000000000000);
    public float[] DirectionF = Direction.SelectArray(o => (float)o);

    public bool Intersects2D(Ray l, AABB bounds)
    {
        BigInteger x1 = Origin[0];
        BigInteger y1 = Origin[1];
        BigInteger x2 = b[0];
        BigInteger y2 = b[1];
        BigInteger x3 = l.Origin[0];
        BigInteger y3 = l.Origin[1];
        BigInteger x4 = l.b[0];
        BigInteger y4 = l.b[1];

        var d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
        if (d == BigInteger.Zero)
        {
            // Console.WriteLine($"parallel");
            return false;
        }
        checked
        {
            var px1 = (x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4);

            var py1 = (x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4);

            float[] p = [(float)new BigRational(px1, d), (float)new BigRational(py1, d)];

            // Console.WriteLine($"{Hits(p)} {l.Hits(p)} {bounds.Contains(p)} {p[0]} {p[1]} ");

            return Hits(p) && l.Hits(p) && bounds.Contains(p);
        }
    }

    public bool Hits(float[] p)
    {
        var pDir = Origin.Zip(p, (i, j) => j - i).ToArray();

        return Direction.Zip(pDir, (d, z) => Math.Sign(d) == Math.Sign(z)).All(a => a);
    }

    public void Intersects3D(Ray l)
    {

    }

    public bool Intersects3D(Ray l, AABB bounds)
    {
        //        Let's try this with vector algebra. First write the two equations like 
        //this.
        //  L1 = P1 + a V1
        //  L2 = P2 + b V2
        var P1 = Origin;
        var V1 = Direction;
        var P2 = l.Origin;
        var V2 = l.Direction;

        //P1 and P2 are points on each line.V1 and V2 are the direction vectors
        //for each line.

        //If we assume that the lines intersect, we can look for the point on L1
        //that satisfies the equation for L2.This gives us this equation to
        //solve.

        //  P1 + a V1 = P2 + b V2

        //Now rewrite it like this.

        //  a V1 = (P2 - P1) + b V2

        //Now take the cross product of each side with V2.This will make the
        //term with 'b' drop out.

        //  a(V1 X V2) = (P2 - P1) X V2

        var V1xV2 = Cross(V1, V2);
        if (Length(V1xV2) == 0)
        {
            return false;
        }

        var PxV2 = Cross(P2.Zip(P1, (p2, p1) => p2 - p1).ToArray(), V2);

        var d = Dot(PxV2, V1xV2);

        if (d is 0)
        {
            return false;
        }

        //var a = Length(PxV2) / Length(V1xV2);

        ////  L1 = P1 + a V1

        //var C = P1.Zip(V1.Select(v1 => v1 * a), (p, v) => p + v);

        //If the lines intersect at a single point, then the resultant vectors
        //on each side of this equation must be parallel, and the left side must
        //not be the zero vector. We should check to make sure that this is
        //true. Once we have checked this, we can solve for 'a' by taking the
        //magnitude of each side and dividing.If the resultant vectors are
        //parallel, but in opposite directions, then 'a' is the negative of the
        //ratio of magnitudes.Once we have 'a' we can go back to the equation
        //for L1 to find the intersection point.

        return true;
    }

    public static long[] Cross(long[] a, long[] b)
    {
        return [
            a[2]*b[0] - a[0]*b[2],
            a[0]*b[1] - a[1]*b[0],
            a[1]*b[2] - a[2]*b[1]
        ];
    }

    public BigInteger[] Cross(BigInteger[] a, BigInteger[] b)
    {
        return [
            a[2]*b[0] - a[0]*b[2],
            a[0]*b[1] - a[1]*b[0],
            a[1]*b[2] - a[2]*b[1]
        ];
    }


    public static double Dot(long[] a, long[] b)
    {
        return Normalize(a).Zip(Normalize(b), (aa, bb) => aa * bb).Sum();
    }

    public static double Length(long[] a)
    {
        return Math.Sqrt(a.Sum(aa => aa * aa));
    }

    public static double[] Normalize(long[] a)
    {
        var l = Length(a);
        return a.SelectArray(aa => aa / l);
    }

    public float[] PointAt(float v)
    {
        return [
           OriginF[0] + DirectionF[0] * v,
           OriginF[1] + DirectionF[1] * v,
           OriginF[2] + DirectionF[2] * v
        ];
    }
}

