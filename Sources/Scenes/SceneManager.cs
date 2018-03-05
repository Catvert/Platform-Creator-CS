using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platform_Creator_CS.Utilities;
using Platform_Creator_CS.Utility;
using IUpdateable = Platform_Creator_CS.Utility.IUpdateable;

namespace Platform_Creator_CS.Scenes {
    public sealed class SceneManager : IRenderable, IUpdateable, IResizable, IDisposable {
        private readonly Dictionary<Type, NextWaitingScene> _waitingScenes = new Dictionary<Type, NextWaitingScene>();

        private float _elapsedTime;

        private bool _isTransitionRuning;

        private NextWaitingScene _nextScene;

        private float _progress;

        public SceneManager(Scene initialScene) {
            CurrentScene = initialScene;
        }

        public Scene CurrentScene { get; private set; }

        public void Dispose() {
            CurrentScene.Dispose();
        }

        public void Render(SpriteBatch batch, float alpha) {
            PCGame.Graphics.GraphicsDevice.Clear(_nextScene?.Scene.BackgroundColor ?? CurrentScene.BackgroundColor);

            ImGui.NewFrame();

            ImGui.GetStyle().Alpha = 1f - _progress;

            CurrentScene.Render(batch, 1f - _progress);

            PCGame.ImGuiMG.Draw();

            if (_nextScene != null) {
                ImGui.NewFrame();

                ImGui.GetStyle().Alpha = _progress;

                _nextScene.Scene.Render(batch, _progress);

                PCGame.ImGuiMG.Draw();
            }
        }

        public void Resize(Size newSize) {
            CurrentScene.Resize(newSize);
            _nextScene?.Scene.Resize(newSize);
        }

        public void Update(GameTime gameTime) {
            if (_nextScene != null && _isTransitionRuning) {
                _elapsedTime += gameTime.ElapsedGameTime.Milliseconds / 1000f;

                var nextScene = _nextScene.Scene;

                _progress = Math.Min(1f, _elapsedTime);

                nextScene.Update(gameTime);

                if (_progress == 1f) {
                    SetScene(nextScene, _nextScene.DisposeCurrentScene);

                    _nextScene = null;
                    _isTransitionRuning = false;

                    if (_waitingScenes.Count > 0) {
                        var scene = _waitingScenes.ElementAt(0);
                        _waitingScenes.Remove(scene.Key);
                        LoadScene(scene.Value.Scene, scene.Value.ApplyTransition, scene.Value.DisposeCurrentScene);
                    }
                    else {
                        _progress = 0f;
                    }
                }
            }
            else {
                CurrentScene.Update(gameTime);
            }
        }

        public void LoadScene(Scene scene, bool applyTransition, bool disposeCurrentScene) {
            var nextWaitingScene = new NextWaitingScene(scene, applyTransition, disposeCurrentScene);

            if (_isTransitionRuning) _waitingScenes[scene.GetType()] = nextWaitingScene;

            if (applyTransition) {
                if (_isTransitionRuning)
                    return;

                _elapsedTime = 0f;

                _progress = 0f;

                _nextScene = nextWaitingScene;

                _isTransitionRuning = true;
            }
            else {
                if (!_isTransitionRuning)
                    SetScene(scene, disposeCurrentScene);
            }
        }

        private void SetScene(Scene scene, bool disposeCurrentScene) {
            if (disposeCurrentScene)
                CurrentScene.Dispose();

            Log.Info($"Chargement de la scène : {scene.GetType().Name}");
            CurrentScene = scene;
        }

        private class NextWaitingScene {
            public NextWaitingScene(Scene scene, bool applyTransition, bool disposeCurrentScene) {
                Scene = scene;
                ApplyTransition = applyTransition;
                DisposeCurrentScene = disposeCurrentScene;
            }

            public Scene Scene { get; }
            public bool ApplyTransition { get; }
            public bool DisposeCurrentScene { get; }
        }
    }
}