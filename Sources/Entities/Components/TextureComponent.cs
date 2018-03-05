using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Platform_Creator_CS.Managers;
using Platform_Creator_CS.Utility;
using IUpdateable = Platform_Creator_CS.Utility.IUpdateable;

namespace Platform_Creator_CS.Entities.Components {
    public class TextureComponent : Component, IRenderable, IUpdateable {
        [JsonProperty("textures")] private readonly List<TextureData> _textures = new List<TextureData>();

        public TextureComponent(params TextureData[] textures) {
            _textures.AddRange(textures);
        }

        [JsonConstructor]
        private TextureComponent() { }

        public int CurrentIndex { get; set; }

        public void Render(SpriteBatch batch, float alpha) {
            _textures[CurrentIndex].Draw(batch, alpha, Entity);
        }

        public void Update(GameTime gameTime) {
            _textures[CurrentIndex].Update(gameTime);
        }

        public class Frame {
            private TextureRegion _frame;

            public Frame(string sheet, string region) {
                Sheet = sheet;
                Region = region;
                LoadResources();
            }

            public Frame(string textureFile) {
                TextureFile = textureFile;
                LoadResources();
            }

            [JsonConstructor]
            private Frame() { }

            [JsonProperty]
            public string Sheet { get; private set; }

            [JsonProperty]
            public string Region { get; private set; }

            [JsonProperty]
            public string TextureFile { get; private set; }

            [OnDeserialized]
            private void OnDeserialized(StreamingContext context) {
                LoadResources();
            }

            public void LoadResources() {
                _frame = TextureFile == null
                    ? ResourceManager.GetSheet(Sheet).GetRegion(Region)
                    : new TextureRegion(ResourceManager.GetTexture(TextureFile));
            }

            public void Draw(SpriteBatch batch, float alpha, Rect destination) {
                batch.Draw(_frame, destination.ToRectangle(), new Color(255, 255, 255, alpha));
            }
        }

        public class TextureData : IUpdateable {
            [JsonProperty("frames")] private readonly List<Frame> _frames = new List<Frame>();

            private float _elapsedTime;

            public TextureData(string name, float frameDuration, params Frame[] frames) {
                Name = name;
                FrameDuration = frameDuration;
                _frames.AddRange(frames);
            }

            [JsonConstructor]
            private TextureData() { }

            public string Name { get; set; }

            public float FrameDuration { get; set; }

            public bool RepeatRegion { get; set; }
            public Size RepeatRegionSize { get; set; } = new Size(50, 50);

            public void Update(GameTime gameTime) {
                _elapsedTime += gameTime.ElapsedGameTime.Milliseconds / 1000f; // TODO overflow
            }


            public Frame GetCurrentFrame() {
                if (_frames.Count == 1) return _frames[0];

                var frameNumber = (int) (_elapsedTime / FrameDuration);

                return _frames[frameNumber % _frames.Count];
            }

            public void Draw(SpriteBatch batch, float alpha, Entity entity) {
                if (RepeatRegion && _frames.Count == 1)
                    for (var x = 0; x < Math.Floor(entity.Box.Width / (double) RepeatRegionSize.Width); ++x)
                    for (var y = 0; y < Math.Floor(entity.Box.Height / (double) RepeatRegionSize.Height); ++y)
                        GetCurrentFrame().Draw(batch, alpha,
                            new Rect(entity.Box.X + x * RepeatRegionSize.Width,
                                entity.Box.Y + y * RepeatRegionSize.Height, RepeatRegionSize.Width,
                                RepeatRegionSize.Height));
                else
                    GetCurrentFrame().Draw(batch, alpha, entity.Box);
            }
        }
    }
}