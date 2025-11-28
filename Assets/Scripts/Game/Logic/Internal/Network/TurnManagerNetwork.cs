using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Board;
using Board.Interfaces;
using Board.Structs;
using Core.Extensions;
using Core.Managers;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;
using Game.Logic.Internal.Interfaces;
using MathModule.Structs;
using Mirror;
using UnityEngine;

namespace Game.Logic.Internal.Network
{
    public class TurnManagerNetwork : BaseNetwork, ITurnManager
    {
        [SyncVar(hook = nameof(ClientTurnChanged))] private Turn _currentTurn = new(0, -1);

        // TODO turns queue
        public Turn CurrentTurn
        {
            get => _currentTurn;
            private set
            {
                var oldTurn = _currentTurn;
                if (value.playerID == oldTurn.playerID && value.index == oldTurn.index)
                {
                    return;
                }

                _currentTurn = value;
                ServerTurnChanged(oldTurn, value);
            }
        }

        public ITurnManagerPreset Preset { get; set; }

        private IDeskService _deskService;
        private IHandService _handService;

        private void Start()
        {
            _deskService = ServiceManager.Instance.GetService<IDeskService>();
            _handService = ServiceManager.Instance.GetService<IHandService>();
        }

        private readonly IList<string> _playersQueue = new List<string>();
        public IReadOnlyList<string> PlayersQueue => _playersQueue as IReadOnlyList<string>;

        public void Register(string playerID)
        {
            if (!_playersQueue.Contains(playerID))
            {
                _playersQueue.Add(playerID);
            }
        }

        public void Unregister(string playerID)
        {
            _playersQueue.Remove(playerID);
        }


        #region PassTurn

        private int? _customSecondsPerTurn = null;

        public int? CustomSecondsPerTurn
        {
            set
            {
                if (_customSecondsPerTurn == value)
                {
                    return;
                }

                _customSecondsPerTurn = value;
                GameEvents.Instance.OnTurnSecondsChanged?.Invoke(CurrentTurn.playerID, SecondsPerTurn);

                if (_passTurnCoroutine != null)
                {
                    StopCoroutine(_passTurnCoroutine);
                }

                if (SecondsPerTurn != -1)
                {
                    _passTurnCoroutine = PassTurnToNext(SecondsPerTurn);
                    StartCoroutine(_passTurnCoroutine);
                }
            }
        }

        public int SecondsPerTurn => _customSecondsPerTurn ?? Preset.SecondsPerTurn;

        [Server]
        public void PassTurn(string playerID)
        {
            if (!_playersQueue.Contains(playerID))
            {
                Debug.LogWarning($"The turn can't be passed to {playerID}. The player is not registered.");
                return;
            }

            var newTurn = CurrentTurn;
            newTurn.index += 1;
            newTurn.playerID = playerID;
            newTurn.expirationTime = SecondsPerTurn == -1 ? -1 : DateTime.UtcNow.AddSeconds(SecondsPerTurn).Ticks;

            CurrentTurn = newTurn;
        }

        [Server]
        public void PassTurnToNext()
        {
            if (_passTurnCoroutine != null)
            {
                StopCoroutine(_passTurnCoroutine);
            }

            if (_playersQueue.IsNullOrEmpty())
            {
                Debug.LogWarning($"The turn can't be passed to the next player. {nameof(PlayersQueue)} is empty.");
                return;
            }

            var nextPlayerIndex = _playersQueue.IndexOf(CurrentTurn.playerID) + 1;
            if (nextPlayerIndex >= _playersQueue.Count)
            {
                nextPlayerIndex = 0;
            }

            var nextPlayerID = _playersQueue[nextPlayerIndex];

            if (CurrentTurn.playerID == nextPlayerID)
            {
                Debug.LogWarning($"The next player: {nextPlayerID} is the same for turn [{CurrentTurn.index}].");
            }
            else if (nextPlayerID.IsNullOrEmpty())
            {
                Debug.LogWarning("The next player is null or empty.");
            }

            PassTurn(nextPlayerID);

            if (SecondsPerTurn != -1)
            {
                _passTurnCoroutine = PassTurnToNext(SecondsPerTurn);
                StartCoroutine(_passTurnCoroutine);
            }
        }

