#region

using Automata.Rendering.GLFW;
using Silk.NET.OpenGL;

#endregion

namespace Automata.Rendering.OpenGL
{
    public class GLAPI : Singleton<GLAPI>
    {
        public GL GL { get; }

        public GLAPI()
        {
            AssignSingletonInstance(this);

            GL = GL.GetApi(AutomataWindow.Instance.GLContext);
        }
    }
}
