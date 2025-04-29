using Days.Y2019.IntCode;
using System.Threading.Channels;
using Tools;
using Input = System.String;

namespace Days.Y2019.Day07;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var input = await File.ReadAllTextAsync(this.GetInputFile("input"));
        return input;
    }

    public async Task<object> Part1Async(Input input)
    {
        var a = Enumerable.Range(0, 5);

        var all = from a1 in a
                  from a2 in a
                  from a3 in a
                  from a4 in a
                  from a5 in a
                  select new[] { a1, a2, a3, a4, a5 };

        var max = 0;
        foreach (var aa in all.Where(aa => aa.Distinct().Count() == aa.Length))
        {
            max = Math.Max(max, await TryInputsPart1(input, aa));
        }

        return max;
    }

    public async Task<object> Part2Async(Input input)
    {
        var a = Enumerable.Range(5, 5);

        var all = from a1 in a
                  from a2 in a
                  from a3 in a
                  from a4 in a
                  from a5 in a
                  select new[] { a1, a2, a3, a4, a5 };

        var max = 0;
        foreach (var aa in all.Where(aa => aa.Distinct().Count() == aa.Length))
        {
            max = Math.Max(max, await TryInputsPart2(input, aa));
        }

        return max;
    }

    private static async Task<int> TryInputsPart1(string input, int[] inputs)
    {
        var amps = new Amp[inputs.Length];
        var channels = Enumerable.Range(0, inputs.Length + 1).Select(i => Channel.CreateUnbounded<int>()).ToArray();

        for (int i = 0; i < amps.Length; i++)
        {
            await channels[i].Writer.WriteAsync(inputs[i]);

            var registers = new Registers<int>
            {
                IN = channels[i],
                OUT = channels[i + 1],
            };

            var memory = input.Split(",").Select(v => new MemCell<int> { Value = int.Parse(v) }).ToArray();

            amps[i] = new Amp
            {
                Cpu = new Decoder<int>(memory),
                Memory = memory,
                Registers = registers,
                ID = i
            };
        }

        await channels[0].Writer.WriteAsync(0);

        await Task.WhenAll(amps.Select(a => a.Run()));

        return await channels[^1].Reader.ReadAsync();
    }

    private static async Task<int> TryInputsPart2(string input, int[] inputs)
    {
        var amps = new Amp[inputs.Length];
        var channels = Enumerable.Range(0, inputs.Length).Select(i => Channel.CreateUnbounded<int>()).ToArray();

        for (int i = 0; i < amps.Length; i++)
        {
            await channels[i].Writer.WriteAsync(inputs[i]);

            var registers = new Registers<int>
            {
                IN = channels[i],
                OUT = channels[(i + 1 + channels.Length) % channels.Length],
            };

            var memory = input.Split(",").Select(v => new MemCell<int> { Value = int.Parse(v) }).ToArray();

            amps[i] = new Amp
            {
                Cpu = new Decoder<int>(memory),
                Memory = memory,
                Registers = registers
            };
        }

        await channels[0].Writer.WriteAsync(0);

        await Task.WhenAll(amps.Select(a => a.Run()));

        return await channels[0].Reader.ReadAsync();
    }
}
file class Amp
{
    public Registers<int> Registers;
    public MemCell<int>[] Memory;
    public Decoder<int> Cpu;
    public int ID;

    public async Task Run()
    {
        await Task.Yield();
        for (; !Registers.Halt;)
        {
            var instruction = Cpu.Decode(Memory[Registers.IP++]);

            var cells = instruction.Modes.Select(m => m[Registers.IP++]).ToArray();
            //Console.WriteLine($"{ID} {instruction.Exec.Method}");
            await instruction.Exec(Registers, cells);
        }
        //Console.WriteLine($"{ID} HALTED");
    }
}