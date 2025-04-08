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

            return Hits(p) && l.Hits(p) && bounds.Contains(p);
        }
    }

    public bool Hits(float[] p)
    {
        var pDir = Origin.Zip(p, (i, j) => j - i).ToArray();

        return Direction.Zip(pDir, (d, z) => Math.Sign(d) == Math.Sign(z)).All(a => a);
    }


    public float[] PointAt(float v)
    {
        return [
           OriginF[0] + DirectionF[0] * v,
           OriginF[1] + DirectionF[1] * v,
           OriginF[2] + DirectionF[2] * v
        ];
    }

    public long[] PointAt(long v)
    {
        return [
           Origin[0] + Direction[0] * v,
           Origin[1] + Direction[1] * v,
           Origin[2] + Direction[2] * v
        ];
    }


    public static long[] Minus(long[] a, long[] b)
    {
        return a.Zip(b, (a, b) => a - b).ToArray();
    }


    public static long[] CalculateLineLineIntersection(Ray line1, Ray line2)
    {
        // Algorithm is ported from the C algorithm of 
        // Paul Bourke at http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline3d/

        var p1 = line1.Origin;
        var p2 = line1.PointAt(1);
        var p3 = line2.Origin;
        var p4 = line2.PointAt(1);
        var p13 = Ray.Minus(p1, p3);
        var p43 = Ray.Minus(p4, p3);
        var p21 = Ray.Minus(p2, p1);

        BigInteger d1343 = p13[0] * p43[0] + p13[1] * p43[1] + p13[2] * p43[2];
        BigInteger d4321 = p43[0] * p21[0] + p43[1] * p21[1] + p43[2] * p21[2];
        BigInteger d1321 = p13[0] * p21[0] + p13[1] * p21[1] + p13[2] * p21[2];
        BigInteger d4343 = p43[0] * p43[0] + p43[1] * p43[1] + p43[2] * p43[2];
        BigInteger d2121 = p21[0] * p21[0] + p21[1] * p21[1] + p21[2] * p21[2];

        var mua = new BigRational(d1343 * d4321 - d1321 * d4343, d2121 * d4343 - d4321 * d4321);
        var mub = (d1343 + d4321 * mua) / d4343;

        return [
            (long)(p1[0] + mua * p21[0]),
            (long)(p1[1] + mua * p21[1]),
            (long)(p1[2] + mua * p21[2])
        ];
    }
}

