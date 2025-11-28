using Core.Extensions;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class AnimatedPage : Page
    {
        #region Inspector

        [Title("Animation")] [BoxGroup(nameof(Page))] [SerializeField] private bool animationEnabled = false;
        [ShowIf(nameof(animationEnabled))] [BoxGroup(nameof(Page))] [SerializeField] private float animationDuration = 0.2f;
        [ShowIf(nameof(animationEnabled))] [BoxGroup(nameof(Page))] [SerializeField] public float animationDelay = 0.0f;
        [ShowIf(nameof(animationEnabled))] [BoxGroup(nameof(Page))] [SerializeField] private Ease showAnimationEase = Ease.OutExpo;
        [ShowIf(nameof(animationEnabled))] [BoxGroup(nameof(Page))] [SerializeField] private Ease hideAnimationEase = Ease.InBack;
        [ShowIf(nameof(animationEnabled))] [BoxGroup(nameof(Page))] [SerializeField] private Vector3 hiddenOffset = Vector3.zero;
        [ShowIf(nameof(animationEnabled))] [BoxGroup(nameof(Page))] [SerializeField] private Vector3 hiddenScale = Vector3.zero;
        [ShowIf(nameof(animationEnabled))] [BoxGroup(nameof(Page))] [SerializeField] private float hiddenAlpha = 1.0f;

        #endregion

        protected Vector3 InitialPagePosition { get; private set; }
        protected Vector3 InitialPageScale { get; private set; }

        protected override void Awake()
        {
            SetupPageTransform();
            base.Awake();
        }

        public void SetupPageTransform()
        {
            var pageTransform = transform;
            InitialPagePosition = pageTransform.localPosition;
            InitialPageScale = pageTransform.localScale;
        }

        public void SetupAnimationParameters(AnimatedPage page)
        {
            animationEnabled = page.animationEnabled;
            animationDuration = page.animationDuration;
            animationDelay = page.animationDelay;
            showAnimationEase = page.showAnimationEase;
            hideAnimationEase = page.hideAnimationEase;
            hiddenOffset = page.hiddenOffset;
            hiddenScale = page.hiddenScale;
            hiddenAlpha = page.hiddenAlpha;
        }

        public override void Show()
        {
            base.Show();

            if (animationEnabled)
            {
                DoHideAnimation(0f, gameObject.activeInHierarchy ? animationDuration : 0f, gameObject.activeInHierarchy ? animationDelay : 0f, showAnimationEase).onComplete = null;
            }
        }

        public override void Hide()
        {
            if (animationEnabled && gameObject.activeInHierarchy)
            {
                DoHideAnimation(1f, animationDuration, animationDelay, hideAnimationEase).onComplete = base.Hide;
            }
            else
            {
                base.Hide();
            }
        }

        #region Tweeners

        private Tweener _hideAnimationTweener;

        private Tweener DoHideAnimation(float value, float duration, float delay, Ease ease)
        {
            if (_hideAnimationTweener.IsActive())
            {
                _hideAnimationTweener.ChangeEndValue(value, duration, true)
                    .SetDelay(delay)
                    .SetEase(ease)
                    .Restart();
            }
            else
            {
                _hideAnimationTweener = DOTween.To(HideAnimationSetter, IsShown ? 0f : 1f, value, duration)
                    .SetDelay(delay)
                    .SetEase(ease)
                    .SetUpdate(true)
                    .SetLink(gameObject)
                    .SetAutoKill(false);
            }

            return _hideAnimationTweener;
        }

        private void HideAnimationSetter(float value)
        {
            var pageTransform = transform;
            var hiddenPosition = InitialPagePosition + hiddenOffset.Multiply(new Vector3(Screen.width, Screen.height));

            pageTransform.localPosition = Vector3.LerpUnclamped(InitialPagePosition, hiddenPosition, value);
            pageTransform.localScale = Vector3.LerpUnclamped(InitialPageScale, hiddenScale, value);
            canvasGroup.alpha = Mathf.LerpUnclamped(InitialCanvasGroupAlpha, hiddenAlpha, value);
        }

        #endregion
    }
}