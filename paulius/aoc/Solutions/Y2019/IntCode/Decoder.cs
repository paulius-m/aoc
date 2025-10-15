using System.Numerics;

namespace Days.Y2019.IntCode
{
    class Decoder<T> where T : notnull, INumber<T>
    {
        private static readonly IMemRef<T>[] _modes =
            [
                new PositionModeRef<T>(),
                new ImmediateModeRef<T>(),
                new RelativeModeRef<T>()
            ];

        public Instruction<T> Decode(MemCell<T> opCell)
        {
            var v = int.CreateChecked(opCell.Value);

            var op = v % 100;
            var modes = new[] { v / 100 % 10, v / 1000 % 10, v / 10000 % 10 }.Select(v => _modes[v]);

            var instruction = op switch
            {
                1 => new Instruction<T>
                {
                    Modes = modes.Take(3).ToArray(),
                    Op = InstructionOperations.Add
                },
                2 => new Instruction<T>
                {
                    Modes = modes.Take(3).ToArray(),
                    Op = InstructionOperations.Multiply
                },
                3 => new Instruction<T>
                {
                    Modes = modes.Take(1).ToArray(),
                    AsycOp = InstructionOperations.Input
                },
                4 => new Instruction<T>
                {
                    Modes = modes.Take(1).ToArray(),
                    AsycOp = InstructionOperations.Output
                },
                5 => new Instruction<T>
                {
                    Modes = modes.Take(2).ToArray(),
                    Op = InstructionOperations.JumpIfNotZero
                },
                6 => new Instruction<T>
                {
                    Modes = modes.Take(2).ToArray(),
                    Op = InstructionOperations.JumpIfZero
                },
                7 => new Instruction<T>
                {
                    Modes = modes.Take(3).ToArray(),
                    Op = InstructionOperations.LessThen
                },
                8 => new Instruction<T>
                {
                    Modes = modes.Take(3).ToArray(),
                    Op = InstructionOperations.Equal
                },
                9 => new Instruction<T>
                {
                    Modes = modes.Take(1).ToArray(),
                    Op = InstructionOperations.SetRB
                },
                99 => new Instruction<T>
                {
                    Op = InstructionOperations.Halt
                }
            };

            return instruction;
        }
    }
}
