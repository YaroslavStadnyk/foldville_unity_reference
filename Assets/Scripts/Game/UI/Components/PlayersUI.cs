using Core.Extensions;
using Core.Pooling;
using Core.UI;
using Game.UI.Components.ListItems;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.UI.Components
{
    public class PlayersUI : AnimatedPage
    {
        #region Inspector

        [HideLabel] [BoxGroup("List Items Pool")]
        [SerializeField] protected PoolDictionary<PlayerListItem, string> listItemsPool = new();
        [SerializeField] [ReadOnly] private LayoutGroup layoutGroup;

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            SetupComponents();
        }

        #endregion

        protected override void Awake()
        {
            base.Awake();
            SetupComponents();
        }

        private void SetupComponents()
        {
            if (layoutGroup == null && !TryGetComponent(out layoutGroup))
            {
                Debug.LogError($"{name} {nameof(layoutGroup)} is missing.");
            }
        }

        public void ShowListItems(params string[] playerIDs)
        {
            foreach (var playerID in playerIDs)
            {
                if (playerID == null)
                {
                    continue;
                }

                var listItem = listItemsPool.Spawn(playerID);
                listItem.Initialize(playerID);
            }
        }

        public void HideListItems(params string[] playerIDs)
        {
            foreach (var playerID in playerIDs)
            {
                if (playerID == null)
                {
                    continue;
                }

                listItemsPool.Release(playerID);
            }
        }

        public void HideAllPlayers()
        {
            listItemsPool.ReleaseAll();
        }

        public void SetListItemValue(string playerID, int value)
        {
            if (playerID == null)
            {
                return;
            }

            if (listItemsPool.SpawnedBehaviours.TryGetValue(playerID, out var listItem) && listItem != null)
            {
                listItem.SetValue(value);
            }
        }

        public void SelectListItem(string playerID)
        {
            DeselectListItems();

            if (!playerID.IsNullOrEmpty() && listItemsPool.SpawnedBehaviours.TryGetValue(playerID, out var listItem) && listItem != null)
            {
                listItem.Select();
            }

            layoutGroup.UpdateElements();
        }

        private void DeselectListItems()
        {
            foreach (var listItem in listItemsPool.SpawnedBehaviours.Values)
            {
                if (listItem != null)
                {
                    listItem.Deselect();
                }
            }

            // layoutGroup.UpdateElements();
        }

        public void SetListItemLostState(string playerID)
        {
            if (!playerID.IsNullOrEmpty() && listItemsPool.SpawnedBehaviours.TryGetValue(playerID, out var listItem) && listItem != null)
            {
                listItem.Deselect();
                listItem.SetLostState();
            }
        }
    }
}
