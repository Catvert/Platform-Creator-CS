using Newtonsoft.Json;

namespace Platform_Creator_CS.Serialization {
    public static class SerializationFactory {
        private static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings() {

        };

        public static string Serialize(object o) {
            return JsonConvert.SerializeObject(o, Formatting.Indented, _jsonSettings);
        }

        public static T Deserialize<T>(string json) {
            return (T) JsonConvert.DeserializeObject(json, typeof(T), _jsonSettings);
        }
    }
}