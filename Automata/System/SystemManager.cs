#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Automata.Entity;

// ReSharper disable MemberCanBePrivate.Global

#endregion

namespace Automata.System
{
    public enum SystemRegistrationOrder
    {
        Before,
        After
    }

    public class FirstOrderSystem : ComponentSystem { }

    public class DefaultOrderSystem : ComponentSystem { }

    public class RenderOrderSystem : ComponentSystem { }

    public class LastOrderSystem : ComponentSystem { }

    public class SystemManager : IDisposable
    {
        private readonly LinkedList<ComponentSystem> _ComponentSystems;
        private readonly Dictionary<Type, LinkedListNode<ComponentSystem>> _ComponentSystemNodes;

        public SystemManager()
        {
            _ComponentSystems = new LinkedList<ComponentSystem>();
            _ComponentSystemNodes = new Dictionary<Type, LinkedListNode<ComponentSystem>>
            {
                // initialize first system
                { typeof(FirstOrderSystem), _ComponentSystems.AddLast(Activator.CreateInstance<FirstOrderSystem>()) },
                { typeof(DefaultOrderSystem), _ComponentSystems.AddLast(Activator.CreateInstance<DefaultOrderSystem>()) },
                { typeof(RenderOrderSystem), _ComponentSystems.AddLast(Activator.CreateInstance<RenderOrderSystem>()) },
                { typeof(LastOrderSystem), _ComponentSystems.AddLast(Activator.CreateInstance<LastOrderSystem>()) }
            };
        }

        public void Update(EntityManager entityManager, Stopwatch frameTimer)
        {
            foreach (ComponentSystem componentSystem in _ComponentSystems.Where(componentSystem =>
                componentSystem.Enabled && VerifyHandledTypesExist(entityManager, componentSystem)))
            {
                componentSystem.Update(entityManager, frameTimer.Elapsed);
            }
        }

        /// <summary>
        ///     Registers a new system of type <see cref="TSystem" />.
        /// </summary>
        /// <typeparam name="TSystem"><see cref="ComponentSystem" /> type to instantiate.</typeparam>
        /// <typeparam name="TUpdateAround"><see cref="ComponentSystem" /> type to update system after.</typeparam>
        /// <exception cref="Exception">
        ///     Thrown when system type <see cref="TSystem" /> has already been instantiated.
        /// </exception>
        /// <exception cref="TypeLoadException">
        ///     Thrown when system of type <see cref="TUpdateAround" /> doesn't exist.
        /// </exception>
        public void RegisterSystem<TSystem, TUpdateAround>(SystemRegistrationOrder registrationOrder)
            where TSystem : ComponentSystem
            where TUpdateAround : ComponentSystem
        {
            if (_ComponentSystemNodes.ContainsKey(typeof(TSystem)))
            {
                throw new Exception("System type already instantiated.");
            }
            else if (!_ComponentSystemNodes.ContainsKey(typeof(TUpdateAround)))
            {
                throw new KeyNotFoundException("System type does not exist.");
            }

            TSystem componentSystem = Activator.CreateInstance<TSystem>();

            foreach (Type type in componentSystem.HandledComponents?.Types ?? Enumerable.Empty<Type>())
            {
                if (!typeof(IComponent).IsAssignableFrom(type))
                {
                    throw new TypeLoadException($"Type '{type}' does not inherit '{nameof(IComponent)}'.");
                }
            }

            switch (registrationOrder)
            {
                case SystemRegistrationOrder.Before:
                    _ComponentSystemNodes.Add(typeof(TSystem),
                        _ComponentSystems.AddBefore(_ComponentSystemNodes[typeof(TUpdateAround)], componentSystem));
                    break;
                case SystemRegistrationOrder.After:
                    _ComponentSystemNodes.Add(typeof(TSystem),
                        _ComponentSystems.AddAfter(_ComponentSystemNodes[typeof(TUpdateAround)], componentSystem));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(registrationOrder), registrationOrder, null);
            }

            componentSystem.Registered();
        }

        /// <summary>
        ///     Returns instantiated system of type <see cref="T" />, if any.
        /// </summary>
        /// <typeparam name="T"><see cref="ComponentSystem" /> <see cref="Type" /> to return instance of.</typeparam>
        /// <returns>Instantiated <see cref="ComponentSystem" /> of type <see cref="T" />, if any.</returns>
        /// <exception cref="KeyNotFoundException">
        ///     <see cref="ComponentSystem" /> of given type <see cref="T" /> has not been instantiated.
        /// </exception>
        public T GetSystem<T>() where T : ComponentSystem
        {
            if (!_ComponentSystemNodes.ContainsKey(typeof(T)))
            {
                throw new KeyNotFoundException("System type has not been instantiated.");
            }

            return (T)_ComponentSystemNodes[typeof(T)].Value;
        }

        #region Helper Methods

        private static bool VerifyHandledTypesExist(EntityManager entityManager, ComponentSystem componentSystem) =>
            componentSystem.HandledComponents is null
            || (componentSystem.HandledComponents.Types.Count == 0)
            || componentSystem.HandledComponents.Types.Any(type => entityManager.GetComponentCount(type) > 0);

        #endregion

        #region IDisposable

        private bool _Disposed;

        protected virtual void DisposeInternal()
        {
            foreach (ComponentSystem componentSystem in _ComponentSystems)
            {
                componentSystem.Dispose();
            }
        }

        public void Dispose()
        {
            if (_Disposed)
            {
                return;
            }

            DisposeInternal();
            GC.SuppressFinalize(this);
            _Disposed = true;
        }

        #endregion
    }
}