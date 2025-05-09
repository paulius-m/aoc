using System.Numerics;
using System.Threading.Channels;

namespace Days.Y2019.IntCode
{
    public class MemCell<T> where T: notnull
    {
        public T Value = default;
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Memory<T> where T : notnull, INumber<T>
    {
        Dictionary<int, MemCell<T>> _memory;

        public Memory(MemCell<T>[] m)
        {
            var i = 0;
            _memory = m.ToDictionary(a => i++, a => a);
        }
        public MemCell<T> this[int addr]
        {
            get
            {
                if (_memory.TryGetValue(addr, out var value))
                {
                    return value;
                }

                var n = new MemCell<T>();
                _memory[addr] = n;
                return n;
            }
        }

        public MemCell<T> this[T addr]
        {
            get
            {
                return this[int.CreateChecked(addr)];
            }
        }
    }

    public class Registers<T> where T : notnull
    {
        public T IP = default;
        public bool Halt = false;
        public T RB = default;

        public ChannelReader<T> IN;
        public ChannelWriter<T> OUT;
    }
}
