using Raylib_cs;
using System.Collections.Immutable;
using System.Numerics;
using Tools;

namespace Days.Y2023.Day25;
using Graph = ImmutableDictionary<string, HashSet<string>>;

file class Solution : ISolution<Graph>
{
    public async Task<Graph> LoadInput()
    {
        return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                select l.Split(": ") into e
                select KeyValuePair.Create(e[0], e[1].Split(' ').ToHashSet())).ToImmutableDictionary();
    }

    public object Part1(Graph graph)
    {
        var edges = (from u in graph
                     from v in u.Value
                     from e in new[] { (u: u.Key, v), (u: v, v: u.Key) }
                     group e by e.u into g
                     select KeyValuePair.Create(g.Key, g.Select(e => e.v).Distinct().ToHashSet())).ToImmutableDictionary();
        return ForceSpring(edges);
    }

    public long ForceSpring(Graph graph)
    {
        var r = new Random(0);
        var vertexes = (from k in graph.Keys
                        select KeyValuePair.Create(k, new Vector2((float)r.NextDouble() * 100, (float)r.NextDouble() * 100))
                       ).ToDictionary();

        Render.InitWindow();
        while (!Raylib.WindowShouldClose())
        {
            Render.Draw(vertexes, graph);
            vertexes = Update(vertexes, graph);
        }
        Render.CloseWindow();

        var eges = (
            from kv in graph
            let u = kv.Key
            from v in kv.Value
            select (U: u, V: v, Length: (vertexes[u] - vertexes[v]).Length()) into e
            orderby e.Length descending
            select (e.U, e.V)).Take(6).ToHashSet();

        return GetPartitionSizes(graph, eges).Product();
    }


    private static Dictionary<string, Vector2> Update(Dictionary<string, Vector2> vertexes, Graph graph)
    {   
        var center = vertexes.Values.Aggregate((a, b) => a + b) / vertexes.Count;

        var updated = new Dictionary<string, Vector2>();
        foreach (var vertex in vertexes)
        {
            var target = graph[vertex.Key].Select(v => vertexes[v]).Aggregate((a, b) => a + b) / graph[vertex.Key].Count;
            var attraction = target - vertexes[vertex.Key];
            var repultion = center - vertexes[vertex.Key];

            updated[vertex.Key] = vertexes[vertex.Key] + attraction * 0.1f * (float)Math.Log(attraction.Length()) - repultion / (float)Math.Pow(repultion.Length(), 2);
        }

        return updated;
    }

    public long[] GetPartitionSizes(Graph graph, HashSet<(string U, string V)> excluded)
    {
        var q = new Queue<string>();
        q.Enqueue(graph.Keys.First());
        var visited = new HashSet<string>();

        while (q.Count > 0)
        {
            var current = q.Dequeue();
            if (visited.Contains(current))
                continue;

            visited.Add(current);

            foreach(var next in graph[current])
            {
                if (excluded.Contains((current, next)) == false)
                    q.Enqueue(next);
            }

        }

        return [visited.Count, graph.Count - visited.Count];
    }

    public object Part2(Graph i)
    {
        return 0;
    }
}
