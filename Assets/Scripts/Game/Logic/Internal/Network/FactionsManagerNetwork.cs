using System.Collections.Generic;
using System.Linq;
using Board.Interfaces;
using Board.Structs;
using Core.Extensions;
using Core.Managers;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Models;
using Game.Logic.Common.Structs;
using Game.Logic.Internal.Interfaces;
using Grid.Common;
using Mirror;
using UnityEngine;

namespace Game.Logic.Internal.Network
{
    public class FactionsManagerNetwork : BaseNetwork, IFactionsManager
    {
        public FactionsDefinition Definition { get; set; }

        private readonly SyncDictionary<string, List<TileType>> _origins = new();
        private readonly SyncDictionary<ExplorationKey, string> _explorations = new();

        public IReadOnlyDictionary<string, List<TileType>> Origins => _origins;
        public IReadOnlyDictionary<ExplorationKey, string> Explorations => _explorations;

        private ICardService _cardService;
        private IHandService _handService;

        private void Start()
        {
            _cardService = ServiceManager.Instance.GetService<ICardService>();
            _handService = ServiceManager.Instance.GetService<IHandService>();
        }

        public override void OnStartClient()
        {
            _origins.Callback += OnOriginsChanged;
            _explorations.Callback += OnExplorationsChanged;
        }

        public override void OnStopClient()
        {
            _origins.Callback -= OnOriginsChanged;
            _explorations.Callback -= OnExplorationsChanged;
        }

        private void OnOriginsChanged(SyncIDictionary<string, List<TileType>>.Operation operation, string playerID, List<TileType> types)
        {
            GameEvents.Instance.OnFactionsOriginsChanged?.Invoke(operation.ToType(), playerID, types);
        }

        private void OnExplorationsChanged(SyncIDictionary<ExplorationKey, string>.Operation operation, ExplorationKey key, string itemID)
        {
            GameEvents.Instance.OnFactionsExplorationsChanged?.Invoke(operation.ToType(), key, itemID);
        }

        public void Register(string playerID)
        {
            if (_origins.ContainsKey(playerID))
            {
                return;
            }

            var originPool = Definition.DefaultOrigin.ToList();

            var randomOriginCount = Mathf.Min(Definition.MaxOriginLenght, originPool.Count);
            var randomOrigin = new List<TileType>(randomOriginCount);
            for (var i = 0; i < randomOriginCount; i++)
            {
                var randomPoolIndex = Random.Range(0, originPool.Count);
                randomOrigin.Add(originPool[randomPoolIndex]);
                originPool.RemoveAt(randomPoolIndex);
            }

            _origins[playerID] = randomOrigin;
        }

        public void Unregister(string playerID)
        {
            if (!_origins.Remove(playerID))
            {
                return;
            }

            foreach (var explorationsKey in _explorations.Keys.ToArray())
            {
                if (explorationsKey.playerID == playerID)
                {
                    _explorations.Remove(explorationsKey);
                }
            }
        }

        public bool IsAvailable(string playerID)
        {
            if (playerID == null)
            {
                return false;
            }

            var resourceKey = new ResourceKey(playerID, FactionsDefinition.CostType);
            var resourceCount = GameManager.Instance.Resources.Data?.FirstOrDefault(resourceKey);
            if (resourceCount < FactionsDefinition.Cost)
            {
                return false;
            }

            return !GetOrigin(playerID).IsNullOrEmpty();
        }

        public bool IsAvailable(string playerID, TileType type)
        {
            if (playerID == null)
            {
                return false;
            }

            var resourceKey = new ResourceKey(playerID, FactionsDefinition.CostType);
            var resourceCount = GameManager.Instance.Resources.Data?.FirstOrDefault(resourceKey);
            if (resourceCount < FactionsDefinition.Cost)
            {
                return false;
            }

            return GetOrigin(playerID).Contains(type);
        }

