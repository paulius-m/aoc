using Tools;
using Input = Tools.Grid<char>;

namespace Days.Y2024.Day04;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return new Input((await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .SelectMany((r, ri) => r.Select((c, ci) => Input.Coord(ri, ci, c))));
    }

    public object Part1(Input input)
    {
        return (from c in input
                where c.Value is 'X'
                let n = from nn in Neighbourhoods.Near8
                        select from i in Enumerable.Range(0, 4) select nn * i + c.Key
                let xmas = from x in n
                           select new string((from xx in x
                                              where input.ContainsKey(xx)
                                              select input[xx]).ToArray())
                select (from x in xmas where x is "XMAS" select 1).Count()).Sum();
    }

    public object Part2(Input input)
    {
        return (from c in input
                where c.Value is 'A'
                let n = from nn in Neighbourhoods.Near4X
                        select from i in Enumerable.Range(0, 3) select -nn * i + c.Key + nn
                let xmas = from x in n
                           select new string((from xx in x
                                              where input.ContainsKey(xx)
                                              select input[xx]).ToArray())
                let mas = (from x in xmas where x is "MAS" select 1).Count()
                where mas is 2
                select 1).Sum();
    }
}