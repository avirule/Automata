using System.Numerics;
using Automata.Engine.Components;

namespace Automata.Engine.Rendering
{
    public class RenderModel : ComponentChangeable
    {
        private Matrix4x4 _Model;

        public Matrix4x4 Model
        {
            get => _Model;
            set
            {
                _Model = value;
                Changed = true;
            }
        }
    }
}
