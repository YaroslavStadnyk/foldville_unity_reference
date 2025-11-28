using System.Collections.Generic;
using Core.Extensions;
using Core.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.UI
{
    public class LayoutGroup : MonoBehaviour
    {
        #region Inspector

        [SerializeField] public TextAnchor childAlignment;
        [SerializeField] private Vector2 direction = Vector2.right;
        [SerializeField] public float spacing;
        [SerializeField] public Vector3 rotation;

        [OnInspectorInit] [OnInspectorDispose]
        private void OnInspector()
        {
            if (Application.isPlaying || this == null)
            {
                return;
            }

            direction = direction.normalized;
            UpdateElements();
        }

        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                return;
            }

            UpdateElements();
        }

        #endregion

        public Vector2 Direction
        {
            get => direction;
            set => direction = value.normalized;
        }

        protected virtual void Awake()
        {
            direction = direction.normalized;
            UpdateElements();
        }

        protected virtual void OnTransformChildrenChanged()
        {
            if (enabled)
            {
                UpdateElements();
            }
        }

        public virtual void UpdateElements()
        {
            var totalDistance = 0.0f;
            foreach (var child in transform.GetRectChilds(false))
            {
                var size = GetElementSize(child);
                var pivot = 0.5f; // (rectTransform.pivot * direction).magnitude;
                var delta = size * pivot + (totalDistance == 0 ? 0 : spacing);

                var distance = totalDistance + delta;
                totalDistance += delta + (size - size * pivot);

                UpdateElement(child, distance);
            }
        }

        protected void UpdateElement(RectTransform element, float distance)
        {
            element.SetAnchors(childAlignment);
            if (element.TryGetComponent<ILayoutItem>(out var layoutItem))
            {
                layoutItem.LayoutPosition = distance * direction;
                layoutItem.LayoutRotation = rotation;
            }
            else
            {
                element.anchoredPosition = distance * direction;
                element.localRotation = Quaternion.Euler(rotation);
            }
        }

        protected float GetElementSize(RectTransform element)
        {
            return (element.rect.size * Direction).magnitude;
        }

        protected float GetContentSize(IReadOnlyList<RectTransform> elements)
        {
            var elementsCount = elements.Count;
            if (elementsCount == 0)
            {
                return 0f;
            }

            var elementSize = (elements[0].sizeDelta * Direction).magnitude;
            if (elementsCount == 1)
            {
                return elementSize;
            }

            var elementsSize = elementSize * elementsCount + spacing * (elementsCount - 1);
            return elementsSize;
        }
    }
}