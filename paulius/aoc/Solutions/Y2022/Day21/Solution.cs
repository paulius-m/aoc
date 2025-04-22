using Tools;

namespace Days.Y2022.Day21
{
    file class Solution : ISolution<string[]>
    {
        public async Task<string[]> LoadInput()
        {
            return (await File.ReadAllLinesAsync(this.GetInputFile("input"))).ToArray();
        }

        public object Part1(string[] lines)
        {
            Dictionary<string, Func<long>> monkeys = new();

            monkeys = (from l in lines
                       select l.Split(" ") switch
                       {
                       [var name, var value] => (name.Trim(':'), FromConstant(value)),
                       [var name, var a, var op, var b] => (name.Trim(':'), FromOp(a, op, b)),

                       }).ToDictionary(kv => kv.Item1, kv => kv.Item2);

            return monkeys["root"]();

            Func<long> FromConstant(string v) => () => long.Parse(v);

            Func<long> FromOp(string a, string op, string b) => op switch
            {
                "*" => () => monkeys[a]() * monkeys[b](),
                "/" => () => monkeys[a]() / monkeys[b](),
                "+" => () => monkeys[a]() + monkeys[b](),
                "-" => () => monkeys[a]() - monkeys[b](),
            };
        }

        public object Part2(string[] lines)
        {
            Dictionary<string, object> monkeys = new();

            monkeys = (from l in lines
                       select l.Split(" ") switch
                       {
                       [var name, var value] => (name.Trim(':'), FromConstant(value)),
                       [var name, var a, var op, var b] => (name.Trim(':'), FromOp(a, op, b)),

                       }).ToDictionary(kv => kv.Item1, kv => kv.Item2);

            monkeys["root"] = monkeys["root"] switch
            {
                (string a, _, string b) => (a, "=", b)
            };

            monkeys["humn"] = "?";

            var red = Reduce(monkeys["root"]);

            while (true)
            {
                red = Equalize(red);
                if (red is ("?", "=", long r))
                {
                    return r;
                }
            }

            object FromConstant(string v) => long.Parse(v);

            object FromOp(string a, string op, string b) => (a, op, b);

            object Reduce(object monkey) => monkey switch
            {
                long v => v,
                string v => v,
                (string a, string op, string b) => (Reduce(monkeys[a]), Reduce(monkeys[b])) switch
                {
                    (long aa, long bb) => op switch
                    {
                        "*" => aa * bb,
                        "/" => aa / bb,
                        "+" => aa + bb,
                        "-" => aa - bb,
                    },
                    (var aa, var bb) => (aa, op, bb)
                }
            };

            object Equalize(object monkey) => monkey switch
            {
                long v => v,
                string v => v,
                ((long a, "+", var b), "=", long r) => (b, "=", r - a),
                ((long a, "-", var b), "=", long r) => (b, "=", -r + a),
                ((long a, "*", var b), "=", long r) => (b, "=", r / a),
                ((long a, "/", var b), "=", long r) => (b, "=", a / r),
                ((var a, "+", long b), "=", long r) => (a, "=", r - b),
                ((var a, "-", long b), "=", long r) => (a, "=", r + b),
                ((var a, "*", long b), "=", long r) => (a, "=", r / b),
                ((var a, "/", long b), "=", long r) => (a, "=", r * b),
            };
        }
    }
}