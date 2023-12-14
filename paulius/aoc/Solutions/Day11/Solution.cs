using MoreLinq;
using Sprache;
using Tools;
using static Tools.Neighbourhoods;
using Input = string[];

namespace Days.Day11
{
    file class Solution : ISolution<Input>
    {
        public async Task<Input> LoadInput()
        {
            return await File.ReadAllLinesAsync(this.GetInputFile("input"));
        }

        public object Part1(Input input)
        {
            var expanded = GetExpanded(GetExpanded(input).Transpose().Select(x => new string(x.ToArray())));

            var galaxies = expanded
                .SelectMany((r, ri) => r.Select((c, ci) => Grid<char>.Coord(ri, ci, c) )
                                        .Where(kv => kv.Value is not '.')).ToArray();


            var sum = galaxies.SelectMany((gi, i) => galaxies.Skip(i).Select ((gj, j) => Math.Abs(gi.Key.c - gj.Key.c) + Math.Abs(gi.Key.r - gj.Key.r)) ).Sum();

            return sum;
            static IEnumerable<string> GetExpanded(IEnumerable<string> input)
            {
                return input.SelectMany(l => l.Distinct().Count() is 1 ? [l, l] : new[] { l });
            }
        }

        public object Part2(Input input)
        {
            var spaceRows = GetExpanded(input).ToArray();

            var  spaceColumns = GetExpanded(input.Transpose().Select(x => new string(x.ToArray()))).ToArray();

            var galaxies = input
                .SelectMany((r, ri) => r.Select((c, ci) => Grid<char>.Coord(ri, ci, c))
                                        .Where(kv => kv.Value is not '.')).ToArray();

            var sum = galaxies
                .SelectMany((gi, i) => galaxies.Skip(i).Select((gj, j) => Math.Abs(GetCoord(gi.Key.c, spaceColumns) - GetCoord(gj.Key.c, spaceColumns)) + Math.Abs(GetCoord(gi.Key.r, spaceRows) - GetCoord(gj.Key.r, spaceRows)))).Sum();

            return sum;
            static IEnumerable<int> GetExpanded(IEnumerable<string> input)
            {
                return input.Select((l, i) => (i, space: l.Distinct().Count() is 1) ).Where( s => s.space is true ).Select(s => s.i);
            }

            static long GetCoord(long l, int[] spaces)
            {
                return l + (spaces.Where(s => s < l).Count() * (1000000 - 1)); //
            }
        }
    }
}