using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Days.Y2019.Day17;

internal partial class Solution
{



    private static char[] pointers = ['^', '>', 'v', '<'];

    private static (int x, int y)[] directions = [(0, -1), (1, 0), (0, 1), (-1, 0)];


    public static IEnumerable<int> Directions()
    {
        int[] d = new[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        for (int i = 0; i < d.Length; i++)
        {
            yield return d[i];
        }


        while (true)
        {
            yield return 1;
        }
    }

    private static string[] GetFunctions(Dictionary<(int x, int y), char> screen)
    {
        var (pos, c) = screen.First(c => pointers.Contains(c.Value));
        var di = Array.IndexOf(pointers, c);
        var instructions = GetMovements([0], screen, pos, di);

        var s = string.Join(',', instructions);

        Console.WriteLine(s);

        var function = Convert(instructions, Array.Empty<string>(), 0);
        var main = function.Item2.Zip(["A", "B", "C"]).Aggregate(s, (a, c) => a.Replace(c.First, c.Second));
        return function.Item2.Prepend(main).ToArray();
    }

    private static string[] GetMovements(List<object> instrucions, Dictionary<(int x, int y), char> screen, (int x, int y) pos, int di)
    {
        var dir = Directions().GetEnumerator();

        var (x, y) = pos;
        do
        {
            var diCandidates = new int[] { di - 1, di, di + 1 }
                .Select(dd => (dd + directions.Length) % directions.Length)
                .Where(dd => directions[dd] is var ddi && (x + ddi.x, y + ddi.y) is var dpos && screen.GetValueOrDefault(dpos) is '#')
                .ToArray();

            if (diCandidates.Length == 0)
            {
                break;
            }
            if (diCandidates.Length == 1 && diCandidates[0] is var newDi)
            {
                AppendInstruction(di, instrucions, newDi);

                di = newDi;
            }
            else
            {
                dir.MoveNext();
                var cdi = diCandidates[dir.Current];
                AppendInstruction(di, instrucions, cdi);
                di = cdi;
            }

            x += directions[di].x;
            y += directions[di].y;
        } while (true);

        return instrucions
            .Where(i => i is not 0)
            .Select(i => i switch
            {
                _ => i.ToString()
            }).ToArray();
    }

    private static List<object> AppendInstruction(int di, List<object> instrucions, int newDi)
    {
        if (newDi == di)
        {
            if (instrucions.Last() is int steps)
            {
                instrucions[^1] = steps + 1;
            }
            else
            {
                instrucions.Add(1);
            }
        }
        else
        {
            instrucions.Add(newDi == (di + 1 + directions.Length) % directions.Length ? "R" : "L");
            instrucions.Add(1);
        }
        return instrucions;
    }

    private static (bool, string[]) Convert(string[] instructions, string[] functions, int curI)
    {
        var serialized = functions.Aggregate(string.Join(',', instructions), (a, c) => a.Replace(c, "").Replace(",,", ",").Trim(','));

        if (functions.Length < 3)
        {

            for (int i = 0 + 1; i < serialized.Length; i++)
            {
                var fCand = serialized[0..i];

                var newF = functions.Append(fCand).ToArray();
                if (newF.Aggregate(serialized, (a, c) => a.Replace(c, "")).Trim(',') is "")
                {
                    return (true, newF);
                }
                var r = Convert(instructions, newF, i);
                if (r.Item1) return r;
            }
        }

        return (false, Array.Empty<string>());
    }

    private static async Task<long> Input(ChannelWriter<long> input, ChannelReader<long> instructions, string[] functions)
    {
        await Task.Yield();

        foreach (var f in functions)
            await WriteString(f);
        await WriteString("n");
        var result = 0L;
        while (await instructions.WaitToReadAsync())
        {
            var r = await instructions.ReadAsync();

            if (r <= char.MaxValue)
                Console.Write((char)r);
            else
                result = r;
        }
        input.Complete();

        return result;

        async Task WriteString(string s)
        {
            foreach (var c in s)
            {
                await input.WriteAsync(c);
            }

            await input.WriteAsync('\n');
        }
    }
}
