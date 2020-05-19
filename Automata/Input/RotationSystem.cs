#region

using System;
using System.Numerics;
using Automata.Rendering;
using Automata.Worlds;

#endregion

namespace Automata.Input
{
    public class RotationSystem : ComponentSystem
    {
        private const float _SENSITIVITY = 10f;

        public RotationSystem()
        {
            HandledComponentTypes = new[]
            {
                typeof(Rotation)
            };
        }

        public override void Update(EntityManager entityManager, TimeSpan delta)
        {
            Vector2 offset = Input.Instance.GetMousePositionRelative();

            // if offset is zero, the mouse has not moved, so return
            if (offset == Vector2.Zero)
            {
                return;
            }

            // clamp offset values to get a normalized direction
            offset = Vector2.Clamp(Input.Instance.GetMousePositionRelative(), new Vector2(-1f), Vector2.One);
            // convert to axis angles (a la yaw/pitch/roll)
            Vector3 axisAngles = new Vector3(offset.Y, offset.X, 0f) * (float)delta.TotalSeconds * _SENSITIVITY;

            foreach ((Camera camera, Rotation rotation) in entityManager.GetComponents<Camera, Rotation>())
            {
                // accumulate angles
                camera.AccumulatedAngles += axisAngles;

                // create quaternions based on local angles
                Quaternion pitch = Quaternion.CreateFromAxisAngle(Vector3.UnitX, (float)camera.AccumulatedAngles.X);
                Quaternion yaw = Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)camera.AccumulatedAngles.Y);

                // rotate around (pitch as global) and (yaw as local)
                rotation.Value = pitch * yaw;
            }

            // reset mouse position to center of screen
            Input.Instance.SetMousePositionRelative(0, Vector2.Zero);
        }
    }
}