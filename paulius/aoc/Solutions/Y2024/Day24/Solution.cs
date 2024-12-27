using MoreLinq;
using Sprache;
using System.Collections.Immutable;
using System.Security.Cryptography;
using Tools;

namespace Days.Y2024.Day24;
using Input = (InputWire[], InputGate[]);

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return Parsers.Input.Parse(await File.ReadAllTextAsync(this.GetInputFile("input")));
    }

    public object Part1(Input t) => 0;
    public async Task<object> Part1Async(Input input)
    {
        var (wires, gates) = input;
        Dictionary<string, Wire> bus = await RunBus(wires, gates);

        var values = await Task.WhenAll(from kv in bus
                                        where kv.Key.StartsWith('z')
                                        orderby kv.Key descending
                                        select kv.Value.Value);

        return values.Aggregate(0L, (a, b) => (a << 1) + b);
    }

    private static async Task<Dictionary<string, Wire>> RunBus(InputWire[] wires, InputGate[] gates)
    {
        var bus = new Dictionary<string, Wire>();
        foreach (var w in wires)
        {
            bus.Add(w.id, new Wire(w.value));
        }

        foreach (var w in gates.SelectMany(g => g.Wires))
        {
            if (!bus.ContainsKey(w))
            {
                bus.Add(w, new Wire());
            }
        }

        await Task.WhenAll(from g in gates
                           select new Gate(bus[g.lId], g.op, bus[g.rId], bus[g.resultId]) into ng
                           select ng.Write());
        return bus;
    }

    public object Part2(Input input)
    {
        var (wires, gates) = input;

        var zWires = gates.Select(g => g.resultId).Where(g => g.StartsWith('z')).Order().ToArray();
        var gateDict = gates.ToDictionary(g => g.resultId);

        string[][] fixes = [["z18", "wss"], ["z23", "bmn"],["rds", "jss"], ["mvb", "z08"]];

        foreach (var fix in fixes)
        {
            var s = fix[0];
            var d = fix[1];

            (gateDict[s], gateDict[d]) = (gateDict[d] with { resultId = s }, gateDict[s] with { resultId = d });
        }
        var badGates = new HashSet<InputGate>();

        var adders = new HashSet<InputGate>[zWires.Length];
        {
            var visited = new HashSet<InputGate>();
            for (int i = 0; i < zWires.Length; i++)
            {
                var q = new Queue<string>();
                var path = new HashSet<InputGate>();
                q.Enqueue(zWires[i]);

                while (q.Count > 0)
                {
                    var w = q.Dequeue();
                    if (!gateDict.TryGetValue(w, out var next))
                    {
                        continue;
                    }

                    if (visited.Contains(next))
                    {
                        continue;
                    }

                    path.Add(next);
                    visited.Add(next);
                    q.Enqueue(next.lId);
                    q.Enqueue(next.rId);
                }

                adders[i] = path;
            }
        }

        var badAdders = new HashSet<HashSet<InputGate>>(HashSet<InputGate>.CreateSetComparer());

        for (int i = 3; i < adders.Length - 1; i++)
        {
            var visited = new HashSet<InputGate>();
            var testPath = GetGatePath(WireId("x", i), WireId("z", i), adders[i]);
            if (!ValidPath(testPath, [Op.XOR, Op.XOR]))
            {
                badAdders.Add(adders[i]);
            }
            foreach (var t in testPath)
            {
                visited.Add(t);
            }

            testPath = GetGatePath(WireId("y", i), WireId("z", i), adders[i]);
            if (!ValidPath(testPath, [Op.XOR, Op.XOR]))
            {
                badAdders.Add(adders[i]);
            }
            foreach (var t in testPath)
            {
                visited.Add(t);
            }
            testPath = GetGatePath(WireId("x", i - 1), WireId("z", i), adders[i]);
            if (!ValidPath(testPath, [Op.XOR, Op.OR, Op.AND]))
            {
                badAdders.Add(adders[i]);
            }
            foreach (var t in testPath)
            {
                visited.Add(t);
            }
            testPath = GetGatePath(WireId("y", i - 1), WireId("z", i), adders[i]);
            if (!ValidPath(testPath, [Op.XOR, Op.OR, Op.AND]))
            {
                badAdders.Add(adders[i]);
            }
            foreach (var t in testPath)
            {
                visited.Add(t);
            }

            var left = adders[i].Except(visited).ToHashSet();
            if (left.Count is 1 && left.First().op is not Op.AND)
            {
                badAdders.Add(adders[i]);
            }
        }

        OutputMermaid(badAdders.SelectMany(a => a).ToArray(), new());
        //OutputMermaid(adders[9].ToArray(), new());
        return fixes.SelectMany(s =>s).Order().ToDelimitedString(",");

        static bool ValidPath(InputGate[] path, Op[] expected)
        {
            return path.Select(p => p.op).SequenceEqual(expected);
        }

        static string WireId(string v, int i)
        {
            return v + i.ToString().PadLeft(2, '0');
        }
    }

    public static InputGate[] GetGatePath(string id, string zId, HashSet<InputGate> gates)
    {
        var paths = new List<InputGate[]>();

        GetGates(id, zId, ImmutableStack<InputGate>.Empty);

        return paths.Single();

        void GetGates(string id, string zId, ImmutableStack<InputGate> stack)
        {
            if (id == zId)
            {
                paths.Add(stack.ToArray());
            }

            foreach (var g in gates.Where(g => g.DependsOn(id)))
            {
                GetGates(g.resultId, zId, stack.Push(g));
            }
        }
    }

    private static void OutputMermaid(InputGate[] gates, HashSet<string> badWires)
    {
        var ids = new List<string>();
        var transitions = new List<string>();

        foreach (var g in gates)
        {
            var id = g.resultId;

            ids.Add($"{id}: {g.op}");
            transitions.Add($"{g.lId} --> {id}: {g.lId}");
            transitions.Add($"{g.rId} --> {id}: {g.rId}");

            if (g.resultId.StartsWith('z'))
            {
                transitions.Add($"{id} --> [*]: {g.resultId}");
            }
        }

        var f = File.Open("mermaid.txt", FileMode.Truncate);
        var fWriter = new StreamWriter(f);
        fWriter.WriteLine("stateDiagram-v2");
        fWriter.WriteLine("classDef badBadEvent fill:#f00,color:white,font-weight:bold,stroke-width:2px,stroke:yellow");
        foreach (var id in ids)
        {
            fWriter.WriteLine(id);
        }

        foreach (var t in transitions.Order())
        {
            fWriter.WriteLine(t);
        }

        foreach (var id in badWires)
        {
            fWriter.WriteLine($"class {id} badBadEvent");
        }
        fWriter.WriteLine();
        fWriter.Close();
        f.Close();
    }
}

