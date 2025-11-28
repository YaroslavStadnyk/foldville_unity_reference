using System;
using Game.Logic.Common.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Logic.Common.Structs
{
    [Serializable]
    public struct PartyPlayerStats
    {
        [ReadOnly] public bool isReady;
        [ReadOnly] public PartyPlayerState state;
        // [ReadOnly] public int rate;
        [Space] [HideLabel] [ReadOnly] public PartyPlayerPerformance performance;
    }
}