#region

using System;
using Silk.NET.OpenGL;

#endregion


namespace Automata.Engine.Rendering.OpenGL.Buffers
{
    public enum BufferDraw
    {
        /// <summary>
        ///     Draw type used when buffer data will be changed every frame (i.e. particles).
        /// </summary>
        StreamDraw = 35040,

        /// <summary>
        ///     Draw type to use when buffer data will never be updated (i.e. static scene geometry).
        /// </summary>
        StaticDraw = 35044,

        /// <summary>
        ///     Draw type to use when buffer will be updated periodically (i.e. chunks).
        /// </summary>
        DynamicDraw = 35048
    }

    public class BufferObject<TData> : OpenGLObject, IDisposable where TData : unmanaged
    {
        private bool _Disposed;

        public uint Length { get; private set; }
        public uint ByteLength { get; private set; }

        public BufferObject(GL gl) : base(gl) => Handle = GL.CreateBuffer();

        public unsafe void SetBufferData(Span<TData> data, BufferDraw bufferDraw)
        {
            Length = (uint)data.Length;
            ByteLength = Length * (uint)sizeof(TData);
            GL.NamedBufferData(Handle, ByteLength, data, (VertexBufferObjectUsage)bufferDraw);
        }

        public unsafe void SetBufferData(int offset, Span<TData> data) => GL.NamedBufferSubData(Handle, data.Length * sizeof(TData), ByteLength, ref data[0]);

        public unsafe void SetBufferData(Span<(int, TData)> data)
        {
            foreach ((int offset, TData datum) in data) GL.NamedBufferSubData(Handle, offset, (uint)sizeof(TData), &datum);
        }

        public unsafe void SetBufferData(uint length, uint indexSize, void* data, BufferDraw bufferDraw)
        {
            Length = length;
            ByteLength = length * indexSize;
            GL.NamedBufferData(Handle, ByteLength, data, (VertexBufferObjectUsage)bufferDraw);
        }

        public unsafe void SetBufferData(int offset, uint length, uint indexSize, void* data)
        {
            Length = length;
            ByteLength = length * indexSize;
            GL.NamedBufferSubData(Handle, offset, ByteLength, data);
        }

        public void Bind(BufferTargetARB target) => GL.BindBuffer(target, Handle);
        public void Unbind(BufferTargetARB target) => GL.BindBuffer(target, 0);

        public void Dispose()
        {
            if (_Disposed) return;

            GC.SuppressFinalize(this);
            GL.DeleteBuffer(Handle);
            _Disposed = true;
        }
    }
}
