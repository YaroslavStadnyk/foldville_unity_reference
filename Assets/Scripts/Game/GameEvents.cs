using System.Collections.Generic;
using Board.Structs;
using Core.Ordinaries;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;
using Grid.Common;
using MathModule.Structs;

namespace Game
{
    /// It contains all the synchronized events of the game.
    public class GameEvents : Singleton<GameEvents>
    {
        public delegate void PartyStateChanged(PartyState oldPartyState, PartyState newPartyState);
        public delegate void PartyPlayerJoined(string playerID);
        public delegate void PartyPlayerLeaved(string playerID);
        public delegate void PartyPlayerReadyChanged(string playerID, bool isReady);
        public delegate void PartyPlayerStateChanged(string playerID, PartyPlayerState oldPlayerState, PartyPlayerState newPlayerState);

        public delegate void TurnSecondsChanged(string playerID, float remainingSeconds);
        public delegate void TurnChanged(Turn oldTurn, Turn newTurn);
        public delegate void CardOwnersChanged(OperationType operationType, CardInfo cardInfo, string oldOwnerID, string newOwnerID);

        public delegate void CardApplied(HandInfo handInfo, CardInfo cardInfo, Int2 indexPosition, PlayerErrorType errorType);
        public delegate void DeskApplied(HandInfo handInfo, DeskInfo deskInfo, CardInfo takenCardInfo, PlayerErrorType errorType);
        //public delegate void BuildingAttackApplied(PlayerErrorType errorType);
        public delegate void FactionExplored(HandInfo handInfo, CardInfo exploredCardInfo, PlayerErrorType errorType);

        public delegate void ResourceChanged(OperationType operationType, ResourceKey resourceKey, int oldValue, int newValue);
        public delegate void TileTypeChanged(OperationType operationType, Int2 indexPosition, TileType oldTileType, TileType newTileType, string captureID);
        public delegate void TileCaptureChanged(OperationType operationType, Int2 indexPosition, TileType tileType, string oldCaptureID, string newCaptureID);
        public delegate void FactionsOriginChanged(OperationType operationType, string playerID, List<TileType> types);
        public delegate void FactionsExplorationsChanged(OperationType operationType, ExplorationKey explorationKey, string itemID);

        public PartyStateChanged OnPartyStateChanged;
        public PartyPlayerJoined OnPartyPlayerJoined;
        public PartyPlayerLeaved OnPartyPlayerLeaved;
        public PartyPlayerReadyChanged OnPartyPlayerReadyChanged;
        public PartyPlayerStateChanged OnPartyPlayerStateChanged;

        public TurnSecondsChanged OnTurnSecondsChanged;
        public TurnChanged OnTurnChanged;
        public CardOwnersChanged OnCardOwnersChanged;

        public CardApplied OnCardApplied;
        public DeskApplied OnDeskApplied;
        //public BuildingAttackApplied OnBuildingAttackApplied;
        public FactionExplored OnFactionExplored;

        public ResourceChanged OnResourceChanged;
        public TileTypeChanged OnTileTypeChanged;
        public TileCaptureChanged OnTileCaptureChanged;
        public FactionsOriginChanged OnFactionsOriginsChanged;
        public FactionsExplorationsChanged OnFactionsExplorationsChanged;
    }
}