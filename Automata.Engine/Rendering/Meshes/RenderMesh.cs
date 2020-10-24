#region

using System;
using System.Numerics;
using Automata.Engine.Components;

#endregion


namespace Automata.Engine.Rendering.Meshes
{
    public class RenderMesh : IComponentChangeable, IDisposable
    {
        private IMesh _Mesh;

        public Guid MeshID { get; } = Guid.NewGuid();
        public Matrix4x4 Model { get; set; } = Matrix4x4.Identity;

        public IMesh Mesh
        {
            get => _Mesh;
            set
            {
                _Mesh = value;
                Changed = true;
            }
        }

        public bool Changed { get; set; }

        public bool ShouldRender => Mesh.Visible && Mesh.IndexesLength > 0;

        public RenderMesh(IMesh mesh) => Mesh = mesh;

        public void Dispose() => _Mesh.Dispose();
    }
}
