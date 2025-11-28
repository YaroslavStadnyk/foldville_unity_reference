using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Board;
using Board.Structs;
using Core.Extensions;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Models;
using Game.Logic.Common.Structs;
using Game.Logic.Configs;
using Game.Players.Player;
using Game.UI.Components.ListItems;
using Game.UI.Components.ListItems.Cards;
using Grid.Common;
using Grid.Hexagonal;
using MathModule.Structs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.UI.Components.Widgets
{
    public class CardsWidget : CardsUI
    {
        [BoxGroup("Animation")] [Space] [SerializeField] private float functionDurationOnSelected = 0.25f;
        [BoxGroup("Animation")] [SerializeField] private Vector3 functionPositionOffsetOnSelected;
        [BoxGroup("Animation")] [SerializeField] private Vector3 functionRotationOffsetOnSelected;

        [SerializeField] private FunctionCardUI functionCardUI;

        private void ShowFunctionCard(TileType tileType)
        {
            if (GameConfig.Instance.BuildingDefinitions.TryGetValue(tileType, out var definition) && definition?.FunctionType == FunctionType.Attacking
                && listItemsPool.SpawnedBehaviours.TryGetValue(tileType, out var listItem) && listItem != null)
            {
                functionCardUI.gameObject.SetActive(true);
                functionCardUI.Initialize(tileType, onAvailableChanged:OnFunctionCardAvailableChanged);
                functionCardUI.IsAvailable = PlayerBehaviour.LocalLatest != null && PlayerBehaviour.LocalLatest.Resources[BuildingDefinition.CostType] >= definition.AttackRule.Cost;

                functionCardUI.LayoutPosition = listItem.LayoutPosition;
                functionCardUI.LayoutRotation = listItem.LayoutRotation;
                functionCardUI.LayoutPositionOffset = Vector3.zero;
                functionCardUI.LayoutRotationOffset = Vector3.zero;
                functionCardUI.DoLayoutPositionOffset(functionPositionOffsetOnSelected, functionDurationOnSelected);
                functionCardUI.DoLayoutRotationOffset(functionRotationOffsetOnSelected, functionDurationOnSelected);
            }
            else
            {
                HideFunctionCard();
            }
        }

        private void HideFunctionCard()
        {
            functionCardUI.gameObject.SetActive(false);
        }

        public override void Hide()
        {
            base.Hide();

            if (PlayerBehaviour.LocalLatest != null)
            {
                PlayerBehaviour.LocalLatest.Selection.CardID = null;
            }

            HideFunctionCard();
        }

        public override void HideAllCards()
        {
            base.HideAllCards();

            if (PlayerBehaviour.LocalLatest != null)
            {
                PlayerBehaviour.LocalLatest.Selection.CardID = null;
            }

            HideFunctionCard();
        }

        protected override void OnCardClicked(TileType tileType)
        {
            if (PlayerBehaviour.LocalLatest != null)
            {
                PlayerBehaviour.LocalLatest.Selection.CardID = PlayerBehaviour.LocalLatest.GetCards()?.FirstOrDefault(tileType).ID;
            }

            // base.OnCardClicked(tileType);
        }

        private void OnSelectedCardChanged(CardInfo oldCardInfo, CardInfo newCardInfo)
        {
            var oldTileType = SelectedTileType;
            var newTileType = newCardInfo == default ? (TileType?)null : newCardInfo.Type;
            SelectedTileType = newTileType;

            if (oldTileType.HasValue)
            {
                UpdateCardAnimation(oldTileType.Value);
            }

            if (newTileType.HasValue)
            {
                UpdateCardAnimation(newTileType.Value);
            }

            HideFunctionCard();
        }

        private void OnSelectedHexTileChanged(HexTile oldHexTile, HexTile newHexTile)
        {
            var oldTileType = SelectedTileType;
            var newTileType = newHexTile == null ? (TileType?)null : newHexTile.Type;
            SelectedTileType = newTileType;

            if (oldTileType.HasValue)
            {
                UpdateCardAnimation(oldTileType.Value);
            }

            if (newTileType.HasValue)
            {
                UpdateCardAnimation(newTileType.Value);
            }

            if (SelectedTileType.HasValue)
            {
                ShowFunctionCard(SelectedTileType.Value);
            }
            else
            {
                HideFunctionCard();
            }
        }

        protected override void OnCardAvailableChanged(TileType tileType, bool isAvailable)
        {
            base.OnCardAvailableChanged(tileType, isAvailable);

            if (isAvailable)
            {
                return;
            }

            if (PlayerBehaviour.LocalLatest == null || PlayerBehaviour.LocalLatest.Selection.CardID.IsNullOrEmpty())
            {
                return;
            }

            if (PlayerBehaviour.LocalLatest.Selection.GetCardInfo().Type == tileType)
            {
                PlayerBehaviour.LocalLatest.Selection.CardID = null;
            }
        }

        private void OnFunctionCardAvailableChanged(TileType tileType, bool isAvailable)
        {
            /* TODO 
            if (isAvailable)
            {
                return;
            }

            if (PlayerBehaviour.LocalLatest == null || PlayerBehaviour.LocalLatest.Selection.HexTile == null)
            {
                return;
            }

            if (PlayerBehaviour.LocalLatest.Selection.HexTile.Type == tileType)
            {
                PlayerBehaviour.LocalLatest.Selection.HexTile = null;
                PlayerBehaviour.LocalLatest.Selection.AttackCoords = default;
            }
            */
        }

        private void OnEnable()
        {
            GameEvents.Instance.OnCardApplied += OnCardApplied;
            GameEvents.Instance.OnCardOwnersChanged += OnCardOwnersChanged;
            GameEvents.Instance.OnTurnChanged += OnTurnChanged;
            GameEvents.Instance.OnFactionsExplorationsChanged += OnFactionsExplorationsChanged;
        }

        private void OnDisable()
        {
            GameEvents.Instance.OnCardApplied -= OnCardApplied;
            GameEvents.Instance.OnCardOwnersChanged -= OnCardOwnersChanged;
            GameEvents.Instance.OnTurnChanged -= OnTurnChanged;
            GameEvents.Instance.OnFactionsExplorationsChanged -= OnFactionsExplorationsChanged;
        }

        private void OnCardApplied(HandInfo handInfo, CardInfo cardInfo, Int2 indexPosition, PlayerErrorType errorType)
        {
            if (errorType is not PlayerErrorType.None)
            {
                return;
            }

            var playerID = handInfo.ID;
            if (playerID.IsNullOrEmpty() || PlayerProfile.LocalLatest == null || !PlayerProfile.LocalLatest.OwnedIDs.Contains(playerID))
            {
                return;
            }

            if (playerID != GameManager.Instance.Turn.CurrentTurn.playerID)
            {
                return;
            }

            RestartUpdateCardUIsCoroutine(playerID);
        }

        private void OnCardOwnersChanged(OperationType operationType, CardInfo cardInfo, string oldOwnerID, string newOwnerID)
        {
            if (operationType is OperationType.Clear)
            {
                HideAllCards();
            }

            var ownerID = operationType is OperationType.Add or OperationType.Set ? newOwnerID : oldOwnerID;
            if (ownerID.IsNullOrEmpty() || PlayerProfile.LocalLatest == null || !PlayerProfile.LocalLatest.OwnedIDs.Contains(ownerID))
            {
                return;
            }

            if (ownerID != GameManager.Instance.Turn.CurrentTurn.playerID)
            {
                return;
            }

            RestartUpdateCardUIsCoroutine(ownerID);
        }

        private void OnTurnChanged(Turn oldTurn, Turn newTurn)
        {
            if (!newTurn.playerID.IsNullOrEmpty() && PlayerProfile.LocalLatest != null && PlayerProfile.LocalLatest.OwnedIDs.Contains(newTurn.playerID))
            {
                SelectedTileType = null;
                HoveredTileType = null;
                ResetCardAnimations();
            }

            if (newTurn.playerID != oldTurn.playerID && PlayerBehaviour.LocalLatest != null)
            {
                PlayerBehaviour.LocalLatest.Selection.CardID = null;
            }

            RestartUpdateCardUIsCoroutine(newTurn.playerID);
        }

        private void OnFactionsExplorationsChanged(OperationType operationType, ExplorationKey explorationKey, string itemID)
        {
            var ownerID = explorationKey.playerID;
            if (ownerID.IsNullOrEmpty() || PlayerProfile.LocalLatest == null || !PlayerProfile.LocalLatest.OwnedIDs.Contains(ownerID))
            {
                return;
            }

            if (ownerID != GameManager.Instance.Turn.CurrentTurn.playerID)
            {
                return;
            }

            RestartUpdateCardUIsCoroutine(ownerID);
        }

        private void UpdateListItemValue(CardListItem listItem, string playerID, TileType type)
        {
            if (PlayerBehaviour.LocalLatest == null || PlayerBehaviour.LocalLatest.LatestID != playerID)
            {
                listItem.ResetValues();
                return;
            }

            var explorations = PlayerBehaviour.LocalLatest.GetExplorations();
            if (!explorations.IsNullOrEmpty() && explorations.Contains(type))
            {
                var cost = GameConfig.Instance.BuildingDefinitions.FirstOrDefault(type).Cost;
                listItem.SetCostValue(cost, BuildingDefinition.CostType);
                listItem.IsAvailable = PlayerBehaviour.LocalLatest.Resources[BuildingDefinition.CostType] >= cost;
                return;
            }

            var cards = PlayerBehaviour.LocalLatest.GetCards();
            if (!cards.IsNullOrEmpty())
            {
                var count = cards.Count(cardInfo => cardInfo.Type == type);
                listItem.SetCountValue(count);
                listItem.IsAvailable = count > 0;
                return;
            }

            listItem.ResetValues();
        }

        private PlayerBehaviour _oldPlayerBehaviour;

        private void UpdateSelectionChangedEvents(PlayerBehaviour playerBehaviour)
        {
            if (playerBehaviour == _oldPlayerBehaviour)
            {
                return;
            }

            if (_oldPlayerBehaviour != null)
            {
                _oldPlayerBehaviour.Selection.OnSelectedCardChanged -= OnSelectedCardChanged;
                _oldPlayerBehaviour.Selection.OnSelectedHexTileChanged -= OnSelectedHexTileChanged;
            }

            playerBehaviour.Selection.OnSelectedCardChanged += OnSelectedCardChanged;
            playerBehaviour.Selection.OnSelectedHexTileChanged += OnSelectedHexTileChanged;
            _oldPlayerBehaviour = playerBehaviour;
        }

        #region Coroutines

        private Coroutine _updateCardUIsCoroutine;

        private IEnumerator UpdateCardUIs(string ownerID)
        {
            if (ownerID.IsNullOrEmpty())
            {
                Hide();
                yield break;
            }

            while (PlayerBehaviour.LocalLatest == null || PlayerBehaviour.LocalLatest.LatestID != ownerID)
            {
                yield return new WaitForEndOfFrame();
            }

            UpdateSelectionChangedEvents(PlayerBehaviour.LocalLatest);

            var cardInfos = PlayerBehaviour.LocalLatest.GetCards();
            if (cardInfos.IsNullOrEmpty())
            {
                HideAllCards();
                yield break;
            }

            foreach (var type in listItemsPool.SpawnedBehaviours.Keys.ToArray())
            {
                if (!cardInfos.Contains(type))
                {
                    HideCard(type);
                }
            }

            foreach (var cardInfo in cardInfos)
            {
                var listItem = ShowCard(cardInfo.Type);
                UpdateListItemValue(listItem, ownerID, cardInfo.Type);
            }

            Show();
        }

        private void RestartUpdateCardUIsCoroutine(string playerID)
        {
            if (_updateCardUIsCoroutine != null)
            {
                StopCoroutine(_updateCardUIsCoroutine);
            }

            _updateCardUIsCoroutine = StartCoroutine(UpdateCardUIs(playerID));
        }

        #endregion
    }
}