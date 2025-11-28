using System.Collections.Generic;
using Core.Extensions;
using Game.Environment;
using Game.Logic.Common.Models;
using Game.Logic.Configs;
using Grid.Common;
using MathModule.Models;
using MathModule.Structs;
using UnityEngine;

namespace Game.Logic.Common
{
    public class PositionRuleChecker
    {
        private readonly HexGrid _hexGrid;

        public PositionRuleChecker(HexGrid hexGrid)
        {
            _hexGrid = hexGrid;
        }

        public bool Check(Int2 indexPosition, TileType type)
        {
            var buildingDefinition = GameConfig.Instance.BuildingDefinitions?.FirstOrDefault(type);
            if (buildingDefinition == null)
            {
                Debug.LogError($"{nameof(BuildingDefinition)} of type {type} not found!");
                return false;
            }

            var positionRule = buildingDefinition.PositionRule;
            if (positionRule == null)
            {
                Debug.LogError($"{nameof(PositionRule)} of type {type} not found!");
                return false;
            }

            return HasRequiredUnderlay(indexPosition, positionRule) && HasRequiredNeighbors(indexPosition, positionRule);
        }

        private bool HasRequiredUnderlay(Int2 indexPosition, PositionRule positionRule)
        {
            var underTile = _hexGrid.GetTile(indexPosition);
            if (underTile == null)
            {
                return false;
            }

            var hasRequiredUnderlay = positionRule.RequiredUnderlay.Contains(underTile.Type);
            return hasRequiredUnderlay;
        }

        private bool HasRequiredNeighbors(Int2 indexPosition, PositionRule positionRule)
        {
            var hasRequiredNeighbors = ContainsNeighbors(indexPosition, positionRule.RequiredNeighbors);
            return hasRequiredNeighbors;
        }

        private bool ContainsNeighbors(Int2 indexPosition, IReadOnlyDictionary<TileType, int> requiredNeighbors)
        {
            foreach (var (requiredType, radius) in requiredNeighbors)
            {
                var form = HexForm.Create(radius, 0);
                var neighborTiles = _hexGrid.GetTiles(indexPosition, form);

                var hasRequiredNeighbor = neighborTiles.Contains(requiredType);
                if (!hasRequiredNeighbor)
                {
                    return false;
                }
            }

            return true;
        }
    }
}