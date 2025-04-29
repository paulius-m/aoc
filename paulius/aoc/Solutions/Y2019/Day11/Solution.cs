using Days.Y2019.IntCode;
using System.Threading.Channels;
using Tools;


namespace Days.Y2019.Day11;

using Input = string;
file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return await File.ReadAllTextAsync(this.GetInputFile("input"));
    }

    public async Task<object> Part1Async(Input input)
    {
        var memory = new IntCode.Memory<long>(input.Split(",").Select(v => new MemCell<long> { Value = long.Parse(v) }).ToArray());

        var brain = new Brain(memory);

        var robotTask = RunRobot(brain, 0);
        await brain.Run();
        return (await robotTask).Count();
    }

    public async Task<object> Part2Async(Input input)
    {
        var memory = new IntCode.Memory<long>(input.Split(",").Select(v => new MemCell<long> { Value = long.Parse(v) }).ToArray());

        var brain = new Brain(memory);

        var robotTask = RunRobot(brain, 1);
        await brain.Run();
        var map = await robotTask;

        var w = map.Keys.Max(k => k.Item1) + 1;
        var h = map.Keys.Max(h => h.Item2) + 1;

        char[][] image = new char[h][];

        foreach (var ((x, y), c) in map)
        {
            if (image[y] is null)
            {
                image[y] = new char[w];
                Array.Fill(image[y], ' ');
            }
            if (c is 1)
                image[y][x] = '#';
        }


        return string.Join('\n', image.Select(ca => new string(ca)));
    }

    private static async Task<Dictionary<(int, int), int>> RunRobot(Brain cpu, int initialColor)
    {
        await Task.Yield();
        var robot = (0, 0);
        var robotDirection = (0, -1);
        var painted = new Dictionary<(int, int), int>() { { robot, initialColor } };
        while (true)
        {
            var currentColor = painted.ContainsKey(robot) ? painted[robot] : 0;

            await cpu.IN.WriteAsync(currentColor);
            if (!await cpu.OUT.WaitToReadAsync())
            {
                break;
            }
            var color = await cpu.OUT.ReadAsync();
            var turn = await cpu.OUT.ReadAsync();

            painted[robot] = (int)color;
            robotDirection = turn switch
            {
                0 => (robotDirection.Item2, -robotDirection.Item1),
                1 => (-robotDirection.Item2, robotDirection.Item1)
            };

            //Console.CursorTop = robot.Item2;
            //Console.CursorLeft = robot.Item1;
            //Console.Write(color == 0 ? " " : "#");

            robot = (robot.Item1 + robotDirection.Item1, robot.Item2 + robotDirection.Item2);

            //Console.Clear();
            //Console.CursorTop = robot.Item2;
            //Console.CursorLeft = robot.Item1;
            //Console.Write(robotDirection switch
            //{
            //    (var a, 0) => a > 0 ? '>' : '<',
            //    (0, var a) => a > 0 ? 'V' : '^'
            //});
            //Console.WriteLine(painted.Count);
        }
        return painted;
    }
}

class Brain
{
    private Registers<long> Registers;
    private IntCode.Memory<long> Memory;
    private Decoder<long> Cpu;

    public ChannelWriter<long> IN { get; }
    public ChannelReader<long> OUT { get; }

    public Brain(IntCode.Memory<long> memory)
    {
        var channel = Channel.CreateUnbounded<long>();
        var outchannel = Channel.CreateUnbounded<long>();
        var registers = new Registers<long>
        {
            IN = channel,
            OUT = outchannel,
        };
        Memory = memory;
        Cpu = new Decoder<long>(memory, registers);
        Registers = registers;
        IN = channel;
        OUT = outchannel;
    }

    public async Task Run()
    {
        await Task.Yield();
        for (; !Registers.Halt;)
        {
            var instruction = Cpu.Decode(Memory[Registers.IP++]);

            var cells = instruction.Modes.Select(m => m[Registers.IP++]).ToArray();
            //Console.WriteLine(instruction.Exec.Method);
            await instruction.Exec(Registers, cells);
        }
        Registers.OUT.Complete();
    }
}