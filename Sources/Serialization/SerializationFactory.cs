using System.IO;
using Newtonsoft.Json;

namespace Platform_Creator_CS.Serialization {
    public static class SerializationFactory {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings {
            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            Formatting = Formatting.Indented
        };

        public static string Serialize(object o) {
            return JsonConvert.SerializeObject(o, Formatting.Indented, JsonSettings);
        }

        public static T Deserialize<T>(string json) {
            return (T) JsonConvert.DeserializeObject(json, typeof(T), JsonSettings);
        }

        public static T Copy<T>(T copy) {
            return Deserialize<T>(Serialize(copy));
        }

        public static void SerializeToFile(object o, string file) {
            using (var fs = new StreamWriter(file, false)) {
                fs.Write(Serialize(o));
            }
        }

        public static T DeserializeFromFile<T>(string file) {
            using (var fs = new StreamReader(file)) {
                return Deserialize<T>(fs.ReadToEnd());
            }
        }
    }
}