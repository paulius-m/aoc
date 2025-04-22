using Tools;
namespace Days.Y2022.Day07;

file class Solution : ISolution<Directory>
{
    public async Task<Directory> LoadInput()
    {
        var input = await System.IO.File.ReadAllLinesAsync(this.GetInputFile("input"));

        Directory root = new Directory("/", null);
        Directory current = root;

        foreach (var line in input)
        {
            switch (line.Split(' '))
            {
                case ["$", "cd", "/"]:
                    current = root;
                    break;
                case ["$", "cd", ".."]:
                    current = current.Parent ?? root;
                    break;
                case ["$", "cd", var dir]:
                    current = current.Children[dir];
                    break;

                case ["$", "ls"]:
                    break;

                case ["dir", var name]:
                    if (!current.Children.ContainsKey(name))
                        current.Children[name] = new Directory(name, current);
                    break;

                case [var size, var name]:
                    current.Files[name] = new File(name, long.Parse(size));
                    break;
            }
        }

        return root;

    }

    public object Part1(Directory root)
    {
        return Flatten(root).Select(d => d.Size()).Where(s => s <= 100000).Sum();
    }

    public object Part2(Directory root)
    {
        var toFree = 30000000 - 70000000 + root.Size();

        return Flatten(root).Select(d => d.Size()).Where(s => s >= toFree).Order().First();
    }

    static IEnumerable<Directory> Flatten(Directory d)
    {
        foreach (var c in d.Children)
        {
            foreach (var cc in Flatten(c.Value))
            {
                yield return cc;
            }

            yield return c.Value;
        }
    }
}


file record FS();

file record Directory(string Name, Directory? Parent) : FS
{
    public Dictionary<string, Directory> Children { get; set; } = new();
    public Dictionary<string, File> Files { get; set; } = new();

    public long Size() => Children.Values.Sum(c => c.Size()) + Files.Values.Sum(c => c.Size);
}

file record File(string Name, long Size) : FS;