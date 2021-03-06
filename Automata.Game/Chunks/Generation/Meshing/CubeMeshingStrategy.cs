using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Automata.Engine.Collections;
using Automata.Engine.Numerics;
using Automata.Engine.Rendering.OpenGL;
using Automata.Game.Blocks;

namespace Automata.Game.Chunks.Generation.Meshing
{
    public struct CubeMeshingStrategy : IMeshingStrategy
    {
        private static readonly int[][] _PackedVertexesByIteration =
        {
            // 3   0
            //
            // 2   1

            // z y x
            new[]
            {
                // East      z      y      x
                0b01_01_10_000001_000001_000001,
                0b01_01_10_000001_000000_000001,
                0b01_01_10_000000_000000_000001,
                0b01_01_10_000000_000001_000001
            },
            new[]
            {
                // Up        z      y      x
                0b01_10_01_000001_000001_000000,
                0b01_10_01_000001_000001_000001,
                0b01_10_01_000000_000001_000001,
                0b01_10_01_000000_000001_000000
            },
            new[]
            {
                // North     z      y      x
                0b10_01_01_000001_000001_000000,
                0b10_01_01_000001_000000_000000,
                0b10_01_01_000001_000000_000001,
                0b10_01_01_000001_000001_000001
            },
            new[]
            {
                // West      z      y      x
                0b01_01_00_000000_000001_000000,
                0b01_01_00_000000_000000_000000,
                0b01_01_00_000001_000000_000000,
                0b01_01_00_000001_000001_000000
            },
            new[]
            {
                // Down      z      y      x
                0b01_00_01_000001_000000_000001,
                0b01_00_01_000001_000000_000000,
                0b01_00_01_000000_000000_000000,
                0b01_00_01_000000_000000_000001
            },
            new[]
            {
                // South     z      y      x
                0b00_01_01_000000_000001_000001,
                0b00_01_01_000000_000000_000001,
                0b00_01_01_000000_000000_000000,
                0b00_01_01_000000_000001_000000
            }
        };

        private static readonly int[] _IndexStepByNormalIndex =
        {
            1,
            GenerationConstants.CHUNK_SIZE_SQUARED,
            GenerationConstants.CHUNK_SIZE,
            -1,
            -GenerationConstants.CHUNK_SIZE_SQUARED,
            -GenerationConstants.CHUNK_SIZE
        };

