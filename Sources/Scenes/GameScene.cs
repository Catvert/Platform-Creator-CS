using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Platform_Creator_CS.Entities;
using Platform_Creator_CS.Entities.Actions;
using Platform_Creator_CS.Entities.Components;
using Platform_Creator_CS.Entities.Containers;
using Platform_Creator_CS.Managers;
using Platform_Creator_CS.Serialization;
using Platform_Creator_CS.Utility;

namespace Platform_Creator_CS.Scenes {
    public sealed class GameScene : Scene {
        protected override EntityContainer EntityContainer { get; } = new EntityMatrixContainer();

        public GameScene() : base(new StandardBackground(Constants.BackgroundsDir + "standard/1.png")) {

            var state = new Entities.EntityState("default");
            state.AddComponent(new TextureComponent(Constants.PacksSMCDir + "blocks.atlas", "blocks/metal/Metal Blue"));
            
            state.AddComponent(new InputComponent(
                new InputData(Keys.Q, true, new MoveAction(-10, 0)),
                new InputData(Keys.D, true, new MoveAction(10, 0)),
                new InputData(Keys.Z, true, new MoveAction(0, -10)),
                new InputData(Keys.S, true, new MoveAction(0, 10))
            ));

            var entity = EntityContainer.AddEntity(new Entities.Entity("test", "hi", new Rect(100, 100, 50, 50), state));

            ((EntityMatrixContainer) EntityContainer).FollowEntity = entity;

            EntityContainer = SerializationFactory.Copy<EntityMatrixContainer>((EntityMatrixContainer)EntityContainer);
            SerializationFactory.SerializeToFile(EntityContainer, Constants.AssetsDir + "data.json");
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            var kState = Keyboard.GetState();

            if (kState.IsKeyDown(Keys.Escape)) {
                PCGame.SceneManager.LoadScene(new MainMenuScene(), true, true);
            }
        }
    }
}