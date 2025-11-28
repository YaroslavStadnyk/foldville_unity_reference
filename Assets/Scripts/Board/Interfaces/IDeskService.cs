using System.Collections.Generic;
using Board.Models;
using Board.Structs;
using Core.Managers;

namespace Board.Interfaces
{
    public interface IDeskService : IService
    {
        public Desk CreateDesk(string deskName);
        public bool RemoveDesk(string deskID);
        public void RemoveAllDesks();

        public Desk GetCreatedDesk(string deskID);
        public List<Desk> GetCreatedDesks();

        public bool TryReadCard(string deskID, out CardInfo cardInfo);
        public Card TakeCard(string deskID);
        public bool PutCard(string deskID, Card card);
    }
}