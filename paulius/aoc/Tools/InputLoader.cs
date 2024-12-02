namespace Tools;

public static class InputLoader
{
    public static string GetInputFile(this object o, string name)
    {
        var folder = string.Join('\\', o.GetType().Namespace!.Split(".")[1 .. ^0]);
        return $@"{folder}\{name}.txt";
    }
}
