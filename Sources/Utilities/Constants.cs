using System.IO;

namespace Platform_Creator_CS.Utility {
    public static class Constants {
        public const string GameTitle = "Platform-Creator";
        public const float GameVersion = 2.0f;

        public const int MaxEntitySize = 10000;

        public const int MinLayer = -100;
        public const int MaxLayer = 100;

        public const int ViewportRatioWidth = 1920;
        public const int ViewportRatioHeight = 1080;

        public const float DefaultWidgetsWidth = 125f;

        public const float PhysicsEpsilon = 0.2f;
        public const float PhysicsDeltaSpeed = 60f;
        public const int PhysicsDefaultGravity = 15;

        public const string AssetsDir = "Assets/";
        public const string ConfigFile = AssetsDir + "config.json";
        public const string ConfigKeysFile = AssetsDir + "keysConfig.json";

        public const string GameDir = AssetsDir + "game/";
        public const string UIDir = AssetsDir + "ui/";
        public const string FontsDir = AssetsDir + "fonts/";
        public const string PacksDir = AssetsDir + "packs/";
        public const string PacksSMCDir = PacksDir + "smc/";
        public const string PacksKenneyDir = PacksDir + "kenney/";
        public const string TexturesDir = AssetsDir + "textures/";
        public const string SoundsDir = AssetsDir + "sounds/";
        public const string MusicsDir = AssetsDir + "music/";
        public const string BackgroundsDir = AssetsDir + "backgrounds/";
        public const string LevelsDir = AssetsDir + "levels/";

        public const string ImGuiFontFile = FontsDir + "imgui.ttf";
        public const string MainFontFile = FontsDir + "mainFont.fnt";
        public const string EditorFontFile = FontsDir + "editorFont.fnt";

        public const string GameBackgroundMenuFile = GameDir + "mainmenu.png";
        public const string GameLogoFile = GameDir + "logo.png";
        public const string MenuMusicFile = GameDir + "mainmusic.ogg";

        public const string PrefabExtension = "prefab";
        public const string LevelExtension = "pclvl";
        public static string[] LevelTextureExtension = { "jpg", "png" };
        public static string[] LevelAtlasExtension = { "atlas" };
        public static string[] LevelSoundExtension = { "mp3", "wav", "ogg" };
        public static string[] LevelScriptExtension = { "js" };
    }
}