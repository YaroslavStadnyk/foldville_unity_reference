using System;
using System.Collections.Generic;
using Board;
using Board.Interfaces;
using Board.Structs;
using Core;
using Core.Attributes;
using Core.Extensions;
using Core.Managers;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Models;
using Game.Logic.Common.Structs;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Logic.Common.Blocks.Game
{
    [Serializable]
    public abstract class GameAction
    {
        public abstract void Invoke();
    }

    [Serializable]
    public class RegisterBoardPlayers : GameAction
    {
#if UNITY_EDITOR
        [InfoBox("It uses " + nameof(GameManager.Instance.Party.JoinedPlayers) + " to register for the board.")]
        [HideLabel] [SerializeField] [ShowOnly] private string infoBoxField;
#endif

        public override void Invoke()
        {
            var partyJoinedPlayers = GameManager.Instance.Party.JoinedPlayers;
            if (partyJoinedPlayers == null)
            {
                DebugUtility.LogError(this, $"{nameof(GameManager.Instance.Party.JoinedPlayers)} list is null.");
                return;
            }

            if (partyJoinedPlayers.Count == 0)
            {
                DebugUtility.LogWarning(this, $"{nameof(GameManager.Instance.Party.JoinedPlayers)} list is empty.");
                return;
            }

            var handService = ServiceManager.Instance.GetService<IHandService>();

            foreach (var playerID in partyJoinedPlayers.Keys)
            {
                handService.CreateHand(playerID);
            }

            DebugUtility.LogInvoke(this);
        }
    }

    [Serializable]
    public class RegisterFactionsPlayers : GameAction
    {
#if UNITY_EDITOR
        [InfoBox("It uses " + nameof(GameManager.Instance.Party.JoinedPlayers) + " to register for factions.")]
        [HideLabel] [SerializeField] [ShowOnly] private string infoBoxField;
#endif

        public override void Invoke()
        {
            var partyJoinedPlayers = GameManager.Instance.Party.JoinedPlayers;
            if (partyJoinedPlayers == null)
            {
                DebugUtility.LogError(this, $"{nameof(GameManager.Instance.Party.JoinedPlayers)} list is null.");
                return;
            }

            if (partyJoinedPlayers.Count == 0)
            {
                DebugUtility.LogWarning(this, $"{nameof(GameManager.Instance.Party.JoinedPlayers)} list is empty.");
                return;
            }

            foreach (var playerID in partyJoinedPlayers.Keys)
            {
                GameManager.Instance.Factions.Register(playerID);
            }

            DebugUtility.LogInvoke(this);
        }
    }

    [Serializable]
    public class RegisterTurnPlayers : GameAction
    {
#if UNITY_EDITOR
        [InfoBox("It uses " + nameof(GameManager.Instance.Party.JoinedPlayers) + " to register for the queue.")]
        [HideLabel] [SerializeField] [ShowOnly] private string infoBoxField;
#endif

        public override void Invoke()
        {
            var partyJoinedPlayers = GameManager.Instance.Party.JoinedPlayers;
            if (partyJoinedPlayers == null)
            {
                DebugUtility.LogError(this, $"{nameof(GameManager.Instance.Party.JoinedPlayers)} list is null.");
                return;
            }

            if (partyJoinedPlayers.Count == 0)
            {
                DebugUtility.LogWarning(this, $"{nameof(GameManager.Instance.Party.JoinedPlayers)} list is empty.");
                return;
            }

            foreach (var playerID in partyJoinedPlayers.Keys)
            {
                GameManager.Instance.Turn.Register(playerID);
            }

            DebugUtility.LogInvoke(this);
        }
    }

    [Serializable]
    public class DistributeDesk : GameAction
    {
        [ValueDropdown("@DeskConfig.Dropdown")]
        [OdinSerialize] private string _deskName;

        public override void Invoke()
        {
            var deskService = ServiceManager.Instance.GetService<IDeskService>();
            deskService.CreateDesk(_deskName);
        }
    }

    [Serializable]
    public class DistributeCards : GameAction
    {
        [ValueDropdown("@CardConfig.Dropdown")]
        [OdinSerialize] private readonly string _cardsTag;
        [OdinSerialize] private readonly bool _random;
        [OdinSerialize] [ShowIf(nameof(_random))] private readonly int _count;

        public override void Invoke()
        {
            var players = GameManager.Instance.Party.JoinedPlayers;
            if (players == null || players.Count == 0)
            {
                DebugUtility.LogWarning(this, $"{nameof(players)} list is null or empty.");
                return;
            }

            var cardService = ServiceManager.Instance.GetService<ICardService>();
            var handService = ServiceManager.Instance.GetService<IHandService>();

            foreach (var playerID in players.Keys)
            {
                var createdCards = _random ? cardService.CreateRandomCards(_cardsTag, _count) : cardService.CreateCards(_cardsTag);
                foreach (var createdCard in createdCards)
                {
                    handService.PutCard(playerID, createdCard);
                }
            }

            DebugUtility.LogInvoke(this);
        }
    }

    [Serializable]
    public class DistributeResource : GameAction
    {
        [SerializeField] private ResourceType type = BuildingDefinition.CostType;
        [SerializeField] private int count = 10;

        public override void Invoke()
        {
            var players = GameManager.Instance.Party.JoinedPlayers;
            if (players == null || players.Count == 0)
            {
                DebugUtility.LogWarning(this, $"{nameof(players)} list is null or empty.");
                return;
            }

            foreach (var playerID in players.Keys)
            {
                var resourceKey = new ResourceKey(playerID, type);
                var resourceCount = GameManager.Instance.Resources.Data.FirstOrDefault(resourceKey);
                GameManager.Instance.Resources.Data[resourceKey] = resourceCount + count;
            }

            DebugUtility.LogInvoke(this);
        }
    }

    [Serializable]
    public class RemoveResource : GameAction
    {
        [SerializeField] private ResourceType type = BuildingDefinition.CostType;
        [LabelText("Clean Out")] [SerializeField] private bool isCleanOut = true;
        [SerializeField] [HideIf(nameof(isCleanOut))] private int count = 10;

        public override void Invoke()
        {
            var players = GameManager.Instance.Party.JoinedPlayers;
            if (players == null || players.Count == 0)
            {
                DebugUtility.LogWarning(this, $"{nameof(players)} list is null or empty.");
                return;
            }

            foreach (var playerID in players.Keys)
            {
                var resourceKey = new ResourceKey(playerID, type);
                var resourceCount = GameManager.Instance.Resources.Data.FirstOrDefault(resourceKey);
                GameManager.Instance.Resources.Data[resourceKey] = isCleanOut ? 0 : resourceCount - count;
            }

            DebugUtility.LogInvoke(this);
        }
    }

    [Serializable]
    public class PassTurn : GameAction
    {
#if UNITY_EDITOR
        [InfoBox("It passes the turn to the next player or to the first player if the turn hasn't started yet.")]
        [HideLabel] [SerializeField] [ShowOnly] private string infoBoxField;
#endif

        public override void Invoke()
        {
            GameManager.Instance.Turn.PassTurn();

            DebugUtility.LogInvoke(this);
        }
    }

    [Serializable]
    public class CompleteGameSession : GameAction
    {
        public override void Invoke()
        {
            GameManager.Instance.Party.CompleteGameSession();

            DebugUtility.LogInvoke(this);
        }
    }
}