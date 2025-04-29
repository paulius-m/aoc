using System.Numerics;

namespace Days.Y2019.IntCode
{
    class Decoder<T> where T : notnull, INumber<T>
    {
        private readonly IMemRef<T>[] _modes;

        public Decoder(MemCell<T>[] memory)
        {
            var m = new Memory<T>(memory);
            _modes = new IMemRef<T>[]
            {
                new PositionModeRef<T>(m),
                new ImmediateModeRef<T>(m)
            };
        }

        public Decoder(Memory<T> memory, Registers<T> registers)
        {
            _modes = new IMemRef<T>[]
            {
                new PositionModeRef<T>(memory),
                new ImmediateModeRef<T>(memory),
                new RelativeModeRef<T>(registers, memory)
            };
        }

        public Instruction<T> Decode(MemCell<T> opCell)
        {
            var v = int.CreateChecked(opCell.Value);

            var op = v % 100;
            var modes = new[] { v / 100 % 10, v / 1000 % 10, v / 10000 % 10 }.Select(v => _modes[v]);

            return op switch
            {
                1 => new Instruction<T>
                {
                    Modes = modes.Take(3),
                    Exec = InstructionOperations.Add
                },
                2 => new Instruction<T>
                {
                    Modes = modes.Take(3),
                    Exec = InstructionOperations.Multiply
                },
                3 => new Instruction<T>
                {
                    Modes = modes.Take(1),
                    Exec = InstructionOperations.Input
                },
                4 => new Instruction<T>
                {
                    Modes = modes.Take(1),
                    Exec = InstructionOperations.Output
                },
                5 => new Instruction<T>
                {
                    Modes = modes.Take(2),
                    Exec = InstructionOperations.JumpIf<T>(true)
                },
                6 => new Instruction<T>
                {
                    Modes = modes.Take(2),
                    Exec = InstructionOperations.JumpIf<T>(false)
                },
                7 => new Instruction<T>
                {
                    Modes = modes.Take(3),
                    Exec = InstructionOperations.LessThen
                },
                8 => new Instruction<T>
                {
                    Modes = modes.Take(3),
                    Exec = InstructionOperations.Equal
                },
                9 => new Instruction<T>
                {
                    Modes = modes.Take(1),
                    Exec = InstructionOperations.SetRB
                },
                99 => new Instruction<T>
                {
                    Exec = InstructionOperations.Halt
                }
            };
        }
    }
}
