using System;
using System.Runtime.InteropServices;

namespace OverlayApplication.Vectors
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        public float x, y, z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            Vector3 v = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
            float distance = (float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
            return distance;
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator *(Vector3 a, float value)
        {
            return new Vector3(a.x * value, a.y * value, a.z * value);
        }

        public override string ToString()
        {
            string t = $"{x} / {y} / {z}";

            return t;
        }
    }
}
