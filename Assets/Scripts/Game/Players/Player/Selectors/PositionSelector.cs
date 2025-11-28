using Core.Extensions;
using Game.Players.Player.Previews;
using UnityEngine;

namespace Game.Players.Player.Selectors
{
    public class PositionSelector : PlayerExtension
    {
        #region Inspector

        [SerializeField] private PositionPreview positionPreview;

        [Space] [SerializeField] private float selectionOffsetY = 0.1f;

        #endregion

        public bool IsPreviewEnabled
        {
            get => positionPreview.IsEnabled;
            set => positionPreview.IsEnabled = value;
        }

        private bool IsPreviewVisible
        {
            get => positionPreview.IsVisible;
            set => positionPreview.IsVisible = value;
        }

        private void Awake()
        {
            positionPreview.Initialize(ContextBehaviour);

            IsPreviewVisible = false;
        }

        private void OnEnable()
        {
            IsPreviewVisible = true;
        }

        private void OnDisable()
        {
            IsPreviewVisible = false;
        }

        private void Update()
        {
            if (!IsEnabled)
            {
                return;
            }

            var hexGrid = GameManager.Instance.HexGrid;
            if (hexGrid == null)
            {
                return;
            }

            var ray = BaseExtensions.ScreenPointToRay(Input.mousePosition);
            var groundPoint = ray.GetGroundPoint(Vector3.zero.WithY(selectionOffsetY));
            var indexPosition = hexGrid.ConvertToIndexPosition(groundPoint);

            ContextBehaviour.Selection.IndexPosition = indexPosition;
            positionPreview.Setup(indexPosition);
        }
    }
}