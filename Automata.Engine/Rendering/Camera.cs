using System.Numerics;

namespace Automata.Engine.Rendering
{
    public class Camera : Component
    {
        public Matrix4x4 View { get; set; } = Matrix4x4.Identity;
        public Layer RenderedLayers { get; set; } = Layer.All;
        public Projector Projector { get; set; }
        public IProjection? Projection { get; set; }
    }
}
