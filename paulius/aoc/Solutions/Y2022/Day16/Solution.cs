using Sprache;
using Tools;
using System.Diagnostics.CodeAnalysis;

namespace Days.Y2022.Day16;

using Input = Dictionary<string, Valve>;

file class Solution : ISolution<Input>
{

    public async Task<Input> LoadInput()
    {
        return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                select Parser.Valve.Parse(l)).ToDictionary(v => v.Id, v => v);
    }

    public object Part1(Input i)
    {
        //return GetFlow(i, "AA", 0, 0, new());
        return 0;// GetFlow(i, new State("AA", 0, 0, new()), 30, new());
    }

    public object Part2(Input i)
    {
        var first = GetFlow(i, new State("AA", 0, 0, new()), 26, new());
        var second = GetFlow(i, new State("AA", 0, 0, new()), 26, first.Open);
        return first.CurrentFlow + second.CurrentFlow;
    }

    public class QueueComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return y - x;
        }
    }

    record State(string Current, int CurrentMinute, int CurrentFlow, HashSet<string> Open)
    {
        public class Comparer : IEqualityComparer<State>
        {
            public bool Equals(State? x, State? y)
            {
                return x.Current == y.Current
                    && x.CurrentMinute == y.CurrentMinute
                    && x.Open.SetEquals(y.Open);
            }

            public int GetHashCode([DisallowNull] State x)
            {
                return x.Current.GetHashCode() ^ x.CurrentMinute.GetHashCode() ^ x.Open.GetHashCode();
            }
        }
    }

    private static State GetFlow(Input graph, State start, int minutes, HashSet<string> alreadyOpen)
    {
        var visited = new HashSet<State>(new State.Comparer());

        var q = new PriorityQueue<State, int>(new QueueComparer());
        q.Enqueue(start, 0);

        var max = 0;
        State maxState = null;

        while (q.Count > 0)
        {
            var state = q.Dequeue();
            if (visited.Contains(state))
            {
                continue;
            }
            visited.Add(state);

            var (current, currentMinute, currentFlow, open) = state;

            var flow = open.Sum(v => graph[v].Flow);
            var newFlow = currentFlow + flow;
            var priority = currentFlow + flow * (minutes - currentMinute);

            if (currentMinute == minutes)
            {
                if (currentFlow > max)
                {
                    Console.WriteLine($"{currentMinute}: {current} ({string.Join(",", open)}) {currentFlow}");

                    max = currentFlow;
                    maxState = state;
                }
                continue;
            }

            if (graph[current].Flow > 0 && !alreadyOpen.Contains(current) && !open.Contains(current))
            {
                var newOpen = open.Append(current).ToHashSet();
                var nextFlow = graph[current].Flow;
                var newstate = new State(current, currentMinute + 1, newFlow, newOpen);
                if (visited.Contains(newstate))
                {
                    continue;
                }

                q.Enqueue(newstate, priority + nextFlow * (minutes - currentMinute - 1));
            }

            foreach (var next in graph[current].LeadsTo)
            {
                var newstate = new State(next, currentMinute + 1, newFlow, open);
                if (visited.Contains(newstate))
                {
                    continue;
                }

                q.Enqueue(newstate, priority);
            }
        }
        return maxState;
    }
}

file record Valve(string Id, int Flow, string[] LeadsTo);

file static class Parser
{
    private static Parser<string> ValveId = Parse.Letter.Many().Text();

    public static Parser<Valve> Valve =
        from _ in Parse.String("Valve ")
        from valve in ValveId
        from __ in Parse.String(" has flow rate=")
        from flow in Parse.Number.Select(int.Parse)
        from ___ in Parse.String("; tunnel leads to valve ").Or(Parse.String("; tunnels lead to valves "))
        from otherValves in ValveId.DelimitedBy(Parse.String(", "))
        select new Valve(valve, flow, otherValves.ToArray());
}
