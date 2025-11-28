using System.Collections.Generic;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Models;
using Game.Logic.Common.Structs;

namespace Game.Logic.Internal.Interfaces
{
    public interface IPartyManager : IBase
    {
        public IPartyManagerPreset Preset { get; set; }

        public PartyState State { get; }
        public float RemainingSeconds { get; }

        public IReadOnlyDictionary<string, PartyPlayerStats> JoinedPlayers { get; }
        public void Join(string playerID, bool isReady);
        public void Leave(string playerID);
        public void SetPlayerReady(string playerID, bool isReady);
        public void SetPlayerState(string playerID, PartyPlayerState state);
        public void SetPlayerPerformance(string playerID, PartyPlayerPerformance performance);

        public void StartGameSession();
        public void CompleteGameSession();
    }
}