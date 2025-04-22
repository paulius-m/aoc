using static MoreLinq.Extensions.SplitExtension;
using static MoreLinq.Extensions.ZipLongestExtension;
using Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Days.Y2022.Day13;

file class Solution : ISolution<object[][]>, IComparer<object>
{
    public async Task<object[][]> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .Split("")
            .Select(b => b.Select(l => JsonConvert.DeserializeObject(l)!).ToArray())
            .ToArray();
    }

    public object Part1(object[][] input)
    {
        var pairs = input.Select((pair, i) => (Compare(pair[0], pair[1]), i)).Where(pair => pair.Item1 <= 0).ToList();
        return pairs.Sum(pair => pair.i + 1);
    }

    public int Compare(object? v1, object? v2)
    {
        var result = (v1, v2) switch
        {
            (_, null) => 1,
            (null, _) => -1,
            (JValue a, JValue b) => a.Value<int>() - b.Value<int>(),
            (JValue a, JArray b) => new JArray(a).ZipLongest(b, Compare).FirstOrDefault(d => d != 0),
            (JArray a, JValue b) => a.ZipLongest(new JArray(b), Compare).FirstOrDefault(d => d != 0),
            (JArray a, JArray b) => a.ZipLongest(b, Compare).FirstOrDefault(d => d != 0),
        };
        return result;
    }

    public object Part2(object[][] input)
    {
        object divider2 = JsonConvert.DeserializeObject("[[2]]")!;
        object divider6 = JsonConvert.DeserializeObject("[[6]]")!;

        var ordered = input.SelectMany(i => i).Append(divider2).Append(divider6).Order(this).ToArray();

        return (Array.IndexOf(ordered, divider2) + 1) * (Array.IndexOf(ordered, divider6) + 1);
    }
}