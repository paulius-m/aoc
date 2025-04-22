using Tools;

namespace Days.Y2022.Day10
{
    file class Solution : ISolution<Instruction[]>
    {
        public async Task<Instruction[]> LoadInput()
        {
            return (from l in await File.ReadAllLinesAsync(this.GetInputFile("input"))
                    select l.Split(" ") switch
                    {
                    ["noop"] => new Instruction(1, 0),
                    ["addx", var value] => new Instruction(2, int.Parse(value))
                    }).ToArray();
        }

        public object Part1(Instruction[] instructions)
        {
            return ToCycles(instructions, 0).Where(c => (c.cycle - 20) % 40 is 0).Sum(c => c.x * c.cycle);
        }

        public object Part2(Instruction[] instructions)
        {
            var top = Console.CursorTop + 1;
            var crt = new char[6][];
            for (int i = 0; i < crt.Length; i++)
            {
                crt[i] = new char[40];
            }

            foreach (var p in ToCycles(instructions, -1))
            {
                crt[p.cycle / 40][p.cycle % 40] = Math.Abs(p.cycle % 40 - p.x) <= 1 ? '#' : ' ';
            }
            return string.Join('\n', crt.Select(l => new string(l)));
        }

        IEnumerable<(int cycle, int x)> ToCycles(Instruction[] instructions, int cycle)
        {
            var x = 1;

            foreach (var c in instructions)
            {
                for (int i = 0; i < c.Cycles; i++)
                {
                    cycle++;
                    yield return (cycle, x);
                }
                x += c.Value;
            }
        }
    }
}

file record Instruction(int Cycles, int Value);