using System;
using FastMember;
using GeonBit.UI.Entities;
using Platform_Creator_CS.Entities;
using Platform_Creator_CS.Sources.Entities.Containers;
using Platform_Creator_CS.Sources.Scenes;
using Platform_Creator_CS.Utility;

namespace Platform_Creator_CS.Sources.UI {
    public static class UIHelper {
        public static void AddUIItem<T>(string label, Panel panel, Func<T> getValue, Action<T> setValue, Entity entity, Level level,
            UI ui, EditorScene.UI editorSceneUI) {

            void SetValue(object newValue) {
                 setValue((T) newValue);
            }

            switch (getValue()) {
                case bool v: {
                    panel.AddChild(new CheckBox(label).Apply((ref CheckBox box) => {
                        box.Checked = v;
                        box.OnValueChange = e => {
                            SetValue(((CheckBox) e).Checked);
                        };
                    }));
                }
                    break;
            }
           
        }

        public static void InsertUI<T>(Panel panel, T instance, Entity entity, Level level, EditorScene.UI editorSceneUI) {
            if (instance is ICustomUI customUI) {
                customUI.InsertUI(panel, level, editorSceneUI);
            }

            var accessor = TypeAccessor.Create(typeof(T), true);

            foreach (var member in accessor.GetMembers()) {
                var ui = (UI) member.GetAttribute(typeof(UI), false);

                if (ui == null) continue;

                var value = accessor[instance, member.Name];

                AddUIItem(ui.Name ?? member.Name, panel, () => value, o => accessor[instance, member.Name] = o, entity, level, ui, editorSceneUI);

                accessor[instance, member.Name] = value;
            }
        }
    }
}