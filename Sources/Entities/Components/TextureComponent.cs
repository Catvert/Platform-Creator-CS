using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platform_Creator_CS.Utility;

namespace Platform_Creator_CS.Entities.Components {
    public class TextureComponent : Component, IRenderable
    {
        private SpriteFrame frame;

        public TextureComponent(SpriteFrame frame) {
            this.frame = frame;
        }
        public void Render(SpriteBatch batch, float alpha) {
            batch.Draw(frame, Entity.Box.ToRectangle(), new Color(255, 255, 255, alpha));
        }
    }
}