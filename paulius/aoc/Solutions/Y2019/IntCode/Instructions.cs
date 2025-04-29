namespace Days.Y2019.IntCode
{
    delegate void Operation(Registers r, MemCell[] m);

    class Instruction
    {
        public Operation Exec;
        public IEnumerable<IMemRef> Modes = Enumerable.Empty<IMemRef>();
    }

    class InstructionOperations
    {
        public static void Add(Registers r, MemCell[] m)
        {
            var (a, b, c) = (m[0], m[1], m[2]);

            c.Value = a.Value + b.Value;
        }

        public static void Multiply(Registers r, MemCell[] m)
        {
            var (a, b, c) = (m[0], m[1], m[2]);

            c.Value = a.Value * b.Value;
        }

        public static void Input(Registers r, MemCell[] m)
        {
            var a = m[0];
            Console.Write("INPUT ");
            a.Value = int.Parse(Console.ReadLine());
        }

        public static void Output(Registers r, MemCell[] m)
        {
            var a = m[0];

            Console.WriteLine($"OUTPUT {a}");
        }

        public static Operation JumpIf(bool v)
        {
            return (r, m) =>
            {
                var (a, b) = (m[0], m[1]);
                if (v == (a.Value != 0))
                {
                    r.IP = b.Value;
                }
            };
        }

        public static void LessThen(Registers r, MemCell[] m)
        {
            var (a, b, c) = (m[0], m[1], m[2]);
            c.Value = a.Value < b.Value ? 1 : 0;
        }

        public static void Equal(Registers r, MemCell[] m)
        {
            var (a, b, c) = (m[0], m[1], m[2]);
            c.Value = a.Value == b.Value ? 1 : 0;
        }
    }
}
