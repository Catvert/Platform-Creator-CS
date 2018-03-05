using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Platform_Creator_CS.Utility;

namespace Platform_Creator_CS {
    public static class GameKeys {
        public enum GKeys {
            EditorCameraLeft,
            EditorCameraRight,
            EditorCameraUp,
            EditorCameraDown,
            EditorRemoveEntity,
            EditorCopyMode,
            EditorGridMode,
            EditorAppendSelectEntities,
            EditorSmartcopyLeft,
            EditorSmartcopyRight,
            EditorSmartcopyUp,
            EditorSmartcopyDown,
            EditorFlipx,
            EditorFlipy,
            EditorUpLayer,
            EditorDownLayer,
            EditorSwitchResizeMode,
            EditorMoveEntityLeft,
            EditorMoveEntityRight,
            EditorMoveEntityUp,
            EditorMoveEntityDown,
            EditorAtlasPreviousFrame,
            EditorAtlasNextFrame,
            EditorTryLevel,
            GamePlayerLeft,
            GamePlayerRight,
            GamePlayerJump,
            GamePlayerGodUp,
            GamePlayerGodDown,
            GameEditLevel,
            CameraZoomUp,
            CameraZoomDown,
            CameraZoomReset,
            DebugMode
        }

        private static readonly Dictionary<GKeys, (string description, Keys key)> _keysMap = new Dictionary<GKeys, (string, Keys)> {
            {GKeys.EditorCameraLeft, ("[Éditeur] Déplacer la caméra vers la gauche", Keys.Q)},
            {GKeys.EditorCameraRight, ("[Éditeur] Déplacer la caméra vers la droite", Keys.D)},
            {GKeys.EditorCameraUp, ("[Éditeur] Déplacer la caméra vers le haut", Keys.Z)},
            {GKeys.EditorCameraDown, ("[Éditeur] Déplacer la caméra vers le bas", Keys.S)},
            {GKeys.EditorRemoveEntity, ("[Éditeur] Supprimer une entité", Keys.Back)},
            {GKeys.EditorCopyMode, ("[Éditeur] Copier une entité", Keys.C)},
            {GKeys.EditorGridMode, ("[Éditeur] Mode grille", Keys.G)},
            {GKeys.EditorAppendSelectEntities, ("[Éditeur] Ajouter une entité sélectionnée", Keys.LeftControl)},
            {GKeys.EditorSmartcopyLeft, ("[Éditeur] Copie intelligente à gauche", Keys.Left)},
            {GKeys.EditorSmartcopyRight, ("[Éditeur] Copie intelligente à droite", Keys.Right)},
            {GKeys.EditorSmartcopyUp, ("[Éditeur] Copie intelligente au-dessus", Keys.Up)},
            {GKeys.EditorSmartcopyDown, ("[Éditeur] Copie intelligente en-dessous", Keys.Down)},
            {GKeys.EditorFlipx, ("[Éditeur] Miroir X (flip)", Keys.X)},
            {GKeys.EditorFlipy, ("[Éditeur] Miroir Y (flip)", Keys.Y)},
            {GKeys.EditorUpLayer, ("[Éditeur] Couche +", Keys.OemPlus)},
            {GKeys.EditorDownLayer, ("[Éditeur] Couche -", Keys.OemMinus)},
            {GKeys.EditorSwitchResizeMode, ("[Éditeur] Changer le redimensionnement", Keys.R)},
            {GKeys.EditorMoveEntityLeft, ("[Éditeur] Déplacer l'entité vers la gauche", Keys.Left)},
            {GKeys.EditorMoveEntityRight, ("[Éditeur] Déplacer l'entité vers la droite", Keys.Right)},
            {GKeys.EditorMoveEntityUp, ("[Éditeur] Déplacer l'entité vers le haut", Keys.Up)},
            {GKeys.EditorMoveEntityDown, ("[Éditeur] Déplacer l'entité vers le bas", Keys.Down)},
            {GKeys.EditorAtlasPreviousFrame, ("[Éditeur] Atlas région précédente", Keys.Left)},
            {GKeys.EditorAtlasNextFrame, ("[Éditeur] Atlas région suivante", Keys.Right)},
            {GKeys.EditorTryLevel, ("[Éditeur] Essayer le niveau", Keys.F2)},
            {GKeys.GamePlayerLeft, ("[Jeu] Déplacer le joueur vers le gauche", Keys.Q)},
            {GKeys.GamePlayerRight, ("[Jeu] Déplacer le joueur vers la droite", Keys.D)},
            {GKeys.GamePlayerJump, ("[Jeu] Faire sauter le joueur", Keys.Space)},
            {GKeys.GamePlayerGodUp, ("[Jeu] Déplacer le joueur vers le haut", Keys.Z)},
            {GKeys.GamePlayerGodDown, ("[Jeu] Déplacer le joueur vers le bas", Keys.S)},
            {GKeys.GameEditLevel, ("[Jeu] Éditer le niveau", Keys.F2)},
            {GKeys.CameraZoomUp, ("[Caméra] Zoom +", Keys.P)},
            {GKeys.CameraZoomDown, ("[Caméra] Zoom -", Keys.M)},
            {GKeys.CameraZoomReset, ("[Caméra] Zoom réinitialiser", Keys.L)},
            {GKeys.DebugMode, ("Mode débug", Keys.F12)}
        };

        public static (string description, Keys key) GetKey(GKeys key) {
            return _keysMap[key];
        }

        public static void SetKey(GKeys key, Keys newKey) {
            _keysMap[key] = (_keysMap[key].description, newKey);
        }

        public static void Load() {
            using (var file = File.OpenText(Constants.ConfigKeysFile)) {
                using (var reader = new JsonTextReader(file)) {
                    var keysJson = (JObject) JToken.ReadFrom(reader);

                    foreach (var key in keysJson["Keys"]) {
                        var s = (GKeys) key.Value<int>("gkey");

                        _keysMap[s] = (_keysMap[s].description, (Keys) key.Value<int>("key"));
                    }
                }
            }
        }

        public static void Save() {
            using (var fileWriter = File.CreateText(Constants.ConfigKeysFile)) {
                using (var writer = new JsonTextWriter(fileWriter)) {
                    dynamic keysJson = new JObject();

                    var keysArray = new JArray();

                    foreach (var key in _keysMap)
                        keysArray.Add(
                            new JObject(new JProperty("gkey", key.Key), new JProperty("key", key.Value.key)));

                    keysJson.Keys = keysArray;

                    keysJson.WriteTo(writer);
                }
            }
        }
    }
}