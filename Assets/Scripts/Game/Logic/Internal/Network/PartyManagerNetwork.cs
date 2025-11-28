using System;
using System.Collections.Generic;
using System.Linq;
#if !UNITY_WEBGL
using EpicTransport;
#endif
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;
using Game.Logic.Configs;
using Game.Logic.Internal.Interfaces;
using Core.UI;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Logic.Internal.Network
{
    public class PartyManagerNetwork : NetworkRoomManager, IPartyManager
    {
        private PartyAssistantNetwork _partyAssistant;

        #region Inspector

        [Space] [SerializeField] [Required] private PartyAssistantNetwork partyAssistantPrefab;

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            if (Application.isPlaying)
            {
                return;
            }

            Preset = GameConfig.Instance.MultiplayerPreset;
            if (Preset != null)
            {
                SetupPreset();
                Debug.Log($"{name} - Preset is setup.");
            }
        }

        public override void OnValidate()
        {
            base.OnValidate();

            if (roomPlayerPrefab != null && !roomPlayerPrefab.TryGetComponent(out IPartyProfile _))
            {
                roomPlayerPrefab = null;
                Debug.LogError($"RoomPlayer prefab must have a {nameof(IPartyProfile)} interface.");
            }

            if (Application.isPlaying)
            {
                return;
            }

            Preset = GameConfig.Instance.MultiplayerPreset;
            if (Preset != null)
            {
                SetupPreset();
            }
        }

        #endregion

        public PartyState State
        {
            get
            {
                if (_partyAssistant == null)
                {
                    return default;
                }

                return _partyAssistant.State;
            }
        }

        public float RemainingSeconds
        {
            get
            {
                if (_partyAssistant == null)
                {
                    return default;
                }

                var timeSpan = new TimeSpan(_partyAssistant.ExpirationTime - DateTime.UtcNow.Ticks);
                return Mathf.Max(0.0f, (float)timeSpan.TotalSeconds);
            }
        }

        public IPartyManagerPreset Preset { get; set; }

        public void Initialize(Action<IBase> callback)
        {
            var prefab = this;
            var instance = Instantiate(prefab);

            callback?.Invoke(instance);
        }

        public void SetTransport(bool isLocal)
        {
#if !UNITY_WEBGL
            transport = isLocal ? gameObject.AddComponent<TelepathyTransport>() : gameObject.AddComponent<EosTransport>();
#else
            transport = gameObject.AddComponent<TelepathyTransport>();
#endif
            Transport.active = transport;
        }

        public override void Awake()
        {
            base.Awake();

            if (!spawnPrefabs.Contains(partyAssistantPrefab.gameObject))
            {
                spawnPrefabs.Add(partyAssistantPrefab.gameObject);
            }
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public override void Start()
        {
            base.Start();

            if (Preset != null)
            {
                SetupPreset();
            }
            else
            {
                Debug.LogError($"{nameof(PartyManagerNetwork)} - {nameof(Preset)} is null.");
            }
        }

        private void SetupPreset()
        {
            onlineScene = Preset.LobbySceneName;
            RoomScene = Preset.LobbySceneName;
            GameplayScene = Preset.GameSceneName;
            maxConnections = Preset.PlayersRange.y;
        }

        public event Action OnServerStarted;
        public override void OnStartServer()
        {
            base.OnStartServer();

            _partyAssistant = Instantiate(partyAssistantPrefab, transform);
            _partyAssistant.LevelLogic = Preset.LevelLogic;
            NetworkServer.Spawn(_partyAssistant.gameObject);

            OnServerStarted?.Invoke();
        }

        private void OnEnable()
        {
            GameEvents.Instance.OnPartyStateChanged += OnPartyStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.Instance.OnPartyStateChanged -= OnPartyStateChanged;
        }

        [ServerCallback]
        private void OnPartyStateChanged(PartyState oldPartyState, PartyState newPartyState)
        {
            if (newPartyState is PartyState.Loading && Utils.IsSceneActive(Preset.LobbySceneName))
            {
                pendingPlayers.Clear();
                allPlayersReady = true;
            }
        }

        public override void OnLoadingSceneAsyncChanged(string newSceneName, AsyncOperation asyncOperation, SceneOperation sceneOperation)
        {
            if (asyncOperation != null && sceneOperation is not SceneOperation.UnloadAdditive)
            {
                LoadingCanvas.Instance.AddOperation(asyncOperation);
            }
        }

        public override void OnRoomServerSceneChanged(string sceneName)
        {
            if (sceneName == offlineScene)
            {
                _partyAssistant.State = PartyState.Waiting;
                _partyAssistant.JoinedPlayers.Clear();
            }
            else if (sceneName == Preset.LobbySceneName)
            {
                _partyAssistant.State = PartyState.Waiting;
            }
            else if (sceneName == Preset.GameSceneName)
            {
                _partyAssistant.State = PartyState.Playing;
            }
        }

        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            if (roomPlayer != null && roomPlayer.TryGetComponent(out IPartyProfile partyProfile))
            {
                partyProfile.OnRoomServerLoadedPlayer(gamePlayer);
            }
            else
            {
                Debug.LogError($"RoomPlayer instance doesn't have {nameof(IPartyProfile)} interface.");
            }

            return true;
        }

        private bool IsLobbyAvailable => State is PartyState.Waiting or PartyState.Starting && Utils.IsSceneActive(Preset.LobbySceneName);
        private bool IsGameAvailable => State is PartyState.Playing && Utils.IsSceneActive(Preset.GameSceneName);

        public IReadOnlyDictionary<string, PartyPlayerStats> JoinedPlayers => _partyAssistant == null ? null : _partyAssistant.JoinedPlayers as IReadOnlyDictionary<string, PartyPlayerStats>;

        [Server]
        public void Join(string playerID, bool isReady)
        {
            var joinedPlayers = _partyAssistant.JoinedPlayers;
            if (joinedPlayers.Count >= Preset.PlayersRange.y)
            {
                Debug.LogWarning("The game is full.");
                return;
            }

            if (joinedPlayers.ContainsKey(playerID))
            {
                SetPlayerReady(playerID, true);
                return;
            }

            if (IsLobbyAvailable)
            {
                joinedPlayers.Add(playerID, new PartyPlayerStats());
                SetPlayerReady(playerID, isReady);
            }
            else
            {
                Debug.LogWarning($"Player: {playerID} cannot be joined.");
            }
        }

        [Server]
        public void Leave(string playerID)
        {
            var joinedPlayers = _partyAssistant.JoinedPlayers;
            if (!joinedPlayers.ContainsKey(playerID))
            {
                return;
            }

            if (IsLobbyAvailable)
            {
                joinedPlayers.Remove(playerID);
                CheckReadyToBegan();
            }
            else
            {
                SetPlayerReady(playerID, false);
            }
        }

        [Server]
        public void SetPlayerReady(string playerID, bool isReady)
        {
            var joinedPlayers = _partyAssistant.JoinedPlayers;
            if (!joinedPlayers.TryGetValue(playerID, out var joinedPlayer) || joinedPlayer.isReady == isReady)
            {
                return;
            }

            joinedPlayer.isReady = isReady;
            joinedPlayers[playerID] = joinedPlayer;

            CheckReadyToBegan();
        }

        [Server]
        public void CheckReadyToBegan()
        {
            if (!IsLobbyAvailable)
            {
                return;
            }

            var joinedPlayers = _partyAssistant.JoinedPlayers;
            var joinedPlayersCount = joinedPlayers.Count;
            var readyPlayersCount = joinedPlayers.Values.Sum(playerStats => playerStats.isReady ? 1 : 0);

            if (readyPlayersCount >= Preset.PlayersRange.x && readyPlayersCount == joinedPlayersCount)
            {
                if (Preset.PlayersWaitingSeconds < 0)
                {
                    _partyAssistant.State = PartyState.Starting;
                }
                else
                {
                    _partyAssistant.StartChangeState(PartyState.Starting, PartyState.Loading, Preset.PlayersWaitingSeconds);
                }
            }
            else
            {
                _partyAssistant.State = PartyState.Waiting;
            }

            if (readyPlayersCount >= Preset.PlayersRange.y)
            {
                if (Preset.PlayersWaitingSeconds >= 0)
                {
                    _partyAssistant.State = PartyState.Loading;
                }
            }
            else
            {
                allPlayersReady = false;
            }
        }

        [Server]
        public void SetPlayerState(string playerID, PartyPlayerState state)
        {
            if (!IsGameAvailable)
            {
                Debug.LogWarning($"State of player: {playerID} cannot be changed while the game is completing.");
                return;
            }

            var joinedPlayers = _partyAssistant.JoinedPlayers;
            if (!joinedPlayers.TryGetValue(playerID, out var joinedPlayer) || joinedPlayer.state == state)
            {
                return;
            }

            joinedPlayer.state = state;
            joinedPlayers[playerID] = joinedPlayer;
        }

        [Server]
        public void SetPlayerPerformance(string playerID, PartyPlayerPerformance performance)
        {
            if (!IsGameAvailable)
            {
                Debug.LogWarning($"Performance of player: {playerID} cannot be changed while the game is completing.");
                return;
            }

            var joinedPlayers = _partyAssistant.JoinedPlayers;
            if (!joinedPlayers.TryGetValue(playerID, out var joinedPlayer))
            {
                return;
            }

            joinedPlayer.performance = performance;
            joinedPlayers[playerID] = joinedPlayer;
        }

        public void StartGameSession()
        {
            if (!IsLobbyAvailable || _partyAssistant.State is not PartyState.Starting)
            {
                Debug.LogWarning($"The game session can't be started.");
                return;
            }

            _partyAssistant.State = PartyState.Loading;
        }

        public void CompleteGameSession()
        {
            if (!IsGameAvailable)
            {
                Debug.LogWarning($"The game session can't be completed.");
                return;
            }

            _partyAssistant.State = PartyState.Completing;
        }
    }
}