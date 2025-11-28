using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Game.Environment;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Models;
using Game.Logic.Common.Structs;
using Game.Logic.Configs;
using Grid.Common;
using MathModule.Models;
using MathModule.Structs;
using UnityEngine;

namespace Game.Logic.Common
{
    // TODO improve performance of methods
    public class BonusRuleCounter
    {
        private readonly HexGrid _hexGrid;

        public BonusRuleCounter(HexGrid hexGrid)
        {
            _hexGrid = hexGrid;
        }

        public int Total(string playerID)
        {
            var totalBonus = 0;
            foreach (var (indexPosition, captureID) in _hexGrid.GetTileCaptures())
            {
                if(playerID != captureID)
                {
                    continue;
                }

                var tile = _hexGrid.GetTile(indexPosition);
                if (tile == null)
                {
                    continue;
                }

                totalBonus += _hexGrid.BonusRuleCounter.Count(indexPosition, tile.Type, captureID);
            }

            return totalBonus;
        }

        public int Count(Int2 indexPosition, TileType type, string playerID)
        {
            var totalBonus = 0;

            var contextBonusPoints = GetContextBonusPoints(indexPosition, type, playerID);
            foreach (var bonusPoint in contextBonusPoints)
            {
                if (bonusPoint.isAvailable && bonusPoint.type == InteractionType.Collecting)
                {
                    totalBonus += bonusPoint.value;
                }
            }

            var extendedBonusPoints = GetExtendedBonusPoints(indexPosition, type, playerID);
            foreach (var bonusPoint in extendedBonusPoints)
            {
                if (bonusPoint.isAvailable && bonusPoint.type == InteractionType.Distributing)
                {
                    totalBonus += bonusPoint.value;
                }
            }

            var bonusRule = GameConfig.Instance.BuildingDefinitions?.FirstOrDefault(type)?.BonusRule;
            if (bonusRule == null)
            {
                return totalBonus;
            }

            return Mathf.Clamp(totalBonus, bonusRule.ValueMin, bonusRule.ValueMax);
        }

        public List<BonusPoint> GetContextBonusPoints(Int2 indexPosition, TileType type, string playerID)
        {
            var bonusPoints = new List<BonusPoint>();

            var buildingDefinitions = GameConfig.Instance.BuildingDefinitions?.FirstOrDefault(type);
            if (buildingDefinitions == null)
            {
                return bonusPoints;
            }

            var bonusRule = buildingDefinitions.BonusRule;
            var form = HexForm.Create(buildingDefinitions.Radius, 0);
            foreach (var localIndexPosition in form)
            {
                var tileIndexPosition = indexPosition + localIndexPosition;
                var tile = _hexGrid.GetTile(tileIndexPosition);
                if (tile != null && TryGetBonusPoint(bonusRule, tileIndexPosition, tile.Type, playerID, out var bonusPoint))
                {
                    bonusPoints.Add(bonusPoint);
                }
            }

            var selfBonusPoint = new BonusPoint();
            selfBonusPoint.type = InteractionType.Collecting;
            selfBonusPoint.indexPosition = indexPosition;
            selfBonusPoint.captureID = _hexGrid.GetTileCapture(indexPosition);
            selfBonusPoint.isAvailable = true;
            selfBonusPoint.value = bonusRule.Value;

            bonusPoints.Add(selfBonusPoint);

            return bonusPoints;
        }

        public IReadOnlyDictionary<TileType, int> GetContextBonuses(TileType type)
        {
            return GameConfig.Instance.BuildingDefinitions?.FirstOrDefault(type)?.BonusRule?.InteractionPairs;
        }

        public List<BonusPoint> GetExtendedBonusPoints(Int2 indexPosition, TileType type, string playerID)
        {
            var bonusPoints = new List<BonusPoint>();

            var tiles = _hexGrid.GetTileTypes();
            foreach (var (tileIndexPosition, tileType) in tiles)
            {
                var tileBuildingDefinition = GameConfig.Instance.BuildingDefinitions?.FirstOrDefault(tileType);
                if (tileBuildingDefinition == null)
                {
                    continue;
                }

                var tileBonusRule = tileBuildingDefinition.BonusRule;
                var form = HexForm.Create(tileBuildingDefinition.Radius, 0);
                if (!form.Contains(tileIndexPosition - indexPosition))
                {
                    continue;
                }

                if (TryGetBonusPoint(tileBonusRule, tileIndexPosition, type, playerID, out var bonusPoint))
                {
                    bonusPoints.Add(bonusPoint);
                }
            }

            return bonusPoints;
        }

        public IReadOnlyDictionary<TileType, int> GetExtendedBonuses(TileType type)
        {
            var extendedBonuses = new Dictionary<TileType, int>();
            foreach (var (tileType, definition) in GameConfig.Instance.BuildingDefinitions)
            {
                if (definition == null)
                {
                    continue;
                }

                foreach (var (interactedType, value) in definition.BonusRule.InteractionPairs)
                {
                    if (interactedType != type)
                    {
                        continue;
                    }

                    extendedBonuses.Add(tileType, value);
                }
            }

            return extendedBonuses;
        }

        private bool TryGetBonusPoint(BonusRule bonusRule, Int2 indexPosition, TileType type, string playerID, out BonusPoint bonusPoint)
        {
            if (bonusRule.InteractionPairs.TryGetValue(type, out var value))
            {
                var captureID = _hexGrid.GetTileCapture(indexPosition);

                bonusPoint = new BonusPoint();
                bonusPoint.type = bonusRule.InteractionType;
                bonusPoint.indexPosition = indexPosition;
                bonusPoint.captureID = captureID;
                bonusPoint.isAvailable = (bonusRule.InteractionTarget == InteractionTarget.Own && captureID == playerID) || bonusRule.InteractionTarget == InteractionTarget.Any;
                bonusPoint.value = value;

                return true;
            }

            bonusPoint = default;
            return false;
        }
    }
}