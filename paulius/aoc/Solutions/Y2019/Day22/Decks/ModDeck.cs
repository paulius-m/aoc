namespace Days.Y2019.Day22.Decks;

class ModDeck : IDeck
{
    private long _length;
    private long _value;
    public ModDeck(long length, int index)
    {
        _length = length;
        _value = index;
    }

    public long CardAt(long index)
    {
        return _value;
    }

    public IDeck CutNCards(int n)
    {
        _value = (_value - n + _length) % _length;
        return this;
    }

    public IDeck DealIntoNewStack()
    {
        DealWithIncrementN(_length - 1);
        CutNCards(1);
        return this;
    }

    public IDeck DealWithIncrementN(long n)
    {
        _value = _value * n % _length;
        return this;
    }

    public long IndexOf(long card)
    {
        return _value;
    }
}
