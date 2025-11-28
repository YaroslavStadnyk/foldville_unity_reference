using System.Collections;
using System.Linq;
using Core.Extensions;
using Game.Logic.Common.Structs;
using Game.Players.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameUIController : MonoBehaviour
    {
        [SerializeField] private Button leaveGameButton;
        [SerializeField] private Button passTurnButton;

        private void OnEnable()
        {
            GameEvents.Instance.OnTurnChanged += OnTurnChanged;

            leaveGameButton.onClick.AddListener(OnLeaveGameButtonClicked);
            passTurnButton.onClick.AddListener(OnPassTurnButtonClicked);

            passTurnButton.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            GameEvents.Instance.OnTurnChanged -= OnTurnChanged;

            leaveGameButton.onClick.RemoveListener(OnLeaveGameButtonClicked);
            passTurnButton.onClick.RemoveListener(OnPassTurnButtonClicked);

            passTurnButton.gameObject.SetActive(false);
        }

        private void OnTurnChanged(Turn oldTurn, Turn newTurn)
        {
            RestartUpdateButtonsCoroutine(newTurn.playerID);
        }

        public void OnLeaveGameButtonClicked()
        {
            GameManager.Instance.LeaveGame();
        }

        public void OnPassTurnButtonClicked()
        {
            if (PlayerBehaviour.LocalLatest != null)
            {
                PlayerBehaviour.LocalLatest.PassTurn();
            }
        }

        #region Coroutines

        private Coroutine _updateButtonsCoroutine;

        private IEnumerator UpdateButtons(string playerID)
        {
            if (playerID.IsNullOrEmpty())
            {
                yield break;
            }

            while (PlayerProfile.LocalLatest == null)
            {
                yield return new WaitForEndOfFrame();
            }

            passTurnButton.gameObject.SetActive(PlayerProfile.LocalLatest.OwnedIDs.Contains(playerID));
        }

        private void RestartUpdateButtonsCoroutine(string playerID)
        {
            if (_updateButtonsCoroutine != null)
            {
                StopCoroutine(_updateButtonsCoroutine);
            }

            _updateButtonsCoroutine = StartCoroutine(UpdateButtons(playerID));
        }

        #endregion
    }
}