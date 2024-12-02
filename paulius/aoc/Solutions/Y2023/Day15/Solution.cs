using Tools;

namespace Days.Y2023.Day15;
using Input = string[];

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return await File.ReadAllLinesAsync(this.GetInputFile("input"));
    }

    public object Part1(Input i)
    {
        return i.Single().Split(',').Select(Hash).Sum();
    }

    public object Part2(Input i)
    {
        Dictionary<long, List<(string lbl, long num)>> hashMap = new();
        foreach (var op in i.Single().Split(','))
        {
            if (op.Contains('='))
            {
                var t = op.Split('=') switch { [var lbl, var num] => (lbl, long.Parse(num)) };
                var hash = Hash(t.lbl);
                if (hashMap.TryGetValue(hash, out var list))
                {
                    if (list.FindIndex(0, m => m.lbl == t.lbl) is var ix && ix is -1)
                    {
                        list.Add(t);
                    }
                    else
                    {
                        list[ix] = t;
                    }
                }
                else
                {
                    hashMap[hash] = new() { t };
                }
            }
            else
            {
                var lbl = op.Split('-')[0];
                var hash = Hash(lbl);

                if (hashMap.TryGetValue(hash, out var list) && list.FindIndex(0, m => m.lbl == lbl) is var ix && ix > -1)
                {
                    list.RemoveAt(ix);
                }
            }
        }

        return hashMap.Where(kv => kv.Value.Count > 0).Sum(kv => kv.Value.Select((v, ix) => (kv.Key + 1) * v.num * (ix + 1)).Sum());

    }

    private long Hash(string input)
    {
        return input.Aggregate(0L, (a, b) => (a + b) * 17 % 256);
    }
}