namespace Platform_Creator_CS.Utility {
    public class Rect {
        private Point _position;
        private Size _size;

        public Point Position { get => _position; set => _position = value; }

        public Size Size { get => _size; set => _size = value; }

        public Rect (Point pos, Size size) {
            _position = pos;
            _size = size;
        }
    }
}