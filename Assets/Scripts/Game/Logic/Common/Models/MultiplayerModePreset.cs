using System;
using Core.Attributes;
using Game.Logic.Internal.Interfaces;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Logic.Common.Models
{
    [Serializable] [HideReferenceObjectPicker]
    public class MultiplayerModePreset : IPartyManagerPreset, ITurnManagerPreset
    {
        #region Inspector

        [OdinSerialize] [ScenePath] private string _lobbySceneName;
        [OdinSerialize] [ScenePath] private string _gameSceneName;

        [LabelText("Players Count (Min - Max)")] [MinMaxSlider(1, 6)]
        [Space] [OdinSerialize] private Vector2Int _playersRange = new (2, 4);
        [OdinSerialize] private int _playersWaitingSeconds = 5;
        private int? _playersWaitingSecondsCustomized = null;

        // [LabelText("Common Level Logic (Custom)")] [Tooltip("The logic will be added to the logic of the level.")]
        // [Space] [OdinSerialize] private LevelLogic _commonLevelLogic;
        [Space] [OdinSerialize] private LevelLogic _levelLogic;
        [OdinSerialize] private FactionsDefinition _factionsDefinition;
        [OdinSerialize] private int _secondsPerTurn = 60;

        #endregion

        public string LobbySceneName => _lobbySceneName;
        public string GameSceneName => _gameSceneName;

        public Vector2Int PlayersRange => _playersRange;
        public int PlayersWaitingSeconds
        {
            get => _playersWaitingSecondsCustomized ?? _playersWaitingSeconds;
            set => _playersWaitingSecondsCustomized = value;
        }

        public LevelLogic LevelLogic
        {
            get
            {
                // _levelLogic.Common = _commonLevelLogic;
                return _levelLogic;
            }
        }

        public FactionsDefinition FactionsDefinition => _factionsDefinition;

        public int SecondsPerTurn => _secondsPerTurn;
    }
}