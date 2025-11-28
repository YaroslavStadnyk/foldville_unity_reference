using System;
using MathModule.Structs;
using Sirenix.OdinInspector;

namespace Game.Logic.Common.Structs
{
    [Serializable]
    public struct AttackPoint
    {
        [ReadOnly] public Int2 indexPosition;
        [ReadOnly] public bool isAvailable;
        [ReadOnly] public bool isReasonable;
    }
}