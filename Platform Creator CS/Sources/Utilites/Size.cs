namespace Platform_Creator_CS.Utility {
    public struct Size {
        public int Width;
        public int Height;

        public Size(int width, int height) {
            Width = width;
            Height = height;
        }

        public Size(int v) : this(v, v) { }
    }
}