using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Automata.Engine.Collections;
using Automata.Engine.Diagnostics;
using Automata.Engine.Rendering.OpenGL;
using Automata.Engine.Rendering.OpenGL.Buffers;
using Automata.Engine.Rendering.OpenGL.Shaders;
using Automata.Engine.Rendering.OpenGL.Textures;
using Serilog;

namespace Automata.Engine.Rendering.Meshes
{
    public class AllocatedMeshingSystem<TIndex, TVertex> : ComponentSystem
        where TIndex : unmanaged, IEquatable<TIndex>
        where TVertex : unmanaged, IEquatable<TVertex>
    {
        private MultiDrawIndirectMesh<TIndex, TVertex>? _MultiDrawIndirectMesh;
        private Material? _MultiDrawIndirectMeshMaterial;

        public void SetTexture(string key, Texture texture)
        {
            if (_MultiDrawIndirectMeshMaterial is null) ThrowHelper.ThrowNullReferenceException(nameof(_MultiDrawIndirectMeshMaterial));

            if (!_MultiDrawIndirectMeshMaterial!.Textures.ContainsKey(key)) _MultiDrawIndirectMeshMaterial.Textures.Add(key, texture);
            else _MultiDrawIndirectMeshMaterial.Textures[key] = texture;
        }

        public void AllocateVertexAttributes(bool replace, bool finalize, params IVertexAttribute[] attributes)
        {
            if (_MultiDrawIndirectMesh is null) ThrowHelper.ThrowNullReferenceException(nameof(_MultiDrawIndirectMesh));

            _MultiDrawIndirectMesh!.AllocateVertexAttributes(replace, attributes);
            if (finalize) _MultiDrawIndirectMesh!.FinalizeVertexArrayObject();
        }


        #region ComponentSystem

        public override void Registered(EntityManager entityManager)
        {
            _MultiDrawIndirectMeshMaterial =
                new Material(ProgramRegistry.Instance.Load("Resources/Shaders/PackedVertex.glsl", "Resources/Shaders/DefaultFragment.glsl"));

            _MultiDrawIndirectMesh = new MultiDrawIndirectMesh<TIndex, TVertex>(GLAPI.Instance.GL, 750_000_000, 500_000_000);

            entityManager.CreateEntity(
                _MultiDrawIndirectMeshMaterial,
                new RenderMesh
                {
                    Mesh = _MultiDrawIndirectMesh
                });
        }

        public override ValueTask Update(EntityManager entityManager, TimeSpan delta)
        {
            bool recreateCommandBuffer = false;

            foreach ((IEntity entity, AllocatedMeshData<TIndex, TVertex> mesh) in entityManager.GetEntitiesWithComponents<AllocatedMeshData<TIndex, TVertex>>())
            {
                ProcessMeshData(entityManager, entity, mesh.Data);
                entityManager.RemoveComponent<AllocatedMeshData<TIndex, TVertex>>(entity);
                recreateCommandBuffer = true;
            }

            if (recreateCommandBuffer)
            {
                using NonAllocatingList<DrawIndirectAllocation<TIndex, TVertex>> allocations = new NonAllocatingList<DrawIndirectAllocation<TIndex, TVertex>>();
                using NonAllocatingList<Matrix4x4> models = new NonAllocatingList<Matrix4x4>();

                foreach ((IEntity entity, DrawIndirectAllocation<TIndex, TVertex> allocation) in
                    entityManager.GetEntitiesWithComponents<DrawIndirectAllocation<TIndex, TVertex>>())
                {
                    allocations.Add(allocation);
                    models.Add(entity.Find<RenderModel>()?.Model ?? Matrix4x4.Identity);
                }

                GenerateDrawElementsIndirectCommands(allocations.Segment);
                _MultiDrawIndirectMesh!.SetSSBOModelsData(models.Segment);
            }

            return ValueTask.CompletedTask;
        }

        #endregion


        #region Data Processing

        private unsafe void GenerateDrawElementsIndirectCommands(Span<DrawIndirectAllocation<TIndex, TVertex>> allocations)
        {
            if (_MultiDrawIndirectMesh is null) ThrowHelper.ThrowNullReferenceException(nameof(_MultiDrawIndirectMesh));

            Span<DrawElementsIndirectCommand> commands = stackalloc DrawElementsIndirectCommand[allocations.Length];

            for (uint index = 0; index < allocations.Length; index++)
            {
                DrawIndirectAllocation<TIndex, TVertex> drawIndirectAllocation = allocations[(int)index];
                if (drawIndirectAllocation.Allocation is null) throw new NullReferenceException("Allocation should not be null at this point.");

                nuint indexesStart = drawIndirectAllocation.Allocation.IndexesArrayMemory.Index / (nuint)sizeof(TIndex);
                nuint vertexesStart = drawIndirectAllocation.Allocation.VertexArrayMemory.Index;

                commands[(int)index] = new DrawElementsIndirectCommand(drawIndirectAllocation.Allocation.VertexArrayMemory.Count, 1u,
                    (uint)indexesStart, (uint)vertexesStart, index);
            }

            _MultiDrawIndirectMesh!.AllocateDrawElementsIndirectCommands(commands);

            Log.Verbose(string.Format(FormatHelper.DEFAULT_LOGGING, nameof(AllocatedMeshingSystem<TIndex, TVertex>),
                $"Allocated {commands.Length} {nameof(DrawElementsIndirectCommand)}"));
        }

        private void ProcessMeshData(EntityManager entityManager, IEntity entity, NonAllocatingQuadsMeshData<TIndex, TVertex> pendingData)
        {
            Stopwatch stopwatch = DiagnosticsPool.Stopwatches.Rent();
            stopwatch.Restart();

            if (ApplyMeshMultiDraw(entityManager, entity, pendingData)) ConfigureMaterial(entityManager, entity);

            DiagnosticsPool.Stopwatches.Return(stopwatch);
        }

        private unsafe bool ApplyMeshMultiDraw(EntityManager entityManager, IEntity entity, NonAllocatingQuadsMeshData<TIndex, TVertex> pendingData)
        {
            if (_MultiDrawIndirectMesh is null) throw new NullReferenceException("Mesh is null!");
            else if (pendingData.IsEmpty) return false;

            if (!entity.TryFind(out DrawIndirectAllocation<TIndex, TVertex>? drawIndirectAllocation))
                drawIndirectAllocation = entityManager.RegisterComponent<DrawIndirectAllocation<TIndex, TVertex>>(entity);

            drawIndirectAllocation.Allocation?.Dispose();
            _MultiDrawIndirectMesh.WaitForBufferFreeSync();

            // create index buffer array memory
            int indexCount = pendingData.Indexes.Count * 6;
            BufferArrayMemory indexArrayMemory = _MultiDrawIndirectMesh.RentIndexBufferArrayMemory((nuint)sizeof(TIndex), (nuint)(indexCount * sizeof(TIndex)));
            MemoryMarshal.AsBytes(pendingData.Indexes.Segment).CopyTo(indexArrayMemory.MemoryOwner.Memory.Span);

            // create vertex buffer array memory
            int vertexCount = pendingData.Vertexes.Count * 4;
            BufferArrayMemory vertexArrayMemory = _MultiDrawIndirectMesh.RentVertexBufferArrayMemory(0u, (nuint)(vertexCount * sizeof(TVertex)));
            MemoryMarshal.AsBytes(pendingData.Vertexes.Segment).CopyTo(vertexArrayMemory.MemoryOwner.Memory.Span);

            drawIndirectAllocation.Allocation = new AllocationWrapper(indexArrayMemory, vertexArrayMemory);

            Log.Verbose(string.Format(FormatHelper.DEFAULT_LOGGING, nameof(AllocatedMeshingSystem<TIndex, TVertex>),
                $"Allocated new {nameof(DrawIndirectAllocation<TIndex, TVertex>)}: indexes ({indexCount}, {indexArrayMemory.MemoryOwner.Memory.Length} bytes), vertexes ({vertexCount}, {vertexArrayMemory.MemoryOwner.Memory.Length} bytes)"));

            return true;
        }

        private static void ConfigureMaterial(EntityManager entityManager, IEntity entity)
        {
            ProgramPipeline programPipeline = ProgramRegistry.Instance.Load("Resources/Shaders/PackedVertex.glsl", "Resources/Shaders/DefaultFragment.glsl");

            if (entity.TryFind(out Material? material))
            {
                if (material.Pipeline.Handle != programPipeline.Handle) material.Pipeline = programPipeline;
            }
            else entityManager.RegisterComponent(entity, new Material(programPipeline));
        }

        #endregion
    }
}
