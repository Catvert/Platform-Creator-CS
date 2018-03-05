using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using Newtonsoft.Json;
using Platform_Creator_CS.Entities.Components;
using Platform_Creator_CS.Managers;
using Platform_Creator_CS.Scenes;
using Platform_Creator_CS.Serialization;
using Platform_Creator_CS.Sources.Entities.Actions;
using Platform_Creator_CS.Utility;

namespace Platform_Creator_CS.Entities.Containers {
    public class Level : EntityMatrixContainer {
        private readonly (string dir, List<string> prefabs) _levelPrefabs;
        private readonly (string dir, List<string> scripts) _levelScripts;
        private readonly (string dir, List<string> sheets) _levelSheets;
        private readonly (string dir, List<string> sounds) _levelSounds;
        private readonly (string dir, List<string> textures) _levelTextures;

        public Color BackgroundColor = Color.White;

        [JsonIgnore] public Action<bool> Exit;

        public Level(string levelPath, float gameVersion, Background background, string musicFile) {
            Exit = success => {
                MediaPlayer.Play(ResourceManager
                    .GetSound(success
                        ? Constants.GameDir + "game-over-success.wav"
                        : Constants.GameDir + "game-over-fail.wav"));
              

                PCGame.SceneManager.LoadScene(new EndLevelScene(this), true, true);
            };

            LevelPath = levelPath;
            GameVersion = gameVersion;
            Background = background;
            MusicFile = musicFile;

            _levelTextures = ($"{Directory.GetParent(levelPath).FullName}/textures", new List<string>());
            _levelSheets = ($"{Directory.GetParent(levelPath).FullName}/sheets", new List<string>());
            _levelSounds = ($"{Directory.GetParent(levelPath).FullName}/sounds", new List<string>());
            _levelPrefabs = ($"{Directory.GetParent(levelPath).FullName}/prefabs", new List<string>());
            _levelScripts = ($"{Directory.GetParent(levelPath).FullName}/scripts", new List<string>());

            if (!Directory.Exists(_levelTextures.dir))
                Directory.CreateDirectory(_levelTextures.dir);
            else
                _levelTextures.textures = Directory
                    .EnumerateFiles(_levelTextures.dir, "*.*", SearchOption.AllDirectories)
                    .Where(file => Constants.LevelTextureExtension.Any(file.EndsWith)).ToList();

            if (!Directory.Exists(_levelSheets.dir))
                Directory.CreateDirectory(_levelSheets.dir);
            else
                _levelSheets.sheets = Directory.EnumerateFiles(_levelSheets.dir, "*.*", SearchOption.AllDirectories)
                    .Where(file => Constants.LevelSheetExtension.Any(file.EndsWith)).ToList();

            if (!Directory.Exists(_levelSounds.dir))
                Directory.CreateDirectory(_levelSounds.dir);
            else
                _levelSounds.sounds = Directory.EnumerateFiles(_levelSounds.dir, "*.*", SearchOption.AllDirectories)
                    .Where(file => Constants.LevelSoundExtension.Any(file.EndsWith)).ToList();

            if (!Directory.Exists(_levelPrefabs.dir))
                Directory.CreateDirectory(_levelPrefabs.dir);
            else
                _levelPrefabs.prefabs = Directory.EnumerateFiles(_levelPrefabs.dir, "*.*", SearchOption.AllDirectories)
                    .Where(file => file.EndsWith(Constants.PrefabExtension)).ToList();


            if (!Directory.Exists(_levelScripts.dir))
                Directory.CreateDirectory(_levelScripts.dir);
            else
                _levelScripts.scripts = Directory.EnumerateFiles(_levelScripts.dir, "*.*", SearchOption.AllDirectories)
                    .Where(file => Constants.LevelScriptExtension.Any(file.EndsWith)).ToList();
        }

        [JsonProperty]
        public string LevelPath { get; private set; }

        [JsonProperty]
        public float GameVersion { get; private set; }

        public Background Background { get; set; }

        public string MusicFile { get; set; }

        public int GravitySpeed { get; set; } = Constants.PhysicsDefaultGravity;

        public List<string> Tags { get; } = new List<string>(Enum.GetNames(typeof(Tags)));

        public List<Entity> Favoris { get; } = new List<Entity>();

        public float InitialZoom { get; set; } = 1f;

        [JsonIgnore]
        public float Zoom { get; set; } = 1f;

