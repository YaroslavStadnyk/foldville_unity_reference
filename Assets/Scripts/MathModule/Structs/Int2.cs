using System;

namespace MathModule.Structs
{
    [Serializable]
    public struct Int2
    {
        public bool Equals(Int2 other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is Int2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public int x;
        public int y;

        public static readonly Int2 zero = new(0, 0);
        public static readonly Int2 one = new(1, 1);

        public float magnitude => (float) Math.Sqrt(x * x + y * y);

        public Int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Int2 operator +(Int2 a, Int2 b)
        {
            return new Int2(a.x + b.x, a.y + b.y);
        }

        public static Int2 operator -(Int2 a, Int2 b)
        {
            return new Int2(a.x - b.x, a.y - b.y);
        }

        public static Int2 operator -(Int2 a)
        {
            return new Int2(-a.x, -a.y);
        }

        public static Int2 operator *(Int2 a, int d)
        {
            return new Int2(a.x * d, a.y * d);
        }

        public static Int2 operator *(int d, Int2 a)
        {
            return new Int2(a.x * d, a.y * d);
        }

        public static Int2 operator /(Int2 a, int d)
        {
            return new Int2(a.x / d, a.y / d);
        }

        public static bool operator ==(Int2 a, Int2 b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Int2 a, Int2 b)
        {
            return !(a == b);
        }
    }
}