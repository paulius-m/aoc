using Sprache;
using Tools;

namespace Days.Y2023.Day20;

using ModuleConfig = (char, string, string[]);
using Input = (char, string, string[])[];
file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                select ModuleConfig.Parse(l)).ToArray();
    }

    public object Part1(Input config)
    {
        Dictionary<string, Module> modules = BuildModules(config);

        var stats = new Dictionary<Pulse, long> { [Pulse.L] = 0, [Pulse.H] = 0 };
        for (var i = 0; i < 1000; i++)
        {
            var q = new Queue<string>();
            q.Enqueue("broadcaster");
            modules["broadcaster"].SetInputValue("button", Pulse.L);
            stats[Pulse.L] += 1;
            while (q.Count > 0)
            {
                var c = q.Dequeue();
                var current = modules[c];

                foreach (var o in current.Process())
                {
                    // Console.WriteLine($"{c} -{o.Value}-> {o.Name}");

                    stats[o.Value.Value] += 1;
                    modules[o.Name].SetInputValue(c, o.Value);
                    q.Enqueue(o.Name);
                }
            }
        }

        return stats.Values.Product(f => f);
    }

    public object Part2(Input config)
    {
        Dictionary<string, Module> modules = BuildModules(config);
        var cycles = new Dictionary<string, long>();
        //Render(modules);

        for (var i = 1; true; i++)
        {
            var q = new Queue<string>();

            q.Enqueue("broadcaster");
            modules["broadcaster"].SetInputValue("button", Pulse.L);
            while (q.Count > 0)
            {
                var c = q.Dequeue();
                var current = modules[c];

                if (c is "pp" or "sj" or "rg" or "zp" && current.Inputs.All(f => f.Value is Pulse.H))
                {
                    if (!cycles.ContainsKey(c))
                    {
                        cycles[c] = i;
                    }
                    if (cycles.Count == 4)
                    {
                        return cycles.Values.Product(f => f);
                    }
                }

                foreach (var o in current.Process())
                {
                    if (o.Name is "rx" && o.Value is Pulse.L)
                    {
                        return i;
                    }
                    modules[o.Name].SetInputValue(c, o.Value);
                    q.Enqueue(o.Name);
                }
            }
        }
    }

    private static void Render(Dictionary<string, Module> modules)
    {
        var visited = new HashSet<string>();
        var q = new Queue<string>();
        q.Enqueue("broadcaster");

        while (q.Count > 0)
        {
            var c = q.Dequeue();
            var current = modules[c];
            if (visited.Contains(c)) continue;
            visited.Add(c);
            foreach (var o in current.Outputs)
            {
                Console.WriteLine($"{c}:::{current.GetType().Name} --> {o.Name}");
                if (!visited.Contains(o.Name))
                    q.Enqueue(o.Name);
            }

        }

    }

    private Dictionary<string, Module> BuildModules((char, string, string[])[] config)
    {
        Dictionary<string, Module> modules = new(config.Select(i => KeyValuePair.Create(i.Item2, BuildModule(i.Item1, i.Item3))));

        foreach (var i in config)
        {
            foreach (var o in i.Item3)
            {
                if (!modules.ContainsKey(o))
                {
                    modules[o] = new Silent();
                }
                modules[o].Inputs.Add(new(i.Item2));
            }
        }
        modules["broadcaster"].Inputs.Add(new("button"));


        var order = new List<string>();
        var s = new Queue<string>();
        s.Enqueue("broadcaster");

        while (s.Count > 0)
        {
            var c = s.Dequeue();
            order.Add(c);
            foreach (var n in modules[c].Outputs)
            {
                if (modules[n.Name] is FlipFlop && modules[n.Name].Inputs.Where(i => modules[i.Name] is FlipFlop).All(i => order.Contains(i.Name)))
                {
                    s.Enqueue(n.Name);
                }
            }
        }

        foreach (var m in modules.Values)
        {
            m.Inputs.Sort((a, b) => order.IndexOf(b.Name) - order.IndexOf(a.Name));
        }

        return modules;
    }

    private Module BuildModule(char type, string[] outputs)
    {
        Module module = type switch
        {
            '%' => new FlipFlop(),
            '&' => new Conjunction(),
            _ => new Broadcast()
        };

        foreach (var o in outputs)
            module.Outputs.Add(new(o));

        return module;
    }

    static Parser<ModuleConfig> ModuleConfig = from type in Parse.Chars('%', '&').Optional()
                                               from name in ParseEx.Word
                                               from _ in Parse.String("->").Token()
                                               from destinations in ParseEx.Word.Token().DelimitedBy(Parse.Char(','))
                                               select (type.GetOrDefault(), name, destinations.ToArray());
}

enum Pulse
{
    H, L
}

class Wire(string name)
{
    public string Name { get; set; } = name;
    public Pulse? Value { get; set; }

    public Pulse? Read()
    {
        var v = Value;
        Value = null;
        return v;
    }
}

abstract class Module
{
    public List<Wire> Inputs = new();
    public List<Wire> Outputs = new();
    protected List<Wire> Empty = new List<Wire>();
    public abstract List<Wire> Process();

    internal void SetInputValue(string c, Pulse? value)
    {
        var i = Inputs.First(i => i.Name == c);
        i.Value = value;
    }
}

class FlipFlop : Module
{
    public bool IsOn;

    public override List<Wire> Process()
    {
        if (!Inputs.Select(i => i.Read()).Any(p => p is Pulse.L))
        {
            return Empty;
        }

        Outputs.ForEach(o => o.Value = IsOn ? Pulse.L : Pulse.H);
        IsOn = !IsOn;
        return Outputs;
    }
}

class Conjunction : Module
{
    public override List<Wire> Process()
    {
        if (Inputs.Select(i => i.Value ?? Pulse.L).All(p => p is Pulse.H))
        {
            Outputs.ForEach(o => o.Value = Pulse.L);
        }
        else
        {
            Outputs.ForEach(o => o.Value = Pulse.H);
        }
        return Outputs;
    }
}

class Broadcast : Module
{
    public override List<Wire> Process()
    {
        var input = Inputs.Select(i => i.Read()).Where(p => p is not null).SingleOrDefault();
        if (input is not null)
        {
            Outputs.ForEach(o => o.Value = input);
            return Outputs;
        }
        else
        {
            return Empty;
        }
    }
}

class Silent : Module
{
    public override List<Wire> Process()
    {
        return Empty;
    }
}