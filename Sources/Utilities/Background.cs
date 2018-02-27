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
        public BackgroundType Type { get; }

        protected Background(BackgroundType type) {
            Type = type;
        }

        public abstract void Render(SpriteBatch batch);
    }

    public class StandardBackground : Background {
        public string BackgroundFile { get;}
        private Texture2D _backgroundTexture;
        public StandardBackground(string file) : base(BackgroundType.Standard) {
            BackgroundFile = file;
            _backgroundTexture = ResourceManager.GetTexture(file);
         }

        public override void Render(SpriteBatch batch) {
            batch.Draw(_backgroundTexture, new Rectangle(0, 0, PCGame.ScreenSize.X, PCGame.ScreenSize.Y), null, Color.White);
        }
    }

    public class ParallaxBackground : Background {
        public ParallaxBackground() : base(BackgroundType.Parallax) { }

        public override void Render(SpriteBatch batch)
        {
            throw new System.NotImplementedException();
        }
    }
}