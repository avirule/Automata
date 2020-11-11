using Automata.Engine;
using Automata.Engine.Numerics;
using Automata.Engine.Rendering.OpenGL;
using Automata.Engine.Rendering.OpenGL.Buffers;
using Automata.Game.Chunks;
using Automata.Game.Chunks.Generation;

namespace Automata.Game
{
    public class VoxelWorld : World
    {
        public ChunkMap Chunks { get; }
        // public ApartmentBufferObject ChunkAllocator { get; }

        public VoxelWorld(bool active) : base(active)
        {
            const uint maximum_vertices = 2048;
            const uint slot_size = (maximum_vertices * 2 * 4) + (((maximum_vertices * 3) / 2) * 4);

            Chunks = new ChunkMap();
            // ChunkAllocator = new ApartmentBufferObject(GLAPI.Instance.GL, 8 * 8 * 8, slot_size);
        }

        public void AllocateChunkModification(Vector3i global, ushort newID)
        {
            Vector3i origin = Vector3i.RoundBy(global, GenerationConstants.CHUNK_SIZE);
            Chunks[]
        }
    }
}
