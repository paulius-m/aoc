using Tools;
using Input = System.Collections.Generic.IEnumerable<int[]>;

namespace Days.Y2025.Day03;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
               let n = from c in l select c - '0'
               select n.ToArray();
    }

    public object Part1(Input input)
    {
        var maxes = from i in input
                    let pairs = i.SelectMany((n, ni) => i.Skip(ni + 1).Select((m) => n * 10 + m))
                    select pairs.Max();

        return maxes.Sum();
    }

    public object Part2(Input input)
    {
        long sum = 0;
        return input.Select(Max12).Sum();


        long Max12(int[] batteries)
        {
            int length = 12;
            var prevB = -1; 
            var buffer = 0L;

            var locations = (from b in batteries.Select((b, i) => (b, i))
                            group b.i by b.b into g
                            orderby g.Key descending
                            select (g.Key, Value: g.Order().ToArray())).ToArray();

            for (int i = 0; i < length; i++)
            {
                    var location = locations
                        .First(l => l.Value.Any(ll => ll > prevB && batteries.Length - ll >= length - i));

                    buffer = buffer * 10 + location.Key;

                    prevB = location.Value.First(l => l > prevB);
            }

            return buffer;
        }
    }
}