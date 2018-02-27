using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Platform_Creator_CS.Managers {
    public static class ResourceManager {
        private static Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        private static Dictionary<string, SoundEffect> _sounds = new Dictionary<string, SoundEffect>();

        public static Texture2D GetTexture(string file) {
            if (_textures.ContainsKey(file))
                return _textures[file];

            using(var fs = File.Open(file, FileMode.Open)) {
                var texture = Texture2D.FromStream(PCGame.Graphics.GraphicsDevice, fs);
                _textures[file] = texture;
                return texture;
            }
        }

        public static SoundEffect GetSound(string file) {
            if (_sounds.ContainsKey(file))
                return _sounds[file];
            using(var fs = File.Open(file, FileMode.Open)) {
                var sound = SoundEffect.FromStream(fs);
                _sounds[file] = sound;
                return sound;
            }
        }

        public static void Dispose() {
            foreach (var texture in _textures.Values)
                texture.Dispose();
            foreach (var sound in _sounds.Values)
                sound.Dispose();
        }
    }
}