using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ImGuiNET;
using Platform_Creator_CS.Utility;

using Vec2 = System.Numerics.Vector2;

namespace Platform_Creator_CS.Scenes {
    public class MainMenuScene : Scene {
        public MainMenuScene() : base(PCGame.MainBackground) {}

        public override void Render(SpriteBatch batch) {
            base.Render(batch);

            DrawMainMenu();
        }

        private void DrawMainMenu() {
            ImGuiHelper.WithMenuWindow(new Vec2(300, 180), () => {
                ImGui.Button("Jouer !", new Vec2(-1, 0));
                ImGui.Button("Options", new Vec2(-1, 0));
                if(ImGui.Button("Quitter", new Vec2(-1, 0)))
                    PCGame.Exit = true;
            });
        }
    }
}