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
        private static Dictionary<GKeys, (string, Keys)> _keysMap = new Dictionary<GKeys, (string, Keys)> () { 
            { GKeys.EDITOR_CAMERA_LEFT, ("[Éditeur] Déplacer la caméra vers la gauche", Keys.Q) }, 
            { GKeys.EDITOR_CAMERA_RIGHT, ("[Éditeur] Déplacer la caméra vers la droite", Keys.D) },
            { GKeys.EDITOR_CAMERA_UP, ("[Éditeur] Déplacer la caméra vers le haut", Keys.Z) },
            { GKeys.EDITOR_CAMERA_DOWN, ("[Éditeur] Déplacer la caméra vers le bas", Keys.S) },
            { GKeys.EDITOR_REMOVE_ENTITY, ("[Éditeur] Supprimer une entité", Keys.Back) },
            { GKeys.EDITOR_COPY_MODE, ("[Éditeur] Copier une entité", Keys.C) },
            { GKeys.EDITOR_GRID_MODE, ("[Éditeur] Mode grille", Keys.G) },
            { GKeys.EDITOR_APPEND_SELECT_ENTITIES, ("[Éditeur] Ajouter une entité sélectionnée", Keys.LeftControl) },
            { GKeys.EDITOR_SMARTCOPY_LEFT, ("[Éditeur] Copie intelligente à gauche", Keys.Left) },
            { GKeys.EDITOR_SMARTCOPY_RIGHT, ("[Éditeur] Copie intelligente à droite", Keys.Right) },
            { GKeys.EDITOR_SMARTCOPY_UP, ("[Éditeur] Copie intelligente au-dessus", Keys.Up) },
            { GKeys.EDITOR_SMARTCOPY_DOWN, ("[Éditeur] Copie intelligente en-dessous", Keys.Down) },
            { GKeys.EDITOR_FLIPX, ("[Éditeur] Miroir X (flip)", Keys.X) },
            { GKeys.EDITOR_FLIPY, ("[Éditeur] Miroir Y (flip)", Keys.Y) },
            { GKeys.EDITOR_UP_LAYER, ("[Éditeur] Couche +", Keys.OemPlus) },
            { GKeys.EDITOR_DOWN_LAYER, ("[Éditeur] Couche -", Keys.OemMinus) },
            { GKeys.EDITOR_SWITCH_RESIZE_MODE, ("[Éditeur] Changer le redimensionnement", Keys.R) },
            { GKeys.EDITOR_MOVE_ENTITY_LEFT, ("[Éditeur] Déplacer l'entité vers la gauche", Keys.Left) },
            { GKeys.EDITOR_MOVE_ENTITY_RIGHT, ("[Éditeur] Déplacer l'entité vers la droite", Keys.Right) },
            { GKeys.EDITOR_MOVE_ENTITY_UP, ("[Éditeur] Déplacer l'entité vers le haut", Keys.Up) },
            { GKeys.EDITOR_MOVE_ENTITY_DOWN, ("[Éditeur] Déplacer l'entité vers le bas", Keys.Down) },
            { GKeys.EDITOR_ATLAS_PREVIOUS_FRAME, ("[Éditeur] Atlas région précédente", Keys.Left) },
            { GKeys.EDITOR_ATLAS_NEXT_FRAME, ("[Éditeur] Atlas région suivante", Keys.Right) },
            { GKeys.EDITOR_TRY_LEVEL, ("[Éditeur] Essayer le niveau", Keys.F2) },
            { GKeys.GAME_PLAYER_LEFT, ("[Jeu] Déplacer le joueur vers le gauche", Keys.Q) },
            { GKeys.GAME_PLAYER_RIGHT, ("[Jeu] Déplacer le joueur vers la droite", Keys.D) },
            { GKeys.GAME_PLAYER_JUMP, ("[Jeu] Faire sauter le joueur", Keys.Space) },
            { GKeys.GAME_PLAYER_GOD_UP, ("[Jeu] Déplacer le joueur vers le haut", Keys.Z) },
            { GKeys.GAME_PLAYER_GOD_DOWN, ("[Jeu] Déplacer le joueur vers le bas", Keys.S) },
            { GKeys.GAME_EDIT_LEVEL, ("[Jeu] Éditer le niveau", Keys.F2) },
            { GKeys.CAMERA_ZOOM_UP, ("[Caméra] Zoom +", Keys.P) },
            { GKeys.CAMERA_ZOOM_DOWN, ("[Caméra] Zoom -", Keys.M) },
            { GKeys.CAMERA_ZOOM_RESET, ("[Caméra] Zoom réinitialiser", Keys.L) },
            { GKeys.DEBUG_MODE, ("Mode débug", Keys.F12) }
        };

        public enum GKeys {
            EDITOR_CAMERA_LEFT,
            EDITOR_CAMERA_RIGHT,
            EDITOR_CAMERA_UP,
            EDITOR_CAMERA_DOWN,
            EDITOR_REMOVE_ENTITY,
            EDITOR_COPY_MODE,
            EDITOR_GRID_MODE,
            EDITOR_APPEND_SELECT_ENTITIES,
            EDITOR_SMARTCOPY_LEFT,
            EDITOR_SMARTCOPY_RIGHT,
            EDITOR_SMARTCOPY_UP,
            EDITOR_SMARTCOPY_DOWN,
            EDITOR_FLIPX,
            EDITOR_FLIPY,
            EDITOR_UP_LAYER,
            EDITOR_DOWN_LAYER,
            EDITOR_SWITCH_RESIZE_MODE,
            EDITOR_MOVE_ENTITY_LEFT,
            EDITOR_MOVE_ENTITY_RIGHT,
            EDITOR_MOVE_ENTITY_UP,
            EDITOR_MOVE_ENTITY_DOWN,
            EDITOR_ATLAS_PREVIOUS_FRAME,
            EDITOR_ATLAS_NEXT_FRAME,
            EDITOR_TRY_LEVEL,
            GAME_PLAYER_LEFT,
            GAME_PLAYER_RIGHT,
            GAME_PLAYER_JUMP,
            GAME_PLAYER_GOD_UP,
            GAME_PLAYER_GOD_DOWN,
            GAME_EDIT_LEVEL,
            CAMERA_ZOOM_UP,
            CAMERA_ZOOM_DOWN,
            CAMERA_ZOOM_RESET,
            DEBUG_MODE
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