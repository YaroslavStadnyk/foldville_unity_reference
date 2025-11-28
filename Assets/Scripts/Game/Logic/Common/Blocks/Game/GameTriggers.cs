using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Logic.Common.Blocks.Game
{
    [Serializable]
    public abstract class GameTrigger
    {
        private bool _isInitialized = false;

        [ListDrawerSettings(DefaultExpandedState = false, DraggableItems = true, AddCopiesLastElement = false, AlwaysAddDefaultValue = false)]
        [LabelText("Actions")] [PropertyOrder(1)] [OdinSerialize] private List<GameAction> _actions = new();

        public event Action Callback;

        public void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning($"{nameof(GameTrigger)} is already initialized.");
                return;
            }

            OnInitialize();
            _isInitialized = true;
        }

        public void Terminate()
        {
            if (!_isInitialized)
            {
                Debug.LogWarning($"{nameof(GameTrigger)} is already terminated.");
            }

            Callback = null;

            OnTerminate();
            _isInitialized = false;
        }

        protected abstract void OnInitialize();
        protected abstract void OnTerminate();

        protected void SendCallback()
        {
            foreach (var action in _actions)
            {
                action?.Invoke();
            }

            Callback?.Invoke();
        }
    }

    /*

    [Serializable]
    public abstract class TurnTrigger : GameTrigger
    {
        [BoxGroup("Params")] [LabelText("Each Batch Index")] [SerializeField] private bool isEachBatchIndex = true;
        [BoxGroup("Params")] [SerializeField] [HideIf(nameof(isEachBatchIndex))] [Min(0)] private int targetBatchIndex;
        [BoxGroup("Params")] [LabelText("Each Player Index")] [SerializeField] private bool isEachPlayerIndex;
        [BoxGroup("Params")] [SerializeField] [HideIf(nameof(isEachPlayerIndex))] [Min(0)] private int targetPlayerIndex;

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

            var isTargetBatchIndex = isEachBatchIndex || targetBatchIndex == Mathf.FloorToInt(turn.index / (float)playersCount);
            var isTargetPlayerIndex = isEachPlayerIndex || targetPlayerIndex == turn.index % playersCount;
            if (isTargetBatchIndex && isTargetPlayerIndex)
            {
                SendCallback();
                return;
            }

            DebugUtility.LogCondition(this, false);
        }
    }

    [Serializable]
    public class OnTurnBegan : TurnTrigger
    {
        protected override void OnInitialize()
        {
            Callback += () => DebugUtility.LogCondition(this, true);
            base.OnInitialize();
        }

        protected override void OnTurnChanged(Turn oldTurn, Turn newTurn)
        {
            CheckTrigger(newTurn);
        }
    }

    [Serializable]
    public class OnTurnEnded : TurnTrigger
    {
        protected override void OnInitialize()
        {
            Callback += () => DebugUtility.LogCondition(this, true);
            base.OnInitialize();
        }

        protected override void OnTurnChanged(Turn oldTurn, Turn newTurn)
        {
            CheckTrigger(oldTurn);
        }
    }

    */

    [Serializable]
    public abstract class PartyStateTrigger : GameTrigger
    {
        [BoxGroup("Params")] [SerializeField] private PartyState targetPartyState;

        protected override void OnInitialize()
        {
            GameEvents.Instance.OnPartyStateChanged += OnPartyStateChanged;
        }

        protected override void OnTerminate()
        {
            GameEvents.Instance.OnPartyStateChanged -= OnPartyStateChanged;
        }

        protected abstract void OnPartyStateChanged(PartyState oldPartyState, PartyState newPartyState);

        protected void CheckTrigger(PartyState partyState)
        {
            if (partyState == targetPartyState)
            {
                SendCallback();
                return;
            }

            DebugUtility.LogCondition(this, false);
        }
    }

    [Serializable]
    public class OnPartyStateBegan : PartyStateTrigger
    {
        protected override void OnInitialize()
        {
            Callback += () => DebugUtility.LogCondition(this, true);
            base.OnInitialize();
        }

        protected override void OnPartyStateChanged(PartyState oldPartyState, PartyState newPartyState)
        {
            CheckTrigger(newPartyState);
        }
    }

    [Serializable]
    public class OnPartyStateEnded : PartyStateTrigger
    {
        protected override void OnInitialize()
        {
            Callback += () => DebugUtility.LogCondition(this, true);
            base.OnInitialize();
        }

        protected override void OnPartyStateChanged(PartyState oldPartyState, PartyState newPartyState)
        {
            CheckTrigger(oldPartyState);
        }
    }

    [Serializable]
    public abstract class PlayerStateTrigger : GameTrigger
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

        protected void CheckTrigger(PartyPlayerState playerState)
        {
            if (playerState == TargetPlayerState)
            {
                SendCallback();
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
            Callback += () => DebugUtility.LogCondition(this, true);
            base.OnInitialize();
        }

        protected override void OnPartyPlayerStateChanged(string playerID, PartyPlayerState oldPlayerState, PartyPlayerState newPlayerState)
        {
            CheckTrigger(newPlayerState);
        }
    }

    [Serializable]
    public class OnPlayerStateEnded : PlayerStateTrigger
    {
        [BoxGroup("Params")] [LabelText("Total Lost Only")] [OdinSerialize] [PropertyOrder(-1)] private bool _isTotalLostOnly;
        [BoxGroup("Params")] [LabelText("Total Lost Offset")] [OdinSerialize] [PropertyOrder(-1)] [Min(0)] [ShowIf(nameof(_isTotalLostOnly))] private int _totalLostOffset = 1;

        protected override void OnInitialize()
        {
            Callback += () => DebugUtility.LogCondition(this, true);
            base.OnInitialize();
        }

        protected override void OnPartyPlayerStateChanged(string playerID, PartyPlayerState oldPlayerState, PartyPlayerState newPlayerState)
        {
            if (_isTotalLostOnly)
            {
                if (oldPlayerState != TargetPlayerState)
                {
                    return;
                }

                var targetPlayerStateCount = GameManager.Instance.Party.JoinedPlayers.Values.Sum(playerStats => playerStats.state == TargetPlayerState ? 1 : 0);
                if (targetPlayerStateCount > _totalLostOffset)
                {
                    return;
                }
            }

            CheckTrigger(oldPlayerState);
        }
    }
}