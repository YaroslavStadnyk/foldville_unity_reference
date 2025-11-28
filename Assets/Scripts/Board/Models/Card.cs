using System;
using Core.Interfaces;
using Grid.Common;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Board.Models
{
    [Serializable] [HideReferenceObjectPicker]
    public class Card : IIdentity
    {
        public string ID { get; }
        [OdinSerialize] public readonly TileType Type = 0;

        public Card()
        {
            ID = Guid.NewGuid().ToString();
        }

        public Card(TileType type)
        {
            ID = Guid.NewGuid().ToString();
            Type = type;
        }

        public Card(Card originalCard)
        {
            ID = Guid.NewGuid().ToString();
            Type = originalCard.Type;
        }
    }
}