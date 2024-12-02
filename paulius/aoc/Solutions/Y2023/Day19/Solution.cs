using MoreLinq;
using Sprache;
using Tools;

namespace Days.Y2023.Day19;
using Part = Dictionary<char, long>;
using PartRange = Dictionary<char, CoordRange>;
using Input = (Dictionary<string, Workflow> flows, Dictionary<char, long>[] parts);

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        Tests();
        var lines = await File.ReadAllLinesAsync(this.GetInputFile("input"));

        var g = lines.Split("").ToArray();

        var flows = g[0].Select(flow.Parse).ToDictionary(f => f.Name, f => f);
        var parts = g[1].Select(part.Parse).ToArray();

        return (flows, parts);
    }

    public object Part1(Input input)
    {
        var (flows, parts) = input;

        var g = from p in parts
                group p by ApplyWorkflow(p, flows, "in");

        return g.First(g => g.Key is "A").Sum(p => p.Values.Sum());
    }

    string ApplyWorkflow(Part p, Dictionary<string, Workflow> flows, string start)
    {
        var c = start;
        while (flows.TryGetValue(c, out var flow))
        {
            c = flow.Process(p);
        }
        return c;
    }

    public object Part2(Input input)
    {
        var (flows, parts) = input;

        Dictionary<string, List<PartRange>> inputs = new()
        {
            ["in"] = [new ()
            {
                ['x'] = new CoordRange(1, 4000),
                ['m'] = new CoordRange(1, 4000),
                ['a'] = new CoordRange(1, 4000),
                ['s'] = new CoordRange(1, 4000)
            }]
        };

        var flowQ = new Queue<string>();
        flowQ.Enqueue("in");

        while (flowQ.Count > 0)
        {
            var name = flowQ.Dequeue();
            var flow = flows[name];
            var ranges = inputs[name];

            foreach (var r in ranges)
            {
                var rr = r;
                foreach (var step in flow.Steps)
                {
                    var (matched, passed) = step.Predicate.CutRange(rr);

                    if (!inputs.ContainsKey(step.Destination))
                    {
                        inputs[step.Destination] = new();
                    }

                    inputs[step.Destination].Add(matched);

                    if (step.Destination is not ("A" or "R"))
                        flowQ.Enqueue(step.Destination);

                    rr = passed;
                    if (rr is null) break;
                }
            }
        }

        var rangeLengths = (from part in inputs["A"]
                            let cat = from c in part select c.Value.To - c.Value.From + 1
                            select cat.ToArray()).ToArray();

        var combinations = (from r in rangeLengths
                            select r.Product(f => f)).ToArray();

        return combinations.Sum();
    }

    static IWorkflowPredicate success = new WorkflowDefaultPredicate(true);

    static Parser<long> number = Parse.Number.Select(long.Parse);

    static Parser<WorkflowPredicate> predicate = from category in Parse.Letter
                                                 from sign in Parse.AnyChar
                                                 from size in number
                                                 select new WorkflowPredicate(category, sign, size);

    static Parser<WorkflowStep> predstep = from p in predicate
                                           from _ in Parse.Char(':')
                                           from destination in Parse.Letter.AtLeastOnce().Text()
                                           select new WorkflowStep(p, destination);

    static Parser<WorkflowStep> sucessStep = from destination in Parse.Letter.AtLeastOnce().Text()
                                             select new WorkflowStep(success, destination);


    static Parser<Workflow> flow = from name in Parse.Letter.Many().Text()
                                   from _ in Parse.Char('{')
                                   from steps in predstep.Or(sucessStep).DelimitedBy(Parse.Char(','))
                                   from __ in Parse.Char('}')
                                   select new Workflow(name, steps.ToArray());

    static Parser<(char, long)> category = from category in Parse.Letter
                                           from _ in Parse.Char('=')
                                           from size in number
                                           select (category, size);

    static Parser<Part> part = from c in category.DelimitedBy(Parse.Char(',')).Contained(Parse.Char('{'), Parse.Char('}'))
                               select c.ToDictionary(c => c.Item1, c => c.Item2);

    public void Tests()
    {
        predicate.Parse("a<2006");
        predstep.Parse("a<2006:qkq");
        predstep.Parse("m>2090:A");
        sucessStep.Parse("rfg");

        flow.Parse("px{a<2006:qkq,m>2090:A,rfg}");
    }
}

file interface IWorkflowPredicate
{
    (PartRange, PartRange?) CutRange(PartRange r);
    bool Evaluate(Part part);
}

file record WorkflowPredicate(char Category, char Sign, long Size) : IWorkflowPredicate
{
    public bool Evaluate(Part part)
    {
        return Sign switch
        {
            '>' => part[Category] > Size,
            '<' => part[Category] < Size
        };
    }

    public (PartRange, PartRange) CutRange(PartRange r)
    {
        var matched = new PartRange();
        var passed = new PartRange();

        foreach (var p in from kv in r
                          select (kv.Key, Cut(kv.Key, kv.Value)))
        {
            matched[p.Key] = EmptyIfReversed(p.Item2.Item1);
            passed[p.Key] = EmptyIfReversed(p.Item2.Item2);
        }

        return (matched, passed);
    }

    (CoordRange, CoordRange?) Cut(char c, CoordRange range)
    {
        if (c == Category)
        {
            return Sign switch
            {
                '>' => (range with { From = Size + 1 }, range with { To = Size }),
                '<' => (range with { To = Size - 1 }, range with { From = Size })
            };
        }
        else
        {
            return (range, range);
        }
    }

    CoordRange EmptyIfReversed(CoordRange r)
    {
        return r.To >= r.From ? r : new CoordRange();
    }


}
file record WorkflowDefaultPredicate(bool Result) : IWorkflowPredicate
{
    public bool Evaluate(Part part)
    {
        return Result;
    }

    public (PartRange, PartRange?) CutRange(PartRange r)
    {
        return (r, null);
    }
}

file record WorkflowStep(IWorkflowPredicate Predicate, string Destination)
{
    public string? Process(Part part)
    {
        return Predicate.Evaluate(part) ? Destination : null;
    }
}

file record Workflow(string Name, WorkflowStep[] Steps)
{
    public string Process(Part part)
    {
        return Steps.Select(s => s.Process(part)).First(s => s != null)!;
    }
}