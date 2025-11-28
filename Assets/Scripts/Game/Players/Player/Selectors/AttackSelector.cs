using Board.Structs;
using Core.Extensions;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;
using Game.Players.Player.Previews;
using Grid.Hexagonal;
using MathModule.Structs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Players.Player.Selectors
{
    public class AttackSelector : PlayerExtension
    {
        #region Inspector

        [SerializeField] private AttackPreview attackPreview;

        [Space] [SerializeField] private float clickDeviation = 5;

        #endregion

        public bool IsPreviewEnabled
        {
            get => attackPreview.IsEnabled;
            set => attackPreview.IsEnabled = value;
        }

        private bool IsPreviewVisible
        {
            get => attackPreview.IsVisible;
            set => attackPreview.IsVisible = value;
        }

        private void Awake()
        {
            attackPreview.Initialize(ContextBehaviour);

            IsPreviewVisible = false;
        }

        private void OnEnable()
        {
            UpdateAttackCoordsToPreview();
            UpdatePreviews();

            GameEvents.Instance.OnTurnChanged += OnTurnChanged;
            ContextBehaviour.Selection.OnSelectedCardChanged += OnSelectedCardChanged;
            ContextBehaviour.Selection.OnSelectedIndexPositionChanged += OnSelectedIndexPositionChanged;
        }

        private void OnDisable()
        {
            IsPreviewVisible = false;

            GameEvents.Instance.OnTurnChanged -= OnTurnChanged;
            ContextBehaviour.Selection.OnSelectedCardChanged -= OnSelectedCardChanged;
            ContextBehaviour.Selection.OnSelectedIndexPositionChanged -= OnSelectedIndexPositionChanged;
        }

        private void OnTurnChanged(Turn oldTurn, Turn newTurn)
        {
            _hexTileToApply = null;

            UpdateAttackCoordsToPreview();
            UpdatePreviews();
        }

        private void OnSelectedCardChanged(CardInfo oldCardInfo, CardInfo newCardInfo)
        {
            _hexTileToApply = null;

            UpdateAttackCoordsToPreview();
            UpdatePreviews();
        }

        private void OnSelectedIndexPositionChanged(Int2 oldIndexPosition, Int2 newIndexPosition)
        {
            UpdateAttackCoordsToPreview();
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

                if (_hexTileToApply == null || IsOverContextPlayerBuilding())
                {
                    SelectHexTile();
                }
                else
                {
                    ApplyHexTile();
                }

                UpdateAttackCoordsToPreview();
                UpdatePreviews();
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

                _hexTileToApply = null;

                UpdateAttackCoordsToPreview();
                UpdatePreviews();
            }
        }

        private HexTile _hexTileToApply;

        public void SetHexTileToApply(HexTile hexTile)
        {
            var functionType = hexTile.GetBuildingDefinition()?.FunctionType;
            if (functionType == null)
            {
                return;
            }

            _hexTileToApply = functionType == FunctionType.Attacking ? hexTile : null;

            UpdateAttackCoordsToPreview();
            UpdatePreviews();
        }

        private bool IsOverContextPlayerBuilding()
        {
            var hexGrid = GameManager.Instance.HexGrid;
            if (hexGrid == null)
            {
                return false;
            }

            var indexPosition = ContextBehaviour.Selection.IndexPosition;
            var captureID = hexGrid.GetTileCapture(indexPosition);
            var hexTile = hexGrid.GetTile(indexPosition) as HexTile;
            if (hexTile == null)
            {
                return false;
            }

            var attackType = _hexTileToApply.GetBuildingDefinition()?.AttackRule?.Type;
            var attackCondition = attackType != AttackType.Border || hexTile != _hexTileToApply;

            return ContextBehaviour.LatestID == captureID && attackCondition;
        }

        private void SelectHexTile()
        {
            var hexGrid = GameManager.Instance.HexGrid;
            if (hexGrid == null)
            {
                return;
            }

            var indexPosition = ContextBehaviour.Selection.IndexPosition;
            var hexTile = hexGrid.GetTile(indexPosition) as HexTile;
            if (hexTile == null)
            {
                return;
            }

            var functionType = hexTile.GetBuildingDefinition()?.FunctionType;
            if (functionType == null)
            {
                return;
            }

            _hexTileToApply = functionType == FunctionType.Attacking ? hexTile : null;
        }

        private void ApplyHexTile()
        {
            var hexGrid = GameManager.Instance.HexGrid;
            if (hexGrid == null)
            {
                return;
            }

            var indexPosition = ContextBehaviour.Selection.IndexPosition;
            var hexTile = hexGrid.GetTile(indexPosition) as HexTile;
            if (hexTile == null || _hexTileToApply == null)
            {
                return;
            }

            var attackType = _hexTileToApply.GetBuildingDefinition()?.AttackRule?.Type;
            if (attackType == null)
            {
                return;
            }

            if (attackType == AttackType.Border && hexTile == _hexTileToApply)
            {
                ContextBehaviour.Selection.AttackCoords = new AttackCoords(_hexTileToApply.IndexPosition);
                ContextBehaviour.ApplySelectedBuildingAttack();
            }
            else if (attackType == AttackType.Range && hexTile != _hexTileToApply)
            {
                ContextBehaviour.Selection.AttackCoords = new AttackCoords(_hexTileToApply.IndexPosition, indexPosition);
                ContextBehaviour.ApplySelectedBuildingAttack();
            }

            _hexTileToApply = null;
        }

        private AttackCoords _attackCoordsToPreview;

        private void UpdateAttackCoordsToPreview()
        {
            var hexGrid = GameManager.Instance.HexGrid;
            if (hexGrid == null)
            {
                return;
            }

            var indexPosition = ContextBehaviour.Selection.IndexPosition;
            var hexTile = hexGrid.GetTile(indexPosition) as HexTile;
            if (hexTile == null || _hexTileToApply == null)
            {
                return;
            }

            var attackType = _hexTileToApply.GetBuildingDefinition()?.AttackRule?.Type;
            if (attackType == null)
            {
                return;
            }

            if (attackType == AttackType.Border)
            {
                _attackCoordsToPreview = new AttackCoords(_hexTileToApply.IndexPosition);
            }
            else if (attackType == AttackType.Range)
            {
                _attackCoordsToPreview = new AttackCoords(_hexTileToApply.IndexPosition, indexPosition);
            }
        }

        private void UpdatePreviews()
        {
            var isAttackAvailable = GameManager.Instance.HexGrid.AttackRuleExecutor.IsAttackAvailable(_attackCoordsToPreview, ContextBehaviour.LatestID, true);
            if (!isAttackAvailable || _hexTileToApply == null)
            {
                attackPreview.IsVisible = false;
            }
            else
            {
                attackPreview.IsVisible = true;
                attackPreview.Setup(_attackCoordsToPreview, ContextBehaviour.LatestID);
            }
        }
    }
}