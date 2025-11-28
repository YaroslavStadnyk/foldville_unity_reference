using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Game.Logic.API;
using Game.Logic.Common;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Models;
using Game.Logic.Common.Structs;
using Game.Logic.Configs;
using Grid.Common;
using Grid.Hexagonal;
using MathModule.Models;
using MathModule.Structs;
using UnityEngine;

namespace Game.Environment
{
    public class HexGrid : HexTileGrid
    {
        [SerializeField] private List<TileType> recreationalTypes = new()
        {
            TileType.Ground,
            TileType.Hills,
            TileType.Water
        };

        public delegate void TileCaptureChanged(HexTile hexTile, string oldCaptureID, string newCaptureID);
        public event TileCaptureChanged OnTileCaptureChanged = null;

        public AttackRuleExecutor AttackRuleExecutor { get; private set; }
        public BonusRuleCounter BonusRuleCounter { get; private set; }
        public PositionRuleChecker PositionRuleChecker { get; private set; }

        private static GridDataManager GridData => GameManager.Instance.GridData;

        protected override void Awake()
        {
            base.Awake();

            InitialTypes = new Dictionary<Int2, TileType>(GetTileTypes());
            GridData.InitialTypes = InitialTypes;
        }

        private void Start()
        {
            AttackRuleExecutor = new AttackRuleExecutor(this);
            BonusRuleCounter = new BonusRuleCounter(this);
            PositionRuleChecker = new PositionRuleChecker(this);
        }

        private void OnEnable()
        {
            GameEvents.Instance.OnTileTypeChanged += OnTypeChanged;
            GameEvents.Instance.OnTileCaptureChanged += OnCaptureChanged;
        }

        private void OnDisable()
        {
            GameEvents.Instance.OnTileTypeChanged -= OnTypeChanged;
            GameEvents.Instance.OnTileCaptureChanged -= OnCaptureChanged;
        }

        private void OnTypeChanged(OperationType operationType, Int2 indexPosition, TileType oldTileType, TileType newTileType, string captureID)
        {
            switch (operationType)
            {
                case OperationType.Add or OperationType.Set:
                    CreateTileInternal(indexPosition, newTileType);
                    break;
                case OperationType.Remove:
                    RemoveTileInternal(indexPosition);
                    break;
                case OperationType.Clear:
                    RemoveTilesInternal();
                    break;
                default:
                    break;
            }
        }

        private void OnCaptureChanged(OperationType operationType, Int2 indexPosition, TileType tileType, string oldCaptureID, string newCaptureID)
        {
            switch (operationType)
            {
                case OperationType.Add or OperationType.Set:
                    CaptureTileInternal(indexPosition, newCaptureID);
                    break;
                case OperationType.Remove:
                    UncaptureTileInternal(indexPosition);
                    break;
                case OperationType.Clear:
                    UncaptureTilesInternal();
                    break;
                default:
                    break;
            }
        }

        #region Capture

        protected readonly HexMap<string> HexCaptureMap = new();

        public bool IsCapturingAvailable(Int2 indexPosition, string captureID = null, CaptureType captureType = CaptureType.Default)
        {
            var tile = GetTile(indexPosition);
            if (tile == null)
            {
                Debug.Log($"The {nameof(indexPosition)} can't be captured by '{captureID}'. There is no a {nameof(tile)} on that {nameof(indexPosition)}.");
                return false;
            }

            if (captureType == CaptureType.Attack)
            {
                return true;
            }

            var oldCaptureID = HexCaptureMap.Get(indexPosition);
            if (oldCaptureID.IsNullOrEmpty() && (captureID.IsNullOrEmpty() || captureType == CaptureType.Persistent))
            {
                return true;
            }

            var isOwner = captureID == oldCaptureID;
            var isNewbie = HexCaptureMap.Cells.ContainsValue(captureID) == false;
            if (isOwner || (isNewbie && oldCaptureID.IsNullOrEmpty()))
            {
                return true;
            }

            Debug.Log($"The {nameof(indexPosition)} can't be captured by '{captureID}'. It's already captured by '{oldCaptureID}'.");
            return false;
        }

