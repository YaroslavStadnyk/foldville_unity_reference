using System.Collections.Generic;
using Board.Models;
using Board.Structs;
using Core.Managers;

namespace Board.Interfaces
{
    public interface IHandService : IService
    {
        public Hand CreateHand(string handID);
        public bool RemoveHand(string handID);
        public void RemoveAllHands();

        public Hand GetCreatedHand(string handID);
        public List<Hand> GetCreatedHands();

        public bool TryReadCard(string handID, string cardID, out CardInfo cardInfo);
        public Card TakeCard(string handID, string cardID);
        public bool PutCard(string handID, Card card);
    }
}