﻿using Days.Y2019.IntCode;
using System.Linq;
using System.Threading.Channels;
using Tools;

namespace Days.Y2019.Day05;
using Input = (Decoder<int>, IntCode.Memory<int>);
file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var input = await File.ReadAllTextAsync(this.GetInputFile("input"));
        var memory = new IntCode.Memory<int>(input.Split(",").Select(v => new MemCell<int> { Value = int.Parse(v) }).ToArray());
        var cpu = new Decoder<int>();
        return (cpu, memory);
    }

    public async Task<object> Part1Async(Input i)
    {
        var (cpu, memory) = i;
        var IN = Channel.CreateUnbounded<int>();
        var OUT = Channel.CreateUnbounded<int>();
        var registers = new Registers<int>() { IN = IN, OUT = OUT};
        await IN.Writer.WriteAsync(1);
        for (; !registers.Halt;)
        {
            var instruction = cpu.Decode(memory[registers.IP++]);

            var cells = instruction.Modes.Select(m => m.Get(memory, registers, registers.IP++)).ToArray();

            await instruction.AsycOp(registers, cells);
        }
        OUT.Writer.Complete();
        return await OUT.Reader.ReadAllAsync().LastAsync();
    }

    public async Task<object> Part2Async(Input i)
    {
        var (cpu, memory) = i;
        var IN = Channel.CreateUnbounded<int>();
        var OUT = Channel.CreateUnbounded<int>();
        var registers = new Registers<int>() { IN = IN, OUT = OUT };
        await IN.Writer.WriteAsync(5);
        for (; !registers.Halt;)
        {
            var instruction = cpu.Decode(memory[registers.IP++]);

            var cells = instruction.Modes.Select(m => m.Get(memory, registers, registers.IP++)).ToArray();

            await instruction.AsycOp(registers, cells);
        }
        return await OUT.Reader.ReadAsync();
    }
}