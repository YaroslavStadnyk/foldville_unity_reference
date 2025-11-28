using System.Collections.Generic;
using Game.Logic.Common.Enums;
using Game.Logic.Configs;
using Game.Logic.Internal.Interfaces;
using Grid.Common;
using MathModule.Structs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Logic.API
{
    public class GridDataManager : MonoBehaviour, IGameManager
    {
        private IGridDataManager _impl;

        public void Initialize(GameMode gameMode)
        {
            var implPrefab = DeveloperConfig.Instance.GetGridDataManagerPrefab(gameMode);
            implPrefab.Initialize(OnInitialize);
        }

        public void ResetImplementation()
        {
            if (_impl == null)
                return;
            
            _impl.Destroy();
            _impl = null;
        }

        private void OnInitialize(IBase impl)
        {
            _impl = impl as IGridDataManager;
            if (_impl == null)
            {
                Debug.LogError($"{typeof(IGridDataManager)} not casted.");
                return;
            }

            if (impl is Component implComponent)
            {
                implComponent.transform.parent = transform;
            }

            if (_initialTypes != null)
            {
                _impl.InitialTypes = _initialTypes;
            }
        }

        private IDictionary<Int2, TileType> _initialTypes;
        public IDictionary<Int2, TileType> InitialTypes
        {
            set
            {
                _initialTypes = value;

                if (_impl != null)
                {
                    _impl.InitialTypes = _initialTypes;
                }
            }
        }

        [BoxGroup("Debug")] [ShowInInspector] [ReadOnly] [HideInEditorMode] public IDictionary<Int2, TileType> Types => _impl?.Types;

        [BoxGroup("Debug")] [ShowInInspector] [ReadOnly] [HideInEditorMode] public IDictionary<Int2, string> Captures => _impl?.Captures;
    }
}