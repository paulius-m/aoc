namespace Days.Y2019.IntCode
{

    interface IMemRef
    {
        MemCell this[int addr] { get; }
    }

    class PositionModeRef : IMemRef
    {
        private readonly MemCell[] _memory;

        public PositionModeRef(MemCell[] memory)
        {
            _memory = memory;
        }

        public MemCell this[int addr] => _memory[_memory[addr].Value];
    }

    class ImmediateModeRef : IMemRef
    {
        private readonly MemCell[] _memory;

        public ImmediateModeRef(MemCell[] memory)
        {
            _memory = memory;
        }

        public MemCell this[int addr] => _memory[addr];
    }
}
