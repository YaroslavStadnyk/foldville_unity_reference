using System.Collections.Generic;
using Core.Extensions;
using Game.Configs;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;
using Game.Logic.Configs;
using Game.Logic.Internal.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Logic.API
{
    /// <summary>
    /// It was created to control local and network player connections.
    /// </summary>
    public class PartyManager : MonoBehaviour, IGameManager
    {
        private IPartyManager _impl;
        private IPartyManagerPreset _preset;

        [BoxGroup("Debug")] [ShowInInspector] [ReadOnly] [HideInEditorMode] public PartyState State => _impl?.State ?? default;
        [BoxGroup("Debug")] [ShowInInspector] [ReadOnly] [HideInEditorMode] public float RemainingSeconds => _impl?.RemainingSeconds ?? default;
        [BoxGroup("Debug")] [ShowInInspector] [ReadOnly] [HideInEditorMode]  public IReadOnlyDictionary<string, PartyPlayerStats> JoinedPlayers => _impl?.JoinedPlayers;

        public void Initialize(GameMode gameMode)
        {
            _preset = gameMode is GameMode.Campaign ? GameConfig.Instance.CampaignModePreset : GameConfig.Instance.MultiplayerPreset;
            if (gameMode is GameMode.OfflineMultiplayer)
            {
                _preset.PlayersWaitingSeconds = -1;
            }

            var implPrefab = DeveloperConfig.Instance.GetPartyManagerPrefab(gameMode);
            implPrefab.Initialize(OnInitialize);
        }

        public void ResetImplementation()
        {
            if (_impl == null)
                return;
            
            _impl.Destroy();
            _impl = null;
        }

        private void OnInitialize(IBase impl)
        {
            _impl = impl as IPartyManager;
            if (_impl == null)
            {
                Debug.LogError($"{typeof(IPartyManager)} not casted.");
                return;
            }

            if (impl is Component implComponent)
            {
                implComponent.transform.parent = transform;
            }

            _impl.Preset = _preset;
        }

        public bool IsFull()
        {
            if (_impl == null || _preset == null || JoinedPlayers == null)
            {
                return false;
            }

            return JoinedPlayers.Count >= _preset.PlayersRange.y;
        }

        public void Join(string playerID, bool isReady)
        {
            if (_impl == null)
            {
                Debug.LogError($"{nameof(PartyManager)} - player {playerID} cannot be joined. {nameof(_impl)} is null.");
                return;
            }

            _impl.Join(playerID, isReady);
        }

        public void Leave(string playerID)
        {
            if (_impl == null)
            {
                return;
            }

            _impl.Leave(playerID);
        }

        public void SetPlayerReady(string playerID, bool isReady)
        {
            if (_impl == null)
            {
                return;
            }

            _impl.SetPlayerReady(playerID, isReady);
        }

        public void SetPlayerState(string playerID, PartyPlayerState state)
        {
            if (_impl == null)
            {
                return;
            }

            _impl.SetPlayerState(playerID, state);
        }

        public void SetPlayerPerformance(string playerID, PartyPlayerPerformance performance)
        {
            if (_impl == null)
            {
                return;
            }

            _impl.SetPlayerPerformance(playerID, performance);
        }

        public void StartGameSession()
        {
            if (_impl == null)
            {
                return;
            }

            Debug.LogWarning("Game Session Started!");
            _impl.StartGameSession();
        }

        public void CompleteGameSession()
        {
            if (_impl == null)
            {
                return;
            }

            Debug.LogWarning("Game Session Completed!");
            _impl.CompleteGameSession();
        }
    }
}