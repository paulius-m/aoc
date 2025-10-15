using System.Numerics;

namespace Days.Y2019.Day22.Decks;

class ReverseModDeck : IDeck
{
    private long _length;
    private long _value;
    public ReverseModDeck(long length, int index)
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
        checked
        {
            _value = (_value + n + _length) % _length;
        }
        return this;
    }

    public IDeck DealIntoNewStack()
    {

        CutNCards(1);
        DealWithIncrementN(_length - 1);
        return this;
    }

    // Uses Extended Euclidean Algorithm to find gcd and inverse
    public IDeck DealWithIncrementN(long n)
    {
        var inv = modInverse(n, _length);

        if (inv == -1)
        {
            throw new ArgumentException("No inverse was found");
        }
        checked
        {
            _value = long.CreateChecked(_value * inv % _length);
        }
        return this;

        BigInteger modInverse(long n, long mod)
        {
            checked
            {
                BigInteger t = 0, newt = 1;
                BigInteger r = mod, newr = n;

                while (newr != 0)
                {
                    var q = r / newr;
                    var temp = newt;
                    newt = t - q * newt;
                    t = temp;

                    temp = newr;
                    newr = r - q * newr;
                    r = temp;
                }

                if (r > 1) return -1; // no inverse
                if (t < 0) t += mod;
                return t;
            }
        }
    }

    public long IndexOf(long card)
    {
        return _value;
    }
}
