using Tools;

namespace Days.Y2021.Day07;

public class Solution : ISolution<int[]>
{
    public async Task<int[]> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input"))).Single()
            .Split(',').Select(int.Parse).ToArray();
    }

    public object Part1(int[] i)
    {
        var top = FindLowestPos(i, (center, pos) =>
            (center, pos.Select(p => Math.Abs(center - p)).Sum()));
        return top.cost;
    }

    public object Part2(int[] i)
    {
        var top = FindLowestPos(i, (center, pos) =>
            (center, pos.SelectMany(p => Enumerable.Range(0, Math.Abs(center - p) + 1)).Sum()));
        return top.cost;
    }

    private delegate (int center, int cost) CostFunction(int center, int[] pos);
    private static (int center, int cost) FindLowestPos(int[] i, CostFunction cost)
    {
        var bottom = cost(i.Min(), i);
        var top = cost(i.Max(), i);

        while (bottom.center != top.center)
        {
            var middle = cost((top.center - bottom.center) / 2 + bottom.center, i);

            if (top.cost > bottom.cost)
            {
                top = middle;
            }
            else
            {
                bottom = middle;
            }
        }

        return top;
    }
}