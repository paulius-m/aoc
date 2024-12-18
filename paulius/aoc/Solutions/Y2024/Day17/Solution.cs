using Sprache;
using System;
using System.Net;
using Tools;

namespace Days.Y2024.Day17;

using Input = (Dictionary<char, ulong> Registers, ulong[] Program);

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return Parsers.Input.Parse(await File.ReadAllTextAsync(this.GetInputFile("input")));
    }

    public object Part1(Input input)
    {
        var (r, program) = input;
        var mem = new Memory(r);
        return string.Join(',', RunProgram(mem, program));
    }

    private static List<ulong> RunProgram(Memory mem, ulong[] program)
    {
        var ip = 0;
        var halt = false;

        var output = new List<ulong>();

        while (!halt)
        {
            if (ip >= program.Length)
            {
                halt = true;
                break;
            }

            ip = Tick(program, mem, ip, output);
        }

        return output;
    }

    private void Decompile(ulong[] program)
    {
        for (var ip = 0; ip < program.Length - 1;)
        {
            var oip = ip++;
            var op = (OpCode)program[oip];
            switch (op)
            {
                case OpCode.adv:
                    {
                        var a = 'A';
                        var b = mem(program[ip++]);
                        var r = "A";
                        Console.WriteLine($"{oip}:\t{op}\t {a} >>> {b} = {r}");
                    }
                    break;
                case OpCode.bxl:
                    {
                        var a = 'B';
                        var b = program[ip++];
                        var r = "B";
                        Console.WriteLine($"{oip}:\t{op}\t {a} ^ {b} = {r}");
                    }
                    break;
                case OpCode.bst:
                    {
                        var a = mem(program[ip++]);
                        var b = 8UL;
                        var r = "B";
                        Console.WriteLine($"{oip}:\t{op}\t {a} % {b} = {r}");
                    }
                    break;
                case OpCode.jnz:
                    {
                        var a = 'A';
                        var b = (int)program[ip];
                        var r = "IP";
                        Console.WriteLine($"{oip}:\t{op}\t {a} {b} = {r}");
                    }
                    break;
                case OpCode.bxc:
                    {
                        var a = 'B';
                        var b = 'C';
                        var r = 'B';
                        Console.WriteLine($"{oip}:\t{op}\t {a} ^ {b} = {r}");
                        ip++;
                    }
                    break;
                case OpCode.Out:
                    {
                        var a = mem(program[ip++]);
                        var b = 8UL;
                        var r = "OUT";
                        Console.WriteLine($"{oip}:\t{op}\t {a} {b} = {r}");
                    }
                    break;
                case OpCode.bdv:
                    {
                        var a = 'A';
                        var b = mem(program[ip++]);
                        var r = 'B';
                        Console.WriteLine($"{oip}:\t{op}\t {a} >>> {b} = {r}");
                    }
                    break;
                case OpCode.cdv:
                    {
                        var a = 'A';
                        var b = mem(program[ip++]);
                        var r = 'C';
                        Console.WriteLine($"{oip}:\t{op}\t {a} >>> {b} = {r}");
                    }
                    break;
            }
        }
        char mem(ulong index)
        {
            return index switch
            {
                var a when a <= 3 => (char)(a + '0'),
                var a when a is >= 3 and < 7 => (char)(a - 4 + 'A')
            };
        }
    }

    private static int Tick(ulong[] program, Memory mem, int ip, List<ulong> output)
    {
        var oip = ip++;
        var op = (OpCode)program[oip];
        switch (op)
        {
            case OpCode.adv:
                {
                    var a = mem['A'];
                    var b = (int)mem[program[ip++]];
                    var r = a >>> b;
                    mem['A'] = r;
                }
                break;
            case OpCode.bxl:
                {
                    var a = mem['B'];
                    var b = program[ip++];
                    var r = a ^ b;
                    mem['B'] = r;
                }
                break;
            case OpCode.bst:
                {
                    var a = mem[program[ip++]];
                    var b = 8UL;
                    var r = a % b;
                    mem['B'] = r;
                }
                break;
            case OpCode.jnz:
                {
                    var a = mem['A'];
                    var b = (int)program[ip];
                    var r = (a is not 0) ? b : ip + 1;
                    ip = r;
                }
                break;
            case OpCode.bxc:
                {
                    var a = mem['B'];
                    var b = mem['C'];
                    var r = a ^ b;
                    mem['B'] = r;
                    ip++;
                }
                break;
            case OpCode.Out:
                {
                    var a = mem[program[ip++]];
                    var b = 8UL;
                    var r = a % b;
                    output.Add(r);
                }
                break;
            case OpCode.bdv:
                {
                    var a = mem['A'];
                    var b = (int)mem[program[ip++]];
                    var r = a >>> b;
                    mem['B'] = r;
                }
                break;
            case OpCode.cdv:
                {
                    var a = mem['A'];
                    var b = (int)mem[program[ip++]];
                    var r = a >>> b;
                    mem['C'] = r;
                }
                break;
        }

        return ip;
    }

    public object Part2(Input input)
    {

        var (r, program) = input;
        Decompile(program);

        var mem = new Memory(3);
        var from = program.Length - 1;
        var a = Reconstruct(program, mem, from);

        if (a != null)
        {
            var mem2 = new Memory(3);
            mem2['A'] = a.Value;
            Console.WriteLine(string.Join(',', RunProgram(mem2, program)));
        }
        return a;
    }

    private static ulong? Reconstruct(ulong[] program, Memory mem, int from)
    {
        Console.WriteLine(Convert.ToString((long)mem['A'], 2));
        for (int i = from; i >= 0; i--)
        {
            mem['B'] = program[i];
            mem['A'] = mem['A'] << 3;
            mem['B'] = mem['B'] ^ 7;

            var matchCount = 0;

            for (ulong b = 0; b < 8; b++)
            {
                var bx6 = b ^ 6;
                var c = (mem['A'] + b) >>> (int)bx6;
                var bcand = (bx6 ^ c) % 8;
                if (bcand == mem['B'])
                {
                    var mem2 = new Memory(3);
                    mem2['A'] = mem['A'] + b;
                    var a = Reconstruct(program, mem2, i - 1);
                    if (a != null)
                    {
                        return a;
                    }
                    matchCount++;
                }
            }
            return null;
        }

        return mem['A'];
    }
}

