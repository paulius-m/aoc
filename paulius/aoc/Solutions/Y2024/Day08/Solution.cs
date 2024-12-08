using MoreLinq;
using Sprache;
using Tools;
using Input = Tools.Grid<char>;

namespace Days.Y2024.Day08
{
    file class Solution : ISolution<Input>
    {
        public async Task<Input> LoadInput()
        {
            return new Input((await File.ReadAllLinesAsync(this.GetInputFile("input"))).SelectMany((r, ri) => r.Select((c, ci) => Input.Coord(ri, ci, c))));
        }

        public object Part1(Input input)
        {
            var antennas = input.Where(i => i.Value != '.').ToArray();
            
            var antinodes = from a in antennas
                            group a by a.Value into g
                            select (g.Key, Antennas: g.Select(g => g.Key).ToArray()) into c
                            from pair in c.Antennas.Cartesian(c.Antennas, (a, b) => (a, b))
                            where pair.a != pair.b
                            select pair.a - pair.b + pair.a into antinode
                            where input.ContainsKey(antinode)
                            select antinode;

            return antinodes.Distinct().Count();
        }

        public object Part2(Input input)
        {
            var antennas = input.Where(i => i.Value != '.').ToArray();

            var antinodes = from a in antennas
                            group a by a.Value into g
                            select (g.Key, Antennas: g.Select(g => g.Key).ToArray()) into c
                            from pair in c.Antennas.Cartesian(c.Antennas, (a, b) => (a, b))
                            where pair.a != pair.b
                            from antinode in EnumerableEx.Gen(pair.a, (a) => pair.a - pair.b + a).TakeWhile(input.ContainsKey)
                            select antinode;

            return antinodes.Distinct().Count();
        }
    }
}