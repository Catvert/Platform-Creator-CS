using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platform_Creator_CS.Utility {
    public static class Extensions {
        public static void Draw (this SpriteBatch batch, SpriteFrame frame, Rectangle destinationRect) {
            batch.Draw (frame.Texture, destinationRect, frame.SourceRect, Color.White, 0f, frame.Origin, SpriteEffects.None, 0f);
        }

    }
}