using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Board.Structs
{
    [Serializable]
    public struct HandInfo
    {
        [OdinSerialize] [ReadOnly] public readonly string ID;
        [OdinSerialize] [ReadOnly] public readonly List<CardInfo> Cards;

        public HandInfo(string id)
        {
            this.ID = id;
            this.Cards = default;
        }

        public HandInfo(string id, List<CardInfo> cards)
        {
            this.ID = id;
            this.Cards = cards;
        }

        #region operators

        public bool Equals(HandInfo other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is HandInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID);
        }

        public static bool operator ==(HandInfo a, HandInfo b)
        {
            return a.ID == b.ID;
        }

        public static bool operator !=(HandInfo a, HandInfo b)
        {
            return !(a == b);
        }

        #endregion
    }
}