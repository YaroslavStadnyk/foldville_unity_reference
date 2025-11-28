using System.Collections.Generic;
using Board.Structs;

namespace Game.Logic.Internal.Interfaces
{
    public interface IBoardManager : IBase
    {
        public IDictionary<string, CardInfo> Cards { get; }
        public IDictionary<string, DeskInfo> Desks { get; }
        public IDictionary<string, HandInfo> Hands { get; }
    }
}