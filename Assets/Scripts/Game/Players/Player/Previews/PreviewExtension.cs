using UnityEngine;

namespace Game.Players.Player.Previews
{
    public abstract class PreviewExtension : PlayerExtension
    {
        private Transform _previewRoot;
        protected Transform PreviewRoot
        {
            get
            {
                if (_previewRoot == null)
                {
                    _previewRoot = new GameObject($"{GetType().Name} Root").transform;
                    _previewRoot.parent = transform;
                }

                return _previewRoot;
            }
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value)
                {
                    return;
                }

                _isVisible = value;

                PreviewRoot.gameObject.SetActive(_isVisible && IsEnabled);
            }
        }

        protected virtual void OnEnable()
        {
            PreviewRoot.gameObject.SetActive(IsVisible);
        }

        protected virtual void OnDisable()
        {
            PreviewRoot.gameObject.SetActive(false);
        }
    }
}