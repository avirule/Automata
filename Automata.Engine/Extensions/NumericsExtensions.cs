using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Automata.Engine.Extensions
{
    public static class NumericsExtensions
    {
        // row major
        public static unsafe Span<float> Unroll(this Matrix4x4 matrix) => MemoryMarshal.CreateSpan(ref matrix.M11, sizeof(Matrix4x4) / sizeof(float));

        public static IEnumerable<float> UnrollColumnMajor(this Matrix4x4 matrix)
        {
            yield return matrix.M11;
            yield return matrix.M21;
            yield return matrix.M31;
            yield return matrix.M41;

            yield return matrix.M12;
            yield return matrix.M22;
            yield return matrix.M32;
            yield return matrix.M42;

            yield return matrix.M13;
            yield return matrix.M23;
            yield return matrix.M33;
            yield return matrix.M43;

            yield return matrix.M14;
            yield return matrix.M24;
            yield return matrix.M34;
            yield return matrix.M44;
        }

        public static Vector3 RoundBy(this Vector3 a, Vector3 by)
        {
            Vector3 rounded = a / by;
            rounded = new Vector3(MathF.Floor(rounded.X), MathF.Floor(rounded.Y), MathF.Floor(rounded.Z));
            return rounded * by;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sum(this Vector2 a) => a.X + a.Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sum(this Vector3 a) => a.X + a.Y + a.Z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sum(this Vector4 a) => a.X + a.Y + a.Z + a.W;
    }
}