using Microsoft.Xna.Framework;

namespace Platform_Creator_CS.Utility {
    public struct Point {
        public float X;
        public float Y;

        public Point(float x, float y) {
            X = x;
            Y = y;
        }

        public Vector2 ToVector2() => new Vector2(X, Y);
    }
}