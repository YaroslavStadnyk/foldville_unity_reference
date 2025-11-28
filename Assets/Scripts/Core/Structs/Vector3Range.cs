using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Structs
{
    [Serializable]
    public struct Vector3Range
    {
        public Vector3 min;
        public Vector3 max;

        public float randomX => Mathf.LerpUnclamped(min.x, max.x, Random.value);
        public float randomY => Mathf.LerpUnclamped(min.y, max.y, Random.value);
        public float randomZ => Mathf.LerpUnclamped(min.z, max.z, Random.value);
        public Vector3 random => new(randomX, randomY, randomZ);
    }
}