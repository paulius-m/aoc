using System.Numerics;

namespace Days.Y2019.IntCode
{

    interface IMemRef<T> where T : notnull, INumber<T>
    {
        MemCell<T> this[T addr] { get; }
    }

    class PositionModeRef<T> : IMemRef<T> where T : notnull, INumber<T>
    {
        private readonly Memory<T> _memory;

        public PositionModeRef(Memory<T> memory)
        {
            _memory = memory;
        }

        public MemCell<T> this[T addr] => _memory[_memory[addr].Value];
    }

    class ImmediateModeRef<T> : IMemRef<T> where T : notnull, INumber<T>
    {
        private readonly Memory<T> _memory;

        public ImmediateModeRef(Memory<T> memory)
        {
            _memory = memory;
        }

        public MemCell<T> this[T addr] => _memory[addr];
    }

    class RelativeModeRef<T> : IMemRef<T> where T : notnull, INumber<T>
    {
        private readonly Registers<T> _registers;
        private readonly Memory<T> _memory;

        public RelativeModeRef(Registers<T> registers, Memory<T> memory)
        {
            _registers = registers;
            _memory = memory;
        }

        public MemCell<T> this[T addr] => _memory[_memory[addr].Value + _registers.RB];
    }
}
