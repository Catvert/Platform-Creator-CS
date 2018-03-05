using Platform_Creator_CS.Entities.Containers;
using Platform_Creator_CS.Scenes;

namespace Platform_Creator_CS.Scenes {
    public class EditorScene : Scene {
        public EditorScene(Level level, bool applyMusicTransition) : base(level.Background) {
            EntityContainer = level;
        }

        public class UI {

        }

        protected override EntityContainer EntityContainer { get; }
    }
}