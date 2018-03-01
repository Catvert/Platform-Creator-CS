using Newtonsoft.Json;

namespace Platform_Creator_CS.Utility {
    public struct GridCell {
        public int X { get; }
        public int Y { get; }

        public GridCell(int x, int y) {
            X = x;
            Y = y;
        }

        public void Deconstruct(out int x, out int y) {
            x = X;
            y = Y;
        }
    }
}