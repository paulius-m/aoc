using Tools;
using static Tools.Neighbourhoods;
using Map = System.Collections.Generic.Dictionary<Tools.Neighbourhoods.Coord2D, char>;

namespace Days.Y2022.Day12
{
    file class Solution : ISolution<Map>
    {
        public async Task<Map> LoadInput()
        {
            return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
                .SelectMany((r, ri) => r.Select((c, ci) => (ri, ci, c)))
                .ToDictionary(p => new Coord2D(p.ri, p.ci), p => p.c);
        }

        public object Part1(Map map)
        {
            var start = map.First(p => p.Value is 'S');
            var end = map.First(p => p.Value is 'E');

            map[start.Key] = 'a';
            map[end.Key] = 'z';

            return GetDistance(map, start.Key, GetCost, GetNext)[end.Key];
        }

        public object Part2(Map map)
        {
            var start = map.First(p => p.Value is 'S');
            var end = map.First(p => p.Value is 'E');

            map[start.Key] = 'a';
            map[end.Key] = 'z';

            return map.Where(p => p.Value is 'a').Select(p => GetDistance(map, p.Key, GetCost, GetNext).GetValueOrDefault(end.Key, long.MaxValue)).Min();
        }

        private long? GetCost(Map map, Coord2D from, Coord2D to)
        {
            if (map[to] - map[from] > 1)
            {
                return null;
            }
            return 1;
        }

        private IEnumerable<Coord2D> GetNext(Map map, Coord2D current)
        {
            return from n in GetNear4(current)
                   where map.ContainsKey(n)
                   select n;
        }

    }
}