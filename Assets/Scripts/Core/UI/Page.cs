using Core.Ordinaries;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Page : PoolBehaviour
    {
        #region Inspector

        [BoxGroup(nameof(Page))] [SerializeField] [ReadOnly] protected CanvasGroup canvasGroup;
        [BoxGroup(nameof(Page))] [SerializeField] public bool disableOnHide = false;
        [BoxGroup(nameof(Page))] [SerializeField] private bool hideOnAwake = false;

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            SetupCanvasGroup();
        }

        #endregion

        protected float InitialCanvasGroupAlpha { get; private set; }
        protected bool InitialCanvasGroupInteractable { get; private set; }
        protected bool InitialCanvasGroupBlocksRaycasts { get; private set; }

        protected virtual void Awake()
        {
            SetupCanvasGroup();

            if (hideOnAwake)
            {
                Hide();
            }
        }

        public void SetupCanvasGroup()
        {
            if (canvasGroup == null && !TryGetComponent(out canvasGroup))
            {
                Debug.LogError($"{name} {nameof(canvasGroup)} is missing.");
                return;
            }

            InitialCanvasGroupAlpha = canvasGroup.alpha;
            InitialCanvasGroupInteractable = canvasGroup.interactable;
            InitialCanvasGroupBlocksRaycasts = canvasGroup.blocksRaycasts;
        }

        public bool IsShown { get; private set; } = true;

        public virtual void Show()
        {
            IsShown = true;
            ShowInternal();
        }

        private void ShowInternal()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            canvasGroup.alpha = InitialCanvasGroupAlpha;
            canvasGroup.interactable = InitialCanvasGroupInteractable;
            canvasGroup.blocksRaycasts = InitialCanvasGroupBlocksRaycasts;
        }

        public virtual void Hide()
        {
            IsShown = false;
            HideInternal();
        }

        private void HideInternal()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (disableOnHide)
            {
                gameObject.SetActive(false);
                return;
            }

            canvasGroup.alpha = 0.0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public override void OnSpawn()
        {
            if (!IsShown && !hideOnAwake)
            {
                Show();
            }
        }

        public override void OnRelease()
        {
            if (IsShown)
            {
                Hide();
            }
        }
    }
}