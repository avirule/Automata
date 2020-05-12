#region

using System;
using System.Collections.Generic;
using System.Linq;
using Automata.Core.Components;

// ReSharper disable MemberCanBePrivate.Global

#endregion

namespace Automata.Core.Systems
{
    public class SystemManager
    {
        private readonly LinkedList<ComponentSystem> _ComponentSystems;
        private readonly Dictionary<Type, LinkedListNode<ComponentSystem>> _ComponentSystemNodes;

        public SystemManager()
        {
            _ComponentSystems = new LinkedList<ComponentSystem>();
            _ComponentSystemNodes = new Dictionary<Type, LinkedListNode<ComponentSystem>>
            {
                // initialize first system
                { typeof(FirstOrderSystem), _ComponentSystems.AddFirst(Activator.CreateInstance<FirstOrderSystem>()) },
                { typeof(DefaultOrderSystem), _ComponentSystems.AddFirst(Activator.CreateInstance<DefaultOrderSystem>()) },
                { typeof(RenderOrderSystem), _ComponentSystems.AddFirst(Activator.CreateInstance<RenderOrderSystem>()) },
                { typeof(LastOrderSystem), _ComponentSystems.AddFirst(Activator.CreateInstance<LastOrderSystem>()) }
            };
        }

        public void Update(EntityManager entityManager, float deltaTime)
        {
            foreach (ComponentSystem componentSystem in _ComponentSystems.Where(componentSystem =>
                VerifyHandledTypesExist(entityManager, componentSystem)))
            {
                componentSystem.Update(entityManager, deltaTime);
            }
        }

        public void Destroy(EntityManager entityManager)
        {
            foreach (ComponentSystem componentSystem in _ComponentSystems)
            {
                componentSystem.Destroy(entityManager);
            }
        }

        /// <summary>
        ///     Registers a new system of type <see cref="T" />.
        /// </summary>
        /// <typeparam name="TSystem"><see cref="ComponentSystem" /> type to instantiate.</typeparam>
        /// <typeparam name="TUpdateAfter"><see cref="ComponentSystem" /> type to update system after.</typeparam>
        /// <exception cref="Exception">
        ///     Thrown when system type <see cref="TSystem" /> has already been instantiated.
        /// </exception>
        /// <exception cref="TypeLoadException">
        ///     Thrown when system of type <see cref="TUpdateAfter" /> doesn't exist.
        /// </exception>
        public void RegisterSystem<TSystem, TUpdateAfter>()
            where TSystem : ComponentSystem
            where TUpdateAfter : ComponentSystem
        {
            if (_ComponentSystemNodes.ContainsKey(typeof(TSystem)))
            {
                throw new Exception("System type already instantiated.");
            }
            else if (!_ComponentSystemNodes.ContainsKey(typeof(TUpdateAfter)))
            {
                throw new KeyNotFoundException("System type does not exist.");
            }

            TSystem componentSystem = Activator.CreateInstance<TSystem>();

            foreach (Type type in componentSystem.HandledComponentTypes)
            {
                if (!typeof(IComponent).IsAssignableFrom(type))
                {
                    throw new TypeLoadException($"Type '{type}' does not inherit '{nameof(IComponent)}'.");
                }
            }

            LinkedListNode<ComponentSystem> updateAfterNode = _ComponentSystemNodes[typeof(TUpdateAfter)];

            _ComponentSystemNodes.Add(typeof(TSystem), _ComponentSystems.AddAfter(updateAfterNode, componentSystem));

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
            (componentSystem.HandledComponentTypes == null)
            || (componentSystem.HandledComponentTypes.Length == 0)
            || componentSystem.HandledComponentTypes.All(type => entityManager.GetComponentCount(type) > 0);

        #endregion
    }
}
