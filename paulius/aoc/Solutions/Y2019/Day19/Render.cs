using Raylib_cs;

namespace Days.Y2019.Day19;

internal class Render
{
    public static void InitWindow()
    {
        Raylib.SetTraceLogLevel(TraceLogLevel.Error);
        Raylib.InitWindow(800, 600, "Preview");
        //Raylib.SetTargetFPS(10);
    }

    public static void CloseWindow()
    {
        Raylib.CloseWindow();
    }

    public static void BeginDraw(float x, float y)
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);
        var camera = new Camera2D(new (200,100), new(x, y), 0, 1f);

        Raylib.BeginMode2D(camera);
    }

    internal static void Draw(long x, long y)
    {
        Raylib.DrawCircle((int)x, (int)y, 1, Color.White);
    }

    internal static void Write(long x, long y, string text)
    {
        Raylib.DrawText(text, (int)x, (int)y, 1, Color.White);
    }

    internal static void EndDraw()
    {
        Raylib.EndMode2D();
        Raylib.EndDrawing();
    }

    internal static void Clear()
    {
        Raylib.BeginDrawing();
        Raylib.EndDrawing();
    }
}
