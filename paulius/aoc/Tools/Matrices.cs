using System.Numerics;
using static System.Numerics.Matrix4x4;

namespace Tools;

public static class Matrices
{
    public static Matrix4x4[] Rotations = new[]
    {
        Identity,
        new Matrix4x4(1, 0, 0, 0,
                      0, 0, -1, 0,
                      0, 1, 0, 0,
                      0, 0, 0, 1),

        new Matrix4x4(1, 0, 0, 0,
                      0, -1, 0, 0,
                      0, 0, -1, 0,
                      0, 0, 0, 1),

        new Matrix4x4(1, 0, 0, 0,
                      0, 0, 1, 0,
                      0, -1, 0, 0,
                      0, 0, 0, 1),

        new Matrix4x4(0, 0, 1, 0,
                      0, 1, 0, 0,
                      -1, 0, 0, 0,
                      0, 0, 0, 1),

        new Matrix4x4(-1, 0, 0, 0,
                      0, 1, 0, 0,
                      0, 0, -1, 0,
                      0, 0, 0, 1),

        new Matrix4x4(0, 0, -1, 0,
                      0, 1, 0, 0,
                      1, 0, 0, 0,
                      0, 0, 0, 1),

        new Matrix4x4(0, -1, 0, 0,
                      1, 0, 0, 0,
                      0, 0, 1, 0,
                      0, 0, 0, 1),

        new Matrix4x4(-1, 0, 0, 0,
                      0, -1, 0, 0,
                      0, 0, 1, 0,
                      0, 0, 0, 1),

        new Matrix4x4(0, 1, 0, 0,
                      -1, 0, 0, 0,
                      0, 0, 1, 0,
                      0, 0, 0, 1)
    };
}