file class Parsers
{
    static Parser<string> Id = from id in Parse.LetterOrDigit.Many().Text()
                               select id;

    static Parser<InputWire> Wire = from id in Id.Then(w => Parse.Char(':').Token().Return(w))
                                    from value in Parse.Digit.Select(d => d - '0')
                                    select new InputWire(id, value);

    static Parser<InputGate> Gate = from lId in Id.Token()
                                    from op in Id.Token()
                                    from rid in Id.Token()
                                    from _ in Parse.String("->").Token()
                                    from resultId in Id
                                    select new InputGate(lId, Enum.Parse<Op>(op), rid, resultId);

    public static Parser<Input> Input = from wires in Wire.DelimitedBy(Parse.LineEnd)
                                        from _ in Parse.LineEnd
                                        from gates in Gate.DelimitedBy(Parse.LineEnd)
                                        select (wires.ToArray(), gates.ToArray());
}

file record InputWire(string id, int value);

file record InputGate(string lId, Op op, string rId, string resultId)
{
    public string[] Wires => [lId, rId, resultId];

    public bool DependsOn(string wireId)
    {
        return lId == wireId || rId == wireId;
    }
}

public class Wire
{
    private TaskCompletionSource<int> _value = new();

    public Wire()
    {
    }

    public Wire(int value)
    {
        SetValue(value);
    }

    public void SetValue(int value)
    {
        _value.SetResult(value);
    }

    public Task<int> Value => _value.Task;
}

file class Gate(Wire left, Op op, Wire right, Wire output)
{
    public async Task Write()
    {
        var l = await left.Value;
        var r = await right.Value;

        var o = op switch
        {
            Op.AND => l & r,
            Op.OR => l | r,
            Op.XOR => l ^ r
        };

        output.SetValue(o);
    }
}

file enum Op
{
    AND,
    OR,
    XOR
}