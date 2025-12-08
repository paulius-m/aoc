using Days.Y2022.Day02;
using MoreLinq;
using Sprache;
using System;
using Tools;
using Input = string[];

namespace Days.Y2025.Day06
{
    file class Solution : ISolution<Input>
    {
        Parser<IEnumerable<long>> NumberTokens = Parse.Number.Token().Select(long.Parse).Many();
        Parser<char[]> SignTokens = Parse.Chars('*', '+').Token().Many().Select(l => l.ToArray());

        public async Task<Input> LoadInput()
        {
            var lines = await File.ReadAllLinesAsync(this.GetInputFile("input"));
            return lines;
        }

        public object Part1(Input lines)
        {
            var numbers = (from l in lines[..^1]
                           select NumberTokens.Parse(l))
                           .Transpose()
                           .Select(r => r.ToArray()
                           ).ToArray();

            var signs = SignTokens.Parse(lines[^1]);

            var sum = 0L;
            for (var i = 0; i < signs.Length; i++)
            {
                sum += signs[i] switch
                {
                    '+' => numbers[i].Sum(),
                    '*' => numbers[i].Product()
                };
            }

            return sum;
        }

        public object Part2(Input lines)
        {
            var numberLines = lines[..^1];
            var lastLine = lines[^1];

            var signs = SignTokens.Parse(lastLine);

            var signPositions = signs.Aggregate(new List<int>() { -1 }, (a, b) => { a.Add(lastLine.IndexOf(b, a[^1] + 1)); return a; });

            signPositions.RemoveAt(0);
            signPositions.Add(int.MaxValue);

            var numbers = signPositions
                .Pairwise((f, t) => numberLines.Select(l => l.Substring(f, Math.Min(t, l.Length) - f)));

            var rtl = (from row in numbers
                       let t = from tt in row.Transpose()
                               select new string(tt.ToArray()) into s
                               where string.IsNullOrWhiteSpace(s)
                               select long.Parse(s)
                       select t.ToArray()).ToArray();

            var sum = 0L;
            for (var i = 0; i < signs.Length; i++)
            {
                var row = signs[i] switch
                {
                    '+' => rtl[i].Sum(),
                    '*' => rtl[i].Product()
                };
                sum += row;
            }

            return sum;
        }
    }
}