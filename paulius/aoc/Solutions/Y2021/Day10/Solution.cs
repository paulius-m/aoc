using System.Linq;
using Tools;
using static System.Array;

namespace Days.Y2021.Day10;

public class Solution : ISolution<string[]>
{
    public async Task<string[]> LoadInput()
    {
        return await File.ReadAllLinesAsync(this.GetInputFile("input"));
    }

    char[] opens = new[] { '(', '{', '[', '<' };
    char[] closes = new[] { ')', '}', ']', '>' };
    int[] scores = new[] { 3, 1197, 57, 25137 };
    long[] scores2 = new[] { 1L, 3L, 2L, 4L };

    public object Part1(string[] i)
    {
        return i
            .Select(Parse)
            .Where(c => c.corrupted)
            .Select(c => scores[IndexOf(closes, c.c)])
            .Sum();
    }

    public object Part2(string[] i)
    {
        var ordered = i.Select(Parse)
            .Where(c => !c.corrupted)
            .Select(c => c.left.Aggregate(0L, (a, c) => a * 5L + scores2[IndexOf(opens, c)]))
            .OrderBy(Operators.Identity).ToArray();

        return ordered[ordered.Length / 2];
    }

    private (bool corrupted, char c, Stack<char> left) Parse(string line)
    {
        var stack = new Stack<char>();

        foreach (var b in line)
        {
            switch (b)
            {
                case var o when opens.Contains(o):
                    stack.Push(o);
                    break;
                case var c when closes.Contains(c):
                    if (c != closes[IndexOf(opens, stack.Pop())])
                        return (true, c, stack);
                    break;
            }
        }
        return (false, 'A', stack);
    }
}