using Core.Extensions;
using Core.Pooling;
using Game.Environment.Common;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Models;
using Game.Logic.Common.Structs;
using Grid.Common;
using Grid.Hexagonal;
using MathModule.Structs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Players.Player.Previews
{
    public class BonusesPreview : PreviewExtension
    {
        #region Inspector

        [SerializeField] private bool showExtendedBonuses = true;
        [SerializeField] private ObjectBonusLabel labelPrefab;

        [Space] [SerializeField] private Color negativeColor = Color.red;

        [Space] [SerializeField] private bool showLineMarkers = true;
        [ShowIf(nameof(showLineMarkers))] [SerializeField] private LineMarker linePrefab;
        [ShowIf(nameof(showLineMarkers))] [SerializeField] private Vector3 lineStartOffset = Vector3.up * 0.6f;
        [ShowIf(nameof(showLineMarkers))] [SerializeField] private Vector3 lineEndOffset = Vector3.up * 0.75f;

        #endregion

        private readonly PoolDictionary<ObjectBonusLabel, int> _labelPool = new();
        private readonly PoolDictionary<LineMarker, int> _linePool = new();

        private void Awake()
        {
            _labelPool.prefab = labelPrefab;
            _labelPool.parent = PreviewRoot;

            _linePool.prefab = linePrefab;
            _linePool.parent = PreviewRoot;
        }

        private void SetupIndexPosition(Int2 indexPosition, TileType type)
        {
            _labelPool.ReleaseAll();
            _linePool.ReleaseAll();

            var hexGrid = GameManager.Instance.HexGrid;
            if (hexGrid == null)
            {
                return;
            }

            var bonusPoints = hexGrid.BonusRuleCounter.GetContextBonusPoints(indexPosition, type, ContextBehaviour.LatestID);
            if (showExtendedBonuses)
            {
                bonusPoints.AddRange(hexGrid.BonusRuleCounter.GetExtendedBonusPoints(indexPosition, type, ContextBehaviour.LatestID));
            }

            foreach (var bonusPoint in bonusPoints)
            {
                if (bonusPoint.value == 0)
                {
                    continue;
                }

                var isActive = bonusPoint.isAvailable || bonusPoint.captureID.IsNullOrEmpty();
                var isNegative = bonusPoint.value < 0;

                var color = isActive && isNegative ? negativeColor : Color.clear;
                var flow = !isActive || isNegative ? 1.0f : 0.0f;

                var localIndexPosition = bonusPoint.indexPosition - indexPosition;
                if (localIndexPosition != Int2.zero)
                {
                    SetupLine(localIndexPosition, bonusPoint, color, flow);
                }

                SetupLabel(localIndexPosition, bonusPoint, color, flow);
            }
        }

        private void SetupLabel(Int2 localIndexPosition, BonusPoint bonusPoint, Color color, float flow = 1.0f)
        {
            var label = _labelPool.Spawn(_labelPool.SpawnedBehaviours.Count);
            label.SetValue(bonusPoint.value);
            label.SetColor(color, flow);
            label.SetType(BuildingDefinition.CostType);

            HexTile.SetIndexPosition(label.transform, localIndexPosition);
        }

        private void SetupLine(Int2 localIndexPosition, BonusPoint bonusPoint, Color color, float flow = 1.0f)
        {
            if (!showLineMarkers)
            {
                return;
            }

            var localPosition = Vector3.zero.WithXZ(HexTile.ConvertToPosition(localIndexPosition));

            var line = _linePool.Spawn(_linePool.SpawnedBehaviours.Count);
            line.paintGroup.SetColor(color, flow);

            if (bonusPoint.type == InteractionType.Distributing)
            {
                line.SetPositions(lineStartOffset, localPosition + lineEndOffset, Space.Self);
            }
            else
            {
                line.SetPositions(localPosition + lineStartOffset, lineEndOffset, Space.Self);
            }
        }

        private Int2 _lastIndexPosition;
        private TileType _lastTileType;

        public void Setup(Int2 newIndexPosition, TileType newTileType)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (newIndexPosition == _lastIndexPosition && newTileType == _lastTileType)
            {
                return;
            }

            _lastIndexPosition = newIndexPosition;
            _lastTileType = newTileType;

            SetupIndexPosition(newIndexPosition, newTileType);
            HexTile.SetIndexPosition(PreviewRoot, newIndexPosition);
        }
    }
}