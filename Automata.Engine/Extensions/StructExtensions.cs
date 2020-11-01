using System.Runtime.CompilerServices;

namespace Automata.Engine.Extensions
{
    public static class StructExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetComponent<T, TComponent>(this T a, int index, TComponent newValue)
            where T : unmanaged
            where TComponent : unmanaged
        {
            ref TComponent component = ref Unsafe.Add(ref Unsafe.As<T, TComponent>(ref a), index);
            Unsafe.WriteUnaligned(ref Unsafe.As<TComponent, byte>(ref component), newValue);
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T WithComponent<T, TComponent>(this T a, int index)
            where T : unmanaged
            where TComponent : unmanaged
        {
            T result = new T();

            ref TComponent aComponentOffset = ref a.GetComponent<T, TComponent>(index);
            ref TComponent resultComponentOffset = ref result.GetComponent<T, TComponent>(index);

            Unsafe.WriteUnaligned(ref Unsafe.As<TComponent, byte>(ref resultComponentOffset), aComponentOffset);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReplaceComponent<T, TComponent>(this T a, int index, TComponent value)
            where T : unmanaged
            where TComponent : unmanaged

        {
            T result = a;

            Unsafe.WriteUnaligned(ref Unsafe.As<TComponent, byte>(ref a.GetComponent<T, TComponent>(index)), value);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ref TComponent GetComponent<T, TComponent>(ref this T a, int index) where T : unmanaged where TComponent : unmanaged =>
            ref Unsafe.Add(ref Unsafe.As<T, TComponent>(ref a), index);
    }
}
