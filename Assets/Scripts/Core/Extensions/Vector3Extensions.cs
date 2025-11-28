using UnityEngine;

namespace Core.Extensions
{
    public static class Vector3Extensions
    {
        /// <summary>
        /// Sets value to vector's axis.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="axis">Axis index of the vector.</param>
        /// <param name="value">Value to set.</param>
        /// <returns>Changed copy of the vector.</returns>
        public static Vector3 With(this Vector3 vector, int axis, float value)
        {
            vector[axis] = value;
            return vector;
        }

        /// <summary>
        /// Sets value to vector's x axis.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="x">Value to set.</param>
        /// <returns>Changed copy of the vector.</returns>
        public static Vector3 WithX(this Vector3 vector, float x) => With(vector, 0, x);

        /// <summary>
        /// Sets value to vector's y axis.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="y">Value to set.</param>
        /// <returns>Changed copy of the vector.</returns>
        public static Vector3 WithY(this Vector3 vector, float y) => With(vector, 1, y);

        /// <summary>
        /// Sets value to vector's z axis.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="z">Value to set.</param>
        /// <returns>Changed copy of the vector.</returns>
        public static Vector3 WithZ(this Vector3 vector, float z) => With(vector, 2, z);

        /// <summary>
        /// Sets values to vector's axes.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="axis1">First axis index of the vector.</param>
        /// <param name="value1">First value to set.</param>
        /// <param name="axis2">Second axis index of the vector.</param>
        /// <param name="value2">Second value to set.</param>
        /// <returns>Changed copy of the vector.</returns>
        public static Vector3 With(this Vector3 vector, int axis1, float value1, int axis2, float value2)
        {
            vector[axis1] = value1;
            vector[axis2] = value2;

            return vector;
        }

        /// <summary>
        /// Sets values to vector's x and y axis.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="x">Value to set.</param>
        /// <param name="y">Value to set.</param>
        /// <returns>Changed copy of the vector.</returns>
        public static Vector3 WithXY(this Vector3 vector, float x, float y) => With(vector, 0, x, 1, y);

        /// <summary>
        /// Sets value to vector's x and y axis.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="value">Value to set.</param>
        /// <returns>Changed copy of the vector.</returns>
        public static Vector3 WithXY(this Vector3 vector, Vector2 value) => With(vector, 0, value.x, 1, value.y);

        /// <summary>
        /// Sets value to vector's x and z axis.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="x">Value to set.</param>
        /// <param name="z">Value to set.</param>
        /// <returns>Changed copy of the vector.</returns>
        public static Vector3 WithXZ(this Vector3 vector, float x, float z) => With(vector, 0, x, 2, z);

        /// <summary>
        /// Sets value to vector's x and z axis.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="value">Value to set.</param>
        /// <returns>Changed copy of the vector.</returns>
        public static Vector3 WithXZ(this Vector3 vector, Vector2 value) => With(vector, 0, value.x, 2, value.y);

        /// <summary>
        /// Sets value to vector's y and z axis.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="y">Value to set.</param>
        /// <param name="z">Value to set.</param>
        /// <returns>Changed copy of the vector.</returns>
        public static Vector3 WithYZ(this Vector3 vector, float y, float z) => With(vector, 1, y, 2, z);

        /// <summary>
        /// Sets value to vector's y and z axis.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="value">Value to set.</param>
        /// <returns>Changed copy of the vector.</returns>
        public static Vector3 WithYZ(this Vector3 vector, Vector2 value) => With(vector, 1, value.x, 2, value.y);

        /// <summary>
        /// Inverts value of specified axis.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="axis">Target axis.</param>
        /// <returns>Vector with inverted axis value.</returns>
        public static Vector3 WithNegate(this Vector3 vector, int axis) => vector.With(axis, -vector[axis]);

        /// <summary>
        /// Inverts x axis value.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Vector with inverted axis value.</returns>
        public static Vector3 WithNegateX(this Vector3 vector) => WithNegate(vector, 0);

