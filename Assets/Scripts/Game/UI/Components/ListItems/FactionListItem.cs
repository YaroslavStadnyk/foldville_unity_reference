using System;
using Core.Extensions;
using Core.Ordinaries;
using Game.Logic.Common.Models;
using Game.Logic.Configs;
using Grid.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Components.ListItems
{
    public class FactionListItem : PoolBehaviour
    {
        [SerializeField] private CardListItem card;
        // [SerializeField] private CostHolder costHolder;
        // [SerializeField] private Button buyButton;

        private TileType _tileType;

        public void Initialize(TileType tileType, Action<TileType> onBuy)
        {
            _tileType = tileType;

            card.Initialize(_tileType, onBuy);
            card.SetCostValue(GameConfig.Instance.BuildingDefinitions.FirstOrDefault(_tileType).Cost, BuildingDefinition.CostType);

            // costHolder.SetCostType(GameManager.Instance.Factions.Definition.CostType);
            // costHolder.SetValue(GameManager.Instance.Factions.Definition.Costs.FirstOrDefault(_tileType));
        }
    }
}