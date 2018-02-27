using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platform_Creator_CS.Utility;

namespace Platform_Creator_CS.Scenes {
    public class SceneManager : IRenderable, IUpdeatable, IDisposable
    {
        private Scene _currentScene;

        public SceneManager(Scene initialScene) {
            _currentScene = initialScene;
        }

        public void Render(SpriteBatch batch) {
            _currentScene.Render(batch);
        }

        public void Update(GameTime gameTime) {
            _currentScene.Update(gameTime);
        }

        public void Dispose() {
            _currentScene.Dispose();
        }
    }
}