file class Memory
{
    ulong[] _registers;
    public Memory(Dictionary<char, ulong> registers)
    {
        _registers = new ulong[registers.Count];
        foreach (var i in registers)
        {
            _registers[i.Key - 'A'] = i.Value;
        }
    }

    public Memory(int regiserCount)
    {
        _registers = new ulong[regiserCount];
    }

    public ulong this[char index]
    {
        get
        {
            return _registers[index - 'A'];
        }
        set
        {
            _registers[index - 'A'] = value;
        }
    }

    public ulong this[ulong index]
    {
        get
        {
            return index switch
            {
                var a when a <= 3 => a,
                var a when a is >= 3 and < 7 => _registers[a - 4]
            };
        }
    }
}

file enum OpCode
{
    adv = 0,
    bxl = 1,
    bst = 2,
    jnz = 3,
    bxc = 4,
    Out = 5,
    bdv = 6,
    cdv = 7,
}

file static class Parsers
{
    private static Parser<KeyValuePair<char, ulong>> Register = from key in Parse.String("Register").Token().Then(_ => Parse.Letter)
                                                                from value in Parse.String(":").Token().Then(_ => Parse.Number).Select(ulong.Parse)
                                                                select KeyValuePair.Create(key, value);

    private static Parser<ulong[]> Program = from _ in Parse.String("Program:").Token()
                                             from values in Parse.Number.Select(ulong.Parse).DelimitedBy(Parse.Char(','))
                                             select values.ToArray();

    public static Parser<Input> Input = from r in Register.DelimitedBy(Parse.LineEnd)
                                        from _ in Parse.LineEnd
                                        from p in Program
                                        select (r.ToDictionary(), p);
}