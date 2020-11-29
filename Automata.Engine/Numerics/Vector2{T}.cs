using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ConvertToAutoProperty

namespace Automata.Engine.Numerics
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Vector2<T> where T : unmanaged
    {
        public static Vector2<T> Zero => new Vector2<T>(default);
        public static Vector2<T> One => new Vector2<T>(Primitive<T>.One);
        public static Vector2<T> UnitX => new Vector2<T>(Primitive<T>.One, default);
        public static Vector2<T> UnitY => new Vector2<T>(default, Primitive<T>.One);

        private readonly T _X;
        private readonly T _Y;

        public T X => _X;
        public T Y => _Y;

        public Vector2(T xy)
        {
            _X = xy;
            _Y = xy;
        }

        public Vector2(T x, T y)
        {
            _X = x;
            _Y = y;
        }

        public Vector2<T> WithX(T x) => new Vector2<T>(x, _Y);
        public Vector2<T> WithY(T y) => new Vector2<T>(_X, y);


        #region Operators

        #region Equals

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector2<T> a, T b) => a == new Vector2<T>(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(T a, Vector2<T> b) => new Vector2<T>(a) == b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector2<T> a, Vector2<T> b) => Vector.EqualsInternal(a, b);

        #endregion


        #region Not Equals

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector2<T> a, T b) => a != new Vector2<T>(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(T a, Vector2<T> b) => new Vector2<T>(a) != b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector2<T> a, Vector2<T> b) => Vector.NotEqualsInternal(a, b);

        #endregion


        #region Add

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> operator +(Vector2<T> a, T b) => a + new Vector2<T>(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> operator +(T a, Vector2<T> b) => new Vector2<T>(a) + b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> operator +(Vector2<T> a, Vector2<T> b) => Vector.AddInternal(a, b);

        #endregion


        #region Subtract

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> operator -(Vector2<T> a, T b) => a - new Vector2<T>(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> operator -(T a, Vector2<T> b) => new Vector2<T>(a) - b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> operator -(Vector2<T> a, Vector2<T> b) => Vector.SubtractInternal(a, b);

        #endregion


        #region Multiply

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> operator *(Vector2<T> a, T b) => a * new Vector2<T>(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> operator *(T a, Vector2<T> b) => new Vector2<T>(a) * b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> operator *(Vector2<T> a, Vector2<T> b) => Vector.MultiplyInternal(a, b);

        #endregion


        #region Divide

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> operator /(Vector2<T> a, T b) => a / new Vector2<T>(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> operator /(T a, Vector2<T> b) => new Vector2<T>(a) / b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> operator /(Vector2<T> a, Vector2<T> b) => Vector.DivideInternal(a, b);

        #endregion

        #endregion
    }
}
