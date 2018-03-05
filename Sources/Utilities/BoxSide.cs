using System;

namespace Platform_Creator_CS.Utilities {
    public enum BoxSide {
        Left,
        Right,
        Up,
        Down,
        All
    }

    public static class BoxSideExtension {
        public static BoxSide Inversed(this BoxSide side) {
            switch (side) {
                case BoxSide.Left: return BoxSide.Right;
                case BoxSide.Right: return BoxSide.Left;
                case BoxSide.Up: return BoxSide.Down;
                case BoxSide.Down: return BoxSide.Up;
                case BoxSide.All: return BoxSide.All;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, "Impossible d'inverser ce side");
            }
        }
    }
}