using Microsoft.Xna.Framework.Graphics;

namespace Platform_Creator_CS.Utility {
    public interface IRenderable {
        void Render(SpriteBatch batch, float alpha);
    }
}