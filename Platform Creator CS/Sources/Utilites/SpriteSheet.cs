using System.Collections.Generic;

namespace Platform_Creator_CS.Utility {
    public class SpriteSheet {
        private readonly Dictionary<string, TextureRegion> _regions = new Dictionary<string, TextureRegion>();

        public void Add(string name, TextureRegion frame) {
            _regions[name] = frame;
        }

        public TextureRegion GetRegion(string name) {
            return _regions[name];
        }

        public IEnumerable<TextureRegion> GetRegions() {
            return _regions.Values;
        }
    }
}