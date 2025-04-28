using static MoreLinq.Extensions.SplitExtension;
using Tools;

namespace Days.Y2021.Day08;

public class Solution : ISolution<(HashSet<char>[] signal, HashSet<char>[] output)[]>
{
    public async Task<(HashSet<char>[] signal, HashSet<char>[] output)[]> LoadInput()
    {
        var i = (await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .Select(l => l.Split(" ").Split("|").Select(a => a.Select(aa => aa.ToHashSet()).ToArray()) switch
            {
                var a => (a.First(), a.Last())
            }).ToArray();
        return i;
    }

    public object Part1((HashSet<char>[] signal, HashSet<char>[] output)[] input)
    {
        return input.SelectMany(i => i.output).Count(i => i is { Count: 2 or 3 or 4 or 7 });
    }

    public object Part2((HashSet<char>[] signal, HashSet<char>[] output)[] input)
    {
        IEnumerable<long> enumerable()
        {
            foreach (var i in input)
            {
                var mapping = Order(i.signal);
                var mapped = i.output.Select(o => mapping.First(kv => kv.Key.SetEquals(o)).Value);
                yield return long.Parse(new string(mapped.ToArray()));
            }
        }

        return enumerable().Sum();
    }

    private Dictionary<HashSet<char>, char> Order(HashSet<char>[] signal)
    {
        HashSet<char>[] ordered = new HashSet<char>[signal.Length];

        var or069 = signal.Where(s => s.Count is 6).ToArray();
        var or235 = signal.Where(s => s.Count is 5).ToArray();

        ordered[1] = signal.Single(s => s.Count is 2);
        ordered[4] = signal.Single(s => s.Count is 4);
        ordered[7] = signal.Single(s => s.Count is 3);
        ordered[8] = signal.Single(s => s.Count is 7);

        ordered[9] = or069.Single(s => s.IsSupersetOf(ordered[4].Union(ordered[7])));

        ordered[3] = or235.Single(s => s.IsSupersetOf(ordered[7]) && ordered[9].IsSupersetOf(ordered[4].Union(s)));
        ordered[5] = or235.Single(s => s.IsSupersetOf(ordered[9].Except(ordered[3])));
        ordered[2] = or235.Except(new[] { ordered[3], ordered[5] }).Single();

        HashSet<char> six = new(ordered[2].Except(ordered[9]).Union(ordered[5]));

        ordered[6] = or069.Single(s => s.SetEquals(six));
        ordered[0] = or069.Except(new[] { ordered[6], ordered[9] }).Single();

        return ordered.Select((s, i) => (s, i)).ToDictionary(kv => kv.s, kv => (char)('0' + (char)kv.i));
    }
}