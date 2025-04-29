using Sprache;
using Tools;

namespace Days.Y2019.Day12;
using Input = Vector[];

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return VectorsParser.Parse(await File.ReadAllTextAsync(this.GetInputFile("input")));
    }

    public object Part1(Input vectors)
    {
        var moons = vectors.Select(v => new Moon { Position = new Vector(v.Coords), Velocity = new Vector(0, 0, 0) }).ToArray();

        var pairs =
            from a in moons
            from b in moons
            select (a, b);

        var periods = new int[3];

        foreach (var iter in Enumerable.Range(0, 1000))
        {
            foreach (var p in pairs)
            {
                ApplyGravity(p);
            }

            foreach (var m in moons)
            {
                ApplyVelocity(m);
            }
        }

        var totalEnergy = moons
            .Select(m => m.Position.Coords.Sum(Math.Abs) * m.Velocity.Coords.Sum(Math.Abs))
            .Sum();
        return totalEnergy;
    }

    public object Part2(Input vectors)
    {
        var moons = vectors.Select(v => new Moon { Position = new Vector(v.Coords), Velocity = new Vector(0, 0, 0) }).ToArray();

        var pairs =
            from a in moons
            from b in moons
            select (a, b);

        var periods = new long[3];

        foreach (var iter in Enumerable.Range(1, int.MaxValue))
        {
            foreach (var p in pairs)
            {
                ApplyGravity(p);
            }

            foreach (var m in moons)
            {
                ApplyVelocity(m);
            }

            var oneCoordIsZero = (from i in Enumerable.Range(0, 3)
                                  from m in moons
                                  select (i, v: m.Velocity[i]) into c
                                  group c.v by c.i into c0
                                  where c0.All(IsZero)
                                  select c0.Key).ToList();

            if (oneCoordIsZero.Count > 0)
            {
                foreach (var c in oneCoordIsZero)
                {
                    if (vectors.Zip(moons, (v, m) => v.Coords[c] == m.Position.Coords[c]).All(e => e))
                    {
                        if (periods[c] == 0)
                        {
                            periods[c] = iter;

                        }
                    }
                }

                if (periods.All(p => p != 0))
                {
                    break;
                }
            }
        }

        return periods.LeastCommonMultiple();
    }

    private static bool IsZero(int arg)
    {
        return arg == 0;
    }

    private static void ApplyVelocity(Moon m)
    {
        foreach (var i in Enumerable.Range(0, 3))
        {
            m.Position[i] += m.Velocity[i];
        }
    }

    static void ApplyGravity((Moon a, Moon b) pair)
    {
        var (a, b) = pair;
        foreach (var i in Enumerable.Range(0, 3))
        {
            var diff = (a.Position[i], b.Position[i]) switch
            {
                (var ca, var cb) when ca > cb => -1,
                (var ca, var cb) when ca < cb => 1,
                _ => 0,
            };
            a.Velocity[i] += diff;
        }
    }

    static Parser<int> CoordinateParser(char coord) => from _ in Parse.String(", ").Optional()
                                                       from a in Parse.Char(coord)
                                                       from e in Parse.Char('=')
                                                       from s in Parse.Char('-').Optional().Select(s => s.GetOrElse(' '))
                                                       from c in Parse.Number
                                                       select int.Parse(s + c);

    static Parser<Vector> VectorParser = from _ in Parse.String("<")
                                         from x in CoordinateParser('x')
                                         from y in CoordinateParser('y')
                                         from z in CoordinateParser('z')
                                         from __ in Parse.String(">")
                                         select new Vector(x, y, z);

    static Parser<Vector[]> VectorsParser = from v in VectorParser.DelimitedBy(Parse.String(Environment.NewLine).Text())
                                            select v.ToArray();

}

class Vector
{
    public readonly int[] Coords = new int[3];

    public Vector(params int[] coords)
    {
        coords.CopyTo(Coords, 0);
    }
    public int this[int index]
    {
        get => Coords[index];
        set => Coords[index] = value;
    }
}

class Moon
{
    public Vector Position;
    public Vector Velocity;
}