        public bool IsCompletelyCaptured()
        {
            if (GridData.Captures.Count != GridData.Types.Count)
            {
                return false;
            }

            foreach (var captureID in GridData.Captures.Values)
            {
                if (captureID.IsNullOrEmpty())
                {
                    return false;
                }
            }

            return true;
        }

        public bool HasTileCapture(string captureID)
        {
            return HexCaptureMap.Cells.ContainsValue(captureID);
        }

        public string GetTileCapture(Int2 indexPosition)
        {
            return HexCaptureMap.Get(indexPosition);
        }

        public Dictionary<Int2, string> GetTileCaptures()
        {
            return new Dictionary<Int2, string>(GridData.Captures);
        }

        private bool CaptureTileInternal(Int2 indexPosition, string captureID)
        {
            var oldCaptureID = HexCaptureMap.Get(indexPosition);
            if (oldCaptureID == captureID)
            {
                return false;
            }

            HexCaptureMap.Set(indexPosition, captureID);

            var hexTile = GetTile(indexPosition) as HexTile;
            OnTileCaptureChanged?.Invoke(hexTile, oldCaptureID, captureID);
            return true;

        }

        public bool CaptureTile(Int2 indexPosition, string captureID, CaptureType captureType = CaptureType.Default)
        {
            var isCapturingAvailable = IsCapturingAvailable(indexPosition, captureID, captureType);
            if (!isCapturingAvailable)
            {
                return false;
            }

            GridData.Captures[indexPosition] = captureID;
            return true;
        }

        public void CaptureTiles(Int2 indexPosition, string captureID, IEnumerable<Int2> form, CaptureType captureType = CaptureType.Default)
        {
            foreach (var localIndexPosition in form)
            {
                CaptureTile(indexPosition + localIndexPosition, captureID, captureType);
            }
        }

        public bool CaptureTiles(Int2 indexPosition, string captureID, TileType type)
        {
            var isCapturingAvailable = IsCapturingAvailable(indexPosition, captureID);
            if (!isCapturingAvailable)
            {
                return false;
            }

            var buildingDefinition = GameConfig.Instance.BuildingDefinitions?.FirstOrDefault(type);
            if (buildingDefinition == null)
            {
                Debug.LogError($"{nameof(BuildingDefinition)} of type {type} not found!", this);
                return false;
            }

            var captureForm = HexForm.Create(buildingDefinition.Radius);
            CaptureTiles(indexPosition, captureID, captureForm, CaptureType.Persistent);

            return true;
        }

        #endregion

        #region Uncapture

        private bool UncaptureTileInternal(Int2 indexPosition)
        {
            if (HexCaptureMap.Remove(indexPosition, out var oldCaptureID))
            {
                var hexTile = GetTile(indexPosition) as HexTile;
                OnTileCaptureChanged?.Invoke(hexTile, oldCaptureID, null);
                return true;
            }

            return false;
        }

        private void UncaptureTilesInternal()
        {
            foreach (var (indexPosition, oldCaptureID) in HexCaptureMap.Cells)
            {
                var hexTile = GetTile(indexPosition) as HexTile;
                OnTileCaptureChanged?.Invoke(hexTile, oldCaptureID, null);
            }

            HexCaptureMap.Clear();
        }

        public bool UncaptureTile(Int2 indexPosition)
        {
            return GridData.Captures.Remove(indexPosition);
        }

        public void UncaptureTiles(Int2 indexPosition, IEnumerable<Int2> form)
        {
            foreach (var localIndexPosition in form)
            {
                UncaptureTile(indexPosition + localIndexPosition);
            }
        }

