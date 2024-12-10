using Sprache;
using System.Collections.Generic;
using System.Text;
using Tools;

namespace Days.Y2024.Day09;

using Input = DiskBlock[];

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return (await File.ReadAllTextAsync(this.GetInputFile("input")))
            .Select((c, i) => new DiskBlock(
                i % 2 == 0 ? ItemType.File : ItemType.FreeSpace,
                c - '0',
                i / 2
                ))
            .ToArray();
    }

    public object Part1(Input input)
    {
        var compacted = Compact(input).ToArray();
        return compacted.Aggregate((Idx: 0L, Aggr: 0L), (a, b) => (a.Idx + b.BlockSize, Aggr: a.Aggr + Check(a.Idx, b))).Aggr;
    }

    private static long Check(long idx, DiskBlock b)
    {
        if (b.Type is ItemType.FreeSpace)
        {
            return 0;
        }

        var first = idx;
        var last = idx + b.BlockSize - 1;
        var IdxSum = (last - first + 1) * (last + first) / 2;
        var check = IdxSum * b.FileId;

        return check;
    }

    private static IEnumerable<DiskBlock> Compact(DiskBlock[] input)
    {
        var lastItemIndex = input.Length - 1;
        var lastItem = input[lastItemIndex];
        for (int i = 0; i < lastItemIndex; i++)
        {
            if (input[i].Type is ItemType.File)
            {
                yield return input[i];
                continue;
            }

            var emptySpace = input[i];

            for (int ri = lastItemIndex - 2; ri > i; ri -= 2)
            {
                var blockSize = Math.Min(lastItem.BlockSize, emptySpace.BlockSize);
                yield return lastItem with { BlockSize = blockSize };

                lastItem = lastItem with { BlockSize = lastItem.BlockSize - blockSize };
                emptySpace = emptySpace with { BlockSize = emptySpace.BlockSize - blockSize };

                if (lastItem.BlockSize is 0)
                {
                    lastItem = input[ri];
                    lastItemIndex = ri;
                }

                if (emptySpace.BlockSize is 0)
                {
                    break;
                }
            }
        }

        if (lastItem.BlockSize > 0)
        {
            yield return lastItem;
        }
    }

    public object Part2(Input input)
    {
        var compacted = CompactWhole(input).ToArray();

        return compacted.Aggregate((Idx: 0L, Aggr: 0L), (a, b) => (a.Idx + b.BlockSize, Aggr: a.Aggr + Check(a.Idx, b))).Aggr;
    }

    private static IEnumerable<DiskBlock> CompactWhole(DiskBlock[] input)
    {
        var disk = new SortedList<long, DiskBlock>();
        {
            var idx = 0L;
            foreach (var d in input)
            {
                if (d.BlockSize is 0) continue;
                disk.Add(idx, d);
                idx += d.BlockSize;
            }
        }

        for (var ri = disk.Count - 1; ri > 0; ri--)
        {
            var lastIdx = disk.GetKeyAtIndex(ri);
            var last = disk[lastIdx];
            if (last.Type is ItemType.FreeSpace)
            {
                continue;
            }

            for (var i = 0; i < ri; i++)
            {
                var firstIdx = disk.GetKeyAtIndex(i);
                var first = disk[firstIdx];
                if (first.Type is ItemType.File)
                {
                    continue;
                }
                if (first.BlockSize >= last.BlockSize)
                {
                    disk[firstIdx] = last;
                    disk[lastIdx] = last with { Type = ItemType.FreeSpace };

                    if (first.BlockSize > last.BlockSize)
                    {
                        disk[firstIdx + last.BlockSize] = first with { BlockSize = first.BlockSize - last.BlockSize };
                        ri++; // duh
                    }
                    break;
                }
            }
        }

        return disk.OrderBy(k => k.Key).Select(x => x.Value);
    }
}

file enum ItemType
{
    FreeSpace,
    File
}

file record DiskBlock(ItemType Type, long BlockSize, long FileId);
