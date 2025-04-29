namespace Days.Y2019.IntCode
{
    delegate Task Operation(Registers r, MemCell[] m);

    class Instruction
    {
        public Operation Exec;
        public IEnumerable<IMemRef> Modes = Enumerable.Empty<IMemRef>();
    }

    class InstructionOperations
    {
        public static Task Add(Registers r, MemCell[] m)
        {
            var (a, b, c) = (m[0], m[1], m[2]);

            c.Value = a.Value + b.Value;
            return Task.CompletedTask;
        }

        public static Task Multiply(Registers r, MemCell[] m)
        {
            var (a, b, c) = (m[0], m[1], m[2]);

            c.Value = a.Value * b.Value;
            return Task.CompletedTask;
        }

        public static async Task Input(Registers r, MemCell[] m)
        {
            var a = m[0];
            a.Value = await r.IN.ReadAsync();
        }

        public static async Task Output(Registers r, MemCell[] m)
        {
            var a = m[0];
            await r.OUT.WriteAsync(a.Value);
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
                return Task.CompletedTask;
            };
        }

        public static Task LessThen(Registers r, MemCell[] m)
        {
            var (a, b, c) = (m[0], m[1], m[2]);
            c.Value = a.Value < b.Value ? 1 : 0;
            return Task.CompletedTask;
        }

        public static Task Equal(Registers r, MemCell[] m)
        {
            var (a, b, c) = (m[0], m[1], m[2]);
            c.Value = a.Value == b.Value ? 1 : 0;
            return Task.CompletedTask;
        }

        public static Task Halt(Registers r, MemCell[] m)
        {
            r.Halt = true;
            return Task.CompletedTask;
        }
    }
}
