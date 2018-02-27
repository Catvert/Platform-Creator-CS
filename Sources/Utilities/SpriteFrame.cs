using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platform_Creator_CS.Utility {
    public class SpriteFrame {
        public Texture2D Texture { get; private set; }
        public Rectangle SourceRect { get; private set; }
        public Vector2 Origin { get; private set; }
        public SpriteFrame(Texture2D texture, Rectangle sourceRect, Vector2 origin) {
            Texture = texture;
            SourceRect = sourceRect;
            Origin = origin;
        }
    }
}