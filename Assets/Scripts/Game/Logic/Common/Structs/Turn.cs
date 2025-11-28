using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Logic.Common.Structs
{
    [Serializable]
    public struct Turn
    {
        [ReadOnly] public long expirationTime;

        [Space] [ReadOnly] public int index;
        [ReadOnly] public string playerID;

        public float RemainingSeconds => Mathf.RoundToInt(expirationTime) == -1 ? -1 : Mathf.Max(0.0f, (float)(new TimeSpan(expirationTime - DateTime.UtcNow.Ticks).TotalSeconds));

        public Turn(long expirationTime, int index, string playerID = default)
        {
            this.expirationTime = expirationTime;
            this.index = index;
            this.playerID = playerID;
        }
    }
}