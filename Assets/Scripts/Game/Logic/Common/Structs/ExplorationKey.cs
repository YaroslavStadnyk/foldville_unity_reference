using System;
using Game.Logic.Common.Enums;
using Grid.Common;
using Sirenix.OdinInspector;

namespace Game.Logic.Common.Structs
{
    [Serializable]
    public struct ExplorationKey
    {
        [ReadOnly] public string playerID;
        [ReadOnly] public TileType type;

        public ExplorationKey(string playerID, TileType type)
        {
            this.playerID = playerID;
            this.type = type;
        }
    }
}