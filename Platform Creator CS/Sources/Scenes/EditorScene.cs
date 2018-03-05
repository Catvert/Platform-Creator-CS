using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Platform_Creator_CS.Sources.Entities.Containers;
using Platform_Creator_CS.Sources.Managers;

namespace Platform_Creator_CS.Sources.Scenes {
    public class EditorScene : Scene {
        public EditorScene(Level level, bool applyMusicTransition) : base(level.Background) {
            EntityContainer = level;
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                var level = EntityContainer as Level;
                SerializationFactory.SerializeToFile(level, level.LevelPath);
                PCGame.SceneManager.LoadScene(new MainMenuScene(), true, true);
            }
        }

        public class UI {

        }

        protected override EntityContainer EntityContainer { get; }
    }
}