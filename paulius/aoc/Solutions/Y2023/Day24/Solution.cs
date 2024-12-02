using Tools;
using System.Diagnostics;

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
        Test();

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

    public object Part2(Input i)
    {





        Render.Draw(i);

        return 0;
    }

    private void Test()
    {
        var a = new Ray([24, 13, 10], [-3, 1, 2]);
        var b = new Ray([19, 13, 30], [-2, 1, -2]);

        Debug.Assert(a.Intersects3D(b, new()));
    }
}

