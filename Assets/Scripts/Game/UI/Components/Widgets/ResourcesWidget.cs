using Board.Structs;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Models;
using Game.Logic.Common.Structs;
using Game.Players.Player;
using MathModule.Structs;
using UnityEngine;

namespace Game.UI.Components.Widgets
{
    public class ResourcesWidget : ResourcesUI
    {
        [SerializeField] private ResourceType[] shownResources = {};

        protected override void Awake()
        {
            base.Awake();

            ShowResources(shownResources);
        }

        private void OnEnable()
        {
            GameEvents.Instance.OnResourceChanged += OnResourceChanged;
            GameEvents.Instance.OnTurnChanged += OnTurnChanged;
            GameEvents.Instance.OnCardApplied += OnCardApplied;
        }

        private void OnDisable()
        {
            GameEvents.Instance.OnResourceChanged -= OnResourceChanged;
            GameEvents.Instance.OnTurnChanged -= OnTurnChanged;
            GameEvents.Instance.OnCardApplied -= OnCardApplied;
        }

        private void OnResourceChanged(OperationType operationType, ResourceKey resourceKey, int oldValue, int newValue)
        {
            if (PlayerProfile.LocalLatest == null)
            {
                return;
            }

            var currentPlayerID = GameManager.Instance.Turn.CurrentTurn.playerID;
            if (currentPlayerID != resourceKey.holderID && PlayerProfile.IsLocalID(currentPlayerID))
            {
                return;
            }

            if (PlayerProfile.IsLocalID(resourceKey.holderID))
            {
                SetResourceValue(resourceKey.type, newValue);
            }
        }

        private void OnTurnChanged(Turn oldTurn, Turn newTurn)
        {
            if (PlayerProfile.IsLocalID(newTurn.playerID))
            {
                UpdateResources(newTurn.playerID);
            }
            else if (PlayerProfile.IsLocalID(oldTurn.playerID))
            {
                UpdateResources(oldTurn.playerID);
            }
        }

        private void OnCardApplied(HandInfo hand, CardInfo card, Int2 indexPosition, PlayerErrorType errorType)
        {
            if (errorType is not PlayerErrorType.NotEnoughResources)
            {
                return;
            }

            if (listItemsPool.SpawnedBehaviours.TryGetValue(BuildingDefinition.CostType, out var listItem) && listItem != null && listItem.gameObject.activeSelf)
            {
                listItem.PlayErrorFeedback();
            }
        }

        private void UpdateResources(string playerID)
        {
            foreach (var (resourceKey, value) in GameManager.Instance.Resources.Data)
            {
                if (resourceKey.holderID == playerID)
                {
                    SetResourceValue(resourceKey.type, value);
                    SetResourceMaxValue(resourceKey.type, value); // TODO implement Resources Data max value
                }
            }
        }
    }
}
