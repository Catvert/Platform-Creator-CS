using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using Platform_Creator_CS.Sources.Entities.Containers;
using Platform_Creator_CS.Sources.Utilites;
using Platform_Creator_CS.Utilities;
using Platform_Creator_CS.Utility;
using IUpdateable = Platform_Creator_CS.Utility.IUpdateable;

namespace Platform_Creator_CS.Sources.Scenes {
    public abstract class Scene : IRenderable, IUpdateable, IResizable, IDisposable {
        protected Scene(Background background) {
            Background = background;
        }

        public Camera2D Camera { get; } = new Camera2D(new ScalingViewportAdapter(
            PCGame.Graphics.GraphicsDevice, Constants.ViewportRatioWidth, Constants.ViewportRatioHeight));

        public Color BackgroundColor { get; protected set; } = Color.Black;

        protected virtual EntityContainer EntityContainer { get; } = new EntityContainer();
        protected Background Background { get; set; }

        public virtual void Dispose() { }

        public virtual void Render(SpriteBatch batch, float alpha) {
            batch.Begin(blendState: BlendState.NonPremultiplied);

            Background?.Render(batch, alpha);

            batch.End();

            batch.Begin(transformMatrix: Camera.GetViewMatrix(), blendState: BlendState.NonPremultiplied);
            EntityContainer.Render(batch, alpha);
            batch.End();
        }

        public virtual void Resize(Size newSize) { }

        public virtual void Update(GameTime gameTime) {
            EntityContainer.Update(gameTime);
        }

        protected bool IsUIHover() {
            return false; //TODO ImGui.IsAnyItemHovered() || ImGui.IsAnyItemActive();
        }
    }
}