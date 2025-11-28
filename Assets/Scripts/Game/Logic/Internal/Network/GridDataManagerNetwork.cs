using System.Collections.Generic;
using Core;
using Core.Extensions;
using Core.Managers;
using Game.Logic.Common.Enums;
using Game.Logic.Internal.Interfaces;
using Grid.Common;
using MathModule.Structs;
using Mirror;
using UnityEngine;

namespace Game.Logic.Internal.Network
{
    public class GridDataManagerNetwork : BaseNetwork, IGridDataManager
    {
        private readonly SyncDictionary<Int2, TileType> _typeData = new();
        private readonly SyncDictionary<Int2, string> _captureData = new();

        private readonly IDictionary<Int2, TileType> _oldTypeData = new Dictionary<Int2, TileType>();
        private readonly IDictionary<Int2, string> _oldCaptureData = new Dictionary<Int2, string>();

        private IDictionary<Int2, TileType> _initialTypeData;
        public IDictionary<Int2, TileType> InitialTypes
        {
            get => _initialTypeData;
            set
            {
                _initialTypeData = value;
                SetupInitialTypeData();
            }
        }

        public IDictionary<Int2, TileType> Types => _typeData;
        public IDictionary<Int2, string> Captures => _captureData;

        public override void OnStartServer()
        {
            base.OnStartServer();

            if (_initialTypeData != null)
            {
                SetupInitialTypeData();
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            _typeData.Callback += OnTypeDataChanged;
            _captureData.Callback += OnCaptureDataChanged;

            SetupTypeData();
            SetupCaptureData();
        }

        private void SetupInitialTypeData()
        {
            if (_initialTypeData == null)
            {
                Debug.LogError($"{nameof(GridDataManagerNetwork)} - {nameof(_initialTypeData)} is null.");
                return;
            }

            _typeData.Clear();
            _captureData.Clear();

            _oldTypeData.Clear();
            _oldCaptureData.Clear();

            foreach (var (indexPosition, type) in _initialTypeData)
            {
                _typeData.Add(indexPosition, type);
                _oldTypeData.Add(indexPosition, type);
            }
        }

        private void SetupTypeData()
        {
            if (_typeData.Count == 0)
            {
                return;
            }

            OnTypeDataChangedAsync(OperationType.Clear, default);

            foreach (var indexPosition in _typeData.Keys)
            {
                OnTypeDataChangedAsync(OperationType.Add, indexPosition);
            }
        }

        private void SetupCaptureData()
        {
            if (_captureData.Count == 0)
            {
                return;
            }

            OnCaptureDataChangedAsync(OperationType.Clear, default);

            foreach (var indexPosition in _captureData.Keys)
            {
                OnCaptureDataChangedAsync(OperationType.Add, indexPosition);
            }
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            _typeData.Callback -= OnTypeDataChanged;
            _captureData.Callback -= OnCaptureDataChanged;
        }

        private void OnTypeDataChanged(SyncDictionary<Int2, TileType>.Operation operation, Int2 indexPosition, TileType tileType)
        {
            AsyncCallbackManager.Queue(() => OnTypeDataChangedAsync(operation.ToType(), indexPosition));
        }

        private void OnCaptureDataChanged(SyncDictionary<Int2, string>.Operation operation, Int2 indexPosition, string captureID)
        {
            AsyncCallbackManager.Queue(() => OnCaptureDataChangedAsync(operation.ToType(), indexPosition));
        }

        private void OnTypeDataChangedAsync(OperationType operationType, Int2 indexPosition)
        {
            var oldTileType = _oldTypeData.FirstOrDefault(indexPosition);
            var newTileType = _typeData.FirstOrDefault(indexPosition);
            var captureID = _captureData.FirstOrDefault(indexPosition);

            switch (operationType)
            {
                case OperationType.Add or OperationType.Set:
                    _oldTypeData[indexPosition] = newTileType;
                    break;
                case OperationType.Remove:
                    _oldTypeData.Remove(indexPosition);
                    break;
                case OperationType.Clear:
                    _oldTypeData.Clear();
                    break;
                default:
                    break;
            }

            GameEvents.Instance.OnTileTypeChanged?.Invoke(operationType, indexPosition, oldTileType, newTileType, captureID);
        }

        private void OnCaptureDataChangedAsync(OperationType operationType, Int2 indexPosition)
        {
            var tileType = _typeData.FirstOrDefault(indexPosition);
            var oldCaptureID = _oldCaptureData.FirstOrDefault(indexPosition);
            var newCaptureID = _captureData.FirstOrDefault(indexPosition);

            switch (operationType)
            {
                case OperationType.Add or OperationType.Set:
                    _oldCaptureData[indexPosition] = newCaptureID;
                    break;
                case OperationType.Remove:
                    _oldCaptureData.Remove(indexPosition);
                    break;
                case OperationType.Clear:
                    _oldCaptureData.Clear();
                    break;
                default:
                    break;
            }

            GameEvents.Instance.OnTileCaptureChanged?.Invoke(operationType, indexPosition, tileType, oldCaptureID, newCaptureID);
        }
    }
}