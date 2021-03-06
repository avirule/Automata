using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace Automata.Engine
{
    public class World : IDisposable
    {
        public EntityManager EntityManager { get; }
        public SystemManager SystemManager { get; }
        public bool Active { get; set; }

        static World() => Worlds = new Dictionary<string, World>();

        protected World(bool active)
        {
            SystemManager = new SystemManager(this);
            EntityManager = new EntityManager();

            Active = active;
        }

        protected virtual async ValueTask UpdateAsync(TimeSpan deltaTime) => await SystemManager.UpdateAsync(EntityManager, deltaTime);


        #region Static

        private static Dictionary<string, World> Worlds { get; }

        public static void RegisterWorld(string name, World world)
        {
            if (!Worlds.TryAdd(name, world))
            {
                throw new ArgumentException(name);
            }

            Log.Information($"({nameof(World)}) Registered {nameof(World)}: '{name}' {world.GetType()}");
        }

        public static bool TryGetWorld(string name, [NotNullWhen(true)] out World? world) => Worlds.TryGetValue(name, out world);

        public static async ValueTask GlobalUpdateAsync(TimeSpan deltaTime)
        {
            foreach (World world in Worlds.Values.Where(world => world.Active))
            {
                await world.UpdateAsync(deltaTime);
            }
        }

        public static void DisposeWorlds()
        {
            foreach ((_, World world) in Worlds)
            {
                world.Dispose();
            }
        }

        #endregion


        #region IDisposable

        private bool _Disposed;

        protected virtual void SafeDispose()
        {
            EntityManager.Dispose();
            SystemManager.Dispose();
        }

        public void Dispose()
        {
            if (_Disposed)
            {
                return;
            }

            SafeDispose();
            GC.SuppressFinalize(this);
            _Disposed = true;
        }

        #endregion
    }
}
