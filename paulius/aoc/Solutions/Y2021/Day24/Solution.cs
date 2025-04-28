using Sprache;
using Tools;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Instruction = System.ValueTuple<string, string, string>;
namespace Days.Y2021.Day24;

public class Solution : ISolution<Instruction[]>
{
    public async Task<Instruction[]> LoadInput()
    {
        var r = new Regex(@"(?<item1>\w+) (?<item2>\w+) ?(?<item3>-?\w+)?");

        return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                select r.Match<(string, string, string)>(l)).ToArray();
    }

    public object Part1(Instruction[] instructions)
    {
        return 0;
        var program = Compile(instructions);

        var consts = new[] { 9, 9, 2, 9, 9, 5, 1, 3, 8, 9, 0, 0, 0, 0 };

        var nums = new[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 };

        var numIdx = new int[14];

        var num = new int[14];
        var result = 1L;
        while (result is not 0)
        {
            for (int i = 0; i < num.Length; i++)
            {
                num[i] = consts[i] is 0 ? nums[numIdx[i]] : consts[i];
            }

            var carry = 0;
            var incIdx = 0;
            do
            {
                while (consts[incIdx] > 0) { incIdx++; }

                carry = 0;
                numIdx[incIdx] += 1;

                if (numIdx[incIdx] >= nums.Length)
                {
                    numIdx[incIdx] = 0;
                    carry = 1;
                }
                incIdx++;
            }
            while (carry > 0);
            var prevResult = result;
            result = program(num)["z"];
            //Console.WriteLine($"{string.Join("", num)} {result} {result - prevResult}  ");
            //Console.ReadLine();
        }

        return string.Join("", num);
    }

    public object Part2(Instruction[] instructions)
    {
        var program = Compile(instructions);

        var consts = new int[14];
        // var consts = new int[14];

        var nums = new int[14][] {
            new[] { 9 },
            new[] { 3, 4, 5, 6, 7, 8, 9 },
            new[] { 1, 2 },
            new[] { 8, 9 },
            new[] { 5, 6, 7, 8, 9 },
            new[] { 1, 2, 3, 4, 5 },
            new[] { 1 },
            new[] { 1, 2, 3, 4 },
            new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            new[] {  9 },
            new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }
        };
        var numIdx = new int[14];

        var num = new int[14];
        var result = 1L;
        long prevMinZ = int.MaxValue;
        var prevResult = long.MaxValue;

        while (result is not 0)
        {
            for (int i = 0; i < num.Length; i++)
            {
                num[i] = consts[i] is 0 ? nums[i][numIdx[i]] : consts[i];
            }

            //var carry = 0;
            //var incIdx = 13;
            //do
            //{
            //    while (consts[incIdx] > 0) { incIdx--; }

            //    carry = 0;
            //    numIdx[incIdx] += 1;

            //    if (numIdx[incIdx] >= nums[incIdx].Length)
            //    {
            //        numIdx[incIdx] = 0;
            //        carry = 1;
            //    }
            //    incIdx--;
            //}
            //while (carry > 0);

            minZ = int.MaxValue;
            result = program(num)["z"];
            if (result <= prevResult)
            {
                Console.WriteLine(minZ);
                Console.WriteLine($"{string.Join("", num)} {result} {result - prevResult}  ");
                prevMinZ = minZ;
                prevResult = result;
            }
            //Console.ReadLine();
        }

        return string.Join("", num);
    }

    long minZ = int.MaxValue;

    Func<int[], Dictionary<string, long>> Compile(Instruction[] instructions) => (input) =>
    {
        Dictionary<string, long> registers = new()
        {
            ["x"] = 0,
            ["y"] = 0,
            ["z"] = 0,
            ["w"] = 0,
        };


        long f(string amt) => amt switch
        {
            var r when registers.ContainsKey(r) => registers[r],
            _ => int.Parse(amt)
        };

        Dictionary<string, string> exps = new()
        {
            ["x"] = "0",
            ["y"] = "0",
            ["z"] = "0",
            ["w"] = "0",
        };

        string expf(string amt) => amt switch
        {
            var r when registers.ContainsKey(r) => exps[r],
            _ => amt
        };

        var index = 0;
        foreach (var i in instructions)
        {
            switch (i)
            {
                case ("inp", var r, _):

                    if (index > 0)
                    {
                        foreach (var e in exps)
                        {
                            Console.WriteLine($"{e} = {registers[e.Key]} ");
                        }

                        if (registers["z"] < minZ)
                        {
                            minZ = registers["z"];
                        }


                        Console.WriteLine(index + " " + string.Join("", input));
                        Console.ReadLine();

                        exps = new(registers.Select(kv => KeyValuePair.Create(kv.Key, kv.Value.ToString())));
                    }

                    exps[r] = $"input[{index}]";
                    registers[r] = input[index++];

                    break;
                case ("add", var r, var amt):
                    registers[r] = registers[r] + f(amt);


                    var left = exps[r];
                    var right = expf(amt);

                    if (left == "0")
                    {
                        exps[r] = $"{right}";
                    }
                    else if (right == "0")
                    {
                        exps[r] = $"{left}";
                    }
                    else
                    {
                        exps[r] = $"({left} + {right})";
                    }

                    break;
                case ("mul", var r, var amt):
                    registers[r] = registers[r] * f(amt);

                    left = exps[r];
                    right = expf(amt);

                    if (left == "0" || right == "0")
                    {
                        exps[r] = $"0";
                    }
                    else
                    {
                        exps[r] = $"({left} * {right})";
                    }
                    break;
                case ("div", var r, var amt):
                    registers[r] = registers[r] / f(amt);

                    left = exps[r];
                    right = expf(amt);

                    if (right == "1")
                    {
                        exps[r] = $"{left}";
                    }
                    else
                    {
                        exps[r] = $"({left} / {right})";
                    }
                    break;
                case ("mod", var r, var amt):
                    registers[r] = registers[r] % f(amt);
                    exps[r] = $"({exps[r]} % {expf(amt)})";

                    break;
                case ("eql", var r, var amt):
                    registers[r] = registers[r] == f(amt) ? 1 : 0;
                    exps[r] = $"({exps[r]} == {expf(amt)})";
                    break;
            }

        }


        foreach (var e in exps)
        {
            Console.WriteLine($"{e} = {registers[e.Key]} ");
        }

        if (registers["z"] < minZ)
        {
            minZ = registers["z"];
        }


        Console.WriteLine(index + " " + string.Join("", input));
        Console.ReadLine();

        exps = new(registers.Select(kv => KeyValuePair.Create(kv.Key, kv.Value.ToString())));

        return registers;
    };
}