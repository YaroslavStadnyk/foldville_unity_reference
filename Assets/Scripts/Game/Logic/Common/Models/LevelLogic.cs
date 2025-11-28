using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Game.Logic.Common.Blocks.Game;
using Game.Logic.Common.Blocks.Player;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Logic.Common.Models
{
    [CreateAssetMenu(fileName = "New " + nameof(LevelLogic), menuName = nameof(LevelLogic), order = 1)]
    public class LevelLogic : SerializedScriptableObject
    {
        // [BoxGroup("Debug")] [ShowInInspector] [ReadOnly] [HideInEditorMode]
        [NonSerialized] public LevelLogic Common;

        #region Inspector

        [ListDrawerSettings(DefaultExpandedState = true, DraggableItems = true, AddCopiesLastElement = false, AlwaysAddDefaultValue = false)]
        [Space] [LabelText("Game Context")] [OdinSerialize] private List<GameTrigger> _gameTriggers = new();

        [ListDrawerSettings(DefaultExpandedState = true, DraggableItems = true, AddCopiesLastElement = false, AlwaysAddDefaultValue = false)]
        [Space] [LabelText("Player Context")] [OdinSerialize] private List<PlayerTrigger> _playerTriggers = new();

        #endregion

        public IReadOnlyList<GameTrigger> GameTriggers
        {
            get
            {
                if (Common == null || Common._gameTriggers.IsNullOrEmpty())
                {
                    return _gameTriggers;
                }

                var gameTriggers = Common._gameTriggers.ToList();
                gameTriggers.AddRange(_gameTriggers);
                return gameTriggers;
            }
        }

        public IReadOnlyList<PlayerTrigger> PlayerTriggers
        {
            get
            {
                if (Common == null || Common._playerTriggers.IsNullOrEmpty())
                {
                    return _playerTriggers;
                }

                var playerTriggers = Common._playerTriggers.ToList();
                playerTriggers.AddRange(_playerTriggers);
                return playerTriggers;
            }
        }
    }
}