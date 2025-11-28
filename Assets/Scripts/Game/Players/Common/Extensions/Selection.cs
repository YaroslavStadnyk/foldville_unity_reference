using System;
using Board;
using Board.Structs;
using Core.Extensions;
using Game.Logic.Common.Structs;
using Grid.Common;
using Grid.Hexagonal;
using MathModule.Structs;
using UnityEngine;

namespace Game.Players.Common.Extensions
{
    [Serializable]
    public class Selection : EntityExtension
    {
        public delegate void SelectedCardChanged(CardInfo oldCardInfo, CardInfo newCardInfo);
        public delegate void SelectedDeskChanged(DeskInfo oldDeskInfo, DeskInfo newDeskInfo);
        public delegate void SelectedIndexPositionChanged(Int2 oldIndexPosition, Int2 newIndexPosition);
        public delegate void SelectedAttackCoordsChanged(AttackCoords oldAttackCoords, AttackCoords newAttackCoords);
        public delegate void SelectedHexTileChanged(HexTile oldHexTile, HexTile newHexTile);
        public delegate void SelectedExplorationTypeChanged(TileType oldType, TileType newType);

        public event SelectedCardChanged OnSelectedCardChanged;
        public event SelectedDeskChanged OnSelectedDeskChanged;
        public event SelectedIndexPositionChanged OnSelectedIndexPositionChanged;
        public event SelectedAttackCoordsChanged OnSelectedAttackCoordsChanged;
        public event SelectedHexTileChanged OnSelectedHexTileChanged;
        public event SelectedExplorationTypeChanged OnSelectedFactionChanged;

        #region Card

        private string _cardID;

        public string CardID
        {
            get => _cardID;
            set
            {
                var oldCardID = _cardID;
                if (oldCardID == value)
                {
                    return;
                }

                _cardID = value;

                OnSelectedCardChanged?.Invoke(GetCardInfo(oldCardID), GetCardInfo(value));
            }
        }

        public CardInfo GetCardInfo()
        {
            return GetCardInfo(_cardID);
        }

        private CardInfo GetCardInfo(string cardID)
        {
            if (cardID.IsNullOrEmpty())
            {
                return default;
            }

            if (!GameManager.Instance.Board.Hands.TryGetValue(ContextBehaviour.LatestID, out var handInfo))
            {
                Debug.LogWarning($"{typeof(HandInfo)} not found by id {ContextBehaviour.LatestID}.");
                return default;
            }

            if (handInfo.Cards == null)
            {
                Debug.LogWarning($"{typeof(HandInfo)} cards is null.");
                return default;
            }

            return handInfo.Cards.FirstOrDefault(cardID);
        }

        #endregion

        #region Desk

        private string _deskID;

        public string DeskID
        {
            get => _deskID;
            set
            {
                var oldDeskID = _deskID;
                if (oldDeskID == value)
                {
                    return;
                }

                _deskID = value;

                OnSelectedDeskChanged?.Invoke(GetDeskInfo(oldDeskID), GetDeskInfo(value));
            }
        }

        public DeskInfo GetDeskInfo()
        {
            return GetDeskInfo(_deskID);
        }

        private DeskInfo GetDeskInfo(string deskID)
        {
            if (deskID.IsNullOrEmpty())
            {
                return default;
            }

            return GameManager.Instance.Board.Desks.FirstOrDefault(deskID);
        }

        #endregion

        #region IndexPosition

        private Int2 _indexPosition;

        public Int2 IndexPosition
        {
            get => _indexPosition;
            set
            {
                var oldIndexPosition = _indexPosition;
                if (oldIndexPosition == value)
                {
                    return;
                }

                _indexPosition = value;

                OnSelectedIndexPositionChanged?.Invoke(oldIndexPosition, _indexPosition);
            }
        }

        #endregion

        #region HexTile

        private HexTile _hexTile;

        public HexTile HexTile
        {
            get => _hexTile;
            set
            {
                var oldHexTile = _hexTile;
                if (oldHexTile == value)
                {
                    return;
                }

                _hexTile = value;

                OnSelectedHexTileChanged?.Invoke(oldHexTile, _hexTile);
            }
        }

        #endregion

        #region AttackCoords

        private AttackCoords _attackCoords;

        public AttackCoords AttackCoords
        {
            get => _attackCoords;
            set
            {
                var oldAttackCoords = _attackCoords;
                _attackCoords = value;

                OnSelectedAttackCoordsChanged?.Invoke(oldAttackCoords, _attackCoords);
            }
        }

        #endregion

        #region Faction

        private TileType _factionType;

        public TileType FactionType
        {
            get => _factionType;
            set
            {
                var oldType = _factionType;
                if (oldType == value)
                {
                    return;
                }

                _factionType = value;

                OnSelectedFactionChanged?.Invoke(oldType, value);
            }
        }

        #endregion
    }
}