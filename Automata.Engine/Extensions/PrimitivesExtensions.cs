using System;
using System.Runtime.CompilerServices;

namespace Automata.Engine.Extensions
{
    public static class PrimitivesExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte AsByte(this bool a) => (byte)(Unsafe.As<bool, byte>(ref a) * byte.MaxValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AsBool(this byte a) => Unsafe.As<byte, bool>(ref a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte FirstByte(this double a) => Unsafe.As<double, byte>(ref a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Unlerp(this float interpolant, float a, float b) => (interpolant - a) / (b - a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToRadians(this float degrees) => degrees * ((float)Math.PI / 180f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte CountBits(this uint i)
        {
            i -= (i >> 1) & 0x55555555;
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (byte)((((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24);
        }
    }
}