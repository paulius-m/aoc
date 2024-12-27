using MoreLinq;
using Sprache;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Cryptography.X509Certificates;
using Tools;

namespace Days.Y2024.Day23;
using Input = Dictionary<string, HashSet<string>>;


file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                let pair = l.Split('-')
                from p in new[] { (a: pair[0], b: pair[1]), (a: pair[1], b: pair[0]) }
                group p.b by p.a into g
                select KeyValuePair.Create(g.Key, g.ToHashSet())).ToDictionary();
    }

    public object Part1(Input input)
    {
        var loops = new HashSet<HashSet<string>>(HashSet<string>.CreateSetComparer());

        foreach (var (start, next) in input)
        {
            if (start.StartsWith('t'))
                foreach (var n1 in next)
                {
                    foreach (var n2 in input[n1])
                    {
                        if (input[n2].Contains(start))
                        {
                            loops.Add(new HashSet<string>([start, n1, n2]));
                        }
                    }
                }
        }

        foreach (var kv in loops)
        {
            Console.WriteLine(string.Join(',', kv.Order()));
        }

        return loops.Count;
    }

    public object Part2(Input input)
    {
        return BronKerbosch(input).MaxBy(c => c.Count)!.Order().ToDelimitedString(",");
    }

    public IEnumerable<HashSet<string>> BronKerbosch(Input graph)
    {
        var cliques = new List<HashSet<string>>();
        var empty = ImmutableHashSet<string>.Empty;
        BronKerbosch1(empty, graph.Keys.ToImmutableHashSet(), empty);

        return cliques;

        void BronKerbosch1(ImmutableHashSet<string> r, ImmutableHashSet<string> p, ImmutableHashSet<string> x)
        {
            if (p.Count is 0 && x.Count is 0)
            {
                cliques.Add(r.ToHashSet());
            }

            foreach (var v in p)
            {
                BronKerbosch1(r.Add(v), p.Intersect(graph[v]), x.Intersect(graph[v]));
                p = p.Remove(v);
                x = x.Add(v);
            }
        }
    }
};