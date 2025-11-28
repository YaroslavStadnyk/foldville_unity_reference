using Core.Extensions;
using Game.Configs;
using Game.Logic.Common.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Components.Holders
{
    public class CostHolder : ValueHolder
    {
        [Space] [SerializeField] private Image iconImage;
        [SerializeField] private bool useOriginalIcon;

        public void SetCostType(ResourceType type)
        {
            var preset = GUIConfig.Instance.ResourceUIPresets.FirstOrDefault(type);
            iconImage.sprite = useOriginalIcon ? preset.originalIcon : preset.secondaryIcon;
        }
    }
}