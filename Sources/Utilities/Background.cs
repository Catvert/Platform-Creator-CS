using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platform_Creator_CS.Managers;

namespace Platform_Creator_CS.Utility {
    public enum BackgroundType {
        Standard,
        Parallax,
        None
    }

    public abstract class Background : IRenderable {
        protected Background(BackgroundType type) {
            Type = type;
        }

        public BackgroundType Type { get; }

        public abstract void Render(SpriteBatch batch, float alpha);
    }

    public class StandardBackground : Background {
        private readonly Texture2D _backgroundTexture;

        public StandardBackground(string file) : base(BackgroundType.Standard) {
            BackgroundFile = file;
            _backgroundTexture = ResourceManager.GetTexture(file);
        }

        public string BackgroundFile { get; }

        public override void Render(SpriteBatch batch, float alpha) {
            batch.Draw(_backgroundTexture, new Rectangle(0, 0, PCGame.ScreenSize.X, PCGame.ScreenSize.Y), null,
                new Color(255, 255, 255, alpha));
        }
    }

    public class ParallaxBackground : Background {
        public ParallaxBackground() : base(BackgroundType.Parallax) { }

        public override void Render(SpriteBatch batch, float alpha) {
            throw new NotImplementedException();
        }
    }
}