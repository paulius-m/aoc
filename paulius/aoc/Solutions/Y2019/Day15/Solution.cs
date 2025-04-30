using Days.Y2019.IntCode;
using Nito.AsyncEx;
using System.Numerics;
using System.Threading.Channels;
using Tools;
using Input = System.String;

namespace Days.Y2019.Day15
{
    file class Solution : ISolution<Input>
    {
        public async Task<Input> LoadInput()
        {
            return await File.ReadAllTextAsync(this.GetInputFile("input"));
        }

        public async Task<object> Part1Async(Input input)
        {
            var memory = new IntCode.Memory<long>(input.Split(",").Select(v => new MemCell<long> { Value = long.Parse(v) }).ToArray());

            var brain = new ProcessingUnit(memory);

            var brainTask = brain.Run();

            var result = AsyncContext.Run(() =>Input1(brain.IN, brain.OUT));
            await brainTask;
            return result;
        }

        private static async Task<int> Input1(ChannelWriter<long> input, ChannelReader<long> instructions)
        {
            await Task.Yield();

            var userInputs = new List<int>();

            var currentPos = (x: 0, y: 0);
            var screenMiddle = (x: 0, y: 0);

            var directions = new[] { ConsoleKey.UpArrow, ConsoleKey.RightArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow };
            var current = 0;
            var rand = new Random();
            var found = false;
            var maxPath = 0;
            var path = new HashSet<(int, int)>();
            var map = new Dictionary<(int, int), string>();
            map[(0, 0)] = "S";

            Render.InitWindow();
            Render.Clear();

            while (!found)
            {
                var (i, dx, dy) =   directions[current] switch
                {
                    ConsoleKey.UpArrow => (1, 0, -1),
                    ConsoleKey.DownArrow => (2, 0, 1),
                    ConsoleKey.LeftArrow => (3, -1, 0),
                    ConsoleKey.RightArrow => (4, 1, 0),
                    ConsoleKey.Escape => (99, 0, 0),
                    _ => (0, 0, 0)
                };

                if (i == 99) break;

                userInputs.Add(i);
                await input.WriteAsync(i);

                if (!await instructions.WaitToReadAsync())
                {
                    break;
                }

                var r = await instructions.ReadAsync();

                switch (r)
                {
                    case 0:
                        var temp = (currentPos.x + dx, currentPos.y + dy);
                        map[(temp.Item1 + screenMiddle.x, temp.Item2 + screenMiddle.y)] = "#";

                        current = (current + 1) % directions.Length;
                        
                        break;
                    case 1:
                        var newPos = (x: currentPos.x + dx, y: currentPos.y + dy);
                        if (path.Contains(newPos))
                        {
                            path.Remove(currentPos);
                        }
                        else
                        {
                            path.Add(currentPos);
                        }

                        current = (current + 3) % directions.Length;
                        currentPos = newPos;

                        break;
                    case 2:

                        newPos = (x: currentPos.x + dx, y: currentPos.y + dy);
                        found = true;
                        currentPos = newPos;

                        map[(currentPos.x + screenMiddle.x, currentPos.y + screenMiddle.y)] = "X";
                        break;
                }

                Display(currentPos, path, map);

                maxPath = Math.Max(path.Count, maxPath);
            }
            Render.CloseWindow();
            input.Complete();
            return maxPath;
        }

        public async Task<object> Part2Async(Input input)
        {
            var memory = new IntCode.Memory<long>(input.Split(",").Select(v => new MemCell<long> { Value = long.Parse(v) }).ToArray());

            var brain = new ProcessingUnit(memory);

            var brainTask = brain.Run();

            var result = AsyncContext.Run(() => Input2(brain.IN, brain.OUT));
            await brainTask;
            return result;
        }


        private static async Task<int> Input2(ChannelWriter<long> input, ChannelReader<long> instructions)
        {
            await Task.Yield();

            var userInputs = new List<int>();

            var currentPos = (x: 0, y: 0);
            var screenMiddle = (x: 0, y: 0);

            var directions = new[] { ConsoleKey.UpArrow, ConsoleKey.RightArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow };
            var current = 0;
            var rand = new Random();
            var found = false;
            var maxPath = 0;
            var path = new HashSet<(int, int)>();
            var map = new Dictionary<(int, int), string>();
            var spaceVisited = new Dictionary<(int, int), int>();

            Render.InitWindow();
            Render.Clear();

            while (true)
            {
                //var key = Console.ReadKey();
                var (i, dx, dy) =  /*key.Key*/ directions[current] switch
                {
                    ConsoleKey.UpArrow => (1, 0, -1),
                    ConsoleKey.DownArrow => (2, 0, 1),
                    ConsoleKey.LeftArrow => (3, -1, 0),
                    ConsoleKey.RightArrow => (4, 1, 0),
                    ConsoleKey.Escape => (99, 0, 0),
                    _ => (0, 0, 0)
                };

                if (i == 99) break;

                userInputs.Add(i);
                await input.WriteAsync(i);

                if (!await instructions.WaitToReadAsync())
                {
                    break;
                }

                var r = await instructions.ReadAsync();

                switch (r)
                {
                    case 0:
                        var temp = (currentPos.x + dx, currentPos.y + dy);
                        map[(temp.Item1 + screenMiddle.x, temp.Item2 + screenMiddle.y)] = "#";

                        current = (current + 1) % directions.Length;

                        break;
                    case 1:
                        var newPos = (x: currentPos.x + dx, y: currentPos.y + dy);
                        if (path.Contains(newPos))
                        {
                            path.Remove(currentPos);
                        }
                        else
                        {
                            path.Add(currentPos);
                        }

                        current = (current + 3) % directions.Length;
                        currentPos = newPos;
                        spaceVisited[currentPos] = spaceVisited.GetValueOrDefault(currentPos, 0) + 1;
                        break;
                    case 2:

                        newPos = (x: currentPos.x + dx, y: currentPos.y + dy);
                        if (!found)
                        {
                            maxPath = 0;
                            spaceVisited.Clear();
                            path.Clear();
                            found = true;
                        }
                        currentPos = newPos;

                        map[(currentPos.x + screenMiddle.x, currentPos.y + screenMiddle.y)] = "X";
                        break;
                }

                Display(currentPos, path, map);

                if (spaceVisited.Values.Count > 1 && spaceVisited.Values.Min() > 2)
                {
                    break;
                }
                maxPath = Math.Max(path.Count, maxPath);
            }
            Render.CloseWindow();
            input.Complete();
            return maxPath;
        }

        private static void Display((int x, int y) currentPos, HashSet<(int, int)> path, Dictionary<(int, int), string> map)
        {
            Render.BeginDraw((float)map.Keys.Average(kv => kv.Item1), (float)map.Keys.Average(kv => kv.Item2));
            foreach (var tile in map)
            {
                Render.Write(tile.Key.Item1, tile.Key.Item2, tile.Value);
            }
            foreach (var tile in path)
            {
                Render.Write(tile.Item1, tile.Item2, ".");
            }
            Render.Write(currentPos.x, currentPos.y, "@");
            Render.EndDraw();
        }
    }
}