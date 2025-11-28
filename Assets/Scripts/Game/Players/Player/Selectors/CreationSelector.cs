using Board.Structs;
using Core.Extensions;
using Game.Logic.Common.Enums;
using Game.Players.Player.Previews;
using MathModule.Structs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Players.Player.Selectors
{
    public class CreationSelector : PlayerExtension
    {
        #region Inspector

        [SerializeField] private CreationPreview creationPreview;
        [SerializeField] private BonusesPreview bonusesPreview;

        [Space] [SerializeField] private float clickDeviation = 5;

        #endregion

        public bool IsPreviewEnabled
        {
            get => creationPreview.IsEnabled && bonusesPreview.IsEnabled;
            set
            {
                creationPreview.IsEnabled = value;
                bonusesPreview.IsEnabled = value;
            }
        }

        private bool IsPreviewVisible
        {
            get => creationPreview.IsVisible && bonusesPreview.IsVisible;
            set
            {
                creationPreview.IsVisible = value;
                bonusesPreview.IsVisible = value;
            }
        }

        private void Awake()
        {
            creationPreview.Initialize(ContextBehaviour);
            bonusesPreview.Initialize(ContextBehaviour);

            IsPreviewVisible = false;
        }

        private void OnEnable()
        {
            UpdatePreviews();

            ContextBehaviour.Selection.OnSelectedIndexPositionChanged += OnSelectedIndexPositionChanged;
            ContextBehaviour.Selection.OnSelectedCardChanged += OnSelectedCardChanged;

            GameEvents.Instance.OnCardApplied += OnCardApplied;
        }

        private void OnDisable()
        {
            IsPreviewVisible = false;

            ContextBehaviour.Selection.OnSelectedIndexPositionChanged -= OnSelectedIndexPositionChanged;
            ContextBehaviour.Selection.OnSelectedCardChanged -= OnSelectedCardChanged;

            GameEvents.Instance.OnCardApplied -= OnCardApplied;
        }

        private void OnSelectedIndexPositionChanged(Int2 oldIndexPosition, Int2 newIndexPosition)
        {
            UpdatePreviews();
        }

        private void OnSelectedCardChanged(CardInfo oldCardInfo, CardInfo newCardInfo)
        {
            UpdatePreviews();
        }

        private void OnCardApplied(HandInfo handInfo, CardInfo cardInfo, Int2 indexPosition, PlayerErrorType errorType)
        {
            if (handInfo.ID != ContextBehaviour.LatestID || cardInfo.ID != ContextBehaviour.Selection.CardID)
            {
                return;
            }

            var cards = ContextBehaviour.GetCards();
            if (cards != null && cards.Contains(cardInfo))
            {
                return;
            }

            ContextBehaviour.Selection.CardID = null;
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

                ContextBehaviour.ApplySelectedCard();
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

                ContextBehaviour.Selection.CardID = null;
            }
        }

        private void UpdatePreviews()
        {
            if (ContextBehaviour.Selection.CardID.IsNullOrEmpty())
            {
                IsPreviewVisible = false;
            }
            else
            {
                IsPreviewVisible = true;

                var selectedIndexPosition = ContextBehaviour.Selection.IndexPosition;
                var selectedCardInfo = ContextBehaviour.Selection.GetCardInfo();

                creationPreview.Setup(selectedIndexPosition, selectedCardInfo.Type);
                bonusesPreview.Setup(selectedIndexPosition, selectedCardInfo.Type);
            }
        }
    }
}