using Board.Structs;
using Core.Extensions;
using Game.Players.Common;
using Game.Players.Player.Selectors;
using Grid.Hexagonal;
using MathModule.Structs;
using UnityEngine;

namespace Game.Players.Player
{
    // TODO update player control and selectors approach (it's confusing and not the best practice right now)
    public class PlayerBehaviour : EntityBehaviour
    {
        public static PlayerBehaviour LocalLatest { get; private set; }

        #region Inspector

        [SerializeField] private PositionSelector positionSelector;
        [SerializeField] private HexTileSelector hexTileSelector;
        [SerializeField] private CreationSelector creationSelector;
        [SerializeField] private AttackSelector attackSelector;

        #endregion

        protected override void Awake()
        {
            base.Awake();

            positionSelector.Initialize(this);
            hexTileSelector.Initialize(this);
            creationSelector.Initialize(this);
            attackSelector.Initialize(this);

            positionSelector.IsEnabled = false;
            hexTileSelector.IsEnabled = false;
            creationSelector.IsEnabled = false;
            attackSelector.IsEnabled = false;
        }

        public override void OnStartLocalPlayer()
        {
            LocalLatest = this;

            positionSelector.IsEnabled = true;
            hexTileSelector.IsEnabled = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            Selection.OnSelectedCardChanged += OnSelectedCardChanged;
            Selection.OnSelectedHexTileChanged += OnSelectedHexTileChanged;
            Selection.OnSelectedIndexPositionChanged += OnSelectedIndexPositionChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Selection.OnSelectedCardChanged -= OnSelectedCardChanged;
            Selection.OnSelectedHexTileChanged -= OnSelectedHexTileChanged;
            Selection.OnSelectedIndexPositionChanged -= OnSelectedIndexPositionChanged;
        }

        private bool IsIndexPositionAvailable
        {
            get
            {
                var hexGrid = GameManager.Instance.HexGrid;
                if (hexGrid == null)
                {
                    return false;
                }

                var indexPosition = Selection.IndexPosition;
                var hexTile = hexGrid.GetTile(indexPosition);
                if (hexTile == null)
                {
                    return false;
                }

                return true;
            }
        }

        private void OnSelectedCardChanged(CardInfo oldCardInfo, CardInfo newCardInfo)
        {
            if (newCardInfo == default)
            {
                positionSelector.IsPreviewEnabled = true;
                hexTileSelector.IsEnabled = true;
                creationSelector.IsEnabled = false;
                attackSelector.IsEnabled = false;
            }
            else
            {
                positionSelector.IsPreviewEnabled = false;
                hexTileSelector.IsEnabled = false;
                creationSelector.IsEnabled = true;
                attackSelector.IsEnabled = false;
            }
        }

        private void OnSelectedHexTileChanged(HexTile oldHexTile, HexTile newHexTile)
        {
            if (newHexTile == null)
            {
                attackSelector.IsEnabled = false;
            }
            else
            {
                attackSelector.IsEnabled = true;
                attackSelector.SetHexTileToApply(newHexTile);
            }
        }

        private void OnSelectedIndexPositionChanged(Int2 oldIndexPosition, Int2 newIndexPosition)
        {
            var isCardAvailable = !Selection.CardID.IsNullOrEmpty();
            positionSelector.IsPreviewEnabled = !isCardAvailable && IsIndexPositionAvailable;
            creationSelector.IsPreviewEnabled = isCardAvailable && IsIndexPositionAvailable;
        }
    }
}