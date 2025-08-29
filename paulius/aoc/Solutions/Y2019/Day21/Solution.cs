using Days.Y2019.IntCode;
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

        var displayTask = Display(system.OUT, true);
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

        var displayTask = Display(system.OUT, true);
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
                AND H T
                OR T J
                AND D J
                RUN
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

    static async Task<long> Display(ChannelReader<long> output, bool consolePrint)
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
                if (consolePrint)
                {
                    var c = (char)r;

                    if (c is '.' && showSourceRegisters)
                    {
                        Console.Write(sourceRegisters[registerI++]);
                        if (registerI == sourceRegisters.Length)
                        {
                            showSourceRegisters = false;
                            registerI = 0;
                        }
                    }
                    else
                    {
                        showSourceRegisters = false;
                        registerI = 0;
                        Console.Write(c);
                    }

                    if (c is '@')
                    {
                        showSourceRegisters = true;
                    }
                }
            }
            else
            {
                result = r;
                break;
            }
        }
        return result;
    }
}
