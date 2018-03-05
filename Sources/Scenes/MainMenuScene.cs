using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platform_Creator_CS.Entities;
using Platform_Creator_CS.Entities.Actions;
using Platform_Creator_CS.Entities.Containers;
using Platform_Creator_CS.Utilities;
using Platform_Creator_CS.Utility;
using Action = Platform_Creator_CS.Entities.Actions.Action;
using Vec2 = System.Numerics.Vector2;

namespace Platform_Creator_CS.Scenes {
    public sealed class MainMenuScene : Scene {
        private readonly ImGuiStringWrapper _copyLevelNameBuf = new ImGuiStringWrapper(64);

        private readonly List<string> _levelDirs =
            Directory.GetDirectories(Constants.LevelsDir, "*", SearchOption.TopDirectoryOnly)
                .Where(dir => File.Exists(dir + $"/{Constants.LevelDataFile}")).ToList();

        private readonly ImGuiStringWrapper _newLevelNameBuf = new ImGuiStringWrapper(64);

        private const string CopyLevelID = "copy level";

        private int _currentLevelIndex;

        private const string ErrorInLevelID = "error in loading level";

        private const string NewLevelID = "new level";
        private bool _newLevelOpen = true;

        private bool _showSelectLevelWindow;
        private bool _showSettingsWindow;

        private readonly Dictionary<GameKeys.GKeys, bool> _settingsKeys = new Dictionary<GameKeys.GKeys, bool>();

        private readonly KeyboardListener _keyboardListener = new KeyboardListener();

        public MainMenuScene() : base(PCGame.MainBackground) {
            PCGame.AddLogoMenu(EntityContainer);

            foreach (GameKeys.GKeys gkey in Enum.GetValues(typeof(GameKeys.GKeys)))
                _settingsKeys[gkey] = false;
        }

        private class Te : ICustomEditorImpl {
            public List<string> p = new List<string>();
            public void InsertImGui(string label, Entity entity, Level level, EditorScene.UI editorSceneUI) {
                ImGuiHelper.AddImGuiListItems(label, p, b => "presser", () => "", new ExposeEditor(maxValue: 100, isTag:true), entity, level, editorSceneUI);
            }
        }
        Te te = new Te();

        public override void Render(SpriteBatch batch, float alpha) {
            base.Render(batch, alpha);

            DrawUI();

            ImGuiHelper.InsertExposeEditor(te, null, null, null);
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            _keyboardListener.Update(gameTime);
        }

        private void DrawUI() {
            DrawMainMenu();

            if (_showSelectLevelWindow)
                DrawSelectLevelWindow();
            if (_showSettingsWindow)
                DrawSettingsWindow();
        }

        private void DrawMainMenu() {
            ImGuiHelper.WithMenuWindow(new Vec2(300, 180), () => {
                if (ImGuiHelper.TickSoundButton("Jouer !", new Vec2(-1, 0)))
                    _showSelectLevelWindow = true;
                if (ImGui.Button("Options", new Vec2(-1, 0))) {
                    foreach (GameKeys.GKeys gkey in Enum.GetValues(typeof(GameKeys.GKeys)))
                        _settingsKeys[gkey] = false;
                    _keyboardListener.ClearEvent();

                    _showSettingsWindow = true;
                }

                if (ImGui.Button("Quitter", new Vec2(-1, 0)))
                    PCGame.Exit = true;
            });
        }

