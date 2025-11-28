using System.Collections.Generic;
using Core.Interfaces;
using Sirenix.OdinInspector;

namespace Board.Models
{
    public class Hand : IIdentity
    {
        public string ID { get; }
        [ReadOnly] public readonly Dictionary<string, Card> Cards = new();

        public Hand(string id)
        {
            ID = id;
        }
    }
}