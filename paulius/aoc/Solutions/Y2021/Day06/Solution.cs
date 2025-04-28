using Tools;

namespace Days.Y2021.Day06;

public class Solution : ISolution<int[]>
{
    public async Task<int[]> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .First().Split(',').Select(int.Parse).ToArray();
    }

    public object Part1(int[] input)
    {
        return GrowCount(input, 80);
    }

    public object Part2(int[] input)
    {
        return GrowCount(input, 256);
    }

    private static object GrowCount(int[] input, int totalDays)
    {
        var boxes = input
            .GroupBy(i => i)
            .Select(g => (g.Key, g.Count()))
            .Aggregate(new long[9], (a, c) => { a[c.Key] = c.Item2; return a; });

        var temp = new long[boxes.Length];

        for (var days = 0; days < totalDays; days++)
        {
            Array.Clear(temp);
            temp[6] = boxes[0];
            temp[8] = boxes[0];
            for (int i = 1; i < boxes.Length; i++)
            {
                temp[i - 1] += boxes[i];
            }
            (boxes, temp) = (temp, boxes);
        }
        return boxes.Sum();
    }
}