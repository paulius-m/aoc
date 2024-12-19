using Tools;

namespace Days.Y2024.Day19;
using Input = (string[] Patterns, string[] Designs);


file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input"))) switch
        {
            [var patterns, "", .. var designs] => (patterns.Split(", "), designs)
        };
    }

    public object Part1(Input input)
    {
        var (patterns, designs) = input;
        return designs.Where(d => Doable(d, patterns)).Count();
    }

    bool Doable(string design, string[] patterns)
    {
        if (design is "")
        {
            return true;
        }

        for (int i = 0; i < patterns.Length; i++)
        {
            var pattern = patterns[i];
            if (design.StartsWith(pattern) && Doable(design.Substring(pattern.Length), patterns))
            {
                return true;
            }
        }
        return false;
    }

    public object Part2(Input input)
    {
        var (patterns, designs) = input;
        Func<string, long> DoableCount = null!;
        DoableCount = FuncEx.Memoize((string design) =>
        {
            if (design is "")
            {
                return 1;
            }
            var count = 0L;
            for (int i = 0; i < patterns.Length; i++)
            {
                var pattern = patterns[i];
                if (design.StartsWith(pattern))
                {
                    count += DoableCount(design.Substring(pattern.Length));
                }
            }
            return count;
        });

        return designs.Sum(d => DoableCount(d));
    }
}