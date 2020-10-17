#region

using Automata.Engine.Components;

#endregion

namespace Automata.Engine.Rendering.OpenGL
{
    public class RenderShader : IComponent
    {
        private static readonly Shader _DefaultShader = new Shader();

        public Shader Value { get; set; } = _DefaultShader;
    }
}