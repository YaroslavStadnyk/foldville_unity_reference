using System;
using Sirenix.Serialization;

namespace Game.Logic.Common.Structs
{
    [Serializable]
    public struct CampaignLevelInfo
    {
        [NonSerialized] public int Index;
        [NonSerialized, OdinSerialize] public string Name;
        [NonSerialized, OdinSerialize] public int StarsCount;
    }
}