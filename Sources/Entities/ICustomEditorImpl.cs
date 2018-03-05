using System;
using System.Collections.Generic;
using System.Text;
using Platform_Creator_CS.Entities.Containers;
using Platform_Creator_CS.Scenes;

namespace Platform_Creator_CS.Entities {
    public interface ICustomEditorImpl {
        void InsertImGui(string label, Entity entity, Level level, EditorScene.UI editorSceneUI);
    }
}