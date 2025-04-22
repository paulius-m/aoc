using static MoreLinq.Extensions.SplitExtension;
using System.Linq.Expressions;
using Tools;
using static Days.Y2022.Day11.Monkey;

namespace Days.Y2022.Day11;

file class Solution : ISolution<Monkey[]>
{
    public async Task<Monkey[]> LoadInput()
    {
        var lines = await File.ReadAllLinesAsync(this.GetInputFile("input"));

        var monkeys = new List<Monkey>();
        Monkey? current = null;
        var itemIdSeq = 0;
        foreach (var m in lines.Split(""))
        {
            foreach (var a in m)
                switch (a.Trim().Split(":"))
                {
                    case [var monkey, ""] when monkey.Split(" ") is ["Monkey", _]:
                        current = new Monkey();
                        monkeys.Add(current);
                        break;
                    case ["Starting items", var list]:
                        current.Catch(list.Split(", ").Select(
                            w => new Item($"{itemIdSeq++} {w}") { Worrie = long.Parse(w) }));
                        break;
                    case ["Operation", var op]:
                        ParameterExpression old = Expression.Parameter(typeof(long), "old");
                        current.Operation = Expression.Lambda<Func<long, long>>(
                            op.Trim().Split(" ") switch
                            {
                            ["new", "=", var op1, var e, var op2] => GetExpression(op1, e, op2, old)
                            },
                            old).Compile();
                        break;
                    case ["Test", var test]:
                        current.Divisor = int.Parse(test.Split(" ")[^1]);
                        break;
                    case ["If true", var ifTrue]:
                        current.IfTrue = int.Parse(ifTrue.Split(" ")[^1]);
                        break;
                    case ["If false", var ifFalse]:
                        current.IfFalse = int.Parse(ifFalse.Split(" ")[^1]);
                        break;
                }
        }

        return monkeys.ToArray();
    }

    private Expression GetExpression(string op1, string e, string op2, ParameterExpression old)
    {
        var eop1 = GetOp(op1);
        var eop2 = GetOp(op2);
        return e switch
        {
            "*" => Expression.Multiply(eop1, eop2),
            "+" => Expression.Add(eop1, eop2),
        };

        Expression GetOp(string op) => op is "old" ? old : Expression.Constant(long.Parse(op), typeof(long));
    }

    public object Part1(Monkey[] monkeys)
    {
        for (int round = 0; round < 20; round++)
        {
            for (int i = 0; i < monkeys.Length; i++)
            {
                var m = monkeys[i];
                while (m.Items.Count > 0)
                {
                    var (item, to) = m.Throw(true);
                    monkeys[to].Catch(item);
                }
            }
        }

        foreach (var m in monkeys)
        {
            Console.WriteLine(m.ThrowCount);
        }

        return monkeys.OrderByDescending(m => m.ThrowCount).Take(2).Product(m => m.ThrowCount);
    }

    public object Part2(Monkey[] monkeys)
    {
        var factor = monkeys.Product(m => m.Divisor);

        for (int round = 0; round < 10000; round++)
        {
            for (int i = 0; i < monkeys.Length; i++)
            {
                var m = monkeys[i];
                while (m.Items.Count > 0)
                {
                    var (item, to) = m.Throw(false);
                    item.Worrie %= factor;
                    monkeys[to].Catch(item);
                }
            }
        }

        foreach (var m in monkeys)
        {
            Console.WriteLine(m.ThrowCount);
        }

        return monkeys.OrderByDescending(m => m.ThrowCount).Take(2).Product(m => m.ThrowCount);
    }

}

file record Monkey
{
    public Queue<Item> Items { get; set; } = new Queue<Item>();
    public int Divisor { get; set; }

    public Func<long, long> Operation { get; set; }

    public int IfTrue { get; set; }
    public int IfFalse { get; set; }

    public int ThrowCount { get; set; }

    public (Item, int) Throw(bool withRelief)
    {
        ThrowCount++;

        var item = Items.Dequeue();
        item.Worrie = Operation(item.Worrie);

        if (withRelief)
        {
            item.Worrie /= 3;
        }

        return (item, item.Worrie % Divisor == 0L ? IfTrue : IfFalse);
    }

    public void Catch(IEnumerable<Item> items)
    {
        foreach (var i in items)
        {
            Catch(i);
        }
    }

    public void Catch(Item item)
    {
        Items.Enqueue(item);
    }

    public record Item(string Id)
    {
        public long Worrie { get; set; }
    }
}