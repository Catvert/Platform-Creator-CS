using System.Collections.Generic;

namespace Platform_Creator_CS.Utility {
    public class SpriteSheet {
        private readonly Dictionary<string, SpriteFrame> _spriteFrames = new Dictionary<string, SpriteFrame>();

        public void Add(string name, SpriteFrame frame) {
            _spriteFrames[name] = frame;
        }

        public SpriteFrame GetFrame(string name) => _spriteFrames[name];

        public IEnumerable<SpriteFrame> GetFrames() => _spriteFrames.Values;
    }
}