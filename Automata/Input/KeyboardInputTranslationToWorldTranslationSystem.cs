#region

using Automata.Core;
using Automata.Core.Components;
using Automata.Core.Systems;

#endregion

namespace Automata.Input
{
    public class KeyboardInputTranslationToWorldTranslationSystem : ComponentSystem
    {
        public KeyboardInputTranslationToWorldTranslationSystem()
        {
            HandledComponentTypes = new[]
            {
                typeof(KeyboardInputTranslation),
                typeof(Translation)
            };
        }

        public override void Update(EntityManager entityManager, float deltaTime)
        {
            foreach ((KeyboardInputTranslation inputTranslation, Translation translation) in entityManager
                .GetComponents<KeyboardInputTranslation, Translation>())
            {
                translation.Value += inputTranslation.Value * deltaTime;
            }
        }
    }
}
