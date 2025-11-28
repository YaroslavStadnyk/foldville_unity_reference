using Core.Pooling;
using Core.UI;
using Game.Logic.Common.Enums;
using Game.UI.Components.ListItems;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.UI.Components
{
    public class ResourcesUI : AnimatedPage
    {
        [HideLabel] [BoxGroup("List Items Pool")]
        [SerializeField] protected PoolDictionary<ResourceListItem, ResourceType> listItemsPool = new();

        public void ShowResources(params ResourceType[] resourceTypes)
        {
            foreach (var resourceType in resourceTypes)
            {
                var listItem = listItemsPool.Spawn(resourceType);
                listItem.Initialize(resourceType);
            }
        }

        public void HideResources(params ResourceType[] resourceTypes)
        {
            foreach (var resourceType in resourceTypes)
            {
                listItemsPool.Release(resourceType);
            }
        }

        public void HideAllResources()
        {
            listItemsPool.ReleaseAll();
        }

        public void SetResourceValue(ResourceType resourceType, int value)
        {
            if (listItemsPool.SpawnedBehaviours.TryGetValue(resourceType, out var listItem) && listItem != null)
            {
                listItem.SetValue(value);
            }
        }

        public void SetResourceMaxValue(ResourceType resourceType, int maxValue)
        {
            if (listItemsPool.SpawnedBehaviours.TryGetValue(resourceType, out var listItem) && listItem != null)
            {
                listItem.SetMaxValue(maxValue);
            }
        }
    }
}
