using System;
using Microsoft.Xna.Framework;

namespace Platform_Creator_CS.Utility {
    public class Rect {
        public delegate void OnChanged(Rect rect);
        
        private Point _position;
        private Size _size;

        public Point Position {
            get => _position;
            set {
                _position = value;
                OnChangedEventHandler?.Invoke(this);
            }
        }

        public Size Size {
            get => _size;
            set {
                _size = value;
                OnChangedEventHandler?.Invoke(this);
            }
        }

        public event OnChanged OnChangedEventHandler;

        public float Left() => _position.X;
        public float Right() => _position.X + _size.Width;
        public float Bottom() => _position.Y; // \!/ Inverse bottom-top
        public float Top() => _position.Y + _size.Height;
        public Point Center() => new Point(_position.X + _size.Width / 2f, _position.Y + _size.Height / 2f);

        public Rect (Point pos, Size size) {
            _position = pos;
            _size = size;
        }

        public Rect(float x, float y, int width, int height) {
            _position = new Point(x, y);
            _size = new Size(width, height);
        }

        public Rectangle ToRectangle() => new Rectangle((int)Math.Round(_position.X), (int)Math.Round(_position.Y), _size.Width, _size.Height);

        public void Move(float moveX, float moveY) {
            Position = new Point(_position.X + moveX, _position.Y + moveY);
        }

        public bool Overlaps(Rect rect) => Left() < rect.Right() && Right() > rect.Left() && Bottom() < rect.Top() &&
                                           Top() > rect.Bottom();

        public bool Contains(Rect rect, bool borderless) {
            if (borderless)
                return rect.Left() >= Left() && rect.Right() <= Right() && rect.Bottom() >= Bottom() &&
                       rect.Top() <= Top();
        
            return rect.Left() > Left() && rect.Right() < Right() && rect.Bottom() > Bottom() && rect.Top() < Top();
        }

        public bool Contains(Point point) =>
            _position.X <= point.X && Right() >= point.X && _position.Y <= point.Y && Top() >= point.Y;
    }
}