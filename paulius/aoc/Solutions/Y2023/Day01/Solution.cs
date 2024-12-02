using Tools;
using Input = string[];

namespace Days.Y2023.Day01;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return await File.ReadAllLinesAsync(this.GetInputFile("input"));
    }

    public object Part1(Input input)
    {
        var numbers =
            from c in input
            let digits = c.Where(char.IsNumber).ToArray()
            select long.Parse([digits[0], digits[^1]]);

        return numbers.Sum();
    }

    public object Part2(Input input)
    {
        var numbers = (
            from c in input
            let number = GetDigits(c)
            select number).ToArray();

        return numbers.Sum();
    }

    private long GetDigits(string c)
    {
        string[] digits = ["1", "2", "3", "4", "5", "6", "7", "8", "9"];
        string[] alsoDigits = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];

        string[] allDigits = [.. digits, .. alsoDigits];

        var first = allDigits.Select(d => (d, c.IndexOf(d))).Where(dd => dd.Item2 >= 0).MinBy(dd => dd.Item2).d;
        var last = allDigits.Select(d => (d, c.LastIndexOf(d))).Where(dd => dd.Item2 >= 0).MaxBy(dd => dd.Item2).d;

        return Number(first) * 10 + Number(last);

        long Number(string n) => n.Length > 1 ? Array.IndexOf(alsoDigits, n) + 1 : long.Parse(n);
    }
}