using Days.Y2019.IntCode;
using System.Net;
using System.Threading.Channels;
using Tools;
using Input = long[];

namespace Days.Y2019.Day23;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (await File.ReadAllTextAsync(this.GetInputFile("input"))).Split(',').SelectArray(long.Parse);
    }

    public async Task<object> Part1Async(Input input)
    {
        var (computers, tasks) = await GetComputers(input);
        Queue<(long, long)>[] q = new Queue<(long, long)>[computers.Length];
        for (int i = 0; i < q.Length; i++)
        {
            q[i] = new Queue<(long, long)>();
        }
        while (true)
        {
            for (int i = 0; i < computers.Length; i++)
            {
                var computer = computers[i];

                if (q.All(qq => qq.Count is 0))
                {
                    await computer.IN.WriteAsync(-1);
                }
                else while (q[i].Count > 0)
                {
                    var (qx, qy) = q[i].Dequeue();
                    await computer.IN.WriteAsync(qx);
                    await computer.IN.WriteAsync(qy);
                }

                if (!computer.OUT.TryRead(out var address))
                {
                    continue;
                }

                var x = await computer.OUT.ReadAsync();
                var y = await computer.OUT.ReadAsync();
                if (address != 255)
                {
                    q[address].Enqueue((x, y));
                }
                else
                {
                    return y;
                }
            }
        }
    }

    public async Task<object> Part2Async(Input input)
    {
        long natX = 0, natY = 0;
        var previousNatY = new HashSet<long>();

        var (computers, tasks) = await GetComputers(input);
        Queue<(long, long)>[] q = new Queue<(long, long)>[computers.Length];
        for (int i = 0; i < q.Length; i++)
        {
            q[i] = new Queue<(long, long)>();
        }
        var idleCycles = 0;
        while (true)
        {
            for (int i = 0; i < computers.Length; i++)
            {
                var computer = computers[i];

                if (q.All(qq => qq.Count is 0))
                {
                    await computers[i].IN.WriteAsync(-1);
                }
                else while (q[i].Count > 0)
                    {
                        var (qx, qy) = q[i].Dequeue();
                        await computers[i].IN.WriteAsync(qx);
                        await computers[i].IN.WriteAsync(qy);
                    }

                if (!computer.OUT.TryRead(out var address))
                {
                    continue;
                }

                var x = await computer.OUT.ReadAsync();
                var y = await computer.OUT.ReadAsync();
                if (address != 255)
                {
                    q[address].Enqueue((x, y));
                }
                else
                {
                    natX = x;
                    natY = y;
                }
            }

            var idle = true;
            for (int i = 0; i < computers.Length; i++)
            {
                if (computers[i].OUT.Count > 0 || q[i].Count > 0)
                    idle = false;
            }

            if (idle) { idleCycles++; } else { idleCycles = 0; }

            if (idleCycles > 3000)
            {
                idleCycles = 0;
                if (previousNatY.Contains(natY))
                {
                    return natY;
                }
                previousNatY.Add(natY);
                q[0].Enqueue((natX, natY));
            }
        }
    }

    private static async Task<(ProcessingUnit[], Task[])> GetComputers(long[] input)
    {
        var computers = new ProcessingUnit[50];
        var tasks = new Task[50];
        for (int i = 0; i < computers.Length; i++)
        {
            var memory = new IntCode.Memory<long>(input.SelectArray(i => new MemCell<long> { Value = i }));
            computers[i] = new ProcessingUnit(memory);

            await computers[i].IN.WriteAsync(i);
            tasks[i] = computers[i].Run();
        }

        return (computers, tasks);
    }
}
