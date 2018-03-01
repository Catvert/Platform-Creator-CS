using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platform_Creator_CS.Utility {
    public static class Extensions {
        public static void Draw (this SpriteBatch batch, SpriteFrame frame, Rectangle destinationRect, Color color) {
            batch.Draw (frame.Texture, destinationRect, frame.SourceRect, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }
        
    }
}