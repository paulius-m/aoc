using Days.Y2019.Day22.Decks;
using Sprache;
using Tools;

namespace Days.Y2019.Day22;

file class Solution : ISolution<(IDeckCommand[] commands, int size, int value, long, long, int)>
{
    public async Task<(IDeckCommand[] commands, int size, int value, long, long, int)> LoadInput()
    {
        if (true)
        {
            var input = await File.ReadAllTextAsync(this.GetInputFile("input"));
            return (Parser.Parse(input), 10007, 2019, 119315717514047, 101741582076661, 2020);
        }
        else
        {
            var input = await File.ReadAllTextAsync(this.GetInputFile("test"));
            return (Parser.Parse(input), 10, 2, 10, 100, 2);
        }
    }

    public object Part1((IDeckCommand[] commands, int size, int value, long, long, int) input)
    {
        var (commands, size, value, _, _, _) = input;
        IDeck deck = new ArrayDeck(size);

        var transformed = commands.Aggregate(deck, (d, c) => c.Transform(d));
        return transformed.IndexOf(value);
    }

    public object Part2((IDeckCommand[] commands, int size, int value, long, long, int) input)
    {
        var (commands, _, _, size, cycleCount, value) = input;

        return 0;
    }

    static Parser<IDeckCommand> DealIntoNewStack = from _ in Parse.String("deal into new stack")
                                                   select new DealIntoNewStack();

    static Parser<IDeckCommand> CutNCards = from _ in Parse.String("cut ")
                                            from sign in Parse.Char('-').Optional()
                                            from n in Parse.Number.Select(int.Parse)
                                            select new CutNCards(sign.IsEmpty ? n : -n);

    static Parser<IDeckCommand> DealWithIncrementN = from _ in Parse.String("deal with increment ")
                                                     from n in Parse.Number.Select(int.Parse)
                                                     select new DealWithIncrementN(n);


    public static Parser<IDeckCommand[]> Parser = from l in DealIntoNewStack.Or(CutNCards).Or(DealWithIncrementN).DelimitedBy(Parse.LineTerminator)
                                                  select l.ToArray();
}

file interface IDeckCommand
{
    IDeck Transform(IDeck d);
}

file class DealIntoNewStack : IDeckCommand
{
    public IDeck Transform(IDeck d)
    {
        return d.DealIntoNewStack();
    }
}

file class CutNCards : IDeckCommand
{
    public CutNCards(int n)
    {
        N = n;
    }

    public int N { get; }

    public IDeck Transform(IDeck d)
    {
        return d.CutNCards(N);
    }
}

file class DealWithIncrementN : IDeckCommand
{
    public DealWithIncrementN(int n)
    {
        N = n;
    }

    public int N { get; }

    public IDeck Transform(IDeck d)
    {
        return d.DealWithIncrementN(N);
    }
}