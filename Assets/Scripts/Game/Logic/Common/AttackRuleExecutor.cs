using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Game.Environment;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Models;
using Game.Logic.Common.Structs;
using Game.Logic.Configs;
using MathModule.Models;
using MathModule.Structs;
using UnityEngine;

namespace Game.Logic.Common
{
    // TODO refactoring and optimization
    public class AttackRuleExecutor
    {
        private readonly HexGrid _hexGrid;

        public AttackRuleExecutor(HexGrid hexGrid)
        {
            _hexGrid = hexGrid;
        }

        public bool IsAttackAvailable(AttackCoords attackCoords, string playerID, bool isResourcesIgnored = false)
        {
            var indexPosition = attackCoords.indexPosition;
            var tile = _hexGrid.GetTile(indexPosition);
            if (tile == null)
            {
                return false;
            }

            var captureID = GameManager.Instance.HexGrid.GetTileCapture(indexPosition);
            if (!(captureID.IsNullOrEmpty() || captureID == playerID))
            {
                return false;
            }

            var buildingDefinition = tile.GetBuildingDefinition();
            if (buildingDefinition == null || buildingDefinition.FunctionType != FunctionType.Attacking)
            {
                return false;
            }

            if (isResourcesIgnored)
            {
                return true;
            }

            var resourceKey = new ResourceKey(playerID, BuildingDefinition.CostType);
            var resourceValue = GameManager.Instance.Resources.Data.FirstOrDefault(resourceKey);
            if (resourceValue < buildingDefinition.AttackRule.Cost)
            {
                return false;
            }

            return true;
        }

        public void Attack(AttackCoords attackCoords, string playerID)
        {
            var isAttackAvailable = IsAttackAvailable(attackCoords, playerID);
            if (!isAttackAvailable)
            {
                Debug.LogWarning($"The {playerID} attack can't be executed. {attackCoords}");
                return;
            }

            var buildingDefinition = _hexGrid.GetTile(attackCoords.indexPosition)?.GetBuildingDefinition();
            if (buildingDefinition == null)
            {
                Debug.LogError($"{nameof(BuildingDefinition)} is null!");
                return;
            }

            var attackPoints = GetAttackPoints(attackCoords, playerID);
            if (attackPoints.IsNullOrEmpty())
            {
                return;
            }

            var attackedPointsCount = 0;

            var attackRule = buildingDefinition.AttackRule;
            var attackType = attackRule.Type;
            if (attackType == AttackType.Range)
            {
                foreach (var attackPoint in attackPoints)
                {
                    if (!attackPoint.isAvailable || !attackPoint.isReasonable)
                    {
                        continue;
                    }

                    var tileType = _hexGrid.GetTileType(attackPoint.indexPosition);
                    var definition = GameConfig.Instance.BuildingDefinitions?.FirstOrDefault(tileType);
                    var radius = definition?.Radius ?? 0;
                    var form = HexForm.Create(radius);
                    var captureID = _hexGrid.GetTileCapture(attackPoint.indexPosition);

                    _hexGrid.UncaptureTiles(attackPoint.indexPosition, form, captureID, false);
                    _hexGrid.RecreateTile(attackPoint.indexPosition);

                    attackedPointsCount += 1;
                }
            }
            else if (attackType == AttackType.Border)
            {
                foreach (var attackPoint in attackPoints)
                {
                    if (!attackPoint.isAvailable || !attackPoint.isReasonable)
                    {
                        continue;
                    }

                    _hexGrid.CaptureTile(attackPoint.indexPosition, playerID, CaptureType.Attack);

                    attackedPointsCount += 1;
                }
            }

            if (attackedPointsCount <= 0)
            {
                return;
            }

            var resourceKey = new ResourceKey(playerID, BuildingDefinition.CostType);
            var resourceCount = GameManager.Instance.Resources.Data.FirstOrDefault(resourceKey);
            GameManager.Instance.Resources.Data[resourceKey] = resourceCount - attackRule.Cost;
        }

        public List<AttackPoint> GetAttackPoints(AttackCoords attackCoords, string playerID)
        {
            var attackPoints = new List<AttackPoint>();

            var attackRule = _hexGrid.GetTile(attackCoords.indexPosition)?.GetBuildingDefinition()?.AttackRule;
            if (attackRule == null)
            {
                return attackPoints;
            }

            var attackType = attackRule.Type;
            if (attackType == AttackType.Range)
            {
                if (attackCoords.specialIndexPositions.IsNullOrEmpty())
                {
                    return attackPoints;
                }

                var form = HexForm.Create(attackRule.Radius, 0);
                var attackPoint = new AttackPoint();
                attackPoint.indexPosition = attackCoords.specialIndexPositions[0];
                attackPoint.isAvailable = form.Contains(attackPoint.indexPosition - attackCoords.indexPosition);
                attackPoint.isReasonable = IsAttackReasonable(attackPoint.indexPosition, playerID, attackType);

                attackPoints.Add(attackPoint);
            }
            else if (attackType == AttackType.Border)
            {
                var form = HexForm.Create(attackRule.Radius);
                foreach (var localIndexPosition in form)
                {
                    var attackPoint = new AttackPoint();
                    attackPoint.indexPosition = attackCoords.indexPosition + localIndexPosition;
                    attackPoint.isAvailable = true;
                    attackPoint.isReasonable = IsAttackReasonable(attackPoint.indexPosition, playerID, attackType);

                    attackPoints.Add(attackPoint);
                }
            }

            return attackPoints;
        }

        private bool IsAttackReasonable(Int2 indexPosition, string playerID, AttackType attackType)
        {
            var captureID = _hexGrid.GetTileCapture(indexPosition);
            if (captureID == playerID)
            {
                return false;
            }

            var initialTileType = _hexGrid.GetInitialTileType(indexPosition);
            var currentTileType = _hexGrid.GetTile(indexPosition)?.Type;
            if (currentTileType == null)
            {
                return false;
            }

            if (attackType == AttackType.Range && currentTileType == initialTileType)
            {
                return false;
            }

            return true;
        }
    }
}