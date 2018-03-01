using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Platform_Creator_CS.Entities.Actions;

namespace Platform_Creator_CS.Entities.Components {
    public class InputComponent : Component, Utility.IUpdateable {
        public List<InputData> Inputs { get; } = new List<InputData>();

        public InputComponent(params InputData[] inputs) {
            Inputs.AddRange(inputs);
        }

        public void Update(GameTime gameTime) {
            foreach (var input in Inputs) {
                input.Update(Entity);
            }
        }
    }

    public class InputData {
        public Keys Key { get; set; }
        public bool Pressed { get; set; }
        public Action Action { get; set; }

        private KeyboardState _kLastState = Keyboard.GetState();

        public InputData(Keys key, bool pressed, Action action) {
            Key = key;
            Pressed = pressed;
            Action = action;
        }

        public void Update(Entity entity) {
            var kState = Keyboard.GetState();

            if (Pressed && kState.IsKeyDown(Key))
                Action.Invoke(entity);
            else if (!Pressed && kState.IsKeyDown(Key) && _kLastState.IsKeyUp(Key))
                Action.Invoke(entity);

            _kLastState = kState;
        }
    }
}