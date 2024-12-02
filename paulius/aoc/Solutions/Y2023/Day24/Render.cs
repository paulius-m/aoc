using Raylib_cs;
using System.Numerics;

namespace Days.Y2023.Day24;

internal class Render
{
    private static Color[] _colors = [
        Color.MAGENTA, Color.RED, Color.BLUE, Color.YELLOW, Color.LIME, Color.GREEN

        ];
    public static void Draw(Ray[] rays)
    {
        Raylib.InitWindow(800, 600, "TEST");

        var target = rays.Aggregate(new[] { 0f, 0f, 0f }, (a, b) => a.Zip(b.OriginF, (aa, bb) => aa + bb / rays.Length).ToArray());

        Camera3D camera = new Camera3D(
            new(rays[0].OriginF),
            new(target),
            new(0, 1, 0), 90, CameraProjection.CAMERA_PERSPECTIVE
            );

        Raylib.SetTargetFPS(60);
        Raylib.HideCursor();
        var i = 999;
        while (!Raylib.WindowShouldClose())
        {
            Raylib.UpdateCamera(ref camera, CameraMode.CAMERA_FIRST_PERSON);
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);

            Raylib.BeginMode3D(camera);
            Vector3? prev = null;
            for (int i1 = 0; i1 < rays.Length; i1++)
            {

                Ray? r = rays[i1];
                var p = new Vector3(r.PointAt(i / 10f));
                Raylib.DrawLine3D(new(r.OriginF), p, _colors[i1 % _colors.Length]);

                if (prev != null)
                {
                    Raylib.DrawLine3D(prev.Value, p, Color.PINK);
                }

                prev = p;
            }

            Raylib.DrawGrid(50, 10);
            Raylib.EndMode3D();

            Raylib.DrawText($"{camera.Position}", 0, 0, 10, Color.BEIGE);

            Raylib.EndDrawing();
        }
        Raylib.CloseWindow();
    }

}
