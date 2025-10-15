namespace Days.Y2019.Day22.Decks;

class ArrayDeck : IDeck
{
    private int[] _cards;

    public ArrayDeck(int deckSize)
    {
        _cards = Enumerable.Range(0, deckSize).ToArray();
    }

    public IDeck DealIntoNewStack()
    {
        _cards = _cards.Reverse().ToArray();
        return this;
    }

    public IDeck CutNCards(int n)
    {
        var l = Math.Abs(n);
        if (n > 0)
        {
            _cards = [.. _cards[l..], .. _cards[0..l]];
        }
        else
        {
            _cards = [.. _cards[^l..], .. _cards[0..^l]];
        }
        return this;
    }

    public IDeck DealWithIncrementN(long n)
    {
        var space = new int[_cards.Length];

        for (int i = 0; i < _cards.Length; i++)
        {
            space[n * i % _cards.Length] = _cards[i];
        }
        _cards = space;
        return this;
    }

    public long IndexOf(long card)
    {
        return Array.IndexOf(_cards, (int)card);
    }

    public long CardAt(long index)
    {
        return _cards[index];
    }

    public override string ToString()
    {
        return string.Join(" ", _cards);
    }
}