        public List<TileType> GetOrigin(string playerID)
        {
            if (_origins.TryGetValue(playerID, out var origin))
            {
                return origin.ToList();
            }

            Debug.LogError($"{name} {nameof(origin)} of {playerID} is empty.");
            return new List<TileType>();
        }

        public void Explore(string playerID, TileType type)
        {
            if (playerID.IsNullOrEmpty())
            {
                ServerFactionExplored(playerID, new CardInfo(playerID, type), PlayerErrorType.NullReference);
            }

            var isExplorationAvailable = IsAvailable(playerID, type);
            if (!isExplorationAvailable)
            {
                Debug.LogWarning($"Player '{playerID}' can't explore '{type}' faction.");
                ServerFactionExplored(playerID, new CardInfo(playerID, type), PlayerErrorType.UnavailableAction);
                return;
            }

            var isResourceApplied = ApplyResourceToExplore(playerID);
            if (!isResourceApplied)
            {
                Debug.Log($"Player '{playerID}' doesn't have enough resources to explore '{type}' faction.");
                ServerFactionExplored(playerID, new CardInfo(playerID, type), PlayerErrorType.NotEnoughResources);
                return;
            }

            var card = _cardService.CreateCard(type);
            var isCardReceived = _handService.PutCard(playerID, card);
            if (!isCardReceived)
            {
                Debug.Log($"Player '{playerID}' can't explore '{type}' faction because his card wasn't received");
                ServerFactionExplored(playerID, new CardInfo(playerID, type), PlayerErrorType.UnavailableAction);
                return;
            }

            _explorations[new ExplorationKey(playerID, type)] = card.ID;
            _origins[playerID] = GetRandomOrigin(playerID, type);

            ServerFactionExplored(playerID, new CardInfo(playerID, type), PlayerErrorType.None);
        }

        private bool ApplyResourceToExplore(string playerID)
        {
            var resourceKey = new ResourceKey(playerID, FactionsDefinition.CostType);
            var resourceCount = GameManager.Instance.Resources.Data.FirstOrDefault(resourceKey);
            if (resourceCount < FactionsDefinition.Cost)
            {
                return false;
            }

            GameManager.Instance.Resources.Data[resourceKey] = 0; // resourceCount - FactionsDefinition.Cost;
            return true;
        }

        [Server]
        private List<TileType> GetRandomOrigin(string playerID, TileType replacedType)
        {
            var originPool = GetOrigin(playerID);
            originPool.Remove(replacedType);

            if (Definition.Nodes.TryGetValue(replacedType, out var node) && !node.IsNullOrEmpty())
            {
                foreach (var type in node)
                {
                    if (!originPool.Contains(type) && !Explorations.ContainsKey(new ExplorationKey(playerID, type)))
                    {
                        originPool.Add(type);
                    }
                }
            }

            var randomOriginCount = Mathf.Min(Definition.MaxOriginLenght, originPool.Count);
            var randomOrigin = new List<TileType>(randomOriginCount);
            for (var i = 0; i < randomOriginCount; i++)
            {
                var randomPoolIndex = Random.Range(0, originPool.Count);
                randomOrigin.Add(originPool[randomPoolIndex]);
                originPool.RemoveAt(randomPoolIndex);
            }

            return randomOrigin;
        }

        [Server]
        private void ServerFactionExplored(string playerID, CardInfo exploredCardInfo, PlayerErrorType errorType)
        {
            RpcFactionExplored(playerID, exploredCardInfo, errorType);

            if (!NetworkClient.active)
            {
                var handInfo = GameManager.Instance.Board.Hands.FirstOrDefault(playerID);
                GameEvents.Instance.OnFactionExplored?.Invoke(handInfo, exploredCardInfo, errorType);
            }
        }

        [ClientRpc]
        private void RpcFactionExplored(string playerID, CardInfo exploredCardInfo, PlayerErrorType errorType)
        {
            var handInfo = GameManager.Instance.Board.Hands.FirstOrDefault(playerID);
            GameEvents.Instance.OnFactionExplored?.Invoke(handInfo, exploredCardInfo, errorType);
        }
    }
}