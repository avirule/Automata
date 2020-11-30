using System.Diagnostics;
using Automata.Engine.Numerics;
using Xunit;

namespace Automata.Engine.Tests.Numerics.Vector2_Types
{
    public class Float
    {
        private static readonly Vector2<float> _A = new Vector2<float>(0, 10);
        private static readonly Vector2<float> _B = new Vector2<float>(0, 0);

        [Fact]
        public void AddOperator()
        {
            Vector2<float> result = _A + _B;

            Debug.Assert(result.X is 0);
            Debug.Assert(result.Y is 10);
        }

        [Fact]
        public void SubtractOperator()
        {
            Vector2<float> result = _A - _B;

            Debug.Assert(result.X is 0);
            Debug.Assert(result.Y is 10);
        }

        [Fact]
        public void MultiplyOperator()
        {
            Vector2<float> result = _A * _B;

            Debug.Assert(result.X is 0);
            Debug.Assert(result.Y is 0);
        }

        [Fact]
        public void DivideOperator()
        {
            Vector2<float> result = _A / _B;

            Debug.Assert(result.X is float.NaN);
            Debug.Assert(result.Y is float.PositiveInfinity);
        }

        [Fact]
        public void AbsOperator()
        {
            Vector2<float> result = Vector2<float>.Abs(new Vector2<float>(-0.5f));

            Debug.Assert(result.X is 0.5f);
            Debug.Assert(result.Y is 0.5f);
        }

        [Fact]
        public void FloorOperator()
        {
            Vector2<float> result = Vector2<float>.Ceiling(new Vector2<float>(-0.5f));

            Debug.Assert(result.X is 0);
            Debug.Assert(result.Y is 0);
        }

        [Fact]
        public void CeilingOperator()
        {
            Vector2<float> result = Vector2<float>.Floor(new Vector2<float>(-0.5f));

            Debug.Assert(result.X is -1);
            Debug.Assert(result.Y is -1);
        }

        [Fact]
        public void EqualsOperator()
        {
            Vector2<bool> result = _A == _B;

            Debug.Assert(result.X is true);
            Debug.Assert(result.Y is false);
        }

        [Fact]
        public void NotEqualOperator()
        {
            Vector2<bool> result = _A != _B;

            Debug.Assert(result.X is false);
            Debug.Assert(result.Y is true);
        }

        [Fact]
        public void GreaterThanOperator()
        {
            Vector2<bool> result = _A > _B;

            Debug.Assert(result.X is false);
            Debug.Assert(result.Y is true);
        }

        [Fact]
        public void LessThanOperator()
        {
            Vector2<bool> result = _A < _B;

            Debug.Assert(result.X is false);
            Debug.Assert(result.Y is false);
        }

        [Fact]
        public void GreaterThanOrEqualOperator()
        {
            Vector2<bool> result = _A >= _B;

            Debug.Assert(result.X is true);
            Debug.Assert(result.Y is true);
        }

        [Fact]
        public void LessThanOrEqualOperator()
        {
            Vector2<bool> result = _A <= _B;

            Debug.Assert(result.X is true);
            Debug.Assert(result.Y is false);
        }
    }
}
