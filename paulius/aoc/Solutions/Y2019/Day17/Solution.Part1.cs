using Days.Y2019.IntCode;
using System.Threading.Channels;

namespace Days.Y2019.Day17;

internal partial class Solution
{

    private static async Task<Dictionary<(int x, int y), char>> GetScreen(string input)
    {
        var memory = new IntCode.Memory<long>(input.Split(",").Select(v => new MemCell<long> { Value = long.Parse(v) }).ToArray());

        var system = new ProcessingUnit(memory);

        var systemTask = system.Run();
        var inputTask = Input(system.IN, system.OUT);

        await systemTask;
        var screen = await inputTask;
        return screen;
    }

    private static int CalculateResult(Dictionary<(int x, int y), char> screen)
    {
        var near = (from x in new[] { 1, 0, -1 }
                    from y in new[] { 1, 0, -1 }
                    where x is 0 || y is 0
                    select (x, y)).ToArray();

        var cross = screen
            .Where(kv => kv.Value is '#')
            .Where(kv =>
               near
                   .Select(n => screen.GetValueOrDefault((n.x + kv.Key.x, n.y + kv.Key.y)))
                   .Where(c => c is '#').Count() == 5
            ).ToArray();

        var result = cross.Select(kv => kv.Key.x * kv.Key.y).Sum();
        return result;
    }


    private static async Task<Dictionary<(int x, int y), char>> Input(ChannelWriter<long> input, ChannelReader<long> instructions)
    {
        await Task.Yield();
        var screen = new Dictionary<(int x, int y), char>();
        var row = 0;
        var column = 0;
        while (true)
        {
            // await input.WriteAsync(i);

            if (await instructions.WaitToReadAsync())
            {
                var r = (char)await instructions.ReadAsync();
                if (r is '\n')
                {
                    row++;
                    column = 0;
                }
                else
                {
                    screen[(column, row)] = r;
                    column++;
                }
                Console.Write(r);

            }
            else
            {
                break;
            }
        }
        input.Complete();

        return screen;
    }
}
