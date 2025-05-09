using System.Numerics;

namespace Days.Y2019.IntCode
{
    interface IMemRef<T> where T : notnull, INumber<T>
    {
        MemCell<T> Get(Memory<T> memory, Registers<T> registers, T addr);
    }

    class PositionModeRef<T> : IMemRef<T> where T : notnull, INumber<T>
    {
        public MemCell<T> Get(Memory<T> memory, Registers<T> registers, T addr) => memory[memory[addr].Value];
    }

    class ImmediateModeRef<T> : IMemRef<T> where T : notnull, INumber<T>
    {
        public MemCell<T> Get(Memory<T> memory, Registers<T> registers, T addr) => memory[addr];
    }

    class RelativeModeRef<T> : IMemRef<T> where T : notnull, INumber<T>
    {
        public MemCell<T> Get(Memory<T> memory, Registers<T> registers, T addr) => memory[memory[addr].Value + registers.RB];
    }
}
