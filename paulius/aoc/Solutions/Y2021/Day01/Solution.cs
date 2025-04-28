using Tools;

namespace Days.Y2021.Day01;

public class Solution : ISolution<int[]>
{
    public object Part1(int[] depths)
    {
        return CounIncreases(depths);
    }

    public async Task<int[]> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .Select(int.Parse)
            .ToArray();
    }

    public object Part2(int[] depths)
    {
        var windows = depths.Select((_, i) => depths.Take(i..(i + 3)).Sum()).ToArray();
        return CounIncreases(windows);
    }

    private static int CounIncreases(int[] depths)
    {
        return depths.Zip(depths[1..], (p, n) => n - p).Count(x => x > 0);
    }
}