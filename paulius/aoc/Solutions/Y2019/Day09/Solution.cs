using Days.Y2019.IntCode;
using System.Numerics;
using System.Threading.Channels;
using Tools;
using Input = System.String;

namespace Days.Y2019.Day09
{
    file class Solution : ISolution<Input>
    {
        public async Task<Input> LoadInput()
        {
            return await File.ReadAllTextAsync(this.GetInputFile("input"));
        }

        public Task<object> Part1Async(Input input)
        {
            return Run(input, 1);
        }

        private static async Task<object> Run(string input, BigInteger i)
        {
            var memory = new IntCode.Memory<BigInteger>(input.Split(",").Select(v => new MemCell<BigInteger> { Value = BigInteger.Parse(v) }).ToArray());
            var IN = Channel.CreateUnbounded<BigInteger>();
            var OUT = Channel.CreateUnbounded<BigInteger>();

            var registers = new Registers<BigInteger>() { IN = IN, OUT = OUT };
            var cpu = new Decoder<BigInteger>(memory, registers);
            await IN.Writer.WriteAsync(i);
            for (; !registers.Halt;)
            {
                var instruction = cpu.Decode(memory[registers.IP++]);

                var cells = instruction.Modes.Select(m => m[registers.IP++]).ToArray();

                await instruction.Exec(registers, cells);
            }

            return await OUT.Reader.ReadAsync();
        }

        public Task<object> Part2Async(Input input)
        {
            return Run(input, 2);
        }
    }
}