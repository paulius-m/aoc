using Tools;

namespace Days.Y2019.Day08;
using Input = List<string>;

file class Solution : ISolution<Input>
{
    static int h = 6;
    static int w = 25;
    static int hw = h * w;

    public async Task<Input> LoadInput()
    {
        var input = await File.ReadAllTextAsync(this.GetInputFile("input"));


        var layers = new Input();
        for (int i = 0; i < input.Length; i += hw)
        {
            layers.Add(input[i..(i + hw)]);
        }
        return layers;
    }

    public object Part1(Input layers)
    {
        var maxZeros = hw;
        var mult21 = 0;

        foreach (var layer in layers)
        {
            var counts = layer.GroupBy(p => p).ToDictionary(g => g.Key, g => g.Count());
            if (counts['0'] < maxZeros)
            {
                maxZeros = counts['0'];
                mult21 = counts['1'] * counts['2'];
            }
        }

        return $"{maxZeros}, {mult21}";
    }

    public object Part2(Input layers)
    {
        var image = layers.Aggregate((l1, l2) => string.Join("", l1.Zip(l2, (p1, p2) => (p1, p2) switch
        {
            ('2', var p) => p,
            (var p, _) => p
        })));

        for (int i = 0; i < hw; i += w)
        {
            Console.WriteLine(image[i..(i + w)].Replace('0', ' ').Replace('1', '#'));
        }
        return "";
    }
}