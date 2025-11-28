using System;
using Game.Logic.Common.Enums;
using Sirenix.OdinInspector;

namespace Game.Logic.Common.Structs
{
    [Serializable]
    public struct ResourceKey
    {
        [ReadOnly] public string holderID;
        [ReadOnly] public ResourceType type;

        public ResourceKey(string holderID, ResourceType type)
        {
            this.holderID = holderID;
            this.type = type;
        }
    }
}