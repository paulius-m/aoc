namespace Days.Y2019.Day22.Decks;

class OffsetIncrementDeck : IDeck
{
    private long _offset = 0;
    private long _increment = 1;
    private long _length;

    public OffsetIncrementDeck(long length)
    {
        _length = length;
    }


    public long CardAt(long index)
    {
        return _offset + _increment * index;
    }

    public IDeck CutNCards(int n)
    {
        _offset += _increment * n;
        return this;
    }

    public IDeck DealIntoNewStack()
    {
        _increment *= -1;
        _offset += _increment;
        return this;
    }

    public IDeck DealWithIncrementN(long n)
    {
        _increment *= ModularPow(n, _length - 2, _length);
        return this;
    }

    long ModularPow(long b, long exponent, long modulus)
    {
        if (modulus == 1)
            return 0;
        long result = 1;

        b = b % modulus;

        while (exponent > 0)
        {
            if (exponent % 2 == 1)
                result = result * b % modulus;
            exponent = exponent >> 1;
            b = b * b % modulus;
        }
        return result;
    }


    public long IndexOf(long card)
    {
        throw new NotImplementedException();
    }
}
