using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platform_Creator_CS.Managers;

namespace Platform_Creator_CS.Utility {
    public static class SpriteSheetLoader {
        public static SpriteSheet Load(string file) {
            var sheet = new SpriteSheet();

            using (var reader = File.OpenText(file)) {
                while (true) {
                    var firstLine = reader.ReadLine();

                    if (reader.EndOfStream)
                        break;

                    var texturePath = string.IsNullOrEmpty(firstLine) ? reader.ReadLine() : firstLine;

                    var pageSize = reader.ReadLine();
                    var format = reader.ReadLine();
                    var filter = reader.ReadLine();
                    var repeat = reader.ReadLine();

                    var texture = ResourceManager.GetTexture($"{new FileInfo(file).Directory}/{texturePath}");

                    while (true) {
                        var nextLine = reader.ReadLine();

                        if (string.IsNullOrEmpty(nextLine))
                            break;

                        var (regionName, frame) = LoadRegion(reader, nextLine, texture);
                        sheet.Add(regionName, frame);
                    }
                }
            }

            return sheet;
        }

        private static (string, TextureRegion) LoadRegion(StreamReader reader, string regionName, Texture2D texture) {
            var rotate = reader.ReadLine();
            var xy = ReadPoint(reader.ReadLine());
            var size = ReadSize(reader.ReadLine());
            var origin = ReadPoint(reader.ReadLine());
            var offset = reader.ReadLine();
            var index = reader.ReadLine();

            return (regionName,
                new TextureRegion(texture, new Rectangle((int) xy.X, (int) xy.Y, size.Width, size.Height)));
        }

        private static bool ReadBool(string line) {
            var value = line.Trim().Split(':')[1].Trim();
            return bool.Parse(value);
        }

        private static Point ReadPoint(string line) {
            var value = line.Trim().Split(':')[1].Trim();
            var xy = value.Split(',');

            return new Point(float.Parse(xy[0].Trim()), float.Parse(xy[1].Trim()));
        }

        private static Size ReadSize(string line) {
            var value = line.Trim().Split(':')[1].Trim();
            var xy = value.Split(',');

            return new Size(int.Parse(xy[0].Trim()), int.Parse(xy[1].Trim()));
        }
    }
}