        private void DrawSelectLevelWindow() {
            ImGuiHelper.WithCenteredWindow("Sélection d'un niveau", ref _showSelectLevelWindow, new Vec2(215f, 165f),
                Condition.Appearing, WindowFlags.NoResize | WindowFlags.NoCollapse,
                () => {
                    var openCopyPopup = false;

                    ImGuiHelper.ComboWithSettingsButton("niveaux", ref _currentLevelIndex,
                        _levelDirs.Select(dir => new DirectoryInfo(dir).Name).ToArray(), () => {
                            if (ImGui.Button("Copier", Constants.DefaultItemWidth)) openCopyPopup = true;

                            if (ImGui.Button("Supprimer", Constants.DefaultItemWidth)) {
                                Directory.Delete(_levelDirs[_currentLevelIndex], true);
                                _levelDirs.RemoveAt(_currentLevelIndex);
                                ImGui.CloseCurrentPopup();
                            }
                        }, searchBar: true);

                    if (openCopyPopup)
                        ImGui.OpenPopup(CopyLevelID);

                    if (ImGui.Button("Jouer", new Vec2(-1, 0))) {
                        var level = Level.LoadFromFile(_levelDirs[_currentLevelIndex]);
                        if (level != null)
                            PCGame.SceneManager.LoadScene(new GameScene(level), true, true);
                        else
                            ImGui.OpenPopup(ErrorInLevelID);
                    }

                    if (ImGui.Button("Éditer", new Vec2(-1, 0))) {
                        var level = Level.LoadFromFile(_levelDirs[_currentLevelIndex]);
                        if (level != null)
                            PCGame.SceneManager.LoadScene(new EditorScene(level, false), true, true);
                        else
                            ImGui.OpenPopup(ErrorInLevelID);
                    }

                    ImGui.Separator();

                    if (ImGui.Button("Nouveau niveau", new Vec2(-1, 0))) {
                        ImGui.OpenPopup(NewLevelID);
                        _newLevelOpen = true;
                    }

                    if (ImGui.BeginPopupModal(NewLevelID, ref _newLevelOpen,
                        WindowFlags.NoCollapse | WindowFlags.NoResize)) {
                        _newLevelNameBuf.ImGuiInputText("nom", true, InputTextFlags.Default, null);

                        if (ImGui.Button("Créer !", new Vec2(-1, 0)))
                            if (!string.IsNullOrWhiteSpace(_newLevelNameBuf.ToString()))
                                PCGame.SceneManager.LoadScene(
                                    new EditorScene(Level.NewLevel(_newLevelNameBuf.ToString()), false), true, true);

                        ImGui.EndPopup();
                    }

                    if (ImGui.BeginPopup(CopyLevelID)) {
                        _copyLevelNameBuf.ImGuiInputText("nom", true, InputTextFlags.Default, null);

                        if (ImGui.Button("Copier", new Vec2(-1, 0)))
                            if (!string.IsNullOrWhiteSpace(_copyLevelNameBuf.ToString())) {
                                var levelDir = _levelDirs[_currentLevelIndex];
                                var copyLevelDir = Constants.LevelsDir + _copyLevelNameBuf;

                                Utility.Utility.CopyDirectoryRecursively(new DirectoryInfo(levelDir),
                                    new DirectoryInfo(copyLevelDir));

                                _levelDirs.Add(copyLevelDir);
                                ImGui.CloseCurrentPopup();
                            }

                        ImGui.EndPopup();
                    }
                });
        }

        private void DrawSettingsWindow() {
            ImGuiHelper.WithCenteredWindow("Paramètres", ref _showSettingsWindow, new Vec2(425f, 435f),
                Condition.Appearing, WindowFlags.NoResize, () => {
                    ImGui.PushItemWidth(Constants.DefaultWidgetsWidth);

                    var soundVolume = PCGame.SoundVolume;
                    if (ImGui.SliderFloat("son", ref soundVolume, 0f, 1f, "%.1f", 1f))
                        PCGame.SoundVolume = soundVolume;

                    var darkUI = PCGame.DarkUI;
                    if (ImGui.Checkbox("DarkUI", ref darkUI))
                        PCGame.DarkUI = darkUI;

                    ImGui.PopItemWidth();

                    ImGui.Separator();

                    if (ImGui.BeginChild("game keys", new Vec2(400f, 265f), false, WindowFlags.Default)) {
                        for (var i = 0; i < _settingsKeys.Count; ++i) {
                            var (gkey, pressed) = _settingsKeys.ElementAt(i);

                            var (description, key) = GameKeys.GetKey(gkey);

                            ImGui.Text(description);

                            ImGui.SameLine(300f);

                            ImGui.PushID(gkey.ToString());
                            if (ImGui.Button(pressed ? "Appuiez.." : key.ToString(), new Vec2(75f, 0f))) {
                                if (!pressed) {
                                    foreach (GameKeys.GKeys gk in Enum.GetValues(typeof(GameKeys.GKeys)))
                                        _settingsKeys[gk] = false;

                                    _keyboardListener.ClearEvent();

                                    _settingsKeys[gkey] = true;

                                    _keyboardListener.OnKeyPressedEventArg += k => {
                                        GameKeys.SetKey(gkey, k);
                                        _settingsKeys[gkey] = false;
                                    };
                                }
                            }

                            ImGui.PopID();
                        }

                        ImGui.EndChild();
                    }
                });
        }

        public override void Dispose() {
            base.Dispose();

            _newLevelNameBuf.Dispose();
            _copyLevelNameBuf.Dispose();
        }
    }
}