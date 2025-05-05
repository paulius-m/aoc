using Days.Y2019.IntCode;
using Nito.AsyncEx;
using Tools;
using static Tools.Neighbourhoods;
using Input = long[];

namespace Days.Y2019.Day19
{
    file class Solution : ISolution<Input>
    {
        public async Task<Input> LoadInput()
        {
            return (await File.ReadAllTextAsync(this.GetInputFile("input"))).Split(",").SelectArray(long.Parse);
        }

        public async Task<object> Part1Async(Input input)
        {
            var screen = new Grid<char>();
            for (int column = 0; column < 50; column++)
                for (int row = 0; row < 50; row++)
                {
                    long r = await QueryLocation(input, column, row);

                    screen[new Coord2D(row, column)] = r is 1 ? 'X' : ' ';
                }
            Console.WriteLine(screen.ToRectString());

            return screen.Count(p => p.Value is 'X');
        }

        // 9611460
        // 9611462
        //14620961
        //14600961
        public async Task<object> Part2Async(Input input)
        {
            var position = AsyncContext.Run(() => RunPart2(input));

            return position.c * 10000 + position.r;
        }

        private static async Task<Coord2D> RunPart2(long[] input)
        {
            var boxSize = 100 - 1;
            Coord2D[] directions = [S, E];
            var edges = directions.Select(d => KeyValuePair.Create(d, new List<Coord2D>())).ToDictionary();

            var currentDir = 0;
            // any point inside a beam
            var position = new Coord2D(12, 18);
            var check = position;
            Coord2D candidatePosition = Coord2D.Zero;

            Render.InitWindow();

            for (int i = 0; i < 100000; i++)
            {
                long r = await QueryLocation(input, position.c, position.r);
                if (r is 1)
                {
                    var checkDir = directions[(currentDir + 1) % directions.Length];
                    check = position;
                    do
                    {
                        check += checkDir;
                        r = await QueryLocation(input, check.c, check.r);
                    } while (r is 1);

                    check -= checkDir;
                    edges[checkDir].Add(check);
                }
                else
                {
                    edges[directions[currentDir]].Clear();
                    position -= directions[currentDir];
                    currentDir = (currentDir + 1) % directions.Length;
                }

                position += directions[currentDir];

                var boxPoint = (from e in edges[E]
                                 from s in edges[S]
                                 where s - S * boxSize == e - E * boxSize
                                 select s - S * boxSize).FirstOrDefault();

                if (boxPoint is not null)
                {
                    candidatePosition = boxPoint;
                    break;
                }

                Render.BeginDraw(position.c, position.r);
                Render.Draw(position.c, position.r);
                foreach (var (dir, e) in edges)
                {
                    foreach (var q in e)
                    {
                        Render.Draw(q.c, q.r);
                    }
                }

                Render.Write(position.c + 8, position.r + 2 * 8, position.ManhatanDistance(check).ToString());
                Render.EndDraw();
            }

            Render.CloseWindow();

            return candidatePosition;
        }

        private static async Task<long> QueryLocation(long[] input, long column, long row)
        {
            //var c = new Coord2D(row, column);
            //if (testGrid.TryGetValue(c, out var v) && v is '#' or 'O')
            //{
            //    return 1;
            //}
            //return 0;

            var memory = new IntCode.Memory<long>(input.Select(v => new MemCell<long> { Value = v }).ToArray());
            var system = new ProcessingUnit(memory);

            await system.IN.WriteAsync(column);
            await system.IN.WriteAsync(row);
            await system.Run();
            var r = await system.OUT.ReadAsync();
            return r;
        }

        static string testMap =
            """
            #.......................................
            .#......................................
            ..##....................................
            ...###..................................
            ....###.................................
            .....####...............................
            ......#####.............................
            ......######............................
            .......#######..........................
            ........########........................
            .........#########......................
            ..........#########.....................
            ...........##########...................
            ...........############.................
            ............############................
            .............#############..............
            ..............##############............
            ...............###############..........
            ................###############.........
            ................#################.......
            .................########OOOOOOOOOO.....
            ..................#######OOOOOOOOOO#....
            ...................######OOOOOOOOOO###..
            ....................#####OOOOOOOOOO#####
            .....................####OOOOOOOOOO#####
            .....................####OOOOOOOOOO#####
            ......................###OOOOOOOOOO#####
            .......................##OOOOOOOOOO#####
            ........................#OOOOOOOOOO#####
            .........................OOOOOOOOOO#####
            ..........................##############
            ..........................##############
            ...........................#############
            ............................############
            .............................###########
            """;

        static Grid<char> testGrid = new Grid<char>(testMap.Split('\n').SelectMany((l, r) => l.Select((i, c) => KeyValuePair.Create(new Coord2D(r, c), i))));


    }
}