using Microsoft.Win32;
using System.Numerics;

namespace Days.Y2019.IntCode
{
    delegate Task Operation<T>(Registers<T> r, MemCell<T>[] m);

    class Instruction<T> where T : notnull, INumber<T>
    {
        public Operation<T> Exec;
        public IEnumerable<IMemRef<T>> Modes = Enumerable.Empty<IMemRef<T>>();
    }

    class InstructionOperations
    {
        public static Task Add<T>(Registers<T> r, MemCell<T>[] m) where T : notnull, INumber<T>
        {
            var (a, b, c) = (m[0], m[1], m[2]);

            c.Value = a.Value + b.Value;
            return Task.CompletedTask;
        }

        public static Task Multiply<T>(Registers<T> r, MemCell<T>[] m) where T : notnull, INumber<T>
        {
            var (a, b, c) = (m[0], m[1], m[2]);

            c.Value = a.Value * b.Value;
            return Task.CompletedTask;
        }

        public static async Task Input<T>(Registers<T> r, MemCell<T>[] m) where T : notnull, INumber<T>
        {
            var a = m[0];
            a.Value = await r.IN.ReadAsync();
        }

        public static async Task Output<T>(Registers<T> r, MemCell<T>[] m) where T : notnull, INumber<T>
        {
            var a = m[0];
            await r.OUT.WriteAsync(a.Value);
        }

        public static Operation<T> JumpIf<T>(bool v) where T : notnull, INumber<T>
        {
            return (r, m) =>
            {
                var (a, b) = (m[0], m[1]);
                if (v == (a.Value != default))
                {
                    r.IP = b.Value;
                }
                return Task.CompletedTask;
            };
        }

        public static Task LessThen<T>(Registers<T> r, MemCell<T>[] m) where T : notnull, INumber<T>
        {
            var (a, b, c) = (m[0], m[1], m[2]);
            c.Value = a.Value < b.Value ? T.One : T.Zero;
            return Task.CompletedTask;
        }

        public static Task Equal<T>(Registers<T> r, MemCell<T>[] m) where T : notnull, INumber<T>
        {
            var (a, b, c) = (m[0], m[1], m[2]);
            c.Value = a.Value == b.Value ? T.One : T.Zero;
            return Task.CompletedTask;
        }

        public static Task SetRB<T>(Registers<T> r, MemCell<T>[] m) where T : notnull, INumber<T>
        {
            var a = m[0];
            r.RB += a.Value;
            return Task.CompletedTask;
        }

        public static Task Halt<T>(Registers<T> r, MemCell<T>[] m) where T : notnull, INumber<T>
        {
            r.Halt = true;
            return Task.CompletedTask;
        }
    }
}
