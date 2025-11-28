using System;
using System.Collections.Generic;
using Core;
using Core.Extensions;
using Core.Interfaces;
using Game.Configs;
using Game.Environment.Common;
using Game.Players.Player;
using Grid.Hexagonal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Environment
{
    public class HexGridPainter : MonoBehaviour
    {
        private readonly Dictionary<HexTile, ObjectPainter> _hexTilePaintersDictionary = new();

        #region Inspector

        [NonSerialized] [ShowInInspector] [ReadOnly] private HexGrid _hexGrid;

        [Space] [SerializeField] private float neutralColorFlow = 0.75f;
        [HideIf("@" + nameof(neutralColorFlow) + " == 0.0f")]
        [SerializeField] private Color neutralColor = Color.grey;

        [Space] [SerializeField] private float captureColorsFlow = 1.0f;
        [HideIf("@" + nameof(captureColorsFlow) + " == 0.0f")]
        [ShowInInspector] private IReadOnlyList<Color> CaptureColors => GUIConfig.Instance.PlayerColors;

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            if (Application.isPlaying || gameObject.IsPrefab())
            {
                return;
            }

            _hexGrid = GameManager.Instance.HexGrid;
        }

        #endregion

        private void Awake()
        {
            _hexGrid = GameManager.Instance.HexGrid;

            foreach (var tile in _hexGrid.GetTiles())
            {
                OnTileCreated(tile as HexTile);
            }
        }

        private void OnEnable()
        {
            _hexGrid.OnTileCreated += OnTileCreated;
            _hexGrid.OnTileRemoved += OnTileRemoved;
            _hexGrid.OnTileCaptureChanged += OnTileCaptureChanged;
        }

        private void OnDisable()
        {
            _hexGrid.OnTileCreated -= OnTileCreated;
            _hexGrid.OnTileRemoved -= OnTileRemoved;
            _hexGrid.OnTileCaptureChanged -= OnTileCaptureChanged;
        }

        private void OnTileCreated(HexTile hexTile)
        {
            if (hexTile.TryGetComponent<ObjectPainter>(out var hexTilePainter))
            {
                _hexTilePaintersDictionary[hexTile] = hexTilePainter;

                var indexPosition = hexTile.IndexPosition;
                var captureID = _hexGrid.GetTileCapture(indexPosition);

                SetupHexTilePainter(hexTilePainter, captureID);
            }
        }

        private void OnTileRemoved(HexTile hexTile)
        {
            _hexTilePaintersDictionary.Remove(hexTile);
        }

        private void OnTileCaptureChanged(HexTile hexTile, string oldCaptureId, string newCaptureId)
        {
            if (_hexTilePaintersDictionary.TryGetValue(hexTile, out var hexTilePainter))
            {
                SetupHexTilePainter(hexTilePainter, newCaptureId);
            }
        }

        private void SetupHexTilePainter(IPaintable hexTilePainter, string captureID)
        {
            var color = GetCaptureColor(captureID);
            var flow = GetCaptureFlow(captureID);

            hexTilePainter.SetColor(color, flow);
        }

        private Color GetCaptureColor(string captureID)
        {
            return captureID.IsNullOrEmpty() ? neutralColor : PlayerProfile.GetColor(captureID);
        }

        private float GetCaptureFlow(string captureID)
        {
            if (captureID.IsNullOrEmpty())
            {
                return neutralColorFlow;
            }

            return captureColorsFlow;
        }
    }
}