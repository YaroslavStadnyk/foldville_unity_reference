using System;
using System.Collections;
using System.Collections.Generic;
using Core.Extensions;
using Game.Logic.Common.Blocks;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Models;
using Game.Logic.Common.Structs;
using Game.Logic.Configs;
using Mirror;
using UnityEngine;

namespace Game.Logic.Internal.Network
{
    public class PartyAssistantNetwork : BaseNetwork
    {
        [SyncVar] private long _expirationTime;
        [SyncVar(hook = nameof(ClientPartyStateChanged))] private PartyState _state = PartyState.Waiting;

        private readonly SyncDictionary<string, PartyPlayerStats> _joinedPlayers = new();
        private readonly IDictionary<string, PartyPlayerStats> _oldJoinedPlayers = new Dictionary<string, PartyPlayerStats>();

        public IDictionary<string, PartyPlayerStats> JoinedPlayers => _joinedPlayers;

        public long ExpirationTime
        {
            get => _expirationTime;
            private set
            {
                var oldExpirationTime = _expirationTime;
                if (value == oldExpirationTime)
                {
                    return;
                }

                _expirationTime = value;
            }
        }

        public PartyState State
        {
            get => _state;
            set
            {
                var oldState = _state;
                if (value == oldState)
                {
                    return;
                }

                StopChangeState();

                _state = value;
                ServerPartyStateChanged(oldState, value);
            }
        }

        public void StartChangeState(PartyState state, PartyState nextState, int delayInSeconds)
        {
            if (delayInSeconds > 0 || ExpirationTime > DateTime.UtcNow.Ticks)
            {
                ExpirationTime = DateTime.UtcNow.AddSeconds(delayInSeconds).Ticks;
            }

            StopChangeState();

            if (delayInSeconds <= 0)
            {
                State = nextState;
            }
            else
            {
                State = state;

                _changePartyStateCoroutine = StartCoroutine(ChangeState(nextState, delayInSeconds));
            }
        }

        private void StopChangeState()
        {
            if (_changePartyStateCoroutine != null)
            {
                StopCoroutine(_changePartyStateCoroutine);
            }
        }

        private readonly CommonLevelLogic _commonLevelLogic = new();
        public LevelLogic LevelLogic { get; set; }

        public override void OnStartServer()
        {
            _joinedPlayers.Clear();

            _commonLevelLogic.Initialize();

            foreach (var trigger in LevelLogic.GameTriggers)
            {
                trigger?.Initialize();
            }

            foreach (var trigger in LevelLogic.PlayerTriggers)
            {
                trigger?.Initialize();
            }
        }

        public override void OnStopServer()
        {
            _commonLevelLogic.Terminate();

            foreach (var trigger in LevelLogic.GameTriggers)
            {
                trigger?.Terminate();
            }

            foreach (var trigger in LevelLogic.PlayerTriggers)
            {
                trigger?.Terminate();
            }
        }

        public override void OnStartClient()
        {
            _joinedPlayers.Callback += OnJoinedPlayersChanged;

            foreach (var (playerID, playerStats) in _joinedPlayers)
            {
                if (!_oldJoinedPlayers.ContainsKey(playerID))
                {
                    GameEvents.Instance.OnPartyPlayerJoined?.Invoke(playerID);
                }

                CheckOnJoinedPlayersChangedSecondaryEvents(playerID, playerStats);
                _oldJoinedPlayers[playerID] = playerStats;
            }
        }

        public override void OnStopClient()
        {
            _joinedPlayers.Callback -= OnJoinedPlayersChanged;
        }

        #region Hooks

        private void OnJoinedPlayersChanged(SyncIDictionary<string, PartyPlayerStats>.Operation operation, string playerID, PartyPlayerStats playerStats)
        {
            var operationType = operation.ToType();
            switch (operationType)
            {
                case OperationType.Add:
                    GameEvents.Instance.OnPartyPlayerJoined?.Invoke(playerID);

                    CheckOnJoinedPlayersChangedSecondaryEvents(playerID, playerStats);
                    _oldJoinedPlayers[playerID] = playerStats;
                    break;
                case OperationType.Remove:
                    GameEvents.Instance.OnPartyPlayerLeaved?.Invoke(playerID);

                    _oldJoinedPlayers.Remove(playerID);
                    break;
                case OperationType.Set:
                    if (!_oldJoinedPlayers.ContainsKey(playerID))
                    {
                        GameEvents.Instance.OnPartyPlayerJoined?.Invoke(playerID);
                    }

                    CheckOnJoinedPlayersChangedSecondaryEvents(playerID, playerStats);
                    _oldJoinedPlayers[playerID] = playerStats;
                    break;
                case OperationType.Clear:
                    _oldJoinedPlayers.Clear();
                    break;
                default:
                    break;
            }
        }

        private void CheckOnJoinedPlayersChangedSecondaryEvents(string playerID, PartyPlayerStats playerStats)
        {
            var oldPlayerStats = _oldJoinedPlayers.FirstOrDefault(playerID);
            if (oldPlayerStats.isReady != playerStats.isReady)
            {
                GameEvents.Instance.OnPartyPlayerReadyChanged?.Invoke(playerID, playerStats.isReady);
            }

            if (oldPlayerStats.state != playerStats.state)
            {
                GameEvents.Instance.OnPartyPlayerStateChanged?.Invoke(playerID, oldPlayerStats.state, playerStats.state);
            }
        }

        [Server]
        private void ServerPartyStateChanged(PartyState oldPartyState, PartyState newPartyState)
        {
            if (!NetworkClient.active)
            {
                GameEvents.Instance.OnPartyStateChanged?.Invoke(oldPartyState, newPartyState);
            }
        }

        [Client]
        private void ClientPartyStateChanged(PartyState oldPartyState, PartyState newPartyState)
        {
            GameEvents.Instance.OnPartyStateChanged?.Invoke(oldPartyState, newPartyState);
        }

        #endregion

        #region Coroutines

        private Coroutine _changePartyStateCoroutine;

        private IEnumerator ChangeState(PartyState state, int delayInSeconds)
        {
            yield return new WaitForSecondsRealtime(delayInSeconds);
            State = state;
        }

        #endregion
    }
}