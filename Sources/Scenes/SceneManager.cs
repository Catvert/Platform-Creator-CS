using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tweening;
using Platform_Creator_CS.Utility;
using System.Linq;
using ImGuiNET;

namespace Platform_Creator_CS.Scenes {
    public class SceneManager : IRenderable, IUpdeatable, IDisposable {
        private class NextWaitingScene {
            public Scene Scene { get; }
            public bool ApplyTransition { get; }
            public bool DisposeCurrentScene { get; }

            public NextWaitingScene(Scene scene, bool applyTransition, bool disposeCurrentScene) {
                Scene = scene;
                ApplyTransition = applyTransition;
                DisposeCurrentScene = disposeCurrentScene;
            }
        }
        public Scene CurrentScene { get; private set; }

        private NextWaitingScene _nextScene = null;

        private Dictionary<Type, NextWaitingScene> _waitingScenes = new Dictionary<Type, NextWaitingScene>();

        private bool _isTransitionRuning = false;

        private float _elapsedTime = 0f;

        private float _progress = 0f;

        public SceneManager(Scene initialScene) {
            CurrentScene = initialScene;
        }

        public void LoadScene(Scene scene, bool applyTransition, bool disposeCurrentScene) {
            var nextWaitingScene = new NextWaitingScene(scene, applyTransition, disposeCurrentScene);

            if (_isTransitionRuning) {
                _waitingScenes[scene.GetType()] = nextWaitingScene;
            }

            if (applyTransition) {
                if(_isTransitionRuning)
                    return;

                _elapsedTime = 0f;

                _progress = 0f;

                _nextScene = nextWaitingScene;

                _isTransitionRuning = true;
            } else {
                if(!_isTransitionRuning)
                    SetScene(scene, disposeCurrentScene);
            }
        }

        private void SetScene(Scene scene, bool disposeCurrentScene) {
            if(disposeCurrentScene)
                CurrentScene.Dispose();

            Log.Info($"Chargement de la scène : {scene.GetType().Name}");
            CurrentScene = scene;
        }

        public void Render(SpriteBatch batch, float alpha) {
            PCGame.Graphics.GraphicsDevice.Clear(_nextScene != null ? _nextScene.Scene.BackgroundColor : CurrentScene.BackgroundColor);

            ImGui.NewFrame();

            ImGui.GetStyle().Alpha = 1f - _progress;

            CurrentScene.Render(batch, 1f - _progress);

            PCGame.ImGuiMG.Draw();

            if(_nextScene != null) {
                ImGui.NewFrame();

                ImGui.GetStyle().Alpha = _progress;

                _nextScene.Scene.Render(batch, _progress);

                PCGame.ImGuiMG.Draw();
            }
        }

        public void Update(GameTime gameTime) {
            if(_nextScene != null && _isTransitionRuning) {
                _elapsedTime += gameTime.ElapsedGameTime.Milliseconds / 1000f;

                var nextScene = _nextScene.Scene;

                _progress = Math.Min(1f, _elapsedTime);

                nextScene.Update(gameTime);

                if(_progress == 1f) {
                    SetScene(nextScene, _nextScene.DisposeCurrentScene);

                    _nextScene = null;
                    _isTransitionRuning = false;

                    if(_waitingScenes.Count > 0) {
                        var scene = _waitingScenes.ElementAt(0);
                        _waitingScenes.Remove(scene.Key);
                        LoadScene(scene.Value.Scene, scene.Value.ApplyTransition, scene.Value.DisposeCurrentScene);
                    }
                    else
                        _progress = 0f;
                }
            }
            else
                CurrentScene.Update(gameTime);
        }

        public void Dispose() {
            CurrentScene.Dispose();
        }
    }
}