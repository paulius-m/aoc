namespace Days.Y2019.Day22.Decks;

interface IDeck
{
    long CardAt(long index);
    IDeck CutNCards(int n);
    IDeck DealIntoNewStack();
    IDeck DealWithIncrementN(long n);
    long IndexOf(long card);
}
