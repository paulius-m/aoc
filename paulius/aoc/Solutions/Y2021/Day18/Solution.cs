using Newtonsoft.Json;
using Tools;
using static Days.Y2021.Day18.Solution;

namespace Days.Y2021.Day18;

public class Solution : ISolution<Number[]>
{
    public async Task<Number[]> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .Select(s => JsonConvert.DeserializeObject<Number>(s)!).ToArray();
    }

    public object Part1(Number[] i)
    {
        return Magnitude(i.Aggregate(Add));
    }


    public object Part2(Number[] i)
    {
        return (from a in i
                from b in i
                where a != b
                select Magnitude(Add(a, b))).Max();
    }


    private Number Add(Number a, Number b)
    {
        return Reduce(new Pair(a, b));
    }

    private long Magnitude(Number a)
    {
        return a switch
        {
            Pair { Left: var l, Right: var r } => Magnitude(l) * 3 + Magnitude(r) * 2,
            Regular { Value: var v } => v
        };
    }

    public Number Reduce(Number a)
    {
        bool cont = true;
        for (int i = 0; cont; i++)
        {
            while (CounDepth(a, 0) > 4)
            {
                a = Explode(a, 0).Item1;
            }
            (a, cont) = Split(a);
        }

        return a;

        int CounDepth(Number a, int depth)
        {
            return a switch
            {
                Pair { Left: var l, Right: var r } => Math.Max(CounDepth(l, depth + 1), CounDepth(r, depth + 1)),
                _ => depth,
            };
        }

        (Number, bool) Split(Number a)
        {
            switch (a)
            {
                case Pair { Left: var l, Right: var r }:
                    (l, var s) = Split(l);
                    if (!s) (r, s) = Split(r);
                    return (new Pair(l, r), s);
                case Regular { Value: var v } when v >= 10:
                    return (new Pair(new Regular(v / 2), new Regular(v / 2 + v % 2)), true);
                default:
                    return (a, false);
            };
        }

        Number AddLeft(Number a, long add)
        {
            return a switch
            {
                Pair { Left: var l, Right: var r } => new Pair(AddLeft(l, add), r),
                Regular { Value: var v } => new Regular(v + add)
            };
        }

        Number AddRight(Number a, long add)
        {
            return a switch
            {
                Pair { Left: var l, Right: var r } => new Pair(l, AddRight(r, add)),
                Regular { Value: var v } => new Regular(v + add)
            };
        }

        (Number, long el, long er) Explode(Number s, int depth)
        {
            if (s is Regular sr)
            {
                return (sr, 0, 0);
            }

            if (s is Pair { Left: var l, Right: var r })
            {
                if (depth >= 4 && l is Regular lr && r is Regular rr)
                {
                    return (new Regular(0), lr.Value, rr.Value);
                }

                (l, var ulel, var uler) = Explode(l, depth + 1);

                if (uler > 0)
                {
                    r = AddLeft(r, uler);
                }

                (r, var urel, var urer) = Explode(r, depth + 1);

                if (urel > 0)
                {
                    l = AddRight(l, urel);
                }

                return (new Pair(l, r), ulel, urer);
            }

            return (s, 0, 0);
        }
    }

    [JsonConverter(typeof(SnailfishNumberConverter))]
    public record Number;
    public record class Pair(Number Left, Number Right) : Number;
    public record class Regular(long Value) : Number;

    public class SnailfishNumberConverter : JsonConverter
    {
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                var a = serializer.Deserialize<Number[]>(reader)!;
                return new Pair(a.First(), a.Last());
            }
            else
            {
                return new Regular(serializer.Deserialize<long>(reader));
            }
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            switch (value)
            {
                case Regular sr:
                    serializer.Serialize(writer, sr.Value);
                    break;
                case Pair sp:
                    serializer.Serialize(writer, new[] { sp.Left, sp.Right });
                    break;
            }
        }

        public override bool CanConvert(Type objectType) => throw new NotImplementedException();
    }
}