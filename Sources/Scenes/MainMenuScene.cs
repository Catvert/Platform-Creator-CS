using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ImGuiNET;
using Platform_Creator_CS.Utility;

using Vec2 = System.Numerics.Vector2;

namespace Platform_Creator_CS.Scenes {
    public sealed class MainMenuScene : Scene {
        public MainMenuScene() : base(PCGame.MainBackground) {
            PCGame.AddLogoMenu(EntityContainer);
        }

        public override void Render(SpriteBatch batch, float alpha) {
            base.Render(batch, alpha);

            DrawMainMenu();
        }

        private void DrawMainMenu() {
            ImGuiHelper.WithMenuWindow(new Vec2(300, 180), () => {
                if(ImGui.Button("Jouer !", new Vec2(-1, 0)))
                    PCGame.SceneManager.LoadScene(new GameScene(), true, true);
                ImGui.Button("Options", new Vec2(-1, 0));
                if(ImGui.Button("Quitter", new Vec2(-1, 0)))
                    PCGame.Exit = true;
            });
        }
    }
}