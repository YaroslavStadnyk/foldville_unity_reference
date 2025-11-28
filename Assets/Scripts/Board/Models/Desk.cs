using System;
using System.Collections.Generic;
using Core.Interfaces;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Board.Models
{
    [Serializable] [HideReferenceObjectPicker]
    public class Desk : IIdentity
    {
        public string ID { get; } = "";
        [OdinSerialize] public readonly bool IsInfinite = true;
        [OdinSerialize] [HideIf(nameof(IsInfinite))] public int Capacity { get; internal set; } = 36;

        [ValueDropdown("@CardConfig.Dropdown")]
        [OdinSerialize] [PropertyOrder(1)] public readonly string CardsTag;
        [NonSerialized] public readonly List<Card> Cards = new();

        public Desk()
        {
        }

        public Desk(Desk originalDesk, string newID = null)
        {
            ID = newID ?? originalDesk.ID;
            IsInfinite = originalDesk.IsInfinite;
            Capacity = originalDesk.Capacity;
            CardsTag = originalDesk.CardsTag;
        }
    }
}