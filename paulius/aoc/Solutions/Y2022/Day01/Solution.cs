using MoreLinq;
using Tools;

namespace Days.Y2022.Day01;

public class Solution : ISolution<int[][]>
{
    public async Task<int[][]> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .Split("")
            .Select(g => g.Select(int.Parse).ToArray())
            .ToArray();
    }

    public object Part1(int[][] calories)
    {
        return calories.Max(c => c.Sum());
    }

    public object Part2(int[][] calories)
    {
        return calories.Select(c => c.Sum()).OrderDescending().Take(3).Sum();
    }
}