using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Platform_Creator_CS.Entities.Containers;

namespace Platform_Creator_CS.Scenes {
    public sealed class GameScene : Scene {
        public GameScene(Level level) : base(level.Background) {
            EntityContainer = level;
        }

        protected override EntityContainer EntityContainer { get; }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            var kState = Keyboard.GetState();

            if (kState.IsKeyDown(Keys.Escape)) PCGame.SceneManager.LoadScene(new MainMenuScene(), true, true);

            UpdateCamera();
        }

        private void UpdateCamera() {
            var kState = Keyboard.GetState();

            if (kState.IsKeyDown(Keys.P)) Camera.ZoomOut(0.01f);
            if (kState.IsKeyDown(Keys.M)) Camera.ZoomOut(0.01f);

            ((Level) EntityContainer).UpdateCamera(Camera, true);
        }
    }
}