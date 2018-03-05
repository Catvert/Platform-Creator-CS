using GeonBit.UI.Entities;
using Platform_Creator_CS.Sources.Entities.Containers;
using Platform_Creator_CS.Sources.Scenes;

namespace Platform_Creator_CS.Sources.UI {
    public interface ICustomUI {
        void InsertUI(Panel panel, Level level, EditorScene.UI editorSceneUI);
    }
}