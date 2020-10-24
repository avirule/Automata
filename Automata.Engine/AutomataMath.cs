#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Automata.Engine.Components;
using Automata.Engine.Entities;

#endregion


// ReSharper disable InconsistentNaming

namespace Automata.Engine
{
    public static class AutomataMath
    {
        public static float UnLerp(float a, float b, float interpolant) => (interpolant - a) / (b - a);

        public static float ToRadians(float degrees) => degrees * ((float)Math.PI / 180f);
        public static Vector3 ToRadians(Vector3 degrees) => new Vector3(ToRadians(degrees.X), ToRadians(degrees.Y), ToRadians(degrees.Z));

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

        public static int Wrap(int v, int delta, int minVal, int maxVal)
        {
            int mod = (maxVal + 1) - minVal;
            v += delta - minVal;
            v += (1 - (v / mod)) * mod;
            return (v % mod) + minVal;
        }

        public static byte AsByte(this bool a) => (byte)(Unsafe.As<bool, byte>(ref a) * byte.MaxValue);
        public static bool AsBool(this byte a) => Unsafe.As<byte, bool>(ref a);
        public static byte FirstByte(this double a) => Unsafe.As<double, byte>(ref a);

        public static Vector3 RoundBy(this Vector3 a, Vector3 b) =>
            new Vector3((float)Math.Floor(a.X / b.X), (float)Math.Floor(a.Y / b.Y), (float)Math.Floor(a.Z / b.Z)) * b;

        public static bool TryCreateModelMatrixFromEntity(IEntity entity, out Matrix4x4 modelMatrix)
        {
            if ((entity.TryGetComponent(out Scale? modelScale) && modelScale.Changed)
                | (entity.TryGetComponent(out Rotation? modelRotation) && modelRotation.Changed)
                | (entity.TryGetComponent(out Translation? modelTranslation) && modelTranslation.Changed))
            {
                modelMatrix = Matrix4x4.Identity;
                modelMatrix *= Matrix4x4.CreateScale(modelScale?.Value ?? Scale.DEFAULT);
                modelMatrix *= Matrix4x4.CreateFromQuaternion(modelRotation?.Value ?? Quaternion.Identity);
                modelMatrix *= Matrix4x4.CreateTranslation(modelTranslation?.Value ?? Vector3.Zero);
                return true;
            }
            else
            {
                modelMatrix = default;
                return false;
            }
        }
    }
}
