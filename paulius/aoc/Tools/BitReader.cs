namespace Tools;

public class BitReader
{
    private readonly byte[] _data;
    private int _offset;
    private int _buff;
    private int _index;

    public BitReader(byte[] data)
    {
        _data = data;
        _offset = 0;
        _index = 0;
        _buff = _data[0];
    }

    public long ReadBits(int count)
    {
        long result = 0L;

        for (int i = 0; i < count; i++)
        {
            long b = (_buff & 0x80) >> 7;
            result <<= 1;
            result |= b;
            _buff <<= 1;
            _offset++;
            if (_offset == 8 && ++_index < _data.Length)
            {
                _buff = _data[_index];
                _offset = 0;
            }
        }
        return result;
    }

    public int CurrentPointer => _index * 8 + _offset;
}
