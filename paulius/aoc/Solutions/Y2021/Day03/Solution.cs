using Tools;

namespace Days.Y2021.Day03;

public class Solution : ISolution<char[][]>
{
    public async Task<char[][]> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .Select(l => l.ToCharArray()).ToArray();
    }

    public object Part1(char[][] input)
    {
        int[] com = CalculateCommonality(input);

        var gamma = Convert.ToInt32(new string(com.Select(a => a > 0 ? '1' : '0').ToArray()), 2);
        var epsilon = Convert.ToInt32(new string(com.Select(a => a < 0 ? '1' : '0').ToArray()), 2);

        return gamma * epsilon;
    }

    private static int[] CalculateCommonality(char[][] input)
    {
        return input.Aggregate(new int[input[0].Length],
            (a, c) => a.Zip(c, (ai, ci) => ai + (ci is '0' ? -1 : 1)).ToArray()).ToArray();
    }

    public object Part2(char[][] input)
    {
        static int FindNumber(char[][] input, char moreCommon, char lessCommon)
        {
            for (var i = 0; input.Length > 1 && i < input[0].Length; i++)
            {
                input = CalculateCommonality(input)[i] switch
                {
                    >= 0 => input.Where(ol => ol[i] == moreCommon).ToArray(),
                    < 0 => input.Where(ol => ol[i] == lessCommon).ToArray()
                };
            }
            return Convert.ToInt32(new string(input.Single()), 2);
        }

        var o = FindNumber(input, '1', '0');
        var c = FindNumber(input, '0', '1');

        return o * c;
    }
}