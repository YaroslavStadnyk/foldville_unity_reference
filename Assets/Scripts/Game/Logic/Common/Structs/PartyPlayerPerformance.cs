using System;
using Game.Logic.Common.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Logic.Common.Structs
{
    [Serializable]
    public struct PartyPlayerPerformance
    {
        [ReadOnly] public int capturesCount;
        [ReadOnly] public int attacksCount;
        [ReadOnly] public int buildingsCount;
        [ReadOnly] public int spendsCount;
        [NonSerialized] [ShowInInspector] [ReadOnly] public readonly ResourceType SpendsType;
    }
}