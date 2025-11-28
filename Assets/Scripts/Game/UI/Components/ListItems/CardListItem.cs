using System;
using Core.Extensions;
using Core.Interfaces;
using Core.Ordinaries;
using DG.Tweening;
using DG.Tweening.Core;
using Game.Configs;
using Game.Environment.Common;
using Game.Logic.Common.Enums;
using Game.Logic.Configs;
using Game.UI.Components.Holders;
using Grid.Common;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Components.ListItems
{
    [RequireComponent(typeof(ObjectPainter))]
    public class CardListItem : PoolBehaviour, ILayoutItem, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        #region Inspector

        [SerializeField] protected TMP_Text label;
        [SerializeField] protected TMP_Text subLabel;
        [SerializeField] protected Image subIcon;

        [Space] [SerializeField] private ValueHolder countHolder;
        [SerializeField] private CostHolder costHolder;

        [Space] [SerializeField] private Color unavailableColor = Color.gray;
        [SerializeField] private float unavailableColorFlow = 0.5f;
        [SerializeField] [ReadOnly] private ObjectPainter painter;

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            SetupComponents();
        }

        #endregion

        private void Awake()
        {
            SetupComponents();
        }

        private void SetupComponents()
        {
            if (painter == null && !TryGetComponent(out painter))
            {
                Debug.LogError($"{name} {nameof(painter)} is missing.");
            }
        }

        private TileType _tileType;

        public virtual void Initialize(TileType tileType, Action<TileType> onClick = null, Action<TileType, bool> onHover = null, Action<TileType, bool> onAvailableChanged = null)
        {
            _tileType = tileType;

            if (!GameConfig.Instance.BuildingDefinitions.TryGetValue(_tileType, out var definition))
            {
                return;
            }

            label.text = _tileType.ToString();
            subLabel.text = definition.FunctionType.ToString().ToLower();
            subIcon.sprite = definition.FunctionType is FunctionType.Attacking
                ? GUIConfig.Instance.AttackTypeUIPresets.FirstOrDefault(definition.AttackRule.Type).originalIcon
                : GUIConfig.Instance.FunctionTypeUIPresets.FirstOrDefault(definition.FunctionType).originalIcon;

            ResetValues();

            OnClick = onClick;
            OnHover = onHover;
            OnAvailableChanged = onAvailableChanged;
        }

        public void SetCountValue(int value)
        {
            countHolder.SetValue(value);
            countHolder.Show();
            costHolder.Hide();
        }

        public void SetCostValue(int value, ResourceType type)
        {
            costHolder.SetValue(value);
            costHolder.SetCostType(type);
            costHolder.Show();
            countHolder.Hide();
        }

        public void ResetValues()
        {
            IsAvailable = true;

            countHolder.Hide();
            costHolder.Hide();
        }

        private bool _isAvailable = true;

        public bool IsAvailable
        {
            get => _isAvailable;
            set
            {
                if (_isAvailable != value)
                {
                    _isAvailable = value;
                    OnAvailableChanged?.Invoke(_tileType, _isAvailable);
                }

                costHolder.SetAvailableState(_isAvailable);
                if (_isAvailable)
                {
                    painter.ResetColor();
                }
                else
                {
                    painter.SetColor(unavailableColor, unavailableColorFlow);
                }
            }
        }

        private event Action<TileType, bool> OnAvailableChanged; 

        #region Pointers

        private event Action<TileType> OnClick;
        private event Action<TileType, bool> OnHover;

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            OnHover?.Invoke(_tileType, true);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            OnHover?.Invoke(_tileType, false);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (!IsAvailable)
            {
                return;
            }

            OnClick?.Invoke(_tileType);
        }

        #endregion

        #region Layout

        private Vector3 _layoutPositionOffset;
        private Vector3 _layoutRotationOffset;

        public Vector3 LayoutPositionOffset
        {
            get => _layoutPositionOffset;
            set
            {
                LayoutPositionOffsetTweener?.Complete();
                LayoutPosition += value - _layoutPositionOffset;
                _layoutPositionOffset = value;
            }
        }

        public Vector3 LayoutRotationOffset
        {
            get => _layoutRotationOffset;
            set
            {
                LayoutRotationOffsetTweener?.Complete();
                LayoutRotation += value - _layoutRotationOffset;
                _layoutRotationOffset = value;
            }
        }

        public Vector3 LayoutPosition
        {
            get => (Vector3)((RectTransform)transform).anchoredPosition - _layoutPositionOffset;
            set => ((RectTransform)transform).anchoredPosition = value + _layoutPositionOffset;
        }

        public Vector3 LayoutRotation
        {
            get => transform.localRotation.eulerAngles - _layoutRotationOffset;
            set => transform.localRotation = Quaternion.Euler(value + _layoutRotationOffset);
        }

        #endregion

        #region Tweeners

        public Tweener LayoutPositionOffsetTweener;

        public Tweener DoLayoutPositionOffset(Vector3 positionOffset, float duration, Ease ease = Ease.Linear)
        {
            if (LayoutPositionOffsetTweener.IsActive())
            {
                LayoutPositionOffsetTweener.ChangeValues(_layoutPositionOffset, positionOffset, duration)
                    .SetEase(ease)
                    .Restart();
            }
            else
            {
                var getter = new DOGetter<Vector3>(() => _layoutPositionOffset);
                var setter = new DOSetter<Vector3>(value =>
                {
                    LayoutPosition += value - _layoutPositionOffset;
                    _layoutPositionOffset = value;
                });

                LayoutPositionOffsetTweener = DOTween.To(getter, setter, positionOffset, duration)
                    .SetEase(ease)
                    .SetUpdate(true)
                    .SetLink(gameObject)
                    .SetAutoKill(false);
            }

            return LayoutPositionOffsetTweener;
        }

        public Tweener LayoutRotationOffsetTweener;

        public Tweener DoLayoutRotationOffset(Vector3 rotationOffset, float duration, Ease ease = Ease.Linear)
        {
            if (LayoutRotationOffsetTweener.IsActive())
            {
                LayoutRotationOffsetTweener.ChangeValues(_layoutRotationOffset, rotationOffset, duration)
                    .SetEase(ease)
                    .Restart();
            }
            else
            {
                var getter = new DOGetter<Vector3>(() => _layoutRotationOffset);
                var setter = new DOSetter<Vector3>(value => {
                    LayoutRotation += value - _layoutRotationOffset;
                    _layoutRotationOffset = value;
                });

                LayoutRotationOffsetTweener = DOTween.To(getter, setter, rotationOffset, duration)
                    .SetEase(ease)
                    .SetUpdate(true)
                    .SetLink(gameObject)
                    .SetAutoKill(false);
            }

            return LayoutPositionOffsetTweener;
        }

        #endregion
    }
}