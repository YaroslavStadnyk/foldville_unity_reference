using System.Linq;
using Core.UI;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;
using UnityEngine;

namespace Game.UI.Components.Widgets
{
    [RequireComponent(typeof(LayoutGroup))]
    public class PlayersWidget : PlayersUI
    {
        protected override void Awake()
        {
            base.Awake();

            if (GameManager.Instance.Party == null)
            {
                return;
            }

            var shownPlayerIDs = GameManager.Instance.Party.JoinedPlayers?.Keys;
            if (shownPlayerIDs == null)
            {
                return;
            }

            ShowListItems(shownPlayerIDs.ToArray());
        }

        private void OnEnable()
        {
            GameEvents.Instance.OnPartyPlayerJoined += OnPartyPlayerJoined;
            GameEvents.Instance.OnPartyPlayerLeaved += OnPartyPlayerLeaved;
            GameEvents.Instance.OnTurnChanged += OnTurnChanged;
            GameEvents.Instance.OnPartyPlayerStateChanged += OnPartyPlayerStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.Instance.OnPartyPlayerJoined -= OnPartyPlayerJoined;
            GameEvents.Instance.OnPartyPlayerLeaved -= OnPartyPlayerLeaved;
            GameEvents.Instance.OnTurnChanged -= OnTurnChanged;
            GameEvents.Instance.OnPartyPlayerStateChanged += OnPartyPlayerStateChanged;
        }

        private void OnPartyPlayerJoined(string playerID)
        {
            ShowListItems(playerID);
        }

        private void OnPartyPlayerLeaved(string playerID)
        {
            HideListItems(playerID);
        }

        private void OnTurnChanged(Turn oldTurn, Turn newTurn)
        {
            SelectListItem(newTurn.playerID);
        }

        private void OnPartyPlayerStateChanged(string playerID, PartyPlayerState oldPlayerState, PartyPlayerState newPlayerState)
        {
            if (newPlayerState is PartyPlayerState.Lost)
            {
                SetListItemLostState(playerID);
            }
        }
    }
}
