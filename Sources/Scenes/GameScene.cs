using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Platform_Creator_CS.Entities.Actions;
using Platform_Creator_CS.Entities.Components;
using Platform_Creator_CS.Managers;
using Platform_Creator_CS.Utility;

namespace Platform_Creator_CS.Scenes {
    public class GameScene : Scene {
        private Entities.Entity entity;

        public GameScene() : base(new StandardBackground(Constants.BackgroundsDir + "standard/1.png")) {
            var sheet = SpriteSheetLoader.Load(Constants.PacksSMCDir + "blocks.atlas");

            var state = new Entities.EntityState("default");
            state.AddComponent(new TextureComponent(sheet.GetFrame("blocks/metal/Metal Blue")));
            
            state.AddComponent(new InputComponent(
                new InputData(Keys.Q, true, new MoveAction(-10, 0)),
                new InputData(Keys.D, true, new MoveAction(10, 0)),
                new InputData(Keys.Z, true, new MoveAction(0, -10)),
                new InputData(Keys.S, true, new MoveAction(0, 10))
            ));

            entity = EntityContainer.AddEntity(new Entities.Entity("test", "hi", new Rect(100, 100, 500, 500), state));
        }
        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            var kState = Keyboard.GetState();

            if (kState.IsKeyDown(Keys.Escape)) {
                PCGame.SceneManager.LoadScene(new MainMenuScene(), true, true);
            }
        }
        public override void Render(SpriteBatch batch, float alpha) {
            base.Render(batch, alpha);
        }
    }
}