using Tools;

namespace Days.Y2023.Day24;
using Input = Ray[];

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                select l.Split('@').SelectArray(s => s.Split(',').SelectArray(long.Parse)) into ab
                select new Ray(ab[0], ab[1])
                ).ToArray();
    }

    //AABB testArea = new AABB
    //{
    //    Ranges = [new CoordRange(200000000000000L, 400000000000000L), new CoordRange(200000000000000L, 400000000000000L)]
    //};

    AABB testArea = new AABB
    {
        Ranges = [new CoordRange(7, 27), new CoordRange(7, 27)]
    };

    public object Part1(Input i)
    {
        var acc = 0;

        for (var j = 0; j < i.Length - 1; j++)
        {
            var a = i[j];

            for (var k = j + 1; k < i.Length; k++)
            {
                var b = i[k];
                if (a.Intersects2D(b, testArea))
                {
                    acc++;
                }
            }
        }

        return acc;
    }

    public object Part2(Input input)
    {
        List<HashSet<long>>[] uniq = [
            [],
            [],
            []
            ];

        for (var i1 = 0; i1 < input.Length - 1; i1++)
        {
            var r1 = input[i1];
            for (var i2 = i1 + 1; i2 < input.Length; i2++)
            {
                var r2 = input[i2];
                for (var i3 = 0; i3 < r1.Direction.Length; i3++)
                {
                    if (r1.Direction[i3] == r2.Direction[i3])
                    {
                        var d = r1.Origin[i3] - r2.Origin[i3];
                        var h = new HashSet<long>();
                        for (var ro1 = -1000; ro1 < 1000; ro1++)
                        {
                            var vd = r1.Direction[i3] - ro1;

                            if (vd != 0 && d % vd == 0)
                                h.Add(ro1);
                        }
                        uniq[i3].Add(h);
                    }
                }
            }
        }

        var v = uniq.SelectArray(u => u.Aggregate((a, b) => a.Intersect(b).ToHashSet()).Single());

        var o1 = new Ray(input[0].Origin, Ray.Minus(input[0].Direction, v));
        var o2 = new Ray(input[1].Origin, Ray.Minus(input[1].Direction, v));
        Render.Draw(input);
        return Ray.CalculateLineLineIntersection(o1, o2).Sum();
    }
}