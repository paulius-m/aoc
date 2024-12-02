namespace Days.Y2023.Day08;

using System.Text.RegularExpressions;
using Tools;
using Input = (string Instructions, Dictionary<string, string> Network, string[] Nodes);

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var lines = await File.ReadAllLinesAsync(this.GetInputFile("test"));
        var r = new Regex(@"(?<item1>\w+) = \((?<item2>\w+), (?<item3>\w+)\)");

        var instructions = lines[0];
        var nodes = lines.Skip(2)
            .Select(r.Match<(string, string, string)>).ToArray();
        var network = nodes
            .SelectMany(m => new[] { (m.Item1 + "-L", m.Item2), (m.Item1 + "-R", m.Item3) })
            .ToDictionary(p => p.Item1, p => p.Item2);

        return (instructions, network, nodes.Select(n => n.Item1).ToArray());
    }

    public object Part1(Input input)
    {
        var (instructions, network, _) = input;

        if (!network.ContainsKey("AAA-R") || !network.ContainsValue("ZZZ"))
        {
            return 0;
        }
        for (var (step, node) = (0, "AAA"); true; step++)
        {
            var i = instructions[step % instructions.Length];
            var child = $"{node}-{i}";
            if (network[child] is var c && c is "ZZZ")
            {
                return step + 1;
            }
            else
            {
                node = c;
            }
        }
    }

    public object Part2(Input input)
    {
        var (instructions, network, allNodes) = input;
        var nodes = allNodes.Where(n => n is [.., 'A']).ToArray();
        var cycles = new Dictionary<string, long>();
        for (var step = 0; true; step++)
        {
            var instruction = instructions[step % instructions.Length];

            for (int i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                var child = $"{node}-{instruction}";
                nodes[i] = network[child];
            }
            foreach (var node in nodes.Where(n => n is [.., 'Z'] && !cycles.ContainsKey(n)))
            {
                Console.WriteLine($"{node} {step + 1}");
                cycles[node] = step + 1;
            }
            if (cycles.Count == nodes.Length)
            {
                return cycles.Values.LeastCommonMultiple();
            }
        }
    }
}

