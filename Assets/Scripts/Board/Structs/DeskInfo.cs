using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Board.Structs
{
    [Serializable]
    public struct DeskInfo
    {
        [OdinSerialize] [ReadOnly] public readonly string ID;
        [OdinSerialize] [ReadOnly] public readonly bool IsEmpty;

        public DeskInfo(string id, bool isEmpty = false)
        {
            this.ID = id;
            this.IsEmpty = isEmpty;
        }

        #region operators

        public bool Equals(DeskInfo other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is DeskInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID);
        }

        public static bool operator ==(DeskInfo a, DeskInfo b)
        {
            return a.ID == b.ID;
        }

        public static bool operator !=(DeskInfo a, DeskInfo b)
        {
            return !(a == b);
        }

        #endregion
    }
}