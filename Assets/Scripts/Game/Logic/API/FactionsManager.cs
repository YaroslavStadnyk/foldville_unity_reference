using System.Collections.Generic;
using Core.Extensions;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Models;
using Game.Logic.Common.Structs;
using Game.Logic.Configs;
using Game.Logic.Internal.Interfaces;
using Grid.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Logic.API
{
    public class FactionsManager : MonoBehaviour, IGameManager
    {
        private IFactionsManager _impl;

        [BoxGroup("Debug")] [ShowInInspector] [ReadOnly] [HideInEditorMode] private FactionsDefinition Definition { get; set; }
        [BoxGroup("Debug")] [ShowInInspector] [ReadOnly] [HideInEditorMode] private IReadOnlyDictionary<string, List<TileType>> Origins => _impl?.Origins;
        [BoxGroup("Debug")] [ShowInInspector] [ReadOnly] [HideInEditorMode] public IReadOnlyDictionary<ExplorationKey, string> Explorations => _impl?.Explorations;

        public void Initialize(GameMode gameMode)
        {
            Definition = gameMode is GameMode.Campaign ? GameConfig.Instance.CampaignModePreset.SelectedLevel.factionsDefinition : GameConfig.Instance.MultiplayerPreset.FactionsDefinition;

            var implPrefab = DeveloperConfig.Instance.GetFactionsManagerPrefab(gameMode);
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
            _impl = impl as IFactionsManager;
            if (_impl == null)
            {
                Debug.LogError($"{typeof(IFactionsManager)} not casted.");
                return;
            }

            if (impl is Component implComponent)
            {
                implComponent.transform.parent = transform;
            }

            _impl.Definition = Definition;
        }

        public void Register(string playerID)
        {
            if (_impl == null)
            {
                Debug.LogError($"{nameof(TurnManager)} - player {playerID} cannot be registered. {nameof(_impl)} is null.");
                return;
            }

            _impl.Register(playerID);
        }

        public void Unregister(string playerID)
        {
            _impl.Unregister(playerID);
        }

        public bool IsAvailable(string playerID)
        {
            if (_impl == null)
            {
                return false;
            }

            return _impl.IsAvailable(playerID);
        }

        public bool IsAvailable(string playerID, TileType type)
        {
            if (_impl == null)
            {
                return false;
            }

            return _impl.IsAvailable(playerID, type);
        }

        public List<TileType> GetOrigin(string playerID)
        {
            if (_impl == null)
            {
                return null;
            }

            return _impl.GetOrigin(playerID);
        }

        public void Explore(string playerID, TileType type)
        {
            if (_impl == null)
            {
                return;
            }

            _impl.Explore(playerID, type);
        }
    }
}