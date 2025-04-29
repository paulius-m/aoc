using Tools;

namespace Days.Y2019.Day10;
using Input = List<Asteroid>;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var input = await File.ReadAllLinesAsync(this.GetInputFile("input"));

        var asteroids = input.SelectMany((ly, y) => ly.Select((a, x) => a switch
        {
            '#' => new Asteroid { X = x, Y = y },
            _ => null
        }).Where(a => a != null)).ToList();

        return asteroids;
    }

    public object Part1(Input asteroids)
    {
        var crossed = GetCrossed(asteroids);
        var rankedAsteroids = GetRanked(crossed);
        var baseAsteroid = rankedAsteroids.First();

        return baseAsteroid.count;
    }

    private static IOrderedEnumerable<(Asteroid asteroid, int count)> GetRanked(IEnumerable<(Asteroid a1, double angle, double distance, Asteroid a2)> crossed)
    {
        return from uniqueAngles in
                   from angle in
                       from c in crossed
                       select (c.a1, c.angle)
                   group angle by angle.a1 into asteroidAngles
                   select (asteroid: asteroidAngles.Key, count: asteroidAngles.Distinct().Count())
               orderby uniqueAngles.count descending
               select uniqueAngles;
    }

    public object Part2(Input asteroids)
    {
        var crossed = GetCrossed(asteroids);
        var rankedAsteroids = GetRanked(crossed);
        var baseAsteroid = rankedAsteroids.First();

        var targets = from a in (
                          from a in crossed
                          where a.a1 == baseAsteroid.asteroid
                          group a by a.angle into g
                          select g.OrderBy(g => g.distance)
                      ).SelectMany(g => g.Select((a, i) => (a.a2, angle: i * Math.PI * 2 + a.angle)))
                      orderby a.angle
                      select a;

        var target200 = targets.ElementAt(199);

        return target200.a2.X * 100 + target200.a2.Y;
    }

    private static IEnumerable<(Asteroid a1, double angle, double distance, Asteroid a2)> GetCrossed(Input asteroids)
    {
        return from a1 in asteroids
               from a2 in asteroids
               where a1 != a2
               select (
                 a1,
                 angle: (Math.Atan2(a2.Y - a1.Y, a2.X - a1.X) + Math.PI / 2) switch
                 {
                     var a when a < 0 => a + Math.PI * 2,
                     var a => a
                 },
                 distance: Math.Pow(a2.Y - a1.Y, 2) + Math.Pow(a2.X - a1.X, 2),
                 a2
                 );
    }

}

class Asteroid
{
    public int X;
    public int Y;
    public override string ToString() => X + "," + Y;
}