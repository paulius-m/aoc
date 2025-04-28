using System.Text.RegularExpressions;
using Tools;

namespace Days.Y2021.Day17;
using Input = Solution.AABB;

public class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .Select(new Regex(@"target area: x=(?<minx>-?\d+)..(?<maxx>-?\d+), y=(?<miny>-?\d+)..(?<maxy>-?\d+)").Match<Input>).Single();
    }

    public object Part1(Input box)
    {
        var maxY = 0;
        for (int vx = 1; vx < 1000; vx++)
            for (int vy = 1; vy < 1000; vy++)
            {
                var pmaxY = 0;
                foreach (var p in CalculateTrajectory(new(0, 0), new(vx, vy)))
                {
                    pmaxY = Math.Max(pmaxY, p.Y);

                    if (box.Overlaps(p))
                    {
                        maxY = Math.Max(pmaxY, maxY);
                        break;
                    }

                    if (box.MaxX < p.X || box.MinY > p.Y)
                    {
                        break;
                    }
                }
            }

        return maxY;
    }

    public IEnumerable<Vect2D> CalculateTrajectory(Vect2D o, Vect2D v)
    {
        while (true)
        {
            yield return o;
            o = new(o.X + v.X, o.Y + v.Y);
            v = new(Math.Max(v.X - 1, 0), v.Y - 1);
        }
    }

    public object Part2(Input box)
    {
        var initialV = new List<Vect2D>();
        for (int vx = 1; vx < 10000; vx++)
            for (int vy = -1000; vy < 1000; vy++)
            {
                foreach (var p in CalculateTrajectory(new(0, 0), new(vx, vy)))
                {
                    if (box.Overlaps(p))
                    {
                        initialV.Add(new(vx, vy));
                        break;
                    }

                    if (box.MaxX < p.X || box.MinY > p.Y)
                    {
                        break;
                    }
                }
            }

        return initialV.Count;
    }

    public record struct Vect2D(int X, int Y);
    public record struct AABB
    {
        public int MinX { get; set; }
        public int MaxX { get; set; }

        public int MinY { get; set; }
        public int MaxY { get; set; }

        public bool Overlaps(Vect2D point)
        {
            var (x, y) = point;

            return MinX <= x && x <= MaxX && MinY <= y && y <= MaxY;
        }
    }
}