        /// <summary>
        /// Inverts y axis value.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Vector with inverted axis value.</returns>
        public static Vector3 WithNegateY(this Vector3 vector) => WithNegate(vector, 1);

        /// <summary>
        /// Inverts z axis value.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Vector with inverted axis value.</returns>
        public static Vector3 WithNegateZ(this Vector3 vector) => WithNegate(vector, 2);

        /// <summary>
        /// Inverts values of specified axes.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="axis1">First axis.</param>
        /// <param name="axis2">Second axis.</param>
        /// <returns>Vector with inverted axes values.</returns>
        public static Vector3 WithNegate(this Vector3 vector, int axis1, int axis2)
        {
            vector[axis1] = -vector[axis1];
            vector[axis2] = -vector[axis2];

            return vector;
        }

        /// <summary>
        /// Inverts x and y axes values.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Vector with inverted axes values.</returns>
        public static Vector3 WithNegateXY(this Vector3 vector) => vector.WithNegate(0, 1);

        /// <summary>
        /// Inverts x and z axes values.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Vector with inverted axes values.</returns>
        public static Vector3 WithNegateXZ(this Vector3 vector) => vector.WithNegate(0, 2);

        /// <summary>
        /// Inverts y and z axes values.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Vector with inverted axes values.</returns>
        public static Vector3 WithNegateYZ(this Vector3 vector) => vector.WithNegate(1, 2);

        /// <summary>
        /// Inverts vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Inverted vector.</returns>
        public static Vector3 Negate(this Vector3 vector) => new Vector3(-vector.x, -vector.y, -vector.z);

        /// <summary>
        /// Gets <see cref="Vector2"/> by axes.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="axis1">First axis.</param>
        /// <param name="axis2">Second axis.</param>
        /// <returns><see cref="Vector2"/> vector.</returns>
        public static Vector2 Get(this Vector3 vector, int axis1, int axis2) => new Vector3(vector[axis1], vector[axis2]);

        /// <summary>
        /// Gets <see cref="Vector2"/> by x and y axes.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <returns><see cref="Vector2"/> vector.</returns>
        public static Vector2 GetXY(this Vector3 vector) => Get(vector, 0, 1);

        /// <summary>
        /// Gets <see cref="Vector2"/> by x and z axes.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <returns><see cref="Vector2"/> vector.</returns>
        public static Vector2 GetXZ(this Vector3 vector) => Get(vector, 0, 2);

        /// <summary>
        /// Gets <see cref="Vector2"/> by y and x axes.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <returns><see cref="Vector2"/> vector.</returns>
        public static Vector2 GetYX(this Vector3 vector) => Get(vector, 1, 0);

        /// <summary>
        /// Gets <see cref="Vector2"/> by y and z axes.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <returns><see cref="Vector2"/> vector.</returns>
        public static Vector2 GetYZ(this Vector3 vector) => Get(vector, 1, 2);

        /// <summary>
        /// Gets <see cref="Vector2"/> by z and x axes.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <returns><see cref="Vector2"/> vector.</returns>
        public static Vector2 GetZX(this Vector3 vector) => Get(vector, 2, 0);

        /// <summary>
        /// Gets <see cref="Vector2"/> by z and y axes.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <returns><see cref="Vector2"/> vector.</returns>
        public static Vector2 GetZY(this Vector3 vector) => Get(vector, 2, 1);

        /// <summary>
        /// Gets vector with swapped axes.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="axis1">First axis.</param>
        /// <param name="axis2">Second axis.</param>
        /// <param name="axis2">Third axis.</param>
        /// <returns><see cref="Vector3"/> vector.</returns>
        public static Vector3 Get(this Vector3 vector, int axis1, int axis2, int axis3) => new Vector3(vector[axis1], vector[axis2], vector[axis3]);

        /// <summary>
        /// Gets vector with order XZY.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <returns><see cref="Vector3"/> vector.</returns>
        public static Vector3 GetXZY(this Vector3 vector) => Get(vector, 0, 2, 1);

