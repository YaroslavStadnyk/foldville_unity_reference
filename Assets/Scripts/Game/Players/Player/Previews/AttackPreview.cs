using System.Collections.Generic;
using Core.Extensions;
using Core.Pooling;
using Game.Environment.Common;
using Game.Logic.Common.Structs;
using Grid.Hexagonal;
using MathModule.Models;
using MathModule.Structs;
using UnityEngine;

namespace Game.Players.Player.Previews
{
    public class AttackPreview : PreviewExtension
    {
        #region Inspector

        [SerializeField] private ObjectPainter hexTileOutlinePrefab;

        [Space] [SerializeField] private Color entryColor = new(1.0f, 0.5f, 0.0f, 1.0f);
        [SerializeField] private float entryFlow = 1.0f;

        [Space] [SerializeField] private Color neutralColor = new(1.0f, 0.5f, 0.0f, 0.25f);
        [SerializeField] private float neutralFlow = 1.0f;

        [Space] [SerializeField] private Color rejectionColor = new(0.0f, 0.0f, 0.0f, 0.25f);
        [SerializeField] private float rejectionFlow = 1.0f;

        #endregion

        private readonly PoolDictionary<ObjectPainter, Int2> _hexTileOutlinePool = new();

        private void Awake()
        {
            _hexTileOutlinePool.prefab = hexTileOutlinePrefab;
            _hexTileOutlinePool.parent = PreviewRoot;
        }

        private void SetupHexTileOutlines(Int2 originIndexPosition, List<AttackPoint> attackPoints)
        {
            _hexTileOutlinePool.ReleaseAll();

            if (attackPoints.IsNullOrEmpty())
            {
                return;
            }

            foreach (var attackPoint in attackPoints)
            {
                var color = attackPoint.isAvailable ? attackPoint.isReasonable ? entryColor : neutralColor : rejectionColor;
                var flow = attackPoint.isAvailable ? attackPoint.isReasonable ? entryFlow : neutralFlow : rejectionFlow;
                var localIndexPosition = attackPoint.indexPosition - originIndexPosition;

                var hexTileOutline = _hexTileOutlinePool.Spawn(localIndexPosition);
                hexTileOutline.SetColor(color, flow);

                HexTile.SetIndexPosition(hexTileOutline.transform, localIndexPosition);
            }
        }

        public void Setup(AttackCoords attackCoords, string playerID)
        {
            if (!IsEnabled)
            {
                return;
            }

            var attackPoints = GameManager.Instance.HexGrid.AttackRuleExecutor.GetAttackPoints(attackCoords, playerID);
            SetupHexTileOutlines(attackCoords.indexPosition, attackPoints);
            HexTile.SetIndexPosition(PreviewRoot, attackCoords.indexPosition);
        }
    }
}