using System.IO;
using GeonBit.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Platform_Creator_CS.Sources.Entities;
using Platform_Creator_CS.Sources.Entities.Components;
using Platform_Creator_CS.Sources.Entities.Containers;
using Platform_Creator_CS.Sources.Managers;
using Platform_Creator_CS.Sources.Scenes;
using Platform_Creator_CS.Sources.Utilites;
using Platform_Creator_CS.Utility;
using Point = Microsoft.Xna.Framework.Point;

namespace Platform_Creator_CS.Sources {
    public class PCGame : Game {
        private SpriteBatch _spriteBatch;

        public PCGame() {
            Graphics = new GraphicsDeviceManager(this);
            GameWindow = Window;

            Content.RootDirectory = "Content";
        }

        public static GraphicsDeviceManager Graphics { get; private set; }
        public static GameWindow GameWindow { get; private set; }

        public static float SoundVolume { get; set; }

        public static SceneManager SceneManager { get; private set; }
        public static Point ScreenSize => GameWindow.ClientBounds.Size;

        public static Background MainBackground { get; private set; }

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

            UserInterface.Initialize(Content, BuiltinThemes.hd);
            UserInterface.Active.ShowCursor = false;
            UserInterface.Active.GlobalScale = 1f;
            UserInterface.Active.UseRenderTarget = true;

            MainBackground = new StandardBackground(Constants.GameBackgroundMenuFile);

            SceneManager = new SceneManager(new MainMenuScene());

            Window.ClientSizeChanged += (sender, args) =>
                SceneManager.Resize(new Size(Window.ClientBounds.Width, Window.ClientBounds.Height));

            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime) {
            UserInterface.Active.Update(gameTime);
            SceneManager.Update(gameTime);

            if (Exit)
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            UserInterface.Active.Draw(_spriteBatch);

            SceneManager.Render(_spriteBatch, 1f);

            UserInterface.Active.DrawMainRenderTarget(_spriteBatch);

            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing) {
            GameKeys.Save();

            SaveGameConfig();

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
                        new JProperty("soundVolume", SoundVolume)
                    );
                    configJson.WriteTo(writer);
                }
            }
        }
    }
}