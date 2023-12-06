using Tools;
using static Tools.Neighbourhoods;
using Input = Tools.Grid<char>;

namespace Days.Day03
{
    file class Solution : ISolution<Input>
    {
        public async Task<Input> LoadInput()
        {
            return new((await File.ReadAllLinesAsync(this.GetInputFile("input")))
                .SelectMany((r, ri) => r.Select((c, ci) => (Key: new Coord2D(ri, ci), Value: c)))
                .Where(c => c.Value != '.')
                .ToDictionary(kv => kv.Key, kv => kv.Value));
        }

        public object Part1(Input i)
        {
            var ns = from s in i
                     where !char.IsNumber(s.Value)
                     from n in GetNear8(s.Key)
                     where i.ContainsKey(n)
                     select n;

            var ranges = (from n in ns
                          select (n.r, GetRange(i, n))).Distinct();

            var numbers = from r in ranges
                          select int.Parse(GetNumber(i, r).ToArray());

            return numbers.Sum();
        }

        public object Part2(Input i)
        {
            var ns = from s in i
                     where s.Value is '*'
                     let ratios = from n in GetNear8(s.Key)
                                  where i.ContainsKey(n)
                                  select n
                     select ratios.ToArray();

            var ranges = from n in ns
                         let r = (from nn in n
                                  select (nn.r, GetRange(i, nn))).Distinct().ToArray()
                         where r.Length > 1
                         select r;

            var numbers = from r in ranges
                          let rr = from ri in r
                                   select long.Parse(GetNumber(i, ri).ToArray())
                          select rr.Product(f => f);

            return numbers.Sum();
        }

        private static CoordRange GetRange(Input i, Coord2D n)
        {
            return new CoordRange
            {
                From = EnumerableEx.Gen(n.c, (i) => i - 1).Select(ci => new Coord2D(n.r, ci)).TakeWhile(cc => i.ContainsKey(cc) && char.IsDigit(i[cc])).Last().c,
                To = EnumerableEx.Gen(n.c, (i) => i + 1).Select(ci => new Coord2D(n.r, ci)).TakeWhile(cc => i.ContainsKey(cc) && char.IsDigit(i[cc])).Last().c,
            };
        }

        private static IEnumerable<char> GetNumber(Input i, (long r, CoordRange c) r)
        {
            return from ccc in Enumerable.Range((int)r.c.From, (int)(r.c.To - r.c.From + 1))
                   select i[new(r.r, ccc)];
        }
    }
}