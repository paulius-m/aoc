using Tools;
using Input = System.Object;

namespace $rootnamespace$.$fileinputname$;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        return await File.ReadAllLinesAsync(this.GetInputFile("test"));
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