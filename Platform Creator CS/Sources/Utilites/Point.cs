using Microsoft.Xna.Framework;

namespace Platform_Creator_CS.Utility {
    public struct Point {
        public float X;
        public float Y;

        public Point(float x, float y) {
            X = x;
            Y = y;
        }

        public Vector2 ToVector2() {
            return new Vector2(X, Y);
        }

        public bool EqualsEpsilon(Point compare, float epsilon) {
            return X.EqualsEpsilon(compare.X, epsilon) && Y.EqualsEpsilon(compare.Y, epsilon);
        }
    }
}