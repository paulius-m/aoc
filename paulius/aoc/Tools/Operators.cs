namespace Tools;

public static class Operators
{
    public static T Identity<T>(T t) => t;
    public static int Multiply(int a, int b) => a * b;
    public static long Multiply(long a, long b) => a * b;
}
