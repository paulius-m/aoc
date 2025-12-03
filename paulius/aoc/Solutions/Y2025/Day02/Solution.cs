using Tools;
using Input = System.Collections.Generic.IEnumerable<(long, long)>;

namespace Days.Y2025.Day02;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var input = from l in (await File.ReadAllTextAsync(this.GetInputFile("input"))).Split(',')
                    select l.Split('-') switch
                    {
                        [var f, var to] => (long.Parse(f), long.Parse(to))
                    };
                    
        return input.ToArray();
    }

    public object Part1(Input input)
    {
        long sum = 0;
        
        foreach (var (f, t) in input)
        {
            foreach (var id in GetInvalidIds(f, t))
            {
                Console.WriteLine(id);
                sum += id;
            }
        }
        return sum;
    }

    Dictionary<(long, long), long[]> cache = new Dictionary<(long, long), long[]>();
    IEnumerable<long> GetInvalidIds(long f, long t)
    {
        long split = 2;
        var fNumCount = (long)Math.Log10(f) + 1;
        var tNumCount = (long)Math.Log10(t) + 1;

        for (var i = fNumCount; i <= tNumCount; i++)
        {
            if (i % split is 1) continue;

            var halfI = i / split;

            if (!cache.TryGetValue((i, split), out var cached))
            {
                var min = (long)Math.Pow(10, halfI - 1);
                var max = (long)Math.Pow(10, halfI);
                var toCache = new List<long>();
                for (var j = min; j < max; j++)
                {
                    toCache.Add(j * max + j);
                }
                cache[(i, split)] = cached = toCache.ToArray();
            }

            for (var j = 0; j < cached.Length; j++)
            {
                if (cached[j] >= f && cached[j] <= t)
                {
                    yield return cached[j];
                }
            }
        }
    }

    IEnumerable<long> GetInvalidIdsMultiSplit(long f, long t)
    {
        var fNumCount = (long)Math.Log10(f) + 1;
        var tNumCount = (long)Math.Log10(t) + 1;

        for (var i = fNumCount; i <= tNumCount; i++)
        {
            for (var s = 2; s <= i; s++)
            {
                if (i % s is 1) continue;

                var halfI = i / s;

                if (!cache.TryGetValue((i, s), out var cached))
                {
                    var min = (long)Math.Pow(10, halfI - 1);
                    var max = (long)Math.Pow(10, halfI);
                    var toCache = new HashSet<long>();

                    for (var j = min; j < max; j++)
                    {
                        long id = 0;
                        for (var jj = 1; jj <= s; jj++) {
                            id = id * max + j;
                        }
                        toCache.Add(id);
                    }

                    cache[(i, s)] = cached = toCache.ToArray();
                }

                for (var j = 0; j < cached.Length; j++)
                {
                    if (cached[j] >= f && cached[j] <= t)
                    {
                        yield return cached[j];
                    }
                }
            }
        }
    }

    public object Part2(Input input)
    {
        long sum = 0;

        foreach (var (f, t) in input)
        {
            foreach (var id in GetInvalidIdsMultiSplit(f, t).Distinct())
            {
                Console.WriteLine(id);
                sum += id;
            }
        }
        return sum;
    }
}