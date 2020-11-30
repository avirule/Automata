using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using Intrinsic = System.Numerics.Vector;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable CognitiveComplexity

namespace Automata.Engine.Numerics
{
    public readonly partial struct Vector2<T> where T : unmanaged
    {
[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> Add(Vector2<T> a, Vector2<T> b)
        {
            if (typeof(T) == typeof(float))
            {
                Vector2 result = a.AsRef<T, Vector2>() + b.AsRef<T, Vector2>();
                return result.AsGenericRef<T>();
            }
            else
            {
                Vector<T> result = a.AsVectorRef() + b.AsVectorRef();
                return result.AsVector2Ref();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> Subtract(Vector2<T> a, Vector2<T> b)
        {
            if (typeof(T) == typeof(float))
            {
                Vector2 result = a.AsRef<T, Vector2>() - b.AsRef<T, Vector2>();
                return result.AsGenericRef<T>();
            }
            else
            {
                Vector<T> result = a.AsVectorRef() - b.AsVectorRef();
                return result.AsVector2Ref();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> Multiply(Vector2<T> a, Vector2<T> b)
        {
            if (typeof(T) == typeof(float))
            {
                Vector2 result = a.AsRef<T, Vector2>() * b.AsRef<T, Vector2>();
                return result.AsGenericRef<T>();
            }
            else
            {
                Vector<T> result = a.AsVectorRef() * b.AsVectorRef();
                return result.AsVector2Ref();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> Divide(Vector2<T> a, Vector2<T> b)
        {
            if (typeof(T) == typeof(float))
            {
                Vector2 result = a.AsRef<T, Vector2>() / b.AsRef<T, Vector2>();
                return result.AsGenericRef<T>();
            }
            else
            {
                Vector<T> result = a.AsVectorRef() / b.AsVectorRef();
                return result.AsVector2Ref();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> And(Vector2<T> a, Vector2<T> b)
        {
            Vector<T> result = Intrinsic.BitwiseAnd(a.AsVectorRef(), b.AsVectorRef());
            return result.AsVector2Ref();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> Or(Vector2<T> a, Vector2<T> b)
        {
            Vector<T> result = Intrinsic.BitwiseOr(a.AsVectorRef(), b.AsVectorRef());
            return result.AsVector2Ref();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> Abs(Vector2<T> a)
        {
            Vector<T> result = Intrinsic.Abs(a.AsVectorRef());
            return result.AsVector2Ref();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> Floor(Vector2<T> a)
        {
            if (typeof(T) == typeof(float))
            {
                Vector<float> result = Intrinsic.Floor(a.AsVector<T, float>());
                return result.AsVector2Ref<float, T>();
            }
            else if (typeof(T) == typeof(double))
            {
                Vector<double> result = Intrinsic.Floor(a.AsVector<T, double>());
                return result.AsVector2Ref<double, T>();
            }
            else
            {
                return a;
            }
        }


        #region Comparison

        /// <summary>
        ///     Reduces a given <see cref="Vector128" /> to booleans representing each of its elements.
        /// </summary>
        /// <param name="a"><see cref="Vector128{T}" /> to reduce.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector2<bool> BooleanReduction<TType>(Vector<TType> a) where TType : unmanaged =>
            new Vector2<bool>(
                !a[0].Equals(default),
                !a[1].Equals(default));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<bool> Equals(Vector2<T> a, Vector2<T> b) => BooleanReduction(Intrinsic.Equals(a.AsVectorRef(), b.AsVectorRef()));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<bool> NotEquals(Vector2<T> a, Vector2<T> b) =>
            BooleanReduction(Intrinsic.OnesComplement(Intrinsic.Equals(a.AsVectorRef(), b.AsVectorRef())));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<bool> GreaterThan(Vector2<T> a, Vector2<T> b) => BooleanReduction(Intrinsic.GreaterThan(a.AsVectorRef(), b.AsVectorRef()));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<bool> LessThan(Vector2<T> a, Vector2<T> b) => BooleanReduction(Intrinsic.LessThan(a.AsVectorRef(), b.AsVectorRef()));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector2<bool> GreaterThanOrEqual(Vector2<T> a, Vector2<T> b) =>
            BooleanReduction(Intrinsic.GreaterThanOrEqual(a.AsVectorRef(), b.AsVectorRef()));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector2<bool> LessThanOrEqual(Vector2<T> a, Vector2<T> b) => BooleanReduction(Intrinsic.LessThanOrEqual(a.AsVectorRef(), b.AsVectorRef()));

        #endregion
    }
}
