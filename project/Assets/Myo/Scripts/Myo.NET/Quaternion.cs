using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thalmic.Myo
{
    public class Quaternion
    {
        private readonly float _x;
        private readonly float _y;
        private readonly float _z;
        private readonly float _w;

        public Quaternion()
            : this(0, 0, 0, 1)
        { }

        public Quaternion(float x, float y, float z, float w)
        {
            _x = x;
            _y = y;
            _z = z;
            _w = w;
        }

        public float X { get { return _x; } }

        public float Y { get { return _y; } }

        public float Z { get { return _z; } }

        public float W { get { return _w; } }



        public static Quaternion operator -(Quaternion quat)
        {
            return new Quaternion(-quat._x, -quat._y, -quat._z, -quat._w);
        }

        public static Quaternion operator +(Quaternion quat1, Quaternion quat2)
        {
            return new Quaternion(quat1._x + quat2._x,
                                  quat1._y + quat2._y,
                                  quat1._z + quat2._z,
                                  quat1._w + quat2._w);
        }

        public static Quaternion operator -(Quaternion quat1, Quaternion quat2)
        {
            return quat1 + (-quat2);
        }

        public static Quaternion operator *(Quaternion quat, float scalar)
        {
            return new Quaternion(quat._x * scalar,
                                  quat._y * scalar,
                                  quat._z * scalar,
                                  quat._w * scalar);
        }

        public static Quaternion operator *(float scalar, Quaternion quat)
        {
            return quat * scalar;
        }

        public static Quaternion operator /(Quaternion quat, float scalar)
        {
            return new Quaternion(quat._x / scalar,
                                  quat._y / scalar,
                                  quat._z / scalar,
                                  quat._w / scalar);
        }

        public static Quaternion operator *(Quaternion quat1, Quaternion quat2)
        {
            return new Quaternion(quat1._w * quat2._x + quat1._x * quat2._w + quat1._y * quat2._z - quat1._z * quat2._y,
                                  quat1._w * quat2._y - quat1._x * quat2._z + quat1._y * quat2._w + quat1._z * quat2._x,
                                  quat1._w * quat2._z + quat1._x * quat2._y - quat1._y * quat2._x + quat1._z * quat2._w,
                                  quat1._w * quat2._w - quat1._x * quat2._x - quat1._y * quat2._y - quat1._z * quat2._z);
        }

        public static Vector3 operator *(Quaternion quat, Vector3 vec)
        {
            var qvec = new Quaternion(vec.X, vec.Y, vec.Z, 0);
            var result = quat * qvec * quat.Conjugate();
            return new Vector3(result.X, result.Y, result.Z);
        }

        //
        // TODO compound arithmetic operators
        //

        public static Quaternion Normalize(Quaternion quat)
        {
            return (quat / quat.Magnitude());
        }

        /// Return the conjugate of the given Quaternion.
        public Quaternion Conjugate()
        {
            return new Quaternion(-_x, -_y, -_z, _w);
        }

        /// Calculate the roll angle represented by the given unit Quaternion.
        public static float Roll(Quaternion quat)
        {
            return (float)Math.Atan2(2.0f * (quat._w * quat._x + quat._y * quat._z),
                                     1.0f - 2.0f * (quat._x * quat._x + quat._y * quat._y));
        }

        /// Calculate the pitch angle represented by the given unit Quaternion.
        public static float Pitch(Quaternion quat)
        {
            return (float)Math.Asin(2.0f * (quat._w * quat._y - quat._z * quat._x));
        }

        /// Calculate the yaw angle represented by the given unit Quaternion.
        public static float Yaw(Quaternion quat)
        {
            return (float)Math.Atan2(2.0f * (quat._w * quat._z + quat._x * quat._y),
                                     1.0f - 2.0f * (quat._y * quat._y + quat._z * quat._z));
        }

        public float Magnitude()
        {
            return (float)Math.Sqrt(_w * _w + _x * _x + _y * _y + _z * _z);
        }

        public override string ToString()
        {
            return String.Format("{0},{1},{2},{3}", X, Y, Z, W);
        }
    }
}
