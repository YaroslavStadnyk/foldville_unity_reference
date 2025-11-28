using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using DG.Tweening;
using Game.Logic.Common.Blocks;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;
using Grid.Common;
using Grid.Hexagonal;
using MathModule.Structs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Players.Player
{
    [RequireComponent(typeof(CameraMovement))]
    public class CameraFocusController : MonoBehaviour
    {
        #region Inspector

        [SerializeField] [ReadOnly] private CameraMovement cameraMovement;

        [Space] [SerializeField] private float focusDuration = 0.5f;
        [SerializeField] private Ease focusEase = Ease.InOutSine;

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
            if (cameraMovement == null && !TryGetComponent(out cameraMovement))
            {
                Debug.LogError($"{name} {nameof(cameraMovement)} is missing.");
            }
        }

        private void OnEnable()
        {
            GameEvents.Instance.OnTileTypeChanged += OnTileTypeChanged;
            GameEvents.Instance.OnTurnChanged += OnTurnChanged;
        }

        private void OnDisable()
        {
            GameEvents.Instance.OnTileTypeChanged -= OnTileTypeChanged;
            GameEvents.Instance.OnTurnChanged -= OnTurnChanged;
        }

        #region PlayerFocus

        private readonly Dictionary<string, Int2> _playerMainTileIndexPositions = new(4);

        private void OnTileTypeChanged(OperationType operationType, Int2 indexPosition, TileType oldTileType, TileType newTileType, string captureID)
        {
            if (operationType is OperationType.Add or OperationType.Set && newTileType is CommonLevelLogic.MainTileType)
            {
                _playerMainTileIndexPositions[captureID] = indexPosition;
            }
            else if (operationType is OperationType.Remove or OperationType.Set && oldTileType is CommonLevelLogic.MainTileType)
            {
                _playerMainTileIndexPositions.Remove(captureID);
            }
            else if (operationType is OperationType.Clear)
            {
                _playerMainTileIndexPositions.Clear();
            }
        }

        private void OnTurnChanged(Turn oldTurn, Turn newTurn)
        {
            var playerID = newTurn.playerID;
            if (playerID.IsNullOrEmpty())
            {
                return;
            }

            if (PlayerProfile.LocalLatest == null || !PlayerProfile.LocalLatest.OwnedIDs.Contains(playerID))
            {
                return;
            }

            if (!_playerMainTileIndexPositions.TryGetValue(playerID, out var mainTileIndexPosition))
            {
                return;
            }

            var hexGrid = GameManager.Instance.HexGrid;
            if (hexGrid == null)
            {
                return;
            }

            var mainTilePosition = hexGrid.ConvertToPosition(mainTileIndexPosition);
            DoFocus(mainTilePosition, focusDuration, focusEase);
        }

        #endregion

        #region Tweeners

        private Tweener _focusTweener;

        private Vector2 _focusStartPosition;
        private Vector2 _focusEndPosition;

        private Tweener DoFocus(Vector2 focusEndPosition, float duration, Ease ease = Ease.InOutSine)
        {
            _focusStartPosition = cameraMovement.Position;
            _focusEndPosition = cameraMovement.ClampPosition(focusEndPosition);

            if (_focusTweener.IsActive())
            {
                _focusTweener.ChangeValues(0f, 1f, duration)
                    .SetEase(ease)
                    .Restart();
            }
            else
            {
                _focusTweener = DOTween.To(FocusSetter, 0f, 1f, duration)
                    .SetEase(ease)
                    .SetUpdate(true)
                    .OnStart(OnFocusStart)
                    .OnComplete(OnFocusComplete)
                    .SetLink(cameraMovement.gameObject)
                    .SetAutoKill(false);
            }

            return _focusTweener;
        }

        private void OnFocusStart()
        {
            cameraMovement.InputEnabled = false;
        }

        private void OnFocusComplete()
        {
            cameraMovement.InputEnabled = true;
        }

        private void FocusSetter(float t)
        {
            cameraMovement.Position = Vector2.LerpUnclamped(_focusStartPosition, _focusEndPosition, t);
            cameraMovement.UpdateMovement();
        }

        #endregion
    }
}