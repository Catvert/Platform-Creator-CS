using System;
using System.IO;

namespace Platform_Creator_CS {
    public static class Log {
        private static TextWriter _fs = File.CreateText("last_log.txt");

        private static string Suffix() => DateTime.Now.ToLongTimeString();

        public static void Info(string message) {
            Write("INFO", message);
        }

        public static void Warn(string message) {
            Write("WARN", message);
        }

        public static void Error(string message) {
            Write("ERROR", message);
        }

        public static void Error(Exception error, string message) {
            Write("ERROR", $"{message} : {error}");
        }

        private static void Write(string type, string message) {
            var msg = $"{Suffix()} [{type}] -> {message}";

            _fs.WriteLine(msg);
            Console.WriteLine(msg);
        }
        public static void Dispose() {
            _fs.Dispose();
        }
    }
}