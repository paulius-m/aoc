using Tools;

namespace Days.Y2022.Day02;

file class Solution : ISolution<Line[]>
{
    public async Task<Line[]> LoadInput()
    {
        return (await File.ReadAllLinesAsync(this.GetInputFile("input")))
            .Select(l => l.Split(" ") switch
            {
            [var o, var p] => new Line(o, p)
            }).ToArray();
    }

    public object Part1(Line[] i)
    {
        return i.Sum(l => l switch
        {
            (Oponent.Rock, Player.Scissors) => Player.Lose(),
            (Oponent.Rock, Player.Paper) => Player.Win(),
            (Oponent.Paper, Player.Rock) => Player.Lose(),
            (Oponent.Paper, Player.Scissors) => Player.Win(),
            (Oponent.Scissors, Player.Paper) => Player.Lose(),
            (Oponent.Scissors, Player.Rock) => Player.Win(),
            _ => Player.Draw()
        } + Player.Shape(l.Player));
    }

    public object Part2(Line[] i)
    {
        return i.Sum(l => l switch
        {
            (Oponent.Rock, Outcome.Lose) => Player.Lose() + Player.Shape(Player.Scissors),
            (Oponent.Rock, Outcome.Win) => Player.Win() + Player.Shape(Player.Paper),
            (Oponent.Paper, Outcome.Lose) => Player.Lose() + Player.Shape(Player.Rock),
            (Oponent.Paper, Outcome.Win) => Player.Win() + Player.Shape(Player.Scissors),
            (Oponent.Scissors, Outcome.Lose) => Player.Lose() + Player.Shape(Player.Paper),
            (Oponent.Scissors, Outcome.Win) => Player.Win() + Player.Shape(Player.Rock),
            (var o, Outcome.Draw) => Player.Draw() + Oponent.Shape(o)
        });
    }

    class Oponent
    {
        public const string Rock = "A";
        public const string Paper = "B";
        public const string Scissors = "C";
        public static int Shape(string s) => s switch
        {
            Rock => 1,
            Paper => 2,
            Scissors => 3,
        };
    }
    class Player
    {
        public const string Rock = "X";
        public const string Paper = "Y";
        public const string Scissors = "Z";

        public static int Win() => 6;
        public static int Draw() => 3;
        public static int Lose() => 0;

        public static int Shape(string s) => s switch
        {
            Rock => 1,
            Paper => 2,
            Scissors => 3,
        };
    }

    public class Outcome
    {
        public const string Lose = "X";
        public const string Draw = "Y";
        public const string Win = "Z";
    }
}

file record Line(string Oponent, string Player);