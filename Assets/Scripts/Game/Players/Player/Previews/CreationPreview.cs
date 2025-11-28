using Core.Extensions;
using Core.Pooling;
using Game.Environment;
using Game.Environment.Common;
using Game.Environment.Tiles;
using Game.Logic.Common.Enums;
using Game.Logic.Configs;
using Grid.Common;
using Grid.Hexagonal;
using MathModule.Models;
using MathModule.Structs;
using UnityEngine;

namespace Game.Players.Player.Previews
{
    public class CreationPreview : PreviewExtension
    {
        #region Inspector

        [SerializeField] private ObjectPainter hexTileOutlinePrefab;

        [Space] [SerializeField] private HexTileDynamic hexTileDynamicPrefab;
        [SerializeField] private float hexTileDynamicOffsetY = 0.005f;

        [Space] [SerializeField] private Color entryColor = Color.green;
        [SerializeField] private float entryFlow = 1.0f;
        [SerializeField] private float entryFlowOfHexTileDynamic = 0.0f;

        [Space] [SerializeField] private Color rejectionColor = Color.red;
        [SerializeField] private float rejectionFlow = 1.0f;

        #endregion

        private readonly PoolDictionary<ObjectPainter, Int2> _hexTileOutlinePool = new();
        private HexTileDynamic _hexTileDynamicInstance;

        private void Awake()
        {
            _hexTileOutlinePool.prefab = hexTileOutlinePrefab;
            _hexTileOutlinePool.parent = PreviewRoot;

            _hexTileDynamicInstance = Instantiate(hexTileDynamicPrefab, PreviewRoot);
            _hexTileDynamicInstance.transform.localPosition += new Vector3(0, hexTileDynamicOffsetY, 0);
        }

        private void SetupHexTileOutlines(int radius)
        {
            var form = HexForm.Create(radius);
            foreach (var localIndexPosition in form)
            {
                var hexTileOutline = _hexTileOutlinePool.Spawn(localIndexPosition);
                hexTileOutline.SetColor(Color.clear);

                HexTile.SetIndexPosition(hexTileOutline.transform, localIndexPosition);
            }
        }

        private void SetupTileType(TileType tileType)
        {
            _hexTileOutlinePool.ReleaseAll();
            _hexTileDynamicInstance.SetType(tileType);

            var buildingDefinition = GameConfig.Instance.BuildingDefinitions?.FirstOrDefault(tileType);
            if (buildingDefinition != null)
            {
                SetupHexTileOutlines(buildingDefinition.Radius);
            }
        }

        private void SetupIndexPosition(Int2 indexPosition, TileType type, string playerID)
        {
            var hexGrid = GameManager.Instance.HexGrid;
            if (hexGrid == null)
            {
                return;
            }

            SetupSpawnedBehavioursColor(hexGrid, indexPosition, type, playerID);
            SetupHexTileDynamicColor(hexGrid, indexPosition, type, playerID);
        }

        private void SetupSpawnedBehavioursColor(HexGrid hexGrid, Int2 indexPosition, TileType type, string playerID)
        {
            var isCreationAvailable = hexGrid.IsCreationAvailable(indexPosition, type, playerID, true);
            foreach (var (localIndexPosition, hexTileOutline) in _hexTileOutlinePool.SpawnedBehaviours)
            {
                if (!isCreationAvailable)
                {
                    hexTileOutline.SetColor(rejectionColor, rejectionFlow);
                    continue;
                }

                var isCapturingAvailable = hexGrid.IsCapturingAvailable(indexPosition + localIndexPosition, playerID, CaptureType.Persistent);

                var color = isCapturingAvailable ? entryColor : rejectionColor;
                var flow = isCapturingAvailable ? entryFlow : rejectionFlow;
                hexTileOutline.SetColor(color, flow);
            }
        }

        private void SetupHexTileDynamicColor(HexGrid hexGrid, Int2 indexPosition, TileType type, string playerID)
        {
            var isCreationAvailable = hexGrid.IsCreationAvailable(indexPosition, type, playerID, true);

            var color = isCreationAvailable ? entryColor : rejectionColor;
            var flow = isCreationAvailable ? entryFlowOfHexTileDynamic : rejectionFlow;
            _hexTileDynamicInstance.SetColor(color, flow);
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

            if (newTileType != _lastTileType)
            {
                SetupTileType(newTileType);
            }

            _lastIndexPosition = newIndexPosition;
            _lastTileType = newTileType;

            SetupIndexPosition(newIndexPosition, newTileType, ContextBehaviour.LatestID);
            HexTile.SetIndexPosition(PreviewRoot, newIndexPosition);
        }
    }
}