using System.Numerics;

namespace Automata.Engine
{
    public class Rotation : ComponentChangeable
    {
        private Vector2 _AccumulatedAngles = Vector2.Zero;
        private Quaternion _Value = Quaternion.Identity;

        public Quaternion Value
        {
            get => _Value;
            set
            {
                _Value = value;
                Changed = true;
            }
        }

        public Rotation() => Value = Quaternion.Identity;

        public void AccumulateAngles(Vector2 axisAngles)
        {
            _AccumulatedAngles += axisAngles;

            // create quaternions based on local angles
            Quaternion yaw = Quaternion.CreateFromAxisAngle(Vector3.UnitY, _AccumulatedAngles.X);
            Quaternion pitch = Quaternion.CreateFromAxisAngle(Vector3.UnitX, _AccumulatedAngles.Y);

            Value = yaw * pitch;
        }
    }
}
