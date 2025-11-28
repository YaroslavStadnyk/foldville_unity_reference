using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Extensions;
using Core.Structs;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;
using Grid.Common;
using MathModule.Structs;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using StringExtensions = Core.Extensions.StringExtensions;

namespace Game.Logic.Common.Blocks.Player
{
    [Serializable]
    public abstract class PlayerTrigger
    {
        private bool _isInitialized = false;

        [ListDrawerSettings(DefaultExpandedState = false, DraggableItems = true, AddCopiesLastElement = false, AlwaysAddDefaultValue = false)]
        [LabelText("Actions")] [PropertyOrder(1)] [OdinSerialize] private List<PlayerAction> _actions = new();

        public event Action<string> Callback;

        public void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning($"{nameof(PlayerTrigger)} is already initialized.");
                return;
            }

            OnInitialize();
            _isInitialized = true;
        }

        public void Terminate()
        {
            if (!_isInitialized)
            {
                Debug.LogWarning($"{nameof(PlayerTrigger)} is already terminated.");
            }

            Callback = null;

            OnTerminate();
            _isInitialized = false;
        }

        protected abstract void OnInitialize();
        protected abstract void OnTerminate();

        protected void SendCallback(string playerID)
        {
            foreach (var action in _actions)
            {
                action?.Invoke(playerID);
            }

            Callback?.Invoke(playerID);
        }
    }

    [Serializable]
    public abstract class PlayerTurnTrigger : PlayerTrigger
    {
        [BoxGroup("Params")] [SerializeField] [Min(1)] private int every = 1;
        [BoxGroup("Params")] [SerializeField] private int from = 0;
        [Tooltip("Use `-1` as infinity.")]
        [BoxGroup("Params")] [SerializeField] private int to = -1;

        protected override void OnInitialize()
        {
            GameEvents.Instance.OnTurnChanged += OnTurnChanged;
        }

        protected override void OnTerminate()
        {
            GameEvents.Instance.OnTurnChanged -= OnTurnChanged;
        }

        protected abstract void OnTurnChanged(Turn oldTurn, Turn newTurn);

        protected void CheckTrigger(Turn turn)
        {
            var playersCount = GameManager.Instance.Turn.PlayersQueue?.Count ?? 0;
            if (playersCount == 0)
            {
                DebugUtility.LogWarning(this, $"{nameof(playersCount)} is zero.");
                return;
            }

            var turnIndex = Mathf.FloorToInt(turn.index / (float)playersCount);
            var inRange = turnIndex >= from && (turnIndex <= to || to == -1);
            var isDividable = (turnIndex - from) % every == 0;
            if (inRange && isDividable)
            {
                SendCallback(turn.playerID);
                return;
            }

            DebugUtility.LogCondition(this, false);
        }
    }

    [Serializable]
    public class OnPlayerTurnBegan : PlayerTurnTrigger
    {
        protected override void OnInitialize()
        {
            Callback += _ => DebugUtility.LogCondition(this, true);
            base.OnInitialize();
        }

        protected override void OnTurnChanged(Turn oldTurn, Turn newTurn)
        {
            CheckTrigger(newTurn);
        }
    }

    [Serializable]
    public class OnPlayerTurnEnded : PlayerTurnTrigger
    {
        protected override void OnInitialize()
        {
            Callback += _ => DebugUtility.LogCondition(this, true);
            base.OnInitialize();
        }

        protected override void OnTurnChanged(Turn oldTurn, Turn newTurn)
        {
            CheckTrigger(oldTurn);
        }
    }

    [Serializable]
    public abstract class PlayerTileTypeTrigger : PlayerTrigger
    {
        [BoxGroup("Params")] [OdinSerialize] private bool _excludeTargetTileTypes = false;
        [ListDrawerSettings(DefaultExpandedState = true, CustomAddFunction = nameof(AddTargetTileType))]
        [BoxGroup("Params")] [OdinSerialize] private HashSet<TileType> _targetTileTypes = new();

        private void AddTargetTileType()
        {
            foreach (var tileType in Enum.GetValues(typeof(TileType)))
            {
                if (_targetTileTypes.Add((TileType)tileType))
                {
                    return;
                }
            }
        }

        protected override void OnInitialize()
        {
            GameEvents.Instance.OnTileTypeChanged += OnTileTypeChanged;
        }

        protected override void OnTerminate()
        {
            GameEvents.Instance.OnTileTypeChanged -= OnTileTypeChanged;
        }

        protected abstract void OnTileTypeChanged(OperationType operationType, Int2 indexPosition, TileType oldTileType, TileType newTileType, string captureID);

        protected void CheckTrigger(TileType tileType, string captureID)
        {
            var isTargetTileType = _targetTileTypes.Contains(tileType);
            if (isTargetTileType != _excludeTargetTileTypes)
            {
                SendCallback(captureID);
                return;
            }

            DebugUtility.LogCondition(this, false);
        }
    }

    [Serializable]
    public class OnPlayerTileCreated : PlayerTileTypeTrigger
    {
        protected override void OnInitialize()
        {
            Callback += _ => DebugUtility.LogCondition(this, true);
            base.OnInitialize();
        }

        protected override void OnTileTypeChanged(OperationType operationType, Int2 indexPosition, TileType oldTileType, TileType newTileType, string captureID)
        {
            if (operationType is OperationType.Add or OperationType.Set && !captureID.IsNullOrEmpty())
            {
                CheckTrigger(newTileType, captureID);
            }
        }
    }

    [Serializable]
    public class OnPlayerTileRemoved : PlayerTileTypeTrigger
    {
        protected override void OnInitialize()
        {
            Callback += _ => DebugUtility.LogCondition(this, true);
            base.OnInitialize();
        }

        protected override void OnTileTypeChanged(OperationType operationType, Int2 indexPosition, TileType oldTileType, TileType newTileType, string captureID)
        {
            if (operationType is OperationType.Remove or OperationType.Set && !captureID.IsNullOrEmpty())
            {
                CheckTrigger(oldTileType, captureID);
            }
        }
    }

    [Serializable]
    public abstract class PlayerTileCaptureTrigger : PlayerTrigger
    {
        [BoxGroup("Params")] [OdinSerialize] private bool _excludeTargetTileTypes = false;
        [ListDrawerSettings(DefaultExpandedState = true, CustomAddFunction = nameof(AddTargetTileType))]
        [BoxGroup("Params")] [OdinSerialize] private HashSet<TileType> _targetTileTypes = new();

        private void AddTargetTileType()
        {
            foreach (var tileType in Enum.GetValues(typeof(TileType)))
            {
                if (_targetTileTypes.Add((TileType)tileType))
                {
                    return;
                }
            }
        }

        protected override void OnInitialize()
        {
            GameEvents.Instance.OnTileCaptureChanged += OnTileCaptureChanged;
        }

        protected override void OnTerminate()
        {
            GameEvents.Instance.OnTileCaptureChanged -= OnTileCaptureChanged;
        }

        protected abstract void OnTileCaptureChanged(OperationType operationType, Int2 indexPosition, TileType tileType, string oldCaptureID, string newCaptureID);

        protected void CheckTrigger(TileType tileType, string captureID)
        {
            var isTargetTileType = _targetTileTypes.Contains(tileType);
            if (isTargetTileType != _excludeTargetTileTypes)
            {
                SendCallback(captureID);
                return;
            }

            DebugUtility.LogCondition(this, false);
        }
    }

    [Serializable]
    public class OnPlayerTileCaptured : PlayerTileCaptureTrigger
    {
        [Tooltip("It means that the event will only be triggered when the map is completely captured.")]
        [BoxGroup("Params")] [LabelText("Total Lost Only")] [OdinSerialize] [PropertyOrder(-1)] private bool _isTotalLostOnly = false;

        protected override void OnInitialize()
        {
            Callback += _ => DebugUtility.LogCondition(this, true);
            base.OnInitialize();
        }

        protected override void OnTileCaptureChanged(OperationType operationType, Int2 indexPosition, TileType tileType, string oldCaptureID, string newCaptureID)
        {
            if (operationType is OperationType.Add or OperationType.Set && !newCaptureID.IsNullOrEmpty())
            {
                if (_isTotalLostOnly)
                {
                    var captures = GameManager.Instance.HexGrid.GetTileCaptures();
                    var tileTypes = GameManager.Instance.HexGrid.GetTileTypes();
                    var hasUncaptureRemains = captures.Count != tileTypes.Count || captures.ContainsValue(null) || captures.ContainsValue("");
                    if (hasUncaptureRemains)
                    {
                        return;
                    }

                    var playerCaptureCounts = captures.Values.CountDuplicates();
                    var largestCapturesCount = 0;
                    var largestPlayerID = "";
                    foreach (var (playerID, capturesCount) in playerCaptureCounts)
                    {
                        if (capturesCount > largestCapturesCount)
                        {
                            largestCapturesCount = capturesCount;
                            largestPlayerID = playerID;
                        }
                    }

                    if (largestPlayerID.IsNullOrEmpty())
                    {
                        return;
                    }

                    CheckTrigger(tileType, largestPlayerID);
                    return;
                }

                CheckTrigger(tileType, newCaptureID);
            }
        }
    }

    [Serializable]
    public class OnPlayerTileUncaptured : PlayerTileCaptureTrigger
    {
        [BoxGroup("Params")] [LabelText("Total Lost Only")] [OdinSerialize] [PropertyOrder(-1)] private bool _isTotalLostOnly;

        protected override void OnInitialize()
        {
            Callback += _ => DebugUtility.LogCondition(this, true);
            base.OnInitialize();
        }

        protected override void OnTileCaptureChanged(OperationType operationType, Int2 indexPosition, TileType tileType, string oldCaptureID, string newCaptureID)
        {
            if (operationType is OperationType.Remove or OperationType.Set && !oldCaptureID.IsNullOrEmpty())
            {
                if (_isTotalLostOnly)
                {
                    var captures = GameManager.Instance.HexGrid.GetTileCaptures();
                    var hasCaptureRemains = captures.ContainsValue(oldCaptureID);
                    if (hasCaptureRemains)
                    {
                        return;
                    }
                }

                CheckTrigger(tileType, oldCaptureID);
            }
        }
    }

    [Serializable]
    public abstract class PlayerStateTrigger : PlayerTrigger
    {
        [BoxGroup("Params")] [OdinSerialize] protected PartyPlayerState TargetPlayerState;

        protected override void OnInitialize()
        {
            GameEvents.Instance.OnPartyPlayerStateChanged += OnPartyPlayerStateChanged;
        }

        protected override void OnTerminate()
        {
            GameEvents.Instance.OnPartyPlayerStateChanged -= OnPartyPlayerStateChanged;
        }

        protected abstract void OnPartyPlayerStateChanged(string playerID, PartyPlayerState oldPlayerState, PartyPlayerState newPlayerState);

        protected void CheckTrigger(PartyPlayerState playerState, string playerID)
        {
            if (playerState == TargetPlayerState)
            {
                SendCallback(playerID);
                return;
            }

            DebugUtility.LogCondition(this, false);
        }
    }

    [Serializable]
    public class OnPlayerStateBegan : PlayerStateTrigger
    {
        protected override void OnInitialize()
        {
            Callback += _ => DebugUtility.LogCondition(this, true);
            base.OnInitialize();
        }

        protected override void OnPartyPlayerStateChanged(string playerID, PartyPlayerState oldPlayerState, PartyPlayerState newPlayerState)
        {
            if (playerID.IsNullOrEmpty())
            {
                return;
            }

            CheckTrigger(newPlayerState, playerID);
        }
    }

    [Serializable]
    public class OnPlayerStateEnded : PlayerStateTrigger
    {
        protected override void OnInitialize()
        {
            Callback += _ => DebugUtility.LogCondition(this, true);
            base.OnInitialize();
        }

        protected override void OnPartyPlayerStateChanged(string playerID, PartyPlayerState oldPlayerState, PartyPlayerState newPlayerState)
        {
            if (playerID.IsNullOrEmpty())
            {
                return;
            }

            CheckTrigger(oldPlayerState, playerID);
        }
    }
}