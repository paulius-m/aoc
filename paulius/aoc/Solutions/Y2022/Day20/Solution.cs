using Tools;

namespace Days.Y2022.Day20;

file class Solution : ISolution<LinkedListNode<long>[]>
{
    public async Task<LinkedListNode<long>[]> LoadInput()
    {
        return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                select new LinkedListNode<long>(long.Parse(l))).ToArray();
    }

    public object Part1(LinkedListNode<long>[] nodes)
    {
        var list = new LinkedList<long>();

        for (long i = 0; i < nodes.Length; i++)
        {
            list.AddLast(nodes[i]);
        }

        var zeroNode = Mix(nodes, list);

        long[] coords = GetCoords(list, zeroNode);

        return coords.Sum();
    }

    public object Part2(LinkedListNode<long>[] nodes)
    {
        var list = new LinkedList<long>();

        for (long i = 0; i < nodes.Length; i++)
        {
            nodes[i].Value *= 811589153;
            list.AddLast(nodes[i]);
        }

        LinkedListNode<long>? zeroNode = null;

        for (long k = 0; k < 10; k++)
        {
            zeroNode = Mix(nodes, list);
        }

        long[] coords = GetCoords(list, zeroNode!);

        return coords.Sum();
    }

    private static LinkedListNode<long> Mix(LinkedListNode<long>[] nodes, LinkedList<long> list)
    {
        LinkedListNode<long>? zeroNode = null;

        for (var i = 0; i < nodes.Length; i++)
        {
            var number = nodes[i].Value;

            long offset = (Math.Abs(number) - 1) % (nodes.Length - 1);
            switch (number)
            {
                case > 0:
                    {
                        var c = nodes[i].Next ?? list.First;
                        list.Remove(nodes[i]);
                        for (long j = 0; j < offset; j++) c = c!.Next ?? list.First;
                        list.AddAfter(c!, nodes[i]);
                        break;
                    }
                case < 0:
                    {
                        var c = nodes[i].Previous ?? list.Last;
                        list.Remove(nodes[i]);
                        for (long j = 0; j < offset; j++) c = c!.Previous ?? list.Last;
                        list.AddBefore(c!, nodes[i]);
                        break;
                    }
                default:
                    zeroNode = nodes[i];
                    break;
            }
        }

        return zeroNode!;
    }

    private static long[] GetCoords(LinkedList<long> list, LinkedListNode<long> zeroNode)
    {
        var c = zeroNode;
        var coords = new long[3];
        for (long j = 0; j < 3; j++)
        {
            for (long i = 0; i < 1_000; i++) c = c!.Next ?? list.First;
            coords[j] = c!.Value;
        }

        return coords;
    }
}