        [JsonIgnore]
        public bool ApplyGravity { get; set; } = true;

        [JsonIgnore]
        public int ScorePoints { get; set; }

        public IEnumerable<string> GetLevelTextures() {
            return _levelTextures.textures;
        }

        public IEnumerable<string> GetLevelSheets() {
            return _levelSheets.sheets;
        }

        public IEnumerable<string> GetLevelSounds() {
            return _levelSounds.sounds;
        }

        public IEnumerable<string> GetLevelPrefabs() {
            return _levelPrefabs.prefabs;
        }

        public IEnumerable<string> GetLevelScripts() {
            return _levelScripts.scripts;
        }

        public override void Update(GameTime gameTime) {
            foreach (var entity in GetProcessEntities())
                if (entity.Box.Bottom() > MatrixRect.Bottom())
                    entity.OnOutOfMapAction.Invoke(entity);

            base.Update(gameTime);
        }

        public void UpdateCamera(Camera2D camera, bool lerp) {
            camera.Zoom = MathHelper.Lerp(camera.Zoom, Zoom, 0.1f);

            var viewportWidthZoom = Constants.ViewportRatioWidth * camera.Zoom;
            var viewportHeightZoom = Constants.ViewportRatioHeight * camera.Zoom;

            ActiveRect.Width = (viewportWidthZoom * 1.0f).Round();
            ActiveRect.Height = (viewportHeightZoom * 1.0f).Round();

            if (FollowEntity != null) {
                var posX = Math.Clamp(FollowEntity.Box.Center().X - viewportWidthZoom / 2f, MatrixRect.Left(),
                    MatrixRect.Right());
                var posY = Math.Clamp(FollowEntity.Box.Center().Y - viewportHeightZoom / 2f, MatrixRect.Top(),
                    MatrixRect.Bottom());

                var lerpX = Math.Clamp(MathHelper.Lerp(camera.Position.X, posX, 0.1f),
                    MatrixRect.Left(), MatrixRect.Right() - viewportWidthZoom);
                var lerpY = Math.Clamp(MathHelper.Lerp(camera.Position.Y, posY, 0.1f),
                    MatrixRect.Top(), MatrixRect.Bottom() - viewportHeightZoom);

                camera.Position = new Vector2(lerp ? lerpX : posX, lerp ? lerpY : posY);
            }
        }

        public static Level NewLevel(string levelName) {
            var dir = Constants.LevelsDir + levelName;
            if (Directory.Exists(dir)) {
                Log.Warn($"Le niveau {levelName} existe déjà !");
                Directory.Delete(dir, true);
            }

            Directory.CreateDirectory(dir);

            var level = new Level(dir + $"/{Constants.LevelDataFile}", Constants.GameVersion, null, null);

            var state = new EntityState("default");
            state.AddComponent(new TextureComponent(new TextureComponent.TextureData("default", 0f,
                new TextureComponent.Frame(Constants.PacksSMCDir + "blocks.atlas", "blocks/metal/Metal Blue"))));

            state.AddComponent(new InputComponent(
                new InputData(Keys.Q, true, new PhysicsAction(PhysicsComponent.PhysicsActions.MoveLeft)),
                new InputData(Keys.D, true, new PhysicsAction(PhysicsComponent.PhysicsActions.MoveRight)),
                new InputData(Keys.Z, true, new PhysicsAction(PhysicsComponent.PhysicsActions.MoveUp)),
                new InputData(Keys.S, true, new PhysicsAction(PhysicsComponent.PhysicsActions.MoveDown)),
                new InputData(Keys.Space, false, new PhysicsAction(PhysicsComponent.PhysicsActions.Jump))
            ));
            //  state.AddComponent(new PhysicsComponent(false, 10, 250, gravity: false){IgnoreTags = { "test"}});

            level.AddEntity(new Entity("test", "hi", new Rect(200, 225, 50, 50), state));

            return level;
        }

        public static Level LoadFromFile(string levelDir) {
            try {
                var level = SerializationFactory.DeserializeFromFile<Level>(levelDir + $"/{Constants.LevelDataFile}");

                if (level.GameVersion != Constants.GameVersion)
                    throw new Exception("Le niveau n'est pas prévu pour cette version du jeu !");
                return level;
            }
            catch (Exception e) {
                Log.Error(e, "Erreur lors du chargement du niveau !");
            }

            return null;
        }
    }
}