using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;
using Game.Logic.Internal.Interfaces;
using Mirror;

namespace Game.Logic.Internal.Network
{
    public class ResourcesManagerNetwork : BaseNetwork, IResourcesManager
    {
        private readonly SyncDictionary<ResourceKey, int> _resources = new();
        private readonly IDictionary<ResourceKey, int> _oldResources = new Dictionary<ResourceKey, int>();

        public IDictionary<ResourceKey, int> Resources => _resources;

        public override void OnStartClient()
        {
            base.OnStartClient();

            _resources.Callback += OnResourcesChanged;

            _oldResources.Clear();
            foreach (var (resourceKey, value) in _resources)
            {
                _oldResources[resourceKey] = value;
            }
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            _resources.Callback -= OnResourcesChanged;
        }

        private void OnResourcesChanged(SyncIDictionary<ResourceKey, int>.Operation operation, ResourceKey key, int value)
        {
            var operationType = operation.ToType();
            var newValue = operationType is OperationType.Remove or OperationType.Clear ? 0 : value;
            GameEvents.Instance.OnResourceChanged?.Invoke(operationType, key, _oldResources.FirstOrDefault(key), newValue);
            _oldResources[key] = value;
        }
    }
}