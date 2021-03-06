using System;
using System.Collections.Generic;
using Automata.Engine.Collections;
using Automata.Engine.Rendering.OpenGL;
using Automata.Game.Blocks;

namespace Automata.Game.Chunks.Generation.Meshing
{
    public class XMeshingStrategy : IMeshingStrategy
    {
        public void Mesh(Span<Block> blocks, Span<Direction> faces, ICollection<QuadIndexes<uint>> indexes, ICollection<QuadVertexes<PackedVertex>> vertexes,
            IReadOnlyList<Palette<Block>?> neighbors, int index, int localPosition, Block block, bool isTransparent)
        {
            int textureDepth = TextureAtlas.Instance.GetTileDepth(BlockRegistry.Instance.GetBlockName(block.ID)) << (GenerationConstants.CHUNK_SIZE_SHIFT * 2);

            uint indexesStart = (uint)vertexes.Count * 4u;

            indexes.Add(new QuadIndexes<uint>(
                indexesStart + 0u,
                indexesStart + 1u,
                indexesStart + 3u,
                indexesStart + 1u,
                indexesStart + 2u,
                indexesStart + 3u
            ));

            vertexes.Add(new QuadVertexes<PackedVertex>(
                new PackedVertex(localPosition + 0b00_01_10_000001_000001_000001, 0b000000_000000_000001 | textureDepth),
                new PackedVertex(localPosition + 0b00_01_10_000001_000000_000001, 0b000000_000001_000001 | textureDepth),
                new PackedVertex(localPosition + 0b00_01_10_000000_000000_000000, 0b000000_000001_000000 | textureDepth),
                new PackedVertex(localPosition + 0b00_01_10_000000_000001_000000, 0b000000_000000_000000 | textureDepth)
            ));

            indexesStart += 4;

            indexes.Add(new QuadIndexes<uint>(
                indexesStart + 0u,
                indexesStart + 1u,
                indexesStart + 3u,
                indexesStart + 1u,
                indexesStart + 2u,
                indexesStart + 3u
            ));

            vertexes.Add(new QuadVertexes<PackedVertex>(
                new PackedVertex(localPosition + 0b10_01_10_000001_000001_000000, 0b000000_000000_000001 | textureDepth),
                new PackedVertex(localPosition + 0b10_01_10_000001_000000_000000, 0b000000_000001_000001 | textureDepth),
                new PackedVertex(localPosition + 0b10_01_10_000000_000000_000001, 0b000000_000001_000000 | textureDepth),
                new PackedVertex(localPosition + 0b10_01_10_000000_000001_000001, 0b000000_000000_000000 | textureDepth)
            ));
        }
    }
}