        public void Mesh(Span<Block> blocks, Span<Direction> faces, ICollection<QuadIndexes<uint>> indexes, ICollection<QuadVertexes<PackedVertex>> vertexes,
            IReadOnlyList<Palette<Block>?> neighbors, int index, int localPosition, Block block, bool isTransparent)
        {
            // iterate once over all 6 faces of given cubic space
            for (int normalIndex = 0; normalIndex < 6; normalIndex++)
            {
                // face direction always exists on a single bit, so shift 1 by the current normalIndex (0-5)
                Direction faceDirection = (Direction)(1 << normalIndex);

                // check if current index has face already
                if (faces[index].HasDirection(faceDirection))
                {
                    continue;
                }

                // indicates whether the current face checking direction is negative or positive
                bool isNegativeNormal = (normalIndex - 3) >= 0;

                // normalIndex constrained to represent the 3 axes
                int componentIndex = normalIndex % 3;
                int componentShift = GenerationConstants.CHUNK_SIZE_SHIFT * componentIndex;

                // axis value of the current face check direction
                int facedAxisValue = (localPosition >> componentShift) & GenerationConstants.CHUNK_SIZE_MASK;

                // indicates whether or not the face check is within the current chunk bounds
                bool facingNeighbor = (!isNegativeNormal && (facedAxisValue == (GenerationConstants.CHUNK_SIZE - 1)))
                                      || (isNegativeNormal && (facedAxisValue == 0));

                // total number of successful traversals
                int traversals = 0;

                for (int perpendicularNormalIndex = 1; perpendicularNormalIndex < 3; perpendicularNormalIndex++)
                {
                    // the index of the int3 traversalNormal to traverse on
                    int traversalNormalIndex = (componentIndex + perpendicularNormalIndex) % 3;
                    int traversalNormalShift = GenerationConstants.CHUNK_SIZE_SHIFT * traversalNormalIndex;

                    // current value of the local position by traversal direction
                    int traversalNormalAxisValue = (localPosition >> traversalNormalShift) & GenerationConstants.CHUNK_SIZE_MASK;

                    // amount by integer to add to current index to get 3D->1D position of traversal position
                    int traversalIndexStep = _IndexStepByNormalIndex[traversalNormalIndex];

                    // current traversal index, which is increased by traversalIndexStep every iteration the for loop below
                    int traversalIndex = index + (traversals * traversalIndexStep);

                    // local start axis position + traversals
                    int totalTraversalLength = traversalNormalAxisValue + traversals;

                    for (;
                        (totalTraversalLength < GenerationConstants.CHUNK_SIZE)
                        && !faces[traversalIndex].HasDirection(faceDirection)
                        && (blocks[traversalIndex].ID == block.ID);
                        traversalIndex += traversalIndexStep, // increment traversal index
                        totalTraversalLength++, // increment total traversals
                        traversals++) // increment traversals
                    {
                        // check if current facing block axis value is within the local chunk
                        if (facingNeighbor)
                        {
                            // this block of code translates the integer local position to the local position of the neighbor at [normalIndex]
                            int sign = isNegativeNormal ? -1 : 1;
                            int componentMask = GenerationConstants.CHUNK_SIZE_MASK << componentShift;
                            int traversalLocalPosition = localPosition + (traversals << traversalNormalShift);

                            int neighborLocalPosition = (~componentMask & traversalLocalPosition)
                                                        | (Wrap(((traversalLocalPosition & componentMask) >> componentShift) + sign,
                                                               GenerationConstants.CHUNK_SIZE, 0, GenerationConstants.CHUNK_SIZE - 1)
                                                           << componentShift);

                            // index into neighbor blocks collections, call .GetPoint() with adjusted local position
                            // remark: if there's no neighbor at the index given, then no chunk exists there (for instance,
                            //     chunks at the edge of render distance). In this case, return NullID so no face is rendered on edges.
                            int facedBlockIndex = Vector3i.Project1D(
                                neighborLocalPosition & GenerationConstants.CHUNK_SIZE_MASK,
                                (neighborLocalPosition >> (GenerationConstants.CHUNK_SIZE_SHIFT * 1)) & GenerationConstants.CHUNK_SIZE_MASK,
                                (neighborLocalPosition >> (GenerationConstants.CHUNK_SIZE_SHIFT * 2)) & GenerationConstants.CHUNK_SIZE_MASK,
                                GenerationConstants.CHUNK_SIZE);

                            ushort facedBlockID = neighbors[normalIndex]?[facedBlockIndex].ID ?? BlockRegistry.NullID;

                            if (isTransparent)
                            {
                                if (block.ID != facedBlockID)
                                {
                                    break;
                                }
                            }
                            else if (!BlockRegistry.Instance.CheckBlockHasProperty(facedBlockID, IBlockDefinition.Attribute.Transparent))
                            {
                                break;
                            }
                        }
                        else
                        {
                            // amount by integer to add to current traversal index to get 3D->1D position of facing block
                            int facedBlockIndex = traversalIndex + _IndexStepByNormalIndex[normalIndex];

                            // if so, index into block ids and set facingBlockId
                            ushort facedBlockID = blocks[facedBlockIndex].ID;

                            // if transparent, traverse so long as facing block is not the same block id
                            // if opaque, traverse so long as facing block is transparent
                            if (isTransparent)
                            {
                                if (block.ID != facedBlockID)
                                {
                                    break;
                                }
                            }
                            else if (!BlockRegistry.Instance.CheckBlockHasProperty(facedBlockID, IBlockDefinition.Attribute.Transparent))
                            {
                                if (!isNegativeNormal)

                                    // we've culled the current face, and faced block is opaque as well, so cull it's face to current.
                                {
                                    faces[facedBlockIndex] |= (Direction)(1 << ((normalIndex + 3) % 6));
                                }

                                break;
                            }
                        }

                        faces[traversalIndex] |= faceDirection;
                    }

                    // face is occluded
                    if (traversals == 0)
                    {
                        break;
                    }

                    // if it's the first traversal and we've only made a 1x1x1 face, continue to test next axis
                    else if ((traversals == 1) && (perpendicularNormalIndex == 1))
                    {
                        continue;
                    }

                    Span<int> compressedVertices = _PackedVertexesByIteration[normalIndex];
                    int traversalComponentMask = GenerationConstants.CHUNK_SIZE_MASK << traversalNormalShift;
                    int unaryTraversalComponentMask = ~traversalComponentMask;

                    // this ternary solution should probably be temporary. not sure if there's a better way, though.
                    int uvShift = (componentIndex + traversalNormalIndex + ((componentIndex == 1) && (traversalNormalIndex == 2) ? 1 : 0)) % 2;

                    int compressedUV = (TextureAtlas.Instance.GetTileDepth(BlockRegistry.Instance.GetBlockName(block.ID))
                                        << (GenerationConstants.CHUNK_SIZE_SHIFT * 2)) // z
                                       | (traversals << (GenerationConstants.CHUNK_SIZE_SHIFT * uvShift)) // traversal component
                                       | (1 << (GenerationConstants.CHUNK_SIZE_SHIFT * ((uvShift + 1) % 2))); // opposite component to traversal

                    uint indexesStart = (uint)(vertexes.Count * 4);

                    indexes.Add(new QuadIndexes<uint>(
                        indexesStart + 0u,
                        indexesStart + 1u,
                        indexesStart + 3u,
                        indexesStart + 1u,
                        indexesStart + 2u,
                        indexesStart + 3u
                    ));

                    vertexes.Add(new QuadVertexes<PackedVertex>(
                        new PackedVertex(
                            localPosition
                            + ((unaryTraversalComponentMask & compressedVertices[0])
                               | ((((compressedVertices[0] >> traversalNormalShift) * traversals) << traversalNormalShift) & traversalComponentMask)),
                            compressedUV & (int.MaxValue << (GenerationConstants.CHUNK_SIZE_SHIFT * 2))),
                        new PackedVertex(
                            localPosition
                            + ((unaryTraversalComponentMask & compressedVertices[1])
                               | ((((compressedVertices[1] >> traversalNormalShift) * traversals) << traversalNormalShift) & traversalComponentMask)),
                            compressedUV & (int.MaxValue << GenerationConstants.CHUNK_SIZE_SHIFT)),
                        new PackedVertex(
                            localPosition
                            + ((unaryTraversalComponentMask & compressedVertices[2])
                               | ((((compressedVertices[2] >> traversalNormalShift) * traversals) << traversalNormalShift) & traversalComponentMask)),
                            compressedUV & int.MaxValue),
                        new PackedVertex(
                            localPosition
                            + ((unaryTraversalComponentMask & compressedVertices[3])
                               | ((((compressedVertices[3] >> traversalNormalShift) * traversals) << traversalNormalShift) & traversalComponentMask)),
                            compressedUV & ~(GenerationConstants.CHUNK_SIZE_MASK << GenerationConstants.CHUNK_SIZE_SHIFT))));

                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Wrap(int value, int delta, int minVal, int maxVal)
        {
            int mod = (maxVal + 1) - minVal;
            value += delta - minVal;
            value += (1 - (value / mod)) * mod;
            return (value % mod) + minVal;
        }
    }
}
