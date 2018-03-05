using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeonBit.UI;
using GeonBit.UI.Entities;
using GeonBit.UI.Utils;
using Microsoft.Xna.Framework;
using Platform_Creator_CS.Sources.Entities.Containers;
using Platform_Creator_CS.Sources.UI;
using Platform_Creator_CS.Utilities;
using Platform_Creator_CS.Utility;
using Entity = GeonBit.UI.Entities.Entity;

namespace Platform_Creator_CS.Sources.Scenes {
    public sealed class MainMenuScene : Scene {
        private readonly List<DirectoryInfo> _levelDirs =
            Directory.GetDirectories(Constants.LevelsDir, "*", SearchOption.TopDirectoryOnly)
                .Where(dir => File.Exists(dir + $"/{Constants.LevelDataFile}")).Select(str => new DirectoryInfo(str))
                .ToList();

        private readonly KeyboardListener _keyboardListener = new KeyboardListener();

        private readonly List<Button> _dynamicBtnLevelList = new List<Button>();

        public MainMenuScene() : base(PCGame.MainBackground) {
            PCGame.AddLogoMenu(EntityContainer);

            ShowMainMenu();
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            _keyboardListener.Update(gameTime);
        }

        private void ShowMainMenu() {
            var panel = new Panel(new Vector2(400, 300), PanelSkin.None);

            panel.AddChild(new Button("Jouer", ButtonSkin.Fancy).Apply((
                (ref Button btn) => { btn.OnClick = entity => ShowSelectLevel(); })));

            panel.AddChild(new Button("Options").Apply((
                (ref Button btn) => { btn.OnClick = entity => ShowSettings(); })));

            panel.AddChild(new Button("Quitter").Apply((
                (ref Button btn) => { btn.OnClick = entity => PCGame.Exit = true; })));

            UserInterface.Active.AddEntity(panel);
        }

        private void ShowErrorLevel(string level) => MessageBox.ShowMsgBox("Erreur de chargement",
            $"Une erreur s'est produite lors du chargement de {level} !", "Fermer");

        private void UpdateDynamicBtnDisabled(SelectList list) {
            foreach (var button in _dynamicBtnLevelList) {
                button.Disabled = list.SelectedIndex == -1;
            }
        }

