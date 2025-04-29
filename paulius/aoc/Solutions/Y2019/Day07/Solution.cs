using Tools;
using Input = System.Object;

namespace Days.Y2019.Day07
{
    file class Solution : ISolution<Input>
    {
        public async Task<Input> LoadInput()
        {
            return await File.ReadAllLinesAsync(this.GetInputFile("input"));
        }

        public object Part1(Input i)
        {
            return 0;
        }

        public object Part2(Input i)
        {
            return 0;
        }
    }
}