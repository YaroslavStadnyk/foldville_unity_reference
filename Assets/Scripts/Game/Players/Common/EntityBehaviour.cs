using System.Collections.Generic;
using System.Linq;
using Board.Structs;
using Core.Extensions;
using Game.Logic.Common.Structs;
using Game.Players.Common.Extensions;
using Grid.Common;
using MathModule.Structs;
using Mirror;
using Sirenix.OdinInspector;
using Resources = Game.Players.Common.Extensions.Resources;
using ShowInInspector = Sirenix.OdinInspector.ShowInInspectorAttribute;

namespace Game.Players.Common
{
    public abstract class EntityBehaviour : NetworkBehaviour
    {
        public EntityProfile Profile { get; private set; }
        [BoxGroup("Debug")] [ShowInInspector] [ReadOnly] [HideInEditorMode] public string LatestID { get; private set; }

        public void OnInitialize(EntityProfile entityProfile)
        {
            Profile = entityProfile;
            LatestID = entityProfile.ID;
        }

        public readonly Selection Selection = new();
        public readonly Resources Resources = new();

        protected virtual void Awake()
        {
            Selection.Initialize(this);
            Resources.Initialize(this);
        }

        public List<CardInfo> GetCards()
        {
            var board = GameManager.Instance.Board;
            if (board == null)
            {
                return null;
            }

            var hands = board.Hands;
            if (hands == null)
            {
                return null;
            }

            return hands.FirstOrDefault(LatestID).Cards;
        }

        public List<TileType> GetExplorations()
        {
            var factions = GameManager.Instance.Factions;
            if (factions == null)
            {
                return null;
            }

            var explorationKeys = factions.Explorations?.Keys;
            if (explorationKeys == null)
            {
                return null;
            }

            var explorations = new List<TileType>();
            foreach (var explorationKey in explorationKeys)
            {
                if (explorationKey.playerID == LatestID)
                {
                    explorations.Add(explorationKey.type);
                }
            }

            return explorations;
        }

        public Dictionary<Int2, TileType> GetBuildings()
        {
            var hexGrid = GameManager.Instance.HexGrid;
            if (hexGrid == null)
            {
                return null;
            }

            var buildings = new Dictionary<Int2, TileType>();
            var tileCaptures = hexGrid.GetTileCaptures();
            var tileTypes = hexGrid.GetTileTypes();
            foreach (var (indexPosition, captureID) in tileCaptures)
            {
                if (captureID == LatestID && tileTypes.TryGetValue(indexPosition, out var type))
                {
                    buildings[indexPosition] = type;
                }
            }

            return buildings;
        }

        protected virtual void OnEnable()
        {
            GameEvents.Instance.OnTurnChanged += OnTurnChanged;
        }

        protected virtual void OnDisable()
        {
            GameEvents.Instance.OnTurnChanged -= OnTurnChanged;
        }

        private void OnTurnChanged(Turn oldTurn, Turn newTurn)
        {
            if (Profile != null && Profile.OwnedIDs.Contains(newTurn.playerID))
            {
                LatestID = newTurn.playerID;
            }
        }

        public void PassTurn()
        {
            CmdPassTurn();
        }

        [Command]
        private void CmdPassTurn()
        {
            if (GameManager.Instance.Turn.CurrentTurn.playerID == LatestID)
            {
                GameManager.Instance.Turn.PassTurn();
            }
        }

        public void ApplySelectedCard()
        {
            if (Selection.CardID.IsNullOrEmpty())
            {
                return;
            }

            CmdApplyCard(Selection.CardID, Selection.IndexPosition);
        }

        [Command]
        private void CmdApplyCard(string cardID, Int2 indexPosition)
        {
            GameManager.Instance.Turn.ApplyCard(cardID, indexPosition);
        }

        public void ApplySelectedDesk()
        {
            if (Selection.DeskID.IsNullOrEmpty())
            {
                return;
            }

            CmdApplyDesk(Selection.DeskID);
        }

        [Command]
        private void CmdApplyDesk(string deskID)
        {
            GameManager.Instance.Turn.ApplyDesk(deskID);
        }

        public void ApplySelectedBuildingAttack()
        {
            CmdApplyBuildingAttack(Selection.AttackCoords);
        }

        [Command]
        private void CmdApplyBuildingAttack(AttackCoords attackCoords)
        {
            GameManager.Instance.Turn.ApplyBuildingAttack(attackCoords);
        }

        public void ExploreSelectedFaction()
        {
            CmdExploreFaction(Selection.FactionType);
        }

        [Command]
        private void CmdExploreFaction(TileType type)
        {
            GameManager.Instance.Factions.Explore(LatestID, type);
        }
    }
}