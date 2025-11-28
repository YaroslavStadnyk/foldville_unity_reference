using System;

namespace MathModule.Structs
{
    [Serializable]
    public struct Int3
    {
        public bool Equals(Int3 other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is Int3 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y, z);
        }

        public int x;
        public int y;
        public int z;

        public float magnitude => (float) Math.Sqrt(x * x + y * y + z * z);

        public Int3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Int3 operator +(Int3 a, Int3 b)
        {
            return new Int3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Int3 operator -(Int3 a, Int3 b)
        {
            return new Int3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Int3 operator -(Int3 a)
        {
            return new Int3(-a.x, -a.y, -a.z);
        }

        public static Int3 operator *(Int3 a, int d)
        {
            return new Int3(a.x * d, a.y * d, a.z * d);
        }

        public static Int3 operator *(int d, Int3 a)
        {
            return new Int3(a.x * d, a.y * d, a.z * d);
        }

        public static Int3 operator /(Int3 a, int d)
        {
            return new Int3(a.x / d, a.y / d, a.z / d);
        }

        public static bool operator ==(Int3 a, Int3 b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(Int3 a, Int3 b)
        {
            return !(a == b);
        }
    }
}