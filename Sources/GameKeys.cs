using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Platform_Creator_CS.Utility;

namespace Platform_Creator_CS {
    public static class GameKeys {
        private static Dictionary<GKeys, (string, Keys)> _keysMap = new Dictionary<GKeys, (string, Keys)> () { { GKeys.EDITOR_CAMERA_LEFT, ("[Éditeur] Déplacer la caméra vers la gauche", Keys.Q) }, { GKeys.EDITOR_CAMERA_RIGHT, ("[Éditeur] Déplacer la caméra vers la droite", Keys.D) }
        };

        public enum GKeys {
            EDITOR_CAMERA_LEFT,
            EDITOR_CAMERA_RIGHT
        }

        public static (string, Keys) GetKey (GKeys key) => _keysMap[key];

        public static void Load () {
            using (StreamReader file = File.OpenText (Constants.ConfigKeysFile)) {
                using (JsonTextReader reader = new JsonTextReader (file)) {
                    var keysJson = (JObject) JToken.ReadFrom (reader);

                    foreach (var key in keysJson["Keys"]) {
                        var s = (GKeys) key.Value<int> ("gkey");

                        _keysMap[s] = (_keysMap[s].Item1, (Keys) key.Value<int> ("key"));
                    }
                }
            }
        }

        public static void Save () {
            using (var fileWriter = File.CreateText (Constants.ConfigKeysFile)) {
                using (var writer = new JsonTextWriter (fileWriter)) {
                    dynamic keysJson = new JObject ();

                    var keysArray = new JArray ();

                    foreach (var key in _keysMap) {
                        keysArray.Add (new JObject ((new JProperty ("gkey", key.Key)), new JProperty ("key", key.Value.Item2)));
                    }

                    keysJson.Keys = keysArray;

                    keysJson.WriteTo (writer);
                }
            }
        }
    }
}