using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platform_Creator_CS.Utility {
    public class TextureRegion {
        public TextureRegion(Texture2D texture, Rectangle sourceRect) {
            Texture = texture;
            SourceRect = sourceRect;
        }

        public TextureRegion(Texture2D texture) {
            Texture = texture;
            SourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
        }

        public Texture2D Texture { get; }
        public Rectangle SourceRect { get; }
    }
}