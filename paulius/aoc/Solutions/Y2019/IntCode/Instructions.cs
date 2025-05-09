using System.Numerics;

namespace Days.Y2019.IntCode
{
    delegate Task AsyncOperation<T>(Registers<T> r, MemCell<T>[] m);
    delegate void Operation<T>(Registers<T> r, MemCell<T>[] m);

    class Instruction<T> where T : notnull, INumber<T>
    {
        public AsyncOperation<T> AsycOp;
        public Operation<T> Op;
        public IMemRef<T>[] Modes = Array.Empty<IMemRef<T>>();
    }

    class InstructionOperations
    {
        public static void Add<T>(Registers<T> r, MemCell<T>[] m) where T : notnull, INumber<T>
        {
            var (a, b, c) = (m[0], m[1], m[2]);

            c.Value = a.Value + b.Value;
        }

        public static void Multiply<T>(Registers<T> r, MemCell<T>[] m) where T : notnull, INumber<T>
        {
            var (a, b, c) = (m[0], m[1], m[2]);

            c.Value = a.Value * b.Value;
        }

        public static async Task Input<T>(Registers<T> r, MemCell<T>[] m) where T : notnull, INumber<T>
        {
            var a = m[0];

            if (await r.IN.WaitToReadAsync())
                a.Value = await r.IN.ReadAsync();
            else
                Halt(r, m);
        }

        public static async Task Output<T>(Registers<T> r, MemCell<T>[] m) where T : notnull, INumber<T>
        {
            var a = m[0];
            await r.OUT.WriteAsync(a.Value);
        }

        public static void JumpIfNotZero<T>(Registers<T> r, MemCell<T>[] m) where T : notnull, INumber<T>
        {
            var (a, b) = (m[0], m[1]);
            if (a.Value != default)
            {
                r.IP = b.Value;
            }
        }

        public static void JumpIfZero<T>(Registers<T> r, MemCell<T>[] m) where T : notnull, INumber<T>
        {
            var (a, b) = (m[0], m[1]);
            if (a.Value == default)
            {
                r.IP = b.Value;
            }
        }

        public static void LessThen<T>(Registers<T> r, MemCell<T>[] m) where T : notnull, INumber<T>
        {
            var (a, b, c) = (m[0], m[1], m[2]);
            c.Value = a.Value < b.Value ? T.One : T.Zero;
        }

        public static void Equal<T>(Registers<T> r, MemCell<T>[] m) where T : notnull, INumber<T>
        {
            var (a, b, c) = (m[0], m[1], m[2]);
            c.Value = a.Value == b.Value ? T.One : T.Zero;
        }

        public static void SetRB<T>(Registers<T> r, MemCell<T>[] m) where T : notnull, INumber<T>
        {
            var a = m[0];
            r.RB += a.Value;
        }

        public static void Halt<T>(Registers<T> r, MemCell<T>[] m) where T : notnull, INumber<T>
        {
            r.Halt = true;
        }
    }
}
