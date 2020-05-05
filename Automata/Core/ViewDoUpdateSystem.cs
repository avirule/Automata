namespace Automata.Core
{
    public class ViewDoUpdateSystem : ComponentSystem
    {
        public ViewDoUpdateSystem()
        {
            UtilizedComponentTypes = new[]
            {
                typeof(WindowViewComponent)
            };
        }

        public override void Update()
        {
            foreach (WindowViewComponent windowViewComponent in EntityManager.GetComponents<WindowViewComponent>())
            {
                if (windowViewComponent.View.IsClosing)
                {
                    continue;
                }

                windowViewComponent.View.DoEvents();
                windowViewComponent.View.DoUpdate();
            }
        }
    }
}
