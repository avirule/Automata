#region

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using Automata.Engine;
using Automata.Engine.Collections;
using Automata.Engine.Components;
using Automata.Engine.Diagnostics;
using Automata.Engine.Entities;
using Automata.Engine.Input;
using Automata.Engine.Numerics;
using Automata.Engine.Rendering.Meshes;
using Automata.Engine.Rendering.OpenGL;
using Automata.Engine.Systems;
using Automata.Game.Blocks;
using ConcurrencyPools;
using DiagnosticsProviderNS;
using Serilog;
using Silk.NET.Input.Common;

#endregion


namespace Automata.Game.Chunks.Generation
{
    public class ChunkGenerationSystem : ComponentSystem
    {
        private static readonly Mutex _SingleThreadedGenerationMutex = new Mutex(false);

        private static readonly UnboundedChannelOptions _DefaultOptions = new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = false
        };

        private static readonly IVertexAttribute[] _DefaultAttributes =
        {
            new VertexAttribute<int>(0u, 1u, 0u),
            new VertexAttribute<int>(1u, 1u, sizeof(int))
        };

        private readonly OrderedLinkedList<GenerationStep> _BuildSteps;
        private readonly Channel<(Chunk, Palette<ushort>)> _PendingBlocks;
        private readonly Channel<(IEntity, PendingMesh<PackedVertex>)> _PendingMeshes;

        private bool _KeysPressed;

        public ChunkGenerationSystem()
        {
            _BuildSteps = new OrderedLinkedList<GenerationStep>();
            _BuildSteps.AddLast(new TerrainGenerationStep());
            _PendingBlocks = Channel.CreateUnbounded<(Chunk, Palette<ushort>)>(_DefaultOptions);
            _PendingMeshes = Channel.CreateUnbounded<(IEntity, PendingMesh<PackedVertex>)>(_DefaultOptions);

            DiagnosticsProvider.EnableGroup<ChunkGenerationDiagnosticGroup>();
        }

        [HandlesComponents(DistinctionStrategy.All, typeof(Translation), typeof(Chunk))]
        public override void Update(EntityManager entityManager, TimeSpan delta)
        {
            while (_PendingBlocks.Reader.TryRead(out (Chunk Chunk, Palette<ushort> Blocks) pendingBlocks))
            {
                pendingBlocks.Chunk.Blocks = pendingBlocks.Blocks;
                pendingBlocks.Chunk.State += 1;
            }

            while (_PendingMeshes.Reader.TryRead(out (IEntity Entity, PendingMesh<PackedVertex> Mesh) pendingMesh))
            {
                Chunk chunk = pendingMesh.Entity.GetComponent<Chunk>();
                ApplyMesh(entityManager, pendingMesh.Entity, chunk.ID, pendingMesh.Mesh);
                chunk.State += 1;
            }

            foreach (IEntity entity in entityManager.GetEntitiesWithComponents<Chunk, Translation>())
            {
                if (!entity.TryGetComponent(out Chunk? chunk) || !entity.TryGetComponent(out Translation? translation)) continue;

                switch (chunk.State)
                {
                    case GenerationState.Ungenerated when chunk.MinimalNeighborState() >= GenerationState.Ungenerated:
                        BoundedPool.Active.QueueWork(() => GenerateBlocks(chunk, Vector3i.FromVector3(translation.Value),
                            new GenerationStep.Parameters(GenerationConstants.Seed, GenerationConstants.FREQUENCY, GenerationConstants.PERSISTENCE)));

                        chunk.State += 1;
                        break;

                    case GenerationState.Unmeshed when chunk.MinimalNeighborState() >= GenerationState.Unmeshed:
                        BoundedPool.Active.QueueWork(() => GenerateMesh(entity, chunk, Vector3i.FromVector3(translation.Value)));

                        chunk.State += 1;
                        break;
                }
            }

            DiagnosticsInputCheck();
        }

        private void GenerateBlocks(Chunk chunk, Vector3i origin, GenerationStep.Parameters parameters)
        {
            bool releaseMutex = false;

            if (Settings.Instance.SingleThreadedGeneration)
            {
                _SingleThreadedGenerationMutex.WaitOne();
                releaseMutex = true;
            }

            Stopwatch stopwatch = DiagnosticsSystem.Stopwatches.Rent();
            stopwatch.Restart();

            Span<ushort> blocks = stackalloc ushort[GenerationConstants.CHUNK_SIZE_CUBED];

            foreach (GenerationStep generationStep in _BuildSteps) generationStep.Generate(origin, parameters, blocks);

            stopwatch.Stop();

            DiagnosticsProvider.CommitData<ChunkGenerationDiagnosticGroup, TimeSpan>(new BuildingTime(stopwatch.Elapsed));

            Log.Verbose(string.Format(FormatHelper.DEFAULT_LOGGING, nameof(ChunkGenerationSystem),
                $"Built: '{chunk.ID}' ({stopwatch.Elapsed.TotalMilliseconds:0.00}ms)"));

            stopwatch.Restart();

            Palette<ushort> palette = new Palette<ushort>(GenerationConstants.CHUNK_SIZE_CUBED, BlockRegistry.AirID);

            for (int index = 0; index < GenerationConstants.CHUNK_SIZE_CUBED; index++) palette[index] = blocks[index];

            if (!_PendingBlocks.Writer.TryWrite((chunk, palette))) Log.Error($"Failed to add chunk({origin}) blocks.");

            stopwatch.Stop();

            DiagnosticsProvider.CommitData<ChunkGenerationDiagnosticGroup, TimeSpan>(new InsertionTime(stopwatch.Elapsed));

            Log.Verbose(string.Format(FormatHelper.DEFAULT_LOGGING, nameof(ChunkGenerationSystem),
                $"Insertion: '{chunk.ID}' ({stopwatch.Elapsed.TotalMilliseconds:0.00}ms)"));

            DiagnosticsSystem.Stopwatches.Return(stopwatch);

            if (releaseMutex) _SingleThreadedGenerationMutex.ReleaseMutex();
        }

