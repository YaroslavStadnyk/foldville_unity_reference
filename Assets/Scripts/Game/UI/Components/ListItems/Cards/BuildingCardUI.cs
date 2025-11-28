using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using DG.Tweening;
using Game.Configs;
using Game.Logic.Common.Models;
using Game.Logic.Configs;
using Grid.Common;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Components.ListItems.Cards
{
    public class BuildingCardUI : CardListItem
    {
        #region Inspector

        [Space] [SerializeField] private Image buildingImage;
        [SerializeField] private TMP_Text statsDescription;
        [SerializeField] private TMP_Text additionalStatsDescription;
        [SerializeField] [Range(0, 1)] private float statsLabelsAlpha = 0.5f;

        [Space] [SerializeField] private bool flippingEnabled = true;
        [SerializeField] private RectTransform flippingRoot;
        [SerializeField] private CanvasGroup frontSideGroup;
        [SerializeField] private CanvasGroup backSideGroup;
        [SerializeField] private float flippingDuration = 0.1f;
        [SerializeField] private Ease flippingEase = Ease.InOutSine;

        #endregion

        public override void Initialize(TileType tileType, Action<TileType> onClick = null, Action<TileType, bool> onHover = null, Action<TileType, bool> onAvailableChanged = null)
        {
            base.Initialize(tileType, onClick, onHover, onAvailableChanged);

            buildingImage.sprite = GUIConfig.Instance.TileTypeUIPresets.FirstOrDefault(tileType).originalIcon;
            buildingImage.gameObject.SetActive(buildingImage.sprite != null);
            statsDescription.text = GetStatsText(tileType);
            additionalStatsDescription.text = GetAdditionalStatsText(tileType);

            ResetFlip();
        }

        #region StatsText

        private string GetStatsText(TileType tileType)
        {
            if (!GameConfig.Instance.BuildingDefinitions.TryGetValue(tileType, out var definition))
            {
                return "";
            }

            var bonusText = definition.BonusRule.Value.ToString("+#;-#;0");
            var resourceTypeText = BuildingDefinition.CostType.ToString().ToLower();
            var interactionRadiusText = definition.Radius;

            var bonusLine = $"{resourceTypeText}: ".ToRichAlpha(statsLabelsAlpha) + bonusText + $" {Emojis.EnergyCoin}";
            var radiusLine = $"radius: ".ToRichAlpha(statsLabelsAlpha) + interactionRadiusText;

            return definition.BonusRule.Value == 0 ? $"{radiusLine}" : $"{bonusLine}\n{radiusLine}";
        }

        private string GetAdditionalStatsText(TileType tileType)
        {
            if (!GameConfig.Instance.BuildingDefinitions.TryGetValue(tileType, out var definition))
            {
                return "";
            }

            var hexGrid = GameManager.Instance.HexGrid;
            if (hexGrid == null)
            {
                return "";
            }

            var contextBonuses = hexGrid.BonusRuleCounter.GetContextBonuses(tileType);
            var extendedBonuses = hexGrid.BonusRuleCounter.GetExtendedBonuses(tileType);
            var bonuses = MergeBonuses(contextBonuses, extendedBonuses)?.OrderByDescending(pair => pair.Value);

            var placementText = definition.PositionRule.RequiredUnderlay.ToStringValues();
            var bonusesText = bonuses?.ToStringValues(InteractionPairSelector, "", "", "\n");

            var placementLine = $"placement: \n".ToRichAlpha(statsLabelsAlpha) + placementText;
            var bonusesLine = (bonusesText.IsNullOrEmpty() ? "no interactions" : "interactions: \n").ToRichAlpha(statsLabelsAlpha) + bonusesText;

            return $"{placementLine}\n\n{bonusesLine}";
        }

        private IReadOnlyCollection<KeyValuePair<TileType, int>> MergeBonuses(IReadOnlyDictionary<TileType, int> contextBonuses, IReadOnlyDictionary<TileType, int> extendedBonuses)
        {
            if (contextBonuses.IsNullOrEmpty() || extendedBonuses.IsNullOrEmpty())
            {
                return contextBonuses.IsNullOrEmpty() ? extendedBonuses : contextBonuses;
            }

            var bonuses = new List<KeyValuePair<TileType, int>>(contextBonuses.Count + extendedBonuses.Count);

            foreach (var (tileType, bonus) in contextBonuses)
            {
                var pair = new KeyValuePair<TileType, int>(tileType, bonus);
                bonuses.Add(pair);
            }

            foreach (var (tileType, bonus) in extendedBonuses)
            {
                var pair = new KeyValuePair<TileType, int>(tileType, bonus);
                if (!bonuses.Contains(pair))
                {
                    bonuses.Add(new KeyValuePair<TileType, int>(tileType, bonus));
                }
            }

            return bonuses;
        }

        private string InteractionPairSelector(int i, TileType tileType, int value)
        {
            return $"{tileType}		{value.ToString("+#;-#;0")} {Emojis.EnergyCoin}";
        }

        #endregion

        public bool IsFlippingEnabled
        {
            get => flippingEnabled;
            set
            {
                flippingEnabled = value;
                if (!flippingEnabled)
                {
                    ResetFlip();
                }
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            if (flippingEnabled)
            {
                DoFlip(1f, flippingDuration, flippingEase);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            DoFlip(0f, flippingDuration, flippingEase);
        }

        #region Tweeners

        private Tweener _flipTweener;

        private Tweener DoFlip(float value, float duration, Ease ease)
        {
            if (_flipTweener.IsActive())
            {
                _flipTweener.ChangeEndValue(value, duration, true)
                    .SetEase(ease)
                    .Restart();
            }
            else
            {
                _flipTweener = DOTween.To(FlipSetter, 0f, value, duration)
                    .SetEase(ease)
                    .SetUpdate(true)
                    .SetLink(gameObject)
                    .SetAutoKill(false);
            }

            return _flipTweener;
        }

        private void FlipSetter(float t)
        {
            var angle = (1f - Mathf.Abs(t - 0.5f) * 2f) * 90f;
            flippingRoot.localRotation = Quaternion.Euler(flippingRoot.localRotation.eulerAngles.WithY(angle));

            var alpha = Mathf.Abs(t) > 0.5f ? 1f : 0f;
            frontSideGroup.alpha = 1f - alpha;
            backSideGroup.alpha = alpha;
        }

        public void ResetFlip()
        {
            DoFlip(0f, gameObject.activeSelf ? flippingDuration : 0f, flippingEase);
        }

        #endregion
    }
}