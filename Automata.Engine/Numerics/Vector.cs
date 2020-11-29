using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Automata.Engine.Numerics
{
    public static class Vector
    {
        public static void ThrowNotSupportedGenericType() => throw new NotSupportedException("Generic vectors only support primitive types.");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<TTo> Coerce<TFrom, TTo>(this Vector2<TFrom> vector)
            where TFrom : unmanaged
            where TTo : unmanaged =>
            typeof(TFrom) == typeof(TTo)
                ? (Vector2<TTo>)(object)vector
                : new Vector2<TTo>((TTo)(object)vector.X, (TTo)(object)vector.Y);


        #region Conversions

        public static Point AsPoint(this Vector2<int> vector) => Unsafe.As<Vector2<int>, Point>(ref vector);
        public static Vector2<int> AsVector(this Point point) => Unsafe.As<Point, Vector2<int>>(ref point);

        public static PointF AsPointF(this Vector2<float> vector) => Unsafe.As<Vector2<float>, PointF>(ref vector);
        public static Vector2<float> AsVector(this PointF pointF) => Unsafe.As<PointF, Vector2<float>>(ref pointF);

        public static Size AsSize(this Vector2<int> vector) => Unsafe.As<Vector2<int>, Size>(ref vector);
        public static Vector2<int> AsVector(this Size size) => Unsafe.As<Size, Vector2<int>>(ref size);

        #endregion


        #region AsVector

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector<T> AsVector<T>(this Vector2<T> vector) where T : unmanaged => Unsafe.As<Vector2<T>, Vector<T>>(ref vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector<T> AsVector<T>(this Vector3<T> vector) where T : unmanaged => Unsafe.As<Vector3<T>, Vector<T>>(ref vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector<T> AsVector<T>(this Vector4<T> vector) where T : unmanaged => Unsafe.As<Vector4<T>, Vector<T>>(ref vector);

        #endregion


        #region AsVector2/3/4

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> AsVector2<T>(this Vector128<T> vector)
            where T : unmanaged =>
            Unsafe.As<Vector128<T>, Vector2<T>>(ref vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<TTo> AsVector2<TFrom, TTo>(this Vector128<TFrom> vector)
            where TFrom : unmanaged
            where TTo : unmanaged =>
            Unsafe.As<Vector128<TFrom>, Vector2<TTo>>(ref vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3<TTo> AsVector3<TFrom, TTo>(this Vector128<TFrom> vector)
            where TFrom : unmanaged
            where TTo : unmanaged =>
            Unsafe.As<Vector128<TFrom>, Vector3<TTo>>(ref vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4<TTo> AsVector4<TFrom, TTo>(this Vector128<TFrom> vector)
            where TFrom : unmanaged
            where TTo : unmanaged =>
            Unsafe.As<Vector128<TFrom>, Vector4<TTo>>(ref vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<TTo> AsVector2<TFrom, TTo>(this Vector256<TFrom> vector)
            where TFrom : unmanaged
            where TTo : unmanaged =>
            Unsafe.As<Vector256<TFrom>, Vector2<TTo>>(ref vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3<TTo> AsVector3<TFrom, TTo>(this Vector256<TFrom> vector)
            where TFrom : unmanaged
            where TTo : unmanaged =>
            Unsafe.As<Vector256<TFrom>, Vector3<TTo>>(ref vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4<TTo> AsVector4<TFrom, TTo>(this Vector256<TFrom> vector)
            where TFrom : unmanaged
            where TTo : unmanaged =>
            Unsafe.As<Vector256<TFrom>, Vector4<TTo>>(ref vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> AsVector2<T>(this Vector<T> vector) where T : unmanaged => Unsafe.As<Vector<T>, Vector2<T>>(ref vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3<T> AsVector3<T>(this Vector<T> vector) where T : unmanaged => Unsafe.As<Vector<T>, Vector3<T>>(ref vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4<T> AsVector4<T>(this Vector<T> vector) where T : unmanaged => Unsafe.As<Vector<T>, Vector4<T>>(ref vector);

        #endregion


        #region AsVector128

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<T> AsVector128<T>(this Vector2<T> vector)
            where T : unmanaged =>
            Unsafe.As<Vector2<T>, Vector128<T>>(ref vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<TTo> AsVector128<TFrom, TTo>(this Vector2<TFrom> vector)
            where TFrom : unmanaged
            where TTo : unmanaged =>
            Unsafe.As<Vector2<TFrom>, Vector128<TTo>>(ref vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<TTo> AsVector128<TFrom, TTo>(this Vector3<TFrom> vector)
            where TFrom : unmanaged
            where TTo : unmanaged =>
            Unsafe.As<Vector3<TFrom>, Vector128<TTo>>(ref vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<TTo> AsVector128<TFrom, TTo>(this Vector4<TFrom> vector)
            where TFrom : unmanaged
            where TTo : unmanaged =>
            Unsafe.As<Vector4<TFrom>, Vector128<TTo>>(ref vector);

        #endregion


        #region AsVector256

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<TTo> AsVector256<TFrom, TTo>(this Vector2<TFrom> vector)
            where TFrom : unmanaged
            where TTo : unmanaged =>
            Unsafe.As<Vector2<TFrom>, Vector256<TTo>>(ref vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<TTo> AsVector256<TFrom, TTo>(this Vector3<TFrom> vector)
            where TFrom : unmanaged
            where TTo : unmanaged =>
            Unsafe.As<Vector3<TFrom>, Vector256<TTo>>(ref vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<TTo> AsVector256<TFrom, TTo>(this Vector4<TFrom> vector)
            where TFrom : unmanaged
            where TTo : unmanaged =>
            Unsafe.As<Vector4<TFrom>, Vector256<TTo>>(ref vector);

        #endregion


        #region AsIntrinsic

        /// <summary>
        ///     Converts a given generic vector to its intrinsic variant.
        /// </summary>
        /// <remarks>
        ///     It's assumed that T is a valid type. No type checking is done by this method for performance.
        /// </remarks>
        /// <param name="vector">Vector to convert.</param>
        /// <typeparam name="T">Unmanaged type to convert generic to.</typeparam>
        /// <returns>Intrinsic variant of the given vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 AsIntrinsic<T>(this Vector2<T> vector) where T : unmanaged => Unsafe.As<Vector2<T>, Vector2>(ref vector);

        /// <inheritdoc cref="AsIntrinsic{T}(Automata.Engine.Numerics.Vector2{T})" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 AsIntrinsic<T>(this Vector3<T> vector) where T : unmanaged => Unsafe.As<Vector3<T>, Vector3>(ref vector);

        /// <inheritdoc cref="AsIntrinsic{T}(Automata.Engine.Numerics.Vector2{T})" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 AsIntrinsic<T>(this Vector4<T> vector) where T : unmanaged => Unsafe.As<Vector4<T>, Vector4>(ref vector);

        #endregion


        #region AsGeneric

        /// <summary>
        ///     Converts a given intrinsic vector to its generic variant.
        /// </summary>
        /// <remarks>
        ///     It's assumed that T is a valid type. No type checking is done by this method for performance.
        /// </remarks>
        /// <param name="vector">Vector to convert.</param>
        /// <typeparam name="T">Unmanaged type to convert generic to.</typeparam>
        /// <returns>Generic variant of the given vector and type <typeparamref name="T" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> AsGeneric<T>(this Vector2 vector) where T : unmanaged => Unsafe.As<Vector2, Vector2<T>>(ref vector);

        /// <inheritdoc cref="AsGeneric{T}(System.Numerics.Vector2)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3<T> AsGeneric<T>(this Vector3 vector) where T : unmanaged => Unsafe.As<Vector3, Vector3<T>>(ref vector);

        /// <inheritdoc cref="AsGeneric{T}(System.Numerics.Vector2)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4<T> AsGeneric<T>(this Vector4 vector) where T : unmanaged => Unsafe.As<Vector4, Vector4<T>>(ref vector);

        #endregion


        #region Equals

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool EqualsInternal<T>(Vector2<T> a, Vector2<T> b) where T : unmanaged => a.AsVector() == b.AsVector();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsInternal<T>(Vector3<T> a, Vector3<T> b) where T : unmanaged => a.AsVector() == b.AsVector();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsInternal<T>(Vector4<T> a, Vector4<T> b) where T : unmanaged => a.AsVector() == b.AsVector();

        #endregion

        #region Not Equals

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool NotEqualsInternal<T>(Vector2<T> a, Vector2<T> b) where T : unmanaged => a.AsVector() != b.AsVector();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotEqualsInternal<T>(Vector3<T> a, Vector3<T> b) where T : unmanaged => a.AsVector() != b.AsVector();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotEqualsInternal<T>(Vector4<T> a, Vector4<T> b) where T : unmanaged => a.AsVector() != b.AsVector();

        #endregion

        #region Add

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Vector2<T> AddInternal<T>(Vector2<T> a, Vector2<T> b) where T : unmanaged => (a.AsVector() + b.AsVector()).AsVector2();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3<T> AddInternal<T>(Vector3<T> a, Vector3<T> b) where T : unmanaged => (a.AsVector() + b.AsVector()).AsVector3();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4<T> AddInternal<T>(Vector4<T> a, Vector4<T> b) where T : unmanaged => (a.AsVector() + b.AsVector()).AsVector4();

        #endregion


        #region Subtract

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Vector2<T> SubtractInternal<T>(Vector2<T> a, Vector2<T> b) where T : unmanaged => (a.AsVector() - b.AsVector()).AsVector2();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3<T> SubtractInternal<T>(Vector3<T> a, Vector3<T> b) where T : unmanaged => (a.AsVector() - b.AsVector()).AsVector3();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4<T> SubtractInternal<T>(Vector4<T> a, Vector4<T> b) where T : unmanaged => (a.AsVector() - b.AsVector()).AsVector4();

        #endregion


        #region Multiply

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Vector2<T> MultiplyInternal<T>(Vector2<T> a, Vector2<T> b) where T : unmanaged =>
            typeof(T) == typeof(float)
                ? (a.AsIntrinsic() * b.AsIntrinsic()).AsGeneric<T>()
                : (a.AsVector() * b.AsVector()).AsVector2();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3<T> MultiplyInternal<T>(Vector3<T> a, Vector3<T> b) where T : unmanaged =>
            typeof(T) == typeof(float)
                ? (a.AsIntrinsic() * b.AsIntrinsic()).AsGeneric<T>()
                : (a.AsVector() * b.AsVector()).AsVector3();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4<T> MultiplyInternal<T>(Vector4<T> a, Vector4<T> b) where T : unmanaged
        {
            // these cases exist because they're more performant than Vector<T>
            if ((typeof(T) == typeof(int)) && Sse41.IsSupported)
            {
                return Sse41.MultiplyLow(a.AsVector128<T, int>(), b.AsVector128<T, int>()).AsVector4<int, T>();
            }
            else if ((typeof(T) == typeof(uint)) && Sse41.IsSupported)
            {
                return Sse41.MultiplyLow(a.AsVector128<T, uint>(), b.AsVector128<T, uint>()).AsVector4<uint, T>();
            }
            else if ((typeof(T) == typeof(short)) && Sse2.IsSupported)
            {
                return Sse2.MultiplyLow(a.AsVector128<T, short>(), b.AsVector128<T, short>()).AsVector4<short, T>();
            }
            else if ((typeof(T) == typeof(ushort)) && Sse2.IsSupported)
            {
                return Sse2.MultiplyLow(a.AsVector128<T, ushort>(), b.AsVector128<T, ushort>()).AsVector4<ushort, T>();
            }
            else if ((typeof(T) == typeof(float)) && Sse.IsSupported)
            {
                return (a.AsIntrinsic() * b.AsIntrinsic()).AsGeneric<T>();
            }
            else
            {
                return (a.AsVector() * b.AsVector()).AsVector4();
            }
        }

        #endregion


        #region Divide

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Vector2<T> DivideInternal<T>(Vector2<T> a, Vector2<T> b) where T : unmanaged =>
            typeof(T) == typeof(float)
                ? (a.AsIntrinsic() / b.AsIntrinsic()).AsGeneric<T>()
                : (a.AsVector() / b.AsVector()).AsVector2();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3<T> DivideInternal<T>(Vector3<T> a, Vector3<T> b) where T : unmanaged =>
            typeof(T) == typeof(float)
                ? (a.AsIntrinsic() / b.AsIntrinsic()).AsGeneric<T>()
                : (a.AsVector() / b.AsVector()).AsVector3();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4<T> DivideInternal<T>(Vector4<T> a, Vector4<T> b) where T : unmanaged =>
            (typeof(T) == typeof(float)) && Sse.IsSupported
                ? (a.AsIntrinsic() / b.AsIntrinsic()).AsGeneric<T>()
                : (a.AsVector() / b.AsVector()).AsVector4();

        #endregion
    }
}
