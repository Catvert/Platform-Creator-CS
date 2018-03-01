using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Platform_Creator_CS.Managers;
using Platform_Creator_CS.Utility;

namespace Platform_Creator_CS.Entities.Components {
    public class TextureComponent : Component, IRenderable
    {
        private SpriteFrame frame;

        public string Sheet;
        public string FrameName;

        [JsonConstructor]
        public TextureComponent(string sheet, string frame) {
            Sheet = sheet;
            FrameName = frame;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            this.frame = ResourceManager.GetFrame(Sheet, FrameName);
        }

        public TextureComponent(SpriteFrame frame) {
            this.frame = frame;
        }

        public void Render(SpriteBatch batch, float alpha) {
            batch.Draw(frame, Entity.Box.ToRectangle(), new Color(255, 255, 255, alpha));
        }
    }
}