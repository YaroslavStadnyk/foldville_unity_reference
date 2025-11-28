using System.Collections;
using System.Linq;
using Core.Extensions;
using Game.Logic.Common.Enums;
using Game.Players.Player;
using UnityEngine;

namespace Game.UI.Components.Widgets
{
    public class TimerWidget : TimerUI
    {
        #region Labels

        private const string GameStartingLabel = "Game starts in:";
        private const string YourTurnLabel = "Your turn:";
        private const string OpponentTurnLabel = "Opponent's turn:";
        private static string GetPlayerTurnLabel(string playerID) => $"{PlayerProfile.GetNickname(playerID)}'s turn:";

        #endregion

        private void OnEnable()
        {
            GameEvents.Instance.OnPartyStateChanged += OnPartyStateChanged;
            GameEvents.Instance.OnTurnSecondsChanged += OnTurnSecondsChanged;
        }

        private void OnDisable()
        {
            GameEvents.Instance.OnPartyStateChanged -= OnPartyStateChanged;
            GameEvents.Instance.OnTurnSecondsChanged -= OnTurnSecondsChanged;
        }

        private void OnPartyStateChanged(PartyState oldPartyState, PartyState newPartyState)
        {
            if (newPartyState is PartyState.Starting)
            {
                StartTimer(GameStartingLabel, GameManager.Instance.Party.RemainingSeconds);
            }
            else if (newPartyState is PartyState.Waiting)
            {
                StopTimer();
            }
        }

        private void OnTurnSecondsChanged(string playerID, float remainingSeconds)
        {
            if (_updateTimerCoroutine != null)
            {
                StopCoroutine(_updateTimerCoroutine);
            }

            _updateTimerCoroutine = StartCoroutine(UpdateTimer(playerID, remainingSeconds));
        }

        #region Coroutines

        private Coroutine _updateTimerCoroutine;

        private IEnumerator UpdateTimer(string playerID, float remainingSeconds)
        {
            if (playerID.IsNullOrEmpty())
            {
                StopTimer();
                yield break;
            }

            while (PlayerProfile.LocalLatest == null)
            {
                yield return new WaitForEndOfFrame();
            }

            if (PlayerProfile.LocalLatest.OwnedIDs.Contains(playerID))
            {
                if (PlayerProfile.LocalLatest.OwnedIDs.Count > 1)
                {
                    var playerTurnLabel = GetPlayerTurnLabel(playerID);
                    StartTimer(playerTurnLabel, remainingSeconds);
                }
                else
                {
                    StartTimer(YourTurnLabel, remainingSeconds);
                }
            }
            else
            {
                StartTimer(OpponentTurnLabel, remainingSeconds);
            }
        }

        #endregion
    }
}