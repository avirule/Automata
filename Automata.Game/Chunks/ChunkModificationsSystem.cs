using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Automata.Engine;
using Automata.Engine.Diagnostics;
using Automata.Engine.Entities;
using Automata.Engine.Input;
using Automata.Engine.Systems;
using Automata.Game.Blocks;
using DiagnosticsProviderNS;
using Serilog;
using Silk.NET.Input.Common;

namespace Automata.Game.Chunks
{
    public class ChunkModificationsSystem : ComponentSystem
    {
        public override void Registered(EntityManager entityManager)
        {
            DiagnosticsProvider.EnableGroup<ChunkModificationsDiagnosticGroup>();

            InputManager.Instance.InputActions.Add(new InputAction(() => Log.Debug(string.Format(FormatHelper.DEFAULT_LOGGING, nameof(ChunkModificationsSystem),
                $"Average update time: {DiagnosticsProvider.GetGroup<ChunkModificationsDiagnosticGroup>().Average():0.00}ms")), Key.ShiftLeft, Key.C));
        }

        [HandledComponents(DistinctionStrategy.All, typeof(Chunk))]
        public override ValueTask Update(EntityManager entityManager, TimeSpan delta)
        {
            Stopwatch stopwatch = DiagnosticsPool.Stopwatches.Rent();
            stopwatch.Restart();

            foreach (Chunk chunk in entityManager.GetComponents<Chunk>())
                if (chunk.State is GenerationState.AwaitingMesh or GenerationState.Finished
                    && chunk.Neighbors.All(neighbor => neighbor?.State is <= GenerationState.AwaitingMesh or GenerationState.Finished)
                    && TryProcessChunkModifications(chunk))
                    chunk.RemeshNeighborhood(true);

            DiagnosticsProvider.CommitData<ChunkModificationsDiagnosticGroup, TimeSpan>(new ChunkModificationTime(stopwatch.Elapsed));
            DiagnosticsPool.Stopwatches.Return(stopwatch);

            return ValueTask.CompletedTask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryProcessChunkModifications(Chunk chunk)
        {
            if (chunk.Blocks is null) ThrowHelper.ThrowNullReferenceException("Chunk must have blocks.");

            bool modified = false;

            while (chunk.Modifications.TryTake(out ChunkModification? modification) && (chunk.Blocks![modification!.BlockIndex].ID != modification.BlockID))
            {
                chunk.Blocks[modification.BlockIndex] = new Block(modification.BlockID);
                modified = true;
            }

            return modified;
        }
    }
}
