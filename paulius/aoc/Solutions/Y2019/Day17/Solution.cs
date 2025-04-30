using Days.Y2019.IntCode;
using Tools;

namespace Days.Y2019.Day17;

internal partial class Solution : ISolution<string>
{
    public async Task<string> LoadInput()
    {
        return await File.ReadAllTextAsync(this.GetInputFile("input"));
    }

    public async Task<object> Part1Async(string input)
    {
        Dictionary<(int x, int y), char> screen = await GetScreen(input);

        return CalculateResult(screen);
    }

    public async Task<object> Part2Async(string input)
    {
        var screen = await GetScreen(input);

        var memory = new IntCode.Memory<long>(input.Split(",").Select(v => new MemCell<long> { Value = long.Parse(v) }).ToArray());
        memory[0].Value = 2;

        var system = new ProcessingUnit(memory);

        var systemTask = system.Run();
        var inputTask = Input(system.IN, system.OUT, GetFunctions(screen));

        await systemTask;

        var result = await inputTask;

        return result;
    }
}