        /// <summary>
        /// Gets vector with order YXZ.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <returns><see cref="Vector3"/> vector.</returns>
        public static Vector3 GetYXZ(this Vector3 vector) => Get(vector, 1, 0, 2);

        /// <summary>
        /// Gets vector with order YZX.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <returns><see cref="Vector3"/> vector.</returns>
        public static Vector3 GetYZX(this Vector3 vector) => Get(vector, 1, 2, 0);

        /// <summary>
        /// Gets vector with order ZXY.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <returns><see cref="Vector3"/> vector.</returns>
        public static Vector3 GetZXY(this Vector3 vector) => Get(vector, 2, 0, 1);

        /// <summary>
        /// Gets vector with order ZYX.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <returns><see cref="Vector3"/> vector.</returns>
        public static Vector3 GetZYX(this Vector3 vector) => Get(vector, 2, 1, 0);

        /// <summary>
        /// Inserts value to x axis and extends vector to 4-dimensional.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="x">Target vector.</param>
        /// <returns><see cref="Vector3"/> vector.</returns>
        public static Vector4 InsertX(this Vector3 vector, float x = 0) => new Vector4(x, vector.x, vector.y, vector.z);

        /// <summary>
        /// Inserts value to y axis and extends vector to 4-dimensional.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="y">Target vector.</param>
        /// <returns><see cref="Vector4"/> vector.</returns>
        public static Vector4 InsertY(this Vector3 vector, float y = 0) => new Vector4(vector.x, y, vector.y, vector.z);

        /// <summary>
        /// Inserts value to z axis and extends vector to 4-dimensional.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="z">Target vector.</param>
        /// <returns><see cref="Vector4"/> vector.</returns>
        public static Vector4 InsertZ(this Vector3 vector, float z = 0) => new Vector4(vector.x, vector.y, z, vector.z);

        /// <summary>
        /// Inserts value to w axis and extends vector to 4-dimensional.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="w">Target vector.</param>
        /// <returns><see cref="Vector4"/> vector.</returns>
        public static Vector4 InsertW(this Vector3 vector, float w = 0) => new Vector4(vector.x, vector.y, vector.z, w);

        private static void Compare(Vector3 vector, ref int index, int compareIndex, int result)
        {
            if (vector[compareIndex].CompareTo(vector[index]) == result)
                index = compareIndex;
        }

        private static int CompareAllComponents(Vector3 vector, int result)
        {
            var index = 0;

            Compare(vector, ref index, 1, result);
            Compare(vector, ref index, 2, result);

            return index;
        }

        /// <summary>
        /// Gets max component index from vector.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <returns>Vector's max component index.</returns>
        public static int MaxComponentIndex(this Vector3 vector) => CompareAllComponents(vector, 1);

        /// <summary>
        /// Gets min component from vector.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <returns>Vector's min component</returns>
        public static float MaxComponent(this Vector3 vector) => vector[MaxComponentIndex(vector)];

        /// <summary>
        /// Gets min component index from vector.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <returns>Vector's min component index.</returns>
        public static int MinComponentIndex(this Vector3 vector) => CompareAllComponents(vector, -1);

        /// <summary>
        /// Gets min component from vector.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <returns>Vector's min component</returns>
        public static float MinComponent(this Vector3 vector) => vector[MinComponentIndex(vector)];

