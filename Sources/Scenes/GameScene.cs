using Microsoft.Xna.Framework.Input;
using Platform_Creator_CS.Utility;

namespace Platform_Creator_CS.Scenes {
    public class GameScene : Scene {
        public GameScene() : base(new StandardBackground(Constants.BackgroundsDir + "standard/1.png")) { }
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime) {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                PCGame.SceneManager.LoadScene(new MainMenuScene(), true, true);
            }
        }
        public override void Render(Microsoft.Xna.Framework.Graphics.SpriteBatch batch, float alpha) {
            base.Render(batch, alpha);
        }
    }
}