using Core.UI;
using TMPro;
using UnityEngine;

namespace Game.UI.Components.Holders
{
    public class ValueHolder : AnimatedPage
    {
        [SerializeField] private string prefix = "";
        [SerializeField] private TMP_Text valueText;
        [SerializeField] private Color unavailableColor = Color.red;
        private Color _availableColor;

        protected override void Awake()
        {
            base.Awake();
            _availableColor = valueText.color;
        }

        public void SetValue(int value)
        {
            valueText.text = $"{prefix}{value}";
        }

        public void SetAvailableState(bool isAvailable)
        {
            valueText.color = isAvailable ? _availableColor : unavailableColor;
        }
    }
}