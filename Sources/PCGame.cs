using System.IO;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Platform_Creator_CS.Entities;
using Platform_Creator_CS.Entities.Components;
using Platform_Creator_CS.Entities.Containers;
using Platform_Creator_CS.Managers;
using Platform_Creator_CS.Scenes;
using Platform_Creator_CS.Utility;
using Point = Microsoft.Xna.Framework.Point;
using Vector4 = System.Numerics.Vector4;

namespace Platform_Creator_CS {
    public class PCGame : Game {
        private static bool _darkUI;

        private SpriteBatch _spriteBatch;

        public PCGame() {
            Graphics = new GraphicsDeviceManager(this);
            GameWindow = Window;

            Content.RootDirectory = "Content";
        }

        public static GraphicsDeviceManager Graphics { get; private set; }
        public static GameWindow GameWindow { get; private set; }

        public static float SoundVolume { get; set; }

        public static bool DarkUI {
            get => _darkUI;
            set {
                _darkUI = value;

                if (value)
                    ImGui.StyleColorsDark(ImGui.GetStyle());
                else
                    ImGui.StyleColorsLight(ImGui.GetStyle());


                ImGui.GetStyle().SetColor(ColorTarget.WindowBg, ImGui.GetStyle().GetColor(ColorTarget.WindowBg).Apply((ref Vector4 v) => v.W = 0.9f));
                ImGui.GetStyle().SetColor(ColorTarget.PopupBg, ImGui.GetStyle().GetColor(ColorTarget.PopupBg).Apply((ref Vector4 v) => v.W = 0.9f));
                ImGui.GetStyle().SetColor(ColorTarget.TitleBg, ImGui.GetStyle().GetColor(ColorTarget.TitleBg).Apply((ref Vector4 v) => v.W = 0.8f));
                ImGui.GetStyle().SetColor(ColorTarget.TitleBgActive, ImGui.GetStyle().GetColor(ColorTarget.TitleBgActive).Apply((ref Vector4 v) => v.W = 0.8f));
                ImGui.GetStyle().SetColor(ColorTarget.TitleBgCollapsed, ImGui.GetStyle().GetColor(ColorTarget.TitleBgCollapsed).Apply((ref Vector4 v) => v.W = 0.8f));
                ImGui.GetStyle().FrameRounding = 3f;
            }
        }

        public static SceneManager SceneManager { get; private set; }
        public static Point ScreenSize => GameWindow.ClientBounds.Size;

        public static Font ImGuiDefaultFont { get; private set; }
        public static Font ImGuiBigFont { get; private set; }

        public static Background MainBackground { get; private set; }
        public static ImGuiMg ImGuiMG { get; private set; }

        public static bool Exit { get; set; }

        public static void AddLogoMenu(EntityContainer container) {
            var logoSize = new Size(600, 125);

            container.AddEntity(new Entity("menu-logo", "menu-logo",
                new Rect(Constants.ViewportRatioWidth / 2f - logoSize.Width / 2f, logoSize.Height, logoSize.Width,
                    logoSize.Height),
                new EntityState("default",
                    new TextureComponent(new TextureComponent.TextureData("default", 0f,
                        new TextureComponent.Frame(Constants.GameLogoFile))))));
        }

        protected override void Initialize() {
            Window.Title = Constants.GameTitle;
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
            IsFixedTimeStep = true;

            LoadGameConfig();

            ImGuiDefaultFont = ImGui.GetIO().FontAtlas.AddFontFromFileTTF(Constants.ImGuiFontFile, 20f);
            ImGuiBigFont = ImGui.GetIO().FontAtlas.AddFontFromFileTTF(Constants.ImGuiFontFile, 32f);

            MainBackground = new StandardBackground(Constants.GameBackgroundMenuFile);

            SceneManager = new SceneManager(new MainMenuScene());

            Window.ClientSizeChanged += (sender, args) =>
                SceneManager.Resize(new Size(Window.ClientBounds.Width, Window.ClientBounds.Height));

            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ImGuiMG = new ImGuiMg(Window, GraphicsDevice);
            ImGuiHelper.Load();
        }

        protected override void Update(GameTime gameTime) {
            SceneManager.Update(gameTime);
            ImGuiMG.Update(gameTime);

            if (Exit)
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            SceneManager.Render(_spriteBatch, 1f);

            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing) {
            GameKeys.Save();

            SaveGameConfig();

            ImGuiMG.Dispose();

            SceneManager.Dispose();

            ResourceManager.Dispose();

            Log.Dispose();

            base.Dispose(disposing);
        }

        private void LoadGameConfig() {
            using (var file = File.OpenText(Constants.ConfigFile))
            using (var reader = new JsonTextReader(file)) {
                var configJson = (JObject) JToken.ReadFrom(reader);
                GraphicsDevice.PresentationParameters.BackBufferWidth = (int) configJson["width"];
                GraphicsDevice.PresentationParameters.BackBufferHeight = (int) configJson["height"];
                GraphicsDevice.PresentationParameters.IsFullScreen = (bool) configJson["fullScreen"];
                SoundVolume = (float) configJson["soundVolume"];
                DarkUI = (bool) configJson["darkUI"];
            }

            Log.Info(
                $"Initialisation en cours .. \n Taille : {GraphicsDevice.PresentationParameters.BackBufferWidth}x{GraphicsDevice.PresentationParameters.BackBufferHeight}");

            GameKeys.Load();
        }

        private void SaveGameConfig() {
            using (var fileWriter = File.CreateText(Constants.ConfigFile)) {
                using (var writer = new JsonTextWriter(fileWriter)) {
                    var configJson = new JObject(
                        new JProperty("width", GraphicsDevice.PresentationParameters.BackBufferWidth),
                        new JProperty("height", GraphicsDevice.PresentationParameters.BackBufferHeight),
                        new JProperty("fullScreen", GraphicsDevice.PresentationParameters.IsFullScreen),
                        new JProperty("soundVolume", SoundVolume),
                        new JProperty("darkUI", DarkUI)
                    );
                    configJson.WriteTo(writer);
                }
            }
        }
    }
}