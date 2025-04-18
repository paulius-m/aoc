﻿namespace Tools;

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
}
