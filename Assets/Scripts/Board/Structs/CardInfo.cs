using System;
using Grid.Common;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Board.Structs
{
    [Serializable]
    public struct CardInfo
    {
        [OdinSerialize] [ReadOnly] public readonly string ID;
        [OdinSerialize] [ReadOnly] public readonly TileType Type;

        public CardInfo(string id, TileType type = default)
        {
            this.ID = id;
            this.Type = type;
        }

        #region operators

        public bool Equals(CardInfo other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is CardInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID);
        }

        public static bool operator ==(CardInfo a, CardInfo b)
        {
            return a.ID == b.ID;
        }

        public static bool operator !=(CardInfo a, CardInfo b)
        {
            return !(a == b);
        }

        #endregion
    }
}