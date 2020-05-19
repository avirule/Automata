#region

using System;
using System.Diagnostics;

#endregion

namespace Automata.Rendering.GLFW
{
    public class ViewDoUpdateSystem : ComponentSystem
    {
        public override void Update(EntityManager entityManager, TimeSpan delta)
        {
            if (!AutomataWindow.TryValidate())
            {
                return;
            }

            Debug.Assert(AutomataWindow.Instance != null);
            Debug.Assert(AutomataWindow.Instance.Window != null);

            AutomataWindow.Instance.Window.DoEvents();
            AutomataWindow.Instance.Window.DoUpdate();
        }
    }
}