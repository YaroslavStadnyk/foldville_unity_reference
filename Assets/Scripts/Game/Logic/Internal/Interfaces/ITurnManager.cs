using System.Collections.Generic;
using Game.Logic.Common.Structs;
using MathModule.Structs;

namespace Game.Logic.Internal.Interfaces
{
    public interface ITurnManager : IBase
    {
        public ITurnManagerPreset Preset { get; set; }

        public Turn CurrentTurn { get; }

        public IReadOnlyList<string> PlayersQueue { get; }
        public void Register(string playerID);
        public void Unregister(string playerID);

        public int SecondsPerTurn { get; }
        public int? CustomSecondsPerTurn { set; }
        public void PassTurn(string playerID);
        public void PassTurnToNext();

        public void ApplyCard(string cardID, Int2 indexPosition);
        public void ApplyDesk(string deskID);
        public void ApplyBuildingAttack(AttackCoords attackCoords);
    }
}