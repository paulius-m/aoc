using Days.Y2019.IntCode;
using MoreLinq;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Channels;
using Tools;
using Input = long[];

namespace Days.Y2019.Day21;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (await File.ReadAllTextAsync(this.GetInputFile("input"))).Split(',').SelectArray(long.Parse);
    }

    public async Task<object> Part1Async(Input input)
    {
        var memory = new IntCode.Memory<long>(input.SelectArray(i => new MemCell<long> { Value = i }));
        var system = new ProcessingUnit(memory);

        var systemTask = system.Run();

        var displayTask = Display(system.OUT);
        var inputTask = Input(system.IN);

        await Task.WhenAll(systemTask, displayTask, inputTask);

        var result = displayTask.Result;


        return result;

        static async Task Input(ChannelWriter<long> input)
        {
            var program =
                """
                NOT A T
                OR T J
                NOT B T
                OR T J
                NOT C T
                OR T J
                AND D J
                WALK
                """;

            foreach (var instruction in program.Split(Environment.NewLine))
            {
                foreach (var c in instruction)
                {
                    await input.WriteAsync(c);
                }
                await input.WriteAsync('\n');
            }
        }
    }

    public async Task<object> Part2Async(Input input)
    {
        var memory = new IntCode.Memory<long>(input.SelectArray(i => new MemCell<long> { Value = i }));
        var system = new ProcessingUnit(memory);

        var systemTask = system.Run();

        var displayTask = Display(system.OUT);
        var inputTask = Input(system.IN);

        await Task.WhenAll(systemTask, displayTask, inputTask);

        var result = displayTask.Result;

        return result;

        static async Task Input(ChannelWriter<long> input)
        {
            var program =
                """
                NOT A T
                OR T J
                NOT B T
                OR T J
                NOT C T
                OR T J
                AND H J
                AND D J
                """;

            Console.WriteLine(Validate(program.Split(Environment.NewLine)));

            foreach (var instruction in program.Split(Environment.NewLine).Append("RUN"))
            {
                foreach (var c in instruction)
                {
                    await input.WriteAsync(c);
                }
                await input.WriteAsync('\n');
            }
        }
    }

    public async Task<object> ForcePart2Async(Input input)
    {
        string[] instructions = ["NOT {0} {1}", "OR {0} {1}", "AND {0} {1}"];
        string[] sourceRegisters = ["E", "F", "G", "H", "I", "A", "B", "C", "D"];
        string[] destRegisters = ["T", "J"];

        var max = 0L;

        var allCases = instructions.Cartesian(sourceRegisters.Concat(destRegisters), destRegisters, string.Format).ToArray();


        PriorityQueue<string[], long>[] bestScanerios = new PriorityQueue<string[], long>[15];

        for (int i = 0; i < bestScanerios.Length; i++)
        {
            bestScanerios[i] = new PriorityQueue<string[], long>();
        }
        bestScanerios[0].Enqueue([], int.MaxValue);

        foreach (var program in EnumerableEx.Permutations(allCases, 3))
        {
            var val = Validate(program);
            if (val > max)
            {
                max = val;
            }
            bestScanerios[0].Enqueue(program, int.MaxValue - val);
        }

        while (true)
        {
            for (int i = 1; i < 15; i++)
            {
                var currentbests = bestScanerios[i - 1].Peek();
                Console.WriteLine(string.Join('\n', currentbests));
                Console.WriteLine(max);

                for (int ti = 0; ti < 1000 && bestScanerios[i - 1].Count > 0; ti++)
                {
                    var initialProgram = bestScanerios[i - 1].Dequeue();
                    var program = new string[initialProgram.Length + 1];
                    Array.Copy(initialProgram, program, initialProgram.Length);
                    foreach (var instruction in allCases)
                    {
                        program[^1] = instruction;
                        var validationScore = Validate(program);
                        var memory = new IntCode.Memory<long>(input.SelectArray(i => new MemCell<long> { Value = i }));
                        var system = new ProcessingUnit(memory);

                        var systemTask = system.Run();

                        var displayTask = Display(system.OUT);
                        var inputTask = Input(program, system.IN);

                        await Task.WhenAll(systemTask, displayTask, inputTask);
                        var result = displayTask.Result;

                        if (result > 0)
                        {
                            return result;
                        }
                        var maxProgram = new string[program.Length];
                        Array.Copy(program, maxProgram, program.Length);

                        bestScanerios[i].Enqueue(maxProgram, int.MaxValue - memory[754].Value - validationScore);
                    }
                }
            }

            for (int i = 1; i < bestScanerios.Length; i++)
            {
                var currentbests = bestScanerios[i].Peek();
                Console.WriteLine(string.Join('\n', currentbests));
                Console.WriteLine();
            }
        }

        static async Task Input(string[] program, ChannelWriter<long> input)
        {
            foreach (var instruction in program.Append("RUN"))
            {
                foreach (var c in instruction)
                {
                    await input.WriteAsync(c);
                }
                await input.WriteAsync('\n');
            }
        }
    }

    static async Task<long> Display(ChannelReader<long> output)
    {
        string[] sourceRegisters = ["A", "B", "C", "D", "E", "F", "G", "H", "I"];
        var showSourceRegisters = false;
        var registerI = 0;
        await Task.Yield();
        var result = 0L;
        while (await output.WaitToReadAsync())
        {
            var r = await output.ReadAsync();

            if (r < char.MaxValue)
            {
                //var c = (char)r;

                //if (c is '.' && showSourceRegisters)
                //{
                //    Console.Write(sourceRegisters[registerI++]);
                //    if (registerI == sourceRegisters.Length)
                //    {
                //        showSourceRegisters = false;
                //        registerI = 0;
                //    }
                //}
                //else
                //{
                //    showSourceRegisters = false;
                //    registerI = 0;
                //    Console.Write(c);
                //}

                //if (c is '@')
                //{
                //    showSourceRegisters = true;
                //}
            }
            else
            {
                result = r;
                break;
            }
        }
        return result;
    }

    static int Validate(string[] program)
    {
        ParameterExpression input = Expression.Parameter(typeof(int), "input");
        Expression T = Expression.Constant(false);
        Expression J = Expression.Constant(false);

        foreach (var instruction in program)
        {
            switch (instruction.Split(' '))
            {
                case [var op, var a, "T"]:
                    T = ToExpression(op, GetExpression(a, input, T, J), T);
                    break;
                case [var op, var a, "J"]:
                    J = ToExpression(op, GetExpression(a, input, T, J), J);
                    break;
            }
        }

        var p = Expression.Lambda<Func<int, bool>>(J, input).Compile();

        int[] jumps = [
            0b111111000,
            0b111111001,
            0b111111010,
            0b111111011,
            0b111111100,
            0b111111101,
            0b110010100,
            ];

        int[] walks = [
            0b000001111,
            0b000101011,
            ];

        var score = 0;

        foreach (var a in jumps)
        {
            if (p(a))
            {
                score++;
            }
        }

        foreach (var a in walks)
        {
            if (p(a) is false)
            {
                score++;
            }
        }
        return score;
    }

    private static Expression ToExpression(string op, Expression a, Expression b)
    {
        switch (op)
        {
            case "AND":
                return Expression.And(a, b);
            case "OR":
                return Expression.Or(a, b);
            case "NOT":
                return Expression.Not(a);
            default: throw new NotImplementedException();
        }
    }

    private static Expression GetExpression(string a, Expression input, Expression t, Expression j)
    {
        string[] sourceRegisters = ["A", "B", "C", "D", "E", "F", "G", "H", "I"];
        return a switch
        {
            "T" => t,
            "J" => j,
            _ => Expression.NotEqual(Expression.And(input, Expression.Constant(1 << Array.IndexOf(sourceRegisters, a))), Expression.Constant(0))
        };
    }
}
