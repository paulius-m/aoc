using Tools;

namespace Days.Y2022.Day25;
using Input = SNAFU[];

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                select new SNAFU(l)).ToArray();
    }

    public object Part1(Input input)
    {
        foreach (var item in input)
        {
            Console.WriteLine(item.ToString());
        }

        return input.Aggregate((a, b) => a + b).ToString();
    }

    public object Part2(Input i)
    {
        return 0;
    }
}

file struct SNAFU
{
    public readonly long Value { get; }
    private const int BASE = 5;

    public SNAFU(string s)
    {
        Value = Parse(s);
    }

    public SNAFU(long i)
    {
        Value = i;
    }

    private static long Parse(string s)
    {
        var value = 0L;
        var position = 1L;

        for (var i = s.Length - 1; i >= 0; i--)
        {
            value += Parse(s[i]) * position;
            position *= BASE;
        }
        return value;
    }

    private static long Parse(char v) => v switch
    {
        '=' => -2,
        '-' => -1,
        '0' => 0,
        '1' => 1,
        '2' => 2
    };

    private static char Parse(long v) => v switch
    {
        -2 => '=',
        -1 => '-',
        0 => '0',
        1 => '1',
        2 => '2',
    };

    public override string ToString()
    {
        var sb = new LinkedList<char>();
        var value = Value;
        while (value > 0)
        {
            var p = value % BASE;
            value /= BASE;

            if (p > 2)
            {
                p -= BASE;
                value += 1;
            }
            sb.AddFirst(Parse(p));
        }

        return new string(sb.ToArray());
    }

    public static SNAFU operator +(SNAFU a, SNAFU b) => new SNAFU(a.Value + b.Value);
}