        public void UncaptureTiles(string captureIDToRemove)
        {
            foreach (var (indexPosition, captureID) in GridData.Captures.ToArray())
            {
                if (captureID == captureIDToRemove)
                {
                    UncaptureTile(indexPosition);
                }
            }
        }

        public void UncaptureTiles(Int2 indexPosition, IEnumerable<Int2> form, string captureIDToRemove, bool ignoreLayering = false)
        {
            if (ignoreLayering)
            {
                foreach (var localIndexPosition in form)
                {
                    var captureID = GetTileCapture(indexPosition + localIndexPosition);
                    if (captureID == captureIDToRemove)
                    {
                        UncaptureTile(indexPosition + localIndexPosition);
                    }
                }
            }
            else
            {
                var filteredForm = form.ToList();

                filteredForm.RemoveAll(localIndexPosition =>
                {
                    var captureID = GetTileCapture(indexPosition + localIndexPosition);
                    return captureID != captureIDToRemove;
                });

                foreach (var (indexPositionToExcept, captureID) in GridData.Captures)
                {
                    if (captureID != captureIDToRemove || indexPositionToExcept == indexPosition)
                    {
                        continue;
                    }

                    if (!GridData.Types.TryGetValue(indexPositionToExcept, out var tileType))
                    {
                        continue;
                    }

                    if (!GameConfig.Instance.BuildingDefinitions.TryGetValue(tileType, out var buildingDefinition) || buildingDefinition == null)
                    {
                        continue;
                    }

                    var buildingForm = HexForm.Create(buildingDefinition.Radius);
                    foreach (var localIndexPositionToExcept in buildingForm)
                    {
                        var localIndexPosition = (indexPositionToExcept + localIndexPositionToExcept) - indexPosition;
                        filteredForm.Remove(localIndexPosition);
                    }
                }

                foreach (var localIndexPosition in filteredForm)
                {
                    UncaptureTile(indexPosition + localIndexPosition);
                }
            }
        }

        #endregion

        #region Recreate

        private IDictionary<Int2, TileType> InitialTypes { get; set; } = new Dictionary<Int2, TileType>();

        public TileType? GetInitialTileType(Int2 indexPosition)
        {
            return InitialTypes.TryGetValue(indexPosition, out var tileType) ? tileType : null;
        }

        public List<TileType?> GetInitialTileTypes(Int2 indexPosition, IEnumerable<Int2> form)
        {
            return form.Select(localIndexPosition => GetInitialTileType(indexPosition + localIndexPosition)).ToList();
        }

        public List<TileType> GetInitialTileTypes()
        {
            return InitialTypes.Values.ToList();
        }

        public bool RecreateTile(Int2 indexPosition)
        {
            var initialTileType = GetInitialTileType(indexPosition);
            var currentTileType = GetTile(indexPosition)?.Type;
            if (currentTileType == initialTileType)
            {
                return false;
            }

            if (currentTileType != null)
            {
                GridData.Types.Remove(indexPosition);
            }

            if (initialTileType != null)
            {
                GridData.Types[indexPosition] = recreationalTypes.IsNullOrEmpty() || recreationalTypes.Contains(initialTileType.Value) ? initialTileType.Value : recreationalTypes[0];
            }

            return true;
        }

        public void RecreateTiles(Int2 indexPosition, IEnumerable<Int2> form)
        {
            foreach (var localIndexPosition in form)
            {
                RecreateTile(indexPosition + localIndexPosition);
            }
        }

        #endregion

        #region Create

        public bool IsCreationAvailable(Int2 indexPosition, TileType type, string captureID = null, bool isResourcesIgnored = true)
        {
            var buildingDefinition = GameConfig.Instance.BuildingDefinitions?.FirstOrDefault(type);
            if (buildingDefinition == null)
            {
                Debug.Log($"{nameof(BuildingDefinition)} of type {type} not found!", this);
                return false;
            }

            var hasPositionRequirements = PositionRuleChecker.Check(indexPosition, type);
            if (!hasPositionRequirements)
            {
                Debug.Log($"The tile creation hasn't position requirements: {type} => {indexPosition}");
                return false;
            }

            var isCapturingAvailable = IsCapturingAvailable(indexPosition, captureID);
            if (!isCapturingAvailable)
            {
                return false;
            }

            return isResourcesIgnored || IsCreationResourcesAvailable(type, captureID);
        }

