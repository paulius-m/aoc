using static MoreLinq.Extensions.BatchExtension;
using Tools;

namespace Days.Y2022.Day03;

file class Solution : ISolution<Rucksack[]>
{
    public async Task<Rucksack[]> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .Select(l => new Rucksack(new(l), new(l[..(l.Length / 2)]), new(l[(l.Length / 2)..])))
            .ToArray();
    }

    public object Part1(Rucksack[] i)
    {
        return i.Sum(r => r.Left.Intersect(r.Right).Sum(RucksackItem.ToPriority));
    }

    public object Part2(Rucksack[] i)
    {
        return i.Batch(3)
            .Select(IntersectBatch)
            .Sum(b => b.Sum(RucksackItem.ToPriority));

        static HashSet<char> IntersectBatch(IEnumerable<Rucksack> b)
        {
            return b
                .Select(r => r.Sack)
                .Aggregate((a, c) => a.Intersect(c).ToHashSet());
        }
    }
}

file record Rucksack(HashSet<char> Sack, HashSet<char> Left, HashSet<char> Right);

file class RucksackItem
{
    public static int ToPriority(char item)
    {
        return char.IsLower(item) ? item - 'a' + 1
            : item - 'A' + 27;
    }
}