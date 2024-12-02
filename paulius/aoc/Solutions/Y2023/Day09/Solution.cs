namespace Days.Y2023.Day09;

using System.Threading.Tasks;
using MoreLinq.Extensions;
using Tools;
using Input = long[][];

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                select l.Split(' ').SelectArray(long.Parse)
        ).ToArray();
    }

    public object Part1(Input t)
    {
        return t.Select(NextGet).Sum();
    }

    public object Part2(Input t)
    {
        return t.Select(NextPrev).Sum();
    }


    private long NextGet(long[] input)
    {
        if (input.Distinct().Count() is 1)
        {
            return input[^1];
        }

        return input[^1] + NextGet(input.Pairwise((a, b) => b - a).ToArray());
    }

    private long NextPrev(long[] input)
    {
        if (input.Distinct().Count() is 1)
        {
            return input[0];
        }

        return input[0] - NextPrev(input.Pairwise((a, b) => b - a).ToArray());
    }


}