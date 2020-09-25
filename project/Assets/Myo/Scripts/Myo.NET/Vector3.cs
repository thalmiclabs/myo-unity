using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Thalmic.Myo
{
    public class Vector3
    {
        private readonly float[] _data;

        public Vector3()
            : this(0, 0, 0)
        { }

        public Vector3(float x, float y, float z)
        {
            _data = new float[3];
            _data[0] = x;
            _data[1] = y;
            _data[2] = z;
        }

        public float X { get { return _data[0]; } }

        public float Y { get { return _data[1]; } }

        public float Z { get { return _data[2]; } }

        public float this[uint index]
        {
            get { return _data[index]; }
        }

        public static Vector3 operator -(Vector3 vector)
        {
            return new Vector3(-vector.X, -vector.Y, -vector.Z);
        }

        public static Vector3 operator +(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(vector1.X + vector2.X,
                               vector1.Y + vector2.Y,
                               vector1.Z + vector2.Z);
        }

        public static Vector3 operator -(Vector3 vector1, Vector3 vector2)
        {
            return vector1 + (-vector2);
        }

        public static Vector3 operator *(Vector3 vector, float scalar)
        {
            return new Vector3(vector.X * scalar,
                               vector.Y * scalar,
                               vector.Z * scalar);
        }

        public static Vector3 operator *(float scalar, Vector3 vector)
        {
            return vector * scalar;
        }

        public static Vector3 operator /(Vector3 vector, float scalar)
        {
            return new Vector3(vector.X / scalar,
                               vector.Y / scalar,
                               vector.Z / scalar);
        }

        //
        // TODO compound arithmetic operators
        //

        public float Magnitude()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        //
        // TODO dot product, cross product, etc.
        //

        public override string ToString()
        {
            return String.Format("{0,6:0.00},{1,6:0.00},{2,6:0.00}", X, Y, Z);
        }
    }
}