        /// <summary>
        /// Creates new vector with absolute components.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <returns>Vector with absolute components.</returns>
        public static Vector3 Abs(this Vector3 vector) => new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));

        /// <summary>
        /// Creates new vector with clamped components.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="min">The minimum floating value to compare against.</param>
        /// <param name="max">The maximum floating value to compare against.</param>
        /// <returns>Clamped vector.</returns>
        public static Vector3 Clamp(this Vector3 vector, float min, float max)
        {
            return new Vector3(Mathf.Clamp(vector.x, min, max), Mathf.Clamp(vector.y, min, max), Mathf.Clamp(vector.z, min, max));
        }

        /// <summary>
        /// Creates and returns a vector whose components are limited to 0 and 1.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <returns>Clamped vector.</returns>
        public static Vector3 Clamp01(this Vector3 vector)
        {
            return new Vector3(Mathf.Clamp01(vector.x), Mathf.Clamp01(vector.y), Mathf.Clamp01(vector.z));
        }

        /// <summary>
        /// Creates and returns a vector whose components are divided by the value.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="other">Vector on which divide</param>
        /// <returns>Divided vector.</returns>
        public static Vector3 Divide(this Vector3 vector, Vector3 other)
        {
            return new Vector3(vector.x / other.x, vector.y / other.y, vector.z / other.z);
        }

        /// <summary>
        /// Creates and returns a vector whose components are multiplied by the value.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="other">Vector on which multiply</param>
        /// <returns>Multiplied vector.</returns>
        public static Vector3 Multiply(this Vector3 vector, Vector3 other)
        {
            return new Vector3(vector.x * other.x, vector.y * other.y, vector.z * other.z);
        }

        public static Vector3 Add(this Vector3 vector, float other)
        {
            return new Vector3(vector.x + other, vector.y + other, vector.z + other);
        }

        public static Vector3 Reminder(this Vector3 vector, Vector3 other)
        {
            return new Vector3(vector.x % other.x, vector.y % other.y, vector.z % other.z);
        }

        public static Vector3 Reminder(this Vector3 vector, float other)
        {
            return new Vector3(vector.x % other, vector.y % other, vector.z % other);
        }

        /// <summary>
        /// Calculate evently distributed point position on sphere.
        /// </summary>
        /// <param name="index">Index of point.</param>
        /// <param name="radius">Radius of the sphere.</param>
        /// <param name="count">Total count of points in sphere.</param>
        /// <returns>Calculated evently distributed point.</returns>
        public static Vector3 EventlyDistributedPointOnSphere(int index, float radius, int count)
        {
            var k = index + 0.5f;
            var phi = Mathf.Acos(1f - 2f * k / count);
            var theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * k;

            var x = Mathf.Cos(theta) * Mathf.Sin(phi);
            var y = Mathf.Sin(theta) * Mathf.Sin(phi);
            var z = Mathf.Cos(phi);

            return new Vector3(x, y, z) * radius;
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, Vector3 t)
        {
            return new Vector3(Mathf.Lerp(a.x, b.x, t.x), Mathf.Lerp(a.y, b.y, t.y), Mathf.Lerp(a.z, b.z, t.z));
        }

        public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, Vector3 t)
        {
            return new Vector3(Mathf.LerpUnclamped(a.x, b.x, t.x), Mathf.LerpUnclamped(a.y, b.y, t.y), Mathf.LerpUnclamped(a.z, b.z, t.z));
        }

        public static Vector3 Lerp3(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            return new Vector3(MathExtensions.Lerp3(a.x, b.x, c.x, t), MathExtensions.Lerp3(a.y, b.y, c.y, t), MathExtensions.Lerp3(a.z, b.z, c.z, t));
        }

        public static Vector3 LerpUnclamped3(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            return new Vector3(MathExtensions.Lerp3Unclamped(a.x, b.x, c.x, t), MathExtensions.Lerp3Unclamped(a.y, b.y, c.y, t), MathExtensions.Lerp3Unclamped(a.z, b.z, c.z, t));
        }

        public static Vector3 Rotate(this Vector3 vector, Vector3 angles)
        {
            return Quaternion.Euler(angles) * vector;
        }

        public static Vector3 RotateAround(this Vector3 vector, Vector3 point, Vector3 angles)
        {
            return Quaternion.Euler(angles) * (vector - point) + point;
        }

        public static Vector3 Ground(this Vector3 vector, Vector3 direction)
        {
            if (direction.y == 0)
            {
                return vector;
            }

            var distance = vector.y / -direction.y;
            var end = vector + direction * distance;

            return end;
        }
    }
}