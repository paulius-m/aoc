using Tools;
using static MoreLinq.Extensions.SplitExtension;
using static MoreLinq.Extensions.TransposeExtension;

namespace Days.Y2021.Day04;

public class Solution : ISolution<(string[] numbers, string[][][] boards)>
{
    private const bool WIN = true;


    public async Task<(string[] numbers, string[][][] boards)> LoadInput()
    {
        var input = await File.ReadAllLinesAsync(this.GetInputFile("input"));

        var p = input.Split(l => l.Length is 0).ToArray();

        return (
                p.First().Single().Split(','),
                p.Skip(1).Select(p => p.Select(l => l.Trim().Replace("  ", " ").Split(' ')).ToArray()).ToArray()
            );
    }

    public object Part1((string[] numbers, string[][][] boards) input)
    {
        var boards = input.boards.Select(b => new Board(b)).ToArray();

        var win = 0;

        foreach (var number in input.numbers)
        {
            foreach (var board in boards)
            {
                if (board.Mark(number) is WIN)
                {
                    win = board.CalcWin(number);
                    goto end;
                }
            }
        }
    end: return Task.FromResult<object>(win);
    }

    public object Part2((string[] numbers, string[][][] boards) input)
    {
        var boards = input.boards.Select(b => new Board(b)).ToArray();

        var win = 0;
        var winners = new HashSet<Board>();

        foreach (var number in input.numbers)
        {
            foreach (var board in boards.Except(winners))
            {
                if (board.Mark(number) is WIN)
                {
                    winners.Add(board);
                }
            }

            if (winners.Count == boards.Length)
            {
                win = winners.Last().CalcWin(number);
                break;
            }
        }
        return Task.FromResult<object>(win);
    }

    public class Board
    {
        private readonly string[][] _rows;
        private readonly string[][] _columns;

        private readonly HashSet<string> _numbers;

        public Board(string[][] numbers)
        {
            _rows = numbers;
            _columns = numbers.Transpose().Select(c => c.ToArray()).ToArray();
            _numbers = numbers.SelectMany(n => n).ToHashSet();
        }

        public bool Mark(string number)
        {
            return _numbers.Remove(number) && (CheckWin(_rows) || CheckWin(_columns));
        }

        private bool CheckWin(string[][] c)
        {
            return c.Any(r => r.All(n => !_numbers.Contains(n)));
        }

        internal int CalcWin(string number)
        {
            return _numbers.Select(int.Parse).Sum() * int.Parse(number);
        }
    }
}