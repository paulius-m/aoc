using System.Reflection;

namespace Days;

public class DayExecutor
{
    public static async Task RunSolution<TSolution, T>() where TSolution: ISolution<T>, new()
    {
        var s = new TSolution();
        Console.WriteLine(await s.Part1Async(await s.LoadInput()));
        Console.WriteLine(await s.Part2Async(await s.LoadInput()));
    }

    public static async Task Run(string? day = null)
    {
        var runnerT = typeof(DayExecutor);
        var type = day is { } d ? FindDay(runnerT.Assembly, d) : FindLatestDay(runnerT.Assembly);
        
        var m = runnerT.GetMethod("RunSolution")!.MakeGenericMethod(new[] { type }.Concat(type.GetInterfaces()[0].GetGenericArguments()).ToArray());
        await (Task)m.Invoke(null, null)!;
    }

    private static Type FindLatestDay(Assembly assembly)
    {
        var solutions = GetSolutions(assembly);

        return solutions
            .MaxBy(t => Convert.ToInt32(new string(t.FullName!.Split(".")[0..^1].SelectMany(d => d.Where(char.IsNumber)).ToArray())))!;
    }

    private static IEnumerable<Type> GetSolutions(Assembly assembly)
    {
        var solutionI = typeof(ISolution<>);
        var solutions = assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == solutionI));
        return solutions;
    }

    private static Type FindDay(Assembly assembly, string day)
    {
        var solutions = GetSolutions(assembly);

        return solutions
            .First(t => t.FullName!.Split(".")[1] == day)!;
    }
}
