using System.Text.RegularExpressions;
using Tools;
using Input = System.Object;

namespace Days.Y2019.Day04
{
    file class Solution : ISolution<Input>
    {
        public async Task<Input> LoadInput()
        {
            return Task.FromResult<Input>(null);
        }

        public object Part1(Input _)
        {
            var twoSameDigits = new Regex(@"(.)\1+");
            var count = 0;
            for (var i = 264360; i <= 746325; i++)
            {
                var si = i.ToString();
                var nonDecreasing = si.Aggregate((nonDec: true, prev: '0'), (acc, i) => (nonDec: acc.nonDec && i >= acc.prev, i)).nonDec;

                if (nonDecreasing && twoSameDigits.IsMatch(si))
                {
                    count++;
                }
            }
            return count;
        }

        public object Part2(Input _)
        {
            var twoSameDigits = new Regex(@"(.)\1+");
            var count = 0;
            for (var i = 264360; i <= 746325; i++)
            {
                var si = i.ToString();
                var nonDecreasing = si.Aggregate((nonDec: true, prev: '0'), (acc, i) => (nonDec: acc.nonDec && i >= acc.prev, i)).nonDec;

                if (nonDecreasing && twoSameDigits.Matches(si) is var match && match.Any(m => m.Groups.Values.Any(v => v.Value.Length == 2)))
                {
                    count++;
                }
            }
            return count;
        }
    }
}