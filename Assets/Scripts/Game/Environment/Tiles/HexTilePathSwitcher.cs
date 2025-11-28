using System.Collections.Generic;
using Game.Environment.Tiles.Models;
using Grid.Common;
using Grid.Hexagonal;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Environment.Tiles
{
    [RequireComponent(typeof(HexTile), typeof(HexTileVariator))]
    public class HexTilePathSwitcher : SerializedMonoBehaviour
    {
        #region Inspector

        [OdinSerialize] [ReadOnly] private HexTile _hexTile;
        [OdinSerialize] [ReadOnly] private HexTileVariator _hexTileVariator;

        [Space] [HideLabel] [OdinSerialize] private QuickHexTilePath _quickPath = new();
        [Space] [HideLabel] [OdinSerialize] private ComplexHexTilePath _complexPath = new();

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            SetupComponents();
        }

        #endregion

        private void Awake()
        {
            SetupComponents();
            _quickPath.Initialize();
            _complexPath.Initialize();

            _hexTileVariator.OnVariationsUpdated += OnVariationsUpdated;
        }

        private void OnDestroy()
        {
            _hexTileVariator.OnVariationsUpdated -= OnVariationsUpdated;
        }

        private void SetupComponents()
        {
            if (_hexTile == null && !TryGetComponent(out _hexTile))
            {
                Debug.LogError($"{name} {nameof(_hexTile)} is missing.");
            }

            if (_hexTileVariator == null && !TryGetComponent(out _hexTileVariator))
            {
                Debug.LogError($"{name} {nameof(_hexTileVariator)} is missing.");
            }
        }

        private void OnVariationsUpdated(IEnumerable<ITile> neighborTiles)
        {
            UpdatePath(neighborTiles);
        }

        private void UpdatePath(IEnumerable<ITile> neighborTiles)
        {
            _quickPath.Reset();

            var pathIndex = GetPathIndex(neighborTiles);
            if (!_complexPath.Switch(pathIndex))
            {
                _quickPath.Switch(pathIndex);
            }
        }

        private List<int> GetPathIndex(IEnumerable<ITile> neighborTiles)
        {
            var pathIndex = new List<int>();
            var directionIndex = 0;
            foreach (var tile in neighborTiles)
            {
                if (tile != null && tile.Type == _hexTile.Type)
                {
                    pathIndex.Add(directionIndex);
                }

                directionIndex += 1;
            }

            return pathIndex;
        }
    }
}