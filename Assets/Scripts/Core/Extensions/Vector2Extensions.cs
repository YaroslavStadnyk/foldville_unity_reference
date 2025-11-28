using UnityEngine;

namespace Core.Extensions
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// Sets value to vector's axis.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="axis">Axis index of the vector.</param>
        /// <param name="value">Value to set.</param>
        /// <returns>Changed copy of the vector.</returns>
        public static Vector2 With(this Vector2 vector, int axis, float value)
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
        public static Vector2 WithX(this Vector2 vector, float x) => With(vector, 0, x);

        /// <summary>
        /// Sets value to vector's y axis.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="y">Value to set.</param>
        /// <returns>Changed copy of the vector.</returns>
        public static Vector2 WithY(this Vector2 vector, float y) => With(vector, 1, y);

        /// <summary>
        /// Sets values to vector's axes.
        /// </summary>
        /// <param name="vector">Target vector.</param>
        /// <param name="axis1">First axis index of the vector.</param>
        /// <param name="value1">First value to set.</param>
        /// <param name="axis2">Second axis index of the vector.</param>
        /// <param name="value2">Second value to set.</param>
        /// <returns>Changed copy of the vector.</returns>
        public static Vector2 With(this Vector2 vector, int axis1, float value1, int axis2, float value2)
        {
            vector[axis1] = value1;
            vector[axis2] = value2;

            return vector;
        }

                /// <summary>
        /// Inverts value of specified axis.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="axis">Target axis.</param>
        /// <returns>Vector with inverted axis value.</returns>
        public static Vector2 WithNegate(this Vector2 vector, int axis) => vector.With(axis, -vector[axis]);

        /// <summary>
        /// Inverts x axis value.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Vector with inverted axis value.</returns>
        public static Vector2 WithNegateX(this Vector2 vector) => WithNegate(vector, 0);

        /// <summary>
        /// Inverts y axis value.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Vector with inverted axis value.</returns>
        public static Vector2 WithNegateY(this Vector2 vector) => WithNegate(vector, 1);

        /// <summary>
        /// Inverts vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Inverted vector.</returns>
        public static Vector2 Negate(this Vector2 vector) => new Vector2(-vector.x, -vector.y);

        public static Vector2 Get(this Vector2 vector, int axis1, int axis2) => new Vector2(vector[axis1], vector[axis2]);
        public static Vector3 Get(this Vector2 vector, int axis1, int? axis2, int axis3) => new Vector3(vector[axis1], axis2.HasValue ? vector[axis2.Value] : 0f, vector[axis3]);

        public static Vector2 GetYX(this Vector2 vector) => Get(vector, 1, 0);
        public static Vector3 GetXZ(this Vector2 vector) => Get(vector, 0, null, 1);

        public static Vector2 Direct(this Vector2 vector, Vector2 direction)
        {
            return new Vector2(vector.x * direction.y + vector.y * direction.x, vector.y * direction.y - vector.x * direction.x);
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, Vector2 t)
        {
            return new Vector2(Mathf.Lerp(a.x, b.x, t.x), Mathf.Lerp(a.y, b.y, t.y));
        }

        public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, Vector2 t)
        {
            return new Vector2(Mathf.LerpUnclamped(a.x, b.x, t.x), Mathf.LerpUnclamped(a.y, b.y, t.y));
        }

        public static Vector2 Lerp3(Vector2 a, Vector2 b, Vector2 c, float t)
        {
            return new Vector2(MathExtensions.Lerp3(a.x, b.x, c.x, t), MathExtensions.Lerp3(a.y, b.y, c.y, t));
        }

        public static Vector2 LerpUnclamped3(Vector2 a, Vector2 b, Vector2 c, float t)
        {
            return new Vector2(MathExtensions.Lerp3Unclamped(a.x, b.x, c.x, t), MathExtensions.Lerp3Unclamped(a.y, b.y, c.y, t));
        }

        public static Vector2 Rotate(this Vector2 vector, Vector3 angles)
        {
            return Quaternion.Euler(angles) * vector;
        }

        public static Vector2 RotateAround(this Vector2 vector, Vector2 point, Vector3 angles)
        {
            return Quaternion.Euler(angles) * ((vector - point) + point);
        }
    }
}