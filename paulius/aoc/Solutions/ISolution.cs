namespace Days;

public interface ISolution<TInput>
{
    Task<TInput> LoadInput();
    object Part1(TInput t) => "";
    Task<object> Part1Async(TInput t) => Task.FromResult(Part1(t));
    object Part2(TInput t) => "";
    Task<object> Part2Async(TInput t) => Task.FromResult(Part2(t));
}
