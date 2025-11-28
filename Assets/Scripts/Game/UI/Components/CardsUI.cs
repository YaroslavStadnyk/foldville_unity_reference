using Core.Pooling;
using Core.UI;
using Game.UI.Components.ListItems;
using Game.UI.Components.ListItems.Cards;
using Grid.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.UI.Components
{
    public class CardsUI : AnimatedPage
    {
        #region Inspector

        [HideLabel] [BoxGroup("List Items Pool")]
        [SerializeField] protected PoolDictionary<CardListItem, TileType> listItemsPool = new();
        [SerializeField] [ReadOnly] private LayoutGroup layoutGroup;

        [BoxGroup("Animation")] [SerializeField] private float durationOnHovered = 0.25f;
        [BoxGroup("Animation")] [SerializeField] private Vector3 positionOffsetOnHovered;
        [BoxGroup("Animation")] [SerializeField] private Vector3 rotationOffsetOnHovered;

        [BoxGroup("Animation")] [Space] [SerializeField] private float durationOnSelected = 0.25f;
        [BoxGroup("Animation")] [SerializeField] private Vector3 positionOffsetOnSelected;
        [BoxGroup("Animation")] [SerializeField] private Vector3 rotationOffsetOnSelected;

        [BoxGroup("Animation")] [Space] [SerializeField] [PropertyOrder(99)] private float durationOnCanceled = 0.25f;

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
            if (layoutGroup == null && (listItemsPool.parent == null || !listItemsPool.parent.TryGetComponent(out layoutGroup)))
            {
                Debug.LogError($"{name} {nameof(layoutGroup)} is missing.");
            }
        }

        public virtual CardListItem ShowCard(TileType tileType)
        {
            if (listItemsPool.SpawnedBehaviours.TryGetValue(tileType, out var listItem))
            {
                return listItem;
            }

            var newListItem = listItemsPool.Spawn(tileType);
            newListItem.Initialize(tileType, OnCardClicked, OnCardHovered, OnCardAvailableChanged);
            layoutGroup.UpdateElements();

            ResetCardAnimation(tileType);

            return newListItem;
        }

        public virtual void HideCard(TileType tileType)
        {
            listItemsPool.Release(tileType);
            layoutGroup.UpdateElements();

            SelectedTileType = SelectedTileType == tileType ? null : SelectedTileType;
            HoveredTileType = HoveredTileType == tileType ? null : HoveredTileType;
        }

        public virtual void HideAllCards()
        {
            listItemsPool.ReleaseAll();

            SelectedTileType = null;
            HoveredTileType = null;
        }

        public override void Show()
        {
            base.Show();
            layoutGroup.UpdateElements();
        }

        public override void Hide()
        {
            base.Hide();

            SelectedTileType = null;
            HoveredTileType = null;
        }

        protected TileType? SelectedTileType;
        protected TileType? HoveredTileType;

        protected virtual void OnCardClicked(TileType tileType)
        {
            var oldTileType = SelectedTileType;
            SelectedTileType = tileType;

            if (oldTileType.HasValue)
            {
                UpdateCardAnimation(oldTileType.Value);
            }

            UpdateCardAnimation(tileType);
        }

        protected virtual void OnCardHovered(TileType tileType, bool isEnter)
        {
            var oldTileType = HoveredTileType;
            HoveredTileType = isEnter ? tileType : null;

            if (oldTileType.HasValue)
            {
                UpdateCardAnimation(oldTileType.Value);
            }

            UpdateCardAnimation(tileType);
        }

        protected virtual void OnCardAvailableChanged(TileType tileType, bool isAvailable)
        {
        }

        protected void UpdateCardAnimation(TileType tileType)
        {
            if (!listItemsPool.SpawnedBehaviours.TryGetValue(tileType, out var listItem) || listItem == null || !listItem.gameObject.activeSelf)
            {
                return;
            }

            if (listItem is BuildingCardUI buildingCardUI)
            {
                buildingCardUI.IsFlippingEnabled = tileType == SelectedTileType;
            }

            var duration = tileType == SelectedTileType ? durationOnSelected : tileType == HoveredTileType ? durationOnHovered : durationOnCanceled; 
            var positionOffset = tileType == SelectedTileType ? positionOffsetOnSelected : tileType == HoveredTileType ? positionOffsetOnHovered : Vector3.zero; 
            var rotationOffset = tileType == SelectedTileType ? rotationOffsetOnSelected : tileType == HoveredTileType ? rotationOffsetOnHovered : Vector3.zero;

            listItem.DoLayoutPositionOffset(positionOffset, duration);
            listItem.DoLayoutRotationOffset(rotationOffset, duration);
        }

        protected void ResetCardAnimation(TileType tileType)
        {
            if (!listItemsPool.SpawnedBehaviours.TryGetValue(tileType, out var listItem) || listItem == null)
            {
                return;
            }

            ResetCardAnimation(listItem);
        }

        protected void ResetCardAnimations()
        {
            foreach (var listItem in listItemsPool.SpawnedBehaviours.Values)
            {
                if (listItem == null)
                {
                    continue;
                }

                ResetCardAnimation(listItem);
            }
        }

        private void ResetCardAnimation(CardListItem listItem)
        {
            if (listItem is BuildingCardUI buildingCardUI)
            {
                buildingCardUI.IsFlippingEnabled = false;
            }

            listItem.LayoutPositionOffset = Vector3.zero;
            listItem.LayoutRotationOffset = Vector3.zero;
        }
    }
}