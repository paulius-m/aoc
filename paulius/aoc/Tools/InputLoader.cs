namespace Tools;

public static class InputLoader
{
    public static string GetInputFile(this object o, string name)
    {
        var folder = o.GetType().Namespace!.Split(".").Last();
        return $@"{folder}\{name}.txt";
    }
}
