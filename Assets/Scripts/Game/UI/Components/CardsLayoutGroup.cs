using System.Collections.Generic;
using Core.Extensions;
using Core.UI;
using UnityEngine;

namespace Game.UI.Components
{
    public class CardsLayoutGroup : LayoutGroup
    {
        [Space] [SerializeField] private bool clampingEnabled = true;

        public override void UpdateElements()
        {
            if (clampingEnabled && ClampElements())
            {
                return;
            }

            base.UpdateElements();
        }

        private bool ClampElements()
        {
            if (transform is not RectTransform root)
            {
                return false;
            }

            var childs = transform.GetRectChilds(false);
            var childsCount = childs.Count;
            if (childsCount < 2)
            {
                return false;
            }

            var contentSize = GetContentSize(childs);
            var rootSize = GetElementSize(root);
            if (rootSize >= contentSize)
            {
                return false;
            }

            var firstChildSize = GetElementSize(childs[0]);
            var firstChildPivot = 0.5f; // (rectTransform.pivot * direction).magnitude;
            var firstChildDistance = firstChildSize * firstChildPivot;

            var delta = rootSize / childsCount - firstChildDistance / childsCount;
            for (var i = 0; i < childsCount; i++)
            {
                var child = childs[i];
                var distance = i * delta + firstChildDistance;

                UpdateElement(child, distance);
            }

            return true;
        }
    }
}