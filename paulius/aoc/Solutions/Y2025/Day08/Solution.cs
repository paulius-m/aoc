using Tools;
using Input = long[][];
namespace Days.Y2025.Day08
{
    file class Solution : ISolution<Input>
    {
        public async Task<Input> LoadInput()
        {
            return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                    select l.Split(',').SelectArray(long.Parse)).ToArray();
        }

        public object Part1(Input input)
        {
            var circuits = new List<HashSet<long[]>>();
            var q = ToPriorityQueue(input);

            for (int i = 0; i < 1_000; i++)
            {
                var (a, b) = q.Dequeue();
                AddToCircuits(circuits, a, b);
            }

            return circuits.OrderByDescending(c => c.Count).Take(3).Product(c => c.Count);
        }

        public object Part2(Input input)
        {
            var circuits = new List<HashSet<long[]>>();
            var q = ToPriorityQueue(input);

            for (int i = 0; true; i++)
            {
                var (a, b) = q.Dequeue();
                AddToCircuits(circuits, a, b);
                if (circuits.Count is 1 && circuits[0].Count == input.Length)
                {
                    return a[0] * b[0];
                }
            }
        }

        private static PriorityQueue<(long[], long[]), double> ToPriorityQueue(long[][] input)
        {
            var q = new PriorityQueue<(long[], long[]), double>();
            for (int i = 0; i < input.Length - 1; i++)
                for (int j = i + 1; j < input.Length; j++)
                {
                    q.Enqueue((input[i], input[j]), Math.Sqrt(input[i].Zip(input[j], (ii, jj) => Math.Pow(ii - jj, 2)).Sum()));
                }

            return q;
        }

        private static void AddToCircuits(List<HashSet<long[]>> circuits, long[] a, long[] b)
        {
            var containingA = circuits.FirstOrDefault(c => c.Contains(a));
            var containingB = circuits.FirstOrDefault(c => c.Contains(b));

            switch ((containingA, containingB))
            {
                case (null, null):
                    circuits.Add([a, b]);
                    break;
                case ({ } c, null):
                    c.Add(b);
                    break;
                case (null, { } c):
                    c.Add(a);
                    break;
                case ({ } ca, { } cb) when ca != cb:
                    ca.UnionWith(cb);
                    circuits.Remove(cb);
                    break;
                case ({ } ca, { } cb) when ca == cb:
                    //Same circuit, do nohing
                    break;
            }
        }

    }
}