using System;

namespace TerrariaArcRaiders.Core.WorldGen
{
    public readonly struct IntRect : IEquatable<IntRect>
    {
        public IntRect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public int Right => X + Width;
        public int Bottom => Y + Height;
        public bool IsValid => Width > 0 && Height > 0;

        public static bool TryCreate(int x, int y, int width, int height, out IntRect rect)
        {
            rect = new IntRect(x, y, width, height);
            return rect.IsValid;
        }

        public bool Equals(IntRect other) => X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;

        public override bool Equals(object obj) => obj is IntRect other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(X, Y, Width, Height);

        public static bool operator ==(IntRect left, IntRect right) => left.Equals(right);

        public static bool operator !=(IntRect left, IntRect right) => !left.Equals(right);

        public override string ToString() => $"IntRect(X={X}, Y={Y}, W={Width}, H={Height})";
    }
}
