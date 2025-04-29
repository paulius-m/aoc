using Days.Y2019.IntCode;
using Tools;

namespace Days.Y2019.Day05;
using Input = (Decoder, MemCell[], Registers);
file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var input = await File.ReadAllTextAsync(this.GetInputFile("input"));
        var memory = input.Split(",").Select(v => new MemCell { Value = int.Parse(v) }).ToArray();
        var registers = new Registers();
        var cpu = new Decoder(memory);
        return (cpu, memory, registers);
    }

    public object Part1(Input i)
    {
        var (cpu, memory, registers) = i;
        for (; !registers.Halt;)
        {
            var instruction = cpu.Decode(memory[registers.IP++]);

            var cells = instruction.Modes.Select(m => m[registers.IP++]).ToArray();

            instruction.Exec(registers, cells);
        }
        return 0;
    }

    public object Part2(Input i)
    {
        var (cpu, memory, registers) = i;
        for (; !registers.Halt;)
        {
            var instruction = cpu.Decode(memory[registers.IP++]);

            var cells = instruction.Modes.Select(m => m[registers.IP++]).ToArray();

            instruction.Exec(registers, cells);
        }
        return 0;
    }
}