using Board.Models;
using Board.Structs;
using Core.Ordinaries;

namespace Board
{
    public class BoardEvents : Singleton<BoardEvents>
    {
        public delegate void CardCreated(CardInfo card);
        public delegate void DeskCreated(DeskInfo desk);
        public delegate void DeskChanged(DeskInfo desk);
        public delegate void DeskRemoved(DeskInfo desk);
        public delegate void HandCreated(HandInfo hand);
        public delegate void HandChanged(HandInfo hand);
        public delegate void HandRemoved(HandInfo hand);

        public CardCreated OnCardCreated;
        public DeskCreated OnDeskCreated;
        public DeskChanged OnDeskChanged;
        public DeskRemoved OnDeskRemoved;
        public HandCreated OnHandCreated;
        public HandChanged OnHandChanged;
        public HandRemoved OnHandRemoved;
    }
}