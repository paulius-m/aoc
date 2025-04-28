using System.Text.RegularExpressions;
using Tools;
using static Days.Y2021.Day02.Solution;

namespace Days.Y2021.Day02;

public class Solution : ISolution<(Direction command, int units)[]>
{
    public async Task<(Direction command, int units)[]> LoadInput()
    {
        var r = new Regex(@"(?<item1>\w+) (?<item2>\d+)", RegexOptions.Compiled);

        return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .Select(l => r.Match<(Direction command, int units)>(l))
            .ToArray();
    }

    public object Part1((Direction command, int units)[] commands)
    {
        var finalPos = commands.Aggregate((x: 0, y: 0), (a, c) =>
         c.command switch
         {
             Direction.forward => a with { x = a.x + c.units },
             Direction.up => a with { y = a.y - c.units },
             Direction.down => a with { y = a.y + c.units }
         });

        return finalPos.x * finalPos.y;
    }

    public object Part2((Direction command, int units)[] commands)
    {
        var finalPos = commands.Aggregate((x: 0, y: 0, aim: 0), (a, c) =>
         c.command switch
         {
             Direction.forward => a with { x = a.x + c.units, y = a.y + a.aim * c.units },
             Direction.up => a with { aim = a.aim - c.units },
             Direction.down => a with { aim = a.aim + c.units }
         });

        return finalPos.x * finalPos.y;
    }

    public enum Direction
    {
        forward,
        up,
        down
    }
}