using System.Collections;
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

    public class Memory<T> : IEnumerable<KeyValuePair<long, MemCell<T>>> where T : notnull, INumber<T> 
    {
        Dictionary<long, MemCell<T>> _memory;

        public Memory(MemCell<T>[] m)
        {
            long i = 0;
            _memory = m.ToDictionary(a => i++, a => a);
        }

        public MemCell<T> this[long addr]
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
                return this[long.CreateChecked(addr)];
            }
        }

        public IEnumerator<KeyValuePair<long, MemCell<T>>> GetEnumerator()
        {
            return _memory.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
