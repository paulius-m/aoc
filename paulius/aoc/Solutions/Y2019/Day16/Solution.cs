using Tools;
using static System.Net.Mime.MediaTypeNames;
using Input = System.String;

namespace Days.Y2019.Day16
{
    file class Solution : ISolution<Input>
    {
        public async Task<Input> LoadInput()
        {
            var text = await File.ReadAllTextAsync(this.GetInputFile("input"));
            return text;
        }

        public object Part1(Input text)
        {
            var input = text.ToCharArray().Select(c => (sbyte)(c - '0')).ToArray().ToArray();
            var skip = 0;

            var patterns = new (long, sbyte[])[input.Length];
            for (var i = 0; i < input.Length; i++)
            {
                patterns[i] = Pattern(i + skip + 1, input.Length, skip);
            }

            for (int k = 0; k < 100; k++)
            {
                for (int i = 0; i < input.Length; i++)
                {
                    input[i] = MultSum(input, patterns[i]);
                }
            }

            return string.Join("", input.Take(8));
        }

        public object Part2(Input text)
        {
            var input = Enumerable.Repeat(text.ToCharArray().Select(c => (sbyte)(c - '0')).ToArray(), 10000).SelectMany(a => a).ToArray();
            var skip = input.Take(7).Aggregate(0, (a, c) => a * 10 + c);

            input = input.Skip(skip).ToArray();

            for (int k = 0; k < 100; k++)
            {
                long sum = 0;

                for (int i = 0; i < input.Length; i++)
                {
                    sum += input[i];
                }

                for (int i = 0; i < input.Length; i++)
                {
                    var a = input[i];
                    input[i] = (sbyte)Math.Abs(sum % 10);
                    sum -= a;
                }
            }

            return string.Join("", input.Take(8));
        }

        private static sbyte MultSum(sbyte[] input, (long, sbyte[]) vs)
        {
            long acc = 0;
            for (var i = vs.Item1; i < input.Length; i++)
            {
                checked
                {
                    acc += input[i] * vs.Item2[i - vs.Item1];
                }
            }
            return (sbyte)Math.Abs(acc % 10);
        }

        private static sbyte[] _pattern = [0, 1, 0, -1];

        private static (long, sbyte[]) Pattern(long order, long length, long skip)
        {
            var zeroSkip = order - skip - 1;
            var r = new sbyte[length - zeroSkip];

            for (var i = zeroSkip; i < length; i++)
            {
                r[i - zeroSkip] = _pattern[(i + skip + 1) / order % _pattern.Length];
            }

            return (zeroSkip, r);
        }
    }
}