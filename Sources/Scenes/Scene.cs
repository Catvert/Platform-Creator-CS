using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.ViewportAdapters;
using Platform_Creator_CS.Utility;

namespace Platform_Creator_CS.Scenes {
    public abstract class Scene : IRenderable, IUpdeatable, IDisposable {
        public Camera2D Camera { get; } = new Camera2D(new BoxingViewportAdapter(PCGame.GameWindow, PCGame.Graphics.GraphicsDevice, Constants.ViewportRatioWidth, Constants.ViewportRatioHeight));
        protected Background Background { get; set; }

        protected Scene(Background background) {
            Background = background;
        }

        public virtual void Render(SpriteBatch batch) {
            batch.Begin();

            Background?.Render(batch);

            batch.End();

        }
        public virtual void Update(GameTime gameTime) {}

        public virtual void Dispose() {

        }
    }
}