        #region Hooks

        [Server]
        private void ServerTurnChanged(Turn oldTurn, Turn newTurn)
        {
            if (!NetworkClient.active)
            {
                GameEvents.Instance.OnTurnChanged?.Invoke(oldTurn, newTurn);
                GameEvents.Instance.OnTurnSecondsChanged?.Invoke(newTurn.playerID, newTurn.RemainingSeconds);
            }
        }

        [Client]
        private void ClientTurnChanged(Turn oldTurn, Turn newTurn)
        {
            GameEvents.Instance.OnTurnChanged?.Invoke(oldTurn, newTurn);
            GameEvents.Instance.OnTurnSecondsChanged?.Invoke(newTurn.playerID, newTurn.RemainingSeconds);
        }

        #endregion

        #endregion

        #region ApplyCard

        [Server]
        public void ApplyCard(string cardID, Int2 indexPosition)
        {
            var playerID = CurrentTurn.playerID;
            if (playerID == null || cardID == null)
            {
                Debug.LogWarning($"Card '{cardID}' of player '{playerID}' wasn't applied. There is an empty id.");
                ServerCardApplied(new CardInfo(cardID), indexPosition, PlayerErrorType.NullReference);
                return;
            }

            if (!_playersQueue.Contains(playerID))
            {
                Debug.LogWarning($"Card '{cardID}' of player '{playerID}' wasn't applied. The player is not registered.");
                ServerCardApplied(new CardInfo(cardID), indexPosition, PlayerErrorType.PlayerNotFound);
                return;
            }

            if (!_handService.TryReadCard(playerID, cardID, out var cardInfo))
            {
                Debug.LogWarning($"{nameof(cardInfo)} '{cardID}' of player '{playerID}' not found.");
                ServerCardApplied(cardInfo, indexPosition, PlayerErrorType.ItemNotFound);
                return;
            }

            var factions = GameManager.Instance.Factions;
            var hexGrid = GameManager.Instance.HexGrid;
            if (hexGrid == null)
            {
                Debug.LogWarning($"Tile '{cardInfo.Type}' of player '{playerID}' can't be created on {indexPosition}. {name} {nameof(hexGrid)} is missing.");
                ServerCardApplied(cardInfo, indexPosition, PlayerErrorType.UnavailableAction);
            }

            var isResourceIgnored = factions == null || factions.Explorations.IsNullOrEmpty() || !factions.Explorations.Values.Contains(cardID);
            var isTileCreated = hexGrid.CreateTile(indexPosition, cardInfo.Type, playerID, isResourceIgnored);
            if (isTileCreated)
            {
                if (isResourceIgnored)
                {
                    _handService.TakeCard(playerID, cardID);
                }

                ServerCardApplied(cardInfo, indexPosition, PlayerErrorType.None);
            }
            else
            {
                if (isResourceIgnored || hexGrid.IsCreationResourcesAvailable(cardInfo.Type, playerID))
                {
                    Debug.LogWarning($"Tile '{cardInfo.Type}' of player '{playerID}' can't be created on {indexPosition}.");
                    ServerCardApplied(cardInfo, indexPosition, PlayerErrorType.UnavailableAction);
                }
                else
                {
                    Debug.Log($"{playerID} doesn't have enough resource to create the tile: {cardInfo.Type} => {indexPosition}.");
                    ServerCardApplied(cardInfo, indexPosition, PlayerErrorType.NotEnoughResources);
                }
            }
        }

        [Server]
        private void ServerCardApplied(CardInfo cardInfo, Int2 indexPosition, PlayerErrorType errorType)
        {
            RpcCardApplied(cardInfo, indexPosition, errorType);

            if (!NetworkClient.active)
            {
                var handInfo = GameManager.Instance.Board.Hands.FirstOrDefault(CurrentTurn.playerID);
                GameEvents.Instance.OnCardApplied?.Invoke(handInfo, cardInfo, indexPosition, errorType);
            }
        }

