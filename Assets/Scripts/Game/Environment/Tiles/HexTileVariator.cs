using System.Collections.Generic;
using Core.Extensions;
using Game.Environment.Tiles.Models;
using Grid.Common;
using Grid.Hexagonal;
using MathModule.Structs;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Environment.Tiles
{
    [RequireComponent(typeof(HexTile))]
    public class HexTileVariator : SerializedMonoBehaviour
    {
        #region Inspector

        [OdinSerialize] [ReadOnly] private HexTile _hexTile;
        [OdinSerialize] [ReadOnly] private HexTileGrid _hexTileGrid;

        [InfoBox("It makes objects active by tile count around.")]
        [ListDrawerSettings(CustomAddFunction = nameof(AddVariation))]
        [Space] [OdinSerialize] private List<TileVariation> _variations = new();

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            SetupComponents();
        }

        private TileVariation AddVariation()
        {
            return new TileVariation();
        }

        #endregion

        public delegate void VariationsUpdated(IEnumerable<ITile> neighborTiles);
        public event VariationsUpdated OnVariationsUpdated;

        private void Awake()
        {
            SetupComponents();
        }

        private void SetupComponents()
        {
            if (_hexTile == null && !TryGetComponent(out _hexTile))
            {
                Debug.LogError($"{name} {nameof(_hexTile)} is missing.");
            }

            if (_hexTileGrid == null && (transform.parent == null || !transform.parent.TryGetComponent(out _hexTileGrid)))
            {
                // Debug.LogError($"{name} {nameof(_hexTileGrid)} is missing.");
            }
        }

        private bool _isInitialized = false;

        private void Start()
        {
            UpdateVariations();
            UpdateNeighborVariations();

            _isInitialized = true;
        }

        private void OnEnable()
        {
            if (_isInitialized)
            {
                UpdateVariations();
                UpdateNeighborVariations();
            }
        }

        private void OnDisable()
        {
            UpdateNeighborVariations();
        }

        private void UpdateVariations()
        {
            var neighborTiles = GetNeighborTiles();
            foreach (var variation in _variations)
            {
                variation?.UpdateGameObjects(neighborTiles.GetTypes());
            }

            OnVariationsUpdated?.Invoke(neighborTiles);
        }

        private void UpdateNeighborVariations()
        {
            var neighborTiles = GetNeighborTiles();
            if (neighborTiles.IsNullOrEmpty())
            {
                return;
            }

            foreach (var neighborTile in neighborTiles)
            {
                if (neighborTile is HexTile hexTile && hexTile.TryGetComponent<HexTileVariator>(out var hexTileVariator))
                {
                    hexTileVariator.UpdateVariations();
                }
            }
        }

        private List<ITile> GetNeighborTiles()
        {
            return _hexTileGrid == null ? new List<ITile>() : _hexTileGrid.GetTiles(_hexTile.IndexPosition, NeighborForm);
        }

        private static readonly Int2[] NeighborForm =
        {
            new(1, 0),
            new(1, -1),
            new(0, -1),
            new(-1, 0),
            new(-1, 1),
            new(0, 1)
        };
    }
}