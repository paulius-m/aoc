using Raylib_cs;
using System.Collections.Immutable;
using System.Numerics;

namespace Days.Y2023.Day25;

internal class Render
{
    private static Camera2D camera = new Camera2D(
            new(50f, 50f), new(0, 0), 0, 5
            );

    public static void InitWindow()
    {
        Raylib.InitWindow(800, 600, "TEST");
        Raylib.SetTargetFPS(60);
    }

    public static void CloseWindow()
    {
        Raylib.CloseWindow();
    }

    internal static void Draw(Dictionary<string, Vector2> vertexes, ImmutableDictionary<string, HashSet<string>> graph)
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        Raylib.BeginMode2D(camera);

        foreach (var (u, vs) in graph)
        {
            foreach (var v in vs)
            {
                Raylib.DrawLineEx(vertexes[u], vertexes[v], 0.5f, Color.White);
            }
            Raylib.DrawPixelV(vertexes[u], new Color((byte)(vertexes[u].X / 100 * byte.MaxValue), (byte)(vertexes[u].Y / 100 * byte.MaxValue), (byte)0, (byte)100));
        }
        Raylib.EndMode2D();
        Raylib.EndDrawing();
    }
}