namespace Tools;

public static class EnumerableEx
{
    public static long Product<T>(this IEnumerable<T> self, Func<T, long> f)
    {
        return self.Select(f).Aggregate(Operators.Multiply);
    }

    public static long Product(this IEnumerable<long> self)
    {
        return self.Aggregate(Operators.Multiply);
    }

    public static TResult[] SelectArray<T, TResult>(this T[] self, Func<T, TResult> f)
    {
        var result = new TResult[self.Length];

        for (var i = 0; i< self.Length; i++)
        {
            result[i] = f(self[i]);
        }

        return result;
    }

    public static IEnumerable<T> Gen<T>(T initial, Func<T,T> next)
    {
        var acc = initial;
        
        while (true) { yield return acc; acc = next(acc); }
    }

    public static IEnumerable<T[]> Combinations<T>(T[] operators, int length, int seed = 0)
    {
        var combinationCount = Math.Pow(operators.Length, length);

        var combination = new T[length];

        for (var i = seed; i < combinationCount; i++)
        {
            var v = i;
            for (var j = 0; j < length; j++)
            {
                var opIdx = v % operators.Length;
                v = v / operators.Length;
                combination[j] = operators[opIdx];
            }
            yield return combination;
        }
    }
}
