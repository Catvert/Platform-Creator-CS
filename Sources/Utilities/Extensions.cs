using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platform_Creator_CS.Utility {
    public static class Extensions {
        public static void Draw(this SpriteBatch batch, TextureRegion frame, Rectangle destinationRect, Color color) {
            batch.Draw(frame.Texture, destinationRect, frame.SourceRect, color, 0f, Vector2.Zero, SpriteEffects.None,
                0f);
        }

        public static bool EqualsEpsilon(this float v, float compare, float epsilon) {
            return Math.Abs(v - compare) < epsilon;
        }

        public static int Round(this float v) => (int)Math.Round(v);

        public static Point ToPCPoint(this Vector2 v) {
            return new Point(v.X, v.Y);
        }

        public delegate void RefAction<T>(ref T value);

        public static T Apply<T>(this T value, RefAction<T> action) {
            action(ref value);
            return value;
        }
    }
}