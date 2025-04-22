using System.Collections.Immutable;
using Tools;

namespace Days.Y2022.Day19;

using Material = KeyValuePair<string, int>;

file class Solution : ISolution<Blueprint[]>
{
    public async Task<Blueprint[]> LoadInput()
    {
        var lines = await File.ReadAllLinesAsync(this.GetInputFile("input"));
        return (from l in lines
                select l.Split(": ") switch
                {
                [var header, var body] => new Blueprint
                (
                 int.Parse(header.Split(" ")[1]),
                 (from sentence in body.Split(". ")
                  select sentence.Trim('.', ' ').Split(' ') switch
                  {
                  ["Each", var robotMaterial, "robot", "costs", var cost, var material] => new Instruction(robotMaterial, new Material[] { new(material, int.Parse(cost)) }),
                  ["Each", var robotMaterial, "robot", "costs", var costA, var materialA, "and", var costB, var materialB] => new Instruction(robotMaterial, new Material[] { new(materialA, int.Parse(costA)), new(materialB, int.Parse(costB)) }),
                  }).ToArray()
                 )

                }).ToArray();
    }

    public object Part1(Blueprint[] blueprints)
    {
        var builder = ImmutableDictionary.CreateBuilder<string, int>();
        builder.Add("ore", 0);
        builder.Add("clay", 0);
        builder.Add("obsidian", 0);
        builder.Add("geode", 0);
        var empty = builder.ToImmutable();

        return blueprints.Sum(b => b.Id * GetMaxGeodesTable(b, new(empty.SetItem("ore", 1), empty, 0), 24, 100000));
    }

    public object Part2(Blueprint[] blueprints)
    {
        var builder = ImmutableDictionary.CreateBuilder<string, int>();
        builder.Add("ore", 0);
        builder.Add("clay", 0);
        builder.Add("obsidian", 0);
        builder.Add("geode", 0);
        var empty = builder.ToImmutable();

        return blueprints.Take(3).Product(b => GetMaxGeodesTable(b, new(empty.SetItem("ore", 1), empty, 0), 32, 200000));
    }

    record State(ImmutableDictionary<string, int> Robots, ImmutableDictionary<string, int> Collected, int Minute)
    {
        public static StateComparer Comparer = new StateComparer();

        public override string ToString()
        {
            var r = string.Join(",", Robots.Select(kv => $"{kv.Key}:{kv.Value}"));
            var c = string.Join(",", Collected.Select(kv => $"{kv.Key}:{kv.Value}"));

            return $"{Minute} - ({r})[{c}]";
        }

        public class StateComparer : IEqualityComparer<State>
        {
            bool IEqualityComparer<State>.Equals(State? x, State? y)
            {
                return x.ToString() == y.ToString();
            }

            int IEqualityComparer<State>.GetHashCode(State obj)
            {
                return obj.ToString().GetHashCode();
            }
        }
    }

    private int GetMaxGeodesTable(Blueprint blueprint, State start, int depth, int toExplore)
    {
        var unexplored = Enumerable.Repeat(0, depth + 1).Select(_ => new PriorityQueue<State, int>()).ToArray();

        unexplored[0].Enqueue(start, 0);

        for (int i = 0; i < depth; i++)
        {
            var current = unexplored[i];
            var next = unexplored[i + 1];
            var e = toExplore;
            while (current.Count > 0 && e-- > 0)
            {
                var (robots, collected, minute) = current.Dequeue();

                var collectedBuilder = collected.ToBuilder();

                foreach (var robot in robots)
                {
                    collectedBuilder[robot.Key] = collected[robot.Key] + robot.Value;
                }
                var updated = collectedBuilder.ToImmutable();

                var futures = new List<(State, int)>();

                foreach (var instruction in blueprint.Sentences.Where(i => i.Materials.All(m => m.Value <= collected[m.Key])))
                {
                    var nextRobots = robots.ToBuilder();
                    var nextCollected = updated.ToBuilder();

                    nextRobots[instruction.Type] = nextRobots[instruction.Type] + 1;

                    foreach (var m in instruction.Materials)
                    {
                        nextCollected[m.Key] -= m.Value;
                    }
                    var n = new State(nextRobots.ToImmutable(), nextCollected.ToImmutable(), minute + 1);
                    futures.Add((n, Score(n, blueprint)));
                }
                {
                    var n = new State(robots, updated, minute + 1);
                    futures.Add((n, Score(n, blueprint)));
                }

                foreach (var (f, score) in futures.OrderByDescending(ff => ff.Item2))
                {
                    next.Enqueue(f, -score);
                }
            }
            unexplored[i] = null;
        }
        var result = unexplored[^1].UnorderedItems.Max(s => s.Element.Collected["geode"]);
        Console.WriteLine($"{blueprint.Id} * {result} = {blueprint.Id * result}");
        return result;
    }

    Dictionary<string, int> weights = new Dictionary<string, int>
    {
        ["ore"] = 1,
        ["clay"] = 1,
        ["obsidian"] = 100,
        ["geode"] = 100000
    };

    private int Score(State next, Blueprint blueprint)
    {
        var (robots, collected, minute) = next;
        var score = robots.Max(c => c.Value * weights[c.Key]);

        return score;
    }

}

file record Blueprint(int Id, Instruction[] Sentences);
file record Instruction(string Type, Material[] Materials);