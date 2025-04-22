using MoreLinq;
using Tools;
using Input = System.String;

namespace Days.Y2022.Day06;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input"))).Single();
    }

    public object Part1(Input input) => FindStart(input, 4);

    public object Part2(Input input) => FindStart(input, 14);

    private static int FindStart(string input, int length)
    {
        for (var i = length; i < input.Length; i++)
        {
            var a = input[(i - length)..i];
            if (a.Distinct().SequenceEqual(a))
                return i;
        }
        return 0;
    }
}