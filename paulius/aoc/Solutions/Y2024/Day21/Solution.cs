using Sprache;
using Tools;
using static Tools.Neighbourhoods;
using Input = string[];

namespace Days.Y2024.Day21;

file class Solution : ISolution<Input>
{
    private Dictionary<char, Coord2D> keypad = new()
    {
        ['7'] = new Coord2D(3, 0),
        ['8'] = new Coord2D(3, 1),
        ['9'] = new Coord2D(3, 2),
        ['4'] = new Coord2D(2, 0),
        ['5'] = new Coord2D(2, 1),
        ['6'] = new Coord2D(2, 2),
        ['1'] = new Coord2D(1, 0),
        ['2'] = new Coord2D(1, 1),
        ['3'] = new Coord2D(1, 2),
        ['0'] = new Coord2D(0, 1),
        ['A'] = new Coord2D(0, 2),
    };

    Dictionary<string, string> cheatsheetKeyPad = new Dictionary<string, string>
    {
        ["00"] = "A",
        ["01"] = "^<A",
        ["02"] = "^A",
        ["03"] = ">^A",
        ["04"] = "^^<A",
        ["05"] = "^^A",
        ["06"] = "^^>A",
        ["07"] = "^^^<A",
        ["08"] = "^^^A",
        ["09"] = ">^^^A",
        ["0A"] = ">A",
        ["10"] = ">vA",
        ["11"] = "A",
        ["12"] = ">A",
        ["13"] = ">>A",
        ["14"] = "^A",
        ["15"] = "^>A",
        ["16"] = "^>>A",
        ["17"] = "^^A",
        ["18"] = "^^>A",
        ["19"] = ">>^^A",
        ["1A"] = ">>vA",
        ["20"] = "vA",
        ["21"] = "<A",
        ["22"] = "A",
        ["23"] = ">A",
        ["24"] = "<^A",
        ["25"] = "^A",
        ["26"] = ">^A",
        ["27"] = "<^^A",
        ["28"] = "^^A",
        ["29"] = "^^>A",
        ["2A"] = "v>A",
        ["30"] = "v<A",
        ["31"] = "<<A",
        ["32"] = "<A",
        ["33"] = "A",
        ["34"] = "^<<A",
        ["35"] = "<^A",
        ["36"] = "^A",
        ["37"] = "^^<<A",
        ["38"] = "<^^A",
        ["39"] = "^^A",
        ["3A"] = "vA",
        ["40"] = ">vvA",
        ["41"] = "vA",
        ["42"] = "v>A",
        ["43"] = ">>vA",
        ["44"] = "A",
        ["45"] = ">A",
        ["46"] = ">>A",
        ["47"] = "^A",
        ["48"] = ">^A",
        ["49"] = ">>^A",
        ["4A"] = ">>vvA",
        ["50"] = "vvA",
        ["51"] = "v<A",
        ["52"] = "vA",
        ["53"] = "v>A",
        ["54"] = "<A",
        ["55"] = "A",
        ["56"] = ">A",
        ["57"] = "<^A",
        ["58"] = "^A",
        ["59"] = ">^A",
        ["5A"] = "vv>A",
        ["60"] = "vv<A",
        ["61"] = "v<<A",
        ["62"] = "v<A",
        ["63"] = "vA",
        ["64"] = "<<A",
        ["65"] = "<A",
        ["66"] = "A",
        ["67"] = "<<^A",
        ["68"] = "<^A",
        ["69"] = "^A",
        ["6A"] = "vvA",
        ["70"] = ">vvvA",
        ["71"] = "vvA",
        ["72"] = "vv>A",
        ["73"] = "vv>>A",
        ["74"] = "vA",
        ["75"] = "v>A",
        ["76"] = "v>>A",
        ["77"] = "A",
        ["78"] = ">A",
        ["79"] = ">>A",
        ["7A"] = ">>vvvA",
        ["80"] = "vvvA",
        ["81"] = "vv<A",
        ["82"] = "vvA",
        ["83"] = "vv>A",
        ["84"] = "v<A",
        ["85"] = "vA",
        ["86"] = "v>A",
        ["87"] = "<A",
        ["88"] = "A",
        ["89"] = ">A",
        ["8A"] = "vvv>A",
        ["90"] = "vvv<A",
        ["91"] = "vv<<A",
        ["92"] = "vv<A",
        ["93"] = "vvA",
        ["94"] = "v<<A",
        ["95"] = "v<A",
        ["96"] = "vA",
        ["97"] = "<<A",
        ["98"] = "<A",
        ["99"] = "A",
        ["9A"] = "vvvA",
        ["A0"] = "<A",
        ["A1"] = "^<<A",
        ["A2"] = "<^A",
        ["A3"] = "^A",
        ["A4"] = "^^<<A",
        ["A5"] = "<^^A",
        ["A6"] = "^^A",
        ["A7"] = "^^^<<A",
        ["A8"] = "<^^^A",
        ["A9"] = "^^^A",
        ["AA"] = "A",
    };

    Dictionary<string, string> cheatsheet = new Dictionary<string, string>
    {
        ["A<"] = "v<<A",
        ["A^"] = "<A",
        ["Av"] = "<vA",
        ["A>"] = "vA",
        ["AA"] = "A",

        ["<v"] = ">A",
        ["<^"] = ">^A",
        ["<>"] = ">>A",
        ["<A"] = ">>^A",
        ["<<"] = "A",

        ["^v"] = "vA",
        ["^<"] = "v<A",
        ["^>"] = "v>A",
        ["^A"] = ">A",
        ["^^"] = "A",

        ["v^"] = "^A",
        ["v<"] = "<A",
        ["v>"] = ">A",
        ["vA"] = "^>A",
        ["vv"] = "A",

        [">^"] = "<^A",
        ["><"] = "<<A",
        [">A"] = "^A",
        [">v"] = "<A",
        [">>"] = "A",
    };

    public async Task<Input> LoadInput()
    {
        return await File.ReadAllLinesAsync(this.GetInputFile("input"));
    }

    public object Part1(Input input)
    {

        var dir1 = GetDirections(input, keypad, Coord2D.Zero).ToArray();

        for (int i = 0; i < 4; i++)
        {
            dir1 = GetDirections2(dir1, cheatsheet).ToArray();
        }

        return input.Zip(dir1, (a, b) => long.Parse(new string(a.Where(char.IsNumber).ToArray())) * b.Length).Sum();
    }

    private IEnumerable<string> GetDirections(string[] input, Dictionary<char, Coord2D> keypad, Coord2D empty, char v = 'A')
    {
        foreach (var code in input)
        {
            var current = keypad[v];
            var directions = "";
            foreach (var c in code)
            {
                if (!keypad.ContainsKey(c)) continue;
                var next = keypad[c];
                var dist = next - current;

                directions += Draw(current, dist, empty);

                current = next;
            }
            yield return directions;
        }

        static string Draw(Coord2D cur, Coord2D dist, Coord2D empty)
        {
            var r = new string(dist.r > 0 ? '^' : 'v', (int)Math.Abs(dist.r));
            var c = new string(dist.c > 0 ? '>' : '<', (int)Math.Abs(dist.c));

            if (cur.r == empty.r && cur.c + dist.c == empty.c)
                return r + c + "A";
            if (cur.c == empty.c && cur.r + dist.r == empty.r)
                return c + r + "A";
            if (dist.r < 0)
                return r + c + "A";
            return c + r + "A";
        }
    }

    private IEnumerable<string> GetDirections2(string[] dir, Dictionary<string, string> cheatsheet)
    {

        foreach (var code in dir)
        {
            var current = "A";
            var directions = "";
            foreach (var c in code)
            {
                var dist = current + c;

                directions += cheatsheet[dist];

                current = new string(c, 1);
            }
            yield return directions;
        }
    }

    private IEnumerable<Dictionary<string, long>> GetDirections2(Dictionary<string, long>[] dir, Dictionary<string, string> cheatsheet)
    {
        foreach (var code in dir)
        {
            var directions = new Dictionary<string, long>();
            foreach (var (seq, count) in code)
            {
                var current = "A";
                foreach (var c in seq)
                {
                    var dist = current + c;
                    var distKey = cheatsheet[dist];
                    directions[distKey] = directions.GetValueOrDefault(distKey, 0L) + count;

                    current = new string(c, 1);
                }
            }
            yield return directions;
        }
    }

    public object Part2(Input input)
    {
        var dir1 = GetDirections2(input, cheatsheetKeyPad).ToArray();

        Dictionary<string, long>[] dic = (from d in dir1
                                          let br = d.Replace("A", "A,").Split(',').GroupBy(x => x).Where(g => g.Key is not "").ToDictionary(g => g.Key, g => g.LongCount())
                                          select br).ToArray();

        for (var i = 0; i < 25; i++)
        {
            dic = GetDirections2(dic, cheatsheet).ToArray();
        }
        return input.Zip(dic, (a, b) => long.Parse(new string(a.Where(char.IsNumber).ToArray())) * b.Sum(bb => bb.Key.Length * bb.Value)).Sum();
    }
}