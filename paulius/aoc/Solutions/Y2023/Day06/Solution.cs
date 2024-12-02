using Tools;

namespace Days.Y2023.Day06;
using Input = string[];

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return await File.ReadAllLinesAsync(this.GetInputFile("input"));
    }

    public object Part1(Input lines)
    {
        var input = GetNumbers(lines[0]).Zip(GetNumbers(lines[1]), (time, distance) => new Race(time, distance)).ToArray();

        return GetWins(input);

        static IEnumerable<long> GetNumbers(string l) => l.Split(" ").Skip(1).Where(n => !string.IsNullOrEmpty(n)).Select(long.Parse);
    }

    public object Part2(Input lines)
    {
        var input = GetNumbers(lines[0]).Zip(GetNumbers(lines[1]), (time, distance) => new Race(time, distance)).ToArray();

        return GetWins(input);

        static IEnumerable<long> GetNumbers(string l) => l.Replace(" ", "").Split(":").Skip(1).Where(n => !string.IsNullOrEmpty(n)).Select(long.Parse);
    }

    private static object GetWins(Race[] input)
    {
        var counts = new long[input.Length];
        for (var t = 0; t < input.Length; t++)
        {
            var (time, recordDistance) = input[t];
            for (var i = 0; i <= time; i++)
            {
                var distance = i * (time - i);
                if (distance > recordDistance)
                {
                    counts[t]++;
                }
            }
        }

        return counts.Product(Operators.Identity);
    }
}

file record Race(long Time, long Distance);