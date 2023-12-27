using Tools;


namespace Days.Day22;

using Input = Brick[];
record Brick : AABB
{
    public bool IsStable { get; set; }
}

file class Solution : ISolution<Input>
{

    public async Task<Input> LoadInput()
    {
        return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
               select l.Split('~').SelectArray(s => s.Split(',').SelectArray(long.Parse)) into ab
               select new Brick { Ranges = ab[0].Zip(ab[1], (a, b) => new CoordRange(Math.Min(a, b), Math.Max(a, b))).ToArray() }
               ).ToArray();
    }

    public object Part1(Input parts)
    {
        Fall(parts);

        var iparts = new Dictionary<int, Brick>(parts.Select((p, i) => KeyValuePair.Create(i, p)));
        Dictionary<int, int[]> held = new();

        var wrap2 = new KeyValuePair<int, Brick>[1];

        foreach (var ip in iparts)
        {
            wrap2[0] = ip;
            held[ip.Key] = iparts.Except(wrap2).Where(
                pp => pp.Value.Ranges[2].From == ip.Value.Ranges[2].To + 1 &&
                pp.Value.Ranges[0].Overlaps(ip.Value.Ranges[0]) &&
                pp.Value.Ranges[1].Overlaps(ip.Value.Ranges[1])
                ).Select(pp => pp.Key).ToArray();
        }

        var toRemove = held.Where(ip => ip.Value.All(v => held.Except([ip]).Any(pp => pp.Value.Contains(v)))).ToArray();
        return toRemove.Count();
    }

    private static void Fall(Brick[] parts)
    {
        var xyOverlaps = parts.Select((p, i) =>
            KeyValuePair.Create(i, parts.Where(pp => pp != p && pp.Ranges[0].Overlaps(p.Ranges[0]) && pp.Ranges[1].Overlaps(p.Ranges[1])).ToArray()))
            .ToDictionary();
        var falling = true;

        while (falling)
        {
            falling = false;
            var fallCount = 0;
            for (int i = 0; i < parts.Length; i++)
            {
                var p = parts[i];
                
                if (p.Ranges[2].From - 1 == 0 || p.IsStable)
                {
                    p.IsStable = true;
                    continue;
                }

                var t = p.Ranges[2];
                t = new() { From = t.From - 1, To = t.To - 1 };

                var overlaps = xyOverlaps[i].Where(pp => pp.Ranges[2].Overlaps(t)).ToArray();
                if (overlaps.Length > 0)
                {
                    p.IsStable = overlaps.Any(b => b.IsStable);
                }
                else
                {
                    p.Ranges[2] = t;
                    fallCount++;
                    falling = true;
                }
            }
        }

    }

    public object Part2(Input parts)
    {
        Fall(parts);

        var iparts = new Dictionary<int, Brick>(parts.Select((p, i) => KeyValuePair.Create(i, p)));
        Dictionary<int, int[]> holds = new();

        var wrap2 = new KeyValuePair<int, Brick>[1];

        foreach (var ip in iparts)
        {
            wrap2[0] = ip;
            holds[ip.Key] = iparts.Except(wrap2).Where(
                pp => pp.Value.Ranges[2].From == ip.Value.Ranges[2].To + 1 &&
                pp.Value.Ranges[0].Overlaps(ip.Value.Ranges[0]) &&
                pp.Value.Ranges[1].Overlaps(ip.Value.Ranges[1])
                ).Select(pp => pp.Key).ToArray();
        }
        var totalFall = 0;
        foreach(var ip in iparts)
        {
            totalFall += GetFalls(ip.Key, holds);
        }

        return totalFall;
    }

    private int GetFalls(int key, Dictionary<int, int[]> holds)
    {
        var order = new HashSet<int>();
        var s = new Queue<int>();
        s.Enqueue(key);

        while (s.Count > 0)
        {
            var c = s.Dequeue();
            order.Add(c);
            foreach (var n in holds[c])
            {
                //held.Except([ip]).Any(pp => pp.Value.Contains(v)))
                if (!holds.Where( h => !order.Contains(h.Key)).Any( h => h.Value.Contains(n)))
                {
                    s.Enqueue(n);
                }
            }
        }
        return order.Count - 1;
    }
}