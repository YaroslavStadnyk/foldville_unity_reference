using System;
using System.Collections.Generic;
using System.Linq;
using Board.Structs;
using Core.Extensions;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;
using Game.Players.Player;
using Grid.Common;
using MathModule.Structs;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Logic.Common.Blocks.Quests
{
    [Serializable]
    public abstract class LevelQuest
    {
        private bool _isInitialized = false;

        public QuestState State { get; private set; }
        public abstract string Description { get; }

        public event Action<bool> OnCompleted;

        public void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning($"{nameof(LevelQuest)} is already initialized.");
                return;
            }

            State = QuestState.Executing;

            OnInitialize();
            _isInitialized = true;
        }

        public void Terminate()
        {
            if (!_isInitialized)
            {
                Debug.LogWarning($"{nameof(LevelQuest)} is already terminated.");
            }

            OnCompleted = null;

            OnTerminate();
            _isInitialized = false;
        }

        protected abstract void OnInitialize();
        protected abstract void OnTerminate();

        protected void Complete(bool isSuccess)
        {
            if (State is not QuestState.Executing)
            {
                return;
            }

            State = isSuccess ? QuestState.Completed : QuestState.Failed;
            OnCompleted?.Invoke(isSuccess);

            Terminate();
        }
    }

    [Serializable]
    public class ReachResources : LevelQuest
    {
        [ShowInInspector] [ReadOnly] [PropertyOrder(-1)]
        public override string Description
        {
            get
            {
                var description = $"Reach {_targetResourceCount.ToShort()} {_targetResourceType.ToString().ToLower()}";
                description += _targetTurnsCount switch
                {
                    < 1 => " per turn",
                    1 => " in the first turn",
                    > 1 => $" in {_targetTurnsCount} turns"
                };

                return $"{description}.";
            }
        }

        [OdinSerialize] private ResourceType _targetResourceType = ResourceType.Energy;
        [OdinSerialize] [Min(1)] private int _targetResourceCount = 10;
        [OdinSerialize] [Min(1)] private int _targetTurnsCount = 1;

        // private int _resourceCount = 0;
        private int _turnsCount = 0;

        protected override void OnInitialize()
        {
            // _resourceCount = 0;
            _turnsCount = 0;

            GameEvents.Instance.OnResourceChanged += OnResourceChanged;
            GameEvents.Instance.OnTurnChanged += OnTurnChanged;
        }

        protected override void OnTerminate()
        {
            GameEvents.Instance.OnResourceChanged -= OnResourceChanged;
            GameEvents.Instance.OnTurnChanged -= OnTurnChanged;
        }

        private void OnResourceChanged(OperationType operationType, ResourceKey resourceKey, int oldValue, int newValue)
        {
            if (PlayerProfile.LocalLatest == null || PlayerProfile.LocalLatest.ID != resourceKey.holderID)
            {
                return;
            }

            if (resourceKey.type != _targetResourceType)
            {
                return;
            }

            // _resourceCount += newValue - oldValue;
            if (newValue >= _targetResourceCount)
            {
                Complete(true);
            }
        }

        private void OnTurnChanged(Turn oldTurn, Turn newTurn)
        {
            if (PlayerProfile.LocalLatest == null || PlayerProfile.LocalLatest.ID != oldTurn.playerID)
            {
                return;
            }

            if (_targetTurnsCount <= 0)
            {
                // _resourceCount = 0;
                return;
            }

            _turnsCount += 1;
            if (_turnsCount >= _targetTurnsCount)
            {
                Complete(false);
            }
        }
    }

    [Serializable]
    public class BuildTiles : LevelQuest
    {
        [ShowInInspector] [ReadOnly] [PropertyOrder(-1)]
        public override string Description
        {
            get
            {
                var description = "Build";
                description += _targetTileTypes.IsNullOrEmpty() ? " any tile" : $" {_targetTileTypes.ToArray().ToStringValues()}";
                description += $" {_targetTilesCount.ToShort()} times";
                description += _targetTurnsCount switch
                {
                    < 1 => " per turn",
                    1 => " in the first turn",
                    > 1 => $" in {_targetTurnsCount} turns"
                };

                return $"{description}.";
            }
        }

        [ListDrawerSettings(CustomAddFunction = nameof(AddTargetTileType))]
        [OdinSerialize] private HashSet<TileType> _targetTileTypes = new();
        [OdinSerialize] [Min(1)] private int _targetTilesCount = 10;
        [OdinSerialize] private int _targetTurnsCount = 1;

        private void AddTargetTileType()
        {
            _targetTileTypes.AddUniqueValue();
        }

        private int _tilesCount = 0;
        private int _turnsCount = 0;

        protected override void OnInitialize()
        {
            _tilesCount = 0;
            _turnsCount = 0;

            GameEvents.Instance.OnCardApplied += OnCardApplied;
            GameEvents.Instance.OnTurnChanged += OnTurnChanged;
        }

        protected override void OnTerminate()
        {
            GameEvents.Instance.OnCardApplied -= OnCardApplied;
            GameEvents.Instance.OnTurnChanged -= OnTurnChanged;
        }

        private void OnCardApplied(HandInfo hand, CardInfo card, Int2 indexPosition, PlayerErrorType errorType)
        {
            if (errorType is not PlayerErrorType.None)
            {
                return;
            }

            if (PlayerProfile.LocalLatest == null || PlayerProfile.LocalLatest.ID != hand.ID)
            {
                return;
            }

            if (!_targetTileTypes.IsNullOrEmpty() && !_targetTileTypes.Contains(card.Type))
            {
                return;
            }

            _tilesCount += 1;
            if (_tilesCount >= _targetTilesCount)
            {
                Complete(true);
            }
        }

        private void OnTurnChanged(Turn oldTurn, Turn newTurn)
        {
            if (PlayerProfile.LocalLatest == null || PlayerProfile.LocalLatest.ID != oldTurn.playerID)
            {
                return;
            }

            if (_targetTurnsCount <= 0)
            {
                _tilesCount = 0;
                return;
            }

            _turnsCount += 1;
            if (_turnsCount >= _targetTurnsCount)
            {
                Complete(false);
            }
        }
    }
}