using Game.Logic.Common.Models;
using UnityEngine;

namespace Game.Logic.Internal.Interfaces
{
    public interface IPartyManagerPreset
    {
        public string LobbySceneName { get; }
        public string GameSceneName { get; }

        public Vector2Int PlayersRange { get; }
        public int PlayersWaitingSeconds { get; set; }

        public LevelLogic LevelLogic { get; }
    }
}