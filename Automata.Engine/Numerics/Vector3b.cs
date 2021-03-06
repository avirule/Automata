using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using Automata.Engine.Extensions;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Automata.Engine.Numerics
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly partial struct Vector3b : IEquatable<Vector3b>
    {
        public static Vector3b True { get; } = new Vector3b(true);
        public static Vector3b False { get; } = new Vector3b(false);

        private readonly byte _X;
        private readonly byte _Y;
        private readonly byte _Z;

        public bool X => _X.AsBool();
        public bool Y => _Y.AsBool();
        public bool Z => _Z.AsBool();

        public bool this[int index] => index switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            _ => throw new IndexOutOfRangeException(nameof(index))
        };

        private Vector3b(byte x, byte y, byte z) => (_X, _Y, _Z) = (x, y, z);

        public Vector3b(bool value)
        {
            byte numericValue = value.AsByte();

            (_X, _Y, _Z) = (numericValue, numericValue, numericValue);
        }

        public Vector3b(bool x, bool y, bool z) =>
            (_X, _Y, _Z) = (x.AsByte(), y.AsByte(), z.AsByte());

        public override bool Equals(object? obj) => obj is Vector3b other && Equals(other);
        public bool Equals(Vector3b other) => All(this == other);

        public override int GetHashCode() => _X.GetHashCode() ^ _Y.GetHashCode() ^ _Z.GetHashCode();

        public override string ToString() => string.Format(FormatHelper.VECTOR_3_COMPONENT, nameof(Vector3b), X, Y, Z);


        #region Operators

        public static Vector3b operator ==(Vector3b a, Vector3b b) => EqualsImpl(a, b);
        public static Vector3b operator !=(Vector3b a, Vector3b b) => NotEqualsImpl(a, b);
        public static Vector3b operator !(Vector3b a) => new Vector3b((byte)~a._X, (byte)~a._Y, (byte)~a._Z);
        public static Vector3b operator |(Vector3b a, Vector3b b) => OrImpl(a, b);

        #endregion


        #region Conversions

        public static explicit operator Vector128<byte>(Vector3b a) => Unsafe.As<Vector3b, Vector128<byte>>(ref a);
        public static explicit operator Vector3b(Vector128<byte> a) => Unsafe.As<Vector128<byte>, Vector3b>(ref a);

        public static explicit operator Vector3b(Vector128<int> a) => new Vector3b(
            (byte)a.GetElement(0),
            (byte)a.GetElement(1),
            (byte)a.GetElement(2));

        public static explicit operator Vector3b(Vector256<double> a) => new Vector3b(
            a.GetElement(0).FirstByte(),
            a.GetElement(1).FirstByte(),
            a.GetElement(2).FirstByte());

        public static implicit operator Vector3b((bool X, bool Y, bool Z) a) => new Vector3b(a.X, a.Y, a.Z);

        #endregion
    }
}
