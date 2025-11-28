using System;
using System.Collections.Generic;
using Board;
using Board.Interfaces;
using Board.Structs;
using Core;
using Core.Extensions;
using Core.Managers;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Models;
using Game.Logic.Common.Structs;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Logic.Common.Blocks.Player
{
    [Serializable]
    public abstract class PlayerAction
    {
        public abstract void Invoke(string playerID);
    }

    [Serializable]
    public class UnregisterFactionsPlayer : PlayerAction
    {
        public override void Invoke(string playerID)
        {
            if (playerID.IsNullOrEmpty())
            {
                DebugUtility.LogWarning(this, "target player ID is null or empty.");
                return;
            }

            GameManager.Instance.Factions.Unregister(playerID);

            DebugUtility.LogInvoke(this);
        }
    }

    [Serializable]
    public class UnregisterTurnPlayer : PlayerAction
    {
        public override void Invoke(string playerID)
        {
            if (playerID.IsNullOrEmpty())
            {
                DebugUtility.LogWarning(this, "target player ID is null or empty.");
                return;
            }

            GameManager.Instance.Turn.Unregister(playerID);

            DebugUtility.LogInvoke(this);
        }
    }

    [Serializable]
    public class DistributeCards : PlayerAction
    {
        [ValueDropdown("@CardConfig.Dropdown")]
        [OdinSerialize] private readonly string _cardsTag;
        [OdinSerialize] private readonly bool _random;
        [OdinSerialize] [ShowIf(nameof(_random))] private readonly int _count;

        public override void Invoke(string playerID)
        {
            if (playerID.IsNullOrEmpty())
            {
                DebugUtility.LogWarning(this, "target player ID is null or empty.");
                return;
            }

            var cardService = ServiceManager.Instance.GetService<ICardService>();
            var handService = ServiceManager.Instance.GetService<IHandService>();

            var createdCards = _random ? cardService.CreateRandomCards(_cardsTag, _count) : cardService.CreateCards(_cardsTag);
            foreach (var createdCard in createdCards)
            {
                handService.PutCard(playerID, createdCard);
            }

            DebugUtility.LogInvoke(this);
        }
    }

    [Serializable]
    public class DistributeResource : PlayerAction
    {
        [SerializeField] private ResourceType type = BuildingDefinition.CostType;
        [OdinSerialize] private PlayerInteger _count = new Default(10);

        public override void Invoke(string playerID)
        {
            if (playerID.IsNullOrEmpty())
            {
                DebugUtility.LogWarning(this, "target player ID is null or empty.");
                return;
            }

            var resourceKey = new ResourceKey(playerID, type);
            var resourceCount = GameManager.Instance.Resources.Data.FirstOrDefault(resourceKey);
            GameManager.Instance.Resources.Data[resourceKey] = resourceCount + _count.GetValue(playerID);

            DebugUtility.LogInvoke(this);
        }
    }

    [Serializable]
    public class RemoveResource : PlayerAction
    {
        [SerializeField] private ResourceType type = BuildingDefinition.CostType;
        [LabelText("Clean Out")] [SerializeField] private bool isCleanOut = true;
        [OdinSerialize] [HideIf(nameof(isCleanOut))] private PlayerInteger _count = new Default(10);

        public override void Invoke(string playerID)
        {
            if (playerID.IsNullOrEmpty())
            {
                return;
            }

            var resourceKey = new ResourceKey(playerID, type);
            var resourceCount = GameManager.Instance.Resources.Data.FirstOrDefault(resourceKey);
            GameManager.Instance.Resources.Data[resourceKey] = isCleanOut ? 0 : resourceCount - _count.GetValue(playerID);

            DebugUtility.LogInvoke(this);
        }
    }

    [Serializable]
    public class SetPlayerState : PlayerAction
    {
        [SerializeField] private PartyPlayerState state;

        public override void Invoke(string playerID)
        {
            if (playerID.IsNullOrEmpty())
            {
                DebugUtility.LogWarning(this, "target player ID is null or empty.");
                return;
            }

            GameManager.Instance.Party.SetPlayerState(playerID, state);

            DebugUtility.LogInvoke(this);
        }
    }
}