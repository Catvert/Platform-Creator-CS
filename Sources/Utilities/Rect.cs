using System;
using Microsoft.Xna.Framework;

namespace Platform_Creator_CS.Utility {
    public class Rect {
        public Point Position;
        public Size Size;

        public Rect (Point pos, Size size) {
            Position = pos;
            Size = size;
        }
        public Rect(float x, float y, int width, int height) {
            Position = new Point(x, y);
            Size = new Size(width, height);
        }
        public Rectangle ToRectangle() => new Rectangle((int)Math.Round(Position.X), (int)Math.Round(Position.Y), Size.Width, Size.Height);
    }
}