        [ClientRpc]
        private void RpcCardApplied(CardInfo cardInfo, Int2 indexPosition, PlayerErrorType errorType)
        {
            var handInfo = GameManager.Instance.Board.Hands.FirstOrDefault(CurrentTurn.playerID);
            GameEvents.Instance.OnCardApplied?.Invoke(handInfo, cardInfo, indexPosition, errorType);
        }

        #endregion

        #region ApplyDesk

        [Server]
        public void ApplyDesk(string deskID)
        {
            var playerID = CurrentTurn.playerID;
            if (playerID == null || deskID == null)
            {
                Debug.LogWarning($"Desk '{deskID}' of player '{playerID}' wasn't applied. There is an empty id.");
                ServerDeskApplied(new DeskInfo(deskID), default, PlayerErrorType.NullReference);
                return;
            }

            if (!_playersQueue.Contains(playerID))
            {
                Debug.LogWarning($"Desk '{deskID}' of player '{playerID}' wasn't applied. The player is not registered.");
                ServerDeskApplied(new DeskInfo(deskID), default, PlayerErrorType.PlayerNotFound);
                return;
            }

            var desk = _deskService.GetCreatedDesk(deskID);
            if (desk == null)
            {
                ServerDeskApplied(new DeskInfo(deskID), default, PlayerErrorType.ItemNotFound);
                return;
            }

            // TODO Take/Put card refactoring
            var card = _deskService.TakeCard(deskID);
            if (card == null)
            {
                ServerDeskApplied(desk.GetInfo(), default, PlayerErrorType.ItemNotFound);
                return;
            }

            var isCardReceived = _handService.PutCard(playerID, card);
            if (!isCardReceived)
            {
                _deskService.PutCard(deskID, card);
                ServerDeskApplied(desk.GetInfo(), card.GetInfo(), PlayerErrorType.UnavailableAction);
                return;
            }

            ServerDeskApplied(desk.GetInfo(), card.GetInfo(), PlayerErrorType.None);
        }

        [Server]
        private void ServerDeskApplied(DeskInfo deskInfo, CardInfo takenCardInfo, PlayerErrorType errorType)
        {
            RpcDeskApplied(deskInfo, takenCardInfo, errorType);

            if (!NetworkClient.active)
            {
                var handInfo = GameManager.Instance.Board.Hands.FirstOrDefault(CurrentTurn.playerID);
                GameEvents.Instance.OnDeskApplied?.Invoke(handInfo, deskInfo, takenCardInfo, errorType);
            }
        }

        [ClientRpc]
        private void RpcDeskApplied(DeskInfo deskInfo, CardInfo takenCardInfo, PlayerErrorType errorType)
        {
            var handInfo = GameManager.Instance.Board.Hands.FirstOrDefault(CurrentTurn.playerID);
            GameEvents.Instance.OnDeskApplied?.Invoke(handInfo, deskInfo, takenCardInfo, errorType);
        }

        #endregion

        #region ApplyBuildingAttack

        [Server]
        public void ApplyBuildingAttack(AttackCoords attackCoords)
        {
            var playerID = CurrentTurn.playerID;
            if (playerID == null)
            {
                Debug.LogWarning($"Building attack wasn't applied. The {nameof(playerID)} is null.");
                return;
            }

            if (!_playersQueue.Contains(playerID))
            {
                Debug.LogWarning($"Building attack wasn't applied. The {nameof(playerID)} is not registered.");
                return;
            }

            GameManager.Instance.HexGrid.AttackRuleExecutor.Attack(attackCoords, CurrentTurn.playerID);
        }

        #endregion

        #region Coroutines

        private IEnumerator _passTurnCoroutine;

        private IEnumerator PassTurnToNext(int delayInSeconds)
        {
            yield return new WaitForSecondsRealtime(delayInSeconds);

            PassTurnToNext();
        }

        #endregion
    }
}