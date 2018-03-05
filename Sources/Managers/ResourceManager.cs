using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Platform_Creator_CS.Utility;

namespace Platform_Creator_CS.Managers {
    public static class ResourceManager {
        private static readonly Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        private static readonly Dictionary<string, Song> Sounds = new Dictionary<string, Song>();
        private static readonly Dictionary<string, SpriteSheet> Sheets = new Dictionary<string, SpriteSheet>();

        public static Texture2D GetTexture(string file) {
            if (Textures.ContainsKey(file))
                return Textures[file];

            using (var fs = File.Open(file, FileMode.Open)) {
                var texture = Texture2D.FromStream(PCGame.Graphics.GraphicsDevice, fs);
                Textures[file] = texture;
                return texture;
            }
        }

        public static Song GetSound(string file) {
            if (Sounds.ContainsKey(file))
                return Sounds[file];
            using (var fs = File.Open(file, FileMode.Open)) {
                var sound = Song.FromUri(file, new Uri(file));
                Sounds[file] = sound;
                return sound;
            }
        }

        public static SpriteSheet GetSheet(string file) {
            if (Sheets.ContainsKey(file))
                return Sheets[file];
            var sheet = SpriteSheetLoader.Load(file);

            Sheets[file] = sheet;

            return sheet;
        }

        public static void Dispose() {
            foreach (var texture in Textures.Values)
                texture.Dispose();
            foreach (var sound in Sounds.Values)
                sound.Dispose();
        }
    }
}