        private void ShowSelectLevel() {
            var panel = new Panel(new Vector2(675, 650), PanelSkin.Fancy);
            UserInterface.Active.AddEntity(panel);


            panel.AddChild(new Header("Sélection d'un niveau"));
            panel.AddChild(new HorizontalLine());

            var panelGroup = new Panel(new Vector2(0, 0), PanelSkin.None, Anchor.TopCenter);

            panel.AddChild(panelGroup);

            var leftPanel = new Panel(new Vector2(0.5f, 0), PanelSkin.None, Anchor.TopLeft);
            panelGroup.AddChild(leftPanel);

            var rightPanel = new Panel(new Vector2(0.5f, 0), PanelSkin.None, Anchor.TopRight);
            panelGroup.AddChild(rightPanel);

            var bottomPanel = new Panel(new Vector2(0f, 100f), PanelSkin.None, Anchor.BottomCenter);
            panelGroup.AddChild(bottomPanel);

            var levelList = (SelectList) leftPanel.AddChild(new SelectList(new Vector2(0, 400)).Apply(
                (ref SelectList list) => {
                    foreach (var level in _levelDirs) {
                        list.AddItem(level.Name);
                    }
                }));
            levelList.OnListChange += entity => UpdateDynamicBtnDisabled(levelList);
            levelList.OnValueChange += entity => UpdateDynamicBtnDisabled(levelList);

            rightPanel.AddChild(new Button("Jouer").Apply((ref Button btn) => {
                _dynamicBtnLevelList.Add(btn);

                btn.OnClick = entity => {
                    if (levelList.SelectedIndex == -1) return;

                    var levelDir = _levelDirs[levelList.SelectedIndex];

                    if (Level.LoadFromFile(levelDir.FullName, out var level))
                        PCGame.SceneManager.LoadScene(new GameScene(level), true, true);
                    else
                        ShowErrorLevel(levelDir.Name);
                };
            }));

            rightPanel.AddChild(new Button("Éditer").Apply((ref Button btn) => {
                _dynamicBtnLevelList.Add(btn);

                btn.OnClick = entity => {
                    if (levelList.SelectedIndex == -1) return;

                    var levelDir = _levelDirs[levelList.SelectedIndex];

                    if (Level.LoadFromFile(levelDir.FullName, out var level))
                        PCGame.SceneManager.LoadScene(new EditorScene(level, false), true, true);
                    else
                        ShowErrorLevel(levelDir.Name);
                };
            }));

            rightPanel.AddChild(new Button("Copier", anchor: Anchor.AutoInline).Apply((ref Button btn) => {
                _dynamicBtnLevelList.Add(btn);

                btn.OnClick = entity => {
                    if (levelList.SelectedIndex == -1) return;

                    var textInput = new TextInput(false) {PlaceholderText = "Nom du niveau"};

                    MessageBox.ShowMsgBox(
                        "Copier un niveau",
                        "Nouveau nom",
                        new[] {
                            new MessageBox.MsgBoxOption("Copier", () => {
                                if (!string.IsNullOrWhiteSpace(textInput.Value)) {
                                    var levelDir = _levelDirs[levelList.SelectedIndex];
                                    var copyLevelDir = new DirectoryInfo(Constants.LevelsDir + textInput.Value);

                                    Utility.Utility.CopyDirectoryRecursively(levelDir, copyLevelDir);

                                    _levelDirs.Add(copyLevelDir);
                                    levelList.AddItem(copyLevelDir.Name);

                                    return true;
                                }

                                return false;
                            }),
                            new MessageBox.MsgBoxOption("Fermer", () => true)
                        }, new Entity[] {
                            textInput
                        }, new Vector2(450, 300));
                };
            }));

            rightPanel.AddChild(new Button("Supprimer", anchor: Anchor.AutoInline).Apply((ref Button btn) => {
                _dynamicBtnLevelList.Add(btn);

                btn.OnClick = entity => {
                    if (levelList.SelectedIndex == -1) return;

                    var selectedIndex = levelList.SelectedIndex;

                    Directory.Delete(_levelDirs[selectedIndex].FullName, true);
                    _levelDirs.RemoveAt(selectedIndex);
                    levelList.RemoveItem(selectedIndex);
                };
            }));

            rightPanel.AddChild(new HorizontalLine());

            rightPanel.AddChild(new Button("Nouveau").Apply((ref Button btn) => btn.OnClick = entity => {
                var textInput = new TextInput(false) {PlaceholderText = "Nom du niveau"};

                MessageBox.ShowMsgBox(
                    "Nouveau niveau",
                    "Entrer un nom",
                    new[] {
                        new MessageBox.MsgBoxOption("Créer !", () => {
                            if (!string.IsNullOrWhiteSpace(textInput.Value))
                                PCGame.SceneManager.LoadScene(
                                    new EditorScene(Level.NewLevel(textInput.Value), false), true, true);
                            return false;
                        }),
                        new MessageBox.MsgBoxOption("Fermer", () => true)
                    }, new Entity[] {
                        textInput
                    }, new Vector2(450, 300));
            }));

            bottomPanel.AddChild(new Button("Fermer").Apply((ref Button btn) =>
                btn.OnClick = entity => panel.RemoveFromParent()));

            UpdateDynamicBtnDisabled(levelList);
        }

        private void ShowSettings() {
            _keyboardListener.ClearEvent();

            var panel = new Panel(new Vector2(650, 650), PanelSkin.Fancy);

            panel.AddChild(new Header("Paramètres du jeu"));
            panel.AddChild(new HorizontalLine());

            var settingsPanel = new Panel(new Vector2(600, 100), PanelSkin.None, Anchor.Auto);

            settingsPanel.AddChild(new Paragraph("Volume audio"));
            settingsPanel.AddChild(new Slider(0, 100).Apply((ref Slider slider) => {
                slider.Value = (PCGame.SoundVolume * 100f).Round();
                slider.OnValueChange += entity => PCGame.SoundVolume = ((Slider) entity).Value / 100f;
            }));

            panel.AddChild(settingsPanel);

            var settingsKeysPanel =
                new Panel(new Vector2(600, 350), PanelSkin.None, Anchor.Auto) {
                    PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll
                };

            foreach (GameKeys.GKeys gk in Enum.GetValues(typeof(GameKeys.GKeys))) {
                var (description, key) = GameKeys.GetKey(gk);

                settingsKeysPanel.AddChild(new Paragraph(description));

                var btn = new Button(key.ToString()) {
                    ToggleMode = true,
                    OnClick = entity => {
                        foreach (var child in settingsKeysPanel.GetChildren()) {
                            if (!ReferenceEquals(child, entity) && child is Button b) {
                                b.Checked = false;
                            }
                        }
                    }
                };

                _keyboardListener.OnKeyPressedEventArg += k => {
                    if (btn.Checked) {
                        GameKeys.SetKey(gk, k);
                        btn.ButtonParagraph.Text = k.ToString();
                    }
                };

                settingsKeysPanel.AddChild(btn);
            }

            panel.AddChild(settingsKeysPanel);

            panel.AddChild(new Button("Fermer").Apply((ref Button btn) =>
                btn.OnClick = entity => panel.RemoveFromParent()));

            UserInterface.Active.AddEntity(panel);
        }
    }
}