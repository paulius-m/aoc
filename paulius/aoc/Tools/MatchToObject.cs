using System.Reflection;
using System.Text.RegularExpressions;

namespace Tools;

public static class MatchToObject
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// var regex = new Regex(@"Sensor at x=(?<Item1>-?\d+), y=(?<Item2>-?\d+): closest beacon is at x=(?<Item3>-?\d+), y=(?<Item4>-?\d+)");
    /// regex.Match<(int, int, int, int)>(l);
    /// </example>
    /// var regex = new Regex(@"(?<from>\d+)-(?<to>\d+)");
    /// l.Split(',').Select(regex.Match<CoordRange>);
    /// <example>
    /// var regex = new Regex(@"Sensor at x=(?<Item1>-?\d+), y=(?<Item2>-?\d+): closest beacon is at x=(?<Item3>-?\d+), y=(?<Item4>-?\d+)");
    /// regex.Match<(int, int, int, int)>(l);
    /// </example>
    /// <typeparam name="T"></typeparam>
    /// <param name="r"></param>
    /// <param name="input">string to match</param>
    /// <returns>if matched will create T object and fill it with matched named groups.</returns>
    public static T Match<T>(this Regex r, string input)
    {
        Type type = typeof(T);

        var m = r.Match(input);

        if (HasDefaultConstructor(type))
        {
            var t = Activator.CreateInstance(type);
            foreach (Group g in m.Groups.Values.Where(g => g.Success))
            {
                var prop = type.GetProperty(g.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (prop is not null)
                {
                    object converted = Convert(g, prop.PropertyType);

                    prop.SetValue(t, converted, null);
                }
                else
                {
                    var field = type.GetField(g.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (field is not null)
                    {
                        object converted = Convert(g, field.FieldType);

                        field.SetValue(t, converted);
                    }
                }
            }
            return (T)t;
        } else
        {
            var icic = StringComparison.InvariantCultureIgnoreCase;

            ConstructorInfo ctor = type.GetConstructors().First(t => t.GetParameters().Length > 0);

            var parameters = ctor.GetParameters()
                .Select(p =>
                    Convert(m.Groups.Values.First(g => g.Success && g.Name.Equals(p.Name, icic)), p.ParameterType)
                    ).ToArray();

            return (T)ctor.Invoke(parameters);
        }
    }

    private static bool HasDefaultConstructor(Type type)
    {
        return type.GetConstructors().Any(t => t.GetParameters().Count() == 0);
    }

    private static object Convert(Group g, Type t)
    {
        return t switch
        {
            var ptype when ptype == typeof(int) => System.Convert.ToInt32(g.Value),
            var ptype when ptype == typeof(long) => System.Convert.ToInt64(g.Value),
            var ptype when ptype == typeof(char) => g.Value[0],
            var ptype when ptype == typeof(string) => g.Value,
            var ptype when ptype.IsEnum => Enum.Parse(ptype, g.Value)
        };
    }
}
