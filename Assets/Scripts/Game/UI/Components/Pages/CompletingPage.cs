using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Core.UI;
using Game.Logic.Common.Enums;
using Game.Logic.Common.Structs;
using Game.Players.Player;
using Game.UI.Components.Holders;
using UnityEngine;

namespace Game.UI.Components.Pages
{
    public class CompletingPage : AnimatedPage
    {
        [SerializeField] private PlayerHolder playerHolder;

        private void OnEnable()
        {
            GameEvents.Instance.OnPartyStateChanged += OnPartyStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.Instance.OnPartyStateChanged -= OnPartyStateChanged;
        }

        private void OnPartyStateChanged(PartyState oldPartyState, PartyState newPartyState)
        {
            if (newPartyState != PartyState.Completing)
            {
                Hide();
                return;
            }

            #region Safe

            var localPlayer = PlayerProfile.LocalLatest;
            if (localPlayer == null)
            {
                Debug.LogError($"{GetType()}: {name}: {nameof(PlayerProfile.LocalLatest)} is null.");
                return;
            }

            var party = GameManager.Instance.Party;
            if (party == null)
            {
                Debug.LogError($"{GetType()}: {name}: {nameof(GameManager.Instance.Party)} is null.");
                return;
            }

            if (party.JoinedPlayers.IsNullOrEmpty())
            {
                Debug.LogError($"{GetType()}: {name}: {nameof(party.JoinedPlayers)} is null or empty.");
                return;
            }

            #endregion

            var winingPlayer = party.JoinedPlayers.FirstOrDefault(IsWinner);
            if (winingPlayer.Key.IsNullOrEmpty())
            {
                var neutralPlayer = party.JoinedPlayers.FirstOrDefault(IsDefault);
                if (neutralPlayer.Key.IsNullOrEmpty())
                {
                    var losingPlayer = party.JoinedPlayers.First();
                    playerHolder.Initialize(losingPlayer.Key, losingPlayer.Value);
                }
                else
                {
                    playerHolder.Initialize(neutralPlayer.Key, neutralPlayer.Value);
                }
            }
            else
            {
                playerHolder.Initialize(winingPlayer.Key, winingPlayer.Value);
            }

            Show();
        }

        private static bool IsWinner(KeyValuePair<string, PartyPlayerStats> joinedPlayer)
        {
            return joinedPlayer.Value.state is PartyPlayerState.Won;
        }

        private static bool IsDefault(KeyValuePair<string, PartyPlayerStats> joinedPlayer)
        {
            return joinedPlayer.Value.state is PartyPlayerState.Default;
        }
    }
}