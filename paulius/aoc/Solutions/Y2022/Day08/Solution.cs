using Tools;
using static Tools.Neighbourhoods;
using Map = System.Collections.Generic.Dictionary<Tools.Neighbourhoods.Coord2D, char>;
using MapPoint = System.Collections.Generic.KeyValuePair<Tools.Neighbourhoods.Coord2D, char>;

namespace Days.Y2022.Day08;

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
        return map.Where(IsVisible).Count();

        bool IsVisible(MapPoint point) => Near4.Any(n => TrackRay(point, n, map).Visible);
    }

    public object Part2(Map map)
    {
        return map.Max(point => Near4.Product(n => TrackRay(point, n, map).Length));
    }

    (bool Visible, long Length) TrackRay(MapPoint point, Coord2D dir, Map map)
    {
        var (c, height) = point;

        for (var i = 1; true; i++)
        {
            var ray = c + dir * i;
            if (map.ContainsKey(ray))
            {
                if (height <= map[ray])
                    return (false, i);
            }
            else
            {
                return (true, i - 1);
            }
        }
    }

}