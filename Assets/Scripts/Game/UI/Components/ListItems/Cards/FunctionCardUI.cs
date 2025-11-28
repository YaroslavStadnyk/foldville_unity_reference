using System;
using Core.Extensions;
using Game.Configs;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Models;
using Game.Logic.Configs;
using Grid.Common;
using TMPro;
using UnityEngine;

namespace Game.UI.Components.ListItems.Cards
{
    public class FunctionCardUI : CardListItem
    {
        [Space] [SerializeField] private TMP_Text functionDescription;
        [SerializeField] private TMP_Text controlDescription;
        [SerializeField] private TMP_Text statsDescription;
        [SerializeField] [Range(0, 1)] private float statsLabelsAlpha = 0.5f;

        public override void Initialize(TileType tileType, Action<TileType> onClick = null, Action<TileType, bool> onHover = null, Action<TileType, bool> onAvailableChanged = null)
        {
            base.Initialize(tileType, onClick, onHover, onAvailableChanged);

            if (!GameConfig.Instance.BuildingDefinitions.TryGetValue(tileType, out var definition))
            {
                return;
            }

            var attackType = definition.AttackRule.Type;
            var attackTypeUIPreset = GUIConfig.Instance.AttackTypeUIPresets.FirstOrDefault(attackType);

            if (definition.FunctionType == FunctionType.Attacking)
            {
                subLabel.text = $"{attackType.ToString().ToLower()}-attack";
                subIcon.sprite = attackTypeUIPreset.originalIcon;
            }

            functionDescription.text = attackTypeUIPreset.functionDescription;
            controlDescription.text = attackTypeUIPreset.controlDescription;
            statsDescription.text = GetStatsText(tileType);

            SetCostValue(definition.AttackRule.Cost, BuildingDefinition.CostType);
        }

        private string GetStatsText(TileType tileType)
        {
            if (!GameConfig.Instance.BuildingDefinitions.TryGetValue(tileType, out var definition))
            {
                return "";
            }

            var attackRadius = definition.AttackRule.Radius;
            var attackRadiusStatLine = $"radius: ".ToRichAlpha(statsLabelsAlpha) + attackRadius;
            return $"{attackRadiusStatLine}";
        }
    }
}