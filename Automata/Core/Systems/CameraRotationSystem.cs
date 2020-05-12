#region

using System.Numerics;
using Automata.Core.Components;

#endregion

namespace Automata.Core.Systems
{
    public class CameraRotationSystem : ComponentSystem
    {
        private Vector2 _LastFrameMouseOffset;

        public CameraRotationSystem()
        {
            _LastFrameMouseOffset = Vector2.Zero;

            HandledComponentTypes = new[]
            {
                typeof(Camera),
                typeof(Rotation)
            };
        }

        public override void Update(EntityManager entityManager, float deltaTime)
        {
            foreach ((Camera _, Rotation rotation) in entityManager.GetComponents<Camera, Rotation>())
            {
                Vector2 offset = InputSingleton.Instance.ViewCenter - InputSingleton.Instance.GetMousePosition(0);

                if (offset == _LastFrameMouseOffset)
                {
                    continue;
                }

                _LastFrameMouseOffset = offset;

                Quaternion axisAngleQuaternion = Quaternion.CreateFromAxisAngle(new Vector3(offset, 0f), deltaTime);
                Quaternion finalRotationPosition = Quaternion.Add(rotation.Value, axisAngleQuaternion);

                rotation.Value = Quaternion.Slerp(rotation.Value, finalRotationPosition, deltaTime);
            }
        }
    }
}