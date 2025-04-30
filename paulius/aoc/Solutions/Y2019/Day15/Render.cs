using Raylib_cs;

namespace Days.Y2019.Day15;

internal class Render
{
    public static void InitWindow()
    {
        Raylib.SetTraceLogLevel(TraceLogLevel.Error);
        Raylib.InitWindow(800, 600, "Preview");
        //Raylib.SetTargetFPS(60);
    }

    public static void CloseWindow()
    {
        Raylib.CloseWindow();
    }

    public static void BeginDraw(float x, float y)
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);
        var camera = new Camera2D(new(x * 8,y * 8), new(400, 300), 0, -1);

        Raylib.BeginMode2D(camera);
    }

    internal static void Write(int v1, int v2, string v3)
    {
        Raylib.DrawText(v3, v1 * 8, v2 * 8, 1, Color.White);
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
