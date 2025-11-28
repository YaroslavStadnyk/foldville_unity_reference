using Core.Extensions;
using Core.Ordinaries;
using DG.Tweening;
using Game.Configs;
using Game.Logic.Common.Enums;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Components.ListItems
{
    public class ResourceListItem : PoolBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text valueText;
        [SerializeField] private string separator = " / ";

        [FoldoutGroup("Feedbacks")] [SerializeField] private float valueChangingDuration = 0.3f;
        [FoldoutGroup("Feedbacks")] [SerializeField] private Ease valueChangingEase = Ease.Linear;
        [FoldoutGroup("Feedbacks")] [Space] [SerializeField] private MMF_Player valueChangedFeedbacks;
        [FoldoutGroup("Feedbacks")] [SerializeField] private MMF_Player valueErrorFeedbacks;

        private int _value;
        private int _maxValue;

        private void Awake()
        {
            valueText.text = "0";
        }

        public void Initialize(ResourceType resourceType)
        {
            iconImage.sprite = GUIConfig.Instance.ResourceUIPresets.FirstOrDefault(resourceType).originalIcon;
        }

        public void SetValue(int value)
        {
            DoUpdateValueText(value, valueChangingDuration, valueChangingEase);
            valueChangedFeedbacks.PlayFeedbacks();
        }

        public void SetMaxValue(int maxValue)
        {
            DoUpdateMaxValueText(maxValue, valueChangingDuration, valueChangingEase);
            valueChangedFeedbacks.PlayFeedbacks();
        }

        public void PlayErrorFeedback()
        {
            valueErrorFeedbacks.PlayFeedbacks();
        }

        #region Tweeners

        private Tweener _updateValueTextTweener;
        private Tweener _updateMaxValueTextTweener;

        private Tweener DoUpdateValueText(float endValue, float duration, Ease ease = Ease.InOutSine)
        {
            if (_updateValueTextTweener.IsActive())
            {
                _updateValueTextTweener.ChangeValues((float)_value, endValue, duration)
                    .SetEase(ease)
                    .Restart();
            }
            else
            {
                _updateValueTextTweener = DOTween.To(UpdateValueTextSetter, _value, endValue, duration)
                    .SetEase(ease)
                    .SetUpdate(true)
                    .SetLink(gameObject)
                    .SetAutoKill(false);
            }

            return _updateValueTextTweener;
        }

        private Tweener DoUpdateMaxValueText(float endMaxValue, float duration, Ease ease = Ease.InOutSine)
        {
            if (_updateMaxValueTextTweener.IsActive())
            {
                _updateMaxValueTextTweener.ChangeValues((float)_maxValue, endMaxValue, duration)
                    .SetEase(ease)
                    .Restart();
            }
            else
            {
                _updateMaxValueTextTweener = DOTween.To(UpdateMaxValueTextSetter, _maxValue, endMaxValue, duration)
                    .SetEase(ease)
                    .SetUpdate(true)
                    .SetLink(gameObject)
                    .SetAutoKill(false);
            }

            return _updateMaxValueTextTweener;
        }

        private void UpdateValueTextSetter(float value)
        {
            _value = Mathf.RoundToInt(value);
            valueText.text = $"{_value.ToShort()}{separator}{_maxValue.ToShort()}";
        }

        private void UpdateMaxValueTextSetter(float maxValue)
        {
            _maxValue = Mathf.RoundToInt(maxValue);
            valueText.text = $"{_value.ToShort()}{separator}{_maxValue.ToShort()}";
        }

        #endregion
    }
}