using MoreLinq;
using Sprache;
using Tools;

namespace Days.Y2023.Day12;
using Input = (string, long[])[];

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                let s = l.Split(' ')
                select (s[0], s[1].Split(',').SelectArray(long.Parse))).ToArray();
    }

    public object Part1(Input input)
    {
        long combinations = 0;

        for (var i = 0; i < input.Length; i++)
        {
            _cache = new();
            var (log, damaged) = input[i];

            var logCombinations = GetValidCount(log, damaged, new());
            combinations += logCombinations;
        }

        return combinations;
    }

    public object Part2(Input input)
    {
        long combinations = 0;

        for (var i = 0; i < input.Length; i++)
        {
            _cache = new();
            var (log, damaged) = input[i];
            var damagedX5 = damaged.Repeat(5).ToArray();

            var logCombinations = GetValidCount(string.Join('?', log, log, log, log, log), damagedX5, new());
            combinations += logCombinations;
        }

        return combinations;
    }

    private Dictionary<State, long> _cache = new Dictionary<State, long>();
    private record State(int damageIndex = 0, long damageCount = 0, int logIndex = 0, bool skip = true);
    private long GetValidCount(string log, long[] damaged, State state)
    {
        if (_cache.ContainsKey(state))
            return _cache[state];

        var (di, count, logIndex, skip) = state;

        if (logIndex < log.Length)
        {
            var c = log[logIndex];

            if (c is '?')
            {
                State newState = new(di, count, logIndex, skip);
                var start = log.Substring(0, logIndex);
                var end = log.Substring(logIndex + 1);

                var dotResult = GetValidCount(start + '.' + end, damaged, newState);
                var dmgResult = GetValidCount(start + '#' + end, damaged, newState);

                _cache[newState] = dotResult + dmgResult;
                return dotResult + dmgResult;

            }

            if (c is '.' && skip)
                return GetValidCount(log, damaged, state with { logIndex = logIndex + 1 });

            if (c is '.' && !skip)
            {
                return GetValidCount(log, damaged, state with { logIndex = logIndex + 1, skip = true });
            }

            if (c is '#' && skip)
            {
                if (count != 0)
                {
                    return 0;
                }
                if (di == damaged.Length)
                {
                    return 0;
                }

                count = damaged[di];
                di++;
            }

            skip = false;

            if (c is '#')
            {
                count--;
            }

            if (count < 0)
            {
                return 0;
            }

            return GetValidCount(log, damaged, new(di, count, logIndex + 1, skip));
        }

        var result = di == damaged.Length && count == 0 ? 1 : 0;
        _cache[state] = result;
        return result;
    }
}