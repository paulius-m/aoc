using Tools;

namespace Days.Y2019.Day06;
using Input = List<(string, string)>;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var input = await File.ReadAllLinesAsync(this.GetInputFile("input"));

        var allOrbits = (from s in input
                         select s.Split(')') switch { var ss => (ss[0], ss[1]) }).ToList();
        return allOrbits;
    }

    public object Part1(Input allOrbits)
    {
        var (ranks, orbiting) = Calculate(allOrbits);

        var checksum = ranks.Select(kv => kv.Value).Sum();
        return checksum;

    }

    private static (Dictionary<string, int>, Dictionary<string, string>) Calculate(Input allOrbits)
    {
        var ranks = new Dictionary<string, int>
            {
                { "COM", 0 }
            };

        var orbiting = new Dictionary<string, string>();

        var que = new Queue<string>();
        que.Enqueue("COM");

        while (que.Count > 0)
        {
            var center = que.Dequeue();

            var orbits = allOrbits.Where(ss => ss.Item1 == center).ToList();

            foreach (var (from, to) in orbits)
            {
                ranks[to] = ranks[from] + 1;
                orbiting[to] = from;
                que.Enqueue(to);
            }
        }

        return (ranks, orbiting);
    }

    public object Part2(Input allOrbits)
    {
        var (ranks, orbiting) = Calculate(allOrbits);

        var youPath = GetOrbitPath(orbiting, "YOU");
        var sanPath = GetOrbitPath(orbiting, "SAN");

        var closestOrbit = youPath.Zip(sanPath, Tuple.Create).Reverse().First(a => a.Item1 == a.Item2).Item1;

        var orbitalJumps = ranks["SAN"] + ranks["YOU"] - 2 * ranks[closestOrbit] - 2;

        return orbitalJumps;
    }

    private static List<string> GetOrbitPath(Dictionary<string, string> orbiting, string from)
    {
        var orbitList = new List<string>();
        while (from != "COM")
        {
            orbitList.Add(from);
            from = orbiting[from];
        }
        orbitList.Reverse();
        return orbitList;
    }
}