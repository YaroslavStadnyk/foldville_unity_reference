using System.Collections.Generic;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;
using Game.Logic.Configs;
using Game.Logic.Internal.Interfaces;
using MathModule.Structs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Logic.API
{
    /// <summary>
    /// It was created to control players queue and what they can do on their turn.
    /// </summary>
    public class TurnManager : MonoBehaviour, IGameManager
    {
        private ITurnManager _impl;
        private ITurnManagerPreset _preset;

        [BoxGroup("Debug")] [ShowInInspector] [ReadOnly] [HideInEditorMode] public Turn CurrentTurn => _impl?.CurrentTurn ?? default;
        [BoxGroup("Debug")] [ShowInInspector] [ReadOnly] [HideInEditorMode] public IReadOnlyList<string> PlayersQueue => _impl?.PlayersQueue;

        public void Initialize(GameMode gameMode)
        {
            _preset = gameMode is GameMode.Campaign ? GameConfig.Instance.CampaignModePreset : GameConfig.Instance.MultiplayerPreset;

            var implPrefab = DeveloperConfig.Instance.GetTurnManagerPrefab(gameMode);
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
            _impl = impl as ITurnManager;
            if (_impl == null)
            {
                Debug.LogError($"{typeof(ITurnManager)} not casted.");
                return;
            }

            if (impl is Component implComponent)
            {
                implComponent.transform.parent = transform;
            }

            _impl.Preset = _preset;
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

        public int SecondsPerTurn => _impl?.SecondsPerTurn ?? -1;
        public int? CustomSecondsPerTurn{
            set
            {
                if (_impl != null)
                {
                    _impl.CustomSecondsPerTurn = value;
                }
            }
        }

        public void PassTurn()
        {
            if (_impl == null)
            {
                Debug.LogError($"{nameof(TurnManager)} - the function cannot be called. {nameof(_impl)} is null.");
                return;
            }

            _impl.PassTurnToNext();
        }

        public void ApplyCard(string cardID, Int2 indexPosition)
        {
            if (_impl == null)
            {
                Debug.LogError($"{nameof(TurnManager)} - the function cannot be called. {nameof(_impl)} is null.");
                return;
            }

            _impl.ApplyCard(cardID, indexPosition);
        }

        public void ApplyDesk(string deskID)
        {
            if (_impl == null)
            {
                Debug.LogError($"{nameof(TurnManager)} - the function cannot be called. {nameof(_impl)} is null.");
                return;
            }

            _impl.ApplyDesk(deskID);
        }

        public void ApplyBuildingAttack(AttackCoords attackCoords)
        {
            if (_impl == null)
            {
                Debug.LogError($"{nameof(TurnManager)} - the function cannot be called. {nameof(_impl)} is null.");
                return;
            }

            _impl.ApplyBuildingAttack(attackCoords);
        }
    }
}