using MoreLinq;
using System.Collections.Frozen;
using Tools;

namespace Days.Day07;
using Input = Hand[];

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                let ss = l.Split()
                select ss switch { [var cards, var bid] => new Hand(cards, long.Parse(bid)) }).ToArray();
    }

    public object Part1(Input i)
    {
        return i.Order(Hand.Comparer).Select((h, i) => (i + 1) * h.Bid).Sum();
    }

    public object Part2(Input i)
    {
        return i.Select(h => new JockerHand(h)).Order(JockerHand.Comparer).Select((h, i) => (i + 1) * h.Bid).Sum();
    }
}


enum HandType
{
    HighCard = 1,
    OnePair,
    TwoPair,
    ThreeOfKind,
    FullHouse,
    FourOfKind,
    FiveOfKind
}

file interface  IHand
{
    string Cards { get; } 
    long Bid { get; }
    HandType HandType { get; }

    protected static HandType MapToType((char Key, int Count)[] grouped)
    {
        return grouped switch
        {
            [_] => HandType.FiveOfKind,
            [{ Count: 4 }, _] => HandType.FourOfKind,
            [{ Count: 3 }, { Count: 2 }] => HandType.FullHouse,
            [{ Count: 3 }, ..] => HandType.ThreeOfKind,
            [{ Count: 2 }, { Count: 2 }, _] => HandType.TwoPair,
            [{ Count: 2 }, ..] => HandType.OnePair,
            { Length: 5 } => HandType.HighCard
        };
    }
}

file record Hand(string Cards, long Bid) : IHand
{
    public HandType HandType { get; } = IHand.MapToType(
        (from c in Cards
         group c by c into g
         select (g.Key, Count: g.Count()) into gg
         orderby gg.Count descending
         select gg).ToArray() )
        ;

    static FrozenDictionary<char, int> _priority = new Dictionary<char, int>
    {
        ['A'] = 14,
        ['K'] = 13,
        ['Q'] = 12,
        ['J'] = 11,
        ['T'] = 10,
        ['9'] = 9,
        ['8'] = 8,
        ['7'] = 7,
        ['6'] = 6,
        ['5'] = 5,
        ['4'] = 4,
        ['3'] = 3,
        ['2'] = 2
    }.ToFrozenDictionary();

    public static IComparer<Hand> Comparer { get; } = new HandComparer(_priority);
}

file record JockerHand : IHand
{
    public JockerHand(Hand hand)
    {
        (Cards, Bid) = hand;

        var grouped = (from c in Cards
                       group c by c into g
                       select (g.Key, Count: g.Count()) into gg
                       orderby gg.Count descending, _jokerPriority[gg.Key] descending
                       select gg).ToArray();

        if (grouped.All(g => g.Key is not 'J'))
        {
            HandType = IHand.MapToType(grouped);
        }
        else
        {
            if (grouped.Length is 1 && grouped[0].Key is 'J' )
            {
                var jockerGrouped = grouped;
                jockerGrouped[0] = jockerGrouped[0] with { Key = 'K' };
                HandType = IHand.MapToType(jockerGrouped);
            }
            else
            {
                var jocker = grouped.First(g => g.Key is 'J');
                var jockerGrouped = grouped.Where(g => g.Key is not 'J').ToArray();
                jockerGrouped[0] = jockerGrouped[0] with { Count = jockerGrouped[0].Count + jocker.Count };
                HandType = IHand.MapToType(jockerGrouped);
            }
        }
    }

    public HandType HandType { get; }
    public string Cards { get; }
    public long Bid { get; }

    static FrozenDictionary<char, int> _jokerPriority = new Dictionary<char, int>
    {
        ['A'] = 14,
        ['K'] = 13,
        ['Q'] = 12,
        ['T'] = 10,
        ['9'] = 9,
        ['8'] = 8,
        ['7'] = 7,
        ['6'] = 6,
        ['5'] = 5,
        ['4'] = 4,
        ['3'] = 3,
        ['2'] = 2,
        ['J'] = 1,
    }.ToFrozenDictionary();

    public static IComparer<JockerHand> Comparer { get; } = new HandComparer(_jokerPriority);

}


file class HandComparer(IDictionary<char, int> Priority) : IComparer<IHand>
{
    public int Compare(IHand? x, IHand? y)
    {
        var c = x.HandType - y.HandType;
        if (c != 0) return c;
        var comp = x.Cards.Zip(y.Cards, (a, b) => Priority[a] - Priority[b]);
        return comp.TakeUntil(c => c != 0).Last();
    }
}
