using System.Buffers;
using Automata.Engine.Memory;
using Serilog;
using Silk.NET.OpenGL;

namespace Automata.Engine.Rendering.OpenGL.Buffers
{
    public class BufferAllocator : OpenGLObject
    {
        private readonly NativeMemoryPool _NativeMemoryPool;

        public int RentedBufferCount => _NativeMemoryPool.RentedBlocks;

        public unsafe BufferAllocator(GL gl, nuint size) : base(gl)
        {
            Handle = GL.CreateBuffer();

            GL.NamedBufferStorage(Handle, size, (void*)null!, (uint)(BufferStorageMask.MapWriteBit
                                                                     | BufferStorageMask.MapPersistentBit
                                                                     | BufferStorageMask.MapCoherentBit));

            void* pointer = GL.MapNamedBuffer(Handle, BufferAccessARB.WriteOnly);
            _NativeMemoryPool = new NativeMemoryPool((byte*)pointer, size);
        }

        public IMemoryOwner<T> Rent<T>(int size, nuint alignment, out nuint index, bool clear = false) where T : unmanaged =>
            _NativeMemoryPool.Rent<T>(size, alignment, out index, clear);

        public void ValidateBlocks()
        {
            _NativeMemoryPool.ValidateBlocks();
            Log.Debug(string.Format(FormatHelper.DEFAULT_LOGGING, $"{nameof(BufferAllocator)} 0x{Handle:x})", "Successfully validated all allocation blocks."));
        }


        #region IDisposable

        protected override void CleanupNativeResources()
        {
            GL.UnmapNamedBuffer(Handle);
            GL.DeleteBuffer(Handle);
        }

        #endregion
    }
}