        private void GenerateMesh(IEntity entity, Chunk chunk, Vector3i origin)
        {
            if (chunk.Blocks is null)
            {
                Log.Warning(string.Format(FormatHelper.DEFAULT_LOGGING, nameof(ChunkGenerationSystem),
                    $"Attempted to mesh chunk {origin}, but it has not generated blocks."));

                return;
            }

            bool releaseMutex = false;

            if (Settings.Instance.SingleThreadedGeneration)
            {
                _SingleThreadedGenerationMutex.WaitOne();
                releaseMutex = true;
            }

            Stopwatch stopwatch = DiagnosticsSystem.Stopwatches.Rent();
            stopwatch.Restart();

            PendingMesh<PackedVertex> pendingMesh = ChunkMesher.GeneratePackedMesh(chunk.Blocks, chunk.NeighborBlocks().ToArray());

            if (!_PendingMeshes.Writer.TryWrite((entity, pendingMesh))) Log.Error($"Failed to add chunk({origin}) mesh.");

            stopwatch.Stop();

            DiagnosticsProvider.CommitData<ChunkGenerationDiagnosticGroup, TimeSpan>(new MeshingTime(stopwatch.Elapsed));

            Log.Verbose(string.Format(FormatHelper.DEFAULT_LOGGING, nameof(ChunkGenerationSystem),
                $"Meshed: '{chunk.ID}' ({stopwatch.Elapsed.TotalMilliseconds:0.00}ms, vertexes {pendingMesh.Vertexes.Length}, indexes {pendingMesh.Indexes.Length})"));

            DiagnosticsSystem.Stopwatches.Return(stopwatch);

            if (releaseMutex) _SingleThreadedGenerationMutex.ReleaseMutex();
        }

        private static void ApplyMesh(EntityManager entityManager, IEntity entity, Guid chunkID, PendingMesh<PackedVertex> pendingMesh)
        {
            Stopwatch stopwatch = DiagnosticsSystem.Stopwatches.Rent();
            stopwatch.Restart();

            if (pendingMesh.IsEmpty) return;

            if (!entity.TryGetComponent(out RenderMesh? renderMesh)) entityManager.RegisterComponent(entity, renderMesh = new RenderMesh());

            if (renderMesh.Mesh is null or not Mesh<PackedVertex>) renderMesh.Mesh = new Mesh<PackedVertex>();

            Mesh<PackedVertex> mesh = (renderMesh.Mesh as Mesh<PackedVertex>)!;

            if (!mesh.VertexArrayObject.VertexAttributes.SequenceEqual(_DefaultAttributes))
            {
                mesh.VertexArrayObject.AllocateVertexAttributes(_DefaultAttributes);
                mesh.VertexArrayObject.CommitVertexAttributes();
            }

            mesh.VertexesBuffer.SetBufferData(pendingMesh.Vertexes, BufferDraw.DynamicDraw);
            mesh.IndexesBuffer.SetBufferData(pendingMesh.Indexes, BufferDraw.DynamicDraw);

            if (Shader.TryLoadWithCache("Resources/Shaders/PackedVertex.glsl", "Resources/Shaders/DefaultFragment.glsl", out Shader? shader))
            {
                if (entity.TryGetComponent(out Material? material))
                {
                    if (material.Shader.ID != shader.ID) material.Shader = shader;
                }
                else entityManager.RegisterComponent(entity, material = new Material(shader));

                material.Textures[0] = TextureAtlas.Instance.Blocks;

                shader.TrySetUniform(Shader.RESERVED_UNIFORM_NAME_INT_COMPONENT_SHIFT, GenerationConstants.CHUNK_SIZE_SHIFT);
                shader.TrySetUniform(Shader.RESERVED_UNIFORM_NAME_INT_COMPONENT_MASK, GenerationConstants.CHUNK_SIZE_MASK);
                shader.TrySetUniform(Shader.RESERVED_UNIFORM_NAME_INT_NORMAL_SHIFT, 2);
            }
            else Log.Error($"Failed to load a shader for chunk at {entity.GetComponent<Translation>().Value}.");

            stopwatch.Stop();

            DiagnosticsProvider.CommitData<ChunkGenerationDiagnosticGroup, TimeSpan>(new ApplyMeshTime(stopwatch.Elapsed));
            DiagnosticsSystem.Stopwatches.Return(stopwatch);

            Log.Verbose(string.Format(FormatHelper.DEFAULT_LOGGING, nameof(ChunkGenerationSystem),
                $"Applied mesh: '{chunkID}' ({stopwatch.Elapsed.TotalMilliseconds:0.00}ms)"));
        }

        private void DiagnosticsInputCheck()
        {
            if (!InputManager.Instance.IsKeyPressed(Key.ShiftLeft) || !InputManager.Instance.IsKeyPressed(Key.B))
            {
                _KeysPressed = false;
                return;
            }
            else if (_KeysPressed) return;

            _KeysPressed = true;

            Log.Information(string.Format(FormatHelper.DEFAULT_LOGGING, nameof(DiagnosticsSystem),
                $"Average generation times: {DiagnosticsProvider.GetGroup<ChunkGenerationDiagnosticGroup>()}"));
        }
    }
}
