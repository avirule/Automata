#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Automata.Engine.Components;

#endregion

namespace Automata.Engine.Entities
{
    public interface IEntity
    {
        Guid ID { get; }
        Dictionary<Type, IComponent>.KeyCollection ComponentTypes { get; }

        void AddComponent(IComponent component);
        void RemoveComponent<T>() where T : class, IComponent;
        void RemoveComponent(Type type);
        T GetComponent<T>() where T : class, IComponent;

        bool TryGetComponent<T>([NotNullWhen(true)] out T? component) where T : class, IComponent;
        bool TryGetComponent(Type type, [NotNullWhen(true)] out IComponent? component);
        IComponent GetComponent(Type componentType);

        int GetHashCode() => ID.GetHashCode();
    }
}