using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using IUpdateable = Platform_Creator_CS.Utility.IUpdateable;

namespace Platform_Creator_CS.Utilities {
    public class KeyboardListener : IUpdateable {
        public delegate void KeyPressedDelegate(Keys key);

        public event KeyPressedDelegate OnKeyPressedEventArg;

        public void Update(GameTime gameTime) {
            foreach (var key in Keyboard.GetState().GetPressedKeys())
                OnKeyPressedEventArg?.Invoke(key);
        }

        public void ClearEvent() {
            if (OnKeyPressedEventArg == null) return;

            foreach (var d in OnKeyPressedEventArg.GetInvocationList()) {
                OnKeyPressedEventArg -= (KeyPressedDelegate)d;
            }
        }
    }
}