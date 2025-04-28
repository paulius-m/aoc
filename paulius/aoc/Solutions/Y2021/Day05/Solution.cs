using System.Text.RegularExpressions;
using Tools;
namespace Days.Y2021.Day05;

public class Solution : ISolution<Solution.LineSegment2D[]>
{
    public async Task<LineSegment2D[]> LoadInput()
    {
        var r = new Regex(@"(?<X1>\d+),(?<Y1>\d+) -> (?<X2>\d+),(?<Y2>\d+)", RegexOptions.Compiled);

        return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .Select(r.Match<LineSegment2D>).ToArray();
    }

    public object Part1(LineSegment2D[] segments)
    {
        return GetPointStatistics(segments.Where(s => s.X1 == s.X2 || s.Y1 == s.Y2)).Count(p => p.count > 1);
    }

    public object Part2(LineSegment2D[] segments)
    {
        return GetPointStatistics(segments).Count(p => p.count > 1);
    }

    private IEnumerable<(int x, int y, int count)> GetPointStatistics(IEnumerable<LineSegment2D> segments)
    {
        var points = segments.SelectMany(s => s.GetSegmentPoints())
            .GroupBy(p => p).Select(g => (g.Key.x, g.Key.y, g.Count()));
        return points;
    }

    public record LineSegment2D
    {
        public int X1 { get; init; }
        public int Y1 { get; init; }

        public int X2 { get; init; }
        public int Y2 { get; init; }

        public IEnumerable<(int x, int y)> GetSegmentPoints()
        {
            var stepX = X2 - X1;
            var stepY = Y2 - Y1;
            var stepCount = Math.Max(Math.Abs(stepX), Math.Abs(stepY));

            for (int i = 0; i <= stepCount; i++)
            {
                yield return (X1 + i * stepX / stepCount, Y1 + i * stepY / stepCount);
            }
        }
    }
}