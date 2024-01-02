using Tools;

namespace Days.Day25;
using Input = Dictionary<string, HashSet<string>>;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (from l in await File.ReadAllLinesAsync(this.GetInputFile("test"))
               select l.Split(": ") into e
               select KeyValuePair.Create(e[0], e[1].Split(' ').ToHashSet())).ToDictionary();
    }

    public object Part1(Input graph)
    {
        var edges = from u in graph
                    from v in u.Value

                    select (u: u.Key, v); 


        return 0;
    }

    public object Part2(Input i)
    {
        return 0;
    }
}