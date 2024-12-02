using MoreLinq;
using Sprache;
using Tools;

namespace Days.Y2023.Day13;
using Input = string[][];

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var lines = await File.ReadAllLinesAsync(this.GetInputFile("input"));

        return lines.Split("").Select(l => l.ToArray()).ToArray();
    }

    public object Part1(Input input)
    {
        var rows = GetMirrorIndexes(input).ToArray();

        var inputTransposed = from set in input
                              select set.Transpose().Select(ts => new string(ts.ToArray())).ToArray();

        var columns = GetMirrorIndexes(inputTransposed).ToArray();

        return rows.Sum(s => s.i) * 100 + columns.Sum(s => s.i);

        static IEnumerable<(string[] set, int i)> GetMirrorIndexes(IEnumerable<string[]> input)
        {
            return from set in input
                   from i in Enumerable.Range(1, set.Length - 1)
                   let d = set[0..i].Reverse().ToArray()
                   let d2 = set[i..]
                   where d2.Zip(d, (a1, a2) => a1 == a2).All(t => t)
                   select (set, i);
        }
    }

    public object Part2(Input input)
    {
        var rows = GetMirrorIndexes(input).ToArray();

        var inputTransposed = from set in input
                              select set.Transpose().Select(ts => new string(ts.ToArray())).ToArray();

        var columns = GetMirrorIndexes(inputTransposed).ToArray();

        return rows.Sum(s => s.i) * 100 + columns.Sum(s => s.i);

        static IEnumerable<(string[] set, int i)> GetMirrorIndexes(IEnumerable<string[]> input)
        {
            return from set in input
                   from i in Enumerable.Range(1, set.Length - 1)
                   let d = set[0..i].Reverse().ToArray()
                   let d2 = set[i..]
                   where d2.Zip(d, (a1, a2) => a1.Zip(a2, (c1, c2) => c1 == c2 ? 0 : 1).Sum()).Sum() == 1
                   select (set, i);
        }
    }
}