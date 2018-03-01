using Newtonsoft.Json;

namespace Platform_Creator_CS.Serialization {
    public static class SerializationFactory {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings() {

        };

        public static string Serialize(object o) {
            return JsonConvert.SerializeObject(o, Formatting.Indented, JsonSettings);
        }

        public static T Deserialize<T>(string json) {
            return (T) JsonConvert.DeserializeObject(json, typeof(T), JsonSettings);
        }
    }
}