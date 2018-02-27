using System;
using System.IO;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Platform_Creator_CS.Managers;
using Platform_Creator_CS.Scenes;
using Platform_Creator_CS.Utility;

namespace Platform_Creator_CS {
    public class PCGame : Game {
        public static GraphicsDeviceManager Graphics { get; private set; }
        public static GameWindow GameWindow { get; private set; }

        public static float SoundVolume { get; set; }
        private static bool _darkUI = false;

        public static bool DarkUI {
            get => _darkUI;
            set {
                _darkUI = value;

                if (value)
                    ImGui.StyleColorsDark(ImGui.GetStyle());
                else
                    ImGui.StyleColorsLight(ImGui.GetStyle());
            }
        }

        public static SceneManager SceneManager { get; private set; }

        public static Microsoft.Xna.Framework.Point ScreenSize => Graphics.GraphicsDevice.PresentationParameters.Bounds.Size;

        public static Font ImGuiDefaultFont { get; private set; }
        public static Font ImGuiBigFont { get; private set; }

        public static Background MainBackground { get; private set; }

        private SpriteBatch spriteBatch;
        private ImGuiMg imguiMG;

        public static bool Exit { get; set; }

        public PCGame() {
            Graphics = new GraphicsDeviceManager(this);
            GameWindow = Window;

            Content.RootDirectory = "Content";
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

            base.Initialize();
        }

  
        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            imguiMG = new ImGuiMg(Window, GraphicsDevice);
        }

        protected override void Update(GameTime gameTime) {
            SceneManager.Update(gameTime);

            if(Exit)
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            imguiMG.Update(gameTime);

            SceneManager.Render(spriteBatch);

            imguiMG.Draw();

            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing) {
            GameKeys.Save();

            SaveGameConfig();

            imguiMG.Dispose();

            SceneManager.Dispose();

            ResourceManager.Dispose();

            Log.Dispose();

            base.Dispose(disposing);
        }

        private void LoadGameConfig() {
            using(StreamReader file = File.OpenText(Constants.ConfigFile))
            using(JsonTextReader reader = new JsonTextReader(file)) {
                JObject configJson = (JObject) JToken.ReadFrom(reader);
                GraphicsDevice.PresentationParameters.BackBufferWidth = (int) configJson["width"];
                GraphicsDevice.PresentationParameters.BackBufferHeight = (int) configJson["height"];
                GraphicsDevice.PresentationParameters.IsFullScreen = (bool) configJson["fullScreen"];
                SoundVolume = (float) configJson["soundVolume"];
                DarkUI = (bool) configJson["darkUI"];
            }

            Log.Info($"Initialisation en cours .. \n Taille : {GraphicsDevice.PresentationParameters.BackBufferWidth}x{GraphicsDevice.PresentationParameters.BackBufferHeight}");

            GameKeys.Load();
        }

        private void SaveGameConfig() {
            using(var fileWriter = File.CreateText(Constants.ConfigFile)) {
                using(var writer = new JsonTextWriter(fileWriter)) {
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