using System;
using MathModule.Structs;
using Sirenix.OdinInspector;

namespace Game.Logic.Common.Structs
{
    [Serializable]
    public struct AttackCoords
    {
        [ReadOnly] public Int2 indexPosition;
        [ReadOnly] public Int2[] specialIndexPositions;

        public AttackCoords(Int2 indexPosition, params Int2[] specialIndexPositions)
        {
            this.indexPosition = indexPosition;
            this.specialIndexPositions = specialIndexPositions;
        }
    }
}