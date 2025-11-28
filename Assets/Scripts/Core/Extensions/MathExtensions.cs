using UnityEngine;

namespace Core.Extensions
{
    public static class MathExtensions
    {
        public static float Lerp3(float a, float b, float c, float t)
        {
            return Lerp3Unclamped(a, b, c, Mathf.Clamp01(t));
        }

        public static float Lerp3Unclamped(float a, float b, float c, float t)
        {
            return Mathf.LerpUnclamped(b, Mathf.LerpUnclamped(a, c, t), Mathf.Abs(t - 0.5f) * 2.0f);
        }
    }
}