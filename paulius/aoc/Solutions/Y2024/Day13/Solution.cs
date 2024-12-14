using Sprache;
using Tools;

namespace Days.Y2024.Day13;
using Input = Claw[];

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return Parsers.Claws.Parse(await File.ReadAllTextAsync(this.GetInputFile("input")));
    }

    public object Part1(Input input)
    {
        long sum = 0;
        foreach (var i in input)
        {
            sum += SolveIter(i, 100);
        }

        return sum;
    }

    private static long SolveIter(Claw claw, long interations)
    {
        var (a, b, p) = claw;
        List<(long m, long n)> solutions = new();

        for (var i = 0; i <= interations; i++)
        {
            if (p.X > b.X * i && p.Y > b.Y * i && (p.X - b.X * i) % a.X == 0 && (p.Y - b.Y * i) % a.Y == 0)
            {
                var ai = (p.Y - b.Y * i) / a.Y;
                if (ai <= 100)
                    solutions.Add((ai, i));
            }
            if (p.X > a.X * i && p.Y > a.Y * i && (p.X - a.X * i) % b.X == 0 && (p.Y - a.Y * i) % b.Y == 0)
            {
                var bi = (p.Y - a.Y * i) / b.Y;
                if (bi <= 100)
                    solutions.Add((i, bi));
            }
        }
        var solved = solutions.Where(s => claw.Verify(s.m, s.n)).ToArray();
        if (solved.Length > 0)
        {
            var s = solved.MinBy(s => s.m * 3 + s.n);
            return s.m * 3 + s.n;
        }
        return 0;
    }

    public object Part2(Input input)
    {
        const long bigNumber = 10000000000000L;
        long sum = 0;
        foreach (var i in input)
        {

            sum += SolveCramer(i with { Prize = new Prize(i.Prize.X + bigNumber, i.Prize.Y + bigNumber) });
        }

        return sum;
    }

    private static long SolveCramer(Claw claw)
    {
        var (a, b, p) = claw;
        List<(long m, long n)> solutions = new();

        var det = a.X * b.Y - a.Y * b.X; 
        var detX = p.X * b.Y - p.Y * b.X; 
        var detY = a.X * p.Y - a.Y * p.X; 

        if (detX % det == 0 && detY % det == 0)
        {
            solutions.Add((detX / det, detY / det));
        }

        var solved = solutions.Where(s => claw.Verify(s.m, s.n)).ToArray();
        if (solved.Length > 0)
        {
            var s = solved.MinBy(s => s.m * 3 + s.n);
            return s.m * 3 + s.n;
        }
        return 0;
    }
}

record Button(char Name, long X, long Y);
record Prize(long X, long Y);

record Claw(Button Button1, Button Button2, Prize Prize)
{
    public bool Verify(long m, long n)
    {
        return Button1.X * m + Button2.X * n == Prize.X &&
               Button1.Y * m + Button2.Y * n == Prize.Y;
    }
}

file class Parsers
{
    // Button A: X+94, Y+34
    static Parser<Button> Button = from button in Parse.String("Button").Token().Then(_ => Parse.AnyChar)
                                   from x in Parse.String(": X+").Then(_ => Parse.Number).Select(long.Parse)
                                   from y in Parse.String(", Y+").Then(_ => Parse.Number).Select(long.Parse)
                                   select new Button(button, x, y);

    static Parser<Prize> Prize = from x in Parse.String("Prize: X=").Then(_ => Parse.Number).Select(long.Parse)
                                 from y in Parse.String(", Y=").Then(_ => Parse.Number).Select(long.Parse)
                                 select new Prize(x, y);


    static Parser<Claw> Claw = from button1 in Button
                               from _ in Parse.LineEnd
                               from button2 in Button
                               from __ in Parse.LineEnd
                               from prize in Prize
                               select new Claw(button1, button2, prize);

    public static Parser<Claw[]> Claws = from c in Claw.DelimitedBy(Parse.LineEnd.Repeat(2))
                                         select c.ToArray();
}