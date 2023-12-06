namespace Tools;

public static class FuncEx
{
    public static Func<T1, TR> Memoize<T1, TR>(Func<T1, TR> f) where T1: notnull
    {
        var cache = new Dictionary<T1, TR>();
        return (t1) =>
            cache[t1] = cache.TryGetValue(t1, out var r) ? r : f(t1);
    }

    public static Func<T1, TR> Memoize<T1, TK, TR>(Func<T1, TR> f, Func<T1, TK> keyT) where T1 : notnull where TK : notnull
    {
        var cache = new Dictionary<TK, TR>();
        return (t1) => {
            var key = keyT(t1);
            return cache[key] = cache.TryGetValue(key, out var r) ? r : f(t1);
        };
    }
}
