using System.Collections.Generic;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;
using Game.Logic.Configs;
using Game.Logic.Internal.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Logic.API
{
    public class ResourcesManager : MonoBehaviour, IGameManager
    {
        private IResourcesManager _impl;

        public void Initialize(GameMode gameMode)
        {
            var implPrefab = DeveloperConfig.Instance.GetResourcesManagerPrefab(gameMode);
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
            _impl = impl as IResourcesManager;
            if (_impl == null)
            {
                Debug.LogError($"{typeof(IResourcesManager)} not casted.");
                return;
            }

            if (impl is Component implComponent)
            {
                implComponent.transform.parent = transform;
            }
        }

        [BoxGroup("Debug")] [ShowInInspector] [ReadOnly] [HideInEditorMode] public IDictionary<ResourceKey, int> Data => _impl?.Resources;
    }
}