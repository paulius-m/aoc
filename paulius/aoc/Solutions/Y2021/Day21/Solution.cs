using System.Collections.Generic;
using Tools;

namespace Days.Y2021.Day21;

public class Solution : ISolution<int[]>
{
    private static int[] TestStartPositions = new[] { 4, 8 };
    private static int[] InputStartPositions = new[] { 5, 10 };

    public Task<int[]> LoadInput()
    {
        return Task.FromResult(InputStartPositions);
    }

    public object Part1(int[] players)
    {
        var c = 0;
        var rollCount = 0;
        IEnumerable<int> Dice()
        {
            while (true)
            {
                rollCount++;
                c++;
                c = c > 100 ? 1 : c;
                yield return c;
            }
        }

        var dice = Dice();

        var scores = new int[players.Length];
        var positions = new int[players.Length];
        Array.Copy(players, positions, players.Length);

        while (scores.All(s => s < 1000))
        {
            for (int i = 0; i < positions.Length; i++)
            {
                var move = dice.Take(3).Sum();
                positions[i] = (positions[i] + move - 1) % 10 + 1;
                scores[i] += positions[i];
                if (scores[i] >= 1000)
                {
                    break;
                }
            }
        }

        return scores.First(s => s < 1000) * rollCount;
    }

    public object Part2(int[] i)
    {
        return GetWinners((i, new int[i.Length], 0)).Max();
    }

    static int[] Rolls3 = (from i1 in new int[] { 1, 2, 3 }
                           from i2 in new int[] { 1, 2, 3 }
                           from i3 in new int[] { 1, 2, 3 }
                           select i1 + i2 + i3).OrderByDescending(Operators.Identity).ToArray();

    static Func<(int[], int[], int), long[]> GetWinnersM = FuncEx.Memoize<(int[], int[], int), string, long[]>(GetWinners, step => $"{step.Item3}_{string.Join(',', step.Item1)}_{string.Join(',', step.Item2)}");

    static long[] GetWinners((int[] prevPositions, int[] prevScores, int i) step)
    {
        var (prevPositions, prevScores, i) = step;

        var nextI = (i + 1) % prevScores.Length;

        var scores = prevScores.ToArray();
        var positions = prevPositions.ToArray();
        var wins = new long[prevPositions.Length];

        foreach (var move in Rolls3)
        {
            positions[i] = (prevPositions[i] + move - 1) % 10 + 1;
            scores[i] = prevScores[i] + positions[i];

            if (scores[i] >= 21)
            {
                wins[i]++;
            }
            else
            {
                var w = GetWinnersM((positions, scores, nextI));
                for (int j = 0; j < w.Length; j++)
                {
                    wins[j] += w[j];
                }
            }
        }
        return wins;
    }
}