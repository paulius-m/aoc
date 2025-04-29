namespace Days.Y2019.IntCode
{
    class Decoder
    {
        private readonly IMemRef[] _modes;

        public Decoder(MemCell[] memory)
        {
            _modes = new IMemRef[]
            {
                new PositionModeRef(memory),
                new ImmediateModeRef(memory)
            };
        }

        public Instruction Decode(MemCell opCell)
        {
            var v = opCell.Value;

            var op = v % 100;
            var modes = new[] { v / 100 % 10, v / 1000 % 10, v / 10000 % 10 }.Select(v => _modes[v]);

            return op switch
            {
                1 => new Instruction
                {
                    Modes = modes.Take(3),
                    Exec = InstructionOperations.Add
                },
                2 => new Instruction
                {
                    Modes = modes.Take(3),
                    Exec = InstructionOperations.Multiply
                },
                3 => new Instruction
                {
                    Modes = modes.Take(1),
                    Exec = InstructionOperations.Input
                },
                4 => new Instruction
                {
                    Modes = modes.Take(1),
                    Exec = InstructionOperations.Output
                },
                5 => new Instruction
                {
                    Modes = modes.Take(2),
                    Exec = InstructionOperations.JumpIf(true)
                },
                6 => new Instruction
                {
                    Modes = modes.Take(2),
                    Exec = InstructionOperations.JumpIf(false)
                },
                7 => new Instruction
                {
                    Modes = modes.Take(3),
                    Exec = InstructionOperations.LessThen
                },
                8 => new Instruction
                {
                    Modes = modes.Take(3),
                    Exec = InstructionOperations.Equal
                },
                99 => new Instruction
                {
                    Exec = InstructionOperations.Halt
                }
            };
        }
    }
}
