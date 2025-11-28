using Core.Extensions;
using Game.Logic.Common.Structs;
using Game.Players.Player.Previews;
using Grid.Hexagonal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Players.Player.Selectors
{
    public class HexTileSelector : PlayerExtension
    {
        #region Inspector

        [SerializeField] private HexTilePreview hexTilePreview;

        [Space] [SerializeField] private float clickDeviation = 5;

        #endregion

        public bool IsPreviewEnabled
        {
            get => hexTilePreview.IsEnabled;
            set => hexTilePreview.IsEnabled = value;
        }

        private bool IsPreviewVisible
        {
            get => hexTilePreview.IsVisible;
            set => hexTilePreview.IsVisible = value;
        }

        private void Awake()
        {
            hexTilePreview.Initialize(ContextBehaviour);

            IsPreviewVisible = false;
        }

        private void OnEnable()
        {
            UpdatePreviews();

            ContextBehaviour.Selection.OnSelectedHexTileChanged += OnSelectedHexTileChanged;
            GameEvents.Instance.OnTurnChanged += OnTurnChanged;
        }

        private void OnDisable()
        {
            IsPreviewVisible = false;

            ContextBehaviour.Selection.HexTile = null;
            ContextBehaviour.Selection.OnSelectedHexTileChanged -= OnSelectedHexTileChanged;
            GameEvents.Instance.OnTurnChanged -= OnTurnChanged;
        }

        private void OnTurnChanged(Turn oldTurn, Turn newTurn)
        {
            ContextBehaviour.Selection.HexTile = null;
        }

        private void OnSelectedHexTileChanged(HexTile oldHexTile, HexTile newHexTile)
        {
            UpdatePreviews();
        }

        private void Update()
        {
            if (!IsEnabled)
            {
                return;
            }

            UpdateInput();
        }

        private Vector2 _keyDownMouse0Position;
        private Vector2 _keyDownMouse1Position;

        private void UpdateInput()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _keyDownMouse0Position = Input.mousePosition.GetXY();
                return;
            }

            if (Input.GetKeyUp(KeyCode.Mouse0) && !BaseExtensions.IsOverUI(PointerInputModule.kMouseLeftId))
            {
                var keyDownMouseDelta = Input.mousePosition.GetXY() - _keyDownMouse0Position;
                if (keyDownMouseDelta.magnitude > clickDeviation)
                {
                    return;
                }

                if (GameManager.Instance.HexGrid.GetTileCapture(ContextBehaviour.Selection.IndexPosition) == ContextBehaviour.LatestID)
                {
                    ContextBehaviour.Selection.HexTile = GameManager.Instance.HexGrid.GetTile(ContextBehaviour.Selection.IndexPosition) as HexTile;
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                _keyDownMouse1Position = Input.mousePosition.GetXY();
                return;
            }

            if (Input.GetKeyUp(KeyCode.Mouse1) && !BaseExtensions.IsOverUI(PointerInputModule.kMouseRightId))
            {
                var keyDownMouseDelta = Input.mousePosition.GetXY() - _keyDownMouse1Position;
                if (keyDownMouseDelta.magnitude > clickDeviation)
                {
                    return;
                }

                ContextBehaviour.Selection.HexTile = null;
            }
        }

        private void UpdatePreviews()
        {
            if (ContextBehaviour.Selection.HexTile == null)
            {
                IsPreviewVisible = false;
            }
            else
            {
                IsPreviewVisible = true;
                hexTilePreview.Setup(ContextBehaviour.Selection.HexTile);
            }
        }
    }
}