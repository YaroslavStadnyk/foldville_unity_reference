using System;
using System.Collections.Generic;
using Core.Attributes;
using Game.Logic.Common.Structs;
using Game.Logic.Internal.Interfaces;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Logic.Common.Models
{
    [Serializable] [HideReferenceObjectPicker]
    public class CampaignModePreset : IPartyManagerPreset, ITurnManagerPreset
    {
        #region Inspector

        [OdinSerialize] [ScenePath] private string _lobbySceneName;
        [OdinSerialize] private int _playersWaitingSeconds = 0;

        // [LabelText("Common Level Logic (Custom)")] [Tooltip("The logic will be added to the logic of each level.")]
        // [Space] [OdinSerialize] private LevelLogic _commonLevelLogic;

        [ListDrawerSettings(DefaultExpandedState = true, DraggableItems = true, ShowIndexLabels = true)]
        [Space] [OdinSerialize] public readonly List<CampaignLevel> Levels = new();

        #endregion

        public int SelectedLevelIndex { get; set; } = -1;
        public CampaignLevel SelectedLevel
        {
            get
            {
                if (SelectedLevelIndex < 0)
                {
                    Debug.LogError($"{nameof(SelectedLevelIndex)} is not selected.");
                    return default;
                }

                return SelectedLevelIndex < Levels.Count ? Levels[SelectedLevelIndex] : default;
            }
        }

        public string LobbySceneName => _lobbySceneName;
        public string GameSceneName => SelectedLevel.gameSceneName;

        public Vector2Int PlayersRange => Vector2Int.one;
        public int PlayersWaitingSeconds
        {
            get => _playersWaitingSeconds;
            set => _playersWaitingSeconds = value;
        }

        public LevelLogic LevelLogic
        {
            get
            {
                var levelLogic = SelectedLevel.levelLogic;
                // levelLogic.Common = _commonLevelLogic;
                return levelLogic;
            }
        }

        public int SecondsPerTurn => SelectedLevel.secondsPerTurn;
    }
}