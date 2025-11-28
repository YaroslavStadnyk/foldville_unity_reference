using System.Collections.Generic;
using System.Linq;
using Board.Interfaces;
using Core;
using Core.Extensions;
using Core.Managers;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;
using Grid.Common;
using MathModule.Structs;

namespace Game.Logic.Common.Blocks
{
    public class CommonLevelLogic
    {
        public const TileType MainTileType = TileType.Castle;

        public void Initialize()
        {
            GameEvents.Instance.OnPartyStateChanged += OnPartyStateChanged;
            GameEvents.Instance.OnPartyPlayerStateChanged += OnPartyPlayerStateChanged;
            GameEvents.Instance.OnTileTypeChanged += OnTileTypeChanged;
            GameEvents.Instance.OnTileCaptureChanged += OnTileCaptureChanged;
        }

        public void Terminate()
        {
            GameEvents.Instance.OnPartyStateChanged -= OnPartyStateChanged;
            GameEvents.Instance.OnPartyPlayerStateChanged -= OnPartyPlayerStateChanged;
            GameEvents.Instance.OnTileTypeChanged -= OnTileTypeChanged;
            GameEvents.Instance.OnTileCaptureChanged -= OnTileCaptureChanged;
        }

        #region Initialization

        private void OnPartyStateChanged(PartyState oldPartyState, PartyState newPartyState)
        {
            if (newPartyState is PartyState.Playing)
            {
                var joinedPlayers = GameManager.Instance.Party.JoinedPlayers;
                RegisterPlayers(joinedPlayers?.Keys?.ToArray());
                GameManager.Instance.Turn.PassTurn();
            }
        }

        private void RegisterPlayers(params string[] playerIDs)
        {
            if (playerIDs == null)
            {
                DebugUtility.LogError(this, $"{nameof(GameManager.Instance.Party.JoinedPlayers)} list is null.");
                return;
            }

            if (playerIDs.Length == 0)
            {
                DebugUtility.LogWarning(this, $"{nameof(GameManager.Instance.Party.JoinedPlayers)} list is empty.");
                return;
            }

            var handService = ServiceManager.Instance.GetService<IHandService>();
            foreach (var playerID in playerIDs)
            {
                handService?.CreateHand(playerID);
                GameManager.Instance.Factions.Register(playerID);
                GameManager.Instance.Turn.Register(playerID);
            }
        }

        private void UnregisterPlayer(string playerID)
        {
            var handService = ServiceManager.Instance.GetService<IHandService>();
            handService?.RemoveHand(playerID);
            GameManager.Instance.Turn.Unregister(playerID);
            GameManager.Instance.Factions.Unregister(playerID);

            var hexGrid = GameManager.Instance.HexGrid;
            if (hexGrid != null)
            {
                hexGrid.UncaptureTiles(playerID);
            }
        }

        #endregion

        #region GameCompletion

        private void OnPartyPlayerStateChanged(string playerID, PartyPlayerState oldPlayerState, PartyPlayerState newPlayerState)
        {
            if (newPlayerState is PartyPlayerState.Won)
            {
                GameManager.Instance.Party.CompleteGameSession();
                return;
            }

            if (newPlayerState is PartyPlayerState.Lost)
            {
                UnregisterPlayer(playerID);
            }

            if (oldPlayerState is PartyPlayerState.Default)
            {
                var joinedPlayers = GameManager.Instance.Party.JoinedPlayers;
                CheckPlayersStateInDefault(joinedPlayers);
            }
        }

        private void CheckPlayersStateInDefault(IReadOnlyDictionary<string, PartyPlayerStats> players)
        {
            if (players.IsNullOrEmpty())
            {
                return;
            }

            var playerIDs = players.Where(pair => pair.Value.state is PartyPlayerState.Default).Select(pair => pair.Key).ToArray();
            if (playerIDs.Length == 1)
            {
                GameManager.Instance.Party.SetPlayerState(playerIDs[0], PartyPlayerState.Won);
            }
        }

        private void OnTileTypeChanged(OperationType operationType, Int2 indexPosition, TileType oldTileType, TileType newTileType, string captureID)
        {
            if (oldTileType == newTileType)
            {
                return;
            }

            if (operationType is OperationType.Remove or OperationType.Set && !captureID.IsNullOrEmpty())
            {
                OnTileRemoved(oldTileType, newTileType, captureID);
            }
        }

        private void OnTileRemoved(TileType oldTileType, TileType newTileType, string captureID)
        {
            if (oldTileType is MainTileType)
            {
                GameManager.Instance.Party.SetPlayerState(captureID, PartyPlayerState.Lost);
            }
        }

        private void OnTileCaptureChanged(OperationType operationType, Int2 indexPosition, TileType tileType, string oldCaptureID, string newCaptureID)
        {
            if (oldCaptureID == newCaptureID)
            {
                return;
            }

            if (operationType is OperationType.Remove or OperationType.Set && !oldCaptureID.IsNullOrEmpty())
            {
                OnTileUncaptured(tileType, oldCaptureID, newCaptureID);
            }

            if (operationType is OperationType.Add or OperationType.Set && !newCaptureID.IsNullOrEmpty())
            {
                OnTileCaptured(tileType, oldCaptureID, newCaptureID);
            }
        }

        private void OnTileUncaptured(TileType tileType, string oldCaptureID, string newCaptureID)
        {
            if (tileType is MainTileType)
            {
                GameManager.Instance.Party.SetPlayerState(oldCaptureID, PartyPlayerState.Lost);
                return;
            }

            var hexGrid = GameManager.Instance.HexGrid;
            if (hexGrid != null && !hexGrid.HasTileCapture(oldCaptureID))
            {
                GameManager.Instance.Party.SetPlayerState(oldCaptureID, PartyPlayerState.Lost);
            }
        }

        private void OnTileCaptured(TileType tileType, string oldCaptureID, string newCaptureID)
        {
            if (oldCaptureID.IsNullOrEmpty() && !newCaptureID.IsNullOrEmpty())
            {
                var hexGrid = GameManager.Instance.HexGrid;
                if (hexGrid != null && hexGrid.IsCompletelyCaptured())
                {
                    var captureIndexPositions = hexGrid.GetTileCaptures();
                    var captureLeaderIDs = captureIndexPositions.Values.CountDuplicates();
                    var captureLeaderID = captureLeaderIDs.OrderByDescending(pair => pair.Value).FirstOrDefault().Key;
                    if (!captureLeaderID.IsNullOrEmpty())
                    {
                        GameManager.Instance.Party.SetPlayerState(captureLeaderID, PartyPlayerState.Won);
                    }
                }
            }
        }

        #endregion
    }
}