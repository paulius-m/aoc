using MoreLinq.Extensions;
using Tools;
using Input = System.Collections.Generic.IEnumerable<long>;

namespace Days.Y2025.Day01;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var input = from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                    select l switch
                    {
                        ['L', .. var num] => -long.Parse(num),
                        ['R', .. var num] => long.Parse(num),
                    };

        return input.ToArray();
    }

    public object Part1(Input input)
    {
        var a = 50L;
        var zeroCount = 0L;
        foreach (var b in input)
        {
            a = (a + b + 100) % 100;
            if (a is 0)
            {
                zeroCount++;
            }
        }
        return zeroCount;
    }

    public object Part2(Input input)
    {
        var a = 50L;
        var zeroCount = 0L;
        foreach (var b in input)
        {
            var newA = a + b % 100;

            zeroCount += newA switch
            {
                > 99 => 1,
                <= 0 when a > 0 => 1,
                _ => 0
            };

            zeroCount += Math.Abs(b / 100);

            a = (newA + 100) % 100;
            
        }
        return zeroCount;
    }
}