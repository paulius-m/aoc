﻿using Days.Y2023.Day05;
using Tools;
using static Tools.Neighbourhoods;

namespace Days.Y2019.Day18;
using Input = Grid<char>;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var lines = await File.ReadAllLinesAsync(this.GetInputFile("input"));

        var map = new Input(lines.SelectMany((line, y) => line.Select((c, x) => KeyValuePair.Create(new Coord2D(y, x), c))));
        return map;
    }

    public object Part1(Input map)
    {
        var start = map.First(m => m.Value is '@');

        var keys = map.Where(m => char.IsLower(m.Value)).ToArray();
        var doors = map.Where(m => char.IsUpper(m.Value)).ToArray();
        var doorSet = doors.Select(d => d.Value).ToHashSet();
        var keySet = keys.Select(d => d.Value).ToHashSet();

        var nodes = keys.Concat(doors).Append(start).ToDictionary(kv => kv.Value, kv => kv.Key);
        var nodeSet = nodes.Keys.ToHashSet();

        var transitions = nodes
            .SelectMany(n => FindPaths(map, n.Value, new()).Select(p => new Transition(n.Key, p.Key, p.Value)))
            .GroupBy(kv => kv.from)
            .ToDictionary(g => g.Key, g => g.ToArray());

        Dictionary<char, Dictionary<HashSet<char>, long>> cache = new();
        long minminCost = int.MaxValue;

        return FindSolution(transitions, '@', ['@'], 0);

        long FindSolution(Dictionary<char, Transition[]> transitions, char current, char[] path, long currentCost)
        {
            var pathSet = path.ToHashSet();
            if (pathSet.IsSupersetOf(keySet))
            {
                if (currentCost <= minminCost)
                {
                    Console.WriteLine($"{string.Join(',', path.Where(c => char.IsLower(c)))} {currentCost}");
                    minminCost = currentCost;
                }
                return 0;
            }

            var cacheKey = path.Where(c => char.IsLower(c)).ToHashSet();

            if (!cache.ContainsKey(current))
            {
                cache[current] = new(HashSet<char>.CreateSetComparer());
            }

            if (cache[current].ContainsKey(cacheKey))
            {
                var cost = cache[current][cacheKey];
                if (currentCost + cost <= minminCost)
                {
                    Console.WriteLine($"{string.Join(',', path.Where(c => char.IsLower(c)))} {currentCost + cost}");
                    minminCost = currentCost + cost;
                }
                return cost;
            }

            var reachable = FindTransitions(transitions, current, pathSet);
            long minCost = int.MaxValue;
            foreach (var (pos, cost) in reachable)
            {
                if (currentCost + cost > minminCost)
                {
                    continue;
                }

                if (char.IsUpper(pos) && !pathSet.Contains(char.ToLower(pos)))
                {
                    continue;
                }

                long realCost;

                realCost = cost + FindSolution(transitions, pos, path.Append(pos).ToArray(), currentCost + cost);

                if (realCost < minCost)
                {
                    minCost = realCost;
                }
            }
            return cache[current][cacheKey] = minCost;
        }
    }

    public object Part2(Input map)
    {
        var startPos = map.First(m => m.Value is '@').Key;
        map[new(startPos.c - 1, startPos.r - 1)] = '1';
        map[new(startPos.c ,    startPos.r - 1)] = '#';
        map[new(startPos.c + 1, startPos.r - 1)] = '2';

        map[new(startPos.c - 1, startPos.r)] = '#';
        map[new(startPos.c ,    startPos.r)] = '#';
        map[new(startPos.c + 1, startPos.r)] = '#';

        map[new(startPos.c - 1, startPos.r + 1)] = '3';
        map[new(startPos.c,     startPos.r + 1)] = '#';
        map[new(startPos.c + 1, startPos.r + 1)] = '4';

        Console.WriteLine( map.ToRectString() );

        var starts = map.Where(m => char.IsNumber(m.Value)).ToArray();

        var keys = map.Where(m => char.IsLower(m.Value)).ToArray();
        var doors = map.Where(m => char.IsUpper(m.Value)).ToArray();
        var doorSet = doors.Select(d => d.Value).ToHashSet();
        var keySet = keys.Select(d => d.Value).ToHashSet();
        var nodes = keys.Concat(doors).Concat(starts).ToDictionary(kv => kv.Value, kv => kv.Key);
        var nodeSet = nodes.Keys.ToHashSet();

        var transitions = nodes
            .SelectMany(n => FindPaths(map, n.Value, new()).Select(p => new Transition(n.Key, p.Key, p.Value)))
            .GroupBy(kv => kv.from)
            .ToDictionary(g => g.Key, g => g.ToArray());

        Dictionary<string, Dictionary<string, long>> cache = new();
        long minminCost = int.MaxValue;

        return FindSolution(transitions, starts.Select(s => s.Value).ToArray(), starts.Select(s => new[] { s.Value }).ToArray(), 0, 0);

        long FindSolution(Dictionary<char, Transition[]> transitions, char[] current, char[][] path, int currentI, long currentCost)
        {
            var pathSet = path.SelectMany(p => p).ToHashSet();
            if (pathSet.IsSupersetOf(keySet))
            {
                if (currentCost <= minminCost)
                {
                    Console.WriteLine($"{currentCost}");
                    minminCost = currentCost;
                }
                return 0;
            }
            var currentKey = new string(current);
            var cacheKey = string.Join(',', path.Select(p => new string(p.ToArray())));
            if (!cache.ContainsKey(currentKey))
            {
                cache[currentKey] = new(/*HashSet<string>.CreateSetComparer()*/);
            }

            if (cache[currentKey].ContainsKey(cacheKey))
            {
                var cost = cache[currentKey][cacheKey];
                if (currentCost + cost <= minminCost)
                {
                    Console.WriteLine($"{currentCost + cost}");
                    minminCost = currentCost + cost;
                }
                return cost;
            }

            long minCost = int.MaxValue;

            for (int i = 0; i < current.Length; i++)
            {

                var c = current[i];
                var copy = current.ToArray();

                var reachable = FindTransitions(transitions, c, pathSet);

                foreach (var (pos, cost) in reachable)
                {
                    if (currentCost + cost > minminCost)
                    {
                        continue;
                    }

                    if (char.IsUpper(pos) && !pathSet.Contains(char.ToLower(pos)))
                    {
                        continue;
                    }

                    long realCost;
                    copy[i] = pos;
                    var pathCopy = path.ToArray();
                    pathCopy[i] = pathCopy[i].Append(pos).ToArray();
                    realCost = cost + FindSolution(transitions, copy, pathCopy, i, currentCost + cost);

                    if (realCost < minCost)
                    {
                        minCost = realCost;
                    }
                }
            }
            return cache[currentKey][cacheKey] = minCost;
        }
    }

    public static Dictionary<char, int> FindPaths(Grid<char> map, Coord2D start, HashSet<char> visited)
    {
        var nodes = new Dictionary<char, int>();

        var costs = new Dictionary<Coord2D, int> { [start] = 0 };
        var toVisit = new PriorityQueue<Coord2D, int>();
        toVisit.Enqueue(start, 0);

        while (toVisit.Count > 0)
        {
            var current = toVisit.Dequeue();
            var currentRisk = costs[current];

            foreach (var (pos, cost) in from n in GetNear4(current)
                                        where map[n] is not '#'
                                        select (n, 1))
            {
                var realCost = cost + currentRisk;
                if (realCost < costs.GetValueOrDefault(pos, int.MaxValue))
                {
                    costs[pos] = realCost;
                    if (map[pos] is var c && c is not '.' && !visited.Contains(c))
                    {
                        nodes[c] = realCost;
                    }
                    else
                    {
                        toVisit.Enqueue(pos, realCost);
                    }
                }
            }
        }

        return nodes;
    }

    public static Dictionary<char, int> FindTransitions(Dictionary<char, Transition[]> map, char start, HashSet<char> visited)
    {
        var nodes = new Dictionary<char, int>();

        var costs = new Dictionary<char, int> { [start] = 0 };
        var toVisit = new PriorityQueue<char, int>();
        toVisit.Enqueue(start, 0);

        while (toVisit.Count > 0)
        {
            var current = toVisit.Dequeue();
            var currentRisk = costs[current];

            foreach (var (_, pos, cost) in map[current])
            {
                var realCost = cost + currentRisk;

                if (realCost < costs.GetValueOrDefault(pos, int.MaxValue))
                {
                    costs[pos] = realCost;
                    if (!visited.Contains(pos))
                    {
                        nodes[pos] = realCost;
                    }
                    else
                    {
                        toVisit.Enqueue(pos, realCost);
                    }
                }
            }
        }

        return nodes;
    }
}

record Transition(char from, char to, int cost);
