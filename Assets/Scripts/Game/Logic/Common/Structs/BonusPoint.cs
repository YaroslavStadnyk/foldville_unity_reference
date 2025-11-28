using System;
using Game.Logic.Common.Enums;
using MathModule.Structs;
using Sirenix.OdinInspector;

namespace Game.Logic.Common.Structs
{
    [Serializable]
    public struct BonusPoint
    {
        [ReadOnly] public InteractionType type;
        [ReadOnly] public Int2 indexPosition;
        [ReadOnly] public string captureID;
        [ReadOnly] public bool isAvailable;
        [ReadOnly] public int value;
    }
}