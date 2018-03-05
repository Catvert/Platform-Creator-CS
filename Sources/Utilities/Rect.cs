using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Platform_Creator_CS.Utility {
    public class Rect {
        public delegate void OnChanged(Rect rect);

        private Point _position;
        private Size _size;

        [JsonConstructor]
        public Rect(Point pos, Size size) {
            _position = pos;
            _size = size;
        }

        public Rect(float x, float y, int width, int height) {
            _position = new Point(x, y);
            _size = new Size(width, height);
        }

        public Rect(Rect r) {
            _position = r.Position;
            _size = r.Size;
        }

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

        public float X {
            get => _position.X;
            set {
                _position.X = value;
                OnChangedEventHandler?.Invoke(this);
            }
        }

        public float Y {
            get => _position.Y;
            set {
                _position.Y = value;
                OnChangedEventHandler?.Invoke(this);
            }
        }

        public int Width {
            get => _size.Width;
            set {
                _size.Width = value;
                OnChangedEventHandler?.Invoke(this);
            }
        }

        public int Height {
            get => _size.Height;
            set {
                _size.Height = value;
                OnChangedEventHandler?.Invoke(this);
            }
        }

        public event OnChanged OnChangedEventHandler;

        public float Left() {
            return X;
        }

        public float Right() {
            return X + Width;
        }

        public float Bottom() {
            return Y + Height;
        }

        public float Top() {
            return Y;
        }

        public Point Center() {
            return new Point(X + Width / 2f, Y + Height / 2f);
        }

        public Rectangle ToRectangle() {
            return new Rectangle(_position.X.Round(), _position.Y.Round(), _size.Width,
                _size.Height);
        }

        public void Move(float moveX, float moveY) {
            X += moveX;
            Y += moveY;
        }

        public bool Overlaps(Rect rect) {
            return Left() < rect.Right() && Right() > rect.Left() && Bottom() > rect.Top() &&
                   Top() < rect.Bottom();
        }

        public bool Contains(Rect rect, bool borderless) {
            if (borderless)
                return rect.Left() >= Left() && rect.Right() <= Right() && rect.Bottom() <= Bottom() &&
                       rect.Top() >= Top();

            return rect.Left() > Left() && rect.Right() < Right() && rect.Bottom() < Bottom() && rect.Top() > Top();
        }

        public bool Contains(Point point) {
            return Left() <= point.X && Right() >= point.X && Bottom() >= point.Y && Top() <= point.Y;
        }

        public Rect Merge(Rect r) {
            var minX = Math.Min(Left(), r.Left());
            var maxX = Math.Max(Right(), r.Right());

            var width = (maxX - minX).Round();

            var minY = Math.Min(Top(), r.Top());
            var maxY = Math.Max(Bottom(), r.Bottom());

            var height = (maxY - minY).Round();

            return new Rect(minX, minY, width, height);
        }
    }
}