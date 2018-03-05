using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using FastMember;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Platform_Creator_CS.Entities;
using Platform_Creator_CS.Entities.Containers;
using Platform_Creator_CS.Managers;
using Platform_Creator_CS.Scenes;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace Platform_Creator_CS.Utility {
    public static class ImGuiHelper {
        private const int MaxTextBufLength = 64;

        private static IntPtr _settingsTexture;
        private static IntPtr _favTexture;
     //   private static Song _tickButtonSound;

        private static readonly Dictionary<string, ImGuiStringWrapper> StringWrappers =
            new Dictionary<string, ImGuiStringWrapper>();

        private static readonly List<int> _hoveredTickButtons = new List<int>();


        public static void Load() {
            _settingsTexture =
                PCGame.ImGuiMG.GetOrCreateTextureBinding(ResourceManager.GetTexture(Constants.UIDir + "settings.png"));
            _favTexture =
                PCGame.ImGuiMG.GetOrCreateTextureBinding(ResourceManager.GetTexture(Constants.UIDir + "fav.png"));
        //    _tickButtonSound = ResourceManager.GetSound(Constants.GameDir + "tick.mp3");
        }

        private static float GetFontSize(Font font) {
            unsafe {
                return font.NativeFont->FontSize;
            }
        }

        public static void WithCenteredWindow(string windowTitle, Vector2 windowSize, Condition centerCond,
            WindowFlags flags, Action block) {
            var open = true;
            WithCenteredWindow(windowTitle, ref open, windowSize, centerCond, flags, block);
        }

        public static void WithCenteredWindow(string windowTitle, ref bool open, Vector2 windowSize,
            Condition centerCond, WindowFlags flags, Action block) {
            ImGui.SetNextWindowSize(windowSize, centerCond);
            ImGui.SetNextWindowPos(
                new Vector2(PCGame.ScreenSize.X / 2f - windowSize.X / 2f, PCGame.ScreenSize.Y / 2f - windowSize.Y / 2f),
                centerCond, new Vector2());

            ImGui.BeginWindow(windowTitle, ref open, flags);

            block();

            ImGui.EndWindow();
        }

        public static void WithMenuWindow(Vector2 windowSize, Action block) {
            ImGui.PushStyleColor(ColorTarget.WindowBg, new Vector4(0f));
            ImGui.PushStyleColor(ColorTarget.Button, new Vector4(0.2f, 0.5f, 0.9f, 1f));
            ImGui.PushStyleColor(ColorTarget.Text, new Vector4(1f));
            ImGui.PushStyleColor(ColorTarget.Border, new Vector4(0f));
            ImGui.PushStyleVar(StyleVar.FrameRounding, 10f);
            ImGui.PushStyleVar(StyleVar.FramePadding, new Vector2(10f));
            ImGui.PushFont(PCGame.ImGuiBigFont);

            WithCenteredWindow("#menu", windowSize, Condition.Always,
                WindowFlags.NoTitleBar | WindowFlags.NoCollapse | WindowFlags.NoMove | WindowFlags.NoResize |
                WindowFlags.NoBringToFrontOnFocus, block);

            ImGui.PopStyleColor(4);
            ImGui.PopStyleVar(2);
            ImGui.PopFont();
        }

        public static void WithIndent(Action block, float indentW = 0f) {
            ImGuiNative.igIndent(indentW);
            block();
            ImGuiNative.igUnindent(indentW);
        }

        public static bool ComboWithSettingsButton(string label, ref int currentItem, string[] items,
            Action popupBlock, bool settingsBtnDisabled = false, Action onSettingsBtnDisabled = null,
            bool searchBar = false) {
            var popupID = $"popup settings {label}";

            var changed = false;

            ImGui.PushItemWidth(Constants.DefaultWidgetsWidth - GetFontSize(PCGame.ImGuiDefaultFont) -
                                ImGui.GetStyle().ItemInnerSpacing.X * 3f);
            ImGui.PushID(label);
            if (searchBar) {
                if (SearchCombo("", ref currentItem, items))
                    changed = true;
            }
            else if (ImGui.Combo("", ref currentItem, items))
                changed = true;

            ImGui.PopID();
            ImGui.PopItemWidth();

            ImGui.SameLine(0f, ImGui.GetStyle().ItemInnerSpacing.X);

            //todo push item flag + onsettingsbtndisabled
            if (SettingsButton())
                ImGui.OpenPopup(popupID);

            ImGui.SameLine(0f, ImGui.GetStyle().ItemInnerSpacing.X);

            ImGui.Text(label);

            if (ImGui.BeginPopup(popupID)) {
                popupBlock();
                ImGui.EndPopup();
            }

            return changed;
        }

        public static bool SearchCombo(string label, ref int currentItem, string[] items,
            Action<int> selectableHovered = null) {
            var changed = false;

            if (ImGui.BeginCombo(label, items.ElementAtOrDefault(currentItem) ?? "", ComboFlags.HeightRegular)) {
                ImGui.SetCursorScreenPos(new Vector2(ImGui.GetCursorScreenPos().X + ImGui.GetStyle().ItemInnerSpacing.X,
                    ImGui.GetCursorScreenPos().Y));

                if (!StringWrappers.TryGetValue(label, out var wrapper)) {
                    wrapper = new ImGuiStringWrapper(MaxTextBufLength);
                    StringWrappers[label] = wrapper;
                }

                wrapper.ImGuiInputText("", true, InputTextFlags.Default, null);
                ImGui.SetCursorScreenPos(new Vector2(ImGui.GetCursorScreenPos().X,
                    ImGui.GetCursorScreenPos().Y + ImGui.GetStyle().ItemInnerSpacing.Y));
                ImGui.Separator();

                for (var i = 0; i < items.Length; ++i) {
                    if (!items[i].StartsWith(wrapper.ToString(), true, CultureInfo.InvariantCulture))
                        continue;

                    ImGui.PushID(i);

                    var itemSelected = i == currentItem;
                    if (ImGui.Selectable(items[i], itemSelected)) {
                        currentItem = i;
                        changed = true;
                    }

                    if (itemSelected)
                        ImGui.SetItemDefaultFocus();

                    if (ImGui.IsItemHovered(HoveredFlags.Default))
                        selectableHovered?.Invoke(i);

                    ImGui.PopID();
                }

                ImGui.EndCombo();
            }

            return changed;
        }

        public static bool SettingsButton() => ImGui.ImageButton(_settingsTexture,
            new Vector2(GetFontSize(PCGame.ImGuiDefaultFont)), new Vector2(0f), new Vector2(1f), 3, new Vector4(),
            new Vector4(1f));


        public static bool FavButton() => ImGui.ImageButton(_favTexture,
            new Vector2(GetFontSize(PCGame.ImGuiDefaultFont)), new Vector2(0f), new Vector2(1f), 3, new Vector4(),
            new Vector4(1f));

        public static void Action(string label, ref Entities.Actions.Action action, Entity entity, Level level,
            EditorScene.UI editorSceneUI) {
            var index = Array.IndexOf(Entities.Actions.Action.Actions, action.GetType());

            ImGui.PushID($"prop {label}");

            Entities.Actions.Action aInsert = action;

            if (ComboWithSettingsButton(label, ref index, Entities.Actions.Action.Actions.Select(e => e.Name).ToArray(),
                () => { InsertExposeEditor(aInsert, entity, level, editorSceneUI); }, searchBar: true)) {
                action = (Entities.Actions.Action) Activator.CreateInstance(Entities.Actions.Action.Actions[index],
                    true);
            }

            ImGui.PopID();
        }

        public static void EntityTag(ref string tag, Level level, string label = "tag") {
            var currentIndex = level.Tags.IndexOf(tag);
            ImGui.PushItemWidth(Constants.DefaultWidgetsWidth);
            if (ImGui.Combo(label, ref currentIndex, level.Tags.ToArray()))
                tag = level.Tags[currentIndex];
            ImGui.PopItemWidth();
        }

        public static void Entity(ref Entity entity, Level level, EditorScene.UI editorSceneUI,
            string label = "entité") {

        }

        public static void Prefab(string label, ref Prefab prefab) { }

        public static void Size(ref Size size, Size minSize, Size maxSize) {
          
        }

        public static void TexturePreviewTooltip(Entity entity) {

        }

        public static void Point(ref Point point, Point minPoint, Point maxPoint, EditorScene.UI editorSceneUI) {

        }

        public static void InputKey(ref Keys key) {

        }

        public static void EnumWithSettingsButton(string label, ref Enum e, Action popupBlock, bool settingsBtnDisabled = false,
            Action onSettingsBtnDisabled = null) {

        }

        public static void TextColored(Color color, string content) {
            ImGui.Text(content, new Vector4(color.R, color.G, color.B, color.A));
        }

        public static void TextPropertyColored(Color color, string propertyName, object value) {
            TextColored(color, propertyName);
            ImGui.SameLine();
            ImGui.Text(value.ToString());
        }

        public static bool TickSoundButton(string message, Vector2 size) {
            var pressed = ImGui.Button(message, size);

            if (ImGui.IsItemHovered(HoveredFlags.Default)) {
             //   MediaPlayer.Play(_tickButtonSound); //.Play(PCGame.SoundVolume, 0f, 0f);

            }

            return pressed;
        }

        public static void Enum(string label, ref Enum e) {
            var enumItems = System.Enum.GetValues(e.GetType()).Cast<Enum>().ToList();
            var currentItem = enumItems.IndexOf(e);

            ImGui.PushItemWidth(Constants.DefaultWidgetsWidth);

            if (ImGui.Combo(label, ref currentItem, enumItems.Select(en => en.ToString()).ToArray()))
                e = enumItems[currentItem];

            ImGui.PopItemWidth();
        }

        public static void AddImGuiListItems<T>(string label, List<T> array, Func<T, string> itemLabel,
            Func<T> createItem,
            Extensions.RefAction<T> itemBlock) {
            var removeItem = false;
            for (var i = 0; i < array.Count; ++i) {
                ImGui.PushID($"remove {i}");
                if (ImGui.Button("Suppr.")) {
                    array.RemoveAt(i);
                    removeItem = true;
                }

                ImGui.PopID();

                if (!removeItem) {
                    ImGui.SameLine();

                    ImGui.PushID($"collapse {i}");

                    if (ImGui.CollapsingHeader("", TreeNodeFlags.CollapsingHeader)) {
                        ImGui.SameLine(0f, ImGui.GetStyle().ItemInnerSpacing.X);
                        ImGui.Text(itemLabel(array[i]));

                        ImGui.PushID($"array item {i}");

                        var item = array[i];
                        WithIndent(() => itemBlock(ref item));
                        array[i] = item;

                        ImGui.PopID();
                    }
                    else {
                        ImGui.SameLine(0f, ImGui.GetStyle().ItemInnerSpacing.X);
                        ImGui.Text(itemLabel(array[i]));
                    }

                    ImGui.PopID();
                }
            }

            ImGui.PushID($"{label} add btn");
            if (ImGui.Button("Ajouter", new Vector2(array.Count > 0 ? -1 : Constants.DefaultWidgetsWidth, 0)))
                array.Add(createItem());
            ImGui.PopID();
        }

        public static void AddImGuiListItems<T>(string label, List<T> array, Func<T, string> itemLabel,
            Func<T> createItem,
            ExposeEditor itemExposeEditor, Entity entity, Level level, EditorScene.UI editorSceneUI) {
            AddImGuiListItems(label, array, itemLabel, createItem,
                (ref T value) =>
                    AddImGuiItem(itemLabel(value), ref value, entity, level, itemExposeEditor, editorSceneUI));
        }

        public static void AddImGuiItem<T>(string label, ref T value, Entity entity, Level level,
            ExposeEditor exposeEditor, EditorScene.UI editorSceneUI) {
            void SetValue(ref T v, object newValue) {
                v = (T) newValue;
            }

            switch (value) {
                case Entities.Actions.Action v: {
                    Action(label, ref v, entity, level, editorSceneUI);
                    SetValue(ref value, v);
                }
                    break;
                case ICustomEditorImpl v:
                    InsertExposeEditor(v, entity, level, editorSceneUI);
                    break;
                case bool v: {
                    ImGui.Checkbox(label, ref v);
                    SetValue(ref value, v);
                }
                    break;
                case int v: {
                    ImGui.PushItemWidth(Constants.DefaultWidgetsWidth);
                    ImGui.SliderInt(label, ref v, exposeEditor.MinValue.Round(), exposeEditor.MaxValue.Round(), "%.0f");
                    ImGui.PopItemWidth();

                    SetValue(ref value, v);
                }
                    break;
                case float v: {
                    ImGui.PushItemWidth(Constants.DefaultWidgetsWidth);
                    ImGui.SliderFloat(label, ref v, exposeEditor.MinValue, exposeEditor.MaxValue, "%.1f", 1f);
                    ImGui.PopItemWidth();
                    SetValue(ref value, v);
                }
                    break;
                case Prefab v: {
                    Prefab(label, ref v);
                    SetValue(ref value, v);
                }
                    break;
                case Size v: {
                    Size(ref v, new Size(exposeEditor.MinValue.Round()), new Size(exposeEditor.MaxValue.Round()));
                    SetValue(ref value, v);
                }
                    break;
                case string v: {
                    if (exposeEditor.IsTag) {
                        EntityTag(ref v, level);
                        SetValue(ref value, v);
                    }
                    else {
                        if (!StringWrappers.TryGetValue(label, out var wrapper)) {
                            wrapper = new ImGuiStringWrapper(MaxTextBufLength, v);
                            StringWrappers[label] = wrapper;
                        }

                        wrapper.ImGuiInputText(label, true, InputTextFlags.Default, null);
                        SetValue(ref value, wrapper.ToString());
                    }
                }
                    break;
                case Enum v: {
                    Enum(label, ref v);
                    SetValue(ref value, v);
                }
                    break;
                default: {
                    InsertExposeEditor(value, entity, level, editorSceneUI);
                }
                    break;
            }

            if (exposeEditor.Description != null) { }
        }

        public static void InsertExposeEditor<T>(T instance, Entity entity, Level level, EditorScene.UI editorSceneUI) {
            if (instance is ICustomEditorImpl impl) {
                impl.InsertImGui(instance.GetType().Name, entity, level, editorSceneUI);
            }

            var accessor = TypeAccessor.Create(typeof(T), true);

            foreach (var member in accessor.GetMembers()) {
                var exposeEditor = (ExposeEditor) member.GetAttribute(typeof(ExposeEditor), false);

                if (exposeEditor == null) continue;

                var value = accessor[instance, member.Name];

                AddImGuiItem(exposeEditor.Name ?? member.Name, ref value, entity, level, exposeEditor,
                    editorSceneUI);

                accessor[instance, member.Name] = value;
            }
        }
    }

    public class ImGuiStringWrapper : IDisposable {
        public ImGuiStringWrapper(int textLength, string initialValue = "") {
            TextLength = textLength;
            TextInputBuffer = Marshal.AllocHGlobal(textLength);

            var initialBytes = Encoding.ASCII.GetBytes(initialValue);
            unsafe {
                var ptr = (long*) TextInputBuffer.ToPointer();
                for (var i = 0; i < textLength / sizeof(long); ++i) ptr[i] = initialBytes.ElementAtOrDefault(i);
            }
        }

        public int TextLength { get; }
        public IntPtr TextInputBuffer { get; private set; }

        public void Dispose() {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        public override string ToString() {
            return Marshal.PtrToStringAnsi(TextInputBuffer);
        }

        public void ImGuiInputText(string label, bool defaultItemWidth, InputTextFlags flags,
            TextEditCallback callback) {
            if (defaultItemWidth)
                ImGui.PushItemWidth(Constants.DefaultWidgetsWidth);
            ImGui.InputText(label, TextInputBuffer, (uint) TextLength, flags, callback);
            if (defaultItemWidth)
                ImGui.PopItemWidth();
        }

        private void ReleaseUnmanagedResources() {
            if (TextInputBuffer != IntPtr.Zero) {
                Marshal.FreeHGlobal(TextInputBuffer);
                TextInputBuffer = IntPtr.Zero;
            }
        }

        ~ImGuiStringWrapper() {
            ReleaseUnmanagedResources();
        }
    }
}