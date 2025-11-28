using Core.Pooling;
using Core.UI;
using Game.UI.Components.ListItems;
using Grid.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.UI.Components
{
    public class FactionsUI : AnimatedPage
    {
        #region Inspector

        [HideLabel] [BoxGroup("List Items Pool")]
        [SerializeField] protected PoolDictionary<FactionListItem, TileType> listItemsPool = new();

        #endregion

        public FactionListItem ShowFaction(TileType tileType)
        {
            if (listItemsPool.SpawnedBehaviours.TryGetValue(tileType, out var listItem))
            {
                return listItem;
            }

            var newListItem = listItemsPool.Spawn(tileType);
            newListItem.Initialize(tileType, OnBuy);

            return newListItem;
        }

        public virtual void HideFaction(TileType tileType)
        {
            listItemsPool.Release(tileType);
        }

        public virtual void HideAllFactions()
        {
            listItemsPool.ReleaseAll();
        }

        public virtual void OnBuy(TileType tileType)
        {
        }
    }
}