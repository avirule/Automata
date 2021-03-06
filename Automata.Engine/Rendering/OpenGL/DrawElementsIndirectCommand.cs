using System.Runtime.InteropServices;

namespace Automata.Engine.Rendering.OpenGL
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct DrawElementsIndirectCommand
    {
        public readonly uint IndexCount;
        public readonly uint InstanceCount;

        /// <summary>
        ///     Offset of your first index, relative to the start of the buffer, in units of your index type.
        /// </summary>
        public readonly uint FirstIndexOffset;

        /// <summary>
        ///     Offset of your first vertex, relative to the start of the indexes, in bytes.
        /// </summary>
        public readonly uint FirstVertexOffset;

        public readonly uint BaseInstance;

        public DrawElementsIndirectCommand(uint indexCount, uint instanceCount, uint firstIndexOffset, uint firstVertexOffset, uint baseInstance)
        {
            IndexCount = indexCount;
            InstanceCount = instanceCount;
            FirstIndexOffset = firstIndexOffset;
            FirstVertexOffset = firstVertexOffset;
            BaseInstance = baseInstance;
        }

        public override string ToString() => $"{nameof(DrawElementsIndirectCommand)} "
                                             + $"{{ {nameof(IndexCount)} = {IndexCount}, "
                                             + $"{nameof(InstanceCount)} = {InstanceCount}, "
                                             + $"{nameof(FirstIndexOffset)} = {FirstIndexOffset}, "
                                             + $"{nameof(FirstVertexOffset)} = {FirstVertexOffset}, "
                                             + $"{nameof(BaseInstance)} = {BaseInstance} }}";
    }
}