        public bool IsCreationResourcesAvailable(TileType type, string captureID)
        {
            var buildingDefinition = GameConfig.Instance.BuildingDefinitions?.FirstOrDefault(type);
            if (buildingDefinition == null)
            {
                Debug.Log($"{nameof(BuildingDefinition)} of type {type} not found!", this);
                return false;
            }

            var resourceKey = new ResourceKey(captureID, BuildingDefinition.CostType);
            var resourceValue = GameManager.Instance.Resources.Data.FirstOrDefault(resourceKey);
            return resourceValue >= buildingDefinition.AttackRule.Cost;
        }

        public TileType GetTileType(Int2 indexPosition)
        {
            return GridData.Types.FirstOrDefault(indexPosition);
        }

        public Dictionary<Int2, TileType> GetTileTypes()
        {
            var tileTypes = new Dictionary<Int2, TileType>();
            foreach (var (indexPosition, tile) in HexTileMap.Cells)
            {
                if (tile != null)
                {
                    tileTypes[indexPosition] = tile.Type;
                }
            }

            return tileTypes;
        }

        private bool CreateTileInternal(Int2 indexPosition, TileType type)
        {
            var existedTile = GetTile(indexPosition);
            if (existedTile != null)
            {
                RemoveTileInternal(indexPosition);
            }

            return base.CreateTile(indexPosition, type);
        }

        public override bool CreateTile(Int2 indexPosition, TileType type)
        {
            var isCreationAvailable = IsCreationAvailable(indexPosition, type);
            if (!isCreationAvailable)
            {
                return false;
            }

            GridData.Types[indexPosition] = type;
            return true;
        }

        public bool CreateTile(Int2 indexPosition, TileType type, string captureID, bool isResourcesIgnored = false)
        {
            var isCreationAvailable = IsCreationAvailable(indexPosition, type, captureID, true);
            if (!isCreationAvailable)
            {
                return false;
            }

            if (!isResourcesIgnored)
            {
                var isResourceApplied = ApplyCreationResources(captureID, type);
                if (!isResourceApplied)
                {
                    return false;
                }
            }

            CaptureTiles(indexPosition, captureID, type);
            GridData.Types[indexPosition] = type;
            return true;
        }

        private bool ApplyCreationResources(string playerID, TileType type)
        {
            var buildingDefinition = GameConfig.Instance.BuildingDefinitions?.FirstOrDefault(type);
            if (buildingDefinition == null)
            {
                Debug.Log($"{nameof(BuildingDefinition)} of type {type} not found!", this);
                return false;
            }

            var buildingCost = buildingDefinition.Cost;
            var resourceKey = new ResourceKey(playerID, BuildingDefinition.CostType);
            var resourceCount = GameManager.Instance.Resources.Data.FirstOrDefault(resourceKey);
            if (resourceCount < buildingCost)
            {
                return false; 
            }

            GameManager.Instance.Resources.Data[resourceKey] = resourceCount - buildingCost;
            return true;
        }

        #endregion

        #region Remove

        private bool RemoveTileInternal(Int2 indexPosition)
        {
            return base.RemoveTile(indexPosition);
        }

        private void RemoveTilesInternal()
        {
            foreach (var indexPosition in HexTileMap.Cells.Keys.ToArray())
            {
                base.RemoveTile(indexPosition);
            }
        }

        public override bool RemoveTile(Int2 indexPosition)
        {
            GridData.Captures.Remove(indexPosition);
            return GridData.Types.Remove(indexPosition);
        }

        